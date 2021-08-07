using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TweetLib.Core.Data;
using Timer = System.Timers.Timer;

namespace TweetLib.Core.Systems.Updates {
	public sealed class UpdateHandler : IDisposable {
		public const int CheckCodeUpdatesDisabled = -1;

		private readonly IUpdateCheckClient client;
		private readonly TaskScheduler scheduler;
		private readonly Timer timer;

		public event EventHandler<UpdateCheckEventArgs>? CheckFinished;
		private ushort lastEventId;

		public UpdateHandler(IUpdateCheckClient client, TaskScheduler scheduler) {
			this.client = client;
			this.scheduler = scheduler;

			this.timer = new Timer {
				AutoReset = false,
				Enabled = false
			};

			this.timer.Elapsed += timer_Elapsed;
		}

		public void Dispose() {
			timer.Dispose();
		}

		private void timer_Elapsed(object sender, ElapsedEventArgs e) {
			Check(false);
		}

		public void StartTimer() {
			if (timer.Enabled) {
				return;
			}

			timer.Stop();

			if (client.CanCheck) {
				DateTime now = DateTime.Now;
				TimeSpan nextHour = now.AddSeconds(60 * (60 - now.Minute) - now.Second) - now;

				if (nextHour.TotalMinutes < 15) {
					nextHour = nextHour.Add(TimeSpan.FromHours(1));
				}

				timer.Interval = (int) Math.Ceiling(nextHour.TotalMilliseconds);
				timer.Start();
			}
		}

		public int Check(bool force) {
			if (client.CanCheck || force) {
				int nextEventId = unchecked(++lastEventId);
				Task<UpdateInfo> checkTask = client.Check();

				checkTask.ContinueWith(task => HandleUpdateCheckSuccessful(nextEventId, task.Result), CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, scheduler);
				checkTask.ContinueWith(task => HandleUpdateCheckFailed(nextEventId, task.Exception!.InnerException!), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, scheduler);

				return nextEventId;
			}

			return CheckCodeUpdatesDisabled;
		}

		private void HandleUpdateCheckSuccessful(int eventId, UpdateInfo info) {
			CheckFinished?.Invoke(this, new UpdateCheckEventArgs(eventId, new Result<UpdateInfo>(info)));
		}

		private void HandleUpdateCheckFailed(int eventId, Exception exception) {
			CheckFinished?.Invoke(this, new UpdateCheckEventArgs(eventId, new Result<UpdateInfo>(exception)));
		}
	}
}
