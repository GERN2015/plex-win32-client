namespace Plex.Client.Win32 {
    partial class Settings {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.grpPlayBack = new System.Windows.Forms.GroupBox();
            this.grpOtherSettings = new System.Windows.Forms.GroupBox();
            this.txtCommandLine = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMPlayerPath = new System.Windows.Forms.TextBox();
            this.rdoOther = new System.Windows.Forms.RadioButton();
            this.rdoFFPlay = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.rdoFFPlayStream = new System.Windows.Forms.RadioButton();
            this.grpPlayBack.SuspendLayout();
            this.grpOtherSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPlayBack
            // 
            this.grpPlayBack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPlayBack.Controls.Add(this.rdoFFPlayStream);
            this.grpPlayBack.Controls.Add(this.grpOtherSettings);
            this.grpPlayBack.Controls.Add(this.rdoOther);
            this.grpPlayBack.Controls.Add(this.rdoFFPlay);
            this.grpPlayBack.Location = new System.Drawing.Point(13, 13);
            this.grpPlayBack.Name = "grpPlayBack";
            this.grpPlayBack.Size = new System.Drawing.Size(498, 138);
            this.grpPlayBack.TabIndex = 0;
            this.grpPlayBack.TabStop = false;
            this.grpPlayBack.Text = "Playback Options";
            // 
            // grpOtherSettings
            // 
            this.grpOtherSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOtherSettings.Controls.Add(this.txtCommandLine);
            this.grpOtherSettings.Controls.Add(this.label2);
            this.grpOtherSettings.Controls.Add(this.btnBrowse);
            this.grpOtherSettings.Controls.Add(this.label1);
            this.grpOtherSettings.Controls.Add(this.txtMPlayerPath);
            this.grpOtherSettings.Location = new System.Drawing.Point(7, 44);
            this.grpOtherSettings.Name = "grpOtherSettings";
            this.grpOtherSettings.Size = new System.Drawing.Size(484, 81);
            this.grpOtherSettings.TabIndex = 2;
            this.grpOtherSettings.TabStop = false;
            // 
            // txtCommandLine
            // 
            this.txtCommandLine.Location = new System.Drawing.Point(118, 50);
            this.txtCommandLine.Name = "txtCommandLine";
            this.txtCommandLine.Size = new System.Drawing.Size(322, 20);
            this.txtCommandLine.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Command Line Args:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(446, 17);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(27, 20);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Media Player:";
            // 
            // txtMPlayerPath
            // 
            this.txtMPlayerPath.Location = new System.Drawing.Point(118, 17);
            this.txtMPlayerPath.Name = "txtMPlayerPath";
            this.txtMPlayerPath.Size = new System.Drawing.Size(322, 20);
            this.txtMPlayerPath.TabIndex = 2;
            // 
            // rdoOther
            // 
            this.rdoOther.AutoSize = true;
            this.rdoOther.Location = new System.Drawing.Point(147, 19);
            this.rdoOther.Name = "rdoOther";
            this.rdoOther.Size = new System.Drawing.Size(188, 17);
            this.rdoOther.TabIndex = 1;
            this.rdoOther.TabStop = true;
            this.rdoOther.Text = "Direct Stream (Other Media Player)";
            this.rdoOther.UseVisualStyleBackColor = true;
            this.rdoOther.Click += new System.EventHandler(this.rdoOther_Click);
            // 
            // rdoFFPlay
            // 
            this.rdoFFPlay.AutoSize = true;
            this.rdoFFPlay.Location = new System.Drawing.Point(7, 20);
            this.rdoFFPlay.Name = "rdoFFPlay";
            this.rdoFFPlay.Size = new System.Drawing.Size(130, 17);
            this.rdoFFPlay.TabIndex = 0;
            this.rdoFFPlay.TabStop = true;
            this.rdoFFPlay.Text = "Direct Stream (FFPlay)";
            this.rdoFFPlay.UseVisualStyleBackColor = true;
            this.rdoFFPlay.Click += new System.EventHandler(this.rdoFFPlay_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(412, 173);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(99, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rdoFFPlayStream
            // 
            this.rdoFFPlayStream.AutoSize = true;
            this.rdoFFPlayStream.Location = new System.Drawing.Point(340, 19);
            this.rdoFFPlayStream.Name = "rdoFFPlayStream";
            this.rdoFFPlayStream.Size = new System.Drawing.Size(140, 17);
            this.rdoFFPlayStream.TabIndex = 3;
            this.rdoFFPlayStream.TabStop = true;
            this.rdoFFPlayStream.Text = "Use Transcoded Stream";
            this.rdoFFPlayStream.UseVisualStyleBackColor = true;
            this.rdoFFPlayStream.Click += new System.EventHandler(this.rdoFFPlayStream_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 209);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpPlayBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.grpPlayBack.ResumeLayout(false);
            this.grpPlayBack.PerformLayout();
            this.grpOtherSettings.ResumeLayout(false);
            this.grpOtherSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPlayBack;
        private System.Windows.Forms.GroupBox grpOtherSettings;
        private System.Windows.Forms.TextBox txtCommandLine;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMPlayerPath;
        private System.Windows.Forms.RadioButton rdoOther;
        private System.Windows.Forms.RadioButton rdoFFPlay;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rdoFFPlayStream;
    }
}