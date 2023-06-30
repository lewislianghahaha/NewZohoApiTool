namespace ZohoApiTool
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.Menu = new System.Windows.Forms.MenuStrip();
            this.tmclick = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.commin = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comhour = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbmessage = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pbar = new System.Windows.Forms.ProgressBar();
            this.nicon = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtmessage = new System.Windows.Forms.TextBox();
            this.Menu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // Menu
            // 
            this.Menu.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmclick});
            this.Menu.Location = new System.Drawing.Point(0, 0);
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(388, 25);
            this.Menu.TabIndex = 0;
            this.Menu.Text = "Menu";
            // 
            // tmclick
            // 
            this.tmclick.Name = "tmclick";
            this.tmclick.Size = new System.Drawing.Size(68, 21);
            this.tmclick.Text = "开始执行";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.commin);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comhour);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 30);
            this.panel1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(232, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "分";
            // 
            // commin
            // 
            this.commin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.commin.FormattingEnabled = true;
            this.commin.Location = new System.Drawing.Point(171, 4);
            this.commin.Name = "commin";
            this.commin.Size = new System.Drawing.Size(57, 20);
            this.commin.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "时";
            // 
            // comhour
            // 
            this.comhour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comhour.FormattingEnabled = true;
            this.comhour.Location = new System.Drawing.Point(90, 4);
            this.comhour.Name = "comhour";
            this.comhour.Size = new System.Drawing.Size(57, 20);
            this.comhour.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "设置执行时间:";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lbmessage);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 55);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(388, 25);
            this.panel2.TabIndex = 2;
            // 
            // lbmessage
            // 
            this.lbmessage.AutoSize = true;
            this.lbmessage.Location = new System.Drawing.Point(8, 5);
            this.lbmessage.Name = "lbmessage";
            this.lbmessage.Size = new System.Drawing.Size(0, 12);
            this.lbmessage.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.pbar);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 246);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(388, 26);
            this.panel3.TabIndex = 3;
            // 
            // pbar
            // 
            this.pbar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbar.Location = new System.Drawing.Point(0, 1);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(386, 23);
            this.pbar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbar.TabIndex = 2;
            // 
            // nicon
            // 
            this.nicon.Icon = ((System.Drawing.Icon)(resources.GetObject("nicon.Icon")));
            this.nicon.Visible = true;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.txtmessage);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 80);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(388, 166);
            this.panel4.TabIndex = 4;
            // 
            // txtmessage
            // 
            this.txtmessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtmessage.Enabled = false;
            this.txtmessage.Location = new System.Drawing.Point(0, 0);
            this.txtmessage.Multiline = true;
            this.txtmessage.Name = "txtmessage";
            this.txtmessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtmessage.Size = new System.Drawing.Size(386, 164);
            this.txtmessage.TabIndex = 0;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 272);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Menu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.Menu;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "ZohoApi数据传输工具";
            this.Menu.ResumeLayout(false);
            this.Menu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip Menu;
        private System.Windows.Forms.ToolStripMenuItem tmclick;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbmessage;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ProgressBar pbar;
        private System.Windows.Forms.NotifyIcon nicon;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox txtmessage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox commin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comhour;
    }
}

