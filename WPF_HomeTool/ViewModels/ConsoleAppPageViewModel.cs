using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.ViewModels
{
    public partial class ConsoleAppPageViewModel : ObservableObject
    {
        private readonly ILogger<ConsoleAppPageViewModel> _logger;
        [ObservableProperty]
        private string _userCommandText;
        [ObservableProperty]
        private string _consoleOutputText;

        private Process cmd;
        public ConsoleAppPageViewModel(ILogger<ConsoleAppPageViewModel> logger)
        {
            _logger = logger;
            initialProcess();
        }

        [RelayCommand]
        private void RunUserInput()
        {

            ExecutePowershellCommand();

            //string result = ExecuteCmdCommand(UserCommandText);
            //ConsoleOutputText=result;
            //_logger.LogInformation(result);
        }

        private void initialProcess()
        {
            cmd = new Process();
            cmd.StartInfo.FileName = "powershell.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            cmd.Start();
            //通过事件处理函数的方式获得命令行中的反馈结果
            cmd.OutputDataReceived += (sender, e) => { 
                ConsoleOutputText += e.Data+Environment.NewLine;
                //Debug.WriteLine(e.Data);
            };
            cmd.ErrorDataReceived += (sender, e) => { 
                ConsoleOutputText += e.Data + Environment.NewLine; 
            };

            cmd.BeginOutputReadLine();
            cmd.BeginErrorReadLine();
        }

        private void ExecutePowershellCommand()
        {
            string s = UserCommandText;
            cmd.StandardInput.WriteLine(s);
            //如果用户输入exit，则关闭cmd进程
            if (s == "exit")
            {
                cmd.StandardInput.Close();
                cmd.WaitForExit();
            }
        }

        /// <summary>
        /// 执行CMD命令并返回其输出结果。
        /// </summary>
        /// <param name="command">要执行的命令字符串（例如："ipconfig" 或 "dir"）。</param>
        /// <returns>命令的标准输出和标准错误输出。</returns>
        private string ExecuteCmdCommand(string command)
        {
            // 使用 /C 参数让 cmd 在执行完命令后自动关闭
            string fullCommand = $"/C {command}";

            // 1. 配置进程启动信息
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = fullCommand,
                // 必须设为 false，否则无法重定向流
                UseShellExecute = false,
                // 必须设为 true，才能捕获输出
                RedirectStandardOutput = true,
                // 如果程序有错误输出，也进行捕获
                RedirectStandardError = true,
                // 不显示CMD的黑色窗口
                CreateNoWindow = true,
                // 设置编码，确保中文等字符正确显示
                StandardOutputEncoding = Encoding.GetEncoding("UTF-8"),
                StandardErrorEncoding = Encoding.GetEncoding("UTF-8")
            };

            // 2. 创建并启动进程
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;

                try
                {
                    process.Start();

                    // 3. 同步读取输出和错误流
                    // ReadToEnd() 会等待进程执行完毕并读取全部输出
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // 等待进程退出
                    process.WaitForExit();

                    // 4. 组合结果
                    if (string.IsNullOrEmpty(error))
                    {
                        return $"命令：{command}\n\n执行成功，输出结果：\n{output}";
                    }
                    else
                    {
                        return $"命令：{command}\n\n执行失败，错误信息：\n{error}\n\n标准输出（可能包含部分结果）：\n{output}";
                    }
                }
                catch (Exception ex)
                {
                    return $"执行命令时发生异常：{ex.Message}";
                }
            }
        }
    }
}
