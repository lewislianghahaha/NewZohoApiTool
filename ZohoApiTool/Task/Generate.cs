using System;
using System.Data;
using System.Data.SqlClient;
using System.Deployment.Application;
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
                LogHelper.WriteLog("开始执行....");

                //获取各功能所需临时表
                //获取Api表头临时表
                var apiheaddt = tempDtList.MakeSalHeadApiDtTemp();
                //获取Api表体临时表
                var apidtldt = tempDtList.MakeSalDetailApiDtTemp();

                //获取T_BOOKS_SAL表信息-插入使用
                var insertDt = tempDtList.MakeSalHeadDtTemp();
                //获取T_BOOK_SALDTL表信息-插入使用
                var insertDtlDt = tempDtList.MakeSalDetailDtTemp();

                //获取T_BOOKS_SAL表信息-更新使用
                var upDt = insertDt.Clone();
                //获取T_BOOK_SALDTL表信息-更新使用
                var updtlDtl = insertDtlDt.Clone();

                //获取从DB获取的各数据源
                //获取表头数据行数-当数据表没有数据时使用(ps:限一次使用)
                var initialDt = searchDt.GetIninitalRecord();
                //获取T_BOOKS_SAL 表头信息-用作比较
                var headDt = searchDt.GetBooksSalHeadRecord();
                //获取T_BOOKS_SALDTL 表体信息-用作比较
                var dtldt = searchDt.GetBooksSalDetailRecord();


                /////////////////////////////API数据收集部份/////////////////////////////////
                //通过zoho Api获取相关信息
                //获取‘刷新令牌’
                var accesstoken = getApi.GetAccessToken();
                //获取zoho api表头返回记录
                apiheaddt.Merge(getApi.GetSalHeadRecord(apiheaddt,accesstoken));

                var b = apiheaddt.Copy();

                //循环获取zoho api表体返回记录
                foreach (DataRow rows in apiheaddt.Rows)
                {
                    var a1 = Convert.ToString(rows[0]);
                    apidtldt.Merge(getApi.GetSalDetailRecord(apidtldt,Convert.ToString(rows[0]),accesstoken));
                }

                var a = apidtldt.Copy();

                //todo:


                //////////////////////////////数据整合部份/////////////////////////////////////
                //当判断数据表没有任何记录时,直接将API返回的表头 表体记录集,整理后分别插入至对应数据表

                if (Convert.ToInt32(initialDt.Rows[0][0]) == 0)
                {
                    //分别将表头 表体数据插入至insertDt 及 insertDtlDt内
                    insertDt.Merge(MakeRecordDtToDb(0, 1,insertDt, apiheaddt));
                    insertDtlDt.Merge(MakeRecordDtToDb(1, 1,insertDtlDt, apidtldt));
                }
                else
                {
                    //todo:日常操作-循环将ApiDt放到HeadDt,DtlDt内查找,无->插入 有->更新
                    //todo:表头操作
                    foreach (DataRow rows in apiheaddt.Rows)
                    {
                        var dtlrows = headDt.Select("salesorder_id ='" + Convert.ToString(rows[0]) + "'");
                        //新记录-插入操作
                        if (dtlrows.Length == 0)
                        {
                            //todo:将dtlrows进行数据整理-(注:以apiheaddt表结构进行插入数据)
                            //todo:将结果添加至MakeRecordDtToDb()内
                            insertDt.Merge(MakeRecordDtToDb(0, 1, insertDt, ExchangeRecordToDb_ByDatarows(rows, apiheaddt.Clone())));
                        }
                        //旧记录-更新操作
                        else
                        {
                            upDt.Merge(MakeRecordDtToDb(0,1,upDt, ExchangeRecordToDb_ByArray(dtlrows, apiheaddt.Clone())));
                        }
                    }

                    //todo:表体操作
                    foreach (DataRow row in apidtldt.Rows)
                    {
                        var dtlrows = dtldt.Select("salesorder_id ='" + Convert.ToString(row[0]) + "'");
                        //新记录-插入操作
                        if (dtlrows.Length == 0)
                        {
                            insertDtlDt.Merge(MakeRecordDtToDb(1,1,insertDtlDt,ExchangeRecordToDb_ByDatarows(row,apidtldt.Clone())));
                        }
                        //旧记录-更新操作
                        else
                        {
                            updtlDtl.Merge(MakeRecordDtToDb(1,1,updtlDtl,ExchangeRecordToDb_ByArray(dtlrows,apidtldt.Clone())));
                        }
                    }

                    var a1 = insertDt.Copy();
                    var a2 = insertDtlDt.Copy();
                    var a3 = upDt.Copy();
                    var a4 = updtlDtl.Copy();

                    ///////////////////////////////盘点操作////////////////////////////////////////////
                    //todo:作用:1.检查表头 表体单据是否删除 2.对表(头)体进行数值更新
                    //
                    



                }

                //todo:执行‘插入’ 及 ‘更新’操作
                if (insertDt.Rows.Count > 0)
                    ImportDtToDb("T_BOOKS_SAL", insertDt);
                if (insertDtlDt.Rows.Count > 0)
                    ImportDtToDb("T_BOOKS_SALDTL", insertDtlDt);

                if (upDt.Rows.Count > 0)
                    UpdateDbFromDt("T_BOOKS_SAL", upDt);

                if (updtlDtl.Rows.Count > 0)
                    UpdateDbFromDt("T_BOOKS_SALDTL",updtlDtl);

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
        /// 根据datarow[] 整理数据至dt  或 dtdtl内
        /// 注:以数组进行整合
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="temp"></param>
        /// <returns></returns>
        private DataTable ExchangeRecordToDb_ByArray(DataRow [] rows,DataTable temp)
        {
            for (var i = 0; i < rows.Length; i++)
            {
               temp.ImportRow(rows[i]);
            }
            return temp;
        }

        /// <summary>
        /// 根据datarow 整理数据至dt  或 dtdtl内
        /// 注:以Datarow进行整合
        /// </summary>
        /// <param name="row"></param>
        /// <param name="temp"></param>
        /// <returns></returns>
        private DataTable ExchangeRecordToDb_ByDatarows(DataRow row, DataTable temp)
        {
            temp.ImportRow(row);
            return temp;
        }

        /// <summary>
        /// 根据不同typeid对表头 表体数据表进行整理并输出
        /// </summary>
        /// <param name="typeid">0:表头 1:表体 用isdel区分是否删除记录</param>
        /// <param name="isdel">是否删除 0:是 1:否</param>
        /// <param name="tempdt"></param>
        /// <param name="sourcedt"></param>
        /// <returns></returns>
        private DataTable MakeRecordDtToDb(int typeid,int isdel,DataTable tempdt,DataTable sourcedt)
        {
            try
            {
                if (typeid == 0)
                {
                    for (var i = 0; i < sourcedt.Rows.Count; i++)
                    {
                        for (var j = 0; j < tempdt.Columns.Count; j++)
                        {
                            var newrow = tempdt.NewRow();

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
                                    newrow[j] = DateTime.Now.Date;
                                    break;
                                default:
                                    newrow[j] = sourcedt.Rows[i][j];
                                    break;
                            }

                            tempdt.Rows.Add(newrow);
                        }
                    }
                }
                else if (typeid == 1)
                {
                    for (var i = 0; i < sourcedt.Rows.Count; i++)
                    {
                        for (var j = 0; j < tempdt.Columns.Count; j++)
                        {
                            var newrow = tempdt.NewRow();

                            switch (j)
                            {
                                //是否删除(0:是 1:否)
                                case 25:
                                    newrow[j] = isdel;
                                    break;
                                //国家类别(暂分为:US,MX)=>用于区分ZOHO不同国家类别
                                case 28:
                                    newrow[j] = "US";
                                    break;
                                //单据插入日期(PS:一经插入,不能修改)
                                case 26:
                                //最后一次操作日期(PS:记录最后一次更新日期,更新时使用;每次Up可覆盖更新)
                                case 27:
                                    newrow[j] = DateTime.Now.Date;
                                    break;
                                default:
                                    newrow[j] = sourcedt.Rows[i][j];
                                    break;
                            }

                            tempdt.Rows.Add(newrow);
                        }
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
        public void ImportDtToDb(string tableName, DataTable dt)
        {
            var sqlcon = conDb.GetConnection();
            // sqlcon.Open(); 若返回一个SqlConnection的话,必须要显式打开 
            //注:1)要插入的DataTable内的字段数据类型必须要与数据库内的一致;并且要按数据表内的字段顺序 2)SqlBulkCopy类只提供将数据写入到数据库内
            using (var sqlBulkCopy = new SqlBulkCopy(sqlcon))
            {
                sqlBulkCopy.BatchSize = 1000;                    //表示以1000行 为一个批次进行插入
                sqlBulkCopy.DestinationTableName = tableName;  //数据库中对应的表名
                sqlBulkCopy.NotifyAfter = dt.Rows.Count;      //赋值DataTable的行数
                sqlBulkCopy.WriteToServer(dt);               //数据导入数据库
                sqlBulkCopy.Close();                        //关闭连接 
            }
            // sqlcon.Close();
        }

        /// <summary>
        /// 根据指定条件对数据表进行更新
        /// </summary>
        public void UpdateDbFromDt(string tablename, DataTable dt)
        {
            var sqladpter = new SqlDataAdapter();
            var ds = new DataSet();

            //根据表格名称获取对应的模板表记录
            var searList = sqlList.SearchUpdateTable(tablename);

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

        /// <summary>
        /// 建立更新模板相关信息
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="conn"></param>
        /// <param name="da"></param>
        /// <returns></returns>
        private SqlDataAdapter GetUpdateAdapter(string tablename, SqlConnection conn, SqlDataAdapter da)
        {
            //根据tablename获取对应的更新语句
            var sqlscript = sqlList.UpdateEntry(tablename);
            da.UpdateCommand = new SqlCommand(sqlscript, conn);

            //定义所需的变量参数
            switch (tablename)
            {
                case "":
                    da.UpdateCommand.Parameters.Add("@FId", SqlDbType.Int, 8, "FId");
                    da.UpdateCommand.Parameters.Add("@OAorderno", SqlDbType.NVarChar, 100, "OAorderno");
                    da.UpdateCommand.Parameters.Add("@Fstatus", SqlDbType.Int, 8, "Fstatus");
                    da.UpdateCommand.Parameters.Add("@CreateDt", SqlDbType.DateTime, 10, "CreateDt");
                    da.UpdateCommand.Parameters.Add("@ConfirmDt", SqlDbType.DateTime, 10, "ConfirmDt");
                    da.UpdateCommand.Parameters.Add("@CreateName", SqlDbType.NVarChar, 100, "CreateName");
                    da.UpdateCommand.Parameters.Add("@Useid", SqlDbType.Int, 8, "Useid");
                    da.UpdateCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 200, "UserName");
                    da.UpdateCommand.Parameters.Add("@Typeid", SqlDbType.Int, 8, "Typeid");
                    da.UpdateCommand.Parameters.Add("@DevGroupid", SqlDbType.Int, 8, "DevGroupid");
                    break;
            }
            return da;
        }

    }
}
