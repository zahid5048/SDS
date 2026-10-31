(function () {
    function readInitial() {
        const el = document.getElementById('Sec03BlocksInitial');
        if (!el) return [];
        try {
            const data = JSON.parse(el.textContent || '[]');
            return Array.isArray(data) ? data : [];
        } catch {
            return [];
        }
    }

    function descriptionToLines(text) {
        if (!text || !String(text).trim()) return [''];
        return String(text).split(/\r?\n/);
    }

    function collectDescLines(card) {
        const lines = [];
        card.querySelectorAll('.sec03-desc-line').forEach(inp => lines.push((inp.value || '').trimEnd()));
        return lines.join('\n').trim();
    }

    function addDescLine(host, root, value) {
        const row = document.createElement('div');
        row.className = 'sec03-desc-row d-flex gap-2 mb-2 align-items-center';
        row.innerHTML = `
            <input type="text" class="form-control form-control-sm sec03-desc-line" placeholder="One line of text" />
            <button type="button" class="btn btn-sm btn-outline-danger sec03-remove-desc-line flex-shrink-0" title="Remove line"><i class="fas fa-times"></i></button>`;
        row.querySelector('.sec03-desc-line').value = value || '';
        row.querySelector('.sec03-desc-line').addEventListener('input', () => syncHidden(root));
        row.querySelector('.sec03-remove-desc-line').addEventListener('click', () => {
            const rows = host.querySelectorAll('.sec03-desc-row');
            if (rows.length <= 1) row.querySelector('.sec03-desc-line').value = '';
            else row.remove();
            syncHidden(root);
        });
        host.appendChild(row);
    }

    function addMixtureRow(tbody, root, rowData) {
        const tr = document.createElement('tr');
        tr.className = 'sec03-mixture-row';
        tr.innerHTML = `
            <td><input type="text" class="form-control form-control-sm sec03-cas" placeholder="25265-71-8" /></td>
            <td><input type="text" class="form-control form-control-sm sec03-name" placeholder="Chemical name" /></td>
            <td><input type="text" class="form-control form-control-sm sec03-proportion" placeholder="25–50%" /></td>
            <td><input type="text" class="form-control form-control-sm sec03-classification" placeholder="Skin Sens. 1B; H317" /></td>
            <td><input type="text" class="form-control form-control-sm sec03-source" placeholder="AU HCIS" /></td>
            <td><button type="button" class="btn btn-sm btn-outline-danger sec03-remove-mix-row" title="Remove"><i class="fas fa-times"></i></button></td>`;
        const d = rowData || {};
        tr.querySelector('.sec03-cas').value = d.cas || d.Cas || '';
        tr.querySelector('.sec03-name').value = d.chemicalName || d.ChemicalName || '';
        tr.querySelector('.sec03-proportion').value = d.proportion || d.Proportion || '';
        tr.querySelector('.sec03-classification').value = d.classification || d.Classification || '';
        tr.querySelector('.sec03-source').value = d.source || d.Source || '';
        tr.querySelectorAll('input').forEach(inp => inp.addEventListener('input', () => syncHidden(root)));
        tr.querySelector('.sec03-remove-mix-row')?.addEventListener('click', () => {
            tr.remove();
            syncHidden(root);
        });
        tbody.appendChild(tr);
    }

    function syncHidden(root) {
        const hidden = root.querySelector('#Sec03Blocks');
        if (!hidden) return;
        const blocks = [];
        root.querySelectorAll('.sec03-block-card').forEach(card => {
            const type = card.dataset.blockType;
            if (type === 'text' || type === 'footnote') {
                const heading = type === 'footnote' ? '' : (card.querySelector('.sec03-heading')?.value || '').trim();
                const description = collectDescLines(card);
                if (!heading && !description) return;
                blocks.push({ type, heading, description });
            } else if (type === 'mixtureTable') {
                const rows = [];
                card.querySelectorAll('.sec03-mixture-row').forEach(tr => {
                    const cas = (tr.querySelector('.sec03-cas')?.value || '').trim();
                    const chemicalName = (tr.querySelector('.sec03-name')?.value || '').trim();
                    const proportion = (tr.querySelector('.sec03-proportion')?.value || '').trim();
                    const classification = (tr.querySelector('.sec03-classification')?.value || '').trim();
                    const source = (tr.querySelector('.sec03-source')?.value || '').trim();
                    if (cas || chemicalName || proportion || classification || source)
                        rows.push({ cas, chemicalName, proportion, classification, source });
                });
                blocks.push({ type: 'mixtureTable', rows: rows.length ? rows : [{}] });
            }
        });
        hidden.value = JSON.stringify(blocks);
    }

    function createBlock(root, block) {
        const card = document.createElement('div');
        card.className = 'sec03-block-card border rounded p-3 bg-light mb-3';
        const type = block.type || 'text';
        card.dataset.blockType = type;

        if (type === 'text' || type === 'footnote') {
            const isFootnote = type === 'footnote';
            card.innerHTML = `
                <div class="d-flex justify-content-between align-items-start mb-2">
                    <span class="badge ${isFootnote ? 'bg-secondary' : 'bg-primary'}">${isFootnote ? 'Footnote' : 'Text subsection'}</span>
                    <button type="button" class="btn btn-sm btn-outline-danger sec03-remove-block" title="Remove"><i class="fas fa-times"></i></button>
                </div>
                ${isFootnote ? '' : `<label class="form-label small text-muted">Heading</label>
                <input type="text" class="form-control sec03-heading mb-2" placeholder="e.g. 3.2 Mixtures" />`}
                <label class="form-label small text-muted">${isFootnote ? 'Footnote text (one line per input)' : 'Description (one line per input)'}</label>
                <div class="sec03-desc-lines mb-2"></div>
                <button type="button" class="btn btn-sm btn-outline-secondary sec03-add-desc-line"><i class="fas fa-plus"></i> Add more</button>`;
            if (!isFootnote) card.querySelector('.sec03-heading').value = block.heading || '';
            const host = card.querySelector('.sec03-desc-lines');
            descriptionToLines(block.description || '').forEach(line => addDescLine(host, root, line));
            card.querySelector('.sec03-add-desc-line').addEventListener('click', () => {
                addDescLine(host, root, '');
                syncHidden(root);
            });
        } else if (type === 'mixtureTable') {
            card.innerHTML = `
                <div class="d-flex justify-content-between align-items-start mb-2">
                    <span class="badge bg-info text-dark">3.2 Mixture ingredients table</span>
                    <button type="button" class="btn btn-sm btn-outline-danger sec03-remove-block" title="Remove table"><i class="fas fa-times"></i></button>
                </div>
                <div class="table-responsive">
                    <table class="table table-sm table-bordered mb-2">
                        <thead class="table-primary">
                            <tr>
                                <th>CAS No.</th>
                                <th>Chemical name</th>
                                <th>Proportion (Sch 8)</th>
                                <th>Classification (GHS)</th>
                                <th>Source</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody class="sec03-mixture-body"></tbody>
                    </table>
                </div>
                <button type="button" class="btn btn-sm btn-outline-secondary sec03-add-mix-row"><i class="fas fa-plus"></i> Add ingredient row</button>`;
            const tbody = card.querySelector('.sec03-mixture-body');
            const rows = block.rows && block.rows.length ? block.rows : [{}];
            rows.forEach(r => addMixtureRow(tbody, root, r));
            card.querySelector('.sec03-add-mix-row').addEventListener('click', () => addMixtureRow(tbody, root, {}));
        }

        card.querySelector('.sec03-remove-block')?.addEventListener('click', () => {
            card.remove();
            syncHidden(root);
        });

        root.querySelector('#sec03BlocksContainer').appendChild(card);
        syncHidden(root);
    }

    function init(root) {
        const container = root.querySelector('#sec03BlocksContainer');
        if (!container) return;
        container.innerHTML = '';
        const blocks = readInitial();
        (blocks.length ? blocks : [
            { type: 'text', heading: '3.1 Substances', description: 'Not applicable — this product is a mixture.' },
            { type: 'text', heading: '3.2 Mixtures', description: '' },
            { type: 'mixtureTable', rows: [{}] },
            { type: 'footnote', description: '' }
        ]).forEach(b => createBlock(root, b));

        root.querySelector('.sec03-add-text')?.addEventListener('click', () => createBlock(root, { type: 'text', heading: '', description: '' }));
        root.querySelector('.sec03-add-table')?.addEventListener('click', () => createBlock(root, { type: 'mixtureTable', rows: [{}] }));
        root.querySelector('.sec03-add-footnote')?.addEventListener('click', () => createBlock(root, { type: 'footnote', description: '' }));

        root.closest('form')?.addEventListener('submit', () => syncHidden(root));
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec03-blocks-editor').forEach(init);
    });
})();
