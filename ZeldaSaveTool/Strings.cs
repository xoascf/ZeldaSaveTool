/* Licensed under the Open Software License version 3.0 */

namespace ZeldaSaveTool;

internal class Strings {
	public const string Ver = "0.5.9";
	public const string PVer = $"{Ver}b";
	public const string PName = "Zelda Save Tool";

	public static readonly byte[] Zero = { 0x00 };

	private static int _latestLoadedDictionary;
	private static readonly string[] Languages = Enum.GetNames(typeof(Language));

	/* Localized strings. */
	private static readonly Dictionary<string, string> LDict = new();

	public static void SetNextLanguage() {
		Language lang = (Language)_latestLoadedDictionary;
		if (lang >= (Language)Languages.Length - 1)
			lang = 0;
		else
			lang++;

		LDict.Clear();
		LoadDictionary(Language.en);
		LoadDictionary(lang);
	}

	private static Language GetLanguage(string? lang) {
		if (lang != null && Enum.IsDefined(typeof(Language), lang))
			return (Language)Enum.Parse(typeof(Language), lang);

		return Language.en;
	}

	public static void SetLocalizedStrings(string? lang) {
		if (string.IsNullOrEmpty(lang))
			lang = CultureInfo.CurrentUICulture.Parent.TwoLetterISOLanguageName;

		LoadDictionary(Language.en);
		LoadDictionary(GetLanguage(lang));
	}

	public static string T(params string[] text) { /* Get localized string! */
		string str = text[0];
		if (LDict.ContainsKey(str))
			str = text.Length switch {
				1 => LDict[text[0]],
				2 => string.Format(LDict[text[0]], text[1]),
				_ => str
			};

		return str;
	}

	private static void LoadDictionary(Language lang) {
		if (R.ResourceManager.GetString(lang.ToString()) is { } resString) {
			List<string> list = new(resString.Split('\n'));

			foreach (string line in list) {
				if (string.IsNullOrEmpty(line) || !line.Contains("="))
					continue;

				string[] keyAndValue = line.Split(new[] { '=' }, 2);

				LDict[keyAndValue[0]] = keyAndValue[1].Replace("\\n", "\n");
			}
		}

		_latestLoadedDictionary = (int)lang;
	}

	private enum Language {
		en, de, fr, es,
	}
}