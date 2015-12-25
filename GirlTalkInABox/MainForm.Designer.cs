namespace GirlTalkInABox
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.songLabel = new System.Windows.Forms.Label();
            this.newSongButton = new System.Windows.Forms.Button();
            this._visualizer = new GirlTalkInABox.Visualizer();
            ((System.ComponentModel.ISupportInitialize)(this._visualizer)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Audio File | *.mp3; *.ogg; *.wav";
            // 
            // songLabel
            // 
            this.songLabel.AutoSize = true;
            this.songLabel.Location = new System.Drawing.Point(93, 17);
            this.songLabel.Name = "songLabel";
            this.songLabel.Size = new System.Drawing.Size(10, 13);
            this.songLabel.TabIndex = 0;
            this.songLabel.Text = " ";
            // 
            // newSongButton
            // 
            this.newSongButton.Location = new System.Drawing.Point(12, 12);
            this.newSongButton.Name = "newSongButton";
            this.newSongButton.Size = new System.Drawing.Size(75, 23);
            this.newSongButton.TabIndex = 1;
            this.newSongButton.Text = "Pick Song";
            this.newSongButton.UseVisualStyleBackColor = true;
            this.newSongButton.Click += new System.EventHandler(this.newSongButton_Click);
            // 
            // _visualizer
            // 
            this._visualizer.Location = new System.Drawing.Point(13, 42);
            this._visualizer.Name = "_visualizer";
            this._visualizer.Size = new System.Drawing.Size(450, 286);
            this._visualizer.TabIndex = 2;
            this._visualizer.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 340);
            this.Controls.Add(this._visualizer);
            this.Controls.Add(this.newSongButton);
            this.Controls.Add(this.songLabel);
            this.Name = "MainForm";
            this.Text = "Girl Talk In A Box";
            ((System.ComponentModel.ISupportInitialize)(this._visualizer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label songLabel;
        private System.Windows.Forms.Button newSongButton;
        private Visualizer _visualizer;
    }
}

