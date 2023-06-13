using BeetleX.FastHttpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zbwebcore.API.Config
{
    public class JWTFilter : FilterAttribute
    {
        public override bool Executing(ActionContext context)
        {
            string token = context.HttpContext.Request.Header[HeaderTypeFactory.AUTHORIZATION];
            var user = JWTHelper.Instance.GetUserInfo(token);
            if (!string.IsNullOrEmpty(user.Name))
            {
                return true;
            }
            else
            {
                context.Result = new TextResult("token not found");
                return false;
            }
        }
    }
}
