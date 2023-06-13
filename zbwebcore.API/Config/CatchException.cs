using BeetleX.FastHttpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zbwebcore.API.Config
{
    public class CatchException : FilterAttribute
    {
       
        public override void Executed(ActionContext context)
        {
            //var token=context.HttpContext.Response.Header["token"];
            
            base.Executed(context);
            if (context.Exception != null)
            {
                context.Result = new TextResult(context.Exception.Message+"测试cch");
                context.Exception = null;
            }
        }
    }
}
