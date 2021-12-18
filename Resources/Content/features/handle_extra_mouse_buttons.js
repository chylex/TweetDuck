import { $ } from "../api/jquery.js";
import { getHoveredColumn } from "../globals/get_hovered_column.js";
import { getHoveredTweet } from "../globals/get_hovered_tweet.js";

const tryClickSelector = function(selector, parent) {
	return $(selector, parent).click().length;
};

const tryCloseModal1 = function() {
	const modal = $("#open-modal");
	return modal.is(":visible") && tryClickSelector("a.mdl-dismiss", modal);
};

const tryCloseModal2 = function() {
	const modal = $(".js-modals-container");
	return modal.length && tryClickSelector("a.mdl-dismiss", modal);
};

const tryCloseHighlightedColumn = function() {
	const column = getHoveredColumn();
	if (!column) {
		return false;
	}
	
	const ele = $(column.ele);
	return (ele.is(".is-shifted-2") && tryClickSelector(".js-tweet-social-proof-back", ele)) || (ele.is(".is-shifted-1") && tryClickSelector(".js-column-back", ele));
};

/**
 * Adds support for back/forward mouse buttons.
 */
export default function() {
	window.TDGF_onMouseClickExtra = function(button) {
		if (button === 1) { // back button
			tryClickSelector(".is-shifted-2 .js-tweet-social-proof-back", ".js-modal-panel") ||
			tryClickSelector(".is-shifted-1 .js-column-back", ".js-modal-panel") ||
			tryCloseModal1() ||
			tryCloseModal2() ||
			tryClickSelector(".js-inline-compose-close") ||
			tryCloseHighlightedColumn() ||
			tryClickSelector(".js-app-content.is-open .js-drawer-close:visible") ||
			tryClickSelector(".is-shifted-2 .js-tweet-social-proof-back, .is-shifted-2 .js-dm-participants-back") ||
			$(".is-shifted-1 .js-column-back").click();
		}
		else if (button === 2) { // forward button
			getHoveredTweet()?.ele.children[0]?.click();
		}
	};
};
