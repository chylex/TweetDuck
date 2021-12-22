import { onAppReady } from "../api/ready.js";

/**
 * Reorders search results so that accounts are above hashtags.
 */
export default function() {
	onAppReady(function moveAccountsAboveHashtagsInSearch() {
		const container = document.querySelector(".js-search-in-popover");
		
		const users = container.querySelector(".js-typeahead-user-list");
		const hashtags = container.querySelector(".js-typeahead-topic-list");
		
		hashtags.insertAdjacentElement("beforebegin", users);
		hashtags.classList.add("list-divider");
	});
};
