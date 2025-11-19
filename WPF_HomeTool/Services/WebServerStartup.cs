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
            if (command == "shutdown")
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                //重定向输入、输出、异常信息
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.StandardInput.WriteLine("shutdown /s /t 0");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                string result = cmd.StandardOutput.ReadToEnd();
                return result;
            }
            else if (command == "sleep")
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                //重定向输入、输出、异常信息
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                //需要在电源选项中关闭“混合睡眠”才能使该命令生效，否则会变成休眠，可以通过在windows系统中执行命令“powercfg -h off”关闭休眠功能
                cmd.StandardInput.WriteLine("rundll32.exe powrprof.dll,SetSuspendState 0,1,0");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                string result = cmd.StandardOutput.ReadToEnd();
                return result;
            }
            else
            {
                return command + " 命令没有对应指令";
            }

            //return command;
        }
    }
}