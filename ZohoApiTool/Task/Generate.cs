using System;
using System.Data;
using System.Data.SqlClient;
using ZohoApiTool.DB;

namespace ZohoApiTool.Task
{
    //运算
    public class Generate
    {
        GetApiRecord getApi = new GetApiRecord();
        SearchDt searchDt = new SearchDt();
        ConDb conDb = new ConDb();
        SqlList sqlList = new SqlList();
        TempDtList tempDtList = new TempDtList();

        /// <summary>
        /// 核心运算（重）
        /// </summary>
        /// <returns></returns>
        public bool GenerateRecord()
        {
            var result = true;

            try
            {
                LogHelper.WriteLog("开始执行运算....");

                //获取各功能所需临时表
                //获取Api表头临时表
                var apiheaddt = tempDtList.MakeSalHeadApiDtTemp();
                //获取Api表体临时表
                var apidtldt = tempDtList.MakeSalDetailApiDtTemp();
                //获取‘监盘操作’所需临时表
                var checkdtldt = tempDtList.MakeSalCheckApiDtTemp();

                //获取‘已删除’的记录集-‘监盘操作’使用
                var deldt = tempDtList.MakeDelDt();

                //获取T_BOOKS_SAL表信息-插入使用
                var insertDt = tempDtList.MakeSalHeadDtTemp();
                //获取T_BOOK_SALDTL表信息-插入使用
                var insertDtlDt = tempDtList.MakeSalDetailDtTemp();

                //获取T_BOOKS_SAL表信息-更新使用
                var upDt = insertDt.Clone();
                //获取T_BOOK_SALDTL表信息-更新使用
                var updtlDt = insertDtlDt.Clone();


                //获取表头数据行数-当数据表没有数据时使用(ps:限一次使用)
                var initialDt = searchDt.GetIninitalRecord();
                //获取T_BOOKS_SAL 表头信息-用作比较
                var headDt = searchDt.GetBooksSalHeadRecord();
                //获取T_BOOKS_SALDTL 表体信息-用作比较
                var dtldt = searchDt.GetBooksSalDetailRecord();

                //通过zoho Api获取相关信息
                //获取‘刷新令牌’
                var accesstoken = getApi.GetAccessToken();

                //////////////////////////////数据整合部份/////////////////////////////////////
                //当判断数据表没有任何记录时,直接将API返回的表头 表体记录集,整理后分别插入至对应数据表
                if (Convert.ToInt32(initialDt.Rows[0][0]) == 0)
                {
                    GlobalClasscs.RmMessage.Ischeck = 1;

                    /////////////////////////////API数据收集部份/////////////////////////////////
                    //获取zoho api表头返回记录
                    apiheaddt.Merge(getApi.GetSalHeadRecord(apiheaddt, accesstoken));
                    //var b = apiheaddt.Copy();

                    if(apiheaddt.Rows.Count == 0) throw new Exception("因没有API表头返回数据,不能进行插入操作-(初始化操作)");

                    //循环通过zoho api表体返回DT记录集
                    foreach (DataRow rows in apiheaddt.Rows)
                    {
                        apidtldt.Merge(getApi.GetSalDetailRecord(apidtldt, Convert.ToString(rows[0]), accesstoken));
                    }

                    if(apidtldt.Rows.Count == 0) throw new Exception("因没有API表体返回数据,不能进行插入操作-(初始化操作)");
                    //var a = apidtldt.Copy();
                    /////////////////////////////////////数据处理////////////////////////////////////////////

                    //分别将表头 表体数据插入至insertDt 及 insertDtlDt内
                    insertDt.Merge(MakeRecordDtToDb(0, 1,insertDt, apiheaddt));
                    insertDtlDt.Merge(MakeRecordDtToDb(1, 1,insertDtlDt, apidtldt));
                }
                //执行‘监盘机器人操作’ - todo:注:每周日执行
                else if (Convert.ToInt32(initialDt.Rows[0][0])>0 && DateTime.Today.DayOfWeek.ToString() == "Sunday")
                {
                    GlobalClasscs.RmMessage.Ischeck = 0;

                    //将headDt循环放到表体API进行查找,并整理成相关DT返回给checkdtldt
                    foreach (DataRow rows in headDt.Rows)
                    {
                        checkdtldt.Merge(getApi.GetSalDetailRecord(checkdtldt,Convert.ToString(rows[0]),accesstoken));
                    }
                    //var c = checkdtldt.Copy();

                    if(checkdtldt.Rows.Count == 0) throw new Exception("因没有API返回数据,监盘机器人停止工作");

                    //使用dtldt进行放到checkdtldt进行查找，并执行以下操作:(重)
                    // 1.若整单不存在,标记IsDel=0 2.若明细行不存在,标记IsDel=0 3.对存在的记录-表头 表体对应字段进行更新
                    foreach (DataRow rows in dtldt.Rows)
                    {
                        var salesorderid = Convert.ToString(rows[0]);
                        var lineitemid = Convert.ToString(rows[1]);

                        //判断若salesorderid 或 lineitemid 在deldt内存在,即continue
                        if (deldt.Select("salesorder_id='" + salesorderid + "' and typeid='0'").Length>0) continue;
                        if (deldt.Select("salesorder_id='" + salesorderid + "' and line_item_id='" + lineitemid + "' and typeid='1'").Length>0) continue;

                        //todo:1.将salesorderid 放到 checkdtldt内查找,若发现不存在,即直接将salesorderid插入至deldt内,并continue
                        if (checkdtldt.Select("salesorder_id='" + salesorderid + "'").Length == 0)
                        {
                            //插入至deldt内
                            deldt.Merge(InsertDelDt(deldt, 0, salesorderid, ""));
                            continue;
                        }
                        //todo:2.将salesorderid && lineitemid 放到 checkdtldt内查找,若发现不存在,即直接将salesorderid lineitemid插入至deldt内,并continue
                        else if (checkdtldt.Select("salesorder_id='" + salesorderid + "' and line_item_id='" + lineitemid + "'").Length == 0)
                        {
                            //插入至deldt内
                            deldt.Merge(InsertDelDt(deldt, 1, salesorderid, lineitemid));
                            continue;
                        }
                        //todo:3.若循环的salesorderid 及 lineitemid都在checkdtldt有记录,即整理后数量,最后更新使用
                        else if(checkdtldt.Select("salesorder_id='" + salesorderid + "' and line_item_id='" + lineitemid + "'").Length >0)
                        {
                            var dtlrows = checkdtldt.Select("salesorder_id='" + salesorderid + "' and line_item_id='" + lineitemid + "'");

                            //将checkdtldt中记录表头信息整理,并最后插入至updt内
                            upDt.Merge(MakeCheckDt(0,upDt, dtlrows));
                            //将checkdtldt中记录表体信息整理,并最后插入至updtldt内
                            updtlDt.Merge(MakeCheckDt(1, updtlDt, dtlrows));
                        }
                    }
                    //var c1 = deldt.Copy();
                    //var c2 = upDt.Copy();
                    //var c3 = updtlDt.Copy();
                }
                //todo:日常操作(除周日外执行) -->循环将ApiHeadDt,ApiDtldt放到HeadDt,DtlDt内查找,无->插入 有->更新
                else if (Convert.ToInt32(initialDt.Rows[0][0]) > 0 && DateTime.Today.DayOfWeek.ToString() != "Sunday")
                {
                    GlobalClasscs.RmMessage.Ischeck = 1;

                    /////////////////////////////API数据收集部份/////////////////////////////////
                    
                    //获取zoho api表头返回记录
                    apiheaddt.Merge(getApi.GetSalHeadRecord(apiheaddt, accesstoken));

                    if(apiheaddt.Rows.Count == 0) throw new Exception("因没有API表头返回数据,不能进行插入操作-(日常操作)");

                    //var b = apiheaddt.Copy();

                    //循环通过zoho api表体返回DT记录集
                    foreach (DataRow rows in apiheaddt.Rows)
                    {
                        apidtldt.Merge(getApi.GetSalDetailRecord(apidtldt, Convert.ToString(rows[0]), accesstoken));
                    }

                    if(apidtldt.Rows.Count == 0) throw new Exception("因没有API表体返回数据,不能进行插入操作-(日常操作)");
                    //var a = apidtldt.Copy();

                    /////////////////////////////////////数据处理////////////////////////////////////////////

                    //表头操作
                    foreach (DataRow rows in apiheaddt.Rows)
                    {
                        //将dtlrows进行数据整理-(注:以apiheaddt表结构进行插入数据)
                        var dtlrows = headDt.Select("salesorder_id ='" + Convert.ToString(rows[0]) + "'");

                        //新记录-插入操作
                        if (dtlrows.Length == 0)
                        {
                            insertDt.Merge(MakeRecordDtToDb(0, 1, insertDt, ExchangeRecordToDb(rows, apiheaddt.Clone())));
                        }
                        //旧记录-更新操作
                        else
                        {
                            upDt.Merge(MakeRecordDtToDb(2,1,upDt, ExchangeRecordToDb(rows, apiheaddt.Clone())));
                        }
                    }

                    //表体操作
                    foreach (DataRow row in apidtldt.Rows)
                    {
                        var dtlrows = dtldt.Select("salesorder_id ='" + Convert.ToString(row[0]) + "' and line_item_id='" + Convert.ToString(row[1]) + "'");

                        //新记录-插入操作
                        if (dtlrows.Length == 0)
                        {
                            insertDtlDt.Merge(MakeRecordDtToDb(1, 1, insertDtlDt, ExchangeRecordToDb(row, apidtldt.Clone())));
                        }
                        //旧记录-更新操作
                        else
                        {
                            updtlDt.Merge(MakeRecordDtToDb(3, 1, updtlDt, ExchangeRecordToDb(row, apidtldt.Clone())));
                        }
                    }
                    //var a1 = insertDt.Copy();
                    //var a2 = insertDtlDt.Copy();
                    //var a3 = upDt.Copy();
                    //var a4 = updtlDt.Copy();
                }

                LogHelper.WriteLog("执行数据结束,执行插入 更新操作");

                //执行‘插入’ 及 ‘更新’操作
                if (insertDt.Rows.Count > 0)
                    if(!ImportDtToDb("T_BOOKS_SAL", insertDt)) throw new Exception("T_BOOKS_SAL表头插入出现异常");
                if (insertDtlDt.Rows.Count > 0)
                    if(!ImportDtToDb("T_BOOKS_SALDTL", insertDtlDt)) throw new Exception("T_BOOKS_SALDTL表体插入出现异常");

                //'日常操作'更新使用
                if (upDt.Rows.Count > 0 && GlobalClasscs.RmMessage.Ischeck == 1)
                    if(!UpdateDbFromDt("T_BOOKS_SAL", upDt)) throw new Exception("'日常操作'-表头更新出现异常");
                if (updtlDt.Rows.Count > 0 && GlobalClasscs.RmMessage.Ischeck == 1)
                    if(!UpdateDbFromDt("T_BOOKS_SALDTL", updtlDt)) throw new Exception("'日常操作'-表体更新出现异常");

                //‘监盘操作’更新使用
                if (upDt.Rows.Count > 0 && GlobalClasscs.RmMessage.Ischeck == 0)
                    if(!UpdateDbFromDt("T_BOOKS_SAL_Check", upDt)) throw new Exception("'监盘操作'-表头更新出现异常");
                if (updtlDt.Rows.Count > 0 && GlobalClasscs.RmMessage.Ischeck == 0)
                    if(!UpdateDbFromDt("T_BOOKS_SALDTL_Check", updtlDt)) throw new Exception("'监盘操作'-表体更新出现异常");

                //执行‘删除’操作-‘监盘操作’使用
                if (deldt.Rows.Count > 0)
                    if(!UseUpdateDelRecord(deldt)) throw new Exception("'监盘操作'-执行‘删除’标记更新出现异常");

                LogHelper.WriteLog("执行结束...");
            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.WriteErrorLog("执行过程中出现异常,原因:",ex);
            }

            return result;
        }

        /// <summary>
        /// 执行对不存在的记录进行-更新IsDel=0操作
        /// </summary>
        /// <param name="sourcedt"></param>
        private bool UseUpdateDelRecord(DataTable sourcedt)
        {
            var result = true;

            try
            {
                for (var i = 0; i < 2; i++)
                {
                    var now = DateTime.Now;
                    var dtlrows = sourcedt.Select("typeid='" + i + "'");
                    if (dtlrows.Length <= 0) continue;
                    var uplist = GetUpDelList(i, dtlrows);
                    searchDt.UpIsDelRecord(i, uplist, now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.WriteErrorLog("更新IsDel出现异常,原因:",ex);
            }

            return result;
        }

        /// <summary>
        /// 整合进行更新的主键值
        /// </summary>
        /// <param name="typeid">0:获取salesorder_id值  1:获取line_item_id值</param>
        /// <param name="row"></param>
        /// <returns></returns>
        private string GetUpDelList(int typeid,DataRow[] row)
        {
            var result = string.Empty;
            //中转比较变量
            var temp = string.Empty;
            //定义要获取的列ID，当typeid=0 获取salesorder_id值  1:获取line_item_id值
            var colid = typeid == 0 ? 1 : 2;

            for (var i = 0; i < row.Length; i++)
            {
                //第一行(初始化)时,将第一行的相关值赋给对应的变量内
                if (temp == "")
                {
                    temp = Convert.ToString(row[i][colid]);
                    result = "'" + Convert.ToString(row[i][colid]) + "'";
                }
                //从第二行开始判断是否一致
                else if (temp != Convert.ToString(row[i][colid]))
                {
                    temp = Convert.ToString(row[i][colid]);
                    result += ',' + "'" + Convert.ToString(row[i][colid]) + "'";
                }
            }

            return result;
        }

        /// <summary>
        /// 整理checkdtldt内的数据,拆分为表头 表体记录，最后用于更新-'监盘功能使用' *
        /// </summary>
        /// <param name="typeid">0:对表头进行操作  1:对表体进行操作</param>
        /// <param name="tempdt">分别对应updt updtldt两个临时表</param>
        /// <param name="row">循环checkdtldt内的每一行记录</param>
        /// <returns></returns>
        private DataTable MakeCheckDt(int typeid,DataTable tempdt,DataRow[] row)
        {
            try
            {
                if (typeid == 0)
                {
                    var now1 = DateTime.Now;
                    var newrow = tempdt.NewRow();
                    newrow[0] = Convert.ToString(row[0][0]);                   //salesorder_id
                    newrow[1] = Convert.ToString(row[0][1]);                   //客户名称
                    newrow[5] = Convert.ToString(row[0][2]);                   //单据编码
                    newrow[6] = Convert.ToString(row[0][3]);                   //参考号码
                    newrow[7] = Convert.ToString(row[0][4]);                   //单据日期
                    newrow[8] = Convert.ToString(row[0][5]);                   //船务日期
                    newrow[12] = Convert.ToString(row[0][6]);                  //货币
                    newrow[13] = Convert.ToDecimal(row[0][7]);                 //Sub Total
                    newrow[14] = Convert.ToDecimal(row[0][8]);                 //Total
                    newrow[17] = Convert.ToString(row[0][9]);                  //最后一次修改日期
                    newrow[19] = Convert.ToDecimal(row[0][10]);                //总数量
                    newrow[20] = Convert.ToDecimal(row[0][11]);                //发票总数量
                    newrow[21] = Convert.ToDecimal(row[0][12]);                //仓库总数量
                    newrow[22] = Convert.ToDecimal(row[0][13]);                //发送总数量
                    newrow[23] = Convert.ToString(row[0][14]);                 //单据状态
                    newrow[24] = Convert.ToString(row[0][15]);                 //Invoice状态
                    newrow[25] = Convert.ToString(row[0][16]);                 //Payment状态
                    newrow[26] = Convert.ToString(row[0][17]);                 //Shipment状态
                    newrow[28] = Convert.ToDecimal(row[0][18]);                //余额
                    newrow[29] = Convert.ToString(row[0][19]);                 //交货方式
                    newrow[30] = 1;                                            //是否删除(0:是 1:否)
                    newrow[32] = now1.ToString("yyyy-MM-dd HH:mm:ss.fff");     //最后一次操作日期

                    tempdt.Rows.Add(newrow);
                }
                else
                {
                    var now1 = DateTime.Now;
                    var newrow = tempdt.NewRow();
                    newrow[0] = Convert.ToString(row[0][0]);                   //fk(T_BOOKS_SAL外键)
                    newrow[1] = Convert.ToString(row[0][20]);                  //pk
                    newrow[3] = Convert.ToString(row[0][21]);                  //物料ID
                    newrow[4] = Convert.ToString(row[0][22]);                  //仓库名称
                    newrow[5] = Convert.ToString(row[0][23]);                  //sku名称
                    newrow[6] = Convert.ToString(row[0][24]);                  //物料名称
                    newrow[7] = Convert.ToString(row[0][25]);                  //组别名称
                    newrow[8] = Convert.ToString(row[0][26]);                  //描述
                    newrow[9] = Convert.ToDecimal(row[0][27]);                 //汇率
                    newrow[10] = Convert.ToDecimal(row[0][28]);                //汇率(显示使用)
                    newrow[11] = Convert.ToInt32(row[0][29]);                  //数量
                    newrow[12] = Convert.ToString(row[0][30]);                 //单位
                    newrow[13] = Convert.ToDecimal(row[0][31]);                //折扣金额
                    newrow[14] = Convert.ToString(row[0][32]);                 //折扣
                    newrow[15] = Convert.ToString(row[0][33]);                 //税类型
                    newrow[16] = Convert.ToString(row[0][34]);                 //免税代码
                    newrow[17] = Convert.ToDecimal(row[0][35]);                //总金额
                    newrow[18] = Convert.ToDecimal(row[0][36]);                //项目合计
                    newrow[19] = Convert.ToString(row[0][37]);                 //生产类别
                    newrow[20] = Convert.ToString(row[0][38]);                 //行物料类别
                    newrow[21] = Convert.ToString(row[0][39]);                 //物料类别
                    newrow[22] = Convert.ToInt32(row[0][40]);                  //Status-Invoiced
                    newrow[23] = Convert.ToInt32(row[0][41]);                  //Status-Packed
                    newrow[24] = Convert.ToInt32(row[0][42]);                  //Status-Shipped
                    newrow[25] = 1;                                            //是否删除(0:是 1:否)
                    newrow[27] = now1.ToString("yyyy-MM-dd HH:mm:ss.fff");     //最后一次操作日期

                    tempdt.Rows.Add(newrow);
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog("拆分checkDtldt表出现异常,原因:", ex);
            }

            return tempdt;
        }

        /// <summary>
        /// 整理不存在的单据ID-‘监盘功能使用’
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="typeid">0:只插入表头主键  1:插入表头及表体主键</param>
        /// <param name="salesorderid"></param>
        /// <param name="lineitemid"></param>
        /// <returns></returns>
        private DataTable InsertDelDt(DataTable temp,int typeid,string salesorderid,string lineitemid)
        {
            try
            {
                var newrow = temp.NewRow();
                newrow[0] = typeid;
                newrow[1] = salesorderid;
                newrow[2] = lineitemid;
                temp.Rows.Add(newrow);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("插入删除表出现异常,原因:",ex);
            }

            return temp;
        }

        /// <summary>
        /// 根据datarow[] 整理数据至dt  或 dtdtl内
        /// 注:以数组进行整合
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="temp"></param>
        /// <returns></returns>
        //private DataTable ExchangeRecordToDb_ByArray(DataRow [] rows,DataTable temp)
        //{
        //    for (var i = 0; i < rows.Length; i++)
        //    {
        //       temp.ImportRow(rows[i]);
        //    }
        //    return temp;
        //}

        /// <summary>
        /// 根据datarow 整理数据至dt  或 dtdtl内
        /// 注:以Datarow进行整合
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private DataTable ExchangeRecordToDb(DataRow row, DataTable temp)
        {
            temp.ImportRow(row);
            return temp;
        }

        /// <summary>
        /// 根据不同typeid对表头 表体数据表进行整理成,可对数据表操作的表格--针对‘日常操作’功能使用
        /// </summary>
        /// <param name="typeid">0:表头插入 1:表体插入 2:表头更新 3:表体更新 用isdel区分是否删除记录</param>
        /// <param name="isdel">是否删除 0:是 1:否</param>
        /// <param name="tempdt"></param>
        /// <param name="sourcedt"></param>
        /// <returns></returns>
        private DataTable MakeRecordDtToDb(int typeid,int isdel,DataTable tempdt,DataTable sourcedt)
        {
            try
            {
                //0:表头插入
                if (typeid == 0)
                {
                    for (var i = 0; i < sourcedt.Rows.Count; i++)
                    {
                        var now1 = DateTime.Now;
                        var newrow = tempdt.NewRow();

                        for (var j = 0; j < tempdt.Columns.Count; j++)
                        {
                            switch (j)
                            {
                                //是否删除(0:是 1:否)
                                case 30:
                                    newrow[j] = isdel;
                                    break;
                                //国家类别(暂分为:US,MX)=>用于区分ZOHO不同国家类别
                                case 33:
                                    newrow[j] = "US";
                                    break;
                                //单据插入日期(PS:一经插入,不能修改)
                                case 31:
                                //最后一次操作日期(PS:记录最后一次更新日期,更新时使用;每次Up可覆盖更新)
                                case 32:
                                    newrow[j] = now1.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    break;
                                default:
                                    newrow[j] = sourcedt.Rows[i][j];
                                    break;
                            }
                        }
                        tempdt.Rows.Add(newrow);
                    }
                }
                //1:表体插入
                else if (typeid == 1)
                {
                    for (var i = 0; i < sourcedt.Rows.Count; i++)
                    {
                        var now1 = DateTime.Now;
                        var newrow = tempdt.NewRow();

                        for (var j = 0; j < tempdt.Columns.Count; j++)
                        {
                            switch (j)
                            {
                                //是否删除(0:是 1:否)
                                case 25:
                                    newrow[j] = isdel;
                                    break;
                                //单据插入日期(PS:一经插入,不能修改)
                                case 26:
                                //最后一次操作日期(PS:记录最后一次更新日期,更新时使用;每次Up可覆盖更新)
                                case 27:
                                    newrow[j] = now1.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    break;
                                default:
                                    newrow[j] = sourcedt.Rows[i][j];
                                    break;
                            }
                        }
                        tempdt.Rows.Add(newrow);
                    }
                }
                //2:表头更新
                else if (typeid == 2)
                {
                    for (var i = 0; i < sourcedt.Rows.Count; i++)
                    {
                        var now1 = DateTime.Now;
                        var newrow = tempdt.NewRow();

                        for (var j = 0; j < tempdt.Columns.Count; j++)
                        {
                            switch (j)
                            {
                                //是否删除(0:是 1:否)
                                case 30:
                                    newrow[j] = isdel;
                                    break;
                                //当碰到‘单据插入日期’(31) 及 ‘国家类别’(33)跳过
                                case 31:
                                case 33:
                                     continue;
                                //最后一次操作日期(PS:记录最后一次更新日期,更新时使用;每次Up可覆盖更新)
                                case 32:
                                    newrow[j] = now1.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    break;
                                default:
                                    newrow[j] = sourcedt.Rows[i][j];
                                    break;
                            }
                        }
                        tempdt.Rows.Add(newrow);
                    }
                }
                //3:表体更新
                else if (typeid == 3)
                {
                    for (var i = 0; i < sourcedt.Rows.Count; i++)
                    {
                        var now1 = DateTime.Now;
                        var newrow = tempdt.NewRow();

                        for (var j = 0; j < tempdt.Columns.Count; j++)
                        {
                            switch (j)
                            {
                                //是否删除(0:是 1:否)
                                case 25:
                                    newrow[j] = isdel;
                                    break;
                                //当碰到‘单据插入日期’(26)跳过
                                case 26:
                                    continue;
                                //最后一次操作日期(PS:记录最后一次更新日期,更新时使用;每次Up可覆盖更新)
                                case 27:
                                    newrow[j] = now1.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    break;
                                default:
                                    newrow[j] = sourcedt.Rows[i][j];
                                    break;
                            }
                        }
                        tempdt.Rows.Add(newrow);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("插入出现异常,原因:",ex);
            }

            return tempdt;
        }

        /// <summary>
        /// 针对指定表进行数据插入
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        public bool ImportDtToDb(string tableName, DataTable dt)
        {
            var result = true;

            try
            {
                var sqlcon = conDb.GetConnection();
                sqlcon.Open(); //若返回一个SqlConnection的话,必须要显式打开 
                               //注:1)要插入的DataTable内的字段数据类型必须要与数据库内的一致;并且要按数据表内的字段顺序 2)SqlBulkCopy类只提供将数据写入到数据库内
                using (var sqlBulkCopy = new SqlBulkCopy(sqlcon))
                {
                    sqlBulkCopy.BatchSize = 1000;                    //表示以1000行 为一个批次进行插入
                    sqlBulkCopy.DestinationTableName = tableName;  //数据库中对应的表名
                    sqlBulkCopy.NotifyAfter = dt.Rows.Count;      //赋值DataTable的行数
                    sqlBulkCopy.WriteToServer(dt);               //数据导入数据库
                    sqlBulkCopy.Close();                        //关闭连接 
                }
                //sqlcon.Close();
            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.WriteErrorLog("使用SqlBulkCopy插入数据出现异常,原因:",ex);
            }

            return result;
        }

        /// <summary>
        /// 根据指定条件对数据表进行更新
        /// </summary>
        public bool UpdateDbFromDt(string tablename, DataTable dt)
        {
            var sqladpter = new SqlDataAdapter();
            var ds = new DataSet();
            var dtName = "";
            var result = true;

            try
            {
                switch (tablename)
                {
                    case "T_BOOKS_SAL_Check":
                    case "T_BOOKS_SAL":
                        dtName = "T_BOOKS_SAL";
                        break;
                    case "T_BOOKS_SALDTL":
                    case "T_BOOKS_SALDTL_Check":
                        dtName = "T_BOOKS_SALDTL";
                        break;
                }

                //var a = tablename;
                //var b = dtName;

                //根据表格名称获取对应的模板表记录
                var searList = sqlList.SearchUpdateTable(dtName);

                using (sqladpter.SelectCommand = new SqlCommand(searList, conDb.GetConnection()))
                {
                    //将查询的记录填充至ds(查询表记录;后面的更新作赋值使用)
                    sqladpter.Fill(ds);
                    //建立更新模板相关信息(包括更新语句 以及 变量参数)
                    sqladpter = GetUpdateAdapter(tablename, conDb.GetConnection(), sqladpter);
                    //开始更新(注:通过对DataSet中存在的表进行循环赋值;并进行更新)
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        for (var j = 0; j < dt.Columns.Count; j++)
                        {
                            ds.Tables[0].Rows[0].BeginEdit();
                            ds.Tables[0].Rows[0][j] = dt.Rows[i][j];
                            ds.Tables[0].Rows[0].EndEdit();
                        }
                        sqladpter.Update(ds.Tables[0]);
                    }
                    //完成更新后将相关内容清空
                    ds.Tables[0].Clear();
                    sqladpter.Dispose();
                    ds.Dispose();
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.WriteErrorLog("更新出现异常,原因:",ex);
            }

            return result;
        }

        /// <summary>
        /// 建立更新模板相关信息
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="conn"></param>
        /// <param name="da"></param>
        /// <returns></returns>
        private SqlDataAdapter GetUpdateAdapter(string tablename, SqlConnection conn, SqlDataAdapter da)
        {
            try
            {
                //根据tablename获取对应的更新语句
                var sqlscript = sqlList.UpdateEntry(tablename);
                da.UpdateCommand = new SqlCommand(sqlscript, conn);

                //定义所需的变量参数
                switch (tablename)
                {
                    case "T_BOOKS_SAL":
                        da.UpdateCommand.Parameters.Add("@salesorder_id", SqlDbType.NVarChar, 500, "salesorder_id");
                        da.UpdateCommand.Parameters.Add("@customer_name", SqlDbType.NVarChar, 500, "customer_name");
                        da.UpdateCommand.Parameters.Add("@email", SqlDbType.NVarChar, 100, "email");
                        da.UpdateCommand.Parameters.Add("@delivery_date", SqlDbType.NVarChar, 100, "delivery_date");
                        da.UpdateCommand.Parameters.Add("@company_name", SqlDbType.NVarChar, 500, "company_name");
                        da.UpdateCommand.Parameters.Add("@salesorder_number", SqlDbType.NVarChar, 300, "salesorder_number");
                        da.UpdateCommand.Parameters.Add("@reference_number", SqlDbType.NVarChar, 1000, "reference_number");
                        da.UpdateCommand.Parameters.Add("@Orderdate", SqlDbType.NVarChar, 100, "Orderdate");
                        da.UpdateCommand.Parameters.Add("@shipment_date", SqlDbType.NVarChar, 100, "shipment_date");
                        da.UpdateCommand.Parameters.Add("@shipment_days", SqlDbType.NVarChar, 100, "shipment_days");
                        da.UpdateCommand.Parameters.Add("@due_by_days", SqlDbType.NVarChar, 100, "due_by_days");
                        da.UpdateCommand.Parameters.Add("@due_in_days", SqlDbType.NVarChar, 100, "due_in_days");
                        da.UpdateCommand.Parameters.Add("@currency_code", SqlDbType.NVarChar, 10, "currency_code");
                        da.UpdateCommand.Parameters.Add("@total", SqlDbType.Decimal, 4, "total");
                        da.UpdateCommand.Parameters.Add("@bcy_total", SqlDbType.Decimal, 4, "bcy_total");
                        da.UpdateCommand.Parameters.Add("@total_invoiced_amount", SqlDbType.Decimal, 4, "total_invoiced_amount");
                        da.UpdateCommand.Parameters.Add("@last_modified_time", SqlDbType.NVarChar, 100, "last_modified_time");
                        da.UpdateCommand.Parameters.Add("@is_emailed", SqlDbType.NVarChar, 10, "is_emailed");
                        da.UpdateCommand.Parameters.Add("@quantity", SqlDbType.Decimal, 4, "quantity");
                        da.UpdateCommand.Parameters.Add("@quantity_invoiced", SqlDbType.Decimal, 4, "quantity_invoiced");
                        da.UpdateCommand.Parameters.Add("@quantity_packed", SqlDbType.Decimal, 4, "quantity_packed");
                        da.UpdateCommand.Parameters.Add("@quantity_shipped", SqlDbType.Decimal, 4, "quantity_shipped");
                        da.UpdateCommand.Parameters.Add("@order_status", SqlDbType.NVarChar, 300, "order_status");
                        da.UpdateCommand.Parameters.Add("@invoiced_status", SqlDbType.NVarChar, 300, "invoiced_status");
                        da.UpdateCommand.Parameters.Add("@paid_status", SqlDbType.NVarChar, 300, "paid_status");
                        da.UpdateCommand.Parameters.Add("@shipped_status", SqlDbType.NVarChar, 300, "shipped_status");
                        da.UpdateCommand.Parameters.Add("@salesperson_name", SqlDbType.NVarChar, 500, "salesperson_name");
                        da.UpdateCommand.Parameters.Add("@balance", SqlDbType.Decimal, 4, "balance");
                        da.UpdateCommand.Parameters.Add("@delivery_method", SqlDbType.NVarChar, 300, "delivery_method");
                        da.UpdateCommand.Parameters.Add("@IsDel", SqlDbType.Int, 8, "IsDel");
                        da.UpdateCommand.Parameters.Add("@LastChangeDt", SqlDbType.NVarChar, 100, "LastChangeDt");
                        break;
                    case "T_BOOKS_SALDTL":
                        da.UpdateCommand.Parameters.Add("@salesorder_id", SqlDbType.NVarChar, 200, "salesorder_id");
                        da.UpdateCommand.Parameters.Add("@line_item_id", SqlDbType.NVarChar, 200, "line_item_id");
                        da.UpdateCommand.Parameters.Add("@Orderdate", SqlDbType.NVarChar, 100, "Orderdate");
                        da.UpdateCommand.Parameters.Add("@item_id", SqlDbType.NVarChar, 100, "item_id");
                        da.UpdateCommand.Parameters.Add("@warehouse_name", SqlDbType.NVarChar, 200, "warehouse_name");
                        da.UpdateCommand.Parameters.Add("@sku", SqlDbType.NVarChar, 200, "sku");
                        da.UpdateCommand.Parameters.Add("@name", SqlDbType.NVarChar, 200, "name");
                        da.UpdateCommand.Parameters.Add("@group_name", SqlDbType.NVarChar, 200, "group_name");
                        da.UpdateCommand.Parameters.Add("@description", SqlDbType.NVarChar, 500, "description");
                        da.UpdateCommand.Parameters.Add("@bcy_rate", SqlDbType.Decimal, 2, "bcy_rate");
                        da.UpdateCommand.Parameters.Add("@rate", SqlDbType.Decimal, 2, "rate");
                        da.UpdateCommand.Parameters.Add("@quantity", SqlDbType.Int, 8, "quantity");
                        da.UpdateCommand.Parameters.Add("@unit", SqlDbType.NVarChar, 10, "unit");
                        da.UpdateCommand.Parameters.Add("@discount_amount", SqlDbType.Decimal, 2, "discount_amount");
                        da.UpdateCommand.Parameters.Add("@discount", SqlDbType.NVarChar, 100, "discount");
                        da.UpdateCommand.Parameters.Add("@tax_type", SqlDbType.NVarChar, 100, "tax_type");
                        da.UpdateCommand.Parameters.Add("@tax_exemption_code", SqlDbType.NVarChar, 200, "tax_exemption_code");
                        da.UpdateCommand.Parameters.Add("@item_total", SqlDbType.Decimal, 2, "item_total");
                        da.UpdateCommand.Parameters.Add("@item_sub_total", SqlDbType.Decimal, 2, "item_sub_total");
                        da.UpdateCommand.Parameters.Add("@product_type", SqlDbType.NVarChar, 100, "product_type");
                        da.UpdateCommand.Parameters.Add("@line_item_type", SqlDbType.NVarChar, 100, "line_item_type");
                        da.UpdateCommand.Parameters.Add("@item_type", SqlDbType.NVarChar, 100, "item_type");
                        da.UpdateCommand.Parameters.Add("@quantity_invoiced", SqlDbType.Int, 8, "quantity_invoiced");
                        da.UpdateCommand.Parameters.Add("@quantity_packed", SqlDbType.Int, 8, "quantity_packed");
                        da.UpdateCommand.Parameters.Add("@quantity_shipped", SqlDbType.Int, 8, "quantity_shipped");
                        da.UpdateCommand.Parameters.Add("@IsDel", SqlDbType.Int, 8, "IsDel");
                        da.UpdateCommand.Parameters.Add("@LastChangeDt", SqlDbType.NVarChar, 100, "LastChangeDt");
                        break;
                    case "T_BOOKS_SAL_Check":
                        da.UpdateCommand.Parameters.Add("@salesorder_id", SqlDbType.NVarChar, 500, "salesorder_id");
                        da.UpdateCommand.Parameters.Add("@customer_name", SqlDbType.NVarChar, 500, "customer_name");
                        //da.UpdateCommand.Parameters.Add("@email", SqlDbType.NVarChar, 100, "email");
                        //da.UpdateCommand.Parameters.Add("@delivery_date", SqlDbType.DateTime, 10, "delivery_date");
                        //da.UpdateCommand.Parameters.Add("@company_name", SqlDbType.NVarChar, 100, "company_name");
                        da.UpdateCommand.Parameters.Add("@salesorder_number", SqlDbType.NVarChar, 300, "salesorder_number");
                        da.UpdateCommand.Parameters.Add("@reference_number", SqlDbType.NVarChar, 1000, "reference_number");
                        da.UpdateCommand.Parameters.Add("@Orderdate", SqlDbType.NVarChar, 100, "Orderdate");
                        da.UpdateCommand.Parameters.Add("@shipment_date", SqlDbType.NVarChar, 100, "shipment_date");
                        //da.UpdateCommand.Parameters.Add("@shipment_days", SqlDbType.Int, 8, "shipment_days");
                        //da.UpdateCommand.Parameters.Add("@due_by_days", SqlDbType.Int, 8, "due_by_days");
                        //da.UpdateCommand.Parameters.Add("@due_in_days", SqlDbType.Int, 8, "due_in_days");
                        da.UpdateCommand.Parameters.Add("@currency_code", SqlDbType.NVarChar, 100, "currency_code");
                        da.UpdateCommand.Parameters.Add("@total", SqlDbType.Decimal, 4, "total");
                        da.UpdateCommand.Parameters.Add("@bcy_total", SqlDbType.Decimal, 4, "bcy_total");
                        //da.UpdateCommand.Parameters.Add("@total_invoiced_amount", SqlDbType.Decimal, 4, "total_invoiced_amount");
                        da.UpdateCommand.Parameters.Add("@last_modified_time", SqlDbType.NVarChar, 100, "last_modified_time");
                        //da.UpdateCommand.Parameters.Add("@is_emailed", SqlDbType.NVarChar, 100, "is_emailed");
                        da.UpdateCommand.Parameters.Add("@quantity", SqlDbType.Decimal, 4, "quantity");
                        da.UpdateCommand.Parameters.Add("@quantity_invoiced", SqlDbType.Decimal, 4, "quantity_invoiced");
                        da.UpdateCommand.Parameters.Add("@quantity_packed", SqlDbType.Decimal, 4, "quantity_packed");
                        da.UpdateCommand.Parameters.Add("@quantity_shipped", SqlDbType.Decimal, 4, "quantity_shipped");
                        da.UpdateCommand.Parameters.Add("@order_status", SqlDbType.NVarChar, 300, "order_status");
                        da.UpdateCommand.Parameters.Add("@invoiced_status", SqlDbType.NVarChar, 300, "invoiced_status");
                        da.UpdateCommand.Parameters.Add("@paid_status", SqlDbType.NVarChar, 300, "paid_status");
                        da.UpdateCommand.Parameters.Add("@shipped_status", SqlDbType.NVarChar, 300, "shipped_status");
                        //da.UpdateCommand.Parameters.Add("@salesperson_name", SqlDbType.NVarChar, 100, "salesperson_name");
                        da.UpdateCommand.Parameters.Add("@balance", SqlDbType.Decimal, 4, "balance");
                        da.UpdateCommand.Parameters.Add("@delivery_method", SqlDbType.NVarChar, 300, "delivery_method");
                        da.UpdateCommand.Parameters.Add("@IsDel", SqlDbType.Int, 8, "IsDel");
                        da.UpdateCommand.Parameters.Add("@LastChangeDt", SqlDbType.NVarChar, 100, "LastChangeDt");
                        break;
                    case "T_BOOKS_SALDTL_Check":
                        da.UpdateCommand.Parameters.Add("@salesorder_id", SqlDbType.NVarChar, 200, "salesorder_id");
                        da.UpdateCommand.Parameters.Add("@line_item_id", SqlDbType.NVarChar, 200, "line_item_id");
                        //da.UpdateCommand.Parameters.Add("@Orderdate", SqlDbType.DateTime, 10, "Orderdate");
                        da.UpdateCommand.Parameters.Add("@item_id", SqlDbType.NVarChar, 100, "item_id");
                        da.UpdateCommand.Parameters.Add("@warehouse_name", SqlDbType.NVarChar, 200, "warehouse_name");
                        da.UpdateCommand.Parameters.Add("@sku", SqlDbType.NVarChar, 200, "sku");
                        da.UpdateCommand.Parameters.Add("@name", SqlDbType.NVarChar, 200, "name");
                        da.UpdateCommand.Parameters.Add("@group_name", SqlDbType.NVarChar, 200, "group_name");
                        da.UpdateCommand.Parameters.Add("@description", SqlDbType.NVarChar, 500, "description");
                        da.UpdateCommand.Parameters.Add("@bcy_rate", SqlDbType.Decimal, 2, "bcy_rate");
                        da.UpdateCommand.Parameters.Add("@rate", SqlDbType.Decimal, 2, "rate");
                        da.UpdateCommand.Parameters.Add("@quantity", SqlDbType.Int, 8, "quantity");
                        da.UpdateCommand.Parameters.Add("@unit", SqlDbType.NVarChar, 10, "unit");
                        da.UpdateCommand.Parameters.Add("@discount_amount", SqlDbType.Decimal, 2, "discount_amount");
                        da.UpdateCommand.Parameters.Add("@discount", SqlDbType.NVarChar, 100, "discount");
                        da.UpdateCommand.Parameters.Add("@tax_type", SqlDbType.NVarChar, 100, "tax_type");
                        da.UpdateCommand.Parameters.Add("@tax_exemption_code", SqlDbType.NVarChar, 200, "tax_exemption_code");
                        da.UpdateCommand.Parameters.Add("@item_total", SqlDbType.Decimal, 2, "item_total");
                        da.UpdateCommand.Parameters.Add("@item_sub_total", SqlDbType.Decimal, 2, "item_sub_total");
                        da.UpdateCommand.Parameters.Add("@product_type", SqlDbType.NVarChar, 100, "product_type");
                        da.UpdateCommand.Parameters.Add("@line_item_type", SqlDbType.NVarChar, 100, "line_item_type");
                        da.UpdateCommand.Parameters.Add("@item_type", SqlDbType.NVarChar, 100, "item_type");
                        da.UpdateCommand.Parameters.Add("@quantity_invoiced", SqlDbType.Int, 8, "quantity_invoiced");
                        da.UpdateCommand.Parameters.Add("@quantity_packed", SqlDbType.Int, 8, "quantity_packed");
                        da.UpdateCommand.Parameters.Add("@quantity_shipped", SqlDbType.Int, 8, "quantity_shipped");
                        da.UpdateCommand.Parameters.Add("@IsDel", SqlDbType.Int, 8, "IsDel");
                        da.UpdateCommand.Parameters.Add("@LastChangeDt", SqlDbType.NVarChar, 100, "LastChangeDt");
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("设置UpdateCommand更新值出现异常,原因:", ex);
            }
            return da;
        }

    }
}
