import { TD } from "../api/td.js";
import { checkPropertyExists, noop } from "../api/utils.js";

function isSupported() {
	return checkPropertyExists(TD, "controller", "columnManager", "getAll");
}

function reloadColumnsImpl() {
	Object.values(TD.controller.columnManager.getAll()).forEach(column => column.reloadTweets());
}

export const reloadColumns = isSupported() ? reloadColumnsImpl : noop;
