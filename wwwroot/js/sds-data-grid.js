/**
 * Editable SDS pipe-grid tables → hidden field sync
 */
(function () {
    const COL_SEP = '|';
    const TRANSPORT_CLASS_IMAGES = {
        '1': '/images/ghs/GHS01.svg',
        '2': '/images/ghs/GHS04.svg',
        '3': '/images/transport/class-3.svg',
        '4': '/images/ghs/GHS02.svg',
        '5': '/images/ghs/GHS03.svg',
        '6': '/images/ghs/GHS06.svg',
        '7': '/images/transport/class-7.svg',
        '8': '/images/ghs/GHS05.svg',
        '9': '/images/ghs/GHS07.svg'
    };

    function serializeGrid(tbody) {
        const lines = [];
        tbody.querySelectorAll('tr').forEach(tr => {
            const cells = [];
            tr.querySelectorAll('[data-col]').forEach(el => {
                cells.push(getCellValue(el));
            });
            if (cells.some(c => c)) lines.push(cells.join(COL_SEP));
        });
        return lines.join('\n');
    }

    function getCellValue(el) {
        if (el.tagName === 'SELECT') return (el.value || '').trim();
        return (el.value || '').trim();
    }

    function syncHidden(tbody) {
        const id = tbody.closest('.sds-grid-editor')?.dataset.target;
        if (!id) return;
        const hidden = document.querySelector(id);
        if (hidden) hidden.value = serializeGrid(tbody);
    }

    function bindRow(tr, tbody) {
        tr.querySelectorAll('[data-col]').forEach(inp => {
            inp.addEventListener('input', () => syncHidden(tbody));
            inp.addEventListener('change', () => syncHidden(tbody));
        });
        const rm = tr.querySelector('.sds-grid-remove');
        if (rm) rm.addEventListener('click', () => {
            tr.remove();
            syncHidden(tbody);
        });
    }

    function addRow(tbody, colCount) {
        const tr = document.createElement('tr');
        let html = '';
        for (let i = 0; i < colCount; i++) {
            html += `<td><input type="text" class="form-control form-control-sm" data-col="${i}" /></td>`;
        }
        html += '<td class="sds-grid-actions"><button type="button" class="btn btn-sm btn-outline-danger sds-grid-remove" title="Remove row"><i class="fas fa-times"></i></button></td>';
        tr.innerHTML = html;
        tbody.appendChild(tr);
        bindRow(tr, tbody);
        syncHidden(tbody);
    }

    function initDynamicGrid(root) {
        const tbody = root.querySelector('tbody');
        const colCount = parseInt(root.dataset.cols || '4', 10);
        const addBtn = root.querySelector('.sds-grid-add');
        if (addBtn) addBtn.addEventListener('click', () => addRow(tbody, colCount));
        tbody.querySelectorAll('tr').forEach(tr => bindRow(tr, tbody));
        const form = root.closest('form');
        if (form) form.addEventListener('submit', () => syncHidden(tbody));
    }

    function serializeTransport(tbody) {
        const lines = [];
        tbody.querySelectorAll('tr').forEach(tr => {
            const field = tr.dataset.field;
            if (!field) return;
            const cells = [field];
            tr.querySelectorAll('[data-col]').forEach(el => cells.push(getCellValue(el)));
            if (cells.slice(1).some(c => c)) lines.push(cells.join(COL_SEP));
        });
        return lines.join('\n');
    }

    function syncTransportHidden(root) {
        const id = root.dataset.target;
        const hidden = id ? document.querySelector(id) : null;
        const tbody = root.querySelector('tbody');
        if (hidden && tbody) hidden.value = serializeTransport(tbody);
    }

    function updateHazardPictograms(tbody) {
        const row = tbody.querySelector('tr[data-field="Transport hazard class(es)"]');
        if (!row) return;
        row.querySelectorAll('.transport-class-cell').forEach(cell => {
            const sel = cell.querySelector('.transport-class-select');
            const img = cell.querySelector('.transport-hazard-icon');
            if (!sel || !img) return;
            const n = sel.value.trim();
            const opt = sel.selectedOptions[0];
            const src = (opt && opt.dataset.img) || TRANSPORT_CLASS_IMAGES[n];
            if (src && n) {
                img.src = src;
                img.classList.remove('d-none');
                img.alt = 'Hazard class ' + n;
            } else if (!cell.querySelector('.transport-hazard-upload')?.dataset.previewActive) {
                img.classList.add('d-none');
            }
        });
    }

    function bindTransportUploads(root) {
        root.querySelectorAll('.transport-hazard-upload').forEach(input => {
            input.addEventListener('change', () => {
                const file = input.files && input.files[0];
                const cell = input.closest('.transport-class-cell');
                const img = cell?.querySelector('.transport-hazard-icon');
                if (!file || !img) return;
                const reader = new FileReader();
                reader.onload = e => {
                    img.src = e.target.result;
                    img.classList.remove('d-none');
                    input.dataset.previewActive = '1';
                    let hint = cell.querySelector('.transport-custom-hint');
                    if (!hint) {
                        hint = document.createElement('small');
                        hint.className = 'text-success d-block transport-custom-hint';
                        cell.querySelector('.transport-class-cell-inner')?.appendChild(hint);
                    }
                    hint.textContent = 'New image — save section to upload';
                };
                reader.readAsDataURL(file);
            });
        });
    }

    function initTransportGrid(root) {
        const tbody = root.querySelector('tbody');
        const onChange = () => {
            syncTransportHidden(root);
            syncTransportQuickFields(root);
            updateHazardPictograms(tbody);
        };
        tbody.querySelectorAll('[data-col]').forEach(el => {
            el.addEventListener('input', onChange);
            el.addEventListener('change', onChange);
        });
        bindTransportUploads(root);
        const form = root.closest('form');
        if (form) form.addEventListener('submit', onChange);
        onChange();
    }

    function syncTransportQuickFields(root) {
        const rows = {};
        root.querySelectorAll('tbody tr').forEach(tr => {
            const field = tr.dataset.field;
            if (!field) return;
            const dot = tr.querySelector('[data-col="1"]');
            if (dot) rows[field] = getCellValue(dot);
        });
        const map = {
            'UN number': 'UNNumber',
            'UN proper shipping name': 'UNProperShippingName',
            'Transport hazard class(es)': 'TransportHazardClass',
            'Packing group': 'PackingGroup',
            'Environmental hazards': 'EnvironmentalHazardsTransport'
        };
        Object.keys(map).forEach(f => {
            const el = document.getElementById(map[f]);
            if (el && rows[f] !== undefined) el.value = rows[f];
        });
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sds-grid-editor[data-grid="dynamic"]').forEach(initDynamicGrid);
        document.querySelectorAll('.sds-grid-editor[data-grid="transport"]').forEach(initTransportGrid);
    });
})();
