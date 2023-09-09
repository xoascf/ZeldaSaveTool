/* Licensed under the Open Software License version 3.0 */

using ZeldaSaveTool.Save;
using ZeldaSaveTool.Utility;

namespace ZeldaSaveTool;

public partial class ToolForm : Form {
	private static readonly BackgroundWorker Worker = new();
	private static File? _save;

	private static string _srcSaveName = "";
	private static byte[] _outSaveData = Zero;

	public ToolForm() {
		InitializeComponent();
		UpdateLocale();

		string? currentExePath = Process.GetCurrentProcess().MainModule?.FileName;
		if (!string.IsNullOrWhiteSpace(currentExePath))
			Icon = System.Drawing.Icon.ExtractAssociatedIcon(currentExePath);

		Worker.DoWork += Worker_DoWork;
		Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
	}

	private void UpdateLocale() {
		Forms.UpdateText(this);

		UpdateLoadedInfo();

		lblVersion.Text = T("Version", PVer);
		lblSupported.Text = T("Supported", ".sra, .sav");
		cmbFormat.Localize(Enum.GetValues(typeof(File.Format)), (int)File.Format.PcPortSav);
		cmbSound.Localize(Enum.GetValues(typeof(File.Sound)));
		cmbZTarget.Localize(Enum.GetValues(typeof(File.ZTargeting)));
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
			Worker.RunWorkerAsync
				(e.Data.GetData(DataFormats.FileDrop) as string[]);

		Enabled = false;
	}

	private void CheckAndWork(string[]? files) {
		_srcSaveName = "";
		_outSaveData = Zero;

		if (files is { Length: 1 }) {
			string saveFilePath = files[0];

			try {
				_save = new(saveFilePath);

				if (_save.ValidSave) {
					LoadSaveFileInfo();
					_srcSaveName = IO.BaseName(saveFilePath);
				} else
					_save = null;
			} catch (Exception ex) {
				Message.Exception(ex);
			}
		} else
			Message.New(Message.Level.E, T("Select_One"));
	}

	private void Worker_DoWork(object? sender, DoWorkEventArgs e) {
		Invoke(new Action<object>((args) => {
			CheckAndWork((string[])args);
		}), e.Argument);
	}

	private void LoadSaveFileInfo() {
		if (_save == null)
			return;

		saveSlot1.Info = _save.Slot1;
		saveSlot2.Info = _save.Slot2;
		saveSlot3.Info = _save.Slot3;

		cmbSound.SelectedIndex = _save.SoundMode;
		cmbZTarget.SelectedIndex = _save.ZTargetingMode;
	}

	private void Worker_RunWorkerCompleted(object? sender, EventArgs e) {
		Enabled = true;
		UpdateLoadedInfo();
	}

	private void UpdateLoadedInfo() {
		if (_save != null && _srcSaveName.Length != 0) {
			splEditors.Enabled = true;
			lblNote.Text = _srcSaveName;
			lblNoteHint.Text = T("Hint_Format", T(_save.FormatUsed.ToString()!));
			return;
		}

		lblNote.Text = T("Drop_It");
		lblNoteHint.Text = T("Hint_Access");
		splEditors.Enabled = false;
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

		Worker.RunWorkerAsync(new[] { openFileDlg.FileName });

		Enabled = false;
	}

	private void ToolForm_KeyDown(object sender, KeyEventArgs e) {
		if (e.KeyCode != Keys.F9)
			return;

		SetNextLanguage();
		UpdateLocale();
	}

	private static void SaveNewSaveFile() {
		if (_outSaveData == Zero)
			return;

		const string sraFn = "THE LEGEND OF ZELDA";
		const string sohFn = "oot_save";
		string sraFf = T("SRA_Filter") + "|" + T("All_Filter");
		string pcFf = T("PC_Filter") + "|" + T("All_Filter");
		bool isSavingForN64 = _save?.FormatExport == File.Format.N64Save;
		using SaveFileDialog savDlg = new();
		savDlg.Title = T("Save_As_Title", _srcSaveName);
		savDlg.FileName = isSavingForN64 ? sraFn : sohFn;
		savDlg.Filter = isSavingForN64 ? sraFf : pcFf;

		if (savDlg.ShowDialog() != DialogResult.OK)
			return;

		try {
			IO.SaveToFile(_outSaveData, savDlg.FileName);
		} catch (Exception ex) {
			Message.Exception(ex);
		}
	}

	private void Save_Click(object sender, EventArgs e) {
		if (_save != null) {
			_save.FormatExport = (File.Format)cmbFormat.SelectedIndex;

			_save.Slot1 = saveSlot1.Info;
			_save.Slot2 = saveSlot2.Info;
			_save.Slot3 = saveSlot3.Info;

			_save.ToNTSC = optNTSC.Checked;
			_save.AlternateChecksum = chkDebug.Checked;

			_save.OverwriteBackups = true;

			_save.SoundMode = (byte)cmbSound.SelectedIndex;
			_save.ZTargetingMode = (byte)cmbZTarget.SelectedIndex;

			_outSaveData = _save.ConvertSave();
			SaveNewSaveFile();
		} else
			Message.New(Message.Level.I, T("Open_First"));
	}

	private void Format_SelectedIndexChanged(object sender, EventArgs e) {
		File.Format format = (File.Format)cmbFormat.SelectedIndex;
		if (format == File.Format.N64Save) {
			chkDebug.Enabled = true;
			grpRegion.Enabled = true;
		} else {
			optPAL.Checked = true;
			chkDebug.Checked = true;
			chkDebug.Enabled = false;
			grpRegion.Enabled = false;
		}
	}
}