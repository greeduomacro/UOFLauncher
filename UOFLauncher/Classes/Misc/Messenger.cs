using System;
using GalaSoft.MvvmLight.Messaging;

namespace UOFLauncher.Classes
{
    internal class MessengerHelper
    {
        public static class ToastSetter
        {
            public static void SetStatus(string s)
            {
                Messenger.Default.Send(new ToastMessage(s));
            }
        }

        public static class ProgressBarSetting
        {
            public static int TotaltoDownload { get; set; }
            public static int TotalDownloaded { get; set; }

            public static void AddToTotaltoDownload(int total)
            {
                TotaltoDownload += total;
            }

            public static void SetStatus(int value)
            {
                TotalDownloaded += value;
                var downloaded = (int) Math.Round(((double) TotalDownloaded/TotaltoDownload)*100);
                Messenger.Default.Send(new ProgressBarValue(downloaded));
            }
        }

        /// <summary>
        ///     A class that holds a status message. Used in StatusSeter
        /// </summary>
        public class ToastMessage
        {
            public ToastMessage(string message)
            {
                Message = message;
            }

            public string Message { get; set; }
        }

        public class ProgressBarValue
        {
            public ProgressBarValue(int value)
            {
                Value = value;
            }

            public int Value { get; set; }
        }
    }
}