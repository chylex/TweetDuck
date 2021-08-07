namespace TweetLib.Api.Data {
	public readonly struct NamespacedResource {
		public Resource Namespace { get; }
		public Resource Path { get; }

		public NamespacedResource(Resource ns, Resource path) {
			Namespace = ns;
			Path = path;
		}

		private bool Equals(NamespacedResource other) {
			return Namespace.Equals(other.Namespace) && Path.Equals(other.Path);
		}

		public override bool Equals(object? obj) {
			return obj is NamespacedResource other && Equals(other);
		}

		public static bool operator ==(NamespacedResource left, NamespacedResource right) {
			return left.Equals(right);
		}

		public static bool operator !=(NamespacedResource left, NamespacedResource right) {
			return !left.Equals(right);
		}

		public override int GetHashCode() {
			unchecked {
				return (Namespace.GetHashCode() * 397) ^ Path.GetHashCode();
			}
		}

		public override string ToString() {
			return $"{Namespace}:{Path}";
		}
	}
}
