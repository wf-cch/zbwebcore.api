using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeetleX.FastHttpApi;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using zbwebcore.API.Config;
using Microsoft.Extensions.Hosting;
using zbwebcore.API.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;

namespace zbwebcore.API
{
    public class Program
    {
        //public static void Main(string[] args)
        //{
        //    CreateWebHostBuilder(args).Build().Run();
        //}
        //private static BeetleX.FastHttpApi.HttpApiServer mApiServer;
        static void Main(string[] args)
        {
            RedisHelper.Initialization(new CSRedis.CSRedisClient(ConfigInfo.Info.csredis));
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<HttpServerHosted>();

                });
            builder.Build().Run();
        }
        public class HttpServerHosted : IHostedService
        {
            private HttpApiServer mApiServer;

            public virtual Task StartAsync(CancellationToken cancellationToken)
            {
                mApiServer = new HttpApiServer();
                mApiServer.Options.LogLevel = BeetleX.EventArgs.LogType.Trace;
                mApiServer.Options.LogToConsole = true;
                mApiServer.Options.AutoGzip = true; //大于2k的json数据自动压缩返回
                //mApiServer.Debug();//set view path with vs project folder
                mApiServer.Options.AddFilter<GlobalFilter>();
                //mApiServer.Options.AddFilter<CatchException>();
                //mApiServer.Options.AddFilter<JWTFilter>();
                mApiServer.Register(typeof(Program).Assembly);
                //mApiServer.Options.Port=80; set listen port to 80
                mApiServer.Open();//default listen port 9090  
                Console.Write(mApiServer.BaseServer);
                Console.Read();
                //普通模式
                
                return Task.CompletedTask;
            }

            public virtual Task StopAsync(CancellationToken cancellationToken)
            {
                mApiServer.Dispose();
                return Task.CompletedTask;
            }
        }
        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>();
    }
}
