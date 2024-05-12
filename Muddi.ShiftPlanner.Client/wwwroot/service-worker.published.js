// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js?v9');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));
self.addEventListener('updatefound', () => {
    let newWorker = self.installing;
    newWorker.addEventListener('statechange', () => {
        switch (newWorker.state) {
            case 'installed':
                if (navigator.serviceWorker.controller) {
                    // Inform any open pages to reload to start using the new service worker.
                    self.clients.matchAll({ type: 'window' }).then(clients => {
                        clients.forEach(client => client.postMessage('reload-window'));
                    });
                }
                break;
        }
    });
});
self.addEventListener('message', event => {
    if (event.data?.type === 'SKIP_WAITING') self.skipWaiting();
});

async function onInstall(event) {
    console.info('Service worker Install ' + cacheName);
    try {
        const assetsRequests = self.assetsManifest.assets
            .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
            .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
            .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
        const cache = await caches.open(cacheName);
        await cache.addAll(assetsRequests);
    } catch (error) {
        console.error('Error during service worker install:', error);
    }
}

async function onActivate(event) {
    console.info('Service worker Activate');
    try {
        const cacheKeys = await caches.keys();
        await Promise.all(cacheKeys
            .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
            .map(key => caches.delete(key)));
    } catch (error) {
        console.error('Error during service worker activate:', error);
    }
}

async function onFetch(event) {
    if (event.request.method !== 'GET') return fetch(event.request);

    const shouldServeIndexHtml = event.request.mode === 'navigate';
    const request = shouldServeIndexHtml ? 'index.html' : event.request;
    const cache = await caches.open(cacheName);

    try {
        const cachedResponse = await cache.match(request);
        return cachedResponse || fetch(event.request);
    } catch (error) {
        return fetch(event.request);
    }
}
