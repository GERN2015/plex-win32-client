namespace Plex.Client.Win32
{
    partial class StreamPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StreamPlayer));
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.axVLCPlugin21 = new AxAXVLC.AxVLCPlugin2();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axVLCPlugin21)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trackBar1.Location = new System.Drawing.Point(0, 433);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(695, 45);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trackBar1_KeyDown);
            // 
            // axVLCPlugin21
            // 
            this.axVLCPlugin21.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.axVLCPlugin21.Enabled = true;
            this.axVLCPlugin21.Location = new System.Drawing.Point(0, 0);
            this.axVLCPlugin21.Name = "axVLCPlugin21";
            this.axVLCPlugin21.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axVLCPlugin21.OcxState")));
            this.axVLCPlugin21.Size = new System.Drawing.Size(695, 434);
            this.axVLCPlugin21.TabIndex = 0;
            this.axVLCPlugin21.MediaPlayerPlaying += new System.EventHandler(this.axVLCPlugin21_MediaPlayerPlaying);
            this.axVLCPlugin21.MediaPlayerTimeChanged += new AxAXVLC.DVLCEvents_MediaPlayerTimeChangedEventHandler(this.axVLCPlugin21_MediaPlayerTimeChanged);
            this.axVLCPlugin21.MediaPlayerPositionChanged += new AxAXVLC.DVLCEvents_MediaPlayerPositionChangedEventHandler(this.axVLCPlugin21_MediaPlayerPositionChanged);
            this.axVLCPlugin21.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.axVLCPlugin21_PreviewKeyDown);
            this.axVLCPlugin21.MediaPlayerOpening += new System.EventHandler(this.axVLCPlugin21_MediaPlayerOpening);
            // 
            // StreamPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 478);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.axVLCPlugin21);
            this.Name = "StreamPlayer";
            this.Text = "StreamPlayer";
            this.Load += new System.EventHandler(this.StreamPlayer_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StreamPlayer_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StreamPlayer_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axVLCPlugin21)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxAXVLC.AxVLCPlugin2 axVLCPlugin21;
        private System.Windows.Forms.TrackBar trackBar1;
    }
}