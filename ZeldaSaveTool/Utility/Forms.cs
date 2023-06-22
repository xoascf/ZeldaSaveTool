/* Licensed under the Open Software License version 3.0 */

using System.Drawing;
using System.Drawing.Imaging;

namespace ZeldaSaveTool.Utility;

internal static class Forms
{
	private static readonly Dictionary<string, string> SDict = new();

	private static bool HasUsableTextProperty(IDisposable ctrl) {
		return ctrl is Label or Button or CheckBox or ComboBox or GroupBox;
	}

	public static void UpdateText(Control col) {
		foreach (Control ctrl in col.Controls) {
			if (!HasUsableTextProperty(ctrl)) continue;
			string name = ctrl.Name;
			string og = ctrl.Text;

			if (!SDict.ContainsKey(name))
				SDict.Add(name, og);

			ctrl.Text = T(SDict[name]);
			UpdateText(ctrl);
		}
	}

	public static void Localize(this ComboBox cb, Array a, int defaultIndex = 0) {
		bool setDefault = cb.SelectedIndex == -1;
		int selected = defaultIndex;
		if (!setDefault)
			selected = cb.SelectedIndex;

		List<string> list = new();

		foreach (object? item in a)
			list.Add(T(item.ToString()));

		cb.DataSource = list;
		cb.SelectedIndex = setDefault ? defaultIndex : selected;
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