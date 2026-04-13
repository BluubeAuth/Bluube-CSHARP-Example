namespace LoginC_
{
    partial class Home
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
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            userInfoList = new ListBox();
            minimize = new Guna.UI2.WinForms.Guna2ControlBox();
            guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.BorderRadius = 25;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.HasFormShadow = false;
            guna2BorderlessForm1.ResizeForm = false;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("AniMe Vision - MB_EN", 20.2499981F, FontStyle.Regular, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.ForeColor = Color.White;
            guna2HtmlLabel1.Location = new Point(32, 48);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(220, 34);
            guna2HtmlLabel1.TabIndex = 2;
            guna2HtmlLabel1.Text = "Authenticated";
            // 
            // userInfoList
            // 
            userInfoList.BackColor = Color.FromArgb(22, 22, 22);
            userInfoList.BorderStyle = BorderStyle.FixedSingle;
            userInfoList.Font = new Font("Segoe UI", 10F);
            userInfoList.ForeColor = Color.White;
            userInfoList.FormattingEnabled = true;
            userInfoList.ItemHeight = 22;
            userInfoList.Location = new Point(32, 88);
            userInfoList.Margin = new Padding(3, 4, 3, 4);
            userInfoList.Name = "userInfoList";
            userInfoList.Size = new Size(482, 290);
            userInfoList.TabIndex = 8;
            // 
            // minimize
            // 
            minimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            minimize.BorderColor = Color.Transparent;
            minimize.ControlBoxStyle = Guna.UI2.WinForms.Enums.ControlBoxStyle.Custom;
            minimize.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            minimize.CustomizableEdges = customizableEdges5;
            minimize.FillColor = Color.Transparent;
            minimize.HoverState.BorderColor = Color.Transparent;
            minimize.HoverState.FillColor = Color.Transparent;
            minimize.HoverState.IconColor = Color.Gray;
            minimize.IconColor = Color.White;
            minimize.Location = new Point(473, 1);
            minimize.Name = "minimize";
            minimize.PressedColor = Color.Transparent;
            minimize.ShadowDecoration.CustomizableEdges = customizableEdges6;
            minimize.Size = new Size(45, 29);
            minimize.TabIndex = 7;
            // 
            // guna2ControlBox1
            // 
            guna2ControlBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            guna2ControlBox1.BorderColor = Color.Transparent;
            guna2ControlBox1.ControlBoxStyle = Guna.UI2.WinForms.Enums.ControlBoxStyle.Custom;
            guna2ControlBox1.CustomizableEdges = customizableEdges7;
            guna2ControlBox1.FillColor = Color.Transparent;
            guna2ControlBox1.HoverState.BorderColor = Color.Transparent;
            guna2ControlBox1.HoverState.FillColor = Color.Transparent;
            guna2ControlBox1.HoverState.IconColor = Color.IndianRed;
            guna2ControlBox1.IconColor = Color.White;
            guna2ControlBox1.Location = new Point(503, 4);
            guna2ControlBox1.Name = "guna2ControlBox1";
            guna2ControlBox1.PressedColor = Color.Transparent;
            guna2ControlBox1.ShadowDecoration.CustomizableEdges = customizableEdges8;
            guna2ControlBox1.Size = new Size(45, 29);
            guna2ControlBox1.TabIndex = 6;
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(16, 16, 16);
            ClientSize = new Size(546, 450);
            Controls.Add(userInfoList);
            Controls.Add(minimize);
            Controls.Add(guna2ControlBox1);
            Controls.Add(guna2HtmlLabel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Home";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Home";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private ListBox userInfoList;
        private Guna.UI2.WinForms.Guna2ControlBox minimize;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
    }
}
