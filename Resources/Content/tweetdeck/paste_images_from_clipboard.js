import { $, getEvents } from "../api/jquery.js";
import { onAppReady } from "../api/ready.js";
import { ensurePropertyExists } from "../api/utils.js";

/**
 * @returns {{ addFilesToUpload: function(File[]) }}
 */
function getUploader() {
	const uploader = getEvents(document)["uiComposeAddImageClick"][0].handler.context;
	ensurePropertyExists(Object.getPrototypeOf(uploader), "addFilesToUpload");
	return uploader;
}

/**
 * Allows pasting images from clipboard when editing a tweet or DM.
 */
export default function() {
	onAppReady(function pasteImagesFromClipboard() {
		const uploader = getUploader();
		
		$(".js-app").delegate(".js-compose-text,.js-reply-tweetbox,.td-detect-image-paste", "paste", function(e) {
			// noinspection JSValidateTypes
			/** @type {{ clipboardData: DataTransfer }} */
			const originalEvent = e.originalEvent;
			
			for (const item of originalEvent.clipboardData.items) {
				if (item.type.startsWith("image/")) {
					const popoutDM = $(this).closest(".rpl").find(".js-reply-popout");
					
					if (popoutDM.length) {
						popoutDM.click();
					}
					else if ($(".js-add-image-button").is(".is-disabled")) {
						// If we're already in composer and uploading additional pictures, check if the upload button is enabled,
						// because the uploader object does not check for invalid state such as too many files.
						return;
					}
					
					uploader.addFilesToUpload([ item.getAsFile() ]);
					break;
				}
			}
		});
	});
};
