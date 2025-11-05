using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/", () => Results.Ok());
app.MapGet("/pc-command/{command}", (string command) => {
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
});

app.Run();

