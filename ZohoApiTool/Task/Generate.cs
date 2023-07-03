﻿using System;
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

                //////////////////////////////数据整合部份/////////////////////////////////////
                //todo:当判断数据表没有任何记录时,直接将API返回的表头 表体记录集,整理后分别插入至数据表
                if (Convert.ToInt32(initialDt.Rows[0][0]) == 0)
                {
                    //todo:分别将表头 表体数据插入至insertDt 及 insertDtlDt内
                    insertDt.Merge(MakeInsertRecordDt(0, insertDt, headDt));
                    insertDtlDt.Merge(MakeInsertRecordDt(1, insertDtlDt, dtldt));
                }
                else
                {
                    //todo:日常操作-将ApiDt放到DbDt内查找,无->插入 有->更新



                    //todo:检测到当天是1号时,执行‘盘点操作’功能-涉及更新
                    //todo 作用:使用headDt放到表体API进行查找,若发现没有的,就将IsDel字段=0;若存在,即更新表体所有字段(除IsDel字段)内容
                    if (1==1)
                    {

                    }
                    //todo:反之,将...
                    else
                    {
                        
                    }
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
        /// 根据不同typeid对表头 表体数据表进行整理并输出
        /// </summary>
        /// <param name="typeid">0:表头 1:表体</param>
        /// <param name="tempdt"></param>
        /// <param name="sourcedt"></param>
        /// <returns></returns>
        private DataTable MakeInsertRecordDt(int typeid,DataTable tempdt,DataTable sourcedt)
        {
            if (typeid == 0)
            {
                foreach (DataRow rows in sourcedt.Rows)
                {
                    var newrow = tempdt.NewRow();
                    //
                    //
                    //
                    //
                    //
                    tempdt.Rows.Add(newrow);
                }
            }
            else
            {
                foreach (DataRow rows in sourcedt.Rows)
                {
                    var newrow = tempdt.NewRow();
                    //
                    //
                    //
                    //
                    //
                    tempdt.Rows.Add(newrow);
                }
            }
            return tempdt;
        }

        /// <summary>
        /// 根据不同typeid对表头 表体数据表进行整理并输出
        /// </summary>
        /// <param name="typeid">0:表头 1:表体(无删除的记录) 2:表体(删除的记录)</param>
        /// <param name="tempdt"></param>
        /// <param name="sourcedt"></param>
        /// <returns></returns>
        private DataTable MakeUpRecordDt(int typeid, DataTable tempdt, DataTable sourcedt)
        {
            switch (typeid)
            {
                case 0:

                    break;
                case 1:

                    break;
                case 2:

                    break;
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
