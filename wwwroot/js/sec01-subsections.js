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

    function descriptionFrom(rows) {
        const row = rows.find(item => String(item.heading || item.Heading || '').trim().toLowerCase().startsWith('1.1'))
            || rows.find(item => (item.description || item.Description) && !(item.fields || item.Fields || []).length)
            || rows[0];
        if (!row) return '';
        return row.description || row.Description || '';
    }

    function plainText(value) {
        if (!value || !value.includes('<')) return (value || '').replace(/\u200b/g, '').trim();
        const holder = document.createElement('div');
        holder.innerHTML = value;
        return (holder.textContent || '').replace(/\u200b/g, '').replace(/\u00a0/g, ' ').replace(/\s+/g, ' ').trim();
    }

    function shortProduct(value) {
        const text = plainText(value).replace(/\s+/g, ' ').trim();
        const match = text.match(/Product identifier\s*\/\s*No\.?\s*:\s*(.+?)(?=\s+\d+\.\d+\b|$)/i);
        if (match && match[1].trim()) return match[1].trim().replace(/[.;:\s]+$/, '').slice(0, 80);
        const stripped = text.replace(/^1\.1\s+Product identifier\s*/i, '').trim();
        const candidate = stripped || text;
        return candidate.length <= 80 ? candidate : candidate.slice(0, 77).trim() + '...';
    }

    function syncHidden(root) {
        const hidden = root.querySelector('#Sec01Subsections');
        const textarea = root.querySelector('.sec01-description');
        const syncPid = document.getElementById('Sec01ProductIdentifierSync');
        if (!hidden || !textarea) return;
        const description = (textarea.value || '').trim();
        const items = description
            ? [{ heading: '1.1 Product identifier', description }]
            : [];
        hidden.value = JSON.stringify(items);
        if (syncPid) syncPid.value = shortProduct(description);
    }

    function init(root) {
        const textarea = root.querySelector('.sec01-description');
        if (!textarea) return;
        textarea.value = descriptionFrom(parseInitial(root));
        const sync = () => syncHidden(root);
        if (window.WordDescriptionEditor)
            window.WordDescriptionEditor.attach(textarea, sync);
        else
            textarea.addEventListener('input', sync);
        sync();
        root.closest('form')?.addEventListener('submit', sync);
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.sec01-subsections-editor').forEach(init);
    });
})();
