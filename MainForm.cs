using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeamngAudioCompressor
{
    public partial class MainForm : Form
    {
        private AudioCompressor _compressor = null;
        private JsonRenamer _renamer = null;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
        private void sliderOggQuality_ValueChanged(object sender, EventArgs e)
        {
            lblOggQualityValue.Text = $"{sliderOggQuality.Value}";
        }

        private void btnBrowseArtFolder_Click(object sender, EventArgs e)
        {
            if (dlgBrowseFolder.ShowDialog() == DialogResult.OK)
            {
                txtBoxSrcFolder.Text = dlgBrowseFolder.SelectedPath.Normalize();
            }
        }

        private void btnBrowseTgtArtFolder_Click(object sender, EventArgs e)
        {
            if (dlgBrowseFolder.ShowDialog() == DialogResult.OK)
            {
                txtBoxTgtFolder.Text = dlgBrowseFolder.SelectedPath.Normalize();
            }
        }

        ProgressDlg dlg;
        private void btnCompressAudio_Click(object sender, EventArgs e)
        {
            _compressor = new AudioCompressor(new AudioCompressorSettings
            {
                SourceArtPath = txtBoxSrcFolder.Text + @"\art",
                TargetArtPath = txtBoxTgtFolder.Text + @"\art",
                OggQuality = sliderOggQuality.Value,
            });
            dlg?.Close();
            dlg = new ProgressDlg();
            dlg.Show(this);
            if (bgworkerWorkCompressAudio.IsBusy) return;
            bgworkerWorkCompressAudio.RunWorkerAsync();
        }

        private async void Continuething(ProgressDlg dlg, Task task)
        {
            task.Wait();
            dlg.Invoke((MethodInvoker)delegate
            {
                dlg.EnableBtn();
            });
            if (task.IsFaulted)
            {
                MessageBox.Show(task.Exception.ToString());
            }
        }
        private void btnFixJbeams_Click(object sender, EventArgs e)
        {
            _renamer = new JsonRenamer(new JsonRenamerSettings
            {
                PrefixBlendName = txtBoxPrefixName.Text,
                SourceArtPath = txtBoxSrcFolder.Text + @"\art",
                TargetArtPath = txtBoxTgtFolder.Text + @"\art",
                SourceVehiclesPath = txtBoxSrcFolder.Text + @"\vehicles",
                TargetVehiclesPath = txtBoxTgtFolder.Text + @"\vehicles",
            });
            dlg?.Close();
            dlg = new ProgressDlg();
            dlg.Show(this);

            _renamer.RenameAll((string msg, float prog) =>
             {
                 dlg.PushText(msg);
                 dlg.UpdateProgress(prog);
             });
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Task result = _compressor.CompressAndFix((string msg, float prog) =>
            {
                dlg.Invoke((MethodInvoker)delegate
                {
                    dlg.PushText(msg);
                    dlg.UpdateProgress(prog);
                });
            });
            Continuething(dlg, result);
        }
    }
}
