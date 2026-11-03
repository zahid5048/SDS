(function () {
    function parse(raw) {
        try {
            const data = JSON.parse(raw || '[]');
            return Array.isArray(data) ? data : [];
        } catch {
            return [];
        }
    }

    function escapeHtml(value) {
        return String(value || '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
    }

    function asHtml(value) {
        const text = String(value || '').trim();
        if (!text) return '';
        if (/<[a-z][\s\S]*>/i.test(text)) return text;
        return '<p>' + escapeHtml(text).replace(/\n/g, '<br>') + '</p>';
    }

    function rowsToHtml(rows) {
        return rows.map(row => {
            const parts = [];
            const description = row.description || row.Description || '';
            const type = String(row.type || row.Type || 'text');
            if (type === 'pictograms') return '';
            if (description) parts.push(asHtml(description));
            (row.fields || row.Fields || []).forEach(field => {
                const label = field.label || field.Label || '';
                const desc = field.description || field.Description || '';
                if (!label && !desc) return;
                parts.push('<p><strong>' + escapeHtml(label) + '</strong> ' + asHtml(desc) + '</p>');
            });
            const tableRows = row.rows || row.Rows || [];
            const body = tableRows.map(item => {
                const cas = item.cas || item.Cas || '';
                const name = item.chemicalName || item.ChemicalName || '';
                if (!cas && !name) return '';
                return '<tr><td>' + escapeHtml(cas) + '</td><td>' + escapeHtml(name) + '</td></tr>';
            }).join('');
            if (body) {
                const title = row.title || row.Title || '';
                if (title) parts.push('<p><strong>' + escapeHtml(title) + '</strong></p>');
                parts.push('<table><tbody><tr><th>CAS No.</th><th>Chemical name</th></tr>' + body + '</tbody></table>');
            }
            return parts.join('');
        }).join('');
    }

    function sync(root) {
        const hidden = root.querySelector('input[type="hidden"]');
        const textarea = root.querySelector('.section-description');
        if (!hidden || !textarea) return;
        const description = (textarea.value || '').trim();
        const heading = root.dataset.heading || 'Description';
        const item = root.dataset.shape === 'block'
            ? { type: 'text', heading: heading, description: description }
            : { heading: heading, description: description };
        hidden.value = JSON.stringify(description ? [item] : []);
    }

    function init(root) {
        const hidden = root.querySelector('input[type="hidden"]');
        const textarea = root.querySelector('.section-description');
        if (!textarea) return;
        const raw = (hidden && hidden.value && hidden.value.trim())
            ? hidden.value.trim()
            : (root.dataset.initial || '[]');
        textarea.value = rowsToHtml(parse(raw));
        const update = () => sync(root);
        if (window.WordDescriptionEditor)
            window.WordDescriptionEditor.attach(textarea, update);
        else
            textarea.addEventListener('input', update);
        update();
        root.closest('form')?.addEventListener('submit', update);
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('.section-description-editor').forEach(init);
    });
})();
