import { $TD } from "../api/bridge.js";
import { onAppReady } from "../api/ready.js";

/**
 * @param {HTMLUListElement} list
 */
function addMenuItems(list) {
	const allDividers = list.querySelectorAll(":scope > li.drp-h-divider");
	const lastDivider = allDividers[allDividers.length - 1];
	
	lastDivider.insertAdjacentHTML("beforebegin", "<li class=\"is-selectable\" data-tweetduck><a href=\"#\" data-action>TweetDuck</a></li>");
	
	const button = list.querySelector("[data-tweetduck]");
	
	button.querySelector("a").addEventListener("click", function() {
		$TD.openContextMenu();
	});
	
	button.addEventListener("mouseenter", function() {
		button.classList.add("is-selected");
	});
	
	button.addEventListener("mouseleave", function() {
		button.classList.remove("is-selected");
	});
}

/**
 * Adds a 'TweetDuck' menu item to the left-side Settings menu.
 */
export default function() {
	onAppReady(function setupSettingsMenu() {
		document.querySelector("[data-action='settings-menu']").addEventListener("click", () => setTimeout(function() {
			const list = document.querySelector("nav.app-navigator .js-dropdown-content ul");
			if (list) {
				addMenuItems(list);
			}
		}, 0));
	});
};
