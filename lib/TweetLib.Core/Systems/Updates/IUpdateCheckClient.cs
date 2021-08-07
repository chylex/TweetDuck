using System.Threading.Tasks;

namespace TweetLib.Core.Systems.Updates {
	public interface IUpdateCheckClient {
		bool CanCheck { get; }
		Task<UpdateInfo> Check();
	}
}
