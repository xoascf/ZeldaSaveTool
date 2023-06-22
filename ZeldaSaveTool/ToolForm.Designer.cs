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
			this.note = new System.Windows.Forms.Label();
			this.version = new System.Windows.Forms.Label();
			this.repo = new System.Windows.Forms.LinkLabel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.saveSlot3 = new ZeldaSaveTool.Controls.SaveSlot();
			this.saveSlot2 = new ZeldaSaveTool.Controls.SaveSlot();
			this.saveSlot1 = new ZeldaSaveTool.Controls.SaveSlot();
			this.TypeRegLbl = new System.Windows.Forms.Label();
			this.tgSwitch = new ZeldaSaveTool.Controls.ToggleSwitch();
			this.ConvertBtn = new System.Windows.Forms.Button();
			this.SoundCb = new System.Windows.Forms.ComboBox();
			this.SndLbl = new System.Windows.Forms.Label();
			this.ZTargetCb = new System.Windows.Forms.ComboBox();
			this.TargetLbl = new System.Windows.Forms.Label();
			this.NewFormatCb = new System.Windows.Forms.ComboBox();
			this.ConvertLbl = new System.Windows.Forms.Label();
			this.FormatLbl = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// note
			// 
			this.note.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.note.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
			this.note.Location = new System.Drawing.Point(0, 0);
			this.note.Name = "note";
			this.note.Size = new System.Drawing.Size(370, 132);
			this.note.TabIndex = 0;
			this.note.Text = "Drop_It";
			this.note.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.note.DoubleClick += new System.EventHandler(this.Note_DoubleClick);
			// 
			// version
			// 
			this.version.AutoSize = true;
			this.version.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.version.Location = new System.Drawing.Point(0, 410);
			this.version.Name = "version";
			this.version.Padding = new System.Windows.Forms.Padding(3);
			this.version.Size = new System.Drawing.Size(48, 19);
			this.version.TabIndex = 1;
			this.version.Text = "Version";
			// 
			// repo
			// 
			this.repo.AutoSize = true;
			this.repo.Dock = System.Windows.Forms.DockStyle.Right;
			this.repo.Location = new System.Drawing.Point(155, 0);
			this.repo.Name = "repo";
			this.repo.Padding = new System.Windows.Forms.Padding(3);
			this.repo.Size = new System.Drawing.Size(215, 23);
			this.repo.TabIndex = 2;
			this.repo.TabStop = true;
			this.repo.Text = "https://github.com/xoascf/ZeldaSaveTool";
			this.repo.UseCompatibleTextRendering = true;
			this.repo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Repo_LinkClicked);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoSize = true;
			this.panel1.Controls.Add(this.saveSlot3);
			this.panel1.Controls.Add(this.saveSlot2);
			this.panel1.Controls.Add(this.saveSlot1);
			this.panel1.Controls.Add(this.TypeRegLbl);
			this.panel1.Controls.Add(this.tgSwitch);
			this.panel1.Controls.Add(this.ConvertBtn);
			this.panel1.Controls.Add(this.SoundCb);
			this.panel1.Controls.Add(this.SndLbl);
			this.panel1.Controls.Add(this.ZTargetCb);
			this.panel1.Controls.Add(this.TargetLbl);
			this.panel1.Controls.Add(this.NewFormatCb);
			this.panel1.Controls.Add(this.ConvertLbl);
			this.panel1.Controls.Add(this.FormatLbl);
			this.panel1.Location = new System.Drawing.Point(12, 135);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(340, 275);
			this.panel1.TabIndex = 3;
			// 
			// saveSlot3
			// 
			this.saveSlot3.CurrentHealth = ((short)(48));
			this.saveSlot3.DeathCount = 0;
			this.saveSlot3.IsDoubleDefense = false;
			this.saveSlot3.Location = new System.Drawing.Point(0, 178);
			this.saveSlot3.Name = "saveSlot3";
			this.saveSlot3.PlayerName = "";
			this.saveSlot3.Size = new System.Drawing.Size(160, 94);
			this.saveSlot3.TabIndex = 16;
			this.saveSlot3.Text = "File_3";
			this.saveSlot3.TotalHealth = ((short)(48));
			// 
			// saveSlot2
			// 
			this.saveSlot2.CurrentHealth = ((short)(48));
			this.saveSlot2.DeathCount = 0;
			this.saveSlot2.IsDoubleDefense = false;
			this.saveSlot2.Location = new System.Drawing.Point(0, 89);
			this.saveSlot2.Name = "saveSlot2";
			this.saveSlot2.PlayerName = "";
			this.saveSlot2.Size = new System.Drawing.Size(160, 94);
			this.saveSlot2.TabIndex = 15;
			this.saveSlot2.Text = "File_2";
			this.saveSlot2.TotalHealth = ((short)(48));
			// 
			// saveSlot1
			// 
			this.saveSlot1.CurrentHealth = ((short)(48));
			this.saveSlot1.DeathCount = 0;
			this.saveSlot1.IsDoubleDefense = false;
			this.saveSlot1.Location = new System.Drawing.Point(0, 0);
			this.saveSlot1.Name = "saveSlot1";
			this.saveSlot1.PlayerName = "";
			this.saveSlot1.Size = new System.Drawing.Size(160, 94);
			this.saveSlot1.TabIndex = 5;
			this.saveSlot1.Text = "File_1";
			this.saveSlot1.TotalHealth = ((short)(48));
			// 
			// TypeRegLbl
			// 
			this.TypeRegLbl.AutoSize = true;
			this.TypeRegLbl.Location = new System.Drawing.Point(189, 153);
			this.TypeRegLbl.Name = "TypeRegLbl";
			this.TypeRegLbl.Size = new System.Drawing.Size(41, 13);
			this.TypeRegLbl.TabIndex = 14;
			this.TypeRegLbl.Text = "Region";
			// 
			// tgSwitch
			// 
			this.tgSwitch.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.tgSwitch.Checked = false;
			this.tgSwitch.Cursor = System.Windows.Forms.Cursors.Hand;
			this.tgSwitch.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.tgSwitch.LeftLabelText = "NTSC";
			this.tgSwitch.Location = new System.Drawing.Point(215, 172);
			this.tgSwitch.Name = "tgSwitch";
			this.tgSwitch.RightLabelText = "PAL";
			this.tgSwitch.Size = new System.Drawing.Size(85, 23);
			this.tgSwitch.TabIndex = 13;
			// 
			// ConvertBtn
			// 
			this.ConvertBtn.Location = new System.Drawing.Point(192, 249);
			this.ConvertBtn.Name = "ConvertBtn";
			this.ConvertBtn.Size = new System.Drawing.Size(145, 23);
			this.ConvertBtn.TabIndex = 12;
			this.ConvertBtn.Text = "Convert_Now";
			this.ConvertBtn.UseVisualStyleBackColor = true;
			this.ConvertBtn.Click += new System.EventHandler(this.ConvertBtn_Click);
			// 
			// SoundCb
			// 
			this.SoundCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SoundCb.FormattingEnabled = true;
			this.SoundCb.Location = new System.Drawing.Point(192, 45);
			this.SoundCb.Name = "SoundCb";
			this.SoundCb.Size = new System.Drawing.Size(145, 21);
			this.SoundCb.TabIndex = 10;
			// 
			// SndLbl
			// 
			this.SndLbl.AutoSize = true;
			this.SndLbl.Location = new System.Drawing.Point(189, 29);
			this.SndLbl.Name = "SndLbl";
			this.SndLbl.Size = new System.Drawing.Size(38, 13);
			this.SndLbl.TabIndex = 6;
			this.SndLbl.Text = "Sound";
			// 
			// ZTargetCb
			// 
			this.ZTargetCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ZTargetCb.FormattingEnabled = true;
			this.ZTargetCb.Location = new System.Drawing.Point(192, 87);
			this.ZTargetCb.Name = "ZTargetCb";
			this.ZTargetCb.Size = new System.Drawing.Size(145, 21);
			this.ZTargetCb.TabIndex = 10;
			// 
			// TargetLbl
			// 
			this.TargetLbl.AutoSize = true;
			this.TargetLbl.Location = new System.Drawing.Point(189, 71);
			this.TargetLbl.Name = "TargetLbl";
			this.TargetLbl.Size = new System.Drawing.Size(65, 13);
			this.TargetLbl.TabIndex = 6;
			this.TargetLbl.Text = "Z_Targeting";
			// 
			// NewFormatCb
			// 
			this.NewFormatCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.NewFormatCb.FormattingEnabled = true;
			this.NewFormatCb.Location = new System.Drawing.Point(192, 128);
			this.NewFormatCb.Name = "NewFormatCb";
			this.NewFormatCb.Size = new System.Drawing.Size(145, 21);
			this.NewFormatCb.TabIndex = 10;
			this.NewFormatCb.SelectedIndexChanged += new System.EventHandler(this.NewFormatCb_SelectedIndexChanged);
			// 
			// ConvertLbl
			// 
			this.ConvertLbl.AutoSize = true;
			this.ConvertLbl.Location = new System.Drawing.Point(189, 112);
			this.ConvertLbl.Name = "ConvertLbl";
			this.ConvertLbl.Size = new System.Drawing.Size(63, 13);
			this.ConvertLbl.TabIndex = 6;
			this.ConvertLbl.Text = "Convert_To";
			// 
			// FormatLbl
			// 
			this.FormatLbl.AutoSize = true;
			this.FormatLbl.Location = new System.Drawing.Point(189, 8);
			this.FormatLbl.Name = "FormatLbl";
			this.FormatLbl.Size = new System.Drawing.Size(39, 13);
			this.FormatLbl.TabIndex = 5;
			this.FormatLbl.Text = "Format";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label1.Location = new System.Drawing.Point(13, 119);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Supported";
			// 
			// ToolForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(370, 429);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.repo);
			this.Controls.Add(this.version);
			this.Controls.Add(this.note);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.MaximizeBox = false;
			this.Name = "ToolForm";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ToolForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ToolForm_DragEnter);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolForm_KeyDown);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private Label note;
		private Label version;
		private LinkLabel repo;
		private Panel panel1;
		private Label ConvertLbl;
		private Label FormatLbl;
		private Button ConvertBtn;
		private ComboBox NewFormatCb;
		private Label label1;
		private Label SndLbl;
		private ComboBox ZTargetCb;
		private Label TargetLbl;
		private ComboBox SoundCb;
		private ToggleSwitch tgSwitch;
		private Label TypeRegLbl;
		private SaveSlot saveSlot1;
		private SaveSlot saveSlot2;
		private SaveSlot saveSlot3;
	}
}