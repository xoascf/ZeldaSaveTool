/* Licensed under the Open Software License version 3.0 */

namespace ZeldaSaveTool.Controls;

internal class LeadingSpinner : NumericUpDown {
	public int Zeros { get; set; }

	protected override void UpdateEditText() {
		string value = Value.ToString(CultureInfo.InvariantCulture);
		Text = value.PadLeft(Zeros, '0');
	}
}