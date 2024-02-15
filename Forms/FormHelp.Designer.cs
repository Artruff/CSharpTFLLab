namespace CSharpTFLLab.Forms
{
    partial class FormHelp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHelp));
            this.HelpTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // HelpTextBox
            // 
            this.HelpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpTextBox.Location = new System.Drawing.Point(13, 13);
            this.HelpTextBox.Name = "HelpTextBox";
            this.HelpTextBox.ReadOnly = true;
            this.HelpTextBox.Size = new System.Drawing.Size(429, 422);
            this.HelpTextBox.TabIndex = 0;
            this.HelpTextBox.Text = resources.GetString("HelpTextBox.Text");
            // 
            // FormHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 447);
            this.Controls.Add(this.HelpTextBox);
            this.Name = "FormHelp";
            this.Text = "Справка";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox HelpTextBox;
    }
}