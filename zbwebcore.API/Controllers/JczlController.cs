using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeetleX.FastHttpApi;
using SqlSugar;
using zbwebcore.API.BllDal;
using zbwebcore.API.Model;

namespace zbwebcore.API.Controllers
{
    [Options(AllowHeaders = "*,token", AllowOrigin = "*")]
    [Controller(BaseUrl = "jczl/")]
    public class JczlController
    {
        #region 增删改查统一调用公共方法
        //可在每个表中增加rowid自增列,这样前端就不用传主键列

        public object getAllData(IHttpContext context)
        {
            
            int total = 0;
            bool flag = false;
            string msg = "无符合条件的数据!";
            DataTable dt = null, dttotal = null;
            try
            {
                //var yhbh = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbh;
                var page = Convert.ToInt32(context.Data["page"]);
                var limit = Convert.ToInt32(context.Data["limit"]);
                if (context.Data.ToString().IndexOf("tname") < 0)
                {
                    msg = "未指定需要查询的表名!";
                }
                else
                {
                    string mcsql="";
                    if (context.Data.ToString().IndexOf("mcfield") >= 0)
                    {
                        mcsql = getmcsql(context.Data["mcfield"].ToString());
                    }
                    var tname = context.Data["tname"].ToString();
                    StringBuilder sb = new StringBuilder();
                    sb.Append($" select t.* {mcsql} from {tname} t where 1=1");
                    if (context.Data.ToString().IndexOf("cxtj") >= 0)
                    {
                        var cxtj = BllPub.Instance.getCxtj(context.Data["cxtj"].ToString());
                        sb.Append(cxtj);
                    }
                    dt = Dbservice.DB.Queryable<object>().AS("(" + sb.ToString() + ") as t").ToDataTablePage(page, limit, ref total);
                    if (dt.Rows.Count > 0)
                    {
                        flag = true;
                        msg = "获取成功!";
                    }
                }
                
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return new JsonResult(new { code = flag ? 0 : 200, msg = msg, count = total, data = dt, totalRow = dttotal });
        }
        public string getmcsql(string mcstr)
        {
            string sql = "";
            try
            {
                var mcarray = mcstr.Split(',');
                for (int i = 0; i < mcarray.Length; i++)
                {
                    var bh = mcarray[i];
                    if (bh == "bmbh")
                    {
                        sql += "," + bh + "mc=(select bmbh+'-'+bmmc from jczl_bmzd where t.bmbh=jczl_bmzd.bmbh) ";
                    }
                    if (bh == "fdbh")
                    {
                        sql += "," + bh + "mc=(select fdbh+'-'+fdmc from jczl_fdzd where t.fdbh=jczl_fdzd.fdbh) ";
                    }
                }
                //if (sql.Length > 0)
                //{
                //    sql = sql.Substring(0, sql.Length - 1);
                //}
            }
            catch (Exception ex)
            {

            }
            return sql;
        }
        public object getOneData(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            DataTable dt = null;
            string sql = "", tfield="";
            try
            {
                if (context.Data.ToString().IndexOf("tname") >= 0)
                {
                    if(context.Data.ToString().IndexOf("tfield") >= 0)
                    {
                        tfield = context.Data["tfield"].ToString();
                    }
                    else
                    {
                        tfield = "rowid";
                    }
                    var tname = context.Data["tname"].ToString();
                    var id = context.Data[$"{tfield}"].ToString();
                    sql = $"select * from {tname} where {tfield}='{id}'";
                    dt = Dbservice.DB.Ado.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        //DataTable dt = dataGridView1.DataSource as DataTable;
                        //foreach (DataColumn dc in dt.Columns)
                        //{
                        //    if (dc.DataType == (new DateTime()).GetType())
                        //    {
                        //        dt.Rows[0][dc.ColumnName] =DateTime.Parse(dt.Rows[0][dc.ColumnName].ToString()).ToString("yyyy-MM-dd");
                        //    }
                        //}
                        flag = true;
                        msg = "获取数据成功!";
                    }
                }
                else
                {
                    msg = "请指定需要查询的表名和字段名!";
                }
            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, data = dt }); ;
        }
        public object saveData(IHttpContext context)
        {
            string msg = "保存失败!", datanum = "", tfield="";
            bool flag = false;
            try
            {
                if (context.Data.ToString().IndexOf("tname") >= 0)
                {
                    if (context.Data.ToString().IndexOf("tfield") >= 0)
                    {
                        tfield = context.Data["tfield"].ToString();
                    }
                    else
                    {
                        tfield = "rowid";
                    }
                    var tname = context.Data["tname"].ToString();
                    var obj = Dbservice.DB.Utilities.DeserializeObject<Dictionary<string, object>>(context.Data["data"].ToString());
                    string getcolsql = $"select name from syscolumns where id=(select id from sysobjects where type='U' and name='{tname}') ";
                    var dtcol = Dbservice.DB.Ado.GetDataTable(getcolsql);
                    string colstr = "";
                    for (int i = 0; i < dtcol.Rows.Count; i++)
                    {
                        colstr += dtcol.Rows[i]["name"].ToString() + ",";
                    }
                    string[] igcol=null;
                    string igcolstr = "";
                    for (int i = 0; i < obj.Count; i++)
                    {
                        var item = obj.ElementAt(i);
                        string key = item.Key.ToString();
                        if (colstr.IndexOf(key) < 0)
                        {
                            igcolstr += key+",";
                        }
                    }
                    if (igcolstr.Length > 0)
                    {
                        igcol = igcolstr.Split(",");
                    }
                    var id = obj[$"{tfield}"].ToString();
                    if (string.IsNullOrEmpty(id))
                    {
                        var ret = Dbservice.DB.Insertable(obj).AS($"{tname}").IgnoreColumns(igcol).ExecuteReturnIdentity();
                        if (ret > 0)
                        {
                            flag = true;
                            msg = "保存成功!";
                            datanum = ret.ToString();
                        }
                    }
                    else
                    {
                        var ret = Dbservice.DB.Updateable(obj).AS($"{tname}").WhereColumns($"{tfield}").IgnoreColumns(igcol).With(SqlWith.UpdLock).ExecuteCommand();
                        if (ret > 0)
                        {
                            flag = true;
                            msg = "更新成功!";
                        }
                    }
                }
                else
                {
                    msg = "请指定需要查询的表名和字段名!";
                }


            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, datanum = datanum }); ;
        }
        public object delOneData(IHttpContext context)
        {
            string msg = "删除数据失败!", tfield="";
            bool flag = false;
            try
            {
                if (context.Data.ToString().IndexOf("tname") >= 0)
                {
                    if (context.Data.ToString().IndexOf("tfield") >= 0)
                    {
                        tfield = context.Data["tfield"].ToString();
                    }
                    else
                    {
                        tfield = "rowid";
                    }
                    var tname = context.Data["tname"].ToString();
                    var id = context.Data[$"{tfield}"].ToString();
                    string sql = $"delete from {tname} where {tfield}='{id}'";
                    var ret = Dbservice.DB.Ado.ExecuteCommand(sql);
                    if (ret > 0)
                    {
                        flag = true;
                        msg = "删除数据成功!";
                    }
                }
                else
                {
                    msg = "请指定需要查询的表名和字段名!";
                }
            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg }); ;
        }
        public object getSelectData(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            StringBuilder sb = new StringBuilder();
           
            DataTable dt = null;
            try
            {
                if (context.Data.ToString().IndexOf("tfield") >= 0 && context.Data.ToString().IndexOf("tname") >= 0 && context.Data.ToString().IndexOf("tvalue") >= 0)
                {
                    var tname = context.Data["tname"].ToString();
                    var tfield = context.Data["tfield"].ToString();
                    var tvalue = context.Data["tvalue"].ToString();
                    sb.Append($"select {tvalue} as value,{tfield} as name from {tname}(nolock) where 1=1  order by {tfield}");
                }
                dt = Dbservice.DB.Ado.GetDataTable(sb.ToString());
                if (dt.Rows.Count > 0)
                {
                    flag = true;
                    msg = "获取数据成功!";
                }
            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, data = dt });
        }
        #endregion
        #region 存储过程统一调用公共方法
        public object getProcData(IHttpContext context)
        {
            bool flag = false;
            string msg = "",cxtj="";
            DataTable dt = null;
            try
            {
                if (context.Data.ToString().IndexOf("cxtj") >= 0)
                {
                    cxtj = context.Data["cxtj"].ToString();
                }
                var p_cxtj = new SugarParameter("@cxtj", cxtj);
                var p_flag = new SugarParameter("@flag", null, true);
                var p_msg = new SugarParameter("@msg", null, true);
                dt = Dbservice.DB.Ado.UseStoredProcedure().GetDataTable(
                    "up_xx_xxxx"
                    , p_cxtj
                    , p_flag
                    , p_msg

                    );

                msg = p_msg.Value.ToString();
                if (dt.Rows.Count > 0)
                {
                    flag = Convert.ToBoolean(int.Parse(p_flag.Value.ToString()));
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, data = dt });
        }
        #endregion

        public object getSelectGyszd(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            StringBuilder sb = new StringBuilder();
            DataTable dt = null;
            string wheresql = "", topstr = "top 20";
            try
            {
                if(context.Data.ToString().IndexOf("val") >= 0)
                {
                    var val = context.Data["val"].ToString();
                    wheresql = $" and gysbh+gysmc like '%{val}%'";
                }
                if(context.Data.ToString().IndexOf("arrstr") >= 0)
                {
                    var arrstr = context.Data["arrstr"].ToString();
                    if (!string.IsNullOrEmpty(arrstr))
                    {
                        wheresql = $" and gysbh in ('{arrstr.Replace(",", "','")}')";
                        topstr = "";
                    }
                }
                
                sb.Append($"select {topstr} gysbh as value,gysbh+'-'+gysmc as name from jczl_gyszd(nolock) where 1=1 {wheresql} order by gysbh");

                dt = Dbservice.DB.Ado.GetDataTable(sb.ToString());
                if (dt.Rows.Count > 0)
                {
                    flag = true;
                    msg = "获取数据成功!";
                }
            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new SpanJsonResult(new { flag = flag, msg = msg, data = dt });
        }
        public object getSelectZgzd(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            StringBuilder sb = new StringBuilder();
            DataTable dt = null;
            string wheresql = "",topstr="top 20";
            try
            {
                if (context.Data.ToString().IndexOf("val") >= 0)
                {
                    var val = context.Data["val"].ToString();
                    wheresql = $" and zgbh+zgmc like '%{val}%'";
                }
                if (context.Data.ToString().IndexOf("arrstr") >= 0)
                {
                    var arrstr = context.Data["arrstr"].ToString();
                    if (!string.IsNullOrEmpty(arrstr))
                    {
                        wheresql = $" and zgbh in ('{arrstr.Replace(",", "','")}')";
                    }
                    topstr = "";
                }
                sb.Append($"select {topstr} zgbh as value,zgbh+'-'+zgmc as name from jczl_zgzd(nolock) where 1=1 {wheresql} order by zgbh");

                dt = Dbservice.DB.Ado.GetDataTable(sb.ToString());
                if (dt.Rows.Count > 0)
                {
                    flag = true;
                    msg = "获取数据成功!";
                }
            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, data = dt });
        }
    }
}