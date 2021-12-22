let isReloading = false;

/**
 * Reloads the website with memory cleanup if available.
 */
export function reloadBrowser() {
	if (isReloading) {
		return;
	}
	
	if ("gc" in window) {
		window.gc();
	}
	
	isReloading = true;
	window.location.reload();
}
