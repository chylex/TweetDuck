async function loadModule(path) {
	let module;
	try {
		module = await import(path);
	} catch (e) {
		console.error(`[TweetDuck] Error loading '${path}': ${e}`);
		return false;
	}
	
	try {
		module.default();
		console.info(`[TweetDuck] Successfully loaded '${path}'`);
		return true;
	} catch (e) {
		console.error(`[TweetDuck] Error executing '${path}': ${e}`);
	}
	
	return false;
}

async function loadFeatures() {
	const script = document.getElementById("tweetduck-bootstrap");
	const features = script.getAttribute("data-features").split("|");
	
	let successes = 0;
	for (const feature of features) {
		if (await loadModule(`./features/${feature}.js`)) {
			++successes;
		}
	}
	
	return [ successes, features.length ];
}

loadFeatures().then(([ successes, total ]) => {
	if ("$TD" in window) {
		window.$TD.onFeaturesLoaded();
	}
	
	if ("TD_PLUGINS" in window) {
		window.TD_PLUGINS.onFeaturesLoaded();
	}
	
	console.info(`[TweetDuck] Successfully loaded ${successes} / ${total} feature(s).`);
});
