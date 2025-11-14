namespace BitRuisseau
{
    partial class Form1
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
            ListeSong = new ListBox();
            Load = new Button();
            label2 = new Label();
            ListeRemoteSong = new ListBox();
            label1 = new Label();
            Refresh = new Button();
            SuspendLayout();
            // 
            // ListeSong
            // 
            ListeSong.FormattingEnabled = true;
            ListeSong.ItemHeight = 15;
            ListeSong.Location = new Point(12, 66);
            ListeSong.Name = "ListeSong";
            ListeSong.Size = new Size(261, 229);
            ListeSong.TabIndex = 0;
            ListeSong.SelectedIndexChanged += ListeSong_SelectedIndexChanged;
            ListeSong.DoubleClick += ListeSong_DoubleClick;
            // 
            // Load
            // 
            Load.Location = new Point(46, 301);
            Load.Name = "Load";
            Load.Size = new Size(189, 44);
            Load.TabIndex = 1;
            Load.Text = "Load Music";
            Load.UseVisualStyleBackColor = true;
            Load.Click += Load_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(72, 48);
            label2.Name = "label2";
            label2.Size = new Size(130, 15);
            label2.TabIndex = 3;
            label2.Text = "Votre musique en Local";
            // 
            // ListeRemoteSong
            // 
            ListeRemoteSong.FormattingEnabled = true;
            ListeRemoteSong.ItemHeight = 15;
            ListeRemoteSong.Location = new Point(469, 66);
            ListeRemoteSong.Name = "ListeRemoteSong";
            ListeRemoteSong.Size = new Size(255, 229);
            ListeRemoteSong.TabIndex = 4;
            ListeRemoteSong.SelectedIndexChanged += ListeRemoteSong_OnClickItems;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(509, 48);
            label1.Name = "label1";
            label1.Size = new Size(184, 15);
            label1.TabIndex = 5;
            label1.Text = "Catalogue de musique disponible";
            // 
            // Refresh
            // 
            Refresh.Location = new Point(542, 301);
            Refresh.Name = "Refresh";
            Refresh.Size = new Size(123, 33);
            Refresh.TabIndex = 6;
            Refresh.Text = "Refresh";
            Refresh.UseVisualStyleBackColor = true;
            Refresh.Click += refresh_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Refresh);
            Controls.Add(label1);
            Controls.Add(ListeRemoteSong);
            Controls.Add(label2);
            Controls.Add(Load);
            Controls.Add(ListeSong);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private ListBox ListeSong;
        private Button Load;
        private Label label2;
        private ListBox ListeRemoteSong;
        private Label label1;
        private Button Refresh;
    }
}