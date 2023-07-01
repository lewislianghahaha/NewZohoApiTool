using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ZohoApiTool.DB
{
    //获取连接字符串,并创建SqlConnection
    public class ConDb
    {
        /// <summary>
        /// 获取配方系统数据库连接
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConnection()
        {
            var sqlcon = new SqlConnection();

            try
            {
                sqlcon = new SqlConnection(GetConnectionString());
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("获取SqlConnection出现异常,原因:",ex);
            }
            return sqlcon;
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            var result = "";

            try
            {
                //读取App.Config配置文件中的Connstring节点
                var pubs = ConfigurationManager.ConnectionStrings["ConnString"];
                result = pubs.ConnectionString;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("获取连接字符串出现异常,原因:", ex);
            }
            return result;
        }
    }
}
