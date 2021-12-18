import { getEvents } from "../api/jquery.js";
import { onAppReady } from "../api/ready.js";
import { TD } from "../api/td.js";
import { checkPropertyExists, noop } from "../api/utils.js";

/**
 * Disables TweetDeck's metrics.
 */
export default function() {
	TD.metrics.inflate = noop;
	TD.metrics.inflateMetricTriple = noop;
	TD.metrics.log = noop;
	TD.metrics.makeKey = noop;
	TD.metrics.send = noop;
	
	onAppReady(function disableMetrics() {
		const events = getEvents(window);
		
		checkPropertyExists(events, "metric");
		checkPropertyExists(events, "metricsFlush");
		
		delete events["metric"];
		delete events["metricsFlush"];
	});
};
