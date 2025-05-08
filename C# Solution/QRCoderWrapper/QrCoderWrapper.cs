using QRCoder;
using System.Linq;

namespace Appeon.ComponentsApp.QrCoderWrapper
{
    public class QrCoderWrapper
    {
        private static int GetQrAsBlobInternal(string content, int ppm, QRCodeGenerator.ECCLevel errorCorrection, out byte[]? graphic, out string? error)
        {
            try
            {
                QRCodeGenerator generator = new();
                QRCodeData data = generator.CreateQrCode(content, errorCorrection);
                var qrCode = new BitmapByteQRCode(data);
                graphic = qrCode.GetGraphic(ppm);
                if (graphic == null)
                {
                    error = "Could not generate graphic. Unknown error";
                    return -1;
                }
                error = null;
                return 1;
            }
            catch (Exception e)
            {
                graphic = null;
                error = e.Message;
                return -1;
            }
        }

        public static int GetQrAsBlob(string content, int ppm, out byte[]? graphic, out string? error)
        {
            return GetQrAsBlobInternal(content, ppm, QRCodeGenerator.ECCLevel.M, out graphic, out error);
        }

        public static int GetQrAsBlob(string content, short ppm, short errorCorrection, out byte[]? graphic, out string? error)
        {
            graphic = null;
            error = null;

            if (!Enum.GetValues<QRCodeGenerator.ECCLevel>()
                .Cast<QRCodeGenerator.ECCLevel>()
                .Select(@enum => (short)@enum)
                .Contains(errorCorrection))
            {
                error = "Invalid error correction value";
                return -1;
            }

            try
            {
                QRCodeGenerator generator = new();
                QRCodeData data = generator.CreateQrCode(content, (QRCodeGenerator.ECCLevel)errorCorrection);
                var qrCode = new BitmapByteQRCode(data);
                graphic = qrCode.GetGraphic(ppm);
                if (graphic == null)
                {
                    error = "Could not generate graphic. Unknown error";
                    return -1;
                }
                error = null;
                return 1;
            }
            catch (Exception e)
            {
                graphic = null;
                error = e.Message;
                return -1;
            }
        }
    }
}