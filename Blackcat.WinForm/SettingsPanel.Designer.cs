namespace Blackcat.WinForm
{
    partial class SettingsPanel
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.labelDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 46);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(303, 370);
            this.propertyGrid1.TabIndex = 0;
            // 
            // labelDescription
            // 
            this.labelDescription.AutoEllipsis = true;
            this.labelDescription.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelDescription.Location = new System.Drawing.Point(0, 0);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(303, 46);
            this.labelDescription.TabIndex = 2;
            this.labelDescription.Text = "...";
            this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.labelDescription);
            this.Name = "SettingsPanel";
            this.Size = new System.Drawing.Size(303, 416);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Label labelDescription;
    }
}
