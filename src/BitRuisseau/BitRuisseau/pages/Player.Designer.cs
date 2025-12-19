namespace BitRuisseau
{
    partial class Player
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
            lblTitle = new Label();
            btnPlayPause = new Button();
            btnStop = new Button();
            waveViewer = new NAudio.Gui.WaveViewer();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F);
            lblTitle.Location = new Point(223, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(71, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "No Song";
            // 
            // btnPlayPause
            // 
            btnPlayPause.Location = new Point(161, 243);
            btnPlayPause.Name = "btnPlayPause";
            btnPlayPause.Size = new Size(100, 40);
            btnPlayPause.TabIndex = 1;
            btnPlayPause.Text = "Play/Pause";
            btnPlayPause.UseVisualStyleBackColor = true;
            btnPlayPause.Click += btnPlayPause_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(267, 243);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(100, 40);
            btnStop.TabIndex = 2;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // waveViewer
            // 
            waveViewer.Location = new Point(12, 117);
            waveViewer.Name = "waveViewer";
            waveViewer.SamplesPerPixel = 128;
            waveViewer.Size = new Size(536, 97);
            waveViewer.StartPosition = 0L;
            waveViewer.TabIndex = 3;
            waveViewer.WaveStream = null;
            // 
            // Player
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(551, 307);
            Controls.Add(waveViewer);
            Controls.Add(btnStop);
            Controls.Add(btnPlayPause);
            Controls.Add(lblTitle);
            Name = "Player";
            Text = "Player";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnStop;
        private NAudio.Gui.WaveViewer waveViewer;
    }
}