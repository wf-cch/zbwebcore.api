using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeetleX.FastHttpApi;
using zbwebcore.API.Model;
using SqlSugar;
using zbwebcore.API.Config;


namespace zbwebcore.API.Controllers
{
    [Options(AllowOrigin = "*", AllowHeaders = "*,token")]
    [Controller(BaseUrl = "login/")]
    public class LoginController
    {
        #region 登录及验证
        [SkipFilter(typeof(GlobalFilter))]
        [Post]
        public object IsLogin(string yhbh, string yhkl)
        {
            bool flag = false;
            string msg = "登录失败!";
            string token = "";
            UserInfo info = null;
            int ok = 1;
            try
            {
                string sql = $"select * from qxgl_yhzd where yhbh='{yhbh}'";
                info = Dbservice.DB.Ado.SqlQuerySingle<UserInfo>(sql);
                if (info != null && !string.IsNullOrEmpty(info.yhbh))
                {
                    //var yhklmd5 = Dbservice.GetMD5(yhkl);
                    var yhklmd5 = Utils.MD5Encrypt(yhkl);
                    if (yhklmd5 == info.yhkl)
                    {
                        token = Guid.NewGuid().ToString("N").ToUpper();
                        info.token = token;
                        info.appid = yhbh;
                        info.logintime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                        RedisHelper.Instance.Set(info.token, info, ConfigInfo.Info.timeout);
                        flag = true;
                        ok = 0;
                        msg = "登录成功!";
                    }
                    else
                    {
                        msg = "密码不正确!";
                    }
                }
                else
                {
                    msg = "无此用户!";
                }

            }
            catch (Exception ex)
            {
                msg = msg+ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg,code=new {ok=ok }, data =new {token=token,yhbh=(info==null?"":info.yhbh),yhmc = (info == null ? "" : info.yhmc) } });
        }
        public object IsLoginTimeout(IHttpContext context)
        {
            bool flag = false;
            string msg = "";
            string token = context.Request.Header["token"];
            try
            {
                if (!RedisHelper.Exists(token))
                {
                    flag = true;
                    msg = "redis中的token过期了!";
                }
                else
                {
                    RedisHelper.Expire(token, ConfigInfo.Info.timeout);
                }
            }
            catch (Exception ex)
            {
                flag = true;
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg });
        }
       
        #endregion

    }
}