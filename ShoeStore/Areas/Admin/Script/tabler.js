/**
 * Tabler - Static Custom JavaScript
 * Minimal JavaScript for theme switching and navbar functionality
 */

(function() {
  'use strict';

  // ========================================
  // Theme Management
  // ========================================

  const THEME_STORAGE_KEY = 'tablerTheme';

  /**
   * Get the stored theme preference or default to light
   */
  function getStoredTheme() {
    return localStorage.getItem(THEME_STORAGE_KEY) || 'light';
  }

  /**
   * Save theme preference to localStorage
   */
  function setStoredTheme(theme) {
    localStorage.setItem(THEME_STORAGE_KEY, theme);
  }

  /**
   * Apply theme to the document
   */
  function setTheme(theme) {
    if (theme === 'auto') {
      // Auto theme: use system preference
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      theme = prefersDark ? 'dark' : 'light';
    }

    document.documentElement.setAttribute('data-bs-theme', theme);
  }

  /**
   * Toggle between light and dark themes
   */
  function toggleTheme() {
    const currentTheme = getStoredTheme();
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    setStoredTheme(newTheme);
    setTheme(newTheme);
  }

  /**
   * Initialize theme on page load
   */
  function initTheme() {
    // Apply stored theme immediately to prevent flash
    const storedTheme = getStoredTheme();
    setTheme(storedTheme);

    // Listen for system theme changes if using auto
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function(e) {
      if (getStoredTheme() === 'auto') {
        setTheme('auto');
      }
    });
  }

  // Apply theme as early as possible
  initTheme();

  // ========================================
  // Navbar Toggle (Mobile Menu)
  // ========================================

  /**
   * Toggle mobile navbar collapse
   */
  function initNavbarToggle() {
    const toggler = document.querySelector('.navbar-toggler');
    const collapse = document.querySelector('.navbar-collapse');

    if (toggler && collapse) {
      toggler.addEventListener('click', function() {
        const isExpanded = toggler.getAttribute('aria-expanded') === 'true';

        // Toggle aria-expanded
        toggler.setAttribute('aria-expanded', !isExpanded);

        // Toggle collapse classes
        if (isExpanded) {
          collapse.classList.remove('show');
          collapse.classList.add('collapsing');

          // Remove collapsing class after animation
          setTimeout(function() {
            collapse.classList.remove('collapsing');
          }, 350);
        } else {
          collapse.classList.add('collapsing');
          collapse.classList.add('show');

          // Remove collapsing class after animation
          setTimeout(function() {
            collapse.classList.remove('collapsing');
          }, 350);
        }
      });

      // Close menu when clicking outside
      document.addEventListener('click', function(e) {
        if (!toggler.contains(e.target) && !collapse.contains(e.target)) {
          if (toggler.getAttribute('aria-expanded') === 'true') {
            toggler.click();
          }
        }
      });
    }
  }

  // ========================================
  // Dropdown Functionality
  // ========================================

  /**
   * Initialize dropdown menus
   */
  function initDropdowns() {
    const dropdownToggles = document.querySelectorAll('[data-bs-toggle="dropdown"]');

    dropdownToggles.forEach(function(toggle) {
      const dropdown = toggle.nextElementSibling;

      if (dropdown && dropdown.classList.contains('dropdown-menu')) {
        // Toggle dropdown on click
        toggle.addEventListener('click', function(e) {
          e.preventDefault();
          e.stopPropagation();

          const isExpanded = toggle.getAttribute('aria-expanded') === 'true';

          // Close all other dropdowns
          closeAllDropdowns();

          // Toggle current dropdown
          if (!isExpanded) {
            toggle.setAttribute('aria-expanded', 'true');
            dropdown.classList.add('show');

            // Position dropdown
            positionDropdown(toggle, dropdown);
          }
        });
      }
    });

    // Close dropdowns when clicking outside
    document.addEventListener('click', function(e) {
      const clickedInside = e.target.closest('.dropdown');
      if (!clickedInside) {
        closeAllDropdowns();
      }
    });

    // Close dropdowns on escape key
    document.addEventListener('keydown', function(e) {
      if (e.key === 'Escape') {
        closeAllDropdowns();
      }
    });
  }

  /**
   * Close all open dropdowns
   */
  function closeAllDropdowns() {
    const openDropdowns = document.querySelectorAll('[data-bs-toggle="dropdown"][aria-expanded="true"]');

    openDropdowns.forEach(function(toggle) {
      toggle.setAttribute('aria-expanded', 'false');
      const dropdown = toggle.nextElementSibling;
      if (dropdown) {
        dropdown.classList.remove('show');
      }
    });
  }

  /**
   * Position dropdown menu
   */
  function positionDropdown(toggle, dropdown) {
    // Check if dropdown has .dropdown-menu-end class
    if (dropdown.classList.contains('dropdown-menu-end')) {
      // Already positioned by CSS
      return;
    }

    // Basic positioning - could be enhanced
    const rect = toggle.getBoundingClientRect();
    const dropdownRect = dropdown.getBoundingClientRect();

    // Check if dropdown would go off screen
    if (rect.left + dropdownRect.width > window.innerWidth) {
      dropdown.classList.add('dropdown-menu-end');
    }
  }

  // ========================================
  // Theme Toggle Button Handlers
  // ========================================

  /**
   * Initialize theme toggle buttons
   */
  function initThemeToggle() {
    const darkToggle = document.getElementById('theme-toggle-dark');
    const lightToggle = document.getElementById('theme-toggle-light');

    if (darkToggle) {
      darkToggle.addEventListener('click', function(e) {
        e.preventDefault();
        setStoredTheme('dark');
        setTheme('dark');
      });
    }

    if (lightToggle) {
      lightToggle.addEventListener('click', function(e) {
        e.preventDefault();
        setStoredTheme('light');
        setTheme('light');
      });
    }
  }

  // ========================================
  // Form Enhancements
  // ========================================

  /**
   * Auto-clear search input on focus
   */
  function initSearchInputs() {
    const searchInputs = document.querySelectorAll('input[type="text"][placeholder*="Search"]');

    searchInputs.forEach(function(input) {
      // Select text on focus
      input.addEventListener('focus', function() {
        this.select();
      });
    });
  }

  // ========================================
  // Accessibility Enhancements
  // ========================================

  /**
   * Add keyboard navigation support
   */
  function initKeyboardNavigation() {
    // Tab trap for modals (if any)
    document.addEventListener('keydown', function(e) {
      if (e.key === 'Tab') {
        // Add tab trap logic if needed for modals
      }
    });

    // Escape key handling
    document.addEventListener('keydown', function(e) {
      if (e.key === 'Escape') {
        // Close any open overlays
        closeAllDropdowns();
      }
    });
  }

  // ========================================
  // Smooth Scroll (for anchor links)
  // ========================================

  /**
   * Add smooth scrolling to anchor links
   */
  function initSmoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach(function(anchor) {
      anchor.addEventListener('click', function(e) {
        const href = this.getAttribute('href');

        // Skip if it's just "#"
        if (href === '#' || href === '#!') {
          return;
        }

        const target = document.querySelector(href);

        if (target) {
          e.preventDefault();
          target.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
          });
        }
      });
    });
  }

  // ========================================
  // Responsive Utilities
  // ========================================

  /**
   * Handle window resize events
   */
  function initResponsiveHandlers() {
    let resizeTimer;

    window.addEventListener('resize', function() {
      clearTimeout(resizeTimer);
      resizeTimer = setTimeout(function() {
        // Close mobile menu on resize to desktop
        if (window.innerWidth >= 768) {
          const collapse = document.querySelector('.navbar-collapse');
          const toggler = document.querySelector('.navbar-toggler');

          if (collapse && toggler) {
            collapse.classList.remove('show');
            toggler.setAttribute('aria-expanded', 'false');
          }
        }

        // Close all dropdowns on resize
        closeAllDropdowns();
      }, 250);
    });
  }

  // ========================================
  // Initialize Everything on DOM Ready
  // ========================================

  /**
   * Main initialization function
   */
  function init() {
    initThemeToggle();
    initNavbarToggle();
    initDropdowns();
    initSearchInputs();
    initKeyboardNavigation();
    initSmoothScroll();
    initResponsiveHandlers();

    console.log('Tabler initialized successfully');
  }

  // Initialize when DOM is ready
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }

  // ========================================
  // Public API (optional)
  // ========================================

  // Expose some functions globally if needed
  window.Tabler = {
    setTheme: function(theme) {
      setStoredTheme(theme);
      setTheme(theme);
    },
    getTheme: getStoredTheme,
    toggleTheme: toggleTheme
  };

})();
