using ZeldaSaveTool.Controls;

namespace ZeldaSaveTool
{
	partial class ToolForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblNote = new System.Windows.Forms.Label();
			this.lblNoteHint = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.llbRepo = new System.Windows.Forms.LinkLabel();
			this.lblSupported = new System.Windows.Forms.Label();
			this.splEditors = new SplitContainer();
			this.saveSlot3 = new ZeldaSaveTool.Controls.BasicSlot();
			this.saveSlot2 = new ZeldaSaveTool.Controls.BasicSlot();
			this.saveSlot1 = new ZeldaSaveTool.Controls.BasicSlot();
			this.lblFormat = new System.Windows.Forms.Label();
			this.cmbFormat = new System.Windows.Forms.ComboBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.grpRegion = new System.Windows.Forms.GroupBox();
			this.optPAL = new System.Windows.Forms.RadioButton();
			this.optNTSC = new System.Windows.Forms.RadioButton();
			this.chkDebug = new System.Windows.Forms.CheckBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.cmbZTarget = new System.Windows.Forms.ComboBox();
			this.lblTarget = new System.Windows.Forms.Label();
			this.cmbSound = new System.Windows.Forms.ComboBox();
			this.lblSound = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splEditors)).BeginInit();
			this.splEditors.Panel1.SuspendLayout();
			this.splEditors.Panel2.SuspendLayout();
			this.splEditors.SuspendLayout();
			this.panel1.SuspendLayout();
			this.grpRegion.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblNote
			// 
			this.lblNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lblNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNote.Location = new System.Drawing.Point(0, 0);
			this.lblNote.Name = "lblNote";
			this.lblNote.Size = new System.Drawing.Size(374, 132);
			this.lblNote.TabIndex = 0;
			this.lblNote.Text = "Drop_It";
			this.lblNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblNote.DoubleClick += new System.EventHandler(this.Note_DoubleClick);
			// 
			// lblNoteHint
			// 
			this.lblNoteHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lblNoteHint.Location = new System.Drawing.Point(0, 79);
			this.lblNoteHint.Name = "lblNoteHint";
			this.lblNoteHint.Size = new System.Drawing.Size(374, 23);
			this.lblNoteHint.TabIndex = 1;
			this.lblNoteHint.Text = "Hint_Access";
			this.lblNoteHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblNoteHint.DoubleClick += new System.EventHandler(this.Note_DoubleClick);
			// 
			// lblVersion
			// 
			this.lblVersion.AutoSize = true;
			this.lblVersion.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblVersion.Location = new System.Drawing.Point(0, 452);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Padding = new System.Windows.Forms.Padding(3);
			this.lblVersion.Size = new System.Drawing.Size(48, 19);
			this.lblVersion.TabIndex = 4;
			this.lblVersion.Text = "Version";
			// 
			// llbRepo
			// 
			this.llbRepo.AutoSize = true;
			this.llbRepo.Dock = System.Windows.Forms.DockStyle.Right;
			this.llbRepo.Location = new System.Drawing.Point(159, 0);
			this.llbRepo.Name = "llbRepo";
			this.llbRepo.Padding = new System.Windows.Forms.Padding(3);
			this.llbRepo.Size = new System.Drawing.Size(215, 23);
			this.llbRepo.TabIndex = 2;
			this.llbRepo.TabStop = true;
			this.llbRepo.Text = "https://github.com/xoascf/ZeldaSaveTool";
			this.llbRepo.UseCompatibleTextRendering = true;
			this.llbRepo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Repo_LinkClicked);
			// 
			// lblSupported
			// 
			this.lblSupported.AutoSize = true;
			this.lblSupported.ForeColor = System.Drawing.SystemColors.GrayText;
			this.lblSupported.Location = new System.Drawing.Point(13, 119);
			this.lblSupported.Name = "lblSupported";
			this.lblSupported.Size = new System.Drawing.Size(56, 13);
			this.lblSupported.TabIndex = 3;
			this.lblSupported.Text = "Supported";
			// 
			// splEditors
			// 
			this.splEditors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.splEditors.IsSplitterFixed = true;
			this.splEditors.Location = new System.Drawing.Point(0, 139);
			this.splEditors.Name = "splEditors";
			// 
			// splEditors.Panel1
			// 
			this.splEditors.Panel1.Controls.Add(this.saveSlot3);
			this.splEditors.Panel1.Controls.Add(this.saveSlot2);
			this.splEditors.Panel1.Controls.Add(this.saveSlot1);
			this.splEditors.Panel1.Padding = new System.Windows.Forms.Padding(10, 10, 10, 0);
			// 
			// splEditors.Panel2
			// 
			this.splEditors.Panel2.AutoScroll = true;
			this.splEditors.Panel2.Controls.Add(this.lblFormat);
			this.splEditors.Panel2.Controls.Add(this.cmbFormat);
			this.splEditors.Panel2.Controls.Add(this.panel1);
			this.splEditors.Panel2.Controls.Add(this.chkDebug);
			this.splEditors.Panel2.Controls.Add(this.btnSave);
			this.splEditors.Panel2.Controls.Add(this.cmbZTarget);
			this.splEditors.Panel2.Controls.Add(this.lblTarget);
			this.splEditors.Panel2.Controls.Add(this.cmbSound);
			this.splEditors.Panel2.Controls.Add(this.lblSound);
			this.splEditors.Panel2.Margin = new System.Windows.Forms.Padding(4);
			this.splEditors.Panel2.Padding = new System.Windows.Forms.Padding(10, 10, 10, 8);
			this.splEditors.Size = new System.Drawing.Size(374, 302);
			this.splEditors.SplitterDistance = 179;
			this.splEditors.SplitterWidth = 1;
			this.splEditors.TabIndex = 5;
			// 
			// saveSlot3
			// 
			this.saveSlot3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.saveSlot3.Dock = System.Windows.Forms.DockStyle.Top;
			this.saveSlot3.Location = new System.Drawing.Point(10, 198);
			this.saveSlot3.Name = "saveSlot3";
			this.saveSlot3.Size = new System.Drawing.Size(159, 94);
			this.saveSlot3.TabIndex = 7;
			this.saveSlot3.TabStop = false;
			this.saveSlot3.Text = "File_3";
			// 
			// saveSlot2
			// 
			this.saveSlot2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.saveSlot2.Dock = System.Windows.Forms.DockStyle.Top;
			this.saveSlot2.Location = new System.Drawing.Point(10, 104);
			this.saveSlot2.Name = "saveSlot2";
			this.saveSlot2.Size = new System.Drawing.Size(159, 94);
			this.saveSlot2.TabIndex = 6;
			this.saveSlot2.TabStop = false;
			this.saveSlot2.Text = "File_2";
			// 
			// saveSlot1
			// 
			this.saveSlot1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.saveSlot1.Dock = System.Windows.Forms.DockStyle.Top;
			this.saveSlot1.Location = new System.Drawing.Point(10, 10);
			this.saveSlot1.Name = "saveSlot1";
			this.saveSlot1.Size = new System.Drawing.Size(159, 94);
			this.saveSlot1.TabIndex = 5;
			this.saveSlot1.TabStop = false;
			this.saveSlot1.Text = "File_1";
			// 
			// lblFormat
			// 
			this.lblFormat.AutoSize = true;
			this.lblFormat.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblFormat.Location = new System.Drawing.Point(10, 124);
			this.lblFormat.Name = "lblFormat";
			this.lblFormat.Padding = new System.Windows.Forms.Padding(0, 8, 0, 4);
			this.lblFormat.Size = new System.Drawing.Size(67, 25);
			this.lblFormat.TabIndex = 12;
			this.lblFormat.Text = "New_Format";
			// 
			// cmbFormat
			// 
			this.cmbFormat.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.cmbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbFormat.FormattingEnabled = true;
			this.cmbFormat.Location = new System.Drawing.Point(10, 149);
			this.cmbFormat.Name = "cmbFormat";
			this.cmbFormat.Size = new System.Drawing.Size(174, 21);
			this.cmbFormat.TabIndex = 13;
			this.cmbFormat.SelectedIndexChanged += new System.EventHandler(this.Format_SelectedIndexChanged);
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.Controls.Add(this.grpRegion);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(10, 170);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(0, 8, 0, 8);
			this.panel1.Size = new System.Drawing.Size(174, 68);
			this.panel1.TabIndex = 14;
			// 
			// grpRegion
			// 
			this.grpRegion.Controls.Add(this.optPAL);
			this.grpRegion.Controls.Add(this.optNTSC);
			this.grpRegion.Dock = System.Windows.Forms.DockStyle.Top;
			this.grpRegion.Location = new System.Drawing.Point(0, 8);
			this.grpRegion.Name = "grpRegion";
			this.grpRegion.Size = new System.Drawing.Size(174, 52);
			this.grpRegion.TabIndex = 14;
			this.grpRegion.TabStop = false;
			this.grpRegion.Text = "Region";
			// 
			// optPAL
			// 
			this.optPAL.Checked = true;
			this.optPAL.Dock = System.Windows.Forms.DockStyle.Top;
			this.optPAL.Location = new System.Drawing.Point(3, 33);
			this.optPAL.Name = "optPAL";
			this.optPAL.Size = new System.Drawing.Size(168, 16);
			this.optPAL.TabIndex = 16;
			this.optPAL.TabStop = true;
			this.optPAL.Text = "PAL";
			this.optPAL.UseVisualStyleBackColor = true;
			// 
			// optNTSC
			// 
			this.optNTSC.AutoSize = true;
			this.optNTSC.Dock = System.Windows.Forms.DockStyle.Top;
			this.optNTSC.Location = new System.Drawing.Point(3, 16);
			this.optNTSC.Name = "optNTSC";
			this.optNTSC.Size = new System.Drawing.Size(168, 17);
			this.optNTSC.TabIndex = 15;
			this.optNTSC.Text = "NTSC";
			this.optNTSC.UseVisualStyleBackColor = true;
			// 
			// chkDebug
			// 
			this.chkDebug.AutoSize = true;
			this.chkDebug.Checked = true;
			this.chkDebug.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkDebug.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.chkDebug.Location = new System.Drawing.Point(10, 238);
			this.chkDebug.Name = "chkDebug";
			this.chkDebug.Padding = new System.Windows.Forms.Padding(0, 8, 0, 8);
			this.chkDebug.Size = new System.Drawing.Size(174, 33);
			this.chkDebug.TabIndex = 17;
			this.chkDebug.Text = "Is_Debug";
			this.chkDebug.UseVisualStyleBackColor = true;
			this.chkDebug.Visible = false;
			// 
			// btnSave
			// 
			this.btnSave.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnSave.Location = new System.Drawing.Point(10, 271);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(174, 23);
			this.btnSave.TabIndex = 18;
			this.btnSave.Text = "Save_As";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.Save_Click);
			// 
			// cmbZTarget
			// 
			this.cmbZTarget.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbZTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbZTarget.FormattingEnabled = true;
			this.cmbZTarget.Location = new System.Drawing.Point(10, 73);
			this.cmbZTarget.Name = "cmbZTarget";
			this.cmbZTarget.Size = new System.Drawing.Size(174, 21);
			this.cmbZTarget.TabIndex = 11;
			// 
			// lblTarget
			// 
			this.lblTarget.AutoSize = true;
			this.lblTarget.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblTarget.Location = new System.Drawing.Point(10, 48);
			this.lblTarget.Name = "lblTarget";
			this.lblTarget.Padding = new System.Windows.Forms.Padding(0, 8, 0, 4);
			this.lblTarget.Size = new System.Drawing.Size(65, 25);
			this.lblTarget.TabIndex = 10;
			this.lblTarget.Text = "Z_Targeting";
			// 
			// cmbSound
			// 
			this.cmbSound.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbSound.FormattingEnabled = true;
			this.cmbSound.Location = new System.Drawing.Point(10, 27);
			this.cmbSound.Name = "cmbSound";
			this.cmbSound.Size = new System.Drawing.Size(174, 21);
			this.cmbSound.TabIndex = 9;
			// 
			// lblSound
			// 
			this.lblSound.AutoSize = true;
			this.lblSound.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblSound.Location = new System.Drawing.Point(10, 10);
			this.lblSound.Name = "lblSound";
			this.lblSound.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.lblSound.Size = new System.Drawing.Size(38, 17);
			this.lblSound.TabIndex = 8;
			this.lblSound.Text = "Sound";
			// 
			// ToolForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(374, 471);
			this.Controls.Add(this.splEditors);
			this.Controls.Add(this.lblSupported);
			this.Controls.Add(this.llbRepo);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.lblNoteHint);
			this.Controls.Add(this.lblNote);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.MaximizeBox = false;
			this.Name = "ToolForm";
			this.Text = "Title";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ToolForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ToolForm_DragEnter);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolForm_KeyDown);
			this.splEditors.Panel1.ResumeLayout(false);
			this.splEditors.Panel2.ResumeLayout(false);
			this.splEditors.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splEditors)).EndInit();
			this.splEditors.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.grpRegion.ResumeLayout(false);
			this.grpRegion.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private Label lblNote;
		private Label lblNoteHint;
		private Label lblVersion;
		private LinkLabel llbRepo;
		private Label lblSupported;
		private BasicSlot saveSlot1;
		private BasicSlot saveSlot2;
		private BasicSlot saveSlot3;
		private SplitContainer splEditors;
		private Button btnSave;
		private ComboBox cmbZTarget;
		private Label lblTarget;
		private ComboBox cmbSound;
		private Label lblSound;
		private Panel panel1;
		private GroupBox grpRegion;
		private RadioButton optPAL;
		private RadioButton optNTSC;
		private CheckBox chkDebug;
		private Label lblFormat;
		private ComboBox cmbFormat;
	}
}