import { TD } from "../api/td.js";
import { checkPropertyExists } from "../api/utils.js";
import { replaceFunction } from "../globals/patch_functions.js";

/**
 * Replaces displayed name and avatar of the official TweetDuck account.
 */
export default function() {
	const realDisplayName = "TweetDuck";
	const realAvatar = "https://ton.twimg.com/tduck/avatar";
	const accountId = "957608948189880320";
	
	if (checkPropertyExists(TD, "services", "TwitterUser", "prototype")) {
		replaceFunction(TD.services.TwitterUser.prototype, "fromJSONObject", function(func, args) {
			/** @type TwitterUser */
			const user = func.apply(this, args);
			
			if (user.id === accountId) {
				user.name = realDisplayName;
				user.emojifiedName = realDisplayName;
				user.profileImageURL = realAvatar;
				user.url = "https://tweetduck.chylex.com";
				
				if (user.entities && user.entities.url) {
					user.entities.url.urls = [{
						url: user.url,
						expanded_url: user.url,
						display_url: "tweetduck.chylex.com",
						indices: [ 0, 23 ]
					}];
				}
			}
			
			return user;
		});
	}
	
	if (checkPropertyExists(TD, "services", "TwitterClient", "prototype")) {
		replaceFunction(TD.services.TwitterClient.prototype, "typeaheadSearch", function(func, args) {
			const [ data, onSuccess, onError ] = args;
			
			if (data.query?.toLowerCase().endsWith("tweetduck")) {
				data.query = "TryMyAwesomeApp";
			}
			
			return func.call(this, data, function(/** @type {{ users: TwitterUserJSON[] }} */ result) {
				for (const user of result.users) {
					if (user.id_str === accountId) {
						user.name = realDisplayName;
						user.profile_image_url = realAvatar;
						user.profile_image_url_https = realAvatar;
						break;
					}
				}
				
				onSuccess.apply(this, arguments);
			}, onError);
		});
	}
};
