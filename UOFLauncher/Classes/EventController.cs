using System;
using UOFLauncher.Models;

namespace UOFLauncher.Classes
{
    public delegate void DownloadsCompletedEventHandler();
    public delegate void UpdatesRetrievedEventHandler( );


    public static class EventController
    {
        public static event DownloadsCompletedEventHandler DownloadsCompleted;

        public static event UpdatesRetrievedEventHandler UpdatesRetrieved;

        public static void InvokeDownloadsComplete()
        {
            if (DownloadsCompleted != null)
            {
                DownloadsCompleted();
            }
        }

        public static void InvokeUpdatesRetrieved()
        {
            if (UpdatesRetrieved != null)
            {
                UpdatesRetrieved();
            }
        }
    }
}