import { $TD } from "../api/bridge.js";

if (!("$TDP" in window)) {
	throw "Missing $TDP in global scope";
}

/**
 * @type {Object}
 * @property {function(token: number, path: string): Promise<boolean>} checkFileExists
 * @property {function(token: number, path: string, cache: boolean): Promise<string>} readFile
 * @property {function(token: number, path: string): Promise<string>} readFileRoot
 * @property {function(token: number)} setConfigurable
 * @property {function(token: number, path: string, contents: string): Promise<string>} writeFile
 */
export const $TDP = window.$TDP;

/**
 * @typedef TweetDuckPlugin
 * @type {Object}
 *
 * @property {string} $id
 * @property {number} $token
 * @property {Storage} [$storage]
 * @property {function} [configure]
 */

/**
 * Validates that a plugin object contains a token.
 * @param {TweetDuckPlugin} pluginObject
 */
export function validatePluginObject(pluginObject) {
	if (!("$token" in pluginObject)) {
		throw "Invalid plugin object.";
	}
}

/**
 * Loads a simple JavaScript object as configuration.
 * @param {TweetDuckPlugin} pluginObject
 * @param {string} fileNameUser
 * @param {string|null} fileNameDefault
 * @param {function(Object)} onSuccess
 * @param {function(Error)} onFailure
 */
export function loadConfigurationFile(pluginObject, fileNameUser, fileNameDefault, onSuccess, onFailure) {
	validatePluginObject(pluginObject);
	
	const identifier = pluginObject.$id;
	const token = pluginObject.$token;
	
	$TDP.checkFileExists(token, fileNameUser).then(exists => {
		/** @type {string|null} */
		const fileName = exists ? fileNameUser : fileNameDefault;
		
		if (fileName === null) {
			onSuccess && onSuccess({});
			return;
		}
		
		(exists ? $TDP.readFile(token, fileName, true) : $TDP.readFileRoot(token, fileName)).then(contents => {
			let obj;
			
			try {
				// noinspection DynamicallyGeneratedCodeJS
				obj = eval("(" + contents + ")");
			} catch (err) {
				if (!(onFailure && onFailure(err))) {
					$TD.alert("warning", "Problem loading '" + fileName + "' file for '" + identifier + "' plugin, the JavaScript syntax is invalid: " + err.message);
				}
				
				return;
			}
			
			onSuccess && onSuccess(obj);
		}).catch(err => {
			if (!(onFailure && onFailure(err))) {
				$TD.alert("warning", "Problem loading '" + fileName + "' file for '" + identifier + "' plugin: " + err.message);
			}
		});
	}).catch(err => {
		if (!(onFailure && onFailure(err))) {
			$TD.alert("warning", "Problem checking '" + fileNameUser + "' file for '" + identifier + "' plugin: " + err.message);
		}
	});
}

/**
 * Creates and returns an object for managing a custom stylesheet.
 * @param {TweetDuckPlugin} pluginObject
 * @returns {{insert: (function(string): number), remove: (function(): void), element: HTMLStyleElement}}
 */
export function createCustomStyle(pluginObject) {
	validatePluginObject(pluginObject);
	
	const element = document.createElement("style");
	element.id = "plugin-" + pluginObject.$id + "-" + Math.random().toString(36).substring(2, 7);
	document.head.appendChild(element);
	
	return {
		insert: (rule) => element.sheet.insertRule(rule, 0),
		remove: () => element.remove(),
		element
	};
}

/**
 * Returns a function that mimics a Storage object that will be saved in the plugin.
 * @param {TweetDuckPlugin} pluginObject
 * @param {function(Storage)} onReady
 */
export function createStorage(pluginObject, onReady) {
	validatePluginObject(pluginObject);
	
	if ("$storage" in pluginObject) {
		if (pluginObject.$storage !== null) { // set to null while the file is still loading
			onReady(pluginObject.$storage);
		}
		
		return;
	}
	
	class Storage {
		get length() {
			return Object.keys(this).length;
		}
		
		key(index) {
			return Object.keys(this)[index];
		}
		
		// noinspection JSUnusedGlobalSymbols
		getItem(key) {
			return this[key] || null;
		}
		
		setItem(key, value) {
			this[key] = value;
			updateFile();
		}
		
		removeItem(key) {
			delete this[key];
			updateFile();
		}
		
		clear() {
			for (const key of Object.keys(this)) {
				delete this[key];
			}
			
			updateFile();
		}
		
		replace(obj, silent) {
			for (const key of Object.keys(this)) {
				delete this[key];
			}
			
			for (const key in obj) {
				this[key] = obj[key];
			}
			
			if (!silent) {
				updateFile();
			}
		}
	}
	
	// noinspection JSUnusedLocalSymbols,JSUnusedGlobalSymbols
	const handler = {
		get(obj, prop, receiver) {
			const value = obj[prop];
			return typeof value === "function" ? value.bind(obj) : value;
		},
		
		set(obj, prop, value) {
			obj.setItem(prop, value);
			return true;
		},
		
		deleteProperty(obj, prop) {
			obj.removeItem(prop);
			return true;
		},
		
		enumerate(obj) {
			return Object.keys(obj);
		}
	};
	
	const storage = new Proxy(new Storage(), handler);
	let delay = -1;
	
	const updateFile = function() {
		window.clearTimeout(delay);
		
		delay = window.setTimeout(function() {
			// noinspection JSIgnoredPromiseFromCall
			$TDP.writeFile(pluginObject.$token, ".storage", JSON.stringify(storage));
		}, 0);
	};
	
	pluginObject.$storage = null;
	
	loadConfigurationFile(pluginObject, ".storage", null, function(obj) {
		storage.replace(obj, true);
		onReady(pluginObject.$storage = storage);
	}, function() {
		onReady(pluginObject.$storage = storage);
	});
}
