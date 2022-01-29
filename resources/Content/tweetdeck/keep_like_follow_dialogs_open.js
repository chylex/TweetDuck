import { $TDX } from "../api/bridge.js";
import { $, getEvents } from "../api/jquery.js";

/**
 * Keeps the Like/Follow dialogs open if enabled in the app configuration.
 */
export default function() {
	const prevSetTimeout = window.setTimeout;
	
	const overrideState = function() {
		if (!$TDX.keepLikeFollowDialogsOpen) {
			return;
		}
		
		window.setTimeout = function(func, timeout) {
			return timeout !== 500 && prevSetTimeout.apply(this, arguments);
		};
	};
	
	const restoreState = function(context, key) {
		window.setTimeout = prevSetTimeout;
		
		if ($TDX.keepLikeFollowDialogsOpen && key in context.state) {
			context.state[key] = false;
		}
	};
	
	$(document).on("uiShowFavoriteFromOptions", function() {
		$(".js-btn-fav", ".js-modal-inner").each(function() {
			const event = getEvents(this).click[0];
			const handler = event.handler;
			
			event.handler = function() {
				overrideState();
				handler.apply(this, arguments);
				restoreState(getEvents(document)["dataFavoriteState"][0].handler.context, "stopSubsequentLikes");
			};
		});
	});
	
	$(document).on("uiShowFollowFromOptions", function() {
		$(".js-component", ".js-modal-inner").each(function() {
			const event = getEvents(this).click[0];
			const handler = event.handler;
			const context = handler.context;
			
			event.handler = function() {
				overrideState();
				handler.apply(this, arguments);
				restoreState(context, "stopSubsequentFollows");
			};
		});
	});
};
