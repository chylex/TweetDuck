import setup from "../setup.js";

export default function() {
	window.PluginBase = class {
		constructor() {}
		run() {}
	};
	
	// noinspection JSUnusedGlobalSymbols
	window.TD_PLUGINS = new class {
		constructor() {
			this.waitingForModules = [];
			this.areModulesLoaded = false;
		}
		
		install(plugin) {
			if (this.areModulesLoaded) {
				plugin.obj.run();
			}
			else {
				this.waitingForModules.push(plugin);
			}
		}
		
		onModulesLoaded(namespace) {
			if (namespace === "plugins/notification") {
				this.waitingForModules.forEach(plugin => plugin.obj.run());
				this.waitingForModules = [];
				this.areModulesLoaded = true;
			}
		}
	};
	
	setup();
};
