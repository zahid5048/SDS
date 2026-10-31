(function () {
    function parseInitial(root) {
        const hidden = root.querySelector('#Sec15Subsections');
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

    function syncHidden(root) {
        const hidden = root.querySelector('#Sec15Subsections');
        if (!hidden) return;
        const items = [];
        root.querySelectorAll('.sec15-subsection-card').forEach(card => {
            const heading = (card.querySelector('.sec15-heading')?.value || '').trim();
            const description = (card.querySelector('.sec15-description')?.value || '').trim();
            if (!heading && !description) return;
            items.push({ heading, description });
        });
        hidden.value = JSON.stringify(items);
    }

    function bindCard(card, root) {
        card.querySelectorAll('.sec15-heading, .sec15-description').forEach(el => {
            el.addEventListener('input', () => syncHidden(root));
        });
        card.querySelector('.sec15-remove')?.addEventListener('click', () => {
            card.remove();
            syncHidden(root);
        });
    }

    function createCard(root, item) {
        const heading = item?.heading || item?.Heading || '';
        const description = item?.description || item?.Description || '';

        const card = document.createElement('div');
        card.className = 'sec15-subsection-card border rounded p-3 bg-light';
        card.innerHTML = `
            <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
                <label class="form-label fw-semibold mb-0 small text-uppercase text-muted">Subsection heading</label>
                <button type="button" class="btn btn-sm btn-outline-danger sec15-remove" title="Remove subsection"><i class="fas fa-times"></i></button>
            </div>
            <input type="text" class="form-control sec15-heading mb-2" placeholder="e.g. 15.3 Australian Industrial Chemicals Introduction Scheme (AICIS)" />
            <label class="form-label small text-muted">Description</label>
            <textarea class="form-control sec15-description" rows="8" placeholder="Regulatory text (use blank lines between paragraphs)"></textarea>`;

        card.querySelector('.sec15-heading').value = heading;
        card.querySelector('.sec15-description').value = description;

        root.querySelector('#sec15SubsectionRows').appendChild(card);
        bindCard(card, root);
    }

    function init(root) {
        const rows = parseInitial(root);
        const list = root.querySelector('#sec15SubsectionRows');
        if (!list) return;
        list.innerHTML = '';
        rows.forEach(r => createCard(root, r));
        syncHidden(root);
        root.querySelector('.sec15-add-subsection')?.addEventListener('click', () => {
            createCard(root, { heading: '', description: '' });
            syncHidden(root);
        });
        root.closest('form')?.addEventListener('submit', () => syncHidden(root));
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec15-subsections-editor').forEach(init);
    });
})();
