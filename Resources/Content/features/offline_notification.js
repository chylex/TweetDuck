const notificationHTML = `
<div id="tweetduck-conn-issues" class="Layer NotificationListLayer">
  <ul class="NotificationList">
    <li class="Notification Notification--red" style="height:63px;">
      <div class="Notification-inner">
        <div class="Notification-icon"><span class="Icon Icon--medium Icon--circleError"></span></div>
        <div class="Notification-content"><div class="Notification-body">Experiencing connection issues</div></div>
        <button type="button" class="Notification-closeButton" aria-label="Close"><span class="Icon Icon--smallest Icon--close" aria-hidden="true"></span></button>
      </div>
    </li>
  </ul>
</div>`;

function fadeOut() {
	const notification = document.getElementById("tweetduck-conn-issues");
	if (!notification || notification.getAnimations().some(anim => anim.id === "fade-out")) {
		return;
	}
	
	const anim = notification?.animate([
		{ opacity: 1 },
		{ opacity: 0 }
	], {
		duration: 200,
		id: "fade-out"
	});
	
	anim.addEventListener("finish", function() {
		notification.remove();
	});
}

/**
 * Adds a notification that appears if the internet is disconnected.
 * Currently it does not notify when the internet is connected but there is no connection to TweetDeck.
 */
export default function() {
	let wasOnline = true;
	
	const onConnectionError = function() {
		if (!wasOnline) {
			return;
		}
		
		wasOnline = false;
		
		document.getElementById("tweetduck-conn-issues")?.remove();
		document.body.insertAdjacentHTML("beforeend", notificationHTML);
		document.querySelector("#tweetduck-conn-issues button").addEventListener("click", function() {
			fadeOut(function(e) {
				e.target.style.opacity = "0";
			});
		});
	};
	
	const onConnectionFine = function() {
		wasOnline = true;
		fadeOut();
	};
	
	window.addEventListener("offline", onConnectionError);
	window.addEventListener("online", onConnectionFine);
};
