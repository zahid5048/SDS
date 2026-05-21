/**
 * Show/hide password — .user-password-group toggle buttons
 */
(function () {
    function initToggle(btn) {
        const group = btn.closest('.user-password-group');
        const input = group?.querySelector('input');
        const icon = btn.querySelector('i');
        if (!input || !icon) return;

        btn.addEventListener('click', function () {
            const show = input.type === 'password';
            input.type = show ? 'text' : 'password';
            icon.classList.toggle('fa-eye', !show);
            icon.classList.toggle('fa-eye-slash', show);
            btn.setAttribute('aria-label', show ? 'Hide password' : 'Show password');
            btn.setAttribute('aria-pressed', show ? 'true' : 'false');
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.user-pwd-toggle').forEach(initToggle);
    });
})();
