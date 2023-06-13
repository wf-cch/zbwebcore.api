
using BeetleX.FastHttpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zbwebcore.API.BllDal;
using zbwebcore.API.Model;

namespace zbwebcore.API.Config
{
    public class GlobalFilter : FilterAttribute
    {
        
        public override bool Executing(ActionContext context)
        {
            bool flag = false;
            string msg = "";
            int code = 0;
            try
            {
                msg = "token不存在或已过期!";
                string token = context.HttpContext.Request.Header["token"];
                if (RedisHelper.Instance.Exists(token))
                {
                    flag = true;
                    msg = "token正常!";
                    RedisHelper.Instance.Expire(token, ConfigInfo.Info.timeout);

                    if (BllPub.Instance.SqlFilter(context.HttpContext.Data.ToString()))
                    {
                        flag = false;
                        msg = "提交的查询数据存在异常关键字!";
                        code = 200;
                    }
                }
                else
                {
                    code = 403;
                }
                
            }
            catch (Exception ex)
            {
                code = 403;
                msg = ex.Message;
            }

            context.Result = new JsonResult(new { code = code,flag=flag, msg = msg });
            //if (!flag)
            //{
            //    //var result = new ActionResult(403, msg);
            //    //result.Data = "";
            //    context.Result = new JsonResult(new { code = 403, msg = msg }); 
            //}
            return flag;

        }
        public override void Executed(ActionContext context)
        {
            base.Executed(context);
            Console.WriteLine($"{DateTime.Now} {context.HttpContext.Request.Url} globalFilter executed");
        }

    }
}
