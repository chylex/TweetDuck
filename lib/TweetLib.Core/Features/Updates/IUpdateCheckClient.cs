using System.Threading.Tasks;

namespace TweetLib.Core.Features.Updates{
    public interface IUpdateCheckClient{
        bool CanCheck { get; }
        Task<UpdateInfo> Check();
    }
}
