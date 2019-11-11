namespace Blackcat.WinForm
{
    partial class SettingsForm
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
            this.settingsPanel1 = new Blackcat.WinForm.SettingsPanel();
            this.pnlWorkingSpace.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWorkingSpace
            // 
            this.pnlWorkingSpace.Controls.Add(this.settingsPanel1);
            this.pnlWorkingSpace.Padding = new System.Windows.Forms.Padding(3);
            this.pnlWorkingSpace.Size = new System.Drawing.Size(296, 327);
            // 
            // pnlControlSpace
            // 
            this.pnlControlSpace.Location = new System.Drawing.Point(0, 327);
            this.pnlControlSpace.Size = new System.Drawing.Size(296, 45);
            // 
            // settingsPanel1
            // 
            this.settingsPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPanel1.Location = new System.Drawing.Point(3, 3);
            this.settingsPanel1.Name = "settingsPanel1";
            this.settingsPanel1.Settings = null;
            this.settingsPanel1.Size = new System.Drawing.Size(290, 321);
            this.settingsPanel1.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 372);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.pnlWorkingSpace.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SettingsPanel settingsPanel1;
    }
}