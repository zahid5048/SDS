(function () {
    function parseInitial(root) {
        const hidden = root.querySelector('#Sec05Subsections');
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
        card.querySelectorAll('.sec05-field-row').forEach(row => {
            const label = (row.querySelector('.sec05-field-label')?.value || '').trim();
            const description = (row.querySelector('.sec05-field-desc')?.value || '').trim();
            if (!label && !description) return;
            fields.push({ label, description });
        });
        return fields;
    }

    function syncHidden(root) {
        const hidden = root.querySelector('#Sec05Subsections');
        if (!hidden) return;
        const items = [];
        root.querySelectorAll('.sec05-subsection-card').forEach(card => {
            const heading = (card.querySelector('.sec05-heading')?.value || '').trim();
            const useFields = card.querySelector('.sec05-use-fields')?.checked;
            const description = (card.querySelector('.sec05-description')?.value || '').trim();
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
        row.className = 'sec05-field-row border rounded p-2 mb-2 bg-white';
        row.innerHTML = `
            <div class="d-flex justify-content-end mb-1">
                <button type="button" class="btn btn-sm btn-outline-danger sec05-remove-field" title="Remove"><i class="fas fa-times"></i></button>
            </div>
            <input type="text" class="form-control form-control-sm sec05-field-label mb-2" placeholder="e.g. Suitable extinguishing agents:" />
            <textarea class="form-control form-control-sm sec05-field-desc" rows="2" placeholder="Text for this label"></textarea>`;
        row.querySelector('.sec05-field-label').value = label || '';
        row.querySelector('.sec05-field-desc').value = description || '';
        row.querySelectorAll('input, textarea').forEach(el => el.addEventListener('input', () => syncHidden(root)));
        row.querySelector('.sec05-remove-field')?.addEventListener('click', () => {
            row.remove();
            syncHidden(root);
        });
        container.appendChild(row);
    }

    function setFieldsMode(card, root, enabled) {
        const descWrap = card.querySelector('.sec05-desc-wrap');
        const fieldsWrap = card.querySelector('.sec05-fields-wrap');
        if (descWrap) descWrap.classList.toggle('d-none', enabled);
        if (fieldsWrap) fieldsWrap.classList.toggle('d-none', !enabled);
        syncHidden(root);
    }

    function bindCard(card, root) {
        card.querySelectorAll('.sec05-heading, .sec05-description').forEach(el => {
            el.addEventListener('input', () => syncHidden(root));
        });
        const useFields = card.querySelector('.sec05-use-fields');
        if (useFields) {
            useFields.addEventListener('change', () => setFieldsMode(card, root, useFields.checked));
        }
        card.querySelector('.sec05-add-field')?.addEventListener('click', () => {
            addFieldRow(card.querySelector('.sec05-fields-host'), root, '', '');
            syncHidden(root);
        });
        card.querySelector('.sec05-remove')?.addEventListener('click', () => {
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
        card.className = 'sec05-subsection-card border rounded p-3 bg-light';
        card.innerHTML = `
            <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
                <label class="form-label fw-semibold mb-0 small text-uppercase text-muted">Subsection heading</label>
                <button type="button" class="btn btn-sm btn-outline-danger sec05-remove" title="Remove subsection"><i class="fas fa-times"></i></button>
            </div>
            <input type="text" class="form-control sec05-heading mb-2" placeholder="e.g. 5.1 Extinguishing media" />
            <div class="form-check mb-2">
                <input class="form-check-input sec05-use-fields" type="checkbox" />
                <label class="form-check-label small">Use labeled fields (Suitable / Unsuitable extinguishing agents, …)</label>
            </div>
            <div class="sec05-desc-wrap">
                <label class="form-label small text-muted">Description</label>
                <textarea class="form-control sec05-description" rows="3" placeholder="Text for this subsection"></textarea>
            </div>
            <div class="sec05-fields-wrap d-none">
                <div class="sec05-fields-host"></div>
                <button type="button" class="btn btn-sm btn-outline-secondary sec05-add-field"><i class="fas fa-plus"></i> Add labeled field</button>
            </div>`;

        const check = card.querySelector('.sec05-use-fields');
        const checkId = 'sec05Fields_' + Math.random().toString(36).slice(2);
        check.id = checkId;
        card.querySelector('.form-check-label').setAttribute('for', checkId);

        card.querySelector('.sec05-heading').value = heading;
        card.querySelector('.sec05-description').value = description;
        check.checked = useFields;

        const host = card.querySelector('.sec05-fields-host');
        if (useFields) {
            fields.forEach(f => addFieldRow(host, root, f.label || f.Label || '', f.description || f.Description || ''));
        }

        root.querySelector('#sec05SubsectionRows').appendChild(card);
        bindCard(card, root);
        setFieldsMode(card, root, useFields);
    }

    function init(root) {
        const rows = parseInitial(root);
        const list = root.querySelector('#sec05SubsectionRows');
        if (!list) return;
        list.innerHTML = '';
        rows.forEach(r => createCard(root, r));
        syncHidden(root);
        root.querySelector('.sec05-add-subsection')?.addEventListener('click', () => {
            createCard(root, { heading: '', description: '' });
            syncHidden(root);
        });
        root.closest('form')?.addEventListener('submit', () => syncHidden(root));
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec05-subsections-editor').forEach(init);
    });
})();
