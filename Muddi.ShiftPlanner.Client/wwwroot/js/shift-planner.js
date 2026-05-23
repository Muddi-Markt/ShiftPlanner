// shift-planner.js - JavaScript helpers for the ShiftPlanner client

/**
 * Reads app settings from localStorage and applies them to the page.
 * Called early so the user sees correct values before Blazor boots.
 */
function applyCachedAppSettings() {
    try {
        const cached = localStorage.getItem('appSettings');
        if (!cached) return;

        const settings = JSON.parse(cached);
        if (!settings) return;

        // Update the loading placeholder in index.html
        const h1 = document.querySelector('.muddi-plain-content h1');
        const p = document.querySelector('.muddi-plain-content p');
        if (h1) h1.textContent = settings.title || '';
        if (p) p.textContent = settings.subtitle || '';

        // Update page title
        if (settings.title) document.title = settings.title;
    } catch (error) {
        console.warn('Could not apply cached app settings:', error);
    }
}

// Apply immediately when this script loads
applyCachedAppSettings();
