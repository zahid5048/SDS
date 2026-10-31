(function () {
    function parseInitial(root) {
        const hidden = root.querySelector('#Sec01Subsections');
        const raw = (hidden && hidden.value && hidden.value.trim())
            ? hidden.value.trim()
            : (root.dataset.initial || '[]');
        try {
            const data = JSON.parse(raw);
            return Array.isArray(data) ? data : [];
        } catch {
            return [];
        }
    }

    function collectFields(card) {
        const fields = [];
        card.querySelectorAll('.sec01-field-row').forEach(row => {
            const label = (row.querySelector('.sec01-field-label')?.value || '').trim();
            const description = (row.querySelector('.sec01-field-desc')?.value || '').trim();
            if (!label && !description) return;
            fields.push({ label, description });
        });
        return fields;
    }

    function syncHidden(root) {
        const hidden = root.querySelector('#Sec01Subsections');
        const syncPid = document.getElementById('Sec01ProductIdentifierSync');
        if (!hidden) return;
        const items = [];
        root.querySelectorAll('.sec01-subsection-card').forEach(card => {
            const heading = (card.querySelector('.sec01-heading')?.value || '').trim();
            const useFields = card.querySelector('.sec01-use-fields')?.checked;
            const description = (card.querySelector('.sec01-description')?.value || '').trim();
            const fields = useFields ? collectFields(card) : [];
            if (!heading && !description && fields.length === 0) return;
            const item = { heading, description: useFields ? '' : description };
            if (useFields && fields.length > 0) item.fields = fields;
            items.push(item);
        });
        hidden.value = JSON.stringify(items);
        if (syncPid) {
            const row11 = items.find(r => r.heading.trimStart().toLowerCase().startsWith('1.1'));
            if (row11 && row11.description) syncPid.value = row11.description.trim();
        }
    }

    function addFieldRow(container, root, label, description) {
        const row = document.createElement('div');
        row.className = 'sec01-field-row border rounded p-2 mb-2 bg-white';
        row.innerHTML = `
            <div class="d-flex justify-content-between align-items-center mb-1">
                <span class="small text-muted fw-semibold">Sub-heading</span>
                <button type="button" class="btn btn-sm btn-outline-danger sec01-remove-field" title="Remove sub-heading"><i class="fas fa-times"></i></button>
            </div>
            <input type="text" class="form-control form-control-sm sec01-field-label mb-2" placeholder="e.g. Manufacturer:" />
            <label class="form-label small text-muted mb-0">Description</label>
            <textarea class="form-control form-control-sm sec01-field-desc" rows="2" placeholder="Text for this sub-heading"></textarea>`;
        row.querySelector('.sec01-field-label').value = label || '';
        row.querySelector('.sec01-field-desc').value = description || '';
        row.querySelectorAll('input, textarea').forEach(el => el.addEventListener('input', () => syncHidden(root)));
        row.querySelector('.sec01-remove-field')?.addEventListener('click', () => {
            row.remove();
            syncHidden(root);
        });
        container.appendChild(row);
    }

    function setFieldsMode(card, root, enabled) {
        const descWrap = card.querySelector('.sec01-desc-wrap');
        const fieldsWrap = card.querySelector('.sec01-fields-wrap');
        if (descWrap) descWrap.classList.toggle('d-none', enabled);
        if (fieldsWrap) fieldsWrap.classList.toggle('d-none', !enabled);
        syncHidden(root);
    }

    function bindCard(card, root) {
        card.querySelectorAll('.sec01-heading, .sec01-description').forEach(el => {
            el.addEventListener('input', () => syncHidden(root));
        });
        const useFields = card.querySelector('.sec01-use-fields');
        if (useFields) {
            useFields.addEventListener('change', () => setFieldsMode(card, root, useFields.checked));
        }
        card.querySelector('.sec01-add-field')?.addEventListener('click', () => {
            const host = card.querySelector('.sec01-fields-host');
            addFieldRow(host, root, '', '');
            syncHidden(root);
        });
        const rm = card.querySelector('.sec01-remove');
        if (rm) rm.addEventListener('click', () => {
            card.remove();
            syncHidden(root);
        });
    }

    function createCard(root, item) {
        const heading = item?.heading || item?.Heading || '';
        const description = item?.description || item?.Description || '';
        const fields = item?.fields || item?.Fields || [];
        const useFields = fields.length > 0;

        const card = document.createElement('div');
        card.className = 'sec01-subsection-card border rounded p-3 bg-light';
        card.innerHTML = `
            <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
                <label class="form-label fw-semibold mb-0 small text-uppercase text-muted">Subsection heading</label>
                <button type="button" class="btn btn-sm btn-outline-danger sec01-remove" title="Remove subsection"><i class="fas fa-times"></i></button>
            </div>
            <input type="text" class="form-control sec01-heading mb-2" placeholder="e.g. 1.3 Details of the supplier of the safety data sheet" />
            <div class="form-check mb-2">
                <input class="form-check-input sec01-use-fields" type="checkbox" id="sec01Fields_${Date.now()}" />
                <label class="form-check-label small" for="sec01Fields_${Date.now()}">Use sub-headings (e.g. Manufacturer:, Australian supplier / importer:)</label>
            </div>
            <div class="sec01-desc-wrap">
                <label class="form-label small text-muted">Description</label>
                <textarea class="form-control sec01-description" rows="3" placeholder="Text for this subsection"></textarea>
            </div>
            <div class="sec01-fields-wrap d-none">
                <div class="sec01-fields-host"></div>
                <button type="button" class="btn btn-sm btn-outline-secondary sec01-add-field"><i class="fas fa-plus"></i> Add sub-heading</button>
            </div>`;

        const check = card.querySelector('.sec01-use-fields');
        const checkId = 'sec01Fields_' + Math.random().toString(36).slice(2);
        check.id = checkId;
        card.querySelector('label[for^="sec01Fields_"]').setAttribute('for', checkId);

        card.querySelector('.sec01-heading').value = heading;
        card.querySelector('.sec01-description').value = description;
        check.checked = useFields;

        const host = card.querySelector('.sec01-fields-host');
        if (useFields) {
            fields.forEach(f => addFieldRow(host, root, f.label || f.Label || '', f.description || f.Description || ''));
        }

        root.querySelector('#sec01SubsectionRows').appendChild(card);
        bindCard(card, root);
        setFieldsMode(card, root, useFields);
    }

    function defaultItems() {
        return [
            { heading: '1.1 Product identifier', description: '' },
            { heading: '1.2 Relevant identified uses and uses advised against', description: '' },
            {
                heading: '1.3 Details of the supplier of the safety data sheet',
                fields: [
                    { label: 'Manufacturer:', description: '' },
                    { label: 'Australian supplier / importer:', description: '' }
                ]
            },
            { heading: '1.4 Emergency telephone number', description: '' }
        ];
    }

    function init(root) {
        const rows = parseInitial(root);
        const list = root.querySelector('#sec01SubsectionRows');
        if (!list) return;
        list.innerHTML = '';
        const data = rows.length ? rows : defaultItems();
        data.forEach(r => createCard(root, r));
        syncHidden(root);
        root.querySelector('.sec01-add-subsection')?.addEventListener('click', () => {
            createCard(root, { heading: '', description: '' });
            syncHidden(root);
        });
        root.closest('form')?.addEventListener('submit', () => syncHidden(root));
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec01-subsections-editor').forEach(init);
    });
})();
