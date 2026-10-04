using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeamngAudioCompressor
{
    public partial class ProgressDlg : Form
    {
        public ProgressDlg()
        {
            InitializeComponent();
        }

        public void PushText(string line)
        {
            listBox1.Items.Add(line);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
        public void UpdateProgress(float perc)
        {
            progressBar1.Value = (int)(perc * 100);
        }

        public void EnableBtn()
        {
            button1.Enabled = true;
        }
        private void ProgressDlg_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var line in listBox1.Items)
            {
                builder.AppendLine(line.ToString());
            }
            Clipboard.SetText(builder.ToString());
        }
    }
}
