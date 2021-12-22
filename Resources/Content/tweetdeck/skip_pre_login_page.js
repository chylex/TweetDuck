import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";
import { replaceFunction } from "./globals/patch_functions.js";

/**
 * Skips the pre-login page so that users immediately see the login page.
 */
export default function() {
	ensurePropertyExists(TD, "controller", "init");
	
	replaceFunction(TD.controller.init, "showLogin", function() {
		location.href = "https://twitter.com/login?hide_message=true&redirect_after_login=https%3A%2F%2Ftweetdeck.twitter.com%2F%3Fvia_twitter_login%3Dtrue";
	});
};
