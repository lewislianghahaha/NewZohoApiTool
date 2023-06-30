using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZohoApiTool
{
    public partial class Main : Form
    {
        private System.Timers.Timer _myTimer = new System.Timers.Timer();
        private int _logipd = 0;

        public Main()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            this.SizeChanged += Main_SizeChanged;
            nicon.Click += Nicon_Click;
            tmclick.Click += Tmclick_Click;
            pbar.Visible = false;
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmclick_Click(object sender, System.EventArgs e)
        {
            try
            {
                LogHelper.WriteLog("OK");

            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("出现异常:", ex);
            }
        }




        /// <summary>
        /// 窗体最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nicon_Click(object sender, System.EventArgs e)
        {
            // 正常显示窗体
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// 窗体最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_SizeChanged(object sender, System.EventArgs e)
        {
            // 判断只有最小化时，隐藏窗体
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }
    }
}
