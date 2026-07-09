namespace ZeldaSaveTool {
	internal class Charset {
		public enum Chars /* Digits + Basic Latin Alphabet + ?. */ {
			/* Numbers. */
			N0, N1, N2, N3, N4, N5, N6, N7, N8, N9,
			/* Uppercase letter set. */
			AaA, AaB, AaC, AaD, AaE, AaF, AaG, AaH, AaI, AaJ, AaK, AaL, AaM,
			AaN, AaO, AaP, AaQ, AaR, AaS, AaT, AaU, AaV, AaW, AaX, AaY, AaZ,
			/* Lowercase letter set. */
			Aaa, Aab, Aac, Aad, Aae, Aaf, Aag, Aah, Aai, Aaj, Aak, Aal, Aam,
			Aan, Aao, Aap, Aaq, Aar, Aas, Aat, Aau, Aav, Aaw, Aax, Aay, Aaz,
			/* Punctuation. */
			Space, Dash, Dot,
			/* PAL ends here. */
			Unk0,
			/* At 171 begins the Basic Latin Alphabet for NTSC. */
			NtscLatin = 171 - AaA,
			NtscDash = 228,
			NtscDot = 234,
		};

		private static readonly string[] JapaneseChars = {
			"あ", "い", "う", "え", "お", "か", "き", "く", "け", "こ",
			"さ", "し", "す", "せ", "そ", "た", "ち", "つ", "て", "と",
			"な", "に", "ぬ", "ね", "の", "は", "ひ", "ふ", "へ", "ほ",
			"ま", "み", "む", "め", "も", "や", "ゆ", "よ", "ら", "り",
			"る", "れ", "ろ", "わ", "を", "ん", "ぁ", "ぃ", "ぅ", "ぇ",
			"ぉ", "っ", "ゃ", "ゅ", "ょ", "が", "ぎ", "ぐ", "げ", "ご",
			"ざ", "じ", "ず", "ぜ", "ぞ", "だ", "ぢ", "づ", "で", "ど",
			"ば", "び", "ぶ", "べ", "ぼ", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ",
			"ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ",
			"サ", "シ", "ス", "セ", "ソ", "タ", "チ", "ツ", "テ", "ト",
			"ナ", "ニ", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ",
			"マ", "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ",
			"ル", "レ", "ロ", "ワ", "ヲ", "ン", "ァ", "ィ", "ゥ", "ェ",
			"ォ", "ッ", "ャ", "ュ", "ョ", "ガ", "ギ", "グ", "ゲ", "ゴ",
			"ザ", "ジ", "ズ", "ゼ", "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド",
			"バ", "ビ", "ブ", "ベ", "ボ", "パ", "ピ", "プ", "ペ", "ポ",
			"ヴ"
		};

		public static string GetReadableName(byte[] nameBytes) {
			string name = "";
			bool isJapanese = false;
			foreach (byte b in nameBytes) {
				if (b >= 65 && b <= 171) {
					isJapanese = true;
					break;
				}
			}

			foreach (byte nameByte in nameBytes) {
				if (isJapanese && nameByte >= 10 && nameByte <= 171) {
					name += JapaneseChars[nameByte - 10];
					continue;
				}
				
				if (!Enum.IsDefined(typeof(Chars), (int)nameByte))
					continue;

				string nameChar;

				if ((Chars)nameByte >= Chars.Space)
					nameChar = (Chars)nameByte switch {
						Chars.Space => " ",
						Chars.Dash => "-",
						Chars.Dot => ".",
						_ => ""
					};
				else
					nameChar = ((Chars)nameByte).ToString();

				name += nameChar.Substring(nameChar.Length - 1);
			}

			return name.TrimEnd();
		}

		public static byte[] GetNameBytes(string name, bool toNtsc = false) {
			byte[] nameBytes = new byte[8];
			for (int i = 0; i < nameBytes.Length; i++) {
				nameBytes[i] = (byte)Chars.Space;
				if (toNtsc) nameBytes[i] += (byte)Chars.NtscLatin;
			}

			for (int index = 0; index < name.Length; index++) {
				char c = name[index];

				if (c is >= '0' and <= '9') {
					nameBytes[index] = (byte)char.GetNumericValue(c);
				} else {
					int jpIndex = Array.IndexOf(JapaneseChars, c.ToString());
					if (jpIndex != -1) {
						nameBytes[index] = (byte)(jpIndex + 10);
					} else if (Enum.IsDefined(typeof(Chars), "Aa" + c)) {
						byte b = (byte)(int)Enum.Parse(typeof(Chars), "Aa" + c);
						if (toNtsc) b += (byte)Chars.NtscLatin;
						nameBytes[index] = b;
					} else {
						byte b = c switch {
							' ' => (byte)Chars.Space,
							'-' => (byte)Chars.Dash,
							'.' => (byte)Chars.Dot,
							_ => (byte)Chars.Space
						};
						if (toNtsc) {
							if (b == (byte)Chars.Dash) b = (byte)Chars.NtscDash;
							else if (b == (byte)Chars.Dot) b = (byte)Chars.NtscDot;
							else if (b == (byte)Chars.Space) b += (byte)Chars.NtscLatin; // Space shifts like letters
						}
						nameBytes[index] = b;
					}
				}
			}

			return nameBytes;
		}
	}
}