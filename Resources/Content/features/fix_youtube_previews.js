import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";

/**
 * Fixes youtu.be previews not showing for https links.
 */
export default function() {
	ensurePropertyExists(TD, "services", "TwitterMedia");
	
	const media = TD.services.TwitterMedia;
	
	ensurePropertyExists(media, "YOUTUBE_TINY_RE");
	ensurePropertyExists(media, "YOUTUBE_LONG_RE");
	ensurePropertyExists(media, "YOUTUBE_RE");
	ensurePropertyExists(media, "SERVICES", "youtube");
	
	media.YOUTUBE_TINY_RE = new RegExp(media.YOUTUBE_TINY_RE.source.replace("http:", "https?:"));
	media.YOUTUBE_RE = new RegExp(media.YOUTUBE_LONG_RE.source + "|" + media.YOUTUBE_TINY_RE.source);
	media.SERVICES["youtube"] = media.YOUTUBE_RE;
};
