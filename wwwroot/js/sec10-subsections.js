(function () {
    function parseInitial(root) {
        const hidden = root.querySelector('#Sec10Subsections');
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
        const hidden = root.querySelector('#Sec10Subsections');
        if (!hidden) return;
        const items = [];
        root.querySelectorAll('.sec10-subsection-card').forEach(card => {
            const heading = (card.querySelector('.sec10-heading')?.value || '').trim();
            const description = (card.querySelector('.sec10-description')?.value || '').trim();
            if (!heading && !description) return;
            items.push({ heading, description });
        });
        hidden.value = JSON.stringify(items);
    }

    function bindCard(card, root) {
        card.querySelectorAll('.sec10-heading, .sec10-description').forEach(el => {
            el.addEventListener('input', () => syncHidden(root));
        });
        card.querySelector('.sec10-remove')?.addEventListener('click', () => {
            card.remove();
            syncHidden(root);
        });
    }

    function createCard(root, item) {
        const heading = item?.heading || item?.Heading || '';
        const description = item?.description || item?.Description || '';

        const card = document.createElement('div');
        card.className = 'sec10-subsection-card border rounded p-3 bg-light';
        card.innerHTML = `
            <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
                <label class="form-label fw-semibold mb-0 small text-uppercase text-muted">Subsection heading</label>
                <button type="button" class="btn btn-sm btn-outline-danger sec10-remove" title="Remove subsection"><i class="fas fa-times"></i></button>
            </div>
            <input type="text" class="form-control sec10-heading mb-2" placeholder="e.g. 10.1 Reactivity" />
            <label class="form-label small text-muted">Description</label>
            <textarea class="form-control sec10-description" rows="3" placeholder="Text for this subsection"></textarea>`;

        card.querySelector('.sec10-heading').value = heading;
        card.querySelector('.sec10-description').value = description;

        root.querySelector('#sec10SubsectionRows').appendChild(card);
        bindCard(card, root);
    }

    function init(root) {
        const rows = parseInitial(root);
        const list = root.querySelector('#sec10SubsectionRows');
        if (!list) return;
        list.innerHTML = '';
        rows.forEach(r => createCard(root, r));
        syncHidden(root);
        root.querySelector('.sec10-add-subsection')?.addEventListener('click', () => {
            createCard(root, { heading: '', description: '' });
            syncHidden(root);
        });
        root.closest('form')?.addEventListener('submit', () => syncHidden(root));
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec10-subsections-editor').forEach(init);
    });
})();
