﻿using System;

namespace TweetLib.Core.Application {
	public interface IAppErrorHandler {
		void HandleException(string caption, string message, bool canIgnore, Exception e);
	}
}
