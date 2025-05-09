﻿
using Betalgo.Ranul.OpenAI.Managers;
using Betalgo.Ranul.OpenAI.ObjectModels;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using PowerBuilderEventInvoker.DotNet;

namespace Appeon.ComponentsApp.OpenAITools
{
    public class CompletionsWrapper
    {
        private readonly OpenAIService service;

        public CompletionsWrapper(OpenAIService service)
        {
            this.service = service;
        }

        public int CreateCompletion(string prompt,
            out string? completionResult,
            out string? error,
            string? model = null
            )
        {
            error = null;
            completionResult = null;
            try
            {
                var modelString = model;
                var result = service.ChatCompletion.CreateCompletion(new Betalgo.Ranul.OpenAI.ObjectModels.RequestModels.ChatCompletionCreateRequest()
                {
                    Messages = new List<ChatMessage> { new ChatMessage() { Role = "user", Content = prompt } },
                    Model = modelString ?? Models.Gpt_3_5_Turbo,
                    MaxTokens = 4000
                }).Result;

                if (result.Successful)
                {

                    completionResult = result.Choices.FirstOrDefault()?.Message.Content;
                    return 1;
                }

                else if (result.Error is null)
                {
                    error = "Unknown error";
                }
                else
                {
                    error = result.Error.Message;
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
            return -1;
        }

        public async void CreateCompletionAsync(string prompt,
            string callbackObject,
            string callbackEvent,
            string errorEvent,
            string? model = null
            )
        {
            EventInvoker.InvokeEvent(callbackObject, "___stub___");
            await Task.Run(() =>
            {
                var resultInt = CreateCompletion(prompt, out var result, out var error, model);
                if (resultInt != 1)
                {
                    EventInvoker.InvokeEvent(callbackObject, errorEvent, error);
                    return;
                }

                EventInvoker.InvokeEvent(callbackObject, callbackEvent, result);

            });
        }

        private static string GetModelFromPropertyName(string propertyName)
        {
            var prop = typeof(Models).GetProperty(propertyName) ?? throw new ArgumentException("Invalid property name", nameof(propertyName));

            return (string)prop.GetValue(null)!;
        }
    }
}