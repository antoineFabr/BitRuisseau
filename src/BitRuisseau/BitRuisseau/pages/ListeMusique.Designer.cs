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
            ListeSong = new ListView();
            colTitle = new ColumnHeader();
            colArtist = new ColumnHeader();
            colAlbum = new ColumnHeader();
            colDuration = new ColumnHeader();
            colSize = new ColumnHeader();
            Load = new Button();
            label2 = new Label();
            ListeRemoteSong = new ListView();
            colRemTitle = new ColumnHeader();
            colRemArtist = new ColumnHeader();
            colRemAlbum = new ColumnHeader();
            colRemDuration = new ColumnHeader();
            colRemSize = new ColumnHeader();
            colRemHolders = new ColumnHeader();
            label1 = new Label();
            Refresh = new Button();
            SuspendLayout();
            // 
            // ListeSong
            // 
            ListeSong.Columns.AddRange(new ColumnHeader[] { colTitle, colArtist, colAlbum, colDuration, colSize });
            ListeSong.FullRowSelect = true;
            ListeSong.GridLines = true;
            ListeSong.Location = new Point(2, 66);
            ListeSong.Name = "ListeSong";
            ListeSong.Size = new Size(451, 264);
            ListeSong.TabIndex = 0;
            ListeSong.UseCompatibleStateImageBehavior = false;
            ListeSong.View = View.Details;
            ListeSong.DoubleClick += ListeSong_DoubleClick;
            // 
            // colTitle
            // 
            colTitle.Text = "Titre";
            colTitle.Width = 120;
            // 
            // colArtist
            // 
            colArtist.Text = "Artiste";
            colArtist.Width = 100;
            // 
            // colAlbum
            // 
            colAlbum.Text = "Album";
            colAlbum.Width = 100;
            // 
            // colDuration
            // 
            colDuration.Text = "Durée";
            // 
            // colSize
            // 
            colSize.Text = "Taille";
            // 
            // Load
            // 
            Load.Location = new Point(223, 336);
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
            ListeRemoteSong.Columns.AddRange(new ColumnHeader[] { colRemTitle, colRemArtist, colRemAlbum, colRemDuration, colRemSize, colRemHolders });
            ListeRemoteSong.FullRowSelect = true;
            ListeRemoteSong.GridLines = true;
            ListeRemoteSong.Location = new Point(459, 66);
            ListeRemoteSong.Name = "ListeRemoteSong";
            ListeRemoteSong.Size = new Size(493, 264);
            ListeRemoteSong.TabIndex = 4;
            ListeRemoteSong.UseCompatibleStateImageBehavior = false;
            ListeRemoteSong.View = View.Details;
            ListeRemoteSong.DoubleClick += ListeRemoteSong_DoubleClick;
            // 
            // colRemTitle
            // 
            colRemTitle.Text = "Titre";
            colRemTitle.Width = 120;
            // 
            // colRemArtist
            // 
            colRemArtist.Text = "Artiste";
            colRemArtist.Width = 100;
            // 
            // colRemAlbum
            // 
            colRemAlbum.Text = "Album";
            colRemAlbum.Width = 100;
            // 
            // colRemDuration
            // 
            colRemDuration.Text = "Durée";
            // 
            // colRemSize
            // 
            colRemSize.Text = "Taille";
            // 
            // colRemHolders
            // 
            colRemHolders.Text = "Sources";
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
            Refresh.Location = new Point(459, 336);
            Refresh.Name = "Refresh";
            Refresh.Size = new Size(166, 44);
            Refresh.TabIndex = 6;
            Refresh.Text = "Refresh";
            Refresh.UseVisualStyleBackColor = true;
            Refresh.Click += refresh_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(950, 450);
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

        private ListView ListeSong;
        private Button Load;
        private Label label2;
        private ListView ListeRemoteSong;
        private Label label1;
        private Button Refresh;
        private ColumnHeader colTitle;
        private ColumnHeader colArtist;
        private ColumnHeader colAlbum;
        private ColumnHeader colDuration;
        private ColumnHeader colSize;
        private ColumnHeader colRemTitle;
        private ColumnHeader colRemArtist;
        private ColumnHeader colRemAlbum;
        private ColumnHeader colRemDuration;
        private ColumnHeader colRemSize;
        private ColumnHeader colRemHolders;
    }
}