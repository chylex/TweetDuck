window["{{bridgename}}"] = (function() {
	const bridge = {};
	const methods = "{{methods}}".split("|");
	
	async function callBridgeMethod(name, args) {
		const jsonArgs = JSON.stringify(args);
		const response = await fetch("https://tweetduck.local/bridge/{{bridgename}}/" + name, {
			method: "POST",
			cache: "no-cache",
			credentials: "omit",
			headers: { "Content-Type": "application/json" },
			body: jsonArgs
		});
		
		const body = await response.text();
		
		if (response.status !== 200) {
			console.error("Error calling {{bridgename}}." + name + " with arguments: " + jsonArgs + "\n" + body);
			throw new Error(body);
		}
		
		return body ? JSON.parse(body) : null;
	}
	
	for (const method of methods) {
		const jsName = method[0].toLowerCase() + method.substring(1);
		
		Object.defineProperty(bridge, jsName, {
			enumerable: true,
			configurable: false,
			writable: false,
			value() {
				return callBridgeMethod(method, Array.from(arguments));
			}
		});
	}
	
	return bridge;
})();
