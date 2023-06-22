namespace ZeldaSaveTool.Controls {
	sealed partial class SaveSlot {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.file = new System.Windows.Forms.GroupBox();
			this.deathCount = new ZeldaSaveTool.Controls.LeadingSpinner();
			this.hearts = new ZeldaSaveTool.Controls.Hearts();
			this.name = new System.Windows.Forms.TextBox();
			this.file.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.deathCount)).BeginInit();
			this.SuspendLayout();
			// 
			// file
			// 
			this.file.Controls.Add(this.deathCount);
			this.file.Controls.Add(this.hearts);
			this.file.Controls.Add(this.name);
			this.file.Location = new System.Drawing.Point(0, 0);
			this.file.Name = "file";
			this.file.Size = new System.Drawing.Size(153, 87);
			this.file.TabIndex = 1;
			this.file.TabStop = false;
			this.file.Text = "File";
			// 
			// deathCount
			// 
			this.deathCount.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.deathCount.Location = new System.Drawing.Point(103, 19);
			this.deathCount.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.deathCount.Name = "deathCount";
			this.deathCount.Size = new System.Drawing.Size(44, 23);
			this.deathCount.TabIndex = 2;
			this.deathCount.Zeros = 3;
			// 
			// hearts
			// 
			this.hearts.CurrentHealth = ((short)(48));
			this.hearts.IsDoubleDefense = false;
			this.hearts.Location = new System.Drawing.Point(6, 48);
			this.hearts.Name = "hearts";
			this.hearts.Size = new System.Drawing.Size(141, 32);
			this.hearts.TabIndex = 1;
			this.hearts.TabStop = false;
			this.hearts.TotalHealth = ((short)(48));
			this.hearts.Click += new System.EventHandler(this.HeartAction);
			// 
			// name
			// 
			this.name.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.name.Location = new System.Drawing.Point(6, 19);
			this.name.MaxLength = 8;
			this.name.Name = "name";
			this.name.Size = new System.Drawing.Size(88, 23);
			this.name.TabIndex = 0;
			// 
			// SaveSlot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.file);
			this.Name = "SaveSlot";
			this.Size = new System.Drawing.Size(155, 88);
			this.file.ResumeLayout(false);
			this.file.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.deathCount)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private GroupBox file;
		private LeadingSpinner deathCount;
		private Hearts hearts;
		private TextBox name;
	}
}
