using System;

namespace Appeon.ComponentsApp.PowerBuilderEventInvoker.DotNetFramework
{
    public static class EventInvoker
    {
        public static int InvokeEvent(string objectName, string eventName, string argument)
        {
            return PowerBuilder.RegisteredObject.TriggerEvent(objectName, eventName, argument);
        }

        public static int TestObjectEventInvokation(string objectName)
        {
            return PowerBuilder.RegisteredObject.TriggerEvent(objectName, Guid.NewGuid().ToString());
        }
    }
}
