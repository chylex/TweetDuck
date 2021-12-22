import { createCustomStyle, createStorage, loadConfigurationFile } from "./base.js";

export default function() {
	window.TDPF_loadConfigurationFile = loadConfigurationFile;
	window.TDPF_createCustomStyle = createCustomStyle;
	window.TDPF_createStorage = createStorage;
	
	if ("TD_PLUGINS_SETUP" in window) {
		for (const callback of window.TD_PLUGINS_SETUP) {
			callback();
		}
		
		delete window["TD_PLUGINS_SETUP"];
	}
	
	/**
	 * Replaces the deferred installation function from PluginScriptGenerator with one that performs the installation function immediately,
	 * since at this point we are all set up and aren't checking TD_PLUGINS_SETUP anymore.
	 * @param {function} f
	 */
	window.TD_PLUGINS_INSTALL = f => f();
};
