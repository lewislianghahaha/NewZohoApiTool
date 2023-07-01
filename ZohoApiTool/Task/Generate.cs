using System;

namespace ZohoApiTool.Task
{
    //运算
    public class Generate
    {
        GetApiRecord getApi=new GetApiRecord();
        SearchDt searchDt=new SearchDt();

        /// <summary>
        /// 核心运算
        /// </summary>
        /// <returns></returns>
        public bool GenerateRecord()
        {
            var result = true;

            try
            {

            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.WriteErrorLog("执行过程中出现异常,原因:",ex);
            }

            return result;
        }



    }
}
