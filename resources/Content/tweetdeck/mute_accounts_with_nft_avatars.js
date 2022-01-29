import { $TDX } from "../api/bridge.js";
import { replaceFunction } from "../api/patch.js";
import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";
import { addNftUserListener, checkUserNftStatus, checkUserNftStatusImmediately, getTweetUserId, setUserNftStatus } from "./globals/user_nft_status.js";

export default function() {
	if (!$TDX.hideTweetsByNftUsers) {
		return;
	}
	
	ensurePropertyExists(TD, "controller", "clients", "getPreferredClient");
	ensurePropertyExists(TD, "services", "TwitterClient", "prototype", "addIdToMuteList");
	ensurePropertyExists(TD, "services", "TwitterUser", "prototype");
	ensurePropertyExists(TD, "vo", "Column", "prototype", "addItemsToIndex");
	
	addNftUserListener(function(id) {
		TD.controller.clients.getPreferredClient().addIdToMuteList(id);
	});
	
	replaceFunction(TD.services.TwitterUser.prototype, "fromJSONObject", function(func, args) {
		/** @type {TwitterUser} */
		const user = func.apply(this, args);
		
		if (args.length > 0 && typeof args[0] === "object") {
			const id = user.id;
			const json = args[0];
			
			if ("ext_has_nft_avatar" in json) {
				setUserNftStatus(id, json.ext_has_nft_avatar === true);
			}
			else {
				checkUserNftStatus(id);
			}
		}
		
		return user;
	});
	
	replaceFunction(TD.vo.Column.prototype, "mergeAndRenderChirps", function(func, args) {
		/** @type ChirpBase[] */
		const tweets = args[0];
		
		if (Array.isArray(tweets)) {
			for (let i = tweets.length - 1; i >= 0; i--) {
				if (checkUserNftStatusImmediately(getTweetUserId(tweets[i]))) {
					tweets.splice(i, 1);
				}
			}
		}
		
		return func.apply(this, args);
	});
};
