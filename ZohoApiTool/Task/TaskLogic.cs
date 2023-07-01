namespace ZohoApiTool.Task
{
    //任务分布(中转站)
    public class TaskLogic
    {
        Generate generate=new Generate();

        #region 变量定义
        private bool _resultMark;        //返回是否成功标记
        #endregion

        #region Get
        /// <summary>
        /// 返回结果标记
        /// </summary>
        public bool ResultMark => _resultMark;
        #endregion

        /// <summary>
        /// 核心运算
        /// </summary>
        public void GenerateRecord()
        {
            _resultMark = generate.GenerateRecord();
        }

    }
}
