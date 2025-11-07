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
            this.ListeSong = new System.Windows.Forms.ListBox();
            this.Load = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ListeSong
            // 
            this.ListeSong.FormattingEnabled = true;
            this.ListeSong.ItemHeight = 15;
            this.ListeSong.Location = new System.Drawing.Point(203, 86);
            this.ListeSong.Name = "ListeSong";
            this.ListeSong.Size = new System.Drawing.Size(346, 229);
            this.ListeSong.TabIndex = 0;
            // 
            // Load
            // 
            this.Load.Location = new System.Drawing.Point(285, 347);
            this.Load.Name = "Load";
            this.Load.Size = new System.Drawing.Size(189, 44);
            this.Load.TabIndex = 1;
            this.Load.Text = "Load Music";
            this.Load.UseVisualStyleBackColor = true;
            this.Load.Click += new System.EventHandler(this.Load_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Load);
            this.Controls.Add(this.ListeSong);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private ListBox ListeSong;
        private Button Load;
    }
}