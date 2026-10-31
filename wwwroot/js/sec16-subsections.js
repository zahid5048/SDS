(function () {
    const ROW_TYPES = {
        '16.1': { addLabel: 'Add H-phrase', labelPh: 'H226', descPh: 'Flammable liquid and vapour.' },
        '16.2': { addLabel: 'Add classification', labelPh: 'Acute Tox.', descPh: 'Acute Toxicity' },
        '16.3': { addLabel: 'Add abbreviation', labelPh: 'ADG', descPh: 'Australian Dangerous Goods Code' }
    };

    function parseInitial(root) {
        const hidden = root.querySelector('#Sec16Subsections');
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

    function headingKey(heading) {
        const m = (heading || '').trim().match(/^(16\.\d+)/);
        return m ? m[1] : '';
    }

    function collectFields(section) {
        const fields = [];
        section.querySelectorAll('.sec16-field-row').forEach(row => {
            const label = (row.querySelector('.sec16-field-label')?.value || '').trim();
            const description = (row.querySelector('.sec16-field-desc')?.value || '').trim();
            if (!label && !description) return;
            fields.push({ label, description });
        });
        return fields;
    }

    function syncHidden(root) {
        const hidden = root.querySelector('#Sec16Subsections');
        if (!hidden) return;
        const items = [];
        root.querySelectorAll('.sec16-block').forEach(block => {
            const heading = (block.querySelector('.sec16-block-heading')?.value || '').trim();
            const key = headingKey(heading);
            if (key === '16.4') {
                const description = (block.querySelector('.sec16-block-description')?.value || '').trim();
                if (heading || description)
                    items.push({ heading, description });
                return;
            }
            if (key === '16.5') {
                const fields = [];
                block.querySelectorAll('.sec16-rev-field').forEach(row => {
                    const label = (row.querySelector('.sec16-rev-label')?.value || '').trim();
                    const description = (row.querySelector('.sec16-rev-value')?.value || '').trim();
                    if (!label && !description) return;
                    fields.push({ label, description });
                });
                if (heading || fields.length > 0)
                    items.push({ heading, fields });
                return;
            }
            const fields = collectFields(block);
            if (heading || fields.length > 0)
                items.push({ heading, fields });
        });
        hidden.value = JSON.stringify(items);
    }

    function addFieldRow(host, root, key, label, description) {
        const cfg = ROW_TYPES[key] || ROW_TYPES['16.1'];
        const row = document.createElement('div');
        row.className = 'sec16-field-row border rounded p-2 mb-2 bg-white';
        row.innerHTML = `
            <div class="d-flex justify-content-end mb-1">
                <button type="button" class="btn btn-sm btn-outline-danger sec16-remove-field" title="Remove"><i class="fas fa-times"></i></button>
            </div>
            <div class="row g-2">
                <div class="col-md-3">
                    <label class="form-label small text-muted mb-0">Code / abbrev.</label>
                    <input type="text" class="form-control form-control-sm sec16-field-label" placeholder="${cfg.labelPh}" />
                </div>
                <div class="col-md-9">
                    <label class="form-label small text-muted mb-0">Full text</label>
                    <input type="text" class="form-control form-control-sm sec16-field-desc" placeholder="${cfg.descPh}" />
                </div>
            </div>`;
        row.querySelector('.sec16-field-label').value = label || '';
        row.querySelector('.sec16-field-desc').value = description || '';
        row.querySelectorAll('input').forEach(el => el.addEventListener('input', () => syncHidden(root)));
        row.querySelector('.sec16-remove-field')?.addEventListener('click', () => {
            row.remove();
            syncHidden(root);
        });
        host.appendChild(row);
    }

    function buildBlock(root, item) {
        const heading = item?.heading || item?.Heading || '';
        const key = headingKey(heading);
        const fields = item?.fields || item?.Fields || [];
        const description = item?.description || item?.Description || '';

        const block = document.createElement('div');
        block.className = 'sec16-block border rounded p-3 mb-3 bg-light';

        if (key === '16.4') {
            block.innerHTML = `
                <input type="hidden" class="sec16-block-heading" />
                <h5 class="sds-subheading mb-2">16.4 Recommended applications</h5>
                <textarea class="form-control sec16-block-description" rows="2" placeholder="Recommended applications"></textarea>`;
            block.querySelector('.sec16-block-heading').value = heading || '16.4 Recommended applications';
            block.querySelector('.sec16-block-description').value = description;
            block.querySelector('.sec16-block-description').addEventListener('input', () => syncHidden(root));
            return block;
        }

        if (key === '16.5') {
            block.innerHTML = `
                <input type="hidden" class="sec16-block-heading" />
                <h5 class="sds-subheading mb-2">16.5 Revision</h5>
                <div class="sec16-rev-fields d-flex flex-column gap-2"></div>`;
            block.querySelector('.sec16-block-heading').value = heading || '16.5 Revision';
            const host = block.querySelector('.sec16-rev-fields');
            const revFields = fields.length > 0 ? fields : [
                { label: 'Revision date', description: '' },
                { label: 'Revision No.', description: '' },
                { label: 'Preparation note', description: 'Prepared for the Australian market.' }
            ];
            revFields.forEach(f => {
                const label = f.label || f.Label || '';
                const desc = f.description || f.Description || '';
                const row = document.createElement('div');
                row.className = 'sec16-rev-field row g-2 align-items-end';
                row.innerHTML = `
                    <div class="col-md-4">
                        <label class="form-label small text-muted mb-0">Field</label>
                        <input type="text" class="form-control form-control-sm sec16-rev-label" readonly />
                    </div>
                    <div class="col-md-8">
                        <label class="form-label small text-muted mb-0">Value</label>
                        <input type="text" class="form-control sec16-rev-value" />
                    </div>`;
                row.querySelector('.sec16-rev-label').value = label;
                row.querySelector('.sec16-rev-value').value = desc;
                row.querySelector('.sec16-rev-value').addEventListener('input', () => syncHidden(root));
                host.appendChild(row);
            });
            return block;
        }

        const cfg = ROW_TYPES[key] || ROW_TYPES['16.1'];
        block.innerHTML = `
            <input type="hidden" class="sec16-block-heading" />
            <h5 class="sds-subheading mb-2"></h5>
            <div class="sec16-rows-host"></div>
            <button type="button" class="btn btn-outline-primary btn-sm sec16-add-row"><i class="fas fa-plus"></i> ${cfg.addLabel}</button>`;
        block.querySelector('.sec16-block-heading').value = heading;
        block.querySelector('h5').textContent = heading || key;
        const host = block.querySelector('.sec16-rows-host');
        fields.forEach(f => addFieldRow(host, root, key, f.label || f.Label, f.description || f.Description));
        block.querySelector('.sec16-add-row')?.addEventListener('click', () => {
            addFieldRow(host, root, key, '', '');
            syncHidden(root);
        });
        return block;
    }

    function init(root) {
        const list = root.querySelector('#sec16Blocks');
        if (!list) return;
        let rows = parseInitial(root);
        if (rows.length === 0) rows = parseInitial(root);
        list.innerHTML = '';
        rows.forEach(item => list.appendChild(buildBlock(root, item)));
        syncHidden(root);
        root.closest('form')?.addEventListener('submit', () => syncHidden(root));
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec16-subsections-editor').forEach(init);
    });
})();
