import { $ } from "../api/jquery.js";
import { TD } from "../api/td.js";

const regexTweet = /^https?:\/\/twitter\.com\/[A-Za-z0-9_]+\/status\/(\d+)\/?\??/;
const regexAccount = /^https?:\/\/twitter\.com\/(?!signup$|tos$|privacy$|search$|search-)([^/?]+)\/?$/;

/**
 * Adds drag & drop behavior for dropping tweet or account links on columns to open their detail view.
 */
export default function() {
	let dragType = false;
	
	// noinspection JSUnusedGlobalSymbols
	const events = {
		dragover(e) {
			e.originalEvent.dataTransfer.dropEffect = dragType ? "all" : "none";
			e.preventDefault();
			e.stopPropagation();
		},
		
		drop(e) {
			const url = e.originalEvent.dataTransfer.getData("URL");
			
			if (dragType === "tweet") {
				const match = regexTweet.exec(url);
				
				if (match.length === 2) {
					const column = TD.controller.columnManager.get($(this).attr("data-column"));
					
					if (column) {
						TD.controller.clients.getPreferredClient().show(match[1], function(chirp) {
							TD.ui.updates.showDetailView(column, chirp, column.findChirp(chirp.id) || chirp);
							$(document).trigger("uiGridClearSelection");
						}, function() {
							alert("error|Could not retrieve the requested tweet.");
						});
					}
				}
			}
			else if (dragType === "account") {
				const match = regexAccount.exec(url);
				
				if (match.length === 2) {
					$(document).trigger("uiShowProfile", { id: match[1] });
				}
			}
			
			e.preventDefault();
			e.stopPropagation();
		}
	};
	
	const app = $(".js-app");
	
	const selectors = {
		tweet: "section.js-column",
		account: app
	};
	
	window.TDGF_onGlobalDragStart = function(type, data) {
		if (dragType) {
			app.off(events, selectors[dragType]);
			dragType = null;
		}
		
		if (type === "link") {
			if (regexTweet.test(data)) {
				dragType = "tweet";
			}
			else if (regexAccount.test(data)) {
				dragType = "account";
			}
			else {
				dragType = null;
			}
			
			app.on(events, selectors[dragType]);
		}
	};
};
