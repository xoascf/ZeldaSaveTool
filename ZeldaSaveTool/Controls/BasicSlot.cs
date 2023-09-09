using System.Drawing;
using System.Text.RegularExpressions;
using ZeldaSaveTool.Save;

namespace ZeldaSaveTool.Controls;

internal sealed class BasicSlot : GroupBox {
	private readonly TextBox playerName;
	private readonly LeadingSpinner deathCount;
	private readonly Hearts hearts;
	private Slot data;

	public Slot Info {
		get {
			data.DeathCount = (short)deathCount.Value;
			data.Name = playerName.Text;
			data.HeartsTotal = hearts.TotalHealth;
			data.HeartsCount = hearts.CurrentHealth;
			data.DoubleDefense = hearts.IsDoubleDefense;
			return data;
		} set {
			data = value;
			deathCount.Value = data.DeathCount;
			playerName.Text = data.Name ?? string.Empty;
			hearts.TotalHealth = data.HeartsTotal;
			hearts.CurrentHealth = data.HeartsCount;
			hearts.IsDoubleDefense = data.DoubleDefense;
		}
	}

	public BasicSlot() {
		playerName = new TextBox();
		deathCount = new LeadingSpinner();
		hearts = new Hearts();

		DoubleBuffered = true;
		Location = new Point(0, 0);
		Size = new Size(153, 88);

		Font font = new("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);

		playerName.Font = font;
		playerName.Location = new Point(6, 19);
		playerName.MaxLength = 8;
		playerName.Size = new Size(88, 23);
		playerName.TabIndex = 0;
		playerName.TextChanged += PlayerNameTextChanged;

		((ISupportInitialize)deathCount).BeginInit();
		deathCount.Font = font;
		deathCount.Location = new Point(103, 19);
		deathCount.Maximum = 999;
		deathCount.Size = new Size(44, 23);
		deathCount.TabIndex = 1;
		deathCount.Zeros = 3;
		((ISupportInitialize)deathCount).EndInit();

		hearts.Location = new Point(6, 48);
		hearts.Size = new Size(141, 32);
		hearts.TabIndex = 2;
		hearts.TabStop = false;
		hearts.Click += HeartAction;
		hearts.MouseWheel += HeartAction;

		Controls.Add(playerName);
		Controls.Add(deathCount);
		Controls.Add(hearts);

		data = new Slot();
	}

	private static readonly Regex NameRegex = new("[^ a-zA-Z0-9.-]+");

	private static void PlayerNameTextChanged(object? sender, EventArgs e){
		if (sender == null)
			return;

		TextBox tb = (TextBox)sender;

		if (NameRegex.Matches(tb.Text).Count == 0)
			return;

		int i = tb.SelectionStart;
		tb.Text = NameRegex.Replace(tb.Text, "");
		tb.SelectionStart = i - 1;
	}

	private void HeartAction(object? sender, EventArgs e) {
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