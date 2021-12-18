import { $ } from "./jquery.js";
import { TD } from "./td.js";
import { crashDebug } from "./utils.js";

const callbacks = [];

/**
 * @param {function} callback
 */
function executeCallback(callback) {
	try {
		callback();
	} catch (e) {
		crashDebug("Caught error in function " + callback.name);
		console.error(e);
	}
}

/**
 * @returns {boolean}
 */
export function isAppReady() {
	return TD.ready;
}

/**
 * @param {function} callback
 */
export function onAppReady(callback) {
	if (isAppReady()) {
		executeCallback(callback);
	}
	else {
		callbacks.push(callback);
	}
}

$(document).one("TD.ready", function() {
	callbacks.forEach(executeCallback);
	callbacks.length = 0;
});
