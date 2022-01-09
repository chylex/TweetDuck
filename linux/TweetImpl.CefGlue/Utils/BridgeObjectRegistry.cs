using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TweetImpl.CefGlue.Handlers.Resource;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.Interfaces;
using TweetLib.Utils.Static;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Utils {
	sealed class BridgeObjectRegistry {
		private const string UrlPrefix = "https://tweetduck.local/bridge/";
		private const string BridgeJsFile = "bridge.skeleton.js";

		private static readonly string BridgeJs;

		private static readonly JsonSerializerOptions JsonOptions = new () {
			NumberHandling = JsonNumberHandling.Strict,
			MaxDepth = 1
		};

		static BridgeObjectRegistry() {
			using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TweetImpl.CefGlue.Resources." + BridgeJsFile) ?? throw new IOException("Missing embedded resource: " + BridgeJsFile);
			using var reader = new StreamReader(stream, Encoding.UTF8);
			BridgeJs = reader.ReadToEnd();
		}

		private readonly Dictionary<string, object> objects = new ();

		public void Attach(string name, object obj) {
			objects.Add(name, obj);
		}

		public void RunScripts(IScriptExecutor executor) {
			foreach (var (name, obj) in objects) {
				var methods = obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Select(static methodInfo => methodInfo.Name);
				var script = BridgeJs;
				script = script.Replace("{{bridgename}}", name);
				script = script.Replace("{{methods}}", string.Join('|', methods));
				executor.RunScript("gen:bridge:" + name, script);
			}
		}

		public CefResourceHandler? TryGetHandler(CefRequest request) {
			string url = request.Url;
			if (!url.StartsWithOrdinal(UrlPrefix)) {
				return null;
			}

			var parts = StringUtils.SplitInTwo(url[UrlPrefix.Length..], '/');
			if (parts == null) {
				return null;
			}

			var (objectName, methodName) = parts.Value;
			if (!objects.TryGetValue(objectName, out var obj)) {
				return null;
			}

			var methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			if (methodInfo == null) {
				return Error("Unknown method.", HttpStatusCode.NotFound);
			}

			var post = request.PostData.GetElements();
			JsonElement[]? values;

			try {
				values = post.Length == 1 ? JsonSerializer.Deserialize<JsonElement[]>(post[0].GetBytes(), JsonOptions) : null;
				if (values == null) {
					throw new JsonException();
				}
			} catch (JsonException) {
				return Error("Body must contain a single JSON array.", HttpStatusCode.BadRequest);
			}

			try {
				return TryCall(obj, methodInfo, values);
			} catch (Exception e) {
				return Error("Unexpected error occurred: " + e, HttpStatusCode.InternalServerError);
			}
		}

		private CefResourceHandler TryCall(object obj, MethodInfo methodInfo, JsonElement[] values) {
			var parameters = methodInfo.GetParameters();
			var convertedValues = new object?[parameters.Length];

			for (var i = 0; i < parameters.Length; i++) {
				var parameter = parameters[i];

				if (i < values.Length) {
					JsonElement value = values[i];
					Type parameterType = parameter.ParameterType;

					if (value.ValueKind == JsonValueKind.Null) {
						bool canAssignNull = !parameterType.IsValueType || (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>));
						if (canAssignNull) {
							convertedValues[i] = null;
							continue;
						}
						else {
							return Error("Null provided for non-nullable parameter '" + parameter.Name + "'.", HttpStatusCode.BadRequest);
						}
					}

					var convertedValue = TryConvertValue(value, parameterType);
					if (convertedValue == null) {
						return Error("Invalid value provided for parameter '" + parameter.Name + "' of type '" + parameterType + "'.", HttpStatusCode.BadRequest);
					}
					else {
						convertedValues[i] = convertedValue;
					}
				}
				else if (parameter.HasDefaultValue) {
					convertedValues[i] = parameter.RawDefaultValue;
				}
				else {
					return Error("No value provided for parameter '" + parameter.Name + "' which has no default value.", HttpStatusCode.BadRequest);
				}
			}

			try {
				var resultObj = methodInfo.Invoke(obj, convertedValues);
				var resultBody = resultObj == null ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(JsonSerializer.Serialize(resultObj, JsonOptions));
				return new ByteArrayResourceHandler(new ByteArrayResource(resultBody));
			} catch (TargetInvocationException e) {
				return Error("Method threw an exception: " + e.InnerException!, HttpStatusCode.InternalServerError);
			} catch (Exception e) {
				return Error("An exception occurred: " + e, HttpStatusCode.InternalServerError);
			}
		}

		[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
		private static object? TryConvertValue(JsonElement ele, Type target) {
			if (target == typeof(string)) {
				return ele.GetString();
			}
			else if (target == typeof(int)) {
				return ele.TryGetInt32(out int value) ? value : null;
			}
			else if (target == typeof(bool)) {
				return ele.ValueKind switch {
					JsonValueKind.True  => true,
					JsonValueKind.False => false,
					_                   => null
				};
			}
			else if (target == typeof(IDisposable)) {
				return null; // TODO callbacks
			}
			else {
				throw new NotSupportedException("Unsupported parameter type: " + target);
			}
		}

		private static ByteArrayResourceHandler Error(string message, HttpStatusCode code) {
			return new ByteArrayResourceHandler(new ByteArrayResource(message, Encoding.UTF8, statusCode: code, statusText: "Error"));
		}
	}
}
