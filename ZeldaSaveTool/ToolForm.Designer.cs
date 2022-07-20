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
			this.SuspendLayout();
			// 
			// note
			// 
			this.note.Dock = System.Windows.Forms.DockStyle.Fill;
			this.note.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
			this.note.Location = new System.Drawing.Point(0, 0);
			this.note.Name = "note";
			this.note.Size = new System.Drawing.Size(434, 271);
			this.note.TabIndex = 0;
			this.note.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.note.DoubleClick += new System.EventHandler(this.Note_DoubleClick);
			// 
			// version
			// 
			this.version.AutoSize = true;
			this.version.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.version.Location = new System.Drawing.Point(0, 252);
			this.version.Name = "version";
			this.version.Padding = new System.Windows.Forms.Padding(3);
			this.version.Size = new System.Drawing.Size(6, 19);
			this.version.TabIndex = 1;
			// 
			// repo
			// 
			this.repo.AutoSize = true;
			this.repo.Dock = System.Windows.Forms.DockStyle.Right;
			this.repo.Location = new System.Drawing.Point(219, 0);
			this.repo.Name = "repo";
			this.repo.Padding = new System.Windows.Forms.Padding(3);
			this.repo.Size = new System.Drawing.Size(215, 23);
			this.repo.TabIndex = 2;
			this.repo.TabStop = true;
			this.repo.Text = "https://github.com/xoascf/ZeldaSaveTool";
			this.repo.UseCompatibleTextRendering = true;
			this.repo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Repo_LinkClicked);
			// 
			// ToolForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 271);
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
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Label note;
		private Label version;
		private LinkLabel repo;
	}
}