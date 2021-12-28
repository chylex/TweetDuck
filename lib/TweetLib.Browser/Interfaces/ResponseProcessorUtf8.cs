using System.Text;

namespace TweetLib.Browser.Interfaces {
	public abstract class ResponseProcessorUtf8 : IResponseProcessor {
		byte[] IResponseProcessor.Process(byte[] response) {
			return Encoding.UTF8.GetBytes(Process(Encoding.UTF8.GetString(response)));
		}

		protected abstract string Process(string response);
	}
}
