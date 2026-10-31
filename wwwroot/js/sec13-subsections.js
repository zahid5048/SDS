(function () {
    function parseInitial(root) {
        const hidden = root.querySelector('#Sec13Subsections');
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
        const hidden = root.querySelector('#Sec13Subsections');
        if (!hidden) return;
        const items = [];
        root.querySelectorAll('.sec13-subsection-card').forEach(card => {
            const heading = (card.querySelector('.sec13-heading')?.value || '').trim();
            const description = (card.querySelector('.sec13-description')?.value || '').trim();
            if (!heading && !description) return;
            items.push({ heading, description });
        });
        hidden.value = JSON.stringify(items);
    }

    function bindCard(card, root) {
        card.querySelectorAll('.sec13-heading, .sec13-description').forEach(el => {
            el.addEventListener('input', () => syncHidden(root));
        });
        card.querySelector('.sec13-remove')?.addEventListener('click', () => {
            card.remove();
            syncHidden(root);
        });
    }

    function createCard(root, item) {
        const heading = item?.heading || item?.Heading || '';
        const description = item?.description || item?.Description || '';

        const card = document.createElement('div');
        card.className = 'sec13-subsection-card border rounded p-3 bg-light';
        card.innerHTML = `
            <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
                <label class="form-label fw-semibold mb-0 small text-uppercase text-muted">Subsection heading</label>
                <button type="button" class="btn btn-sm btn-outline-danger sec13-remove" title="Remove subsection"><i class="fas fa-times"></i></button>
            </div>
            <input type="text" class="form-control sec13-heading mb-2" placeholder="e.g. 13.1 Waste treatment methods" />
            <label class="form-label small text-muted">Description</label>
            <textarea class="form-control sec13-description" rows="5" placeholder="Disposal instructions"></textarea>`;

        card.querySelector('.sec13-heading').value = heading;
        card.querySelector('.sec13-description').value = description;

        root.querySelector('#sec13SubsectionRows').appendChild(card);
        bindCard(card, root);
    }

    function init(root) {
        const rows = parseInitial(root);
        const list = root.querySelector('#sec13SubsectionRows');
        if (!list) return;
        list.innerHTML = '';
        rows.forEach(r => createCard(root, r));
        syncHidden(root);
        root.querySelector('.sec13-add-subsection')?.addEventListener('click', () => {
            createCard(root, { heading: '', description: '' });
            syncHidden(root);
        });
        root.closest('form')?.addEventListener('submit', () => syncHidden(root));
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec13-subsections-editor').forEach(init);
    });
})();
