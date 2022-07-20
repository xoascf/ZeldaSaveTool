/* Licensed under the Open Software License version 3.0 */

namespace ZeldaSaveTool;

internal class Strings
{
	public const string Ver = "0.1";
	public const string PVer = $"{Ver}b";

	private static int _latestLoadedDictionary;
	private static readonly string[] Languages = Enum.GetNames(typeof(Language));

	/* Localized strings. */
	private static readonly Dictionary<string, string> LDict = new();

	private static readonly string? SystemLang =
		CultureInfo.CurrentUICulture.Parent.TwoLetterISOLanguageName;

	public static void SetNextLanguage()
	{
		var lang = (Language)_latestLoadedDictionary;
		if (lang >= (Language)Languages.Length - 1)
			lang = 0;
		else
			lang++;

		LoadDictionary(lang);
	}

	private static Language GetLanguage(string? lang)
	{
		if (lang != null && Enum.IsDefined(typeof(Language), lang))
			return (Language)Enum.Parse(typeof(Language), lang);

		return Language.en;
	}

	public static void SetLocalizedStrings(string? lang)
	{
		if (string.IsNullOrEmpty(lang))
			lang = SystemLang;

		LoadDictionary(GetLanguage(lang));
	}

	public static string _(params string[] text)
	{ /* Get localized string! */
		var str = text[0];
		if (LDict.ContainsKey(str))
			str = text.Length switch
			{
				1 => LDict[text[0]],
				2 => string.Format(LDict[text[0]], text[1]),
				_ => str
			};

		return str;
	}

	private static void LoadDictionary(Language lang)
	{
		var resString = Resources.ResourceManager.GetString(lang.ToString());

		if (resString != null)
		{
			var list = new List<string>(resString.Split('\n'));

			foreach (var line in list)
			{
				if (string.IsNullOrEmpty(line) || !line.Contains("="))
					continue;

				var keyAndValue = line.Split(new[] { '=' }, 2);

				LDict.Remove(keyAndValue[0]);
				LDict.Add(keyAndValue[0], keyAndValue[1]);
			}
		}

		_latestLoadedDictionary = (int)lang;
	}

	private enum Language
	{
		en,
		es
	}
}