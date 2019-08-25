//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Text;
//using System.Threading;
//using AudioToolbox;
//using AVFoundation;
//using Foundation;
//using Podcastomatik.Shared.Models;
//using UIKit;

//namespace Podcastomatik.Shared.Services
//{
//    public class PlaybackDownloadedHandler
//    {
//        NSTimer updatingTimer;
//        StreamingPlayback player;

//        public event EventHandler<ErrorArg> ErrorOccurred;
//        public string SourceUrl { get; private set; }
//        public PlayerOption PlayerOption { get; private set; }

//        public class ErrorArg : EventArgs
//        {
//            public string Description { get; set; }
//        }

//        public PlaybackDownloadedHandler()
//        {

//        }

//        void StreamDownloadedHandler(IAsyncResult result)
//        {
//            var buffer = new byte[8192];
//            int l = 0;
//            int inputStreamLength;
//            double sampleRate = 0;

//            Stream inputStream;
//            AudioQueueTimeline timeline = null;

//            var request = result.AsyncState as HttpWebRequest;
//            try
//            {
//                var response = request.EndGetResponse(result);
//                var responseStream = response.GetResponseStream();

//                if (PlayerOption == PlayerOption.StreamAndSave)
//                    inputStream = GetQueueStream(responseStream);
//                else
//                    inputStream = responseStream;

//                using (player = new StreamingPlayback())
//                {
//                    player.OutputReady += delegate {
//                        timeline = player.OutputQueue.CreateTimeline();
//                        sampleRate = player.OutputQueue.SampleRate;
//                    };

//                    NSObject.InvokeOnMainThread(delegate {
//                        if (updatingTimer != null)
//                            updatingTimer.Invalidate();

//                        updatingTimer = NSTimer.CreateRepeatingScheduledTimer(0.5, (timer) => RepeatingAction(timeline, sampleRate));
//                    });

//                    while ((inputStreamLength = inputStream.Read(buffer, 0, buffer.Length)) != 0 && player != null)
//                    {
//                        l += inputStreamLength;
//                        player.ParseBytes(buffer, inputStreamLength, false, l == (int)response.ContentLength);

//                        InvokeOnMainThread(delegate {
//                            progressBar.Progress = l / (float)response.ContentLength;
//                        });
//                    }
//                }

//            }
//            catch (Exception e)
//            {
//                RaiseErrorOccurredEvent("Error fetching response stream\n" + e);
//                Debug.WriteLine(e);
//                InvokeOnMainThread(delegate {
//                    if (NavigationController != null)
//                        NavigationController.PopToRootViewController(true);
//                });
//            }
//        }

//        Stream GetQueueStream(Stream responseStream)
//        {
//            var queueStream = new QueueStream(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/copy.mp3");
//            var t = new Thread((x) => {
//                var tbuf = new byte[8192];
//                int count;

//                while ((count = responseStream.Read(tbuf, 0, tbuf.Length)) != 0)
//                    queueStream.Push(tbuf, 0, count);

//            });
//            t.Start();
//            return queueStream;
//        }

//        void RepeatingAction(AudioQueueTimeline timeline, double sampleRate)
//        {
//            var queue = player.OutputQueue;
//            if (queue == null || timeline == null)
//                return;

//            bool disc = false;
//            var time = new AudioTimeStamp();
//            queue.GetCurrentTime(timeline, ref time, ref disc);

//            playbackTime.Text = FormatTime(time.SampleTime / sampleRate);
//        }

//        string FormatTime(double time)
//        {
//            double minutes = time / 60;
//            double seconds = time % 60;

//            return string.Format("{0}:{1:D2}", (int)minutes, (int)seconds);
//        }

//        void RaiseErrorOccurredEvent(string message)
//        {
//            var handler = ErrorOccurred;
//            if (handler != null)
//                handler(this, new ErrorArg { Description = message });
//        }
//    }
//}
