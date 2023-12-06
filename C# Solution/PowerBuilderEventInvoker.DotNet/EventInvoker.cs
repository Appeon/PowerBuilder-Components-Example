namespace PowerBuilderEventInvoker.DotNet
{
    public static class EventInvoker
    {
        public static int TestObjectEventInvokation(string objectName)
        {
            return PowerBuilder.RegisteredObject.TriggerEvent(objectName, Guid.NewGuid().ToString());
        }

        public static int InvokeEvent(string objectName, string eventName, string? argument = null)
        {
            return PowerBuilder.RegisteredObject.TriggerEvent(objectName, eventName, argument);
        }
    }
}