// Dark Mode Toggle
(function() {
    const themeKey = 'taskflow-theme';
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)');

    // Set initial theme
    function initTheme() {
        const savedTheme = localStorage.getItem(themeKey);
        const isDark = savedTheme ? savedTheme === 'dark' : prefersDark.matches;
        
        setTheme(isDark ? 'dark' : 'light');
    }

    // Set theme
    function setTheme(theme) {
        const html = document.documentElement;
        html.setAttribute('data-bs-theme', theme);
        localStorage.setItem(themeKey, theme);
        updateThemeButton(theme);
    }

    // Update theme button icon
    function updateThemeButton(theme) {
        const btn = document.getElementById('themeToggleBtn');
        if (btn) {
            const icon = btn.querySelector('i');
            if (theme === 'dark') {
                icon.classList.remove('fa-moon');
                icon.classList.add('fa-sun');
                btn.title = 'Switch to Light Mode';
            } else {
                icon.classList.remove('fa-sun');
                icon.classList.add('fa-moon');
                btn.title = 'Switch to Dark Mode';
            }
        }
    }

    // Toggle theme
    window.toggleTheme = function() {
        const html = document.documentElement;
        const currentTheme = html.getAttribute('data-bs-theme') || 'light';
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        setTheme(newTheme);
    };

    // Initialize on load
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initTheme);
    } else {
        initTheme();
    }

    // Listen to system theme changes
    prefersDark.addEventListener('change', (e) => {
        if (!localStorage.getItem(themeKey)) {
            setTheme(e.matches ? 'dark' : 'light');
        }
    });
})();

// Delete confirmation handler
$(document).ready(function() {
    $('.delete-project, .delete-task, .delete-task-detail').on('click', function() {
        var id = $(this).data('id');
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $('#delete-form-' + id).submit();
            }
        })
    });
});
