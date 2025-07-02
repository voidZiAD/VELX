namespace VELX.UI
{
    partial class VELXNotif
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
            panel1 = new Panel();
            X = new VELX.Custom_UI_Elements.GradientTextLabel();
            VEL = new Label();
            pbIcon = new PictureBox();
            lbError = new Label();
            ((System.ComponentModel.ISupportInitialize)pbIcon).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.Purple;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(5, 113);
            panel1.TabIndex = 0;
            // 
            // X
            // 
            X.AutoSize = true;
            X.BackColor = Color.Transparent;
            X.Font = new Font("Poppins", 30F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            X.GradientAngle = 90F;
            X.GradientEnd = Color.Purple;
            X.GradientStart = Color.Plum;
            X.Location = new Point(48, -15);
            X.Name = "X";
            X.Size = new Size(73, 88);
            X.TabIndex = 4;
            X.Text = "X";
            // 
            // VEL
            // 
            VEL.AutoSize = true;
            VEL.Font = new Font("Poppins", 20F, FontStyle.Bold | FontStyle.Italic);
            VEL.ForeColor = Color.White;
            VEL.Location = new Point(8, -1);
            VEL.Name = "VEL";
            VEL.Size = new Size(84, 60);
            VEL.TabIndex = 5;
            VEL.Text = "VEL";
            // 
            // pbIcon
            // 
            pbIcon.BackColor = Color.Transparent;
            pbIcon.Image = Properties.Resources.icons8_so_so_24;
            pbIcon.Location = new Point(21, 55);
            pbIcon.Name = "pbIcon";
            pbIcon.Size = new Size(24, 24);
            pbIcon.SizeMode = PictureBoxSizeMode.AutoSize;
            pbIcon.TabIndex = 12;
            pbIcon.TabStop = false;
            // 
            // lbError
            // 
            lbError.AutoSize = true;
            lbError.Font = new Font("Poppins", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbError.ForeColor = Color.FromArgb(150, 150, 150);
            lbError.Location = new Point(49, 54);
            lbError.Name = "lbError";
            lbError.Size = new Size(32, 23);
            lbError.TabIndex = 13;
            lbError.Text = "null";
            // 
            // VELXNotif
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(25, 25, 25);
            ClientSize = new Size(250, 113);
            ControlBox = false;
            Controls.Add(lbError);
            Controls.Add(pbIcon);
            Controls.Add(X);
            Controls.Add(VEL);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "VELXNotif";
            ShowIcon = false;
            ShowInTaskbar = false;
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)pbIcon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Custom_UI_Elements.GradientTextLabel X;
        private Label VEL;
        private PictureBox pbIcon;
        private Label lbError;
    }
}