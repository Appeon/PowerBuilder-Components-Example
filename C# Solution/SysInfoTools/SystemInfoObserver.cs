using Appeon.ComponentsApp.PowerBuilderEventInvoker.DotNetFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Appeon.ComponentsApp.SystemInfoTools
{
    public class SystemInfoObserver
    {
        private readonly Dictionary<int, Thread> _threads = new Dictionary<int, Thread>();
        private readonly Dictionary<int, bool> _threadContinuationFlags = new Dictionary<int, bool>();
        private readonly Random _random = new Random(DateTime.Now.Ticks.GetHashCode());

        private bool _canContinue = true;

        public int Subscribe(
            string callbackObject,
            string perfInfoCallback,
            string processCallback,
            string errorCallback,
            int updateMs)
        {
            _canContinue = true;

            EventInvoker.TestObjectEventInvokation(callbackObject);
            //EventInvoker.InvokeEvent(callbackObject, "ue_error", "test");
            int idx;
            while (_threadContinuationFlags.ContainsKey(idx = _random.Next())) ;


            var thread = new Thread(() =>
            {
                try
                {
                    while (_canContinue && _threadContinuationFlags[idx])
                    {
                            EventInvoker.InvokeEvent(callbackObject,
                                                perfInfoCallback,
                                                $"{string.Join(",", SystemInfoTools.GetPerformanceInfo().ToDoubleArray())}"
                                                );

                            var sb = new StringBuilder();
                            var processes = SystemInfoTools.GetProcesses();

                            for (int i = 0; i < processes.Count; i++)
                                sb.AppendLine(processes[i].ToString());


                            EventInvoker.InvokeEvent(callbackObject,
                                processCallback,
                                sb.ToString());

                            Thread.Sleep(updateMs);
                        
                    }
                }
                catch (ThreadAbortException) { }
                catch(Exception e)
                {
                    EventInvoker.InvokeEvent(callbackObject,
                                                errorCallback,
                                                $"{e.Message}"
                                                );
                }
            })
            {

            };
            _threads[idx] = thread;
            _threadContinuationFlags[idx] = true;

            thread.Start();
            return idx;
        }

        public void Unsubscribe(int id)
        {
            _threadContinuationFlags[id] = false;

            _threads.Remove(id);
        }

        public void UnsubscribeAll()
        {
            _canContinue = false;

            _threads.Clear();
        }


    }
}
