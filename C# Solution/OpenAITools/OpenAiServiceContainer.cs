using OpenAI;
using OpenAI.Managers;

namespace Appeon.ComponentsApp.OpenAITools
{
    public class OpenAiServiceContainer
    {
        public static OpenAIService? Instance { get; private set; }

        public static void Init(
            string apiKey,
            string? organtization)
        {
            Instance ??= new OpenAIService(
                new OpenAiOptions
                {
                    ApiKey = apiKey,
                    Organization = organtization,
                }
                );
        }

        public static short TestService(out string? error)
        {
            error = null;

            if (Instance is null)
            {
                error = "Service is not initialized";
                return -1;
            }

            try
            {
                lock (Instance)
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    var taskFinished = false;

                    var timeoutThread = new Thread(() =>
                                { // Create a thread that checks if the task has completed after 5s
                                    try
                                    {
                                        Thread.Sleep(10000);
                                        if (!taskFinished)
                                        {
                                            cancellationTokenSource.Cancel();
                                        }
                                    }
                                    catch (ThreadInterruptedException)
                                    {
                                    }
                                });

                    timeoutThread.Start();

                    var models = Instance.ListModel(cancellationTokenSource.Token).Result;
                    taskFinished = true;
                    timeoutThread.Interrupt();
                    if (models is not null)
                        return 1;
                    return -1;
                }

            }
            catch (Exception e) when (
                (e is AggregateException ae
                    && ae.InnerException is TaskCanceledException)
                || e is TaskCanceledException)
            {
                error = "Test credentials timed out";
                return -1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            };
        }
    }
}
