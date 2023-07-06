using System;
using System.Data;

namespace ZohoApiTool.DB
{
    //临时表
    public class TempDtList
    {
        /// <summary>
        /// T_BOOKS_SAL表信息-插入时使用
        /// </summary>
        /// <returns></returns>
        public DataTable MakeSalHeadDtTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 34; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //(主键) PK
                    case 0:
                        dc.ColumnName = "salesorder_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //客户名称
                    case 1:
                        dc.ColumnName = "customer_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //邮箱
                    case 2:
                        dc.ColumnName = "email";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //交货日期
                    case 3:
                        dc.ColumnName = "delivery_date";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //公司名称
                    case 4:
                        dc.ColumnName = "company_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据编码
                    case 5:
                        dc.ColumnName = "salesorder_number";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //参考号码
                    case 6:
                        dc.ColumnName = "reference_number";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据日期
                    case 7:
                        dc.ColumnName = "Orderdate";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //船务日期
                    case 8:
                        dc.ColumnName = "shipment_date";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //船务天数
                    case 9:
                        dc.ColumnName = "shipment_days";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //按天计算
                    case 10:
                        dc.ColumnName = "due_by_days";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //截止日期:天
                    case 11:
                        dc.ColumnName = "due_in_days";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //货币
                    case 12:
                        dc.ColumnName = "currency_code";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Sub Total
                    case 13:
                        dc.ColumnName = "total";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    //Total
                    case 14:
                        dc.ColumnName = "bcy_total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //发票总金额
                    case 15:
                        dc.ColumnName = "total_invoiced_amount";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //创建日期
                    case 16:
                        dc.ColumnName = "created_time";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //最后一次修改日期
                    case 17:
                        dc.ColumnName = "last_modified_time";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //是否有EMAIL
                    case 18:
                        dc.ColumnName = "is_emailed";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //总数量
                    case 19:
                        dc.ColumnName = "quantity";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //发票总数量
                    case 20:
                        dc.ColumnName = "quantity_invoiced";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //仓库总数量
                    case 21:
                        dc.ColumnName = "quantity_packed";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //发送总数量
                    case 22:
                        dc.ColumnName = "quantity_shipped";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //单据状态
                    case 23:
                        dc.ColumnName = "order_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Invoice状态
                    case 24:
                        dc.ColumnName = "invoiced_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Payment状态
                    case 25:
                        dc.ColumnName = "paid_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Shipment状态
                    case 26:
                        dc.ColumnName = "shipped_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //销售员
                    case 27:
                        dc.ColumnName = "salesperson_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //余额
                    case 28:
                        dc.ColumnName = "balance";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    //交货方式
                    case 29:
                        dc.ColumnName = "delivery_method";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //是否删除(0:是 1:否)
                    case 30:
                        dc.ColumnName = "IsDel";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //单据插入日期(PS:一经插入,不能修改)
                    case 31:
                        dc.ColumnName = "OrderCreateDt";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //最后一次操作日期(PS:记录最后一次更新日期,更新时使用;每次Up可覆盖更新)
                    case 32:
                        dc.ColumnName = "LastChangeDt";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //国家类别(暂分为:US,MX)=>用于区分ZOHO不同国家类别
                    case 33:
                        dc.ColumnName = "CountryType";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// T_BOOKS_SALDTL表信息-插入时使用
        /// </summary>
        /// <returns></returns>
        public DataTable MakeSalDetailDtTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 28; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //fk(T_BOOKS_SAL外键)
                    case 0:
                        dc.ColumnName = "salesorder_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料行ID pk
                    case 1:
                        dc.ColumnName = "line_item_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据日期
                    case 2:
                        dc.ColumnName = "Orderdate";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //物料ID
                    case 3:
                        dc.ColumnName = "item_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //仓库名称
                    case 4:
                        dc.ColumnName = "warehouse_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //sku名称
                    case 5:
                        dc.ColumnName = "sku";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 6:
                        dc.ColumnName = "name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //组别名称
                    case 7:
                        dc.ColumnName = "group_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //描述
                    case 8:
                        dc.ColumnName = "description";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //汇率
                    case 9:
                        dc.ColumnName = "bcy_rate";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    //汇率(显示使用)
                    case 10:
                        dc.ColumnName = "rate";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //数量
                    case 11:
                        dc.ColumnName = "quantity";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //单位
                    case 12:
                        dc.ColumnName = "unit";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //折扣金额
                    case 13:
                        dc.ColumnName = "discount_amount";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //折扣
                    case 14:
                        dc.ColumnName = "discount";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //税类型
                    case 15:
                        dc.ColumnName = "tax_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //免税代码
                    case 16:
                        dc.ColumnName = "tax_exemption_code";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //总金额
                    case 17:
                        dc.ColumnName = "item_total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //项目合计
                    case 18:
                        dc.ColumnName = "item_sub_total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //生产类别
                    case 19:
                        dc.ColumnName = "product_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //行物料类别
                    case 20:
                        dc.ColumnName = "line_item_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料类别
                    case 21:
                        dc.ColumnName = "item_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Status-Invoiced
                    case 22:
                        dc.ColumnName = "quantity_invoiced";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Status-Packed
                    case 23:
                        dc.ColumnName = "quantity_packed";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Status-Shipped
                    case 24:
                        dc.ColumnName = "quantity_shipped";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //是否删除(0:是 1:否)
                    case 25:
                        dc.ColumnName = "IsDel";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //单据插入日期(PS:一经插入,不能修改)
                    case 26:
                        dc.ColumnName = "OrderCreateDt";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    // 最后一次操作日期(PS:记录最后一次更新日期,更新时使用;Up可覆盖更新)
                    case 27:
                        dc.ColumnName = "LastChangeDt";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }
        
        /// <summary>
        /// 收集表头API返回记录集
        /// </summary>
        /// <returns></returns>
        public DataTable MakeSalHeadApiDtTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 30; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //(主键) PK
                    case 0:
                        dc.ColumnName = "salesorder_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //客户名称
                    case 1:
                        dc.ColumnName = "customer_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //邮箱
                    case 2:
                        dc.ColumnName = "email";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //交货日期
                    case 3:
                        dc.ColumnName = "delivery_date";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //公司名称
                    case 4:
                        dc.ColumnName = "company_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据编码
                    case 5:
                        dc.ColumnName = "salesorder_number";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //参考号码
                    case 6:
                        dc.ColumnName = "reference_number";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据日期
                    case 7:
                        dc.ColumnName = "Orderdate";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //船务日期
                    case 8:
                        dc.ColumnName = "shipment_date";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //船务天数
                    case 9:
                        dc.ColumnName = "shipment_days";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //按天计算
                    case 10:
                        dc.ColumnName = "due_by_days";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //截止日期:天
                    case 11:
                        dc.ColumnName = "due_in_days";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //货币
                    case 12:
                        dc.ColumnName = "currency_code";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Sub Total
                    case 13:
                        dc.ColumnName = "total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //Total
                    case 14:
                        dc.ColumnName = "bcy_total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //发票总金额
                    case 15:
                        dc.ColumnName = "total_invoiced_amount";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //创建日期
                    case 16:
                        dc.ColumnName = "created_time";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //最后一次修改日期
                    case 17:
                        dc.ColumnName = "last_modified_time";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //是否有EMAIL
                    case 18:
                        dc.ColumnName = "is_emailed";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //总数量
                    case 19:
                        dc.ColumnName = "quantity";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //发票总数量
                    case 20:
                        dc.ColumnName = "quantity_invoiced";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //仓库总数量
                    case 21:
                        dc.ColumnName = "quantity_packed";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //发送总数量
                    case 22:
                        dc.ColumnName = "quantity_shipped";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //单据状态
                    case 23:
                        dc.ColumnName = "order_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Invoice状态
                    case 24:
                        dc.ColumnName = "invoiced_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Payment状态
                    case 25:
                        dc.ColumnName = "paid_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Shipment状态
                    case 26:
                        dc.ColumnName = "shipped_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //销售员
                    case 27:
                        dc.ColumnName = "salesperson_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //余额
                    case 28:
                        dc.ColumnName = "balance";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //交货方式
                    case 29:
                        dc.ColumnName = "delivery_method";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 收集表体API返回记录集
        /// </summary>
        /// <returns></returns>
        public DataTable MakeSalDetailApiDtTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 25; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //fk(T_BOOKS_SAL外键)
                    case 0:
                        dc.ColumnName = "salesorder_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料行ID pk
                    case 1:
                        dc.ColumnName = "line_item_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据日期
                    case 2:
                        dc.ColumnName = "Orderdate";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料ID
                    case 3:
                        dc.ColumnName = "item_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //仓库名称
                    case 4:
                        dc.ColumnName = "warehouse_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //sku名称
                    case 5:
                        dc.ColumnName = "sku";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 6:
                        dc.ColumnName = "name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //组别名称
                    case 7:
                        dc.ColumnName = "group_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //描述
                    case 8:
                        dc.ColumnName = "description";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //汇率
                    case 9:
                        dc.ColumnName = "bcy_rate";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //汇率(显示使用)
                    case 10:
                        dc.ColumnName = "rate";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //数量
                    case 11:
                        dc.ColumnName = "quantity";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //单位
                    case 12:
                        dc.ColumnName = "unit";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //折扣金额
                    case 13:
                        dc.ColumnName = "discount_amount";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //折扣
                    case 14:
                        dc.ColumnName = "discount";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //税类型
                    case 15:
                        dc.ColumnName = "tax_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //免税代码
                    case 16:
                        dc.ColumnName = "tax_exemption_code";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //总金额
                    case 17:
                        dc.ColumnName = "item_total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //项目合计
                    case 18:
                        dc.ColumnName = "item_sub_total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //生产类别
                    case 19:
                        dc.ColumnName = "product_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //行物料类别
                    case 20:
                        dc.ColumnName = "line_item_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料类别
                    case 21:
                        dc.ColumnName = "item_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Status-Invoiced
                    case 22:
                        dc.ColumnName = "quantity_invoiced";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Status-Packed
                    case 23:
                        dc.ColumnName = "quantity_packed";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Status-Shipped
                    case 24:
                        dc.ColumnName = "quantity_shipped";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// ‘盘点功能’使用-注:包含表头(一部份字段) 及 表体字段信息-作用:对表头 表体指定字段更新
        /// </summary>
        /// <returns></returns>
        public DataTable MakeSalCheckApiDtTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 43; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //(主键) PK
                    case 0:
                        dc.ColumnName = "salesorder_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //客户名称
                    case 1:
                        dc.ColumnName = "customer_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据编码
                    case 2:
                        dc.ColumnName = "salesorder_number";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //参考号码
                    case 3:
                        dc.ColumnName = "reference_number";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //单据日期
                    case 4:
                        dc.ColumnName = "Orderdate";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //船务日期
                    case 5:
                        dc.ColumnName = "shipment_date";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //货币
                    case 6:
                        dc.ColumnName = "currency_code";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Sub Total
                    case 7:
                        dc.ColumnName = "total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //Total
                    case 8:
                        dc.ColumnName = "bcy_total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //最后一次修改日期
                    case 9:
                        dc.ColumnName = "last_modified_time";
                        dc.DataType = Type.GetType("System.String");
                        break;

                    //总数量
                    case 10:
                        dc.ColumnName = "head_quantity";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //发票总数量
                    case 11:
                        dc.ColumnName = "head_quantity_invoiced";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //仓库总数量
                    case 12:
                        dc.ColumnName = "head_quantity_packed";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //发送总数量
                    case 13:
                        dc.ColumnName = "head_quantity_shipped";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;

                    //单据状态
                    case 14:
                        dc.ColumnName = "order_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Invoice状态
                    case 15:
                        dc.ColumnName = "invoiced_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Payment状态
                    case 16:
                        dc.ColumnName = "paid_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Shipment状态
                    case 17:
                        dc.ColumnName = "shipped_status";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //余额
                    case 18:
                        dc.ColumnName = "balance";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //交货方式
                    case 19:
                        dc.ColumnName = "delivery_method";
                        dc.DataType = Type.GetType("System.String");
                        break;


                    ///////////////////表体明细部份//////////////////////
                    //物料行ID pk
                    case 20:
                        dc.ColumnName = "line_item_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料ID
                    case 21:
                        dc.ColumnName = "item_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //仓库名称
                    case 22:
                        dc.ColumnName = "warehouse_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //sku名称
                    case 23:
                        dc.ColumnName = "sku";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 24:
                        dc.ColumnName = "name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //组别名称
                    case 25:
                        dc.ColumnName = "group_name";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //描述
                    case 26:
                        dc.ColumnName = "description";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //汇率
                    case 27:
                        dc.ColumnName = "bcy_rate";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //汇率(显示使用)
                    case 28:
                        dc.ColumnName = "rate";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //数量
                    case 29:
                        dc.ColumnName = "quantity";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //单位
                    case 30:
                        dc.ColumnName = "unit";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //折扣金额
                    case 31:
                        dc.ColumnName = "discount_amount";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //折扣
                    case 32:
                        dc.ColumnName = "discount";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //税类型
                    case 33:
                        dc.ColumnName = "tax_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //免税代码
                    case 34:
                        dc.ColumnName = "tax_exemption_code";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //总金额
                    case 35:
                        dc.ColumnName = "item_total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //项目合计
                    case 36:
                        dc.ColumnName = "item_sub_total";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    //生产类别
                    case 37:
                        dc.ColumnName = "product_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //行物料类别
                    case 38:
                        dc.ColumnName = "line_item_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料类别
                    case 39:
                        dc.ColumnName = "item_type";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //Status-Invoiced
                    case 40:
                        dc.ColumnName = "quantity_invoiced";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Status-Packed
                    case 41:
                        dc.ColumnName = "quantity_packed";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Status-Shipped
                    case 42:
                        dc.ColumnName = "quantity_shipped";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }
        /// <summary>
        /// 记录要进行更新删除的字段
        /// </summary>
        /// <returns></returns>
        public DataTable MakeDelDt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 3; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    case 0:
                        dc.ColumnName = "typeid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //T_BOOKS_SAL (主键) PK
                    case 1:
                        dc.ColumnName = "salesorder_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //T_BOOKS_SALDTL 主键
                    case 2:
                        dc.ColumnName = "line_item_id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

    }
}
