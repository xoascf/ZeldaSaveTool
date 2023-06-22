/* Licensed under the Open Software License version 3.0 */

using ZeldaSaveTool.Save;
using ZeldaSaveTool.Utility;

namespace ZeldaSaveTool;

internal static class Program {
	public static BackgroundWorker Worker = new();
	public static Save.File? SaveFileInst;

	private static string _srcSaveName = "";
	public static byte[] OutSaveData = Zero;

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	private static void Main() {
		Worker.DoWork += Worker_DoWork;
		Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

		SetLocalizedStrings(null);

#if (NET6_0_OR_GREATER)
		ApplicationConfiguration.Initialize();
#else
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
#endif
		Application.Run(new ToolForm());
	}

	private static void CheckAndWork(string[]? files) {
		_srcSaveName = "";
		OutSaveData = Zero;

		if (files is { Length: 1 }) {
			string saveFilePath = files[0];
			try {
				SaveFileInst = new(saveFilePath);

				if (SaveFileInst.ValidSave)
					_srcSaveName = IO.BaseName(saveFilePath);
				else
					SaveFileInst = null;
			}
			catch (Exception ex) {
				Message.Exception(ex);
			}
		}
		else
			Message.New(Message.Level.E, T("Select_One"));
	}

	private static void Worker_DoWork(object? sender, DoWorkEventArgs e) {
		CheckAndWork(e.Argument as string[]);
	}

	public static void SaveNewSaveFile() {
		if (OutSaveData == Zero)
			return;

		const string sraFn = "THE LEGEND OF ZELDA";
		const string sohFn = "oot_save";
		string sraFf = T("SRA_Filter") + "|" + T("All_Filter");
		string pcFf = T("PC_Filter") + "|" + T("All_Filter");
		bool isSavingForN64 = SaveFileInst?.FormatExport == File.Format.N64Save;
		using SaveFileDialog savDlg = new();
		savDlg.Title = T("Save_As_Title", _srcSaveName);
		savDlg.FileName = isSavingForN64 ? sraFn : sohFn;
		savDlg.Filter = isSavingForN64 ? sraFf : pcFf;

		if (savDlg.ShowDialog() != DialogResult.OK)
			return;

		try {
			IO.SaveToFile(OutSaveData, savDlg.FileName);
		} catch (Exception ex) {
			Message.Exception(ex);
		}
	}

	private static void Worker_RunWorkerCompleted
		(object? sender, RunWorkerCompletedEventArgs e) {
		SaveNewSaveFile();
	}
}