function updateQueryParameters(params) {
    const url = new URL(window.location);
    for (const key in params) {
        if (params.hasOwnProperty(key)) {
            if (params[key] === null || params[key] === undefined) {
                url.searchParams.delete(key);
            } else {
                url.searchParams.set(key, params[key]);
            }
        }
    }
    window.history.pushState({}, '', url);
}