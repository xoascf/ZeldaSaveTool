using System.Text;

namespace ZeldaSaveTool.Utility;

/// <summary>
/// A lightweight, dependency-free JSON parsing engine.
/// Parses a JSON string into nested standard collections: Dictionary<string, object> and List<object>.
/// </summary>
internal static class LiteJsonParser {
	public static object? Parse(string json) {
		if (string.IsNullOrEmpty(json))
			return null;

		Parser parser = new(json);
		return parser.ParseValue();
	}

	private class Parser {
		private readonly string json;
		private readonly int length;
		private int index;

		public Parser(string json) {
			this.json = json;
			this.length = json.Length;
			this.index = 0;
		}

		public object? ParseValue() {
			SkipWhitespace();
			if (index >= length) return null;

			char c = json[index];
			switch (c) {
				case '{': return ParseObject();
				case '[': return ParseArray();
				case '"': return ParseString();
				case 't':
				case 'f': return ParseBool();
				case 'n': return ParseNull();
				default:
					if (c == '-' || (c >= '0' && c <= '9'))
						return ParseNumber();
					return null; // Silent skip on malformed data
			}
		}

		private Dictionary<string, object> ParseObject() {
			index++;
			Dictionary<string, object> dict = new(StringComparer.OrdinalIgnoreCase);
			bool expectComma = false;

			while (index < length) {
				SkipWhitespace();
				if (index >= length) break;

				char c = json[index];
				if (c == '}') {
					index++;
					return dict;
				}

				if (expectComma) {
					if (c == ',') { index++; SkipWhitespace(); expectComma = false; continue; }
					else break;
				}

				if (c != '"') break;

				string key = ParseString();
				SkipWhitespace();

				if (index >= length || json[index] != ':') break;
				index++;

				object? value = ParseValue();
				if (value != null) {
					dict[key] = value;
				}
				expectComma = true;
			}
			return dict;
		}

		private List<object> ParseArray() {
			index++;
			List<object> list = new();
			bool expectComma = false;

			while (index < length) {
				SkipWhitespace();
				if (index >= length) break;

				char c = json[index];
				if (c == ']') {
					index++;
					return list;
				}

				if (expectComma) {
					if (c == ',') { index++; SkipWhitespace(); expectComma = false; continue; }
					else break;
				}

				object? val = ParseValue();
				if (val != null) {
					list.Add(val);
				}
				expectComma = true;
			}
			return list;
		}

		private string ParseString() {
			index++; // Skip opening '"'
			int start = index;
			bool hasEscape = false;

			while (index < length) {
				char c = json[index];
				if (c == '"') {
					// Allocation-free slice if no escape characters exist
					string result = hasEscape
						? ParseEscapedString(start)
						: json.Substring(start, index - start);
					index++; // Skip closing '"'
					return result;
				}
				if (c == '\\') hasEscape = true;
				index++;
			}
			return string.Empty;
		}

		private string ParseEscapedString(int start) {
			index = start;
			StringBuilder sb = new();

			while (index < length) {
				char c = json[index++];
				if (c == '"') {
					index--;
					return sb.ToString();
				}
				if (c == '\\' && index < length) {
					char escape = json[index++];
					switch (escape) {
						case '"': sb.Append('"'); break;
						case '\\': sb.Append('\\'); break;
						case '/': sb.Append('/'); break;
						case 'b': sb.Append('\b'); break;
						case 'f': sb.Append('\f'); break;
						case 'n': sb.Append('\n'); break;
						case 'r': sb.Append('\r'); break;
						case 't': sb.Append('\t'); break;
						case 'u':
							if (index + 4 <= length) {
								string hex = json.Substring(index, 4);
								index += 4;
								ushort code;
								if (ushort.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code))
									sb.Append((char)code);
							}
							break;
					}
				}
				else {
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		private double ParseNumber() {
			int start = index;
			while (index < length) {
				char c = json[index];
				if ((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
					index++;
				else
					break;
			}
			string numStr = json.Substring(start, index - start);
			double val;
			double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out val);
			return val;
		}

		private bool ParseBool() {
			if (index + 4 <= length && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e') {
				index += 4;
				return true;
			}
			if (index + 5 <= length && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e') {
				index += 5;
				return false;
			}
			index++;
			return false;
		}

		private object? ParseNull() {
			if (index + 4 <= length && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
				index += 4;
			else
				index++;
			return null;
		}

		private void SkipWhitespace() {
			while (index < length) {
				char c = json[index];
				if (c == ' ' || c == '\t' || c == '\n' || c == '\r') index++;
				else break;
			}
		}
	}
}