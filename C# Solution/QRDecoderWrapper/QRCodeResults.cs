using QRCodeDecoderLibrary;

namespace Appeon.ComponentsApp.QRDecoderWrapper
{
    public class QRCodeResults
    {
        public QRCodeResults(IList<QRCodeResult> results)
        {
            Results = results;
        }

        public IList<QRCodeResult> Results { get; set; }

        public int Count => Results.Count;
    }
}
