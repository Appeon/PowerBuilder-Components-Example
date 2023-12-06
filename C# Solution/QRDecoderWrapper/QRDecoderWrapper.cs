using Appeon.ComponentsApp.QRDecoderWrapper;
using QRCodeDecoderLibrary;
using System.Drawing;

namespace QRDecoderWrapper
{
    public class QRDecoderWrapper
    {
        public QRDecoder Decoder { get; }

        public QRDecoderWrapper()
        {
            Decoder = new QRDecoder();
#if DEBUG
            //QRCodeTrace.Open("qrcodetrace.log");
#endif
        }

        public int Decode(string path, out QRCodeResults? results, out string? error)
        {
            results = null;
            error = null;

            try
            {
                using var bitmap = new Bitmap(path);
                var result = Decoder.ImageDecoder(bitmap);
                if (result is null)
                {
                    error = "No decode results";
                    return -1;
                }

                results = new QRCodeResults(result);
            }
            catch (Exception e)
            {
                error = e.Message;

                return -1;
            }

            return 1;
        }

        public static int? GetDataString(in QRCodeResults? results, int index, out string? data)
        {
            data = null;

            if (results is null)
                return null;
            data = QRDecoder.ByteArrayToStr(results.Results[index].DataArray);

            return 1;
        }

        public static int? GetECIAssignValue(in QRCodeResults? results, int index, out int? data)
        {
            data = null;

            if (results is null)
                return null;
            data = results.Results[index].ECIAssignValue;

            return 1;
        }


        public static int? GetQRCodeVersion(in QRCodeResults? results, int index, out int? data)
        {
            data = null;

            if (results is null)
                return null;
            data = results.Results[index].QRCodeVersion;

            return 1;
        }

        public static int? GetQRCodeDimension(in QRCodeResults? results, int index, out int? data)
        {
            data = null;

            if (results is null)
                return null;
            data = results.Results[index].QRCodeDimension;

            return 1;
        }

        public static int? GetErrorCorrection(in QRCodeResults? results, int index, out string? data)
        {
            data = null;

            if (results is null)
                return null;
            data = results.Results[index].ErrorCorrection.ToString();

            return 1;
        }
    }
}