namespace Blackcat.WinForm
{
    partial class VerticalLayoutForm
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
            this.pnlWorkingSpace = new System.Windows.Forms.Panel();
            this.pnlControlSpace = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlWorkingSpace
            // 
            this.pnlWorkingSpace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWorkingSpace.Location = new System.Drawing.Point(0, 0);
            this.pnlWorkingSpace.Name = "pnlWorkingSpace";
            this.pnlWorkingSpace.Size = new System.Drawing.Size(587, 331);
            this.pnlWorkingSpace.TabIndex = 0;
            // 
            // pnlControlSpace
            // 
            this.pnlControlSpace.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControlSpace.Location = new System.Drawing.Point(0, 331);
            this.pnlControlSpace.Name = "pnlControlSpace";
            this.pnlControlSpace.Size = new System.Drawing.Size(587, 62);
            this.pnlControlSpace.TabIndex = 1;
            // 
            // VerticalLayoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 393);
            this.Controls.Add(this.pnlWorkingSpace);
            this.Controls.Add(this.pnlControlSpace);
            this.Name = "VerticalLayoutForm";
            this.Text = "VerticalLayoutForm";
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Panel pnlWorkingSpace;
        protected System.Windows.Forms.Panel pnlControlSpace;
    }
}