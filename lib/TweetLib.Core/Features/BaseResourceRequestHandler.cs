using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TweetLib.Browser.Interfaces;
using TweetLib.Browser.Request;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Features {
	public class BaseResourceRequestHandler : IResourceRequestHandler {
		private static readonly Regex TweetDeckResourceUrl = new (@"/dist/(.*?)\.(.*?)\.(css|js)$");
		private static readonly SortedList<string, string> TweetDeckHashes = new (4);

		public static void LoadResourceRewriteRules(string rules) {
			if (string.IsNullOrEmpty(rules)) {
				return;
			}

			TweetDeckHashes.Clear();

			foreach (string rule in rules.Replace(" ", "").ToLower().Split(',')) {
				var (key, hash) = StringUtils.SplitInTwo(rule, '=') ?? throw new ArgumentException("A rule must have one '=' character: " + rule);

				if (hash.All(static chr => char.IsDigit(chr) || chr is >= 'a' and <= 'f')) {
					TweetDeckHashes.Add(key, hash);
				}
				else {
					throw new ArgumentException("Invalid hash characters: " + rule);
				}
			}
		}

		public virtual RequestHandleResult? Handle(string url, ResourceType resourceType) {
			if (resourceType is ResourceType.Script or ResourceType.Stylesheet && TweetDeckHashes.Count > 0) {
				Match match = TweetDeckResourceUrl.Match(url);

				if (match.Success && TweetDeckHashes.TryGetValue($"{match.Groups[1]}.{match.Groups[3]}", out string hash)) {
					if (match.Groups[2].Value == hash) {
						App.Logger.Debug("[RequestHandlerBase] Accepting " + url);
					}
					else {
						App.Logger.Debug("[RequestHandlerBase] Replacing " + url + " hash with " + hash);
						return new RequestHandleResult.Redirect(TweetDeckResourceUrl.Replace(url, $"/dist/$1.{hash}.$3"));
					}
				}
			}

			return null;
		}
	}
}
