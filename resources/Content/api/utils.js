/**
 * Throws if an object is missing any property in the chain.
 * @param {Object} obj
 * @param {...string} chain
 */
export function ensurePropertyExists(obj, ...chain) {
	for (const prop of chain) {
		if (obj.hasOwnProperty(prop)) {
			obj = obj[prop];
		}
		else {
			throw "Missing property '" + prop + "' in chain [obj]." + chain.join(".");
		}
	}
}

/**
 * Returns true if an object has every property in the chain.
 * Otherwise, returns false and triggers a debug-only error message.
 * @param {Object} obj
 * @param {...string} chain
 * @returns {boolean}
 */
export function checkPropertyExists(obj, ...chain) {
	try {
		ensurePropertyExists(obj, ...chain);
		return true;
	} catch (err) {
		crashDebug(err);
		return false;
	}
}

/**
 * Reports an error to the console, and also shows an error message if in debug mode.
 */
export function crashDebug(message) {
	console.error(message);
	debugger;
	
	if ("$TD" in window) {
		window.$TD.crashDebug(message);
	}
}

/**
 * No-op function.
 */
export function noop() {}
