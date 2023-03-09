import { $TD } from "../api/bridge.js";

export default function() {
	if (!("TD" in window)) {
		$TD.alert("warning", "Some TweetDuck features failed to load. This might happen if your Twitter account is enrolled into the TweetDeck Preview, which TweetDuck does not support. Try opting out of the TweetDeck Preview to restore TweetDuck's functionality.");
	}
}
