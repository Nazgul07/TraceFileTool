﻿namespace TraceFileTool
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.btGo = new System.Windows.Forms.Button();
			this.tbFile = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btBrowse = new System.Windows.Forms.Button();
			this.progress = new System.Windows.Forms.ProgressBar();
			this.btMine = new System.Windows.Forms.Button();
			this.cbExcludeInternals = new System.Windows.Forms.CheckBox();
			this.btnSessions = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btGo
			// 
			this.btGo.Location = new System.Drawing.Point(248, 54);
			this.btGo.Name = "btGo";
			this.btGo.Size = new System.Drawing.Size(94, 23);
			this.btGo.TabIndex = 0;
			this.btGo.Text = "Fix Trace File";
			this.btGo.UseVisualStyleBackColor = true;
			this.btGo.Click += new System.EventHandler(this.btGo_Click);
			// 
			// tbFile
			// 
			this.tbFile.Location = new System.Drawing.Point(73, 28);
			this.tbFile.Name = "tbFile";
			this.tbFile.Size = new System.Drawing.Size(236, 20);
			this.tbFile.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Trace File";
			// 
			// btBrowse
			// 
			this.btBrowse.Location = new System.Drawing.Point(315, 26);
			this.btBrowse.Name = "btBrowse";
			this.btBrowse.Size = new System.Drawing.Size(27, 23);
			this.btBrowse.TabIndex = 3;
			this.btBrowse.Text = "...";
			this.btBrowse.UseVisualStyleBackColor = true;
			this.btBrowse.Click += new System.EventHandler(this.btBrowse_Click);
			// 
			// progress
			// 
			this.progress.Location = new System.Drawing.Point(5, 113);
			this.progress.Maximum = 1000;
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size(337, 23);
			this.progress.TabIndex = 4;
			this.progress.Visible = false;
			// 
			// btMine
			// 
			this.btMine.Location = new System.Drawing.Point(248, 83);
			this.btMine.Name = "btMine";
			this.btMine.Size = new System.Drawing.Size(94, 23);
			this.btMine.TabIndex = 5;
			this.btMine.Text = "Mine Trace File";
			this.btMine.UseVisualStyleBackColor = true;
			this.btMine.Click += new System.EventHandler(this.btMine_Click);
			// 
			// cbExcludeInternals
			// 
			this.cbExcludeInternals.AutoSize = true;
			this.cbExcludeInternals.Checked = true;
			this.cbExcludeInternals.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbExcludeInternals.Location = new System.Drawing.Point(116, 87);
			this.cbExcludeInternals.Name = "cbExcludeInternals";
			this.cbExcludeInternals.Size = new System.Drawing.Size(132, 17);
			this.cbExcludeInternals.TabIndex = 6;
			this.cbExcludeInternals.Text = "Exclude DCT Internals";
			this.cbExcludeInternals.UseVisualStyleBackColor = true;
			// 
			// btnSessions
			// 
			this.btnSessions.Location = new System.Drawing.Point(148, 54);
			this.btnSessions.Name = "btnSessions";
			this.btnSessions.Size = new System.Drawing.Size(94, 23);
			this.btnSessions.TabIndex = 7;
			this.btnSessions.Text = "Split by Sessions";
			this.btnSessions.UseVisualStyleBackColor = true;
			this.btnSessions.Click += new System.EventHandler(this.btnSessions_Click);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(346, 148);
			this.Controls.Add(this.btnSessions);
			this.Controls.Add(this.cbExcludeInternals);
			this.Controls.Add(this.btMine);
			this.Controls.Add(this.progress);
			this.Controls.Add(this.btBrowse);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbFile);
			this.Controls.Add(this.btGo);
			this.Name = "Main";
			this.Text = "Trace File Tool";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btGo;
        private System.Windows.Forms.TextBox tbFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btBrowse;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Button btMine;
        private System.Windows.Forms.CheckBox cbExcludeInternals;
		private System.Windows.Forms.Button btnSessions;
	}
}

