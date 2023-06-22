/* Licensed under the Open Software License version 3.0 */

using System.Drawing;
using ZeldaSaveTool.Save;
using ZeldaSaveTool.Utility;

namespace ZeldaSaveTool;

public partial class ToolForm : Form {
	private File.Format? _format;

	public ToolForm() {
		InitializeComponent();
		UpdateLocale();
		Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
		Program.Worker.DoWork += Worker_DoWork;
		Program.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
	}

	private static string FallbackText(string? text, string fallback)
		=> string.IsNullOrEmpty(text) ? fallback : text!;

	public void UpdateLocale() {
		Forms.UpdateText(this);
		Forms.UpdateText(panel1);
		Text = T("Title");
		version.Text = T("Version", PVer);
		label1.Text = T("Supported", ".sra, .sav");
		FormatLbl.Text = T("Format", T(FallbackText(_format.ToString(), "Not_Now")));

		saveSlot1.Text = T("File_1");
		saveSlot2.Text = T("File_2");
		saveSlot3.Text = T("File_3");

		SoundCb.Localize(Enum.GetValues(typeof(File.Sound)));
		ZTargetCb.Localize(Enum.GetValues(typeof(File.ZTargeting)));
		NewFormatCb.Localize(Enum.GetValues(typeof(File.Format)), (int)File.Format.PcPortSav);
	}

	private void ToolForm_DragEnter(object sender, DragEventArgs e) {
		if (!Enabled)
			return;

		if (e.Data != null &&
			e.Data.GetDataPresent(DataFormats.FileDrop))
			e.Effect = DragDropEffects.Copy;
	}

	private void ToolForm_DragDrop(object sender, DragEventArgs e) {
		if (e.Data != null)
			Program.Worker.RunWorkerAsync
				(e.Data.GetData(DataFormats.FileDrop) as string[]);

		Enabled = false;
	}

	private void Worker_DoWork(object? sender, DoWorkEventArgs e) {
		if (Program.SaveFileInst == null)
			return;

		File save = Program.SaveFileInst;

		saveSlot1.DeathCount = save.Slot1.DeathCount;
		saveSlot1.PlayerName = save.Slot1.Name ?? string.Empty;
		saveSlot1.TotalHealth = save.Slot1.HeartsTotal;
		saveSlot1.CurrentHealth = save.Slot1.HeartsCount;
		saveSlot1.IsDoubleDefense = save.Slot1.DoubleDefense;

		saveSlot2.DeathCount = save.Slot2.DeathCount;
		saveSlot2.PlayerName = save.Slot2.Name ?? string.Empty;
		saveSlot2.TotalHealth = save.Slot2.HeartsTotal;
		saveSlot2.CurrentHealth = save.Slot2.HeartsCount;
		saveSlot2.IsDoubleDefense = save.Slot2.DoubleDefense;

		saveSlot3.DeathCount = save.Slot3.DeathCount;
		saveSlot3.PlayerName = save.Slot3.Name ?? string.Empty;
		saveSlot3.TotalHealth = save.Slot3.HeartsTotal;
		saveSlot3.CurrentHealth = save.Slot3.HeartsCount;
		saveSlot3.IsDoubleDefense = save.Slot3.DoubleDefense;

		_format = save.FormatUsed;
		FormatLbl.Text = T("Format", T(FallbackText(_format.ToString(), "Not_Now")));

		SoundCb.SelectedIndex = save.SoundMode;
		ZTargetCb.SelectedIndex = save.ZTargetingMode;
	}

	private void Worker_RunWorkerCompleted(object? sender, EventArgs e) {
		Enabled = true;
	}

	private void Repo_LinkClicked(object sender, EventArgs e) {
		Process.Start(new ProcessStartInfo(((LinkLabel)sender).Text) {
			UseShellExecute = true
		});
	}

	private void Note_DoubleClick(object sender, EventArgs e) {
		if (!Enabled)
			return;

		using OpenFileDialog openFileDlg = new();

		openFileDlg.Filter = T("Supported_Filter") + @"|" +
							 T("SRA_Filter") + @"|" +
							 T("PC_Filter") + @"|" +
							 T("All_Filter");
		openFileDlg.RestoreDirectory = true;

		if (openFileDlg.ShowDialog() != DialogResult.OK)
			return;

		Program.Worker.RunWorkerAsync(new[] { openFileDlg.FileName });

		Enabled = false;
	}

	private void ToolForm_KeyDown(object sender, KeyEventArgs e) {
		if (e.KeyCode != Keys.F9)
			return;

		SetNextLanguage();
		UpdateLocale();
	}

	private void ConvertBtn_Click(object sender, EventArgs e) {
		if (Program.SaveFileInst != null) {
			File save = Program.SaveFileInst;

			save.FormatExport = (File.Format)NewFormatCb.SelectedIndex;

			save.Slot1.DeathCount = saveSlot1.DeathCount;
			save.Slot1.Name = saveSlot1.PlayerName;
			save.Slot1.HeartsTotal = saveSlot1.TotalHealth;
			save.Slot1.HeartsCount = saveSlot1.CurrentHealth;
			save.Slot1.DoubleDefense = saveSlot1.IsDoubleDefense;

			save.Slot2.DeathCount = saveSlot2.DeathCount;
			save.Slot2.Name = saveSlot2.PlayerName;
			save.Slot2.HeartsTotal = saveSlot2.TotalHealth;
			save.Slot2.HeartsCount = saveSlot2.CurrentHealth;
			save.Slot2.DoubleDefense = saveSlot2.IsDoubleDefense;

			save.Slot3.DeathCount = saveSlot3.DeathCount;
			save.Slot3.Name = saveSlot3.PlayerName;
			save.Slot3.HeartsTotal = saveSlot3.TotalHealth;
			save.Slot3.HeartsCount = saveSlot3.CurrentHealth;
			save.Slot3.DoubleDefense = saveSlot3.IsDoubleDefense;

			if (save.FormatExport == File.Format.N64Save) {
				save.ToNTSC = !tgSwitch.Checked;
				//save.IsMQ = false;
			} else {
				save.ToNTSC = false; // All PC Ports use PAL charset for now
				//save.IsMQ = tgSwitch.Checked;
			}

			save.OverwriteBackups = true;

			save.SoundMode = (byte)SoundCb.SelectedIndex;
			save.ZTargetingMode = (byte)ZTargetCb.SelectedIndex;

			Program.OutSaveData = save.ConvertSave();
			Program.SaveNewSaveFile();
		} else {
			Message.New(Message.Level.I, T("Open_First"));
		}
	}

	private void NewFormatCb_SelectedIndexChanged(object sender, EventArgs e) {
		File.Format format = (File.Format)NewFormatCb.SelectedIndex;
		if (format == File.Format.N64Save) {
			TypeRegLbl.Visible = true;
			tgSwitch.Visible = true;
			TypeRegLbl.Text = T("Region");
			tgSwitch.LeftLabelText = "NTSC";
			tgSwitch.RightLabelText = "PAL";
		} else {
			TypeRegLbl.Visible = false;
			tgSwitch.Visible = false;
			//TypeRegLbl.Text = T("Quest");
			//tgSwitch.LeftLabelText = "Vanilla";
			//tgSwitch.RightLabelText = "MQ";
		}
	}
}