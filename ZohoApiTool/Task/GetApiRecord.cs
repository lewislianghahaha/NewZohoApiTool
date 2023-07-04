using System;
using System.Data;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ZohoApiTool.Task
{
    //与ZOHO API交互操作
    public class GetApiRecord
    {
        //api所需organizationid
        private string _organizationid = "678315528";

        //定义‘刷新令牌’变量
        private string _accesstoken = "";
        //定义‘销售订单’表头DT
        private DataTable _headDt = new DataTable();
        //定义‘销售订单’明细DT
        private DataTable _dtldt = new DataTable();

        /// <summary>
        /// 获取access_Token（刷新令牌）;在每次执行API获取数据时使用
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken()
        {
            try
            {
                LogHelper.WriteLog("获取刷新令牌开始");

                //todo:设置API 刷新令牌URL 地址
                var serviceUrl = $@"https://accounts.zoho.com/oauth/v2/token?refresh_token=1000.de999e5653faa9642fbce6501a337516.bb785e602be2b63616c88eb0d3ce2d19
                                &client_id=1000.VCUSUC900CXB0UEOT18NV3XS1L3EQT
                                &client_secret=fa638b638a7ff82e42bdfb4a9b5271a4a816b8f027
                                &redirect_uri=http://www.zoho.com/Books&grant_type=refresh_token";

                //todo:一定要这一句,不然会出现:"未能创建SSL/TLS安全通道"异常
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                //todo:执行调用API获取Access_Token(刷新令牌)
                var client = new RestClient(serviceUrl);
                //定义request对象
                var request = new RestRequest();
                //设置相关请求方式
                request.Method = Method.Post;

                //执行调用API
                var response = client.Execute(request);

                if (Convert.ToString(response.StatusCode) != "OK") throw new Exception($"{response.ErrorMessage}");
                else
                {
                    //获取API返回JSON结果
                    GetApiJsonRecord(0,response.Content);
                    LogHelper.WriteLog($"已获取'{_accesstoken}'新令牌");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("获取Access_Token令牌异常,原因:",ex);
            }

            return _accesstoken;
        }

        /// <summary>
        /// 获取销售订单表头API返回记录集-在每次执行API获取数据时使用
        /// </summary>
        /// <param name="tempdt">表头临时表</param>
        /// <param name="accesstoken">刷新令牌</param>
        /// <returns></returns>
        public DataTable GetSalHeadRecord(DataTable tempdt,string accesstoken)
        {
            try
            {
                LogHelper.WriteLog("获取刷新令牌开始");

                _headDt = tempdt.Clone();

                var serviceUrl = $"https://www.zohoapis.com/books/v3/salesorders?organization_id={_organizationid}";

                //todo:一定要这一句,不然会出现:"未能创建SSL/TLS安全通道"异常
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                var client = new RestClient(serviceUrl);
                //定义request对像
                var request = new RestRequest();
                //设置相关请求方式
                request.Method = Method.Get;
                //添加标头-访问令牌在此添加(ZOHO API需要) todo:注:Zoho-oauthtoken与'刷新令牌'中间一定要有空格
                request.AddHeader("Authorization", $"Zoho-oauthtoken {accesstoken}");

                //执行调用API
                var response = client.Execute(request);
                if (Convert.ToString(response.StatusCode) == "OK") //throw new Exception($"调用Api出现异常,原因:{response.ErrorMessage}");
                {
                    GetApiJsonRecord(1,response.Content);
                    LogHelper.WriteLog($"已获取API返回记录,行数'{_headDt.Rows.Count}");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("获取表头API出现异常,原因:", ex);
            }

            return _headDt;
        }

        /// <summary>
        /// 根据销售订单ID-获取销售订单表头API返回记录集-在每次执行API获取数据时使用
        /// </summary>
        /// <param name="tempdt">表体临时表</param>
        /// <param name="salesorderId">销售订单ID</param>
        /// <param name="accesstoken">刷新令牌</param>
        /// <returns></returns>
        public DataTable GetSalDetailRecord(DataTable tempdt,string salesorderId, string accesstoken)
        {
            try
            {
                LogHelper.WriteLog("获取刷新令牌开始");

                _dtldt = tempdt.Clone();

                var serviceUrl = $"https://www.zohoapis.com/books/v3/salesorders/{salesorderId}?organization_id={_organizationid}";

                //todo:一定要这一句,不然会出现:"未能创建SSL/TLS安全通道"异常
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                var client = new RestClient(serviceUrl);
                //定义request对象
                var request = new RestRequest();
                //设置相关请求方式
                request.Method = Method.Get;
                //添加标头-访问令牌在此添加(ZOHO API需要) todo:注:Zoho-oauthtoken与'刷新令牌'中间一定要有空格
                request.AddHeader("Authorization", $"Zoho-oauthtoken {accesstoken}");

                //执行调用API
                var response = client.Execute(request);

                if (Convert.ToString(response.StatusCode) == "OK") //throw new Exception($"调用Api出现异常,原因:{response.ErrorMessage}");
                {
                    GetApiJsonRecord(GlobalClasscs.RmMessage.Ischeck == 0 ? 3 : 2, response.Content);
                    LogHelper.WriteLog($"已获取API返回记录,行数'{_dtldt.Rows.Count}");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("获取表体API出现异常,原因:", ex);
            }

            return _dtldt;
        }

        /// <summary>
        /// 根据API返回的结果集,整理有需要的信息
        /// </summary>
        /// <param name="typeid">0:获取刷新令牌  1:整理销售订单表头API信息(返回DT)  2:整理销售订单表体API信息(返回DT)
        /// 3.整理表头表体信息(盘点功能使用) </param>
        /// <param name="jrecord"></param>
        private void GetApiJsonRecord(int typeid,string jrecord)
        {
            var obj = JObject.Parse(jrecord);

            if (typeid == 0)
            {
                _accesstoken = obj["access_token"].ToString();
            }
            else if (typeid == 1)
            {
                //循环获取表头API返回JSON数组 (200个)
                var ords = (JArray) obj["salesorders"];

                foreach (var ordJToken in ords)
                {
                    var newrow = _headDt.NewRow();
                    newrow[0] = Convert.ToString(ordJToken["salesorder_id"]);           //获取单据主键ID
                    newrow[1] = Convert.ToString(ordJToken["customer_name"]);           //客户名称
                    newrow[2] = Convert.ToString(ordJToken["email"]);                   //邮箱
                    newrow[3] = Convert.ToDateTime(ordJToken["delivery_date"]);         //交货日期
                    newrow[4] = Convert.ToString(ordJToken["company_name"]);            //公司名称
                    newrow[5] = Convert.ToString(ordJToken["salesorder_number"]);       //单据编码
                    newrow[6] = Convert.ToString(ordJToken["reference_number"]);        //参考号码
                    newrow[7] = Convert.ToDateTime(ordJToken["date"]);                  //单据日期
                    newrow[8] = Convert.ToDateTime(ordJToken["shipment_date"]);         //船务日期
                    newrow[9] = Convert.ToInt32(ordJToken["shipment_days"]);            //船务天数
                    newrow[10] = Convert.ToInt32(ordJToken["due_by_days"]);             //按天计算
                    newrow[11] = Convert.ToInt32(ordJToken["due_in_days"]);             //截止日期:天
                    newrow[12] = Convert.ToString(ordJToken["currency_code"]);          //货币
                    newrow[13] = Convert.ToDecimal(ordJToken["total"]);                 //Sub Total
                    newrow[14] = Convert.ToDecimal(ordJToken["bcy_total"]);             //Total
                    newrow[15] = Convert.ToDecimal(ordJToken["total_invoiced_amount"]); //发票总金额
                    newrow[16] = Convert.ToDateTime(ordJToken["created_time"]);         //创建日期
                    newrow[17] = Convert.ToDateTime(ordJToken["last_modified_time"]);   //最后一次修改日期
                    newrow[18] = Convert.ToString(ordJToken["is_emailed"]);             //是否有EMAIL
                    newrow[19] = Convert.ToDecimal(ordJToken["quantity"]);              //总数量
                    newrow[20] = Convert.ToDecimal(ordJToken["quantity_invoiced"]);     //发票总数量
                    newrow[21] = Convert.ToDecimal(ordJToken["quantity_packed"]);       //仓库总数量
                    newrow[22] = Convert.ToDecimal(ordJToken["quantity_shipped"]);      //发送总数量
                    newrow[23] = Convert.ToString(ordJToken["order_status"]);           //单据状态
                    newrow[24] = Convert.ToString(ordJToken["invoiced_status"]);        //Invoice状态
                    newrow[25] = Convert.ToString(ordJToken["paid_status"]);            //Payment状态
                    newrow[26] = Convert.ToString(ordJToken["shipped_status"]);         //Shipment状态
                    newrow[27] = Convert.ToString(ordJToken["salesperson_name"]);       //销售员
                    newrow[28] = Convert.ToDecimal(ordJToken["balance"]);               //余额
                    newrow[29] = Convert.ToString(ordJToken["delivery_method"]);        //交货方式
                    _headDt.Rows.Add(newrow);
                }
                var a = _headDt.Copy();
            }
            else if (typeid == 2)
            {
                //todo:获取JSON中‘salesorder_id’ 及 ‘date’ 对应记录
                var salesorderid = Convert.ToString(obj["salesorder"]["salesorder_id"]);
                var salesdt = Convert.ToDateTime(obj["salesorder"]["date"]);

                //todo:循环获取表体API返回JSON
                var ords = (JArray) obj["salesorder"]["line_items"];

                foreach (var ordJToken in ords)
                {
                    var newrow = _dtldt.NewRow();
                    newrow[0] = Convert.ToString(salesorderid);                     //salesorder_id
                    newrow[1] = Convert.ToString(ordJToken["line_item_id"]);        //获取明细行主键ID
                    newrow[2] = Convert.ToDateTime(salesdt);                        //单据日期
                    newrow[3] = Convert.ToString(ordJToken["item_id"]);             //物料ID
                    newrow[4] = Convert.ToString(ordJToken["warehouse_name"]);      //仓库名称
                    newrow[5] = Convert.ToString(ordJToken["sku"]);                 //sku名称
                    newrow[6] = Convert.ToString(ordJToken["name"]);                //物料名称
                    newrow[7] = Convert.ToString(ordJToken["group_name"]);          //组别名称
                    newrow[8] = Convert.ToString(ordJToken["description"]);         //描述
                    newrow[9] = Convert.ToDecimal(ordJToken["bcy_rate"]);           //汇率
                    newrow[10] = Convert.ToDecimal(ordJToken["rate"]);              //汇率(显示使用)
                    newrow[11] = Convert.ToInt32(ordJToken["quantity"]);            //数量
                    newrow[12] = Convert.ToString(ordJToken["unit"]);               //单位
                    newrow[13] = Convert.ToDecimal(ordJToken["discount_amount"]);   //折扣金额
                    newrow[14] = Convert.ToDecimal(ordJToken["discount"]);          //折扣
                    newrow[15] = Convert.ToString(ordJToken["tax_type"]);           //税类型
                    newrow[16] = Convert.ToString(ordJToken["tax_exemption_code"]); //免税代码
                    newrow[17] = Convert.ToDecimal(ordJToken["item_total"]);        //总金额
                    newrow[18] = Convert.ToDecimal(ordJToken["item_sub_total"]);    //项目合计
                    newrow[19] = Convert.ToString(ordJToken["product_type"]);       //生产类别
                    newrow[20] = Convert.ToString(ordJToken["line_item_type"]);     //行物料类别
                    newrow[21] = Convert.ToString(ordJToken["item_type"]);          //物料类别
                    newrow[22] = Convert.ToInt32(ordJToken["quantity_invoiced"]);   //Status-Invoiced
                    newrow[23] = Convert.ToInt32(ordJToken["quantity_packed"]);     //Status-Packed
                    newrow[24] = Convert.ToInt32(ordJToken["quantity_shipped"]);    //Status-Shipped
                    _dtldt.Rows.Add(newrow);
                }
                var b = _dtldt.Copy();
            }
            //‘盘点功能’使用-包含表头 表体信息 
            //todo:(重) 分别将明细行的 quantity quantity_invoiced quantity_packed quantity_shipped 累加至表头对应的项内
            else
            {
                //分别定义 quantity quantity_invoiced quantity_packed quantity_shipped 相关变量,用于统计值
                decimal headQuantity = 0;
                decimal headQuantityInvoiced = 0;
                decimal headQuantityPacked = 0;
                decimal headQuantityShipped = 0;

                //todo:获取JSON中‘salesorder_id’ 及 ‘date’ 对应记录
                var salesorderid = Convert.ToString(obj["salesorder"]["salesorder_id"]);            //主表主键
                var customer = Convert.ToString(obj["salesorder"]["customer_name"]);                //客户名称
                var salesorder = Convert.ToString(obj["salesorder"]["salesorder_number"]);          //单据编码
                var referenceNumber = Convert.ToString(obj["salesorder"]["reference_number"]);      //参考号码
                var salesdt = Convert.ToDateTime(obj["salesorder"]["date"]);                        //单据日期
                var shipmentDate = Convert.ToDateTime(obj["salesorder"]["shipment_date"]);          //船务日期
                var currencyCode = Convert.ToString(obj["salesorder"]["currency_code"]);            //货币
                var total = Convert.ToDecimal(obj["salesorder"]["total"]);                          //Sub Total
                var bcyTotal = Convert.ToDecimal(obj["salesorder"]["bcy_total"]);                   //Total
                var lastModifiedTime = Convert.ToDecimal(obj["salesorder"]["last_modified_time"]);  //最后一次修改日期
                var orderStatus = Convert.ToString(obj["salesorder"]["order_status"]);              //单据状态
                var invoicedStatus = Convert.ToString(obj["salesorder"]["invoiced_status"]);        //Invoice状态
                var paidStatus = Convert.ToString(obj["salesorder"]["paid_status"]);                //Payment状态
                var shippedStatus = Convert.ToString(obj["salesorder"]["shipped_status"]);          //Shipment状态
                var balance = Convert.ToDecimal(obj["salesorder"]["balance"]);                      //余额
                var deliveryMethod = Convert.ToString(obj["salesorder"]["delivery_method"]);        //交货方式

                //todo:循环获取表体API返回JSON
                var ords = (JArray)obj["salesorder"]["line_items"];

                //todo:分别循环ords 并统计出quantity quantity_invoiced quantity_packed quantity_shipped 
                foreach (var ordJToken in ords)
                {
                    headQuantity += Convert.ToDecimal(ordJToken["quantity"]);                   //数量
                    headQuantityInvoiced += Convert.ToDecimal(ordJToken["quantity_invoiced"]);  //Status-Invoiced
                    headQuantityPacked += Convert.ToDecimal(ordJToken["quantity_packed"]);      //Status-Packed
                    headQuantityShipped += Convert.ToDecimal(ordJToken["quantity_shipped"]);    //Status-Shipped
                }

                //todo:插入相关值操作:
                foreach (var ordJToken in ords)
                {
                    var newrow = _dtldt.NewRow();
                    newrow[0] = salesorderid;                     //salesorder_id
                    newrow[1] = customer;                         //客户名称
                    newrow[2] = salesorder;                       //单据编码
                    newrow[3] = referenceNumber;                  //参考号码
                    newrow[4] = salesdt;                          //单据日期
                    newrow[5] = shipmentDate;                     //船务日期
                    newrow[6] = currencyCode;                     //货币
                    newrow[7] = total;                            //Sub Total
                    newrow[8] = bcyTotal;                         //Total
                    newrow[9] = lastModifiedTime;                 //最后一次修改日期
                    newrow[10] = headQuantity;                    //总数量
                    newrow[11] = headQuantityInvoiced;            //发票总数量
                    newrow[12] = headQuantityPacked;              //仓库总数量
                    newrow[13] = headQuantityShipped;             //发送总数量
                    newrow[14] = orderStatus;                     //单据状态
                    newrow[15] = invoicedStatus;                  //Invoice状态
                    newrow[16] = paidStatus;                      //Payment状态
                    newrow[17] = shippedStatus;                   //Shipment状态
                    newrow[18] = balance;                         //余额
                    newrow[19] = deliveryMethod;                  //交货方式

                    newrow[20] = Convert.ToString(ordJToken["line_item_id"]);         //物料行ID pk
                    newrow[21] = Convert.ToString(ordJToken["item_id"]);              //物料ID
                    newrow[22] = Convert.ToString(ordJToken["warehouse_name"]);       //仓库名称
                    newrow[23] = Convert.ToString(ordJToken["sku"]);                  //sku名称
                    newrow[24] = Convert.ToString(ordJToken["name"]);                 //物料名称
                    newrow[25] = Convert.ToString(ordJToken["group_name"]);           //组别名称
                    newrow[26] = Convert.ToString(ordJToken["description"]);          //描述
                    newrow[27] = Convert.ToDecimal(ordJToken["bcy_rate"]);            //汇率
                    newrow[28] = Convert.ToDecimal(ordJToken["rate"]);                //汇率(显示使用)
                    newrow[29] = Convert.ToInt32(ordJToken["quantity"]);              //数量
                    newrow[30] = Convert.ToString(ordJToken["unit"]);                 //单位
                    newrow[31] = Convert.ToDecimal(ordJToken["discount_amount"]);     //折扣金额
                    newrow[32] = Convert.ToDecimal(ordJToken["discount"]);            //折扣
                    newrow[33] = Convert.ToString(ordJToken["tax_type"]);             //税类型
                    newrow[34] = Convert.ToString(ordJToken["tax_exemption_code"]);   //免税代码
                    newrow[35] = Convert.ToDecimal(ordJToken["item_total"]);          //总金额
                    newrow[36] = Convert.ToDecimal(ordJToken["item_sub_total"]);      //项目合计
                    newrow[37] = Convert.ToString(ordJToken["product_type"]);         //生产类别
                    newrow[38] = Convert.ToString(ordJToken["line_item_type"]);       //行物料类别
                    newrow[39] = Convert.ToString(ordJToken["item_type"]);            //物料类别
                    newrow[40] = Convert.ToInt32(ordJToken["quantity_invoiced"]);     //Status-Invoiced
                    newrow[41] = Convert.ToInt32(ordJToken["quantity_packed"]);       //Status-Packed
                    newrow[42] = Convert.ToInt32(ordJToken["quantity_shipped"]);      //Status-Shipped
                    _dtldt.Rows.Add(newrow);
                }
                var c = _dtldt.Copy();
            }
        }

    }
}
