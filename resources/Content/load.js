(function() {
	const script = document.currentScript;
	const namespace = script.getAttribute("data-namespace");
	const tag = `[TweetDuck:${namespace}]`;
	
	async function loadModule(path) {
		let module;
		try {
			module = await import(path);
		} catch (e) {
			console.error(`${tag} Error loading '${path}': ${e}`);
			return false;
		}
		
		try {
			module.default();
			console.info(`${tag} Successfully loaded '${path}'`);
			return true;
		} catch (e) {
			console.error(`${tag} Error executing '${path}': ${e}`);
		}
		
		return false;
	}
	
	async function loadModules() {
		const modules = script.getAttribute("data-modules").split("|");
		
		let successes = 0;
		for (const module of modules) {
			if (await loadModule(`./${namespace}/${module}.js`)) {
				++successes;
			}
		}
		
		return [ successes, modules.length ];
	}
	
	function notifyModulesLoaded(obj) {
		try {
			obj.onModulesLoaded(namespace);
		} catch (e) {
			console.error(`${tag} Error in post-load notification: ${e}`);
		}
	}
	
	loadModules().then(([ successes, total ]) => {
		if ("$TD" in window && "onModulesLoaded" in window.$TD) {
			notifyModulesLoaded(window.$TD);
		}
		
		if ("TD_PLUGINS" in window) {
			notifyModulesLoaded(window.TD_PLUGINS);
		}
		
		console.info(`${tag} Successfully loaded ${successes} / ${total} module(s).`);
	});
})();
