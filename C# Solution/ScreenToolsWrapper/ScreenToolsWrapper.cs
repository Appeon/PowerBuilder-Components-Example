using Appeon.ComponentsApp.PowerBuilderEventInvoker.DotNetFramework;
using Appeon.ComponentsApp.ScreenTools;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appeon.ComponentsApp.ScreenToolsWrapper
{
    public class ScreenToolsWrapper : IDisposable
    {
        private Thread ScreenShotThread { get; set; }
        private bool KeepTakingScreenshots;
        private ConcurrentQueue<Action> Queue { get; set; }

        private Thread Worker { get; }
        private bool RunWorker { get; set; } = true;

        private bool disposedValue;

        public int ContainerWidth { get; set; }
        public int ContainerHeight { get; set; }

        public ScreenToolsWrapper()
        {
            Queue = new ConcurrentQueue<Action>();
            Worker = new Thread(() =>
            {
                while (RunWorker)
                {
                    if (Queue.IsEmpty)
                    {
                        Thread.Sleep(1);
                    }
                    else
                    {
                        var dequed = Queue.TryDequeue(out Action action);
                        if (dequed)
                            action();
                    }
                }
            });

            Worker.Start();
        }


        ~ScreenToolsWrapper() => Dispose(false);

        public static async void TakeScreenshot(string callbackObject, string eventName, double scale = 1.0)
        {
            var result = await Task.Run(() =>
            {
                var array = ScreenShotter.TakeScreenshot(scale);
                var b64 = Convert.ToBase64String(array);
                return b64;
            });

            EventInvoker.InvokeEvent(callbackObject, eventName, result);
        }

        public int TakeScreenshot(string screenName, string callbackObject, string eventName, out string error, double scale = 1.0)
        {
            error = null;

            EventInvoker.TestObjectEventInvokation(callbackObject);

            try
            {
                var screenDef = ScreenManager.GetScreens()
                        .Where(s => s.ScreenName == screenName)
                        .FirstOrDefault();

                if (screenDef == null)
                {
                    error = "No screen with that name";
                    return -1;
                }


                this.Queue.Enqueue(() =>
                {
                    var array = ScreenShotter.TakeScreenshot(screenDef, scale);
                    var b64 = Convert.ToBase64String(array);
                    EventInvoker.InvokeEvent(callbackObject, eventName, b64);
                });

            }
            catch (Exception e)
            {

                error = e.Message;
                return -1;
            }

            return 1;
        }

        public int TakeScreenshot(
            string screenName,
            string callbackObject,
            string eventName,
            out string error)
        {
            error = null;

            EventInvoker.TestObjectEventInvokation(callbackObject);

            try
            {
                var screenDef = ScreenManager.GetScreens()
                        .Where(s => s.ScreenName == screenName)
                        .FirstOrDefault();

                if (screenDef == null)
                {
                    error = "No screen with that name";
                    return -1;
                }


                this.Queue.Enqueue(() =>
                {
                    var array = ScreenShotter.TakeScreenshot(screenDef);
                    var b64 = Convert.ToBase64String(array);
                    EventInvoker.InvokeEvent(callbackObject, eventName, b64);
                });

            }
            catch (Exception e)
            {

                error = e.Message;
                return -1;
            }

            return 1;
        }

        public int GetBitmapDimensions(byte[] blob, out int width, out int height, out string error)
        {
            error = null;
            width = -1;
            height = -1;

            try
            {
                var bitmap = new Bitmap(new MemoryStream(blob), false);

                width = bitmap.Width;
                height = bitmap.Height;

                return 1;
            }
            catch (Exception e)
            {
                error = $"Could not get image dimensions: {e.Message}";
                return -1;
            }
        }

        public int StartTakingScreenshots(
            string screenName,
            double interval,
            string callbackObject,
            string eventName,
            string errorCallback,
            out string error,
            double scale = 1.0)
        {
            error = null;

            if (ScreenShotThread != null)
            {
                error = "Already running";
                return -1;
            }

            int intervalMs = (int)(interval * 1000.0);

            KeepTakingScreenshots = true;

            ScreenShotThread = new Thread(() =>
            {
                try
                {
                    while (KeepTakingScreenshots)
                    {
                        try
                        {
                            var res = TakeScreenshot(screenName, callbackObject, eventName, out string localerror, scale);
                            Thread.Sleep(intervalMs);
                        }
                        catch (Exception e)
                        {
                            EventInvoker.InvokeEvent(callbackObject, errorCallback, e.Message);
                            break;
                        }
                    }
                }
                catch (ThreadInterruptedException)
                {
                }
            });

            ScreenShotThread.Start();

            return 1;
        }

        public int StartTakingScreenshots(
            string screenName,
            double interval,
            string callbackObject,
            string eventName,
            string errorCallback,
            out string error)
        {
            error = null;


            if (ScreenShotThread != null)
            {
                error = "Already running";
                return -1;
            }

            PowerBuilderEventInvoker.DotNetFramework.EventInvoker.TestObjectEventInvokation(callbackObject);

            int intervalMs = (int)(interval * 1000.0);

            KeepTakingScreenshots = true;

            ScreenShotThread = new Thread(() =>
            {
                try
                {

                    while (KeepTakingScreenshots)
                    {
                        try
                        {
                            var res = TakeScreenshot(screenName, callbackObject, eventName, out string localerror);
                            Thread.Sleep(intervalMs);
                        }
                        catch (ThreadInterruptedException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            EventInvoker.InvokeEvent(callbackObject, errorCallback, e.Message);
                            break;
                        }


                    }
                }
                catch (ThreadInterruptedException)
                {

                }
            });

            ScreenShotThread.Start();

            return 1;
        }

        public int StopTakingScreenshots(out string error)
        {
            error = null;

            if (ScreenShotThread is null)
            {
                error = "Not running";
                return -1;
            }

            while (!Queue.IsEmpty)
            {
                Queue.TryDequeue(out var queue);
            }
            ScreenShotThread.Interrupt();
            KeepTakingScreenshots = false;

            ScreenShotThread = null;

            return 1;
        }

        public static int GetScreenNames(out string[] names, out string[] resolutions, out string error)
        {
            resolutions = null;
            names = null;
            error = null;

            try
            {
                var displays = ScreenManager
                        .GetScreens()
                        .ToList();

                names = new string[displays.Count];
                resolutions = new string[displays.Count];

                for (int i = 0; i < displays.Count; ++i)
                {
                    names[i] = displays[i].ScreenName;
                    resolutions[i] = $"{displays[i].Width}x{displays[i].Height}";
                }

            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            return 1;
        }

        public int GetScreenDimensions(string screenName, out int x, out int y, out string error)
        {
            error = null;
            x = -1;
            y = -1;


            try
            {
                var display = ScreenManager.GetScreens()
                        .Where(s => s.ScreenName == screenName)
                        .Single();

                x = display.X;
                y = display.Y;

                return 1;
            }
            catch (Exception e)
            {
                error = "Could not find display with that name";
                return -1;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    RunWorker = false;
                    Worker.Join();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
