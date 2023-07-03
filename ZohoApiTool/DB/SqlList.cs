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
        /// 作用:1.与API返回结果进行比较,判断API返回数据是否新记录
        ///      2.放到表体API进行查找,用于判断此单据是否已删除
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
        /// 作用:将表体API放到此查询语句返回数据集内，判断是否存在对应记录
        /// </summary>
        /// <returns></returns>
        public string GetSearchBooksSalDtl()
        {
            _result = @"SELECT X.salesorder_id,A.line_item_id
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

        /// <summary>
        /// 根据表名获取查询表体语句(更新时使用) 只显示TOP 1记录
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string SearchUpdateTable(string tableName)
        {
            _result = $@"
                          SELECT Top 1 a.*
                          FROM {tableName} a
                        ";
            return _result;
        }

        /// <summary>
        /// 更新语句
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public string UpdateEntry(string tablename)
        {
            switch (tablename)
            {
                case "":
                    _result = @"UPDATE dbo.T_OfferOrder SET OAorderno=@OAorderno,Fstatus=@Fstatus,ConfirmDt=@ConfirmDt,CreateDt=@CreateDt,
                                                            CreateName=@CreateName,Useid=@Useid,UserName=@UserName,Typeid=@Typeid,DevGroupid=@DevGroupid
                                WHERE FId=@FId";
                    break;
                
            }
            return _result;
        }

    }
}
