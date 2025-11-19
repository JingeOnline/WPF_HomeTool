using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using WPF_HomeTool.Controls;
using WPF_HomeTool.Helpers;
using WPF_HomeTool.Models;

namespace WPF_HomeTool.ViewModels
{
    public partial class ConsoleAppPageViewModel : ObservableObject
    {
        private readonly ILogger<ConsoleAppPageViewModel> _logger;
        [ObservableProperty]
        private string _userCommandText;
        [ObservableProperty]
        private string _consoleOutputText;
        [ObservableProperty]
        private string _selectedApp;
        [ObservableProperty]
        private Visibility _wizardPanelVisibility = Visibility.Visible;
        [ObservableProperty]
        private string _YouTubeDownloadUrl;
        [ObservableProperty]
        private string _YouTubeVideoDownloadPath;
        partial void OnYouTubeVideoDownloadPathChanged(string? oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                ConfigHelper.WriteKeyValue("YoutubeDownloadDirectory", newValue);
            }
        }
        [ObservableProperty]
        private ObservableCollection<RadioButtonItemModel> _youtubeRadioButtons = new ObservableCollection<RadioButtonItemModel>();

        private Dictionary<string, string> _youtubeDownloadModes = new Dictionary<string, string>
        {
            {"默认webm视频", "$ExePath -P \"$DownloadPath\" $VideoUrl --cookies-from-browser chrome"},
            {"MP4视频", "$ExePath -P \"$DownloadPath\" -f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best\" $VideoUrl --cookies-from-browser chrome" },
            {"m4a音频", "$ExePath -P \"$DownloadPath\" -f\"bestaudio[ext=m4a]\" $VideoUrl --cookies-from-browser chrome"},
            {"webm视频+str字幕", "$ExePath -P \"$DownloadPath\" --write-sub --convert-subs \"srt\" --sub-lang $Language $VideoUrl --cookies-from-browser chrome"},
            {"MP4视频+srt字幕","$ExePath -P \"$DownloadPath\" -f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best\" --write-sub --convert-subs \"srt\" --sub-lang $Language $VideoUrl --cookies-from-browser chrome"},
            {"仅下载srt字幕" ,"$ExePath -P \"$DownloadPath\" --skip-download --write-sub --convert-subs \"srt\" --sub-lang $Language $VideoUrl --cookies-from-browser chrome"},
            {"模拟VR客户端下载8K视频","$ExePath -P \"$DownloadPath\" $VideoUrl --extractor-arg \"youtube:player_client=android_vr\" -S res:4320 --cookies-from-browser chrome" },
            {"查看视频分辨率列表","$ExePath -F $VideoUrl --list-formats --cookies-from-browser chrome" }
        };

        private Process powershell;
        private List<string> resultLines = new List<string>();
        public ConsoleAppPageViewModel(ILogger<ConsoleAppPageViewModel> logger)
        {
            _logger = logger;
            initialProcess();
            foreach (var mode in _youtubeDownloadModes)
            {
                YoutubeRadioButtons.Add(new RadioButtonItemModel(mode.Key, mode.Value));
            }
            YoutubeRadioButtons[0].IsChecked = true;
            YouTubeVideoDownloadPath = ConfigHelper.ReadKeyValue("YoutubeDownloadDirectory")!;
        }

        [RelayCommand]
        private void RunUserInput()
        {
            //if (CheckAppPath())
            //{
            resultLines.Clear();
            ExecuteUserInputIntoPowershell();
            //}
        }
        [RelayCommand]
        private void ClearConsoleOutputText()
        {
            ConsoleOutputText = string.Empty;
        }
        [RelayCommand]
        private void SetWizardPanelVisibility()
        {
            WizardPanelVisibility = WizardPanelVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
        [RelayCommand]
        private void CreateCommandLine()
        {
            if (CheckAppPath() && CheckYouTubeDownloadPath())
            {
                string commandLine = YoutubeRadioButtons.First(x => x.IsChecked).Command;
                commandLine = commandLine.Replace("$ExePath", ConfigHelper.ReadKeyValue("yt-dlp_Path"));
                commandLine = commandLine.Replace("$DownloadPath", ConfigHelper.ReadKeyValue("YoutubeDownloadDirectory"));
                if (!string.IsNullOrEmpty(YouTubeDownloadUrl))
                {
                    commandLine = commandLine.Replace("$VideoUrl", YouTubeDownloadUrl);
                }
                UserCommandText = commandLine;
            }

        }
        [RelayCommand]
        private void SelectYoutubeVideosSavePath()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    YouTubeVideoDownloadPath = dialog.FileName;
                }
            }
        }
        [RelayCommand]
        private void OpenDownloadFolderInFileExplorer()
        {
            if (!string.IsNullOrEmpty(YouTubeVideoDownloadPath) && Directory.Exists(YouTubeVideoDownloadPath))
            {
                string windowsFormatPath = Path.GetFullPath(YouTubeVideoDownloadPath);
                System.Diagnostics.Process.Start("explorer.exe", windowsFormatPath);
            }
        }
        private void initialProcess()
        {
            powershell = new Process();
            powershell.StartInfo.FileName = "powershell.exe";
            powershell.StartInfo.RedirectStandardInput = true;
            powershell.StartInfo.RedirectStandardOutput = true;
            powershell.StartInfo.RedirectStandardError = true;
            powershell.StartInfo.CreateNoWindow = true;
            powershell.StartInfo.UseShellExecute = false;

            powershell.Start();
            //通过事件处理函数的方式获得命令行中的反馈结果
            powershell.OutputDataReceived += (sender, e) =>
            {
                ConsoleOutputText += e.Data + Environment.NewLine;
                if (e.Data != null)
                {
                    resultLines.Add(e.Data);
                }
                //Debug.WriteLine(e.Data);
            };
            powershell.ErrorDataReceived += (sender, e) =>
            {
                ConsoleOutputText += e.Data + Environment.NewLine;
            };

            powershell.BeginOutputReadLine();
            powershell.BeginErrorReadLine();
        }

        private void ExecuteUserInputIntoPowershell()
        {
            string s = UserCommandText;
            powershell.StandardInput.WriteLine(s);
            //如果用户输入exit，则关闭cmd进程
            if (s == "exit")
            {
                powershell.StandardInput.Close();
                powershell.WaitForExit();
            }
        }

        private bool CheckAppPath()
        {
            bool isExist = false;
            if (SelectedApp == "yt-dlp")
            {
                isExist = File.Exists(ConfigHelper.ReadKeyValue("yt-dlp_Path"));
                if (!isExist)
                {
                    ModernMessageBox.Show("无法找到yt-dlp.exe，请先在设置中配置正确的yt-dlp路径。", "错误", Controls.MessageBoxButton.OK, Controls.MessageBoxImage.Error);
                }
            }
            return isExist;
        }

        private bool CheckYouTubeDownloadPath()
        {
            bool isExist = false;
            if (SelectedApp == "yt-dlp")
            {
                isExist = Directory.Exists(ConfigHelper.ReadKeyValue("YoutubeDownloadDirectory"));
                if (!isExist)
                {
                    ModernMessageBox.Show("下载路径不存在，请在设置中重新选择保存路径", "错误", Controls.MessageBoxButton.OK, Controls.MessageBoxImage.Error);
                }
            }
            return isExist;
        }
    }
}
