import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";
import { replaceFunction } from "../globals/patch_functions.js";

const formatRegex = /\?.*format=(\w+)/;

/**
 * @param {string} url
 * @return {string}
 */
function fixPreviewURL(url) {
	if (url.startsWith("https://ton.twitter.com/1.1/ton/data/dm/") || url.startsWith("https://pbs.twimg.com/tweet_video_thumb/")) {
		const format = url.match(formatRegex);
		
		if (format?.length === 2) {
			const fix = `.${format[1]}?`;
			
			if (!url.includes(fix)) {
				return url.replace("?", fix);
			}
		}
	}
	
	return url;
}

/**
 * Fixes DM image previews and GIF thumbnails not loading due to new URLs.
 */
export default function() {
	ensurePropertyExists(TD, "services", "TwitterMedia", "prototype");
	
	replaceFunction(TD.services.TwitterMedia.prototype, "getTwitterPreviewUrl", function(func, args) {
		return fixPreviewURL(func.apply(this, args));
	});
};
