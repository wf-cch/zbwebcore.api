using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeetleX.FastHttpApi;
using BeetleX.FastHttpApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using zbwebcore.API.BllDal;
using zbwebcore.API.Config;
using zbwebcore.API.Model;


namespace zbwebcore.API.Controllers
{

    [Options(AllowOrigin = "*",AllowHeaders = "*,token")]
    [Controller(BaseUrl = "fhp/")]


    public class FastHttpApiTestController
    {

        //RedisDB rdb = new RedisDB(ConfigInfo.Info.dbNum, new JsonFormater());
        public object getSpxsls(IHttpContext context)
        {
            bool flag = false;
            string msg = "无符合条件的数据!";
            DataTable dt = null;
            try
            {
                var yhbh = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbh;
                var page = Convert.ToInt32(context.Data["page"]);
                var limit = Convert.ToInt32(context.Data["limit"]);
                StringBuilder sb = new StringBuilder();
                sb.Append(" select top 1000 * from spxslsrb t where 1=1");
                if (context.Data.ToString().IndexOf("cxtj") > 0)
                {
                    var cxtj = BllPub.Instance.getCxtj(context.Data["cxtj"].ToString());
                    cxtj = cxtj.Replace("rq", "xsrq").Replace("-","");
                    sb.Append(cxtj);
                }
                dt = Dbservice.DBFD.Ado.GetDataTable(sb.ToString());
                if (dt.Rows.Count > 0)
                {
                    flag = true;
                    msg = "获取成功!";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return new JsonResult(new { code = flag ? 0 : 200, msg = msg, data = dt });
        }
        [Post]
        public object test1(string name)
        {
            string msg = "error";
            bool flag = false;
            string dtime="";
            DataTable dt=null;
            string dbconn = "";
            try
            {
                dbconn = AppConfigurtaionServices.Configuration["connectionString"]; 
                dtime = Dbservice.DB.GetDate().ToString();
                dt = Dbservice.DB.Ado.GetDataTable("select * from jczl_fdzd");
                msg = "ok";
                flag = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            //return new { msg=msg,flag=flag, dbconn= dbconn, Hello = "我叫 " + name, InSchoolTime = new DateTime(1997, 9, 1, 10, 0, 0),dtime=dtime,dt=dt };
            return new JsonResult(new { flag = flag, dt = dt, msg = msg });
            //return (new { flag = flag, dt = dt });
            //return (new { msg=msg});

        }
        [SkipFilter]
        public object ApiTest(string json)
        {
            string msg = "";
            bool flag = false;
            try
            {
                flag = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                
            }
            
            return new JsonResult(new { flag = flag, ret = json, msg = msg });
        }
        [Post]
        public object getFdzdSelect()
        {
            DataTable dt = null;
            try
            {
                dt = Dbservice.DB.Ado.GetDataTable("select fdbh as value,fdbh+'-'+fdmc as name from jczl_fdzd");
                
            }
            catch (Exception)
            {

            }
            return new JsonResult(new { code = new { ok = "0" }, dt = dt });
        }
        public object getFdzdOne(string fdbh)
        {
            DataTable dt = null;
            try
            {
                dt = Dbservice.DB.Ado.GetDataTable($"select fdbh as value,fdbh+'-'+fdmc as name from jczl_fdzd where fdbh='{fdbh}'");

            }
            catch (Exception)
            {

            }
            return new JsonResult(new {code ="0", dt = dt });
        }
        public object test2(string name)
        {
            string msg = "error";
            bool flag = false;
            DataTable dt = null;
           
                var date = Dbservice.DBFD.GetDate();
                dt = Dbservice.DBFD.Ado.GetDataTable("select * from xlzd");
                msg = "ok";
                flag = true;
          
            return new { msg = msg, flag = flag, Hello = "我叫 " + name, InSchoolTime = new DateTime(1997, 9, 1, 10, 0, 0),dt=dt };

        }
        public void testerr()
        {
            throw new Exception("hello");
        }
        
        //string dbip = AppConfigurtaionServices.Configuration["redis:ip"];
        //static int dbNum = Convert.ToInt32(AppConfigurtaionServices.Configuration["redis:dbnum"]);
        //int timeout = Convert.ToInt32(AppConfigurtaionServices.Configuration["redis:timeout"]);
        
        public object redisTest()
        {
            bool flag = false;
            string msg = "测试失败!";
            UserInfo info = new UserInfo();
            try
            {
             
                //rdb.Host.AddWriteHost(ConfigInfo.Info.redisIp);
                
                info.yhbh = "wf_cch";
                info.yhmc = "cch";
                info.yhnc = "海阔天空";
                info.yhkl = "123456";
                info.appid = "testid";
                //rdb.Set(info.appid, info,ConfigInfo.Info.timeout);
                flag = true;
                msg = "成功!";
            }
            catch (Exception ex)
            {
                msg =msg+ ex.Message;
                
            }
            return new JsonResult(new { flag = flag, msg = msg });
        }
       
        //[SkipFilter(typeof(JWTFilter))]
        //public object gettoken(string name,string role,IHttpContext context)
        //{
        //    var token = JWTHelper.Instance.CreateToken(name,role);
        //    context.Response.Header.Add(HeaderTypeFactory.AUTHORIZATION, token);
        //    return new TextResult(token);
        //}
        #region MENU 
        /// <summary>
        /// 获取类别列表
        /// </summary>
        /// <param name="parent_id">父ID</param>
        /// <param name="nav_type">导航类别</param>
        /// <returns>DataTable</returns>
        public DataTable GetList(string parent_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM qx_yhb");
            strSql.Append($" where  left(zybh,2)='{parent_id}'");
            strSql.Append(" order by  zybh asc ");
            DataSet ds = null;// DbHelperSQL.Query(strSql.ToString());
            //重组列表
            DataTable oldData = ds.Tables[0] as DataTable;
            if (oldData == null)
            {
                return null;
            }
            //创建一个新的DataTable增加一个深度字段
            DataTable newData = new DataTable();
            newData.Columns.Add("KeyId", typeof(int));
            newData.Columns.Add("PID", typeof(int));
            newData.Columns.Add("DeptId", typeof(int));
            newData.Columns.Add("Deptlevel", typeof(int));
            newData.Columns.Add("Code", typeof(string));
            newData.Columns.Add("Name", typeof(string));
            newData.Columns.Add("note", typeof(string));
            //调用迭代组合成DAGATABLE
            GetChilds(oldData, newData, parent_id, 0);
            return newData;
        }


        /// <summary>
        /// 从内存中取得所有下级类别列表（自身迭代）
        /// </summary>
        private void GetChilds(DataTable oldData, DataTable newData, string parent_id, int class_layer)
        {
            class_layer++;
            DataRow[] dr = oldData.Select("PID=" + parent_id);
            for (int i = 0; i < dr.Length; i++)
            {
                //添加一行数据
                DataRow row = newData.NewRow();
                row["KeyId"] = int.Parse(dr[i]["KeyId"].ToString());
                row["PID"] = int.Parse(dr[i]["PID"].ToString());
                row["DeptId"] = int.Parse(dr[i]["DeptId"].ToString());
                row["Deptlevel"] = int.Parse(dr[i]["Deptlevel"].ToString());
                row["Code"] = dr[i]["Code"].ToString();
                row["Name"] = dr[i]["Name"].ToString();
                row["note"] = dr[i]["note"].ToString();

                newData.Rows.Add(row);
                //调用自身迭代
                this.GetChilds(oldData, newData, dr[i]["KeyId"].ToString(), class_layer);
            }
        }




        #endregion
        #region MENU2
        //[SkipFilter(typeof(JWTFilter))]
        public object initTree()
        {
            //DataSet ds = Dal.ExcSql.GetDataSet("select t.id,t.name text,t.parentid from jsjd_type t where t.istop=0");//先查出节点的所有数据
            //ViewState["ds"] = ds;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM qx_zyb");
            strSql.Append($" where  left(zybh,2)='01'");
            strSql.Append(" order by  zybh asc ");
            var dt = Dbservice.DBFD.Ado.GetDataTable(strSql.ToString());
            var jsonstr=AddNode(dt,"01", 1);
            return new { menu = jsonstr };
            //Response.End();
        }
        /// <summary>
        /// 无限递归，生成easyui tree可用的json数据 wyw308
        /// </summary>
        /// <param name="ParentID">父id</param>
        /// <param name="deep">开始深度，默认为1</param>
        /// <returns></returns>
        //[SkipFilter(typeof(JWTFilter))]
        private string AddNode(DataTable dt, string ParentID, int deep)
        {
            string str = "";
            //DataSet ds = (DataSet)this.ViewState["ds"];

            DataView dvTree = new DataView(dt);
            dvTree.RowFilter = "zybh   ='" + ParentID + "'";//过滤出ParentId

            int i = 0;
            int d = deep;
            foreach (DataRowView drv in dvTree)
            {

                if (i == 0)//如果是某一层的开始，需要“[”开始
                {
                    if (d == 1)//如果深度为1,即第一层
                        str = "[";
                    else//否则，为第二层或更深
                        str = ",\"children\":[";
                }

                else
                    str = str + ",";



                str = str + "{";
                str = str + "\"zybh\":\"" + drv["zybh"] + "\",";
                str = str + "\"zymc\":\"" + drv["zymc"] + "\"";
                str = str + "\"url\":\"" + drv["linkurl"] + "\"";

                str = str + AddNode(dt,drv["zybh"].ToString(), deep + 1);//递归

                str = str + "}";
                i = i + 1;

                if (dvTree.Count == i)//如果某一层到了末尾,需要"]"结束
                    str = str + "]";

            }

            return str;
        }

        #endregion
        #region MENU3
        public object getmenu()
        {
            string jsonstr = "";
            string childsstr = "";
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM qx_zyb");
            strSql.Append($" where  left(zybh,2)='01'");
            strSql.Append(" order by  zybh asc ");
            var dt = Dbservice.DBFD.Ado.GetDataTable(strSql.ToString());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string zybh1 = dt.Rows[i]["zybh"].ToString();
                string zybh2 = "";
                if (i > 0)
                {
                    zybh2 = dt.Rows[i - 1]["zybh"].ToString();
                    if (zybh1.Length == zybh2.Length)
                    {
                        jsonstr += "\"title\": \"" + dt.Rows[i]["zymc"].ToString() + "\",";
                    }
                    else
                    {
                        childsstr += "\"title\": \"" + dt.Rows[i]["zymc"].ToString() + "\",";
                    }
                    
                }
                    

            }
            return new { menu = jsonstr };
        }
        #endregion
        #region MENU4
        [Post]
        //[JsonDataConvert]
        public object GetMenuJson()
        {
            //Database db = DatabaseFactory.CreateDatabase();
            //string sql = @"SELECT PageId
            //              ,pageName
            //              ,cregex
            //              ,parentId
            //              ,dregex FROM LogFile_Page ";
            //DbCommand mycmd = db.GetSqlStringCommand(sql);
            //DataSet ds = db.ExecuteDataSet(mycmd);
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM qx_zyb");
            strSql.Append($" where  left(zybh,2)='01'");
            strSql.Append(" order by  zybh asc ");
            var dt = Dbservice.DBFD.Ado.GetDataTable(strSql.ToString());
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                s.Append(GetChildNodes(dt, "00"));
                s = s.Remove(s.Length - 2, 2);

            }
            //return new  {flag=true,msg="ok",data= s.ToString() };
            return new TextResult ("{\"code\":0,\"msg\":\"ok\",\"data:\"" + s.ToString()+"}" );
            //return new TextResult("\"data:\"" + s.ToString() + "}");
        }
        public object GetMenuJson1()
        {
            //Database db = DatabaseFactory.CreateDatabase();
            //string sql = @"SELECT PageId
            //              ,pageName
            //              ,cregex
            //              ,parentId
            //              ,dregex FROM LogFile_Page ";
            //DbCommand mycmd = db.GetSqlStringCommand(sql);
            //DataSet ds = db.ExecuteDataSet(mycmd);
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM qx_zyb");
            strSql.Append($" where  left(zybh,2)='01'");
            strSql.Append(" order by  zybh asc ");
            var dt = Dbservice.DBFD.Ado.GetDataTable(strSql.ToString());
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                s.Append(GetChildNodes(dt, "00"));
                s = s.Remove(s.Length - 2, 2);

            }
            //return new { flag = true, msg = "ok", data = s.ToString() };
            return new TextResult ("{\"code\":0,\"msg\":\"ok\",\"data:\"" + s.ToString()+"}" );
            //return new TextResult("\"data:\"" + s.ToString() + "}");
        }
        public string GetChildNodes(DataTable dt, string parentid)
        {
            StringBuilder stringbuilder = new StringBuilder();
            DataRow[] CRow = dt.Select("fcode=" + parentid);
            if (CRow.Length > 0)
            {
                stringbuilder.Append("[");
                for (int i = 0; i < CRow.Length; i++)
                {
                    string chidstring = GetChildNodes(dt, CRow[i]["zybh"].ToString());
                    if (!string.IsNullOrEmpty(chidstring))
                    {
                        stringbuilder.Append("{ \"title\":\"" + CRow[i]["zymc"].ToString() + "\",\"href\":\"" + CRow[i]["linkurl"].ToString() + "\",\"childs\":");
                        stringbuilder.Append(chidstring);
                    }
                    else
                    {
                        stringbuilder.Append("{\"title\":\"" + CRow[i]["zymc"].ToString() + "\",\"href\":\"" + CRow[i]["linkurl"].ToString() + "\"},");
                    }
                }
                stringbuilder.Replace(',', ' ', stringbuilder.Length - 1, 1);
                stringbuilder.Append("]},");
            }
            return stringbuilder.ToString();
        }
        #endregion
        #region MENU5

        [Post]
        //[JsonDataConvert]
        [FormUrlDataConvert]
        public object getmenu5(string a1,string a2, IHttpContext context)
        {
            string msg = "error";
            List<TreeModel> list = null;
            try
            {
                Console.WriteLine(context.Data);
                list = GetTreeList("0");
                msg = a1 + a2;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return new JsonResult(new { list = list, msg = msg });
        }
        [Post]
        //[JsonDataConvert]
        [FormUrlDataConvert]
        public object getmenu6(string a1,string a2,IHttpContext context)
        {
            string msg = "error";
            List<TreeModel> list = null;
            try
            {
                var b1 = context.Data["a1"];
                var b2 = context.Data["a2"];
                var obj = Dbservice.DB.Utilities.DeserializeObject<Dictionary<string, object>>(context.Data["body"].ToString());
                Console.WriteLine(context.Data);
                list = GetTreeList("0");
                msg = a1 + a2;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return new JsonResult(new { list = list, msg = msg });
        }

        public object getmenu7(IHttpContext context)
        {
            string msg = "error";
            int code = -1;
            List<TreeModel> list = null;
            try
            {
                list = GetTreeList("00");
                code = 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return new JsonResult(new { code=code,msg = msg,data = list  });
        }
        public object getmenu8(IHttpContext context)
        {
            string msg = "error";
            int code = -1;
            string sql = "select zybh,zymc as title,fcode,linkurl as href,iconcls as icon,sno as id from qx_zyb where isvisible='1' order by zybh";
            List<qxgl_menu> list = null;
            List<qxgl_menu> listnew = null;
            try
            {
                list = Dbservice.DB.Ado.SqlQuery<qxgl_menu>(sql);
                listnew = GetTreeListDb(list,"00");

                code = 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return new JsonResult(new { code = code, msg = msg, data = listnew });
        }
        public List<qxgl_menu> GetChildMenuListDb(List<qxgl_menu> list,string fcode)
        {
            var result = list.Where(x => x.fcode == fcode);
            return result.ToList();
        }
        public List<qxgl_menu> GetTreeListDb(List<qxgl_menu> list,string fcode)
        {
            List<qxgl_menu> TreeList = new List<qxgl_menu>();
            List<qxgl_menu> ModelList = GetChildMenuListDb(list, fcode);
            foreach (var item in ModelList)
            {
                qxgl_menu m = new qxgl_menu();
                m.id = string.IsNullOrEmpty(item.id) ? "" : item.id.ToString(); 
                m.title = string.IsNullOrEmpty(item.title) ? "" : item.title.ToString();
                m.href = string.IsNullOrEmpty(item.href) ? "" : item.href.ToString(); 
                m.icon = string.IsNullOrEmpty(item.icon)?"":item.icon.ToString();
                m.childs = GetTreeListDb(list,item.zybh);
                TreeList.Add(m);
            }
            return TreeList;
        }
        public List<TreeModel> GetTreeList(string ParentID)
        {
            List<TreeModel> TreeList = new List<TreeModel>();
            List<MenuModel> ModelList = GetChildMenuList(ParentID);
            foreach (var item in ModelList)
            {
                TreeModel m = new TreeModel();
                m.id = item.ID.ToString();
                m.title = item.MenuName.ToString();
                m.childs = GetTreeList(item.ID);
                TreeList.Add(m);
            }
            return TreeList;
        }
        /// <summary>
        /// 获取所有CD数据
        /// </summary>
        /// <returns></returns>
        public List<MenuModel> GetAllMenuList()
        {
            List<MenuModel> list = new List<MenuModel>();
            list.Add(new MenuModel { ID = "01", MenuName = "CD1", ParentID = "00" });
            list.Add(new MenuModel { ID = "0101", MenuName = "CD1.1", ParentID = "01" });
            list.Add(new MenuModel { ID = "0102", MenuName = "CD1.2", ParentID = "01" });
            list.Add(new MenuModel { ID = "010101", MenuName = "CD1.1.1", ParentID = "0101" });
            list.Add(new MenuModel { ID = "02", MenuName = "CD2", ParentID = "00" });
            return list;
        }
        /// <summary>
        /// 根据父节点获取子节点
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public List<MenuModel> GetChildMenuList(string ParentID)
        {
            List<MenuModel> list = GetAllMenuList();
            var result = list.Where(x => x.ParentID == ParentID);
            return result.ToList();
        }
        
        #endregion
    }

}