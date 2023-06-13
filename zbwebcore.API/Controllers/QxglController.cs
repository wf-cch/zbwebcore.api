using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BeetleX.FastHttpApi;
using BeetleX.FastHttpApi.Data;
using SqlSugar;
using zbwebcore.API.BllDal;
using zbwebcore.API.Config;
using zbwebcore.API.Model;

namespace zbwebcore.API.Controllers
{
    [Options(AllowHeaders = "*,token", AllowOrigin = "*")]
    [Controller(BaseUrl = "qxgl/")]
    public class QxglController
    {
        #region 菜单
        public object getMenu(IHttpContext context)
        {
            string msg = "error";
            int code = -1;
            List<qxgl_menu> list = null;
            List<qxgl_menu> listnew = null;
            try
            {
                var yhzidstr = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhzid;
                var yhzid = "'" + yhzidstr.Replace(",", "','") + "'";
                string sql = $"select distinct zybh,title,fcode,href,icon from qxgl_menu t left join qxgl_yhzqxmenu a on t.menuid=a.menuid where t.isvisible='1' and a.yhzid in ({yhzid}) order by zybh";
                list = Dbservice.DB.Ado.SqlQuery<qxgl_menu>(sql);
                listnew = GetTreeListDb(list, "00");

                code = 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return new JsonResult(new { code = code, msg = msg, data = listnew });
        }

        public List<qxgl_menu> GetTreeListDb(List<qxgl_menu> list, string fcode)
        {
            List<qxgl_menu> TreeList = new List<qxgl_menu>();
            List<qxgl_menu> ModelList = GetChildMenuListDb(list, fcode);
            foreach (var item in ModelList)
            {
                qxgl_menu m = new qxgl_menu();
                //m.id = string.IsNullOrEmpty(item.id) ? "" : item.id.ToString();
                m.title = string.IsNullOrEmpty(item.title) ? "" : item.title.ToString();
                m.href = string.IsNullOrEmpty(item.href) ? "" : item.href.ToString();
                m.icon = string.IsNullOrEmpty(item.icon) ? "" : item.icon.ToString();
                m.childs = GetTreeListDb(list, item.zybh);
                TreeList.Add(m);
            }
            return TreeList;
        }
        public List<qxgl_menu> GetChildMenuListDb(List<qxgl_menu> list, string fcode)
        {
            var result = list.Where(x => x.fcode == fcode);
            return result.ToList();
        }

        public object getMenuEdit(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            int total = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from qxgl_menu where 1=1 ");

            DataTable dt = null;
            try
            {
                if (context.Data.ToString().IndexOf("cxtj") > 0)
                {
                    var cxtj = BllPub.Instance.getCxtj(context.Data["cxtj"].ToString());
                    sb.Append(cxtj);
                }

                var yhbh = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbh;
                var page = Convert.ToInt32(context.Data["page"].ToString());
                var limit = Convert.ToInt32(context.Data["limit"].ToString());
                dt = Dbservice.DB.Queryable<object>().AS("(" + sb.ToString() + ") as t").ToDataTablePage(page, limit, ref total);
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
            return new JsonResult(new { code = flag ? 0 : 200, count = total, msg = msg, data = dt });
        }
        public object getMenuAll(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            int total = 0;
            var yhzid = context.Data["yhzid"].ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append($"select zybh,title,menuid,LAY_CHECKED=(select 'true' from qxgl_yhzqxmenu where menuid=t.menuid and yhzid='{yhzid}' ) from qxgl_menu t where 1=1 ");
           
            DataTable dt = null;
            try
            {
                dt = Dbservice.DB.Ado.GetDataTable(sb.ToString());
                if (dt.Rows.Count > 0)
                {
                    total = dt.Rows.Count;
                    flag = true;
                    msg = "获取数据成功!";
                }
              
            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, count = total, msg = msg, data = dt });
        }
        public object getMenuOne(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            DataTable dt = null;
            try
            {
                var menuid = context.Data["menuid"].ToString();
                string sql = $"select * from qxgl_menu where menuid='{menuid}'";
                dt = Dbservice.DB.Ado.GetDataTable(sql);
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
            return new JsonResult(new { flag = flag, msg = msg, data = dt }); ;
        }
        public object delMenu(IHttpContext context)
        {
            string msg = "删除数据失败!";
            bool flag = false;
            try
            {
                var menuid = context.Data["menuid"].ToString();
                string sql = $"delete from qxgl_menu where menuid='{menuid}'";
                var ret = Dbservice.DB.Ado.ExecuteCommand(sql);
                if (ret > 0)
                {
                    flag = true;
                    msg = "删除数据成功!";
                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg }); ;
        }
        public object saveMenu(IHttpContext context)
        {
            string msg = "保存失败!", datanum = "";
            bool flag = false;
            try
            {
                var obj = Dbservice.DB.Utilities.DeserializeObject<Dictionary<string, object>>(context.Data["data"].ToString());
                var id = obj["menuid"].ToString();
                if (string.IsNullOrEmpty(id))
                {
                    var ret = Dbservice.DB.Insertable(obj).AS("qxgl_menu").ExecuteReturnIdentity();
                    if (ret > 0)
                    {
                        flag = true;
                        msg = "保存成功!";
                        datanum = ret.ToString();
                    }
                }
                else
                {
                    var ret = Dbservice.DB.Updateable(obj).AS("qxgl_menu").WhereColumns("menuid").With(SqlWith.UpdLock).ExecuteCommand();
                    if (ret > 0)
                    {
                        flag = true;
                        msg = "更新成功!";
                    }
                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, datanum = datanum }); ;
        }
        #endregion
        #region 用户

        public object getUser(IHttpContext context)
        {
            
            int total = 0;
            bool flag = false;
            string msg = "无符合条件的数据!";
            DataTable dt = null, dttotal=null;
            try
            {
                var yhbh = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbh;
                var page = Convert.ToInt32(context.Data["page"]);
                var limit = Convert.ToInt32(context.Data["limit"]);
                StringBuilder sb = new StringBuilder();
                sb.Append(" select * from qxgl_yhzd t where 1=1");
                if (context.Data.ToString().IndexOf("cxtj") > 0)
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
                string totalsql = "select '合计:' as  yhbh,count(*) as yhmc from qxgl_yhzd where 1=1";
                dttotal = Dbservice.DB.Ado.GetDataTable(totalsql);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return new JsonResult(new { code = flag ? 0 : 200, msg = msg, count = total, data = dt, totalRow = dttotal });
        }
        public object getUserOne(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            DataTable dt = null;
            string sql = "",yhbhid="";
            try
            {
                if (context.Data.ToString().IndexOf("yhbhid") >= 0)
                {
                    yhbhid = context.Data["yhbhid"].ToString();

                }
                else
                {
                    yhbhid = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbhid;
                }

                sql = $"select * from qxgl_yhzd where yhbhid='{yhbhid}'";
                dt = Dbservice.DB.Ado.GetDataTable(sql);
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
            return new JsonResult(new { flag = flag, msg = msg, data = dt }); ;
        }
        public object saveUser(IHttpContext context)
        {
            string msg = "保存失败!", datanum = "";
            bool flag = false;
            try
            {
                var obj = Dbservice.DB.Utilities.DeserializeObject<Dictionary<string, object>>(context.Data["data"].ToString());
                var id = obj["yhbhid"].ToString();
                if (string.IsNullOrEmpty(id))
                {
                    var ret = Dbservice.DB.Insertable(obj).AS("qxgl_yhzd").ExecuteReturnIdentity();
                    if (ret > 0)
                    {
                        flag = true;
                        msg = "保存成功!";
                        datanum = ret.ToString();
                    }
                }
                else
                {
                    var ret = Dbservice.DB.Updateable(obj).AS("qxgl_yhzd").WhereColumns("yhbhid").With(SqlWith.UpdLock).ExecuteCommand();
                    if (ret > 0)
                    {
                        flag = true;
                        msg = "更新成功!";
                    }
                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, datanum = datanum }); ;
        }
        public object delYhzd(IHttpContext context)
        {
            string msg = "删除数据失败!";
            bool flag = false;
            try
            {
                var yhbhid = context.Data["yhbhid"].ToString();
                string sql = $"delete from qxgl_yhzd where yhbhid='{yhbhid}'";
                var ret = Dbservice.DB.Ado.ExecuteCommand(sql);
                if (ret > 0)
                {
                    flag = true;
                    msg = "删除数据成功!";
                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg }); ;
        }
        [Post]
        [MultiDataConvert]
        //[SkipFilter(typeof(GlobalFilter))]
        public object SaveExcel(IHttpContext context)
        {
            string msg = "保存失败!";
            bool flag = false;
            try
            {
                var files = context.Request.Files[0];
                var dict = NpoiHelper.ExcelToDictList(files.Data, files.FileName);
                if (dict != null)
                {
                    Dbservice.DB.BeginTran();
                    var delret = Dbservice.DB.Ado.ExecuteCommand("delete from qxgl_yhzd where 1=1");
                    var ret = Dbservice.DB.Insertable(dict).AS("qxgl_yhzd").IgnoreColumns("yhbhid").ExecuteCommand();

                    if (ret > 0)
                    {
                        flag = true;
                        msg = "导入成功";
                        Dbservice.DB.CommitTran();
                    }
                    else
                    {
                        Dbservice.DB.RollbackTran();
                    }

                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
                Dbservice.DB.RollbackTran();
            }
            
            return new JsonResult(new { flag = flag, msg = msg}); ;
        }
      
        [Get]
        [SkipFilter(typeof(GlobalFilter))]
        public object DownloadExcel(IHttpContext context)
        {
            try
            {
                var dt = Dbservice.DB.Ado.GetDataTable("select * from qxgl_yhzd");
                //var stream = NpoiHelper.Export(dt, false);
                var bytes = NpoiHelper.ExportByte(dt);
                //var bytes = BllPub.Instance.StreamToBytes(stream);


                return new DownLoadExcel(bytes, "用户字典");

            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
        #endregion
        #region 用户组
        public object getYhzzd(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            int total=0;
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from qxgl_yhzzd where 1=1");

            DataTable dt = null;
            try
            {
                if (context.Data.ToString().IndexOf("cxtj") > 0)
                {
                    var cxtj = BllPub.Instance.getCxtj(context.Data["cxtj"].ToString());
                    sb.Append(cxtj);
                }
               
                var yhbh = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbh;
                var page = Convert.ToInt32(context.Data["page"].ToString());
                var limit = Convert.ToInt32(context.Data["limit"].ToString());
                dt = Dbservice.DB.Queryable<object>().AS("(" + sb.ToString() + ") as t").OrderBy("yhzid").ToDataTablePage(page, limit, ref total);
                if (dt.Rows.Count > 0)
                {
                    flag=true;
                    msg = "获取数据成功!";
                }
                
            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { code = flag?0:200,count=total, msg = msg, data = dt });
        }
        public object getYhzzdAll(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("select yhzid as value,yhzmc as name from qxgl_yhzzd where 1=1  order by yhzid");

            DataTable dt = null;
            try
            {
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
        public object getYhzzdOne(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            DataTable dt = null;
            try
            {
                var yhzid = context.Data["yhzid"].ToString();
                string sql = $"select * from qxgl_yhzzd where yhzid='{yhzid}'";
                dt = Dbservice.DB.Ado.GetDataTable(sql);
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
            return new JsonResult(new { flag = flag, msg = msg, data = dt }); ;
        }
        public object delYhzzd(IHttpContext context)
        {
            string msg = "删除数据失败!";
            bool flag = false;
            try
            {
                var yhzid = context.Data["yhzid"].ToString();
                string sql = $"delete from qxgl_yhzzd where yhzid='{yhzid}'";
                var ret = Dbservice.DB.Ado.ExecuteCommand(sql);
                if (ret > 0)
                {
                    string sqldel = $" delete from qxgl_yhzqxbtn where yhzid='{yhzid}'";
                    sqldel = sqldel + $" delete from qxgl_yhzqxmenu where yhzid='{yhzid}'";
                    Dbservice.DB.Ado.ExecuteCommand(sqldel);
                    flag = true;
                    msg = "删除数据成功!";
                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg }); ;
        }
        public object saveYhzzd(IHttpContext context)
        {
            string msg = "保存失败!",datanum="";
            bool flag = false;
            try
            {
                var obj = Dbservice.DB.Utilities.DeserializeObject<Dictionary<string, object>>(context.Data["data"].ToString());
                var id = obj["yhzid"].ToString();
                if (string.IsNullOrEmpty(id))
                {
                    var ret = Dbservice.DB.Insertable(obj).AS("qxgl_yhzzd").ExecuteReturnIdentity();
                    if (ret > 0)
                    {
                        flag = true;
                        msg = "保存成功!";
                        datanum = ret.ToString();
                    }
                }
                else
                {
                    var ret = Dbservice.DB.Updateable(obj).AS("qxgl_yhzzd").WhereColumns("yhzid").With(SqlWith.UpdLock).ExecuteCommand();
                    if (ret > 0)
                    {
                        flag = true;
                        msg = "更新成功!";
                    }
                }
               
            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, datanum = datanum }); ;
        }
        public object saveYhzQx(IHttpContext context)
        {
            string msg = "保存失败!";
            bool flag = false;
            try
            {
                var yhzid = context.Data["yhzid"].ToString();
                var lx = context.Data["lx"].ToString();
                if (lx == "menu")
                {
                    var delsql = $"delete from qxgl_yhzqxmenu where yhzid='{yhzid}'";
                    var cnt=Dbservice.DB.Ado.ExecuteCommand(delsql);
                    if (cnt > 0)
                    {
                        flag = true;
                        msg = "菜单删除成功!";
                    }
                    else
                    {
                        flag = true;
                        msg = "没有任何数据!";
                    }
                    if (context.Data["datamenu"].Length > 2)
                    {
                        var obj = Dbservice.DB.Utilities.DeserializeObject<List<Dictionary<string, object>>>(context.Data["datamenu"].ToString());
                        for (int i = 0; i < obj.Count; i++)
                        {
                            obj[i]["yhzid"] = yhzid;
                        }
                        if (obj.Count > 0)
                        {
                            var ret = Dbservice.DB.Insertable(obj).AS("qxgl_yhzqxmenu").IgnoreColumns("zybh","title").ExecuteCommand();
                            if (ret > 0)
                            {
                                flag = true;
                                msg = "菜单保存成功!";
                            }
                        }
                        else
                        {
                            flag = true;
                            msg = "菜单保存成功!";
                        }
                    }
                    
                    

                }
                if (lx == "btn")
                {
                    var menuid = context.Data["menuid"].ToString();
                    var delsql = $"delete from qxgl_yhzqxbtn where yhzid='{yhzid}' and menuid='{menuid}'";
                    var cnt = Dbservice.DB.Ado.ExecuteCommand(delsql);
                    if (cnt > 0)
                    {
                        flag = true;
                        msg = "按钮删除成功!";
                    }
                    else
                    {
                        flag = true;
                        msg = "没有任何数据!";
                    }
                    if (context.Data["databtn"].Length > 2)
                    {
                        var obj = Dbservice.DB.Utilities.DeserializeObject<List<Dictionary<string, object>>>(context.Data["databtn"].ToString());
                        for (int i = 0; i < obj.Count; i++)
                        {
                            obj[i]["yhzid"] = yhzid;
                            obj[i]["menuid"] = menuid;
                        }
                        if (obj.Count > 0)
                        {
                            var ret = Dbservice.DB.Insertable(obj).AS("qxgl_yhzqxbtn").IgnoreColumns("btnname").ExecuteCommand();
                            if (ret > 0)
                            {
                                flag = true;
                                msg = "按钮保存成功!";
                            }
                        }
                        else
                        {
                            flag = true;
                            msg = "按钮保存成功!";
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg }); ;
        }
        #endregion
        #region 按钮
        public string getBtn(IHttpContext context)
        {
            string btnhtml = "";
            try
            {
                var yhzidstr = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhzid;
                var yhzid = "'" + yhzidstr.Replace(",", "','") + "'";
                var menumc = context.Data["menumc"].ToString();
                string menuidsql = $"select menuid from qxgl_menu where title='{menumc}'";
                var menuid = Dbservice.DB.Ado.GetString(menuidsql);
                if (!string.IsNullOrEmpty(menuid))
                {
                    string sql = $"select distinct t.btnid,t.btnhtml from qxgl_button t left join qxgl_yhzqxbtn a on t.btnid=a.btnid where a.yhzid in ({yhzid}) and menuid='{menuid}' order by t.btnid";
                    var dt = Dbservice.DB.Ado.GetDataTable(sql);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        btnhtml = btnhtml + dt.Rows[i]["btnhtml"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return btnhtml;
        }
        public object getBtnMenu(IHttpContext context)
        {
            string msg = "获取数据失败！";
            bool flag = false;
            
            DataTable dt = null;
            try
            {
                var yhzid = context.Data["yhzid"].ToString();
                var menuid = context.Data["menuid"].ToString();
                StringBuilder sb = new StringBuilder();
                sb.Append(" select btnid,btnname ");
                sb.Append($" ,LAY_CHECKED=(select 'true' from qxgl_yhzqxbtn where btnid=t.btnid and yhzid='{yhzid}' and menuid='{menuid}' )");
                sb.Append(" from qxgl_button t where 1=1 ");
                var yhbh = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbh;
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
        #region 条件查询
        public object getCxtjHtml(IHttpContext context)
        {
            string cxtjhtml = "",cxtjdata="";

            try
            {
                var menumc = context.Data["menumc"].ToString();
                var menuid = Dbservice.DB.Ado.GetString($"select menuid from qxgl_menu where title='{menumc}'");
                if (!string.IsNullOrEmpty(menuid))
                {
                    string sql = $"select * from qxgl_cxtj t left join qxgl_cxtjqx a on t.cxtjid=a.cxtjid where a.menuid='{menuid}' order by t.cxtjid";
                    var dt = Dbservice.DB.Ado.GetDataTable(sql);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var cxtjmc = dt.Rows[i]["cxtjmc"].ToString();
                        var cxtjcolumn= dt.Rows[i]["cxtjcolumn"].ToString();
                        cxtjhtml = cxtjhtml + dt.Rows[i]["cxtjhtml"].ToString().Replace("cxtjmc",cxtjmc).Replace("cxtjcolumn",cxtjcolumn);
                    }
                    var yhbhid = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbhid;
                    cxtjdata = Dbservice.DB.Ado.GetString($"select cxtj from qxgl_cxsave where yhbhid='{yhbhid}' and menuid='{menuid}'");
                }
            }
            catch (Exception ex)
            {

            }
            return new JsonResult(new { cxtjhtml = cxtjhtml ,cxtjdata= cxtjdata });
        }
        public object getCxtjQx(IHttpContext context)
        {
            string msg = "获取数据失败！";
            bool flag = false;

            DataTable dt = null;
            try
            {
                var menuid = context.Data["menuid"].ToString();
                StringBuilder sb = new StringBuilder();
                sb.Append(" select cxtjid,cxtjmc,cxtjcolumn ");
                sb.Append($" ,LAY_CHECKED=(select 'true' from qxgl_cxtjqx where cxtjid=t.cxtjid and menuid='{menuid}' )");
                sb.Append(" from qxgl_cxtj t where 1=1 ");
                var yhbh = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbh;
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
        public object saveCxtjQx(IHttpContext context)
        {
            string msg = "保存失败!";
            bool flag = false;
            try
            {
                var menuid = context.Data["menuid"].ToString();

                var delsql = $"delete from qxgl_cxtjqx where menuid='{menuid}'";
                var cnt = Dbservice.DB.Ado.ExecuteCommand(delsql);
                if (cnt > 0)
                {
                    flag = true;
                    msg = "查询权限删除成功!";
                }
                else
                {
                    flag = true;
                    msg = "没有任何数据!";
                }
                if (context.Data["datacxtj"].Length > 2)
                {
                    var obj = Dbservice.DB.Utilities.DeserializeObject<List<Dictionary<string, object>>>(context.Data["datacxtj"].ToString());
                    for (int i = 0; i < obj.Count; i++)
                    {
                        obj[i]["menuid"] = menuid;
                    }
                    if (obj.Count > 0)
                    {
                        var ret = Dbservice.DB.Insertable(obj).AS("qxgl_cxtjqx").IgnoreColumns("cxtjmc","cxtjcolumn").ExecuteCommand();
                        if (ret > 0)
                        {
                            flag = true;
                            msg = "查询条件权限保存成功!";
                        }
                    }
                    else
                    {
                        flag = true;
                        msg = "查询条件权限保存成功!";
                    }
                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg }); ;
        }

        public object getCxtjAll(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            int total = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from qxgl_cxtj where 1=1 ");

            DataTable dt = null;
            try
            {
                if (context.Data.ToString().IndexOf("cxtj") > 0)
                {
                    var cxtj = BllPub.Instance.getCxtj(context.Data["cxtj"].ToString());
                    sb.Append(cxtj);
                }

                var yhbh = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbh;
                var page = Convert.ToInt32(context.Data["page"].ToString());
                var limit = Convert.ToInt32(context.Data["limit"].ToString());
                dt = Dbservice.DB.Queryable<object>().AS("(" + sb.ToString() + ") as t").ToDataTablePage(page, limit, ref total);
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
            return new JsonResult(new { code = flag ? 0 : 200, count = total, msg = msg, data = dt });
        }
        public object getCxtjOne(IHttpContext context)
        {
            string msg = "获取数据失败!";
            bool flag = false;
            DataTable dt = null;
            try
            {
                var cxtjid = context.Data["cxtjid"].ToString();
                string sql = $"select * from qxgl_cxtj where cxtjid='{cxtjid}'";
                dt = Dbservice.DB.Ado.GetDataTable(sql);
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
            return new JsonResult(new { flag = flag, msg = msg, data = dt }); ;
        }
        public object delCxtj(IHttpContext context)
        {
            string msg = "删除数据失败!";
            bool flag = false;
            try
            {
                var cxtjid = context.Data["cxtjid"].ToString();
                string sql = $"delete from qxgl_cxtj where cxtjid='{cxtjid}'";
                var ret = Dbservice.DB.Ado.ExecuteCommand(sql);
                if (ret > 0)
                {
                    flag = true;
                    msg = "删除数据成功!";
                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg }); ;
        }
        public object saveCxtj(IHttpContext context)
        {
            string msg = "保存失败!", datanum = "";
            bool flag = false;
            try
            {
                var obj = Dbservice.DB.Utilities.DeserializeObject<Dictionary<string, object>>(context.Data["data"].ToString());
                var id = obj["cxtjid"].ToString();
                if (string.IsNullOrEmpty(id))
                {
                    var ret = Dbservice.DB.Insertable(obj).AS("qxgl_cxtj").ExecuteReturnIdentity();
                    if (ret > 0)
                    {
                        flag = true;
                        msg = "保存成功!";
                        datanum = ret.ToString();
                    }
                }
                else
                {
                    var ret = Dbservice.DB.Updateable(obj).AS("qxgl_cxtj").WhereColumns("cxtjid").With(SqlWith.UpdLock).ExecuteCommand();
                    if (ret > 0)
                    {
                        flag = true;
                        msg = "更新成功!";
                    }
                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, datanum = datanum }); ;
        }
        public object saveCx(IHttpContext context)
        {
            string msg = "保存失败!";
            bool flag = false;
            try
            {
                //var obj = Dbservice.DB.Utilities.DeserializeObject<Dictionary<string, object>>(context.Data["cxtj"].ToString());
                var obj = new Dictionary<string, object>();

                var menumc = context.Data["menumc"].ToString();
                var menuid = Dbservice.DB.Ado.GetString($"select menuid from qxgl_menu where title='{menumc}'");
                var yhbhid = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbhid;
                obj["menuid"] = menuid;
                obj["yhbhid"] = yhbhid;
                obj["cxtj"] = context.Data["cxtj"].ToString();
                Dbservice.DB.Ado.ExecuteCommand($"delete from qxgl_cxsave where yhbhid='{yhbhid}' and menuid='{menuid}'");
                if (!string.IsNullOrEmpty(menuid))
                {
                    var ret = Dbservice.DB.Insertable(obj).AS("qxgl_cxsave").ExecuteCommand();
                    if (ret > 0)
                    {
                        flag = true;
                        msg = "保存成功!";

                    }
                }
                
            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg }); ;
        }
        #endregion
    }
}