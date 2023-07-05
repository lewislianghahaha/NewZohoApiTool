using System;
using System.Data;
using System.Data.SqlClient;
using ZohoApiTool.DB;

namespace ZohoApiTool.Task
{
    //查询
    public class SearchDt
    {
        ConDb conDb = new ConDb();
        SqlList sqlList = new SqlList();

        /// <summary>
        /// 根据SQL语句查询得出对应的DT
        /// </summary>
        /// <param name="sqlscript"></param>
        /// <returns></returns>
        private DataTable UseSqlSearchIntoDt(string sqlscript)
        {
            var resultdt = new DataTable();

            try
            {
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, conDb.GetConnection());
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }

            return resultdt;
        }

        /// <summary>
        /// 按照指定的SQL语句执行记录并返回执行结果（true 或 false） 更新 删除时使用
        /// </summary>
        private bool Generdt(string sqlscript)
        {
            var result = true;

            try
            {
                using (var sql = conDb.GetConnection())
                {
                    sql.Open();
                    var sqlCommand = new SqlCommand(sqlscript, sql);
                    sqlCommand.ExecuteNonQuery();
                    sql.Close();
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 初始化获取表记录;注:只是第一次使用工具时会执行
        /// </summary>
        /// <returns></returns>
        public DataTable GetIninitalRecord()
        {
            var dt = UseSqlSearchIntoDt(sqlList.GetSearchNum());
            return dt;
        }

        /// <summary>
        /// 初始化获取T_BOOKS_SAL表记录(注:只获取上月1号至当天 且 IsDel=1的记录)
        /// </summary>
        /// <returns></returns>
        public DataTable GetBooksSalHeadRecord()
        {
            var dt = UseSqlSearchIntoDt(sqlList.GetSearchBooksSalHead());
            return dt;
        }

        /// <summary>
        /// 初始化根据表头信息获取表体line_item_id记录 (作用:用于判断已存在的明细记录是否删除)
        /// </summary>
        /// <returns></returns>
        public DataTable GetBooksSalDetailRecord()
        {
            var dt = UseSqlSearchIntoDt(sqlList.GetSearchBooksSalDtl());
            return dt;
        }

        /// <summary>
        /// 对‘不存在’的记录进行更新IsDel标记--'监盘操作'
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="uplist"></param>
        /// <returns></returns>
        public bool UpIsDelRecord(int typeid, string uplist)
        {
            var sqlscript = sqlList.UpIsDelRecord(typeid, uplist);
            return Generdt(sqlscript);
        }

    }
}
