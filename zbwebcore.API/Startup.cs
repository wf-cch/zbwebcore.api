using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using zbwebcore.API.Config;
using zbwebcore.API.Model;

namespace zbwebcore.API
{
    public class Startup
    {
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;

        //}

        //public IConfiguration Configuration { get; }

        //// This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        //{
        //     services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        //    //读取appsettings.json文件中的MongoDbSettings节点数据，赋值给MongoDbSettings对象
        //    //services.Configure<DataBaseSetting>(
        //    //    Configuration.GetSection(nameof(DataBaseSetting)));

        //    ////注入MongoDbSettings对象
        //    //services.AddSingleton<IDataBaseSetting>(sp =>
        //    //    sp.GetRequiredService<IOptions<DataBaseSetting>>().Value);

        //    //////注入上下文对象
        //    //services.AddScoped<MongodbHelper>();
        //}

        ////This method gets called by the runtime.Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }

        //    app.UseMvc();
        //}

    }
}
