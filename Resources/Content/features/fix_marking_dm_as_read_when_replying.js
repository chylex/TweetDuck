import { $ } from "../api/jquery.js";
import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";

/**
 * @typedef DmSentEventData
 * @type {Object}
 *
 * @property {{ accountKey: string, conversationId: string }} request
 */

/**
 * Fixes DMs not being marked as read when replying to them.
 */
export default function() {
	ensurePropertyExists(TD, "controller", "clients", "getClient");
	ensurePropertyExists(TD, "services", "Conversations", "prototype", "getConversation");
	
	$(document).on("dataDmSent", function(e, /** @type DmSentEventData */ data) {
		TD.controller.clients.getClient(data.request.accountKey)?.conversations.getConversation(data.request.conversationId)?.markAsRead();
	});
};
