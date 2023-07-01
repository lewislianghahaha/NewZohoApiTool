namespace ZohoApiTool.DB
{
    //SQL语句集合
    public class SqlList
    {
        //根据SQLID返回对应的SQL语句  
        private string _result;

        /// <summary>
        /// 初始化数据使用(注:当数据库表没有任何记录时使用)
        /// </summary>
        /// <returns></returns>
        public string GetSearchNum()
        {
            _result = "SELECT COUNT(*) COUNUM FROM dbo.T_BOOKS_SAL";
            return _result;
        }

        /// <summary>
        /// 初始化获取T_BOOKS_SAL表记录(注:只获取上月1号至当天 且 IsDel=1的记录)
        /// </summary>
        /// <returns></returns>
        public string GetSearchBooksSalHead()
        {
            _result = @"SELECT TOP 9700 A.salesorder_id
                        FROM T_BOOKS_SAL A
                        WHERE A.IsDel = 1
                        AND A.CountryType = 'US'
                        AND CONVERT(VARCHAR(10), A.OrderCreateDt, 23)>= CONVERT(VARCHAR(10), DATEADD(dd, -day(dateadd(month, -1, getdate())) + 1, dateadd(month, -1, getdate())), 23)
                        AND CONVERT(VARCHAR(10), A.OrderCreateDt, 23)<= CONVERT(VARCHAR(10), GETDATE(), 23)";
            return _result;
        }

        /// <summary>
        /// 根据表头信息获取表体line_item_id记录 (作用:用于判断已存在的明细记录是否删除)
        /// </summary>
        /// <returns></returns>
        public string GetSearchBooksSalDtl()
        {
            _result = @"SELECT A.line_item_id
                        FROM T_BOOKS_SALDTL A
                        INNER JOIN (
                                        SELECT TOP 9700 A.salesorder_id
						                FROM T_BOOKS_SAL A
						                WHERE A.IsDel=1
						                AND A.CountryType='US'
						                AND CONVERT(VARCHAR(10),A.OrderCreateDt,23)>=CONVERT(VARCHAR(10),DATEADD(dd,-day(dateadd(month,-1,getdate()))+1,dateadd(month,-1,getdate())),23) 
						                AND CONVERT(VARCHAR(10),A.OrderCreateDt,23)<=CONVERT(VARCHAR(10),GETDATE(),23)
                                    )X ON A.salesorder_id=X.salesorder_id";
            return _result;
        }



    }
}
