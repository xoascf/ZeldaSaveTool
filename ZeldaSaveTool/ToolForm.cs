/* Licensed under the Open Software License version 3.0 */

namespace ZeldaSaveTool;

public partial class ToolForm : Form
{
	public ToolForm()
	{
		InitializeComponent();
		UpdateLocale();

		Program.Worker.RunWorkerCompleted +=
			Worker_RunWorkerCompleted;
	}

	public void UpdateLocale()
	{
		Text = _("Title");

		note.Text = _("Drop_It");
		version.Text = _("Version", PVer);
	}

	private void ToolForm_DragEnter(object sender, DragEventArgs e)
	{
		if (!Enabled)
			return;

		if (e.Data != null &&
			e.Data.GetDataPresent(DataFormats.FileDrop))
			e.Effect = DragDropEffects.Copy;
	}

	private void ToolForm_DragDrop(object sender, DragEventArgs e)
	{
		if (e.Data != null)
			Program.Worker.RunWorkerAsync
				(e.Data.GetData(DataFormats.FileDrop) as string[]);

		Enabled = false;
	}

	private void Worker_RunWorkerCompleted
		(object? sender, RunWorkerCompletedEventArgs e)
	{
		Enabled = true;
	}

	private void Repo_LinkClicked
		(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(new ProcessStartInfo(((LinkLabel)sender).Text)
		{
			UseShellExecute = true
		});
	}

	private void Note_DoubleClick(object sender, EventArgs e)
	{
		if (!Enabled)
			return;

		using OpenFileDialog openFileDlg = new();

		openFileDlg.Filter = _("Open_Filter");
		openFileDlg.RestoreDirectory = true;

		if (openFileDlg.ShowDialog() != DialogResult.OK)
			return;

		Program.Worker.RunWorkerAsync
		(new[]
		{
			openFileDlg.FileName
		});

		Enabled = false;
	}

	private void ToolForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode != Keys.F9)
			return;

		SetNextLanguage();
		UpdateLocale();
	}
}