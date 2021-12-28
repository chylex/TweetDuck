namespace TweetLib.Browser.Interfaces {
	public interface IResponseProcessor {
		byte[] Process(byte[] response);
	}
}
