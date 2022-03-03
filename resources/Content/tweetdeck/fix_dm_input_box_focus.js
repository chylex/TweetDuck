import { $TDX } from "../api/bridge.js";
import { runAfterFunction } from "../api/patch.js";
import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";

function focusDmInput() {
	document.querySelector(".js-reply-tweetbox")?.focus();
}

/**
 * Fixes DM reply input box not getting focused after opening a conversation.
 */
export default function() {
	ensurePropertyExists(TD, "components", "ConversationDetailView", "prototype");
	
	runAfterFunction(TD.components.ConversationDetailView.prototype, "showChirp", function() {
		if ($TDX.focusDmInput) {
			setTimeout(focusDmInput, 100);
		}
	});
};
