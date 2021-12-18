import { onAppReady } from "../api/ready.js";
import { ensurePropertyExists } from "../api/utils.js";

/**
 * Dispatches the 'Ready' event to all enabled plugins.
 */
export default function() {
	ensurePropertyExists(window, "TD_PLUGINS");
	onAppReady(() => window.TD_PLUGINS.onReady());
};
