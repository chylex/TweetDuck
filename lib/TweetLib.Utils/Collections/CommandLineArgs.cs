﻿using System.Collections.Generic;
using System.Text;

namespace TweetLib.Utils.Collections {
	/// <summary>
	/// Represents a collection of command line arguments, which can contain flag properties with no values, and valued properties with text values.
	/// </summary>
	public sealed class CommandLineArgs {
		public static CommandLineArgs FromStringArray(char entryChar, string[] array) {
			CommandLineArgs args = new CommandLineArgs();
			ReadStringArray(entryChar, array, args);
			return args;
		}

		private static void ReadStringArray(char entryChar, string[] array, CommandLineArgs targetArgs) {
			for (int index = 0; index < array.Length; index++) {
				string entry = array[index];

				if (entry.Length > 0 && entry[0] == entryChar) {
					if (index < array.Length - 1) {
						string potentialValue = array[index + 1];

						if (potentialValue.Length > 0 && potentialValue[0] == entryChar) {
							targetArgs.AddFlag(entry);
						}
						else {
							targetArgs.SetValue(entry, potentialValue);
							++index;
						}
					}
					else {
						targetArgs.AddFlag(entry);
					}
				}
			}
		}

		private readonly HashSet<string> flags = new ();
		private readonly Dictionary<string, string> values = new ();

		public int Count => flags.Count + values.Count;

		public void AddFlag(string flag) {
			flags.Add(flag.ToLower());
		}

		public bool HasFlag(string flag) {
			return flags.Contains(flag.ToLower());
		}

		public void RemoveFlag(string flag) {
			flags.Remove(flag.ToLower());
		}

		public void SetValue(string key, string value) {
			values[key.ToLower()] = value;
		}

		public string? GetValue(string key) {
			return values.TryGetValue(key.ToLower(), out var val) ? val : null;
		}

		public void RemoveValue(string key) {
			values.Remove(key.ToLower());
		}

		public CommandLineArgs Clone() {
			CommandLineArgs copy = new CommandLineArgs();

			foreach (string flag in flags) {
				copy.AddFlag(flag);
			}

			foreach (var kvp in values) {
				copy.SetValue(kvp.Key, kvp.Value);
			}

			return copy;
		}

		public void ToDictionary(IDictionary<string, string> target) {
			foreach (string flag in flags) {
				target[flag] = "1";
			}

			foreach (var kvp in values) {
				target[kvp.Key] = kvp.Value;
			}
		}

		public override string ToString() {
			StringBuilder build = new StringBuilder();

			foreach (string flag in flags) {
				build.Append(flag).Append(' ');
			}

			foreach (var kvp in values) {
				build.Append(kvp.Key).Append(" \"").Append(kvp.Value).Append("\" ");
			}

			return build.Length == 0 ? string.Empty : build.Remove(build.Length - 1, 1).ToString();
		}
	}
}
