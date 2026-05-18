(function () {
    const DEBOUNCE_MS = 350;

    document.querySelectorAll('[data-live-search]').forEach(function (form) {
        const input = form.querySelector('.live-search-input');
        if (!input) return;

        const hint = form.querySelector('.search-hint');
        const pageInput = form.querySelector('.search-page-input');
        let timer = null;
        let lastSubmitted = input.value;

        function showLoading(show) {
            if (hint) hint.classList.toggle('d-none', !show);
        }

        function submitSearch() {
            if (pageInput) pageInput.value = '1';
            showLoading(true);
            lastSubmitted = input.value;
            form.requestSubmit();
        }

        input.addEventListener('input', function () {
            clearTimeout(timer);
            showLoading(true);
            timer = setTimeout(function () {
                if (input.value !== lastSubmitted) {
                    submitSearch();
                } else {
                    showLoading(false);
                }
            }, DEBOUNCE_MS);
        });

        input.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                clearTimeout(timer);
                submitSearch();
            }
        });
    });
})();
