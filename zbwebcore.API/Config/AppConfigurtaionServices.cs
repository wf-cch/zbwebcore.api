using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zbwebcore.API.Config
{
    public class AppConfigurtaionServices
    {
        public static IConfiguration Configuration { get; set; }
        static AppConfigurtaionServices()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载            
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
        }
        //读取配置文件的代码完成了，只要引用了NetCoreOrder.Common类库的项目中都能方便读取数据库链接字符串和其他配置，使用方法如下：

        //AppConfigurtaionServices.Configuration.GetConnectionString("CxyOrder"); 
        ////得到 Server=LAPTOP-AQUL6MDE\\MSSQLSERVERS;Database=CxyOrder;User ID=sa;Password=123456;Trusted_Connection=False;

        //读取一级配置节点配置

        //AppConfigurtaionServices.Configuration["ServiceUrl"];

        //读取二级子节点配置

        //AppConfigurtaionServices.Configuration["Appsettings:SystemName"];
        ////得到 PDF .NET CORE
        //AppConfigurtaionServices.Configuration["Appsettings:Author"];
        ////得到 PDF
    }
}
