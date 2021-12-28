using System;

namespace TweetLib.Core.Features.Twitter {
	public static class JQuery {
		public static int GetDatePickerDayOfWeek(DayOfWeek dow) {
			return dow switch {
				DayOfWeek.Monday    => 1,
				DayOfWeek.Tuesday   => 2,
				DayOfWeek.Wednesday => 3,
				DayOfWeek.Thursday  => 4,
				DayOfWeek.Friday    => 5,
				DayOfWeek.Saturday  => 6,
				_                   => 0
			};
		}
	}
}
