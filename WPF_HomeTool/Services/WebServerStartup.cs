using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Services
{
    //参考文档：https://blog.csdn.net/scixing/article/details/143103916
    public class WebServerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStopping.Register(() => {
                //当程序退出时，会执行这里的回调方法
            });
            app.UseRouting();
            app.UseEndpoints(routes =>
            {
                routes.MapControllers();
            });
        }
    }

    public class HomeController()
    {
        [HttpGet("/")]
        public string Get() => "This is the root url of api.";
    }

    [Route("pc-command")]
    public class PcCommandController()
    {
        [HttpGet]  //url无参情况下
        public string Get() => "pc-command";

        [HttpGet("{command}")]  //url有参情况下
        public string Get(string command)
        {
            return command;
        }
    }
}