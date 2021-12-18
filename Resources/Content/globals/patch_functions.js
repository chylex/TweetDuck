import { crashDebug } from "../api/utils.js";

/**
 * @callback FunctionReplacementCallback
 * @param {function} func original function
 * @param {*[]} args original arguments
 * @returns {*} the value to return from the replaced function
 */

/**
 * Replaces a function `ownerObject.functionName` with a callback.
 * The callback is bound to the original function's `this`.
 * @param {Object} ownerObject
 * @param {string} functionName
 * @param {FunctionReplacementCallback} callback
 * @returns {boolean} whether the replacement was successful
 */
export function replaceFunction(ownerObject, functionName, callback) {
	const originalFunction = ownerObject[functionName];
	
	if (typeof originalFunction !== "function") {
		crashDebug("Missing function '" + functionName + "' for replacement!");
		return false;
	}
	
	ownerObject[functionName] = function() {
		return callback.call(this, originalFunction, arguments);
	};
	
	return true;
}

/**
 * Replaces a function `ownerObject.functionName` with one that calls the provided callback after the original function returns.
 * The callback is bound to the original function's `this`.
 * Anything the callback returns is ignored.
 * @param {Object} ownerObject
 * @param {string} functionName
 * @param {function} callback
 * @returns {boolean} whether the replacement was successful
 */
export function runAfterFunction(ownerObject, functionName, callback) {
	return replaceFunction(ownerObject, functionName, function(func, args) {
		const result = func.apply(this, args);
		callback.call(this, func, args);
		return result;
	});
}
