using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using PowerBuilderEventInvoker.DotNet;
using System.Text;
using System.Text.Json;

namespace Appeon.ComponentsApp.OpenAITools
{
    public class ImageGenerationWrapper
    {
        private readonly OpenAIService service;

        public ImageGenerationWrapper(OpenAIService service)
        {
            this.service = service;
        }

        public int CreateImage(string prompt, int images, string size, out byte[][]? imageData, out string? error)
        {
            imageData = null;
            error = null;

            var sizeString = size switch
            {
                "256" or "512" or "1024" => $"{size}x{size}",
                _ => throw new ArgumentException("Invalid image size", nameof(size))
            };

            var result = service.Image.CreateImage(
                new ImageCreateRequest()
                {
                    Prompt = prompt,
                    N = images,
                    ResponseFormat = "b64_json",
                    Size = sizeString,
                    User = "TestUser",
                }
                ).Result;

            if (result.Successful)
            {
                imageData = result.Results.Select(res => Convert.FromBase64String(res.B64)).ToArray();
                return 1;
            }
            else if (result.Error is null)
            {
                error = "Unknown error";
                return -1;
            }
            error = result.Error.Message;
            return -1;
        }

        public async void CreateImageAsync(
            string prompt,
            int images,
            string size,
            string callbackObject,
            string callbackEvent,
            string errorEvent)
        {
            EventInvoker.InvokeEvent(callbackObject, "___stub___");
            await Task.Run(() =>
            {
                var res = CreateImage(prompt, images, size, out byte[][]? data, out var error);

                if (res != 1)
                {
                    EventInvoker.InvokeEvent(callbackObject, errorEvent, error);
                    return;
                }

                EventInvoker.InvokeEvent(callbackObject, callbackEvent, JsonSerializer.Serialize(new ImageGenerationResult()
                {
                    Prompt = prompt,
                    Data = data?.Select(blob => Convert.ToBase64String(blob)).ToArray(),

                }));
            });
        }

        public static int Deserialize(string data, out ImageGenerationResult? result, out string? error)
        {
            result = null;
            error = null;
            try
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
                var res = JsonSerializer.Deserialize<ImageGenerationResult>(stream);
                if (res is null)
                {
                    error = "Deserialize returned null";
                    return -1;
                }

                result = res;
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int CreateImage(string prompt, int images, string size, out string[]? url, out string? error)
        {
            url = null;
            error = null;

            var sizeString = size switch
            {
                "256" or "512" or "1024" => $"{size}x{size}",
                _ => throw new ArgumentException("Invalid image size", nameof(size))
            };

            var result = service.Image.CreateImage(
                new ImageCreateRequest()
                {
                    Prompt = prompt,
                    N = images,
                    ResponseFormat = "url",
                    Size = sizeString,
                    User = "TestUser"
                }
                ).Result;

            if (result.Successful)
            {
                url = result.Results.Select(res => res.Url).ToArray();
                return 1;
            }
            else if (result.Error is null)
            {
                error = "Unknown error";
                return -1;
            }
            error = result.Error.Message;
            return -1;
        }

        public static void TriggerPbEvent(string @object, string @event)
        {
            EventInvoker.InvokeEvent(@object, @event);
        }
    }
}
