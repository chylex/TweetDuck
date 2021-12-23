import { $TD, $TDX } from "../api/bridge.js";

/**
 * Adds an event listener to all elements in a collection.
 */
function addEventListener(collection, type, listener) {
	for (const ele of collection) {
		ele.addEventListener(type, listener);
	}
}

/**
 * @param {MouseEvent} e
 */
function handleLinkClick(e) {
	if (e.button === 0 || e.button === 1) {
		const ele = e.currentTarget;
		
		$TD.openBrowser(ele.href);
		e.preventDefault();
		
		if ($TDX.skipOnLinkClick) {
			const parentClasses = ele.parentNode.classList;
			
			if (parentClasses.contains("js-tweet-text") || parentClasses.contains("js-quoted-tweet-text") || parentClasses.contains("js-timestamp")) {
				$TD.loadNextNotification();
			}
		}
	}
}

/**
 * Bypasses default link open function, and skips notification when opening links if enabled in app configuration.
 */
export default function() {
	const links = document.getElementsByTagName("A");
	addEventListener(links, "click", handleLinkClick);
	addEventListener(links, "auxclick", handleLinkClick);
};
