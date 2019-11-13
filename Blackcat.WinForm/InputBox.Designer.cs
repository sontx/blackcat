namespace Blackcat.WinForm
{
    partial class InputBox
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
            this.labDescription = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.pnlWorkingSpace.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWorkingSpace
            // 
            this.pnlWorkingSpace.Controls.Add(this.labDescription);
            this.pnlWorkingSpace.Controls.Add(this.txtInput);
            this.pnlWorkingSpace.Padding = new System.Windows.Forms.Padding(10, 8, 10, 18);
            this.pnlWorkingSpace.Size = new System.Drawing.Size(389, 108);
            // 
            // pnlControlSpace
            // 
            this.pnlControlSpace.Location = new System.Drawing.Point(0, 108);
            this.pnlControlSpace.Size = new System.Drawing.Size(389, 42);
            // 
            // labDescription
            // 
            this.labDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labDescription.Location = new System.Drawing.Point(10, 8);
            this.labDescription.Name = "labDescription";
            this.labDescription.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.labDescription.Size = new System.Drawing.Size(369, 60);
            this.labDescription.TabIndex = 0;
            this.labDescription.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtInput
            // 
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtInput.Location = new System.Drawing.Point(10, 68);
            this.txtInput.Name = "txtInput";
            this.txtInput.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtInput.Size = new System.Drawing.Size(369, 22);
            this.txtInput.TabIndex = 1;
            // 
            // InputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 150);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "InputBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InputBox";
            this.pnlWorkingSpace.ResumeLayout(false);
            this.pnlWorkingSpace.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label labDescription;
    }
}