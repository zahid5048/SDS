(function () {
    function parseInitial(root) {
        const hidden = root.querySelector('#Sec14Subsections');
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
        const hidden = root.querySelector('#Sec14Subsections');
        if (!hidden) return;
        const items = [];
        root.querySelectorAll('.sec14-subsection-card').forEach(card => {
            const heading = (card.querySelector('.sec14-heading')?.value || '').trim();
            const description = (card.querySelector('.sec14-description')?.value || '').trim();
            if (!heading && !description) return;
            items.push({ heading, description });
        });
        hidden.value = JSON.stringify(items);
    }

    function bindCard(card, root) {
        card.querySelectorAll('.sec14-heading, .sec14-description').forEach(el => {
            el.addEventListener('input', () => syncHidden(root));
        });
        card.querySelector('.sec14-remove')?.addEventListener('click', () => {
            card.remove();
            syncHidden(root);
        });
    }

    function createCard(root, item) {
        const heading = item?.heading || item?.Heading || '';
        const description = item?.description || item?.Description || '';

        const card = document.createElement('div');
        card.className = 'sec14-subsection-card border rounded p-3 bg-light';
        card.innerHTML = `
            <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
                <label class="form-label fw-semibold mb-0 small text-uppercase text-muted">Subsection heading</label>
                <button type="button" class="btn btn-sm btn-outline-danger sec14-remove" title="Remove subsection"><i class="fas fa-times"></i></button>
            </div>
            <input type="text" class="form-control sec14-heading mb-2" placeholder="e.g. 14.1 UN number or ID number" />
            <label class="form-label small text-muted">Description</label>
            <textarea class="form-control sec14-description" rows="4" placeholder="Use ADR / IMDG / IATA / ADG: on its own line where shown on the SDS"></textarea>`;

        card.querySelector('.sec14-heading').value = heading;
        card.querySelector('.sec14-description').value = description;

        root.querySelector('#sec14SubsectionRows').appendChild(card);
        bindCard(card, root);
    }

    function init(root) {
        const rows = parseInitial(root);
        const list = root.querySelector('#sec14SubsectionRows');
        if (!list) return;
        list.innerHTML = '';
        rows.forEach(r => createCard(root, r));
        syncHidden(root);
        root.querySelector('.sec14-add-subsection')?.addEventListener('click', () => {
            createCard(root, { heading: '', description: '' });
            syncHidden(root);
        });
        root.closest('form')?.addEventListener('submit', () => syncHidden(root));
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec14-subsections-editor').forEach(init);
    });
})();
