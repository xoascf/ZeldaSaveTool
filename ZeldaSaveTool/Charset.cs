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

		public static string GetReadableName(byte[] nameBytes) {
			string name = "";
			foreach (byte nameByte in nameBytes) {
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

		public static byte[] GetNameBytes(string name) {
			byte[] nameBytes = new byte[8];
			for (int i = 0; i < nameBytes.Length; i++)
				nameBytes[i] = (byte)Chars.Space;

			for (int index = 0; index < name.Length; index++) {
				char c = name[index];

				if (c is >= '0' and <= '9')
					nameBytes[index] = (byte)char.GetNumericValue(c);
				else if (Enum.IsDefined(typeof(Chars), "Aa" + c))
					nameBytes[index] =
					(byte)(int)Enum.Parse(typeof(Chars), "Aa" + c);
				else
					nameBytes[index] = c switch {
						' ' => (byte)Chars.Space,
						'-' => (byte)Chars.Dash,
						'.' => (byte)Chars.Dot,
						_ => (byte)Chars.Space
					};
			}

			return nameBytes;
		}
	}
}