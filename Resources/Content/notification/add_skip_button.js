import { $TD } from "../api/bridge.js";

const iconHTML = `
<svg id="td-skip" width="10" height="17" viewBox="0 0 350 600">
  <path fill="#888" d="M0,151.656l102.208-102.22l247.777,247.775L102.208,544.986L0,442.758l145.546-145.547">
</svg>`;

/**
 * Adds an icon button that skips to the next notification.
 */
export default function() {
	document.body.children[0].insertAdjacentHTML("beforeend", iconHTML);
	
	document.getElementById("td-skip").addEventListener("click", function() {
		$TD.loadNextNotification();
	});
};
