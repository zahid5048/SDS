(function () {
    const ghsCatalog = window.__sec02GhsCatalog || [];

    function readInitial() {
        const el = document.getElementById('Sec02BlocksInitial');
        if (!el) return [];
        try {
            const data = JSON.parse(el.textContent || '[]');
            return Array.isArray(data) ? data : [];
        } catch {
            return [];
        }
    }

    function syncHidden(root) {
        const hidden = root.querySelector('#Sec02Blocks');
        const ghsHidden = document.getElementById('GhsPictogramCodes');
        if (!hidden) return;
        const blocks = [];
        root.querySelectorAll('.sec02-block-card').forEach(card => {
            const type = card.dataset.blockType;
            if (type === 'text') {
                const heading = (card.querySelector('.sec02-heading')?.value || '').trim();
                const lines = [];
                card.querySelectorAll('.sec02-desc-line').forEach(inp => {
                    lines.push((inp.value || '').trimEnd());
                });
                const description = lines.join('\n').trim();
                if (!heading && !description) return;
                blocks.push({ type: 'text', heading, description });
            } else if (type === 'pictograms') {
                const codes = [];
                card.querySelectorAll('.sec02-ghs-check:checked').forEach(ch => codes.push(ch.value));
                blocks.push({
                    type: 'pictograms',
                    blockId: card.dataset.blockId || '',
                    codes: codes.join(','),
                    customImagePath: card.dataset.customImagePath || ''
                });
            } else if (type === 'table') {
                const title = (card.querySelector('.sec02-table-title')?.value || '').trim();
                const rows = [];
                card.querySelectorAll('.sec02-table-row').forEach(tr => {
                    const cas = (tr.querySelector('.sec02-cas')?.value || '').trim();
                    const chemicalName = (tr.querySelector('.sec02-chem-name')?.value || '').trim();
                    if (cas || chemicalName) rows.push({ cas, chemicalName });
                });
                if (!title && rows.length === 0) return;
                blocks.push({ type: 'table', title, rows: rows.length ? rows : [{ cas: '', chemicalName: '' }] });
            }
        });
        hidden.value = JSON.stringify(blocks);
        if (ghsHidden) {
            const allCodes = blocks.filter(b => b.type === 'pictograms').flatMap(b =>
                (b.codes || '').split(',').map(c => c.trim()).filter(Boolean));
            ghsHidden.value = [...new Set(allCodes)].join(',');
        }
    }

    function descriptionToLines(text) {
        if (!text || !String(text).trim()) return [''];
        return String(text).split(/\r?\n/);
    }

    function addDescLine(host, root, value) {
        const row = document.createElement('div');
        row.className = 'sec02-desc-row d-flex gap-2 mb-2 align-items-center';
        row.innerHTML = `
            <input type="text" class="form-control form-control-sm sec02-desc-line" placeholder="One line (e.g. • H315: Causes skin irritation.)" />
            <button type="button" class="btn btn-sm btn-outline-danger sec02-remove-desc-line flex-shrink-0" title="Remove line"><i class="fas fa-times"></i></button>`;
        row.querySelector('.sec02-desc-line').value = value || '';
        row.querySelector('.sec02-desc-line').addEventListener('input', () => syncHidden(root));
        row.querySelector('.sec02-remove-desc-line').addEventListener('click', () => {
            const lines = host.querySelectorAll('.sec02-desc-row');
            if (lines.length <= 1) {
                row.querySelector('.sec02-desc-line').value = '';
            } else {
                row.remove();
            }
            syncHidden(root);
        });
        host.appendChild(row);
    }

    function bindInputs(card, root) {
        card.querySelectorAll('input:not(.sec02-desc-line), textarea').forEach(el => {
            el.addEventListener('input', () => syncHidden(root));
            el.addEventListener('change', () => syncHidden(root));
        });
        const rm = card.querySelector('.sec02-remove-block');
        if (rm) rm.addEventListener('click', () => { card.remove(); syncHidden(root); });
    }

    function renderGhsPicker(selectedCsv) {
        const selected = new Set((selectedCsv || '').split(',').map(s => s.trim()).filter(Boolean));
        let html = '<div class="ghs-pictogram-grid sec02-ghs-grid">';
        ghsCatalog.forEach(g => {
            const on = selected.has(g.code);
            html += `<label class="ghs-pictogram-item ${on ? 'selected' : ''}">
                <input type="checkbox" class="sec02-ghs-check ghs-pictogram-check" value="${g.code}" ${on ? 'checked' : ''} />
                <img src="${g.imagePath}" alt="${g.label}" loading="lazy" />
                <span>${g.label}</span></label>`;
        });
        html += '</div>';
        return html;
    }

    function addTableRow(tbody, root) {
        const tr = document.createElement('tr');
        tr.className = 'sec02-table-row';
        tr.innerHTML = `
            <td><input type="text" class="form-control form-control-sm sec02-cas" placeholder="54464-57-2" /></td>
            <td><input type="text" class="form-control form-control-sm sec02-chem-name" placeholder="Chemical name" /></td>
            <td class="sds-grid-actions"><button type="button" class="btn btn-sm btn-outline-danger sec02-remove-row" title="Remove row"><i class="fas fa-times"></i></button></td>`;
        tbody.appendChild(tr);
        tr.querySelectorAll('input').forEach(inp => {
            inp.addEventListener('input', () => syncHidden(root));
        });
        tr.querySelector('.sec02-remove-row')?.addEventListener('click', () => {
            tr.remove();
            syncHidden(root);
        });
        syncHidden(root);
    }

    function createBlock(root, block) {
        const card = document.createElement('div');
        card.className = 'sec02-block-card border rounded p-3 bg-light mb-3';
        const type = block.type || 'text';
        card.dataset.blockType = type;

        if (type === 'text') {
            card.innerHTML = `
                <div class="d-flex justify-content-between align-items-start mb-2">
                    <span class="badge bg-secondary">Text subsection</span>
                    <button type="button" class="btn btn-sm btn-outline-danger sec02-remove-block" title="Remove"><i class="fas fa-times"></i></button>
                </div>
                <label class="form-label small text-muted">Heading</label>
                <input type="text" class="form-control sec02-heading mb-2" placeholder="e.g. 2.1 Classification of the substance or mixture" />
                <label class="form-label small text-muted">Description (one line per input)</label>
                <div class="sec02-desc-lines mb-2"></div>
                <button type="button" class="btn btn-sm btn-outline-secondary sec02-add-desc-line"><i class="fas fa-plus"></i> Add more</button>`;
            card.querySelector('.sec02-heading').value = block.heading || '';
            const descHost = card.querySelector('.sec02-desc-lines');
            descriptionToLines(block.description || '').forEach(line => addDescLine(descHost, root, line));
            card.querySelector('.sec02-add-desc-line').addEventListener('click', () => {
                addDescLine(descHost, root, '');
                syncHidden(root);
            });
        } else if (type === 'pictograms') {
            const blockId = (block.blockId && String(block.blockId).trim()) ? block.blockId : crypto.randomUUID();
            const customPath = block.customImagePath || '';
            card.dataset.blockId = blockId;
            card.dataset.customImagePath = customPath;
            const previewHtml = customPath
                ? `<div class="sec02-custom-preview mb-2"><img src="/${customPath.replace(/^\//, '')}" alt="Custom pictogram" class="ghs-label-upload-img" /><small class="d-block text-muted">Current custom pictogram — upload to replace.</small></div>`
                : '';
            card.innerHTML = `
                <div class="d-flex justify-content-between align-items-start mb-2">
                    <span class="badge bg-warning text-dark">Hazard pictograms</span>
                    <button type="button" class="btn btn-sm btn-outline-danger sec02-remove-block" title="Remove"><i class="fas fa-times"></i></button>
                </div>
                <p class="text-muted small mb-2">Select standard GHS pictograms below, or upload your own if none apply.</p>
                <div class="sec02-ghs-host"></div>
                <hr class="my-3" />
                <label class="form-label small fw-semibold">Custom pictogram image</label>
                ${previewHtml}
                <input type="file" class="form-control form-control-sm sec02-custom-pic-file" name="sec02CustomPic_${blockId}" accept="image/jpeg,image/png,image/webp,image/gif" />`;
            card.querySelector('.sec02-ghs-host').innerHTML = renderGhsPicker(block.codes);
            card.querySelectorAll('.sec02-ghs-check').forEach(ch => {
                ch.addEventListener('change', () => {
                    ch.closest('.ghs-pictogram-item')?.classList.toggle('selected', ch.checked);
                    syncHidden(root);
                });
            });
            card.querySelector('.sec02-custom-pic-file')?.addEventListener('change', () => syncHidden(root));
        } else if (type === 'table') {
            card.innerHTML = `
                <div class="d-flex justify-content-between align-items-start mb-2">
                    <span class="badge bg-info text-dark">CAS table</span>
                    <button type="button" class="btn btn-sm btn-outline-danger sec02-remove-block" title="Remove table"><i class="fas fa-times"></i></button>
                </div>
                <label class="form-label small text-muted">Table title</label>
                <input type="text" class="form-control sec02-table-title mb-2" placeholder="Hazardous component(s) for labelling:" />
                <div class="table-responsive">
                    <table class="table table-sm table-bordered mb-2">
                        <thead><tr><th>CAS No.</th><th>Chemical name</th><th style="width:40px"></th></tr></thead>
                        <tbody class="sec02-table-body"></tbody>
                    </table>
                </div>
                <button type="button" class="btn btn-sm btn-outline-secondary sec02-add-row"><i class="fas fa-plus"></i> Add row</button>`;
            card.querySelector('.sec02-table-title').value = block.title || '';
            const tbody = card.querySelector('.sec02-table-body');
            const rows = block.rows && block.rows.length ? block.rows : [{ cas: '', chemicalName: '' }];
            rows.forEach(r => {
                addTableRow(tbody, root);
                const last = tbody.lastElementChild;
                last.querySelector('.sec02-cas').value = r.cas || r.Cas || '';
                last.querySelector('.sec02-chem-name').value = r.chemicalName || r.ChemicalName || '';
            });
            card.querySelector('.sec02-add-row').addEventListener('click', () => addTableRow(tbody, root));
        }

        root.querySelector('#sec02BlocksContainer').appendChild(card);
        bindInputs(card, root);
        syncHidden(root);
    }

    function init(root) {
        const list = root.querySelector('#sec02BlocksContainer');
        if (!list) return;
        list.innerHTML = '';
        const blocks = readInitial();
        const data = blocks.length ? blocks : [
            { type: 'text', heading: '2.1 Classification of the substance or mixture', description: '' },
            { type: 'text', heading: '2.2 Label elements', description: '' },
            { type: 'pictograms', codes: '' }
        ];
        data.forEach(b => createBlock(root, b));

        root.querySelector('.sec02-add-text')?.addEventListener('click', () => createBlock(root, { type: 'text', heading: '', description: '' }));
        root.querySelector('.sec02-add-table')?.addEventListener('click', () => createBlock(root, { type: 'table', title: '', rows: [{ cas: '', chemicalName: '' }] }));
        root.querySelector('.sec02-add-pictograms')?.addEventListener('click', () => createBlock(root, { type: 'pictograms', codes: '' }));

        const form = root.closest('form');
        if (form) form.addEventListener('submit', () => syncHidden(root));
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec02-blocks-editor').forEach(init);
    });
})();
