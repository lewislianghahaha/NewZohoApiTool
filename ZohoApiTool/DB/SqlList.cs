using System.Collections;

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
            _result = @"SELECT TOP 1000 A.salesorder_id
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
                                        SELECT TOP 1000 A.salesorder_id
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
                case "T_BOOKS_SAL":
                    _result = @"UPDATE dbo.T_BOOKS_SAL SET  customer_name=@customer_name,email=@email,delivery_date=@delivery_date,
												company_name=@company_name,salesorder_number=@salesorder_number,
												reference_number=@reference_number,Orderdate=@Orderdate ,
												shipment_date=@shipment_date,shipment_days=@shipment_days ,
											    due_by_days=@due_by_days,due_in_days=@due_in_days ,currency_code=@currency_code ,
											    total=@total,bcy_total=@bcy_total,total_invoiced_amount=@total_invoiced_amount,
												last_modified_time=@last_modified_time,
												is_emailed=@is_emailed,quantity=@quantity,quantity_invoiced=@quantity_invoiced,
												quantity_packed=@quantity_packed,quantity_shipped=@quantity_shipped,
												order_status=@order_status,invoiced_status=@invoiced_status,
												paid_status=@paid_status,shipped_status=@shipped_status,
												salesperson_name=@salesperson_name,balance=@balance,
												delivery_method=@delivery_method,IsDel=@IsDel,
												LastChangeDt=@LastChangeDt
                                WHERE salesorder_id=@salesorder_id";
                    break;
                case "T_BOOKS_SALDTL":
                    _result = @"UPDATE dbo.T_BOOKS_SALDTL SET Orderdate=@Orderdate,item_id=@item_id,warehouse_name=@warehouse_name,
													 sku=@sku,name=@name,group_name=@group_name,description=@description,
													 bcy_rate=@bcy_rate,rate=@rate,quantity=@quantity,unit=@unit,
													 discount_amount=@discount_amount,discount=@discount,
													 tax_type=@tax_type,tax_exemption_code=@tax_exemption_code,
													 item_total=@item_total,item_sub_total=@item_sub_total,product_type=@product_type,
												     line_item_type=@line_item_type,item_type=@item_type,quantity_invoiced=@quantity_invoiced,
													 quantity_packed=@quantity_packed,quantity_shipped=@quantity_shipped,
													 IsDel=@IsDel,LastChangeDt=@LastChangeDt
                                  WHERE salesorder_id=@salesorder_id AND line_item_id=@line_item_id";
                    break;
                case "T_BOOKS_SAL_Check":
                    _result = @"UPDATE dbo.T_BOOKS_SAL SET  customer_name=@customer_name,salesorder_number=@salesorder_number,
												reference_number=@reference_number,Orderdate=@Orderdate ,
												shipment_date=@shipment_date,currency_code=@currency_code ,
											    total=@total,bcy_total=@bcy_total,last_modified_time=@last_modified_time,
												quantity=@quantity,quantity_invoiced=@quantity_invoiced,
												quantity_packed=@quantity_packed,quantity_shipped=@quantity_shipped,
												order_status=@order_status,invoiced_status=@invoiced_status,
												paid_status=@paid_status,shipped_status=@shipped_status,
												balance=@balance,delivery_method=@delivery_method,IsDel=@IsDel,
												LastChangeDt=@LastChangeDt
                                WHERE salesorder_id=@salesorder_id";
                    break;
                case "T_BOOKS_SALDTL_Check":
                    _result = @"UPDATE dbo.T_BOOKS_SALDTL SET item_id=@item_id,warehouse_name=@warehouse_name,
													 sku=@sku,name=@name,group_name=@group_name,description=@description,
													 bcy_rate=@bcy_rate,rate=@rate,quantity=@quantity,unit=@unit,
													 discount_amount=@discount_amount,discount=@discount,
													 tax_type=@tax_type,tax_exemption_code=@tax_exemption_code,
													 item_total=@item_total,item_sub_total=@item_sub_total,product_type=@product_type,
												     line_item_type=@line_item_type,item_type=@item_type,quantity_invoiced=@quantity_invoiced,
													 quantity_packed=@quantity_packed,quantity_shipped=@quantity_shipped,
													 IsDel=@IsDel,LastChangeDt=@LastChangeDt
                                WHERE salesorder_id=@salesorder_id AND line_item_id=@line_item_id";
                    break;
            }
            return _result;
        }

        /// <summary>
        /// 对需要进行更新IsDel=0的记录进行操作
        /// </summary>
        /// <param name="typeid">0:对表头进行更新操作 1:对表体进行更新操作</param>
        /// <param name="uplist">更新主键值;表头=>salesorder_id 表体=>line_item_id</param>
        /// <returns></returns>
        public string UpIsDelRecord(int typeid,string uplist)
        {
            //对表头进行更新操作
            if (typeid == 0)
            {
                //TODO:1.对表头IsDel字段更新 2.根据salesorder_id对应的表体IsDel字段更新
                _result = $@"UPDATE dbo.T_BOOKS_SAL SET IsDel=0
                             WHERE salesorder_id IN ({uplist})
                            
                            UPDATE A SET ISDEL=0
                            FROM dbo.T_BOOKS_SALDTL A
                            WHERE A.salesorder_id IN  ({uplist})                           
                            ";
            }
            //对表体进行更新操作
            else
            {
                _result = $@"UPDATE dbo.T_BOOKS_SALDTL SET IsDel=0
                             WHERE line_item_id IN ({uplist})";
            }

            return _result;
        }

    }
}
