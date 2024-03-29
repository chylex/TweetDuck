import { $TD } from "../api/bridge.js";
import { $ } from "../api/jquery.js";
import { onAppReady } from "../api/ready.js";
import { TD } from "../api/td.js";

const introductionHTML = `
<div id="td-introduction-modal" class="ovl scroll-v scroll-styled-v">
  <div class="mdl is-inverted-dark">
    <header class="mdl-header">
      <h3 class="mdl-header-title">Welcome to TweetDuck</h3>
      <a href="#" class="mdl-dismiss link-normal-dark"><i class="icon icon-close"></i></a>
    </header>
    <div class="mdl-inner">
      <div class="mdl-content">
        <p>Thank you for downloading TweetDuck!</p>
        <p><a id="td-introduction-follow" href="#">Follow @TryMyAwesomeApp</a> for latest news and updates about the app.</p>
        <div class="main-menu"></div>
        <p><strong>Right-click anywhere</strong> or click <strong>Settings&nbsp;–&nbsp;TweetDuck</strong> in the left panel to open the main menu. You can also right-click links, tweets, images and videos, and desktop notifications to access their respective context menus.</p>
        <p>Click <strong>Show Guide</strong> to see awesome features TweetDuck offers, or view the guide later by going to <strong>About TweetDuck</strong> and clicking the help button on top.</p>
      </div>
      <footer class="txt-right">
        <button class="Button--primary" data-guide><span class="label">Show Guide</span></button>
        <button class="Button--secondary"><span class="label">Close</span></button>
      </footer>
    </div>
  </div>
</div>`;

/**
 * Shows an introduction dialog.
 */
export default function() {
	const dialog = $(introductionHTML).appendTo(".js-app");
	
	let tdUser = null;
	
	const loadTweetDuckUser = (onSuccess, onError) => {
		if (tdUser !== null) {
			onSuccess(tdUser);
		}
		else {
			TD.controller.clients.getPreferredClient().getUsersByIds([ "957608948189880320" ], users => onSuccess(users[0]), onError);
		}
	};
	
	onAppReady(function loadIntroductionTweetDuckUser() {
		loadTweetDuckUser(user => tdUser = user);
	});
	
	dialog.find("#td-introduction-follow").click(function() {
		loadTweetDuckUser(user => {
			$(document).trigger("uiShowFollowFromOptions", { userToFollow: user });
			
			$(".js-modals-container").find("header a[rel='user']").each(function() {
				this.outerHTML = "TweetDuck";
			});
		}, () => alert("An error occurred when retrieving the account information."));
	});
	
	dialog.find("button, a.mdl-dismiss").click(function() {
		const showGuide = $(this)[0].hasAttribute("data-guide");
		
		dialog.fadeOut(200, function() {
			$TD.onIntroductionClosed(showGuide);
			document.getElementById("tweetduck-styles-introduction-introduction").remove();
			dialog.remove();
		});
	});
};
