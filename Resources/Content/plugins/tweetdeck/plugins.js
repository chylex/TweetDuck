import { isAppReady } from "../../api/ready.js";
import { ensurePropertyExists } from "../../api/utils.js";
import { $TDP } from "../base.js";
import setup from "../setup.js";

export default function() {
	ensurePropertyExists(window, "TD_PLUGINS_DISABLE");
	
	window.PluginBase = class {
		/**
		 * @param {{ [requiresPageReload]: boolean }} pluginSettings
		 */
		constructor(pluginSettings) {
			this.$requiresReload = !!(pluginSettings && pluginSettings.requiresPageReload);
		}
		
		enabled() {}
		ready() {}
		disabled() {}
	};
	
	// noinspection JSUnusedGlobalSymbols
	window.TD_PLUGINS = new class {
		constructor() {
			this.installed = [];
			this.disabled = window["TD_PLUGINS_DISABLE"];
			this.waitingForModules = [];
			this.waitingForReady = [];
			this.areModulesLoaded = false;
		}
		
		isDisabled(plugin) {
			return this.disabled.includes(plugin.id);
		}
		
		findObject(identifier) {
			return this.installed.find(plugin => plugin.id === identifier);
		}
		
		install(plugin) {
			this.installed.push(plugin);
			
			if (typeof plugin.obj.configure === "function") {
				$TDP.setConfigurable(plugin.obj.$token);
			}
			
			if (!this.isDisabled(plugin)) {
				this.runWhenModulesLoaded(plugin);
				this.runWhenReady(plugin);
			}
		}
		
		runWhenModulesLoaded(plugin) {
			if (this.areModulesLoaded) {
				plugin.obj.enabled();
			}
			else {
				this.waitingForModules.push(plugin);
			}
		}
		
		runWhenReady(plugin) {
			if (isAppReady()) {
				plugin.obj.ready();
			}
			else {
				this.waitingForReady.push(plugin);
			}
		}
		
		setState(plugin, enable) {
			const reloading = plugin.obj.$requiresReload;
			
			if (enable && this.isDisabled(plugin)) {
				if (reloading) {
					window.TDPF_requestReload();
				}
				else {
					this.disabled.splice(this.disabled.indexOf(plugin.id), 1);
					plugin.obj.enabled();
					this.runWhenReady(plugin);
				}
			}
			else if (!enable && !this.isDisabled(plugin)) {
				if (reloading) {
					window.TDPF_requestReload();
				}
				else {
					this.disabled.push(plugin.id);
					plugin.obj.disabled();
					
					for (const key of Object.keys(plugin.obj)) {
						if (key[0] !== "$") {
							delete plugin.obj[key];
						}
					}
				}
			}
		}
		
		onModulesLoaded(namespace) {
			if (namespace === "tweetdeck") {
				window.TDPF_getColumnName = window.TDGF_getColumnName;
				window.TDPF_reloadColumns = window.TDGF_reloadColumns;
				window.TDPF_prioritizeNewestEvent = window.TDGF_prioritizeNewestEvent;
				window.TDPF_injectMustache = window.TDGF_injectMustache;
				window.TDPF_registerPropertyUpdateCallback = window.TDGF_registerPropertyUpdateCallback;
				window.TDPF_playVideo = function(urlOrObject, username) {
					if (typeof urlOrObject === "string") {
						window.TDGF_playVideo(urlOrObject, null, username);
					}
					else {
						// noinspection JSUnresolvedVariable
						window.TDGF_playVideo(urlOrObject.videoUrl, urlOrObject.tweetUrl, urlOrObject.username);
					}
				};
				
				this.waitingForModules.forEach(plugin => plugin.obj.enabled());
				this.waitingForModules = [];
				this.areModulesLoaded = true;
			}
		}
		
		onReady() {
			this.waitingForReady.forEach(plugin => plugin.obj.ready());
			this.waitingForReady = [];
		}
	};
	
	/**
	 * Changes plugin enabled state.
	 * @param {string} identifier
	 * @param {boolean} enable
	 */
	window.TDPF_setPluginState = function(identifier, enable) {
		window.TD_PLUGINS.setState(window.TD_PLUGINS.findObject(identifier), enable);
	};
	
	/**
	 * Triggers configure() function on a plugin object.
	 * @param {string} identifier
	 */
	window.TDPF_configurePlugin = function(identifier) {
		window.TD_PLUGINS.findObject(identifier).obj.configure();
	};
	
	(function() {
		let isReloading = false;
		
		/**
		 * Reloads the page.
		 */
		window.TDPF_requestReload = function() {
			if (!isReloading) {
				window.setTimeout(window.TDGF_reload, 1);
				isReloading = true;
			}
		};
	})();
	
	setup();
};
