import { $TD } from "../api/bridge.js";
import { runAfterFunction } from "../api/patch.js";
import { onAppReady } from "../api/ready.js";
import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";
import { getClassStyleProperty } from "./globals/get_class_style_property.js";

function refreshSettings() {
	const fontSizeName = TD.settings.getFontSize();
	const themeName = TD.settings.getTheme();
	
	const doc = document.documentElement;
	doc.setAttribute("data-td-font", fontSizeName);
	doc.setAttribute("data-td-theme", themeName);
	
	// noinspection HtmlMissingClosingTag,HtmlRequiredLangAttribute,HtmlRequiredTitleElement
	const tags = [
		"<html " + Array.prototype.map.call(doc.attributes, ele => `${ele.name}="${ele.value}"`).join(" ") + "><head>"
	];
	
	for (const ele of document.head.querySelectorAll("link[rel='stylesheet']:not([data-td-exclude-notification]),meta[charset]")) {
		tags.push(ele.outerHTML);
	}
	
	tags.push("<style>body { background: " + getClassStyleProperty("column-panel", "background-color") + " !important; }</style>");
	
	$TD.loadNotificationLayout(fontSizeName, tags.join(""));
}

/**
 * Hooks into TweetDeck settings object to detect when the settings change, and update html attributes and notification layout accordingly.
 */
export default function() {
	ensurePropertyExists(TD, "settings", "getFontSize");
	ensurePropertyExists(TD, "settings", "setFontSize");
	ensurePropertyExists(TD, "settings", "getTheme");
	ensurePropertyExists(TD, "settings", "setTheme");
	
	runAfterFunction(TD.settings, "setFontSize", function() {
		setTimeout(refreshSettings, 0);
	});
	
	runAfterFunction(TD.settings, "setTheme", function() {
		setTimeout(refreshSettings, 0);
	});
	
	onAppReady(refreshSettings);
}
