using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_HomeTool.Helpers
{
    public class ToastNotificationHelper
    {
        private const string APP_ID = "HomeTool.WPF.Toast";

        /// <summary>
        /// 显示简单的 Toast 通知
        /// </summary>
        public static void ShowSimpleToast(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }

        /// <summary>
        /// 显示带按钮的 Toast 通知
        /// </summary>
        public static void ShowToastWithButtons(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .AddButton(new ToastButton()
                    .SetContent("确认")
                    .AddArgument("action", "confirm"))
                .AddButton(new ToastButton()
                    .SetContent("取消")
                    .AddArgument("action", "cancel"))
                .Show();
        }

        /// <summary>
        /// 显示带图片的 Toast 通知
        /// </summary>
        public static void ShowToastWithImage(string title, string message, string imagePath)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .AddInlineImage(new Uri(imagePath))
                .Show();
        }

        /* 代码有问题，编译器都过不了
        /// <summary>
        /// 显示进度条 Toast 通知
        /// </summary>
        public static void ShowProgressToast(string title, string status)
        {
            var content = new ToastContentBuilder()
                .AddText(title)
                .AddVisualChild(new AdaptiveProgressBar()
                {
                    Value = new BindableProgressBarValue("progressValue"),
                    Status = new BindableString("progressStatus")
                })
                .GetToastContent();

            var toast = new ToastNotification(content.GetXml())
            {
                Tag = "progress-toast",
                Group = "progress-group",
                Data = new NotificationData()
            };

            toast.Data.Values["progressValue"] = "0.0";
            toast.Data.Values["progressStatus"] = status;
            toast.Data.SequenceNumber = 0;

            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
        }

        /// <summary>
        /// 更新进度条 Toast
        /// </summary>
        public static void UpdateProgressToast(double progress, string status)
        {
            var data = new NotificationData
            {
                SequenceNumber = 0
            };
            data.Values["progressValue"] = progress.ToString("0.00");
            data.Values["progressStatus"] = status;

            ToastNotificationManager.CreateToastNotifier(APP_ID)
                .Update(data, "progress-toast", "progress-group");
        }
        */

        /// <summary>
        /// 显示带音频的 Toast 通知
        /// </summary>
        public static void ShowToastWithAudio(string title, string message, ToastAudio audio = null)
        {
            var builder = new ToastContentBuilder()
                .AddText(title)
                .AddText(message);

            if (audio != null)
            {
                builder.AddAudio(audio.Src, audio.Silent, audio.Loop);
            }

            builder.Show();
        }

        /// <summary>
        /// 显示自定义持续时间的 Toast
        /// </summary>
        public static void ShowLongToast(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .SetToastDuration(ToastDuration.Long) // 显示 25 秒
                .Show();
        }

        /// <summary>
        /// 显示带输入框的 Toast 通知
        /// </summary>
        public static void ShowToastWithInput(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .AddInputTextBox("replyBox", "输入回复...")
                .AddButton(new ToastButton()
                    .SetContent("发送")
                    .AddArgument("action", "reply")
                    .SetTextBoxId("replyBox"))
                .Show();
        }
    }

    // 音频辅助类，暂时用不上，等用上了再打卡
    //public class ToastAudio
    //{
    //    public Uri Src { get; set; }
    //    public bool Silent { get; set; }
    //    public bool Loop { get; set; }

    //    public static ToastAudio Default => new ToastAudio { Src = new Uri("ms-winsoundevent:Notification.Default") };

    //    public static ToastAudio SilentAudio => new ToastAudio { Silent = true };
    //}
}
