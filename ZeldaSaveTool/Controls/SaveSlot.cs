using System.Text.RegularExpressions;

namespace ZeldaSaveTool.Controls;

internal sealed partial class SaveSlot : UserControl {

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	public override string Text {
		get => file.Text;
		set => file.Text = value;
	}

	public string PlayerName {
		get => name.Text;
		set => name.Text = value;
	}

	public short DeathCount {
		get => (short)deathCount.Value;
		set => deathCount.Value = value;
	}

	public short TotalHealth {
		get => hearts.TotalHealth;
		set => hearts.TotalHealth = value;
	}

	public short CurrentHealth {
		get => hearts.CurrentHealth;
		set => hearts.CurrentHealth = value;
	}

	public bool IsDoubleDefense {
		get => hearts.IsDoubleDefense;
		set => hearts.IsDoubleDefense = value;
	}

	public SaveSlot() {
		InitializeComponent();
		DoubleBuffered = true;
		name.TextChanged += NameTextChanged;
		hearts.MouseWheel += HeartAction;
	}

	private static void NameTextChanged(object? sender, EventArgs e) {
		if (sender == null)
			return;

		TextBox tb = (TextBox)sender;
		tb.Text = Regex.Replace(tb.Text, "[^ a-zA-Z0-9.-]+", "");
	}

	private void HeartAction(object sender, EventArgs e) {
		MouseEventArgs me = (MouseEventArgs)e;
		switch (ModifierKeys) {
			case Keys.Control:
				hearts.TotalHealth += (short)
				(me.Button == MouseButtons.Right || me.Delta < 0 ? -16 : 16);
				break;

			default:
				if (me.Button == MouseButtons.Middle)
					hearts.IsDoubleDefense = !hearts.IsDoubleDefense;
				else if (me.Button == MouseButtons.Right || me.Delta < 0)
					hearts.CurrentHealth -= 4;
				else
					hearts.CurrentHealth += 4;
				break;
		}
	}
}