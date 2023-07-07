using System;
using System.Data;
using System.Windows.Forms;
using ZohoApiTool.Task;

namespace ZohoApiTool
{
    public partial class Main : Form
    {
        TaskLogic task=new TaskLogic();

        private System.Timers.Timer _myTimer = new System.Timers.Timer();
        //作用:用于设定同一天只能在设定的时间内执行一次
        private int _logipd = 0;
        //收集前端选择的时间参数
        private string _genTime = "";

        //定义委托(调用子线程使用)
        delegate void SetCallBack();

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
            OnShowHourList();
            OnShowSecondList();
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmclick_Click(object sender, EventArgs e)
        {
            try
            {
                if (tmclick.Text == $"开始执行")
                {
                    if (txtmessage.Text != "")
                    {
                        txtmessage.Text = "";
                    }

                    tmclick.Text = $"结束执行";

                    //获取下拉列表所选值(时)
                    var dvHour = (DataRowView)comhour.Items[comhour.SelectedIndex];
                    //获取下拉列表所选值(分)
                    var dvmin = (DataRowView)commin.Items[commin.SelectedIndex];
                    //对所选择的‘’及‘分’进行数据组合
                    _genTime = $"{Convert.ToString(dvHour["Id"])}:{Convert.ToString(dvmin["Id"])}";

                    //设置两个下拉列表为不可操作
                    comhour.Enabled = false;
                    commin.Enabled = false;

                    //设置显示信息
                    lbmessage.Text = $"在每天{_genTime}执行,将从'{DateTime.Now.ToString("yyyy-MM-dd")}'开始执行计划";

                    //对_myTimer对象进行相关设置
                    _myTimer.Interval = 1000;
                    _myTimer.Elapsed += _myTimer_Elapsed;
                    _myTimer.AutoReset = true;
                    _myTimer.Enabled = true;
                    txtmessage.AppendText($"开始执行=>");
                    LogHelper.WriteLog("开始执行=>");
                }
                //关闭执行
                else
                {
                    tmclick.Text = $"开始执行";
                    txtmessage.AppendText($"\r\n" + $"定时执行停止,暂不执行任务");
                    //设置添加文本后自动滚动显示到最后一行
                    txtmessage.ScrollToCaret();
                    _myTimer.Stop();
                    pbar.Visible = false;
                    comhour.Enabled = true;
                    commin.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("出现异常,原因:", ex);
            }
        }

        /// <summary>
        /// 定时达到后执行(调用委托)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                var deg = new SetCallBack(GenerateRecord);
                this.Invoke(deg, new object[] {});
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("Elapsed出现异常:", ex);
            }
        }

        /// <summary>
        /// 核心运算入口
        /// </summary>
        private void GenerateRecord()
        {
            var now = DateTime.Now;

            if (DateTime.Compare(Convert.ToDateTime(now.ToString("HH:mm")), Convert.ToDateTime(_genTime)) > 0)
            {
                pbar.Visible = false;
                _logipd = 0;
                //txtmessage.AppendText($"\r\n" + now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $" 时间超过，不执行任务");
                ////设置添加文本后自动滚动显示到最后一行
                //txtmessage.ScrollToCaret();
            }
            else if (now.ToString("HH:mm") == _genTime && _logipd == 0)
            {
                tmclick.Enabled = false;
                pbar.Visible = true;
                //将内容插入至多行文本(与+=一样作用) 换行\r\n （或System.Environment.NewLine）
                txtmessage.AppendText($"\r\n" + now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $" 时间已到,开始执行任务");

                //执行核心运算
                //task.GenerateRecord();



                //根据返回值确定是否成功
                if (!task.ResultMark)
                {
                    var now1 = DateTime.Now;
                    txtmessage.AppendText($"\r\n" + now1.ToString("yyyy-MM-dd HH:mm:ss.fff") + $" 出现异常,请查看日志信息");
                }
                else
                {
                    var now2 = DateTime.Now;
                    txtmessage.AppendText($"\r\n" + now2.ToString("yyyy-MM-dd HH:mm:ss.fff") + $" 执行结束");
                }
                //设置添加文本后自动滚动显示到最后一行
                txtmessage.ScrollToCaret();
                //作用:用于设定同一天只能在设定的时间内执行一次
                _logipd = 1;
                //当运算成功后,点击按钮才可以继续执行
                tmclick.Enabled = true;
            }
            else
            {
                pbar.Visible = false;
                //txtmessage.AppendText($"\r\n" + now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $" 暂不执行任务");
                ////LogHelper.WriteLog(now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "时间已到，执行任务");
                ////设置添加文本后自动滚动显示到最后一行
                //txtmessage.ScrollToCaret();
            }
        }


        /// <summary>
        ///'时'下拉列表
        /// </summary>
        private void OnShowHourList()
        {
            var dt = new DataTable();
            var id = 0;

            //创建表头
            for (var i = 0; i < 2; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    case 0:
                        dc.ColumnName = "Id";
                        break;
                    case 1:
                        dc.ColumnName = "Name";
                        break;
                }
                dt.Columns.Add(dc);
            }

            //创建行内容
            for (var j = 0; j < 24; j++)
            {
                var dr = dt.NewRow();
                dr[id] = j <= 9 ? (object)("0" + $"{j}") : $"{j}";
                dr[id + 1] = j <= 9 ? (object)("0" + $"{j}") : $"{j}";
                dt.Rows.Add(dr);
                id = 0;
            }

            comhour.DataSource = dt;
            comhour.DisplayMember = "Name"; //设置显示值
            comhour.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        ///‘分’下拉列表
        /// </summary>
        private void OnShowSecondList()
        {
            var dt = new DataTable();
            var id = 0;

            //创建表头
            for (var i = 0; i < 2; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    case 0:
                        dc.ColumnName = "Id";
                        break;
                    case 1:
                        dc.ColumnName = "Name";
                        break;
                }
                dt.Columns.Add(dc);
            }

            //创建行内容
            for (var j = 0; j < 60; j++)
            {
                var dr = dt.NewRow();
                dr[id] = j <= 9 ? ("0" + $"{j}") : $"{j}";
                dr[id + 1] = j <= 9 ? ("0" + $"{j}") : $"{j}";
                dt.Rows.Add(dr);
                id = 0;
            }

            commin.DataSource = dt;
            commin.DisplayMember = "Name"; //设置显示值
            commin.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        /// 窗体最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nicon_Click(object sender, EventArgs e)
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
        private void Main_SizeChanged(object sender, EventArgs e)
        {
            // 判断只有最小化时，隐藏窗体
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }
    }
}
