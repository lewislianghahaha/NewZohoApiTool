using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using ZohoApiTool.Task;

namespace ZohoApiTool
{
    public partial class Main : Form
    {
        TaskLogic task = new TaskLogic();

        private System.Timers.Timer _myTimer = new System.Timers.Timer();

        //作用:用于设定同一天只能在设定的时间内执行一次
        private int _logipd = 0;
        //收集前端选择的时间参数
        private string _genTime = "";

        //作用:用于控件同一个天内‘开始显示’描述只执行一次(0:是 1：否)
        private int _showid = 0;
        //作用:当前线程标记;Elapsed事件内的方法使用 (0:可执行 1:否)
        private static int _inTimer = 0;

        //定义委托(更新UI线程控件-开始显示使用)
        private delegate void ShowStart();

        //定义委托(更新UI线程控件-结束显示使用)
        private delegate void ShowResultToContol(bool value);


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

                    //新增子线程并执行
                    var searchDevice = new Thread(new ThreadStart(Timer));
                    searchDevice.IsBackground = true;
                    searchDevice.Start();

                    LogHelper.WriteLog($"设定时间:{_genTime}已到,开始执行=>");
                }
                //关闭执行
                else
                {
                    tmclick.Text = $"开始执行";
                    txtmessage.AppendText($"定时执行停止,暂不执行任务" + Environment.NewLine);
                    //将光标设置到末尾位置
                    //txtmessage.Select(txtmessage.Text.Length, 0);
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
        /// 子线程函数(重)
        /// </summary>
        private void Timer()
        {
            //对_myTimer对象进行相关设置
            _myTimer.Interval = 10000; //设置时间间隔10秒 (1秒=1000毫秒)
            _myTimer.AutoReset = true;
            _myTimer.Enabled = true;

            //todo:主要分三部份==>1.Elapsed 创建子线线程,用于根据不同条件进行相关操作
            //todo:2.ShowStartToContol() 及 ShowRdToControl() 为委托方法,主要用于通知委托更新指定UI线程内控件的值
            //todo:3.GenerateRecord() 核心运算方法;主体执行在此,并最后将返回值传输至ShowRdToControl() 委托方法进行更改控件显示
            _myTimer.Elapsed += (a1, a2) =>
            {
                //todo:通过Interlocked.Exchange()设置防止多线程重入(重); 达到效果:每次只允许一个线程进入以下逻辑运算
                //todo:--做法:判断标记inTimer是否为0,若为0即修改为1并进行逻辑代码,在逻辑代码执行完成后,将inTimer设置为0 (重)

                //todo:运算逻辑
                //todo:如果当前有线程正在处理定时器事件，则标志位inTimer == 1,其他线程无法进入；
                //todo:如果没有线程正在处理，则标志位inTimer == 0,该线程可以进入，并将标志位置为 1，处理完成后，再将标志位置为 0.
                if (Interlocked.Exchange(ref _inTimer, 1) == 0)
                {
                    if (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToString("HH:mm")), Convert.ToDateTime(_genTime)) > 0)
                    {
                        _logipd = 0;
                        _showid = 0;
                    }
                    else if (DateTime.Now.ToString("HH:mm") == _genTime && _logipd == 0)
                    {
                        //todo:显示开始提示
                        ShowStartToContol();
                        //todo:执行逻辑处理程序(重)
                         var result = GenerateRecord();
                        //todo:调用委托函数-输出至控件-(提示结束)
                        ShowRdToControl(result);
                        //作用:用于设定同一天只能在设定的时间内执行一次
                        _logipd = 1;
                    }
                    Interlocked.Exchange(ref _inTimer, 0);
                }
                //todo:清理内存
                GC.Collect();
                GC.WaitForPendingFinalizers();
            };
        }

        /// <summary>
        /// 定义委托方法-当时间达到开始执行时,更改控件显示值使用
        /// </summary>
        private void ShowStartToContol()
        {
            if (this.InvokeRequired)
            {
                var showStart = new ShowStart(ShowStartToContol);
                this.Invoke(showStart, new object[] { });
                //this.Invoke(new GetNum(GenerateRecord));
            }
            else
            {
                if (_showid == 0)
                {
                    //将内容插入至多行文本(与+=一样作用) 换行\r\n （或System.Environment.NewLine）
                    txtmessage.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $" 时间已到,开始执行任务"+ Environment.NewLine);
                    //将光标设置到末尾位置
                    txtmessage.Select(txtmessage.Text.Length, 0);
                    //设置添加文本后自动滚动显示到最后一行
                    txtmessage.ScrollToCaret();
                    tmclick.Enabled = false;
                    pbar.Visible = true;
                    //作用:判断同一天内只执行一次
                    _showid = 1;
                }
            }
        }

        /// <summary>
        ///  定义委托方法-当有返回值时,更改控件显示值使用
        /// </summary>
        /// <param name="value"></param>
        private void ShowRdToControl(bool value)
        {
            if (this.InvokeRequired)
            {
                var showResultToContol = new ShowResultToContol(ShowRdToControl);
                this.Invoke(showResultToContol,new object[] {value});
            }
            else
            {
                if (!value)
                {
                    txtmessage.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $" 出现异常,请查看日志信息" + Environment.NewLine);
                }
                else
                {
                    txtmessage.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $" 执行结束" + Environment.NewLine);
                }
                //将光标设置到末尾位置
                txtmessage.Select(txtmessage.Text.Length,0);
                //设置添加文本后自动滚动显示到最后一行
                txtmessage.ScrollToCaret();
                pbar.Visible = false;
                tmclick.Enabled = true;
            }
        }

        /// <summary>
        /// 核心运算入口(重)
        /// </summary>
        private bool GenerateRecord()
        {
            var result = true;

            try
            {
                //执行核心运算
                task.GenerateRecord();
                //根据返回值确定是否成功
                result = task.ResultMark;
            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.WriteErrorLog("返回结果出现异常,原因:",ex);
            }

            return result;
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
