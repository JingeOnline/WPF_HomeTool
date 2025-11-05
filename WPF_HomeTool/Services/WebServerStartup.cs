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

        public void Configure(IApplicationBuilder app)
        {
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
        public string Get() => "Hello World!";
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