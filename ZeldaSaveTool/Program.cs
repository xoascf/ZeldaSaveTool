/* Licensed under the Open Software License version 3.0 */

namespace ZeldaSaveTool;

internal static class Program
{
	public static BackgroundWorker Worker = new();

	private static byte[]? _portSaveData;
	private static string? _srcSaveName;

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	private static void Main()
	{
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

	private static void CheckAndWork(string[]? files)
	{
		_portSaveData = null;

		_srcSaveName = null;

		if (files is { Length: 1 })
		{
			var saveFilePath = files[0];
			try
			{
				if (!SaveFile.Validate(saveFilePath))
					return;

				_portSaveData = SaveFile.ToPcPortSave(saveFilePath);
				_srcSaveName = Path.GetFileName(saveFilePath);
			}
			catch (IOException ex)
			{
				Message.Exception(ex);
			}
		}
		else
			Message.New
				(Message.Level.E, _("Select_One"));
	}

	private static void Worker_DoWork(object? sender, DoWorkEventArgs e)
	{
		CheckAndWork(e.Argument as string[]);
	}

	private static void Worker_RunWorkerCompleted
		(object? sender, RunWorkerCompletedEventArgs e)
	{
		if (_portSaveData == null || _srcSaveName == null)
			return;

		using SaveFileDialog savDlg = new();

		savDlg.Title = _("Save_As_Title", _srcSaveName);
		savDlg.FileName = "oot";
		savDlg.Filter = "PC Port Save File|*.sav";

		if (savDlg.ShowDialog() != DialogResult.OK)
			return;

		try
		{
			Io.SaveToFile(_portSaveData, savDlg.FileName);
		}
		catch (IOException ex)
		{
			Message.Exception(ex);
		}
	}
}