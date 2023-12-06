using OpenAI.Managers;

namespace Appeon.ComponentsApp.OpenAITools
{
    public class ToolsBuilder
    {
        public static CompletionsWrapper NewCompletionsWrapper(
            OpenAIService service)
        {
            return new CompletionsWrapper(service);
        }

        public static ImageGenerationWrapper NewImageGeneration(
            OpenAIService service)
        {
            return new ImageGenerationWrapper(service);
        }

    }
}
