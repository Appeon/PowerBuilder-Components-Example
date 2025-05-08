using Betalgo.Ranul.OpenAI;
using Betalgo.Ranul.OpenAI.Managers;

namespace Appeon.ComponentsApp.OpenAITools
{
    public class OpenAiServiceBuilder
    {
        public string? ApiKey { get; set; }
        public string? Organization { get; set; }


        public OpenAIService? Build(out string? error)
        {
            error = null;
            if (ApiKey is null)
            {
                error = "No API Key specified";
                return null;
            }

            try
            {
                return new OpenAIService(
                        new OpenAIOptions
                        {
                            ApiKey = ApiKey,
                            Organization = Organization,

                        }
                        );
            }
            catch (Exception e)
            {
                error = e.Message;
                return null;
            }
        }

        public static short TestService(OpenAIService service, out string? error)
        {
            error = null;

            try
            {
                lock (service)
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

                    var models = service.ListModel(cancellationTokenSource.Token).Result;
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
