(function () {
    const hidden = document.getElementById('GhsPictogramCodes');
    if (!hidden) return;

    function sync() {
        const codes = [];
        document.querySelectorAll('.ghs-pictogram-check:checked').forEach(function (cb) {
            codes.push(cb.value);
        });
        hidden.value = codes.join(',');
    }

    document.querySelectorAll('.ghs-pictogram-check').forEach(function (cb) {
        cb.addEventListener('change', function () {
            const item = cb.closest('.ghs-pictogram-item');
            if (item) item.classList.toggle('selected', cb.checked);
            sync();
        });
    });

    const form = document.getElementById('sdsWizardForm');
    if (form) form.addEventListener('submit', sync);
    sync();
})();
