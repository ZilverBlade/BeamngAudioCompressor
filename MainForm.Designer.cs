namespace BeamngAudioCompressor
{
    partial class MainForm
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
            this.btnCompressAudio = new System.Windows.Forms.Button();
            this.dlgBrowseFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.txtBoxSrcFolder = new System.Windows.Forms.TextBox();
            this.lblSrcFolder = new System.Windows.Forms.Label();
            this.btnBrowseSrcFolder = new System.Windows.Forms.Button();
            this.sliderOggQuality = new System.Windows.Forms.TrackBar();
            this.lblOggQuality = new System.Windows.Forms.Label();
            this.lblOggQualityValue = new System.Windows.Forms.Label();
            this.lblPrefixName = new System.Windows.Forms.Label();
            this.txtBoxPrefixName = new System.Windows.Forms.TextBox();
            this.btnBrowseTgtFolder = new System.Windows.Forms.Button();
            this.lblTgtFolder = new System.Windows.Forms.Label();
            this.txtBoxTgtFolder = new System.Windows.Forms.TextBox();
            this.btnFixJbeams = new System.Windows.Forms.Button();
            this.bgworkerWorkCompressAudio = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.sliderOggQuality)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCompressAudio
            // 
            this.btnCompressAudio.Location = new System.Drawing.Point(12, 145);
            this.btnCompressAudio.Name = "btnCompressAudio";
            this.btnCompressAudio.Size = new System.Drawing.Size(129, 24);
            this.btnCompressAudio.TabIndex = 0;
            this.btnCompressAudio.Text = "Compress Audio";
            this.btnCompressAudio.UseVisualStyleBackColor = true;
            this.btnCompressAudio.Click += new System.EventHandler(this.btnCompressAudio_Click);
            // 
            // txtBoxSrcFolder
            // 
            this.txtBoxSrcFolder.Location = new System.Drawing.Point(117, 22);
            this.txtBoxSrcFolder.Name = "txtBoxSrcFolder";
            this.txtBoxSrcFolder.Size = new System.Drawing.Size(238, 22);
            this.txtBoxSrcFolder.TabIndex = 1;
            // 
            // lblSrcFolder
            // 
            this.lblSrcFolder.AutoSize = true;
            this.lblSrcFolder.Location = new System.Drawing.Point(12, 25);
            this.lblSrcFolder.Name = "lblSrcFolder";
            this.lblSrcFolder.Size = new System.Drawing.Size(92, 16);
            this.lblSrcFolder.TabIndex = 2;
            this.lblSrcFolder.Text = "Source Folder";
            // 
            // btnBrowseSrcFolder
            // 
            this.btnBrowseSrcFolder.Location = new System.Drawing.Point(362, 22);
            this.btnBrowseSrcFolder.Name = "btnBrowseSrcFolder";
            this.btnBrowseSrcFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseSrcFolder.TabIndex = 3;
            this.btnBrowseSrcFolder.Text = "Browse";
            this.btnBrowseSrcFolder.UseVisualStyleBackColor = true;
            this.btnBrowseSrcFolder.Click += new System.EventHandler(this.btnBrowseArtFolder_Click);
            // 
            // sliderOggQuality
            // 
            this.sliderOggQuality.Location = new System.Drawing.Point(200, 76);
            this.sliderOggQuality.Name = "sliderOggQuality";
            this.sliderOggQuality.Size = new System.Drawing.Size(104, 56);
            this.sliderOggQuality.TabIndex = 4;
            this.sliderOggQuality.Value = 5;
            this.sliderOggQuality.ValueChanged += new System.EventHandler(this.sliderOggQuality_ValueChanged);
            // 
            // lblOggQuality
            // 
            this.lblOggQuality.AutoSize = true;
            this.lblOggQuality.Location = new System.Drawing.Point(12, 76);
            this.lblOggQuality.Name = "lblOggQuality";
            this.lblOggQuality.Size = new System.Drawing.Size(182, 16);
            this.lblOggQuality.TabIndex = 5;
            this.lblOggQuality.Text = "OGG Quality (Higher is better)";
            // 
            // lblOggQualityValue
            // 
            this.lblOggQualityValue.AutoSize = true;
            this.lblOggQualityValue.Location = new System.Drawing.Point(310, 76);
            this.lblOggQualityValue.Name = "lblOggQualityValue";
            this.lblOggQualityValue.Size = new System.Drawing.Size(14, 16);
            this.lblOggQualityValue.TabIndex = 6;
            this.lblOggQualityValue.Text = "5";
            // 
            // lblPrefixName
            // 
            this.lblPrefixName.AutoSize = true;
            this.lblPrefixName.Location = new System.Drawing.Point(12, 116);
            this.lblPrefixName.Name = "lblPrefixName";
            this.lblPrefixName.Size = new System.Drawing.Size(118, 16);
            this.lblPrefixName.TabIndex = 8;
            this.lblPrefixName.Text = "Prefix Blend Name";
            // 
            // txtBoxPrefixName
            // 
            this.txtBoxPrefixName.Location = new System.Drawing.Point(151, 116);
            this.txtBoxPrefixName.Name = "txtBoxPrefixName";
            this.txtBoxPrefixName.Size = new System.Drawing.Size(217, 22);
            this.txtBoxPrefixName.TabIndex = 7;
            // 
            // btnBrowseTgtFolder
            // 
            this.btnBrowseTgtFolder.Location = new System.Drawing.Point(362, 51);
            this.btnBrowseTgtFolder.Name = "btnBrowseTgtFolder";
            this.btnBrowseTgtFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseTgtFolder.TabIndex = 11;
            this.btnBrowseTgtFolder.Text = "Browse";
            this.btnBrowseTgtFolder.UseVisualStyleBackColor = true;
            this.btnBrowseTgtFolder.Click += new System.EventHandler(this.btnBrowseTgtArtFolder_Click);
            // 
            // lblTgtFolder
            // 
            this.lblTgtFolder.AutoSize = true;
            this.lblTgtFolder.Location = new System.Drawing.Point(12, 54);
            this.lblTgtFolder.Name = "lblTgtFolder";
            this.lblTgtFolder.Size = new System.Drawing.Size(89, 16);
            this.lblTgtFolder.TabIndex = 10;
            this.lblTgtFolder.Text = "Target Folder";
            // 
            // txtBoxTgtFolder
            // 
            this.txtBoxTgtFolder.Location = new System.Drawing.Point(117, 51);
            this.txtBoxTgtFolder.Name = "txtBoxTgtFolder";
            this.txtBoxTgtFolder.Size = new System.Drawing.Size(238, 22);
            this.txtBoxTgtFolder.TabIndex = 9;
            // 
            // btnFixJbeams
            // 
            this.btnFixJbeams.Location = new System.Drawing.Point(12, 175);
            this.btnFixJbeams.Name = "btnFixJbeams";
            this.btnFixJbeams.Size = new System.Drawing.Size(129, 25);
            this.btnFixJbeams.TabIndex = 12;
            this.btnFixJbeams.Text = "Fix J-Beam";
            this.btnFixJbeams.UseVisualStyleBackColor = true;
            this.btnFixJbeams.Click += new System.EventHandler(this.btnFixJbeams_Click);
            // 
            // bgworkerWorkCompressAudio
            // 
            this.bgworkerWorkCompressAudio.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 212);
            this.Controls.Add(this.btnFixJbeams);
            this.Controls.Add(this.btnBrowseTgtFolder);
            this.Controls.Add(this.lblTgtFolder);
            this.Controls.Add(this.txtBoxTgtFolder);
            this.Controls.Add(this.lblPrefixName);
            this.Controls.Add(this.txtBoxPrefixName);
            this.Controls.Add(this.lblOggQualityValue);
            this.Controls.Add(this.lblOggQuality);
            this.Controls.Add(this.sliderOggQuality);
            this.Controls.Add(this.btnBrowseSrcFolder);
            this.Controls.Add(this.lblSrcFolder);
            this.Controls.Add(this.txtBoxSrcFolder);
            this.Controls.Add(this.btnCompressAudio);
            this.Name = "MainForm";
            this.Text = "BeamNG Audio Compressor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sliderOggQuality)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCompressAudio;
        private System.Windows.Forms.FolderBrowserDialog dlgBrowseFolder;
        private System.Windows.Forms.TextBox txtBoxSrcFolder;
        private System.Windows.Forms.Label lblSrcFolder;
        private System.Windows.Forms.Button btnBrowseSrcFolder;
        private System.Windows.Forms.TrackBar sliderOggQuality;
        private System.Windows.Forms.Label lblOggQuality;
        private System.Windows.Forms.Label lblOggQualityValue;
        private System.Windows.Forms.Label lblPrefixName;
        private System.Windows.Forms.TextBox txtBoxPrefixName;
        private System.Windows.Forms.Button btnBrowseTgtFolder;
        private System.Windows.Forms.Label lblTgtFolder;
        private System.Windows.Forms.TextBox txtBoxTgtFolder;
        private System.Windows.Forms.Button btnFixJbeams;
        private System.ComponentModel.BackgroundWorker bgworkerWorkCompressAudio;
    }
}

