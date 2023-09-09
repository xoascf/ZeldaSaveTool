/* Licensed under the Open Software License version 3.0 */

using System.Drawing;
using System.Drawing.Imaging;

namespace ZeldaSaveTool.Utility;

internal static class Forms {
	private static readonly Dictionary<string, string> SDict = new();

	private static bool HasUsableTextProperty(IDisposable ctr) {
		return ctr is Form or Label or Button or CheckBox or ComboBox or GroupBox;
	}

	public static void UpdateText(Control ctrMain) {
		ApplyUpdatedText(ctrMain);

		foreach (Control ctr in ctrMain.Controls) {
			if (ctr.HasChildren)
				UpdateText(ctr);
			else
				ApplyUpdatedText(ctr);
		}
	}

	private static void ApplyUpdatedText(Control ctr) {
		if (!HasUsableTextProperty(ctr))
			return;

		string name = ctr.Name;
		string og = ctr.Text;

		if (!SDict.ContainsKey(name))
			SDict.Add(name, og);

		ctr.Text = T(SDict[name]);
	}

	public static void Localize(this ComboBox cmb, Array a, int defaultIndex = 0) {
		bool setDefault = cmb.SelectedIndex == -1;
		int selected = defaultIndex;
		if (!setDefault)
			selected = cmb.SelectedIndex;

		List<string> list = new();

		foreach (object? item in a)
			list.Add(T(item.ToString()));

		cmb.DataSource = list;
		cmb.SelectedIndex = setDefault ? defaultIndex : selected;
	}

	public class Tint {
		private readonly List<ColorMap> _cm = new();
		private readonly ImageAttributes _ia = new();

		public void Replace(Color from, Color to)
			=> _cm.Add(new() { OldColor = from, NewColor = to });

		public ImageAttributes GetImageAttributes() {
			_ia.SetRemapTable(_cm.ToArray());
			return _ia;
		}
	}
}