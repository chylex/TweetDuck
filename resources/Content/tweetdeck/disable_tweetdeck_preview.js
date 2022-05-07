import { TD } from "../api/td.js";

export default function() {
	const overlay = TD.config.config_overlay || (TD.config.config_overlay = {});
	overlay["tweetdeck_gryphon_beta_enabled"] = { value: false };
	overlay["tweetdeck_gryphon_beta_bypass_enabled"] = { value: false };
}
