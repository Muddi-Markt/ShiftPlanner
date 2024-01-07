async function setTitleFromAppSettings() {
    try {
        const response = await fetch('customization/customization.json');
        const settings = await response.json();
        const title = settings.App.Title; // Replace 'AppTitle' with your actual key
        const subtitle = settings.App.Subtitle; // Replace 'AppTitle' with your actual key
        document.querySelector('.muddi-plain-content h1').textContent = title;
        document.querySelector('.muddi-plain-content p').textContent = subtitle;
        document.title = title;
    } catch (error) {
        console.error('Error setting title from customization.json:', error);
    }
}

setTitleFromAppSettings();