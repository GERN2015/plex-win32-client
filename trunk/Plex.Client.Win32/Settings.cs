using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Plex.Client.Win32 {
    public partial class Settings : Form {

        public Enums.PlaybackType PlayBackMode;
        public string PlayerPath;
        public string PlayerArgs;
        private bool _showControls = false;

        public Settings() {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e) {
            try {

                PlayBackMode = (Enums.PlaybackType)Properties.Settings.Default.PlaybackMode;

                switch (PlayBackMode) {
                    case Enums.PlaybackType.UseFFPlayDirect:
                        rdoFFPlay.Checked = true;
                        _showControls = false;
                        break;
                    case Enums.PlaybackType.UseOtherMP:
                        rdoOther.Checked = true;
                        _showControls = true;
                        txtMPlayerPath.Text = PlayerPath = Properties.Settings.Default.PlayerPath.Trim();
                        txtCommandLine.Text = PlayerArgs = Properties.Settings.Default.PlayerArgs.Trim();
                        break;
                    case Enums.PlaybackType.UseFFPlayStream:
                        rdoFFPlayStream.Checked = true;
                        _showControls = false;
                        break;
                }

                SetControls(_showControls);

            } catch {
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            if ((PlayBackMode == Enums.PlaybackType.UseOtherMP) && (string.IsNullOrEmpty(PlayerPath)) ) {
                MessageBox.Show("You must supply a path to your own Media Player or use one of the other modes.", "Media Player Required!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);               
            } else {
                Properties.Settings.Default.PlaybackMode = (int)PlayBackMode;
                Properties.Settings.Default.PlayerArgs = PlayerArgs = txtCommandLine.Text;
                Properties.Settings.Default.PlayerPath = PlayerPath = txtMPlayerPath.Text;
                Properties.Settings.Default.Save();
                this.Close();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e) {
            System.Threading.ThreadPool.QueueUserWorkItem((arg) => {
                System.Threading.Thread.Sleep(250);

                Invoke(new EventHandler((o, parms) => {
                    ShowFileChooser();
                }));

            });

        }

        private void rdoFFPlay_Click(object sender, EventArgs e) {
            PlayBackMode = Enums.PlaybackType.UseFFPlayDirect;
            _showControls = false;
            
            SetControls(_showControls);
        }

        private void rdoOther_Click(object sender, EventArgs e) {
            PlayBackMode = Enums.PlaybackType.UseOtherMP;
            _showControls = true;

            SetControls(_showControls);
        }

        private void rdoFFPlayStream_Click(object sender, EventArgs e) {
            PlayBackMode = Enums.PlaybackType.UseFFPlayStream;
            _showControls = false;

            SetControls(_showControls);
        }

        private void SetControls(bool showControls) {
            this.txtCommandLine.Enabled = showControls;
            this.txtMPlayerPath.Enabled = showControls;
            this.btnBrowse.Enabled = showControls;
        }
        
        private void ShowFileChooser() {
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Multiselect = false;
            ofn.Filter = "Executables (*.exe)|*.exe|All Files (*.*)|*.*";
            ofn.Title = "Browse to Media Player";

            if (ofn.ShowDialog() == DialogResult.OK) {
                if (string.IsNullOrEmpty(ofn.FileName)) {
                    MessageBox.Show("You must supply a path to your own Media Player or use one of the other modes.", "Media Player Required!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                } else {
                    FileInfo fi = new FileInfo(ofn.FileName);
                    PlayerPath = txtMPlayerPath.Text = fi.FullName;

                }

            }
        }
    }
}
