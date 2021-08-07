using System;
using System.Text.RegularExpressions;

namespace TweetLib.Api.Data {
	public readonly struct Resource {
		private const string ValidCharacterPattern = "^[a-z0-9_]+$";
		private static readonly Regex ValidCharacterRegex = new Regex(ValidCharacterPattern, RegexOptions.Compiled);

		public string Name { get; }

		public Resource(string name) {
			if (!ValidCharacterRegex.IsMatch(name)) {
				throw new ArgumentException("Resource name must match the regex: " + ValidCharacterPattern);
			}

			Name = name;
		}

		private bool Equals(Resource other) {
			return Name == other.Name;
		}

		public override bool Equals(object? obj) {
			return obj is Resource other && Equals(other);
		}

		public static bool operator ==(Resource left, Resource right) {
			return left.Equals(right);
		}

		public static bool operator !=(Resource left, Resource right) {
			return !left.Equals(right);
		}

		public override int GetHashCode() {
			return Name.GetHashCode();
		}

		public override string ToString() {
			return Name;
		}
	}
}
