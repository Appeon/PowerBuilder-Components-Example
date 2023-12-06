namespace Appeon.ComponentsApp.DdeTools.DdeClient
{
    public class DdeContext
    {
        public Dictionary<int, object> Instances { get; set; }

        public DdeContext(Dictionary<int, object> instances)
        {
            Instances = instances;
        }
    }
}
