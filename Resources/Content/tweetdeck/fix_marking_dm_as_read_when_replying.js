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
	
	/**
	 * @param e
	 * @param {DmSentEventData} data
	 */
	const onDataDmSent = function(e, data) {
		TD.controller.clients.getClient(data.request.accountKey)?.conversations.getConversation(data.request.conversationId)?.markAsRead();
	};
	
	$(document).on("dataDmSent", onDataDmSent);
};
