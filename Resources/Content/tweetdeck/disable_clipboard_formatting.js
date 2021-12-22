import { $TD } from "../api/bridge.js";

/**
 * Removes HTML styles when copying HTML content to clipboard.
 */
export default function() {
	document.addEventListener("copy", function() {
		window.setTimeout($TD.fixClipboard, 0);
	});
};
