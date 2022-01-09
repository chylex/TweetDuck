using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TweetLib.Utils.Serialization.Converters;
using TweetLib.Utils.Static;

namespace TweetLib.Utils.Serialization {
	public sealed class SimpleObjectSerializer<T> {
		private const string NewLineReal = "\r\n";
		private const string NewLineCustom = "\r~\n";

		private static string EscapeLine(string input) => input.Replace("\\", "\\\\").Replace(Environment.NewLine, "\\\r\n");
		private static string UnescapeLine(string input) => input.Replace(NewLineCustom, Environment.NewLine);

		private static string UnescapeStream(StreamReader reader) {
			string data = reader.ReadToEnd();

			StringBuilder build = new StringBuilder(data.Length);
			int index = 0;

			while (true) {
				int nextIndex = data.IndexOf('\\', index);

				if (nextIndex == -1 || nextIndex + 1 >= data.Length) {
					break;
				}
				else {
					build.Append(data.Substring(index, nextIndex - index));

					char next = data[nextIndex + 1];

					if (next == '\\') { // convert double backslash to single backslash
						build.Append('\\');
						index = nextIndex + 2;
					}
					else if (next == '\r' && nextIndex + 2 < data.Length && data[nextIndex + 2] == '\n') { // convert backslash followed by CRLF to custom new line
						build.Append(NewLineCustom);
						index = nextIndex + 3;
					}
					else { // single backslash
						build.Append('\\');
						index = nextIndex + 1;
					}
				}
			}

			return build.Append(data.Substring(index)).ToString();
		}

		private readonly TypeConverterRegistry converterRegistry;
		private readonly Dictionary<string, PropertyInfo> props;

		public SimpleObjectSerializer(TypeConverterRegistry converterRegistry) {
			this.converterRegistry = converterRegistry;
			this.props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(static prop => prop.CanWrite).ToDictionary(static prop => prop.Name);
		}

		public void Write(string file, T obj) {
			if (props.Count == 0) {
				File.Delete(file);
				return;
			}

			var errors = new LinkedList<string>();

			FileUtils.CreateDirectoryForFile(file);

			using (StreamWriter writer = new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))) {
				foreach (KeyValuePair<string, PropertyInfo> prop in props) {
					var type = prop.Value.PropertyType;
					var converter = converterRegistry.TryGet(type) ?? ClrTypeConverter.Instance;

					if (converter.TryWriteType(type, prop.Value.GetValue(obj), out string? converted)) {
						if (converted != null) {
							writer.Write(prop.Key);
							writer.Write(' ');
							writer.Write(EscapeLine(converted));
							writer.Write(NewLineReal);
						}
					}
					else {
						errors.AddLast($"Missing converter for type: {type}");
					}
				}
			}

			if (errors.First != null) {
				throw new SerializationSoftException(errors.ToArray());
			}
		}

		public void Read(string file, T obj) {
			string contents;

			using (StreamReader reader = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))) {
				contents = UnescapeStream(reader);
			}

			if (string.IsNullOrWhiteSpace(contents)) {
				throw new FormatException("File is empty.");
			}
			else if (contents[0] <= (char) 1) {
				throw new FormatException("Input appears to be a binary file.");
			}

			var errors = new LinkedList<string>();
			int currentPos = 0;

			do {
				string line;
				int nextPos = contents.IndexOf(NewLineReal, currentPos);

				if (nextPos == -1) {
					line = contents.Substring(currentPos);
					currentPos = -1;

					if (string.IsNullOrEmpty(line)) {
						break;
					}
				}
				else {
					line = contents.Substring(currentPos, nextPos - currentPos);
					currentPos = nextPos + NewLineReal.Length;
				}

				int space = line.IndexOf(' ');

				if (space == -1) {
					errors.AddLast($"Missing separator on line: {line}");
					continue;
				}

				string property = line.Substring(0, space);
				string value = UnescapeLine(line.Substring(space + 1));

				if (props.TryGetValue(property, out PropertyInfo info)) {
					var type = info.PropertyType;
					var converter = converterRegistry.TryGet(type) ?? ClrTypeConverter.Instance;

					if (converter.TryReadType(type, value, out object? converted)) {
						info.SetValue(obj, converted);
					}
					else {
						errors.AddLast($"Failed reading property {property} with value: {value}");
					}
				}
			} while (currentPos != -1);

			if (errors.First != null) {
				throw new SerializationSoftException(errors.ToArray());
			}
		}

		public void ReadIfExists(string file, T obj) {
			try {
				Read(file, obj);
			} catch (FileNotFoundException) {
				// ignore
			} catch (DirectoryNotFoundException) {
				// ignore
			}
		}
	}
}
