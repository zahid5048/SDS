(function () {
    function parseInitial(root) {
        const hidden = root.querySelector('#Sec04Subsections');
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
        card.querySelectorAll('.sec04-field-row').forEach(row => {
            const label = (row.querySelector('.sec04-field-label')?.value || '').trim();
            const description = (row.querySelector('.sec04-field-desc')?.value || '').trim();
            if (!label && !description) return;
            fields.push({ label, description });
        });
        return fields;
    }

    function syncHidden(root) {
        const hidden = root.querySelector('#Sec04Subsections');
        if (!hidden) return;
        const items = [];
        root.querySelectorAll('.sec04-subsection-card').forEach(card => {
            const heading = (card.querySelector('.sec04-heading')?.value || '').trim();
            const useFields = card.querySelector('.sec04-use-fields')?.checked;
            const description = (card.querySelector('.sec04-description')?.value || '').trim();
            const fields = useFields ? collectFields(card) : [];
            if (!heading && !description && fields.length === 0) return;
            const item = { heading, description: useFields ? '' : description };
            if (useFields && fields.length > 0) item.fields = fields;
            items.push(item);
        });
        hidden.value = JSON.stringify(items);
    }

    function addFieldRow(container, root, label, description) {
        const row = document.createElement('div');
        row.className = 'sec04-field-row border rounded p-2 mb-2 bg-white';
        row.innerHTML = `
            <div class="d-flex justify-content-end mb-1">
                <button type="button" class="btn btn-sm btn-outline-danger sec04-remove-field" title="Remove"><i class="fas fa-times"></i></button>
            </div>
            <input type="text" class="form-control form-control-sm sec04-field-label mb-2" placeholder="e.g. Inhalation:" />
            <label class="form-label small text-muted mb-0">First aid text</label>
            <textarea class="form-control form-control-sm sec04-field-desc" rows="2" placeholder="Instructions for this exposure route"></textarea>`;
        row.querySelector('.sec04-field-label').value = label || '';
        row.querySelector('.sec04-field-desc').value = description || '';
        row.querySelectorAll('input, textarea').forEach(el => el.addEventListener('input', () => syncHidden(root)));
        row.querySelector('.sec04-remove-field')?.addEventListener('click', () => {
            row.remove();
            syncHidden(root);
        });
        container.appendChild(row);
    }

    function setFieldsMode(card, root, enabled) {
        const descWrap = card.querySelector('.sec04-desc-wrap');
        const fieldsWrap = card.querySelector('.sec04-fields-wrap');
        if (descWrap) descWrap.classList.toggle('d-none', enabled);
        if (fieldsWrap) fieldsWrap.classList.toggle('d-none', !enabled);
        syncHidden(root);
    }

    function bindCard(card, root) {
        card.querySelectorAll('.sec04-heading, .sec04-description').forEach(el => {
            el.addEventListener('input', () => syncHidden(root));
        });
        const useFields = card.querySelector('.sec04-use-fields');
        if (useFields) {
            useFields.addEventListener('change', () => setFieldsMode(card, root, useFields.checked));
        }
        card.querySelector('.sec04-add-field')?.addEventListener('click', () => {
            addFieldRow(card.querySelector('.sec04-fields-host'), root, '', '');
            syncHidden(root);
        });
        card.querySelector('.sec04-remove')?.addEventListener('click', () => {
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
        card.className = 'sec04-subsection-card border rounded p-3 bg-light';
        card.innerHTML = `
            <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
                <label class="form-label fw-semibold mb-0 small text-uppercase text-muted">Subsection heading</label>
                <button type="button" class="btn btn-sm btn-outline-danger sec04-remove" title="Remove subsection"><i class="fas fa-times"></i></button>
            </div>
            <input type="text" class="form-control sec04-heading mb-2" placeholder="e.g. 4.1 Description of first aid measures" />
            <div class="form-check mb-2">
                <input class="form-check-input sec04-use-fields" type="checkbox" />
                <label class="form-check-label small">Use labeled routes (General advice, Inhalation, Skin contact, …)</label>
            </div>
            <div class="sec04-desc-wrap">
                <label class="form-label small text-muted">Description</label>
                <textarea class="form-control sec04-description" rows="3" placeholder="Text for this subsection"></textarea>
            </div>
            <div class="sec04-fields-wrap d-none">
                <div class="sec04-fields-host"></div>
                <button type="button" class="btn btn-sm btn-outline-secondary sec04-add-field"><i class="fas fa-plus"></i> Add labeled route</button>
            </div>`;

        const check = card.querySelector('.sec04-use-fields');
        const checkId = 'sec04Fields_' + Math.random().toString(36).slice(2);
        check.id = checkId;
        card.querySelector('.form-check-label').setAttribute('for', checkId);

        card.querySelector('.sec04-heading').value = heading;
        card.querySelector('.sec04-description').value = description;
        check.checked = useFields;

        const host = card.querySelector('.sec04-fields-host');
        if (useFields) {
            fields.forEach(f => addFieldRow(host, root, f.label || f.Label || '', f.description || f.Description || ''));
        }

        root.querySelector('#sec04SubsectionRows').appendChild(card);
        bindCard(card, root);
        setFieldsMode(card, root, useFields);
    }

    function init(root) {
        const rows = parseInitial(root);
        const list = root.querySelector('#sec04SubsectionRows');
        if (!list) return;
        list.innerHTML = '';
        rows.forEach(r => createCard(root, r));
        syncHidden(root);
        root.querySelector('.sec04-add-subsection')?.addEventListener('click', () => {
            createCard(root, { heading: '', description: '' });
            syncHidden(root);
        });
        root.closest('form')?.addEventListener('submit', () => syncHidden(root));
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec04-subsections-editor').forEach(init);
    });
})();
