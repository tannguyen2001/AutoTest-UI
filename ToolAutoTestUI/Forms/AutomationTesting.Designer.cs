namespace AutomationAUI
{
    partial class frmAutomationTesting
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.btnStartTesting = new System.Windows.Forms.Button();
            this.progressBarTest = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChooseExcel = new System.Windows.Forms.Button();
            this.lblMessage = new Infragistics.Win.Misc.UltraLabel();
            this.SuspendLayout();
            // 
            // btnStartTesting
            // 
            this.btnStartTesting.BackColor = System.Drawing.Color.LightSalmon;
            this.btnStartTesting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartTesting.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartTesting.Location = new System.Drawing.Point(201, 339);
            this.btnStartTesting.Name = "btnStartTesting";
            this.btnStartTesting.Size = new System.Drawing.Size(143, 67);
            this.btnStartTesting.TabIndex = 3;
            this.btnStartTesting.Text = "Bắt đầu kiểm thử";
            this.btnStartTesting.UseVisualStyleBackColor = false;
            this.btnStartTesting.Click += new System.EventHandler(this.btnStartTesting_Click);
            // 
            // progressBarTest
            // 
            this.progressBarTest.Location = new System.Drawing.Point(12, 77);
            this.progressBarTest.Name = "progressBarTest";
            this.progressBarTest.Size = new System.Drawing.Size(343, 62);
            this.progressBarTest.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DarkGreen;
            this.label1.Location = new System.Drawing.Point(50, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(266, 32);
            this.label1.TabIndex = 9;
            this.label1.Text = "Tiến trình kiểm thử";
            // 
            // btnChooseExcel
            // 
            this.btnChooseExcel.BackColor = System.Drawing.Color.LightSalmon;
            this.btnChooseExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChooseExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChooseExcel.Location = new System.Drawing.Point(32, 339);
            this.btnChooseExcel.Name = "btnChooseExcel";
            this.btnChooseExcel.Size = new System.Drawing.Size(136, 66);
            this.btnChooseExcel.TabIndex = 1;
            this.btnChooseExcel.Text = "Chọn file test case";
            this.btnChooseExcel.UseVisualStyleBackColor = false;
            this.btnChooseExcel.Click += new System.EventHandler(this.btnChooseExcel_Click);
            // 
            // lblMessage
            // 
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.lblMessage.Appearance = appearance1;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(12, 149);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(343, 186);
            this.lblMessage.TabIndex = 11;
            this.lblMessage.Text = "Vui lòng chọn file kiểm thử";
            // 
            // frmAutomationTesting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Tan;
            this.ClientSize = new System.Drawing.Size(367, 426);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnChooseExcel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBarTest);
            this.Controls.Add(this.btnStartTesting);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAutomationTesting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kiểm thử phần mềm tự động";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnStartTesting;
        private System.Windows.Forms.ProgressBar progressBarTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnChooseExcel;
        private Infragistics.Win.Misc.UltraLabel lblMessage;
    }
}

