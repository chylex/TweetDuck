import { TD } from "../api/td.js";
import { checkPropertyExists } from "../api/utils.js";
import { injectMustache } from "../globals/inject_mustache.js";
import { runAfterFunction } from "../globals/patch_functions.js";

/**
 * Makes texts saying 'Retweet' lowercase.
 */
export default function() {
	injectMustache("status/tweet_single.mustache", "replace", "{{_i}} Retweeted{{/i}}", "{{_i}} retweeted{{/i}}");
	
	if (checkPropertyExists(TD, "services", "TwitterActionRetweet", "prototype")) {
		runAfterFunction(TD.services.TwitterActionRetweet.prototype, "generateText", function() {
			this.text = this.text.replace(" Retweeted", " retweeted");
			this.htmlText = this.htmlText.replace(" Retweeted", " retweeted");
		});
	}
	
	if (checkPropertyExists(TD, "services", "TwitterActionRetweetedInteraction", "prototype")) {
		runAfterFunction(TD.services.TwitterActionRetweetedInteraction.prototype, "generateText", function() {
			this.htmlText = this.htmlText.replace(" Retweeted", " retweeted").replace(" Retweet", " retweet");
		});
	}
};
