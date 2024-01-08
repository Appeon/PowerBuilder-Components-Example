using BarcodeStandard;
using Fare;
using System.Text.RegularExpressions;
using Type = BarcodeStandard.Type;

namespace Appeon.ComponentsApp.BarcodeGeneration;

public class BarcodeGenerator
{
    private const int BarcodeSamples = 5;
    private string? lastBarcode;

    public static int CreateBarcode(
        string input,
        string barcodeStandard,
        bool showLabel,
        int labelFontSize,
        int width,
        int height,
        out string? base64img,
        out string? error)
    {
        base64img = null;
        error = null;

        try
        {
            if (!Enum.TryParse(barcodeStandard, out TypeAdapter typeEnum))
            {
                error = "Invalid Barcode Standard";
                return -1;
            }
            var normalizedBarcodeType = (BarcodeStandard.Type)(int)typeEnum;

            var b = new Barcode()
            {
                IncludeLabel = showLabel,
                Width = width,
                Height = height,
                LabelFont = new SkiaSharp.SKFont(SkiaSharp.SKTypeface.Default, labelFontSize, 1, 0),
                ImageFormat = SkiaSharp.SKEncodedImageFormat.Png
            };

            b.Encode(normalizedBarcodeType, input);
            base64img = Convert.ToBase64String(b.GetImageData(SaveTypes.Png));
        }
        catch (Exception e)
        {
            error = e.Message;
            return -1;
        }

        return 1;
    }

    public static bool CanCreateBarcode(string input, string barcodeStandard, out string? error)
    {
        error = null;
        try
        {
            if (!Enum.TryParse(barcodeStandard, out TypeAdapter typeEnum))
            {
                error = "Invalid Barcode Standard";
                return false;
            }
            var normalizedBarcodeType = (BarcodeStandard.Type)(int)typeEnum;

            var b = new Barcode();

            b.Encode(normalizedBarcodeType, input);
        }
        catch (Exception e)
        {
            error = e.Message;
            return false;
        }

        return true;
    }

    public static bool CanCreateBarcode(
        string input,
        string barcodeStandard,
        int width,
        int height,
        out string? error)
    {
        error = null;
        try
        {
            if (!Enum.TryParse(barcodeStandard, out TypeAdapter typeEnum))
            {
                error = "Invalid Barcode Standard";
                return false;
            }
            var normalizedBarcodeType = (BarcodeStandard.Type)(int)typeEnum;

            var b = new Barcode()
            {
                Width = width,
                Height = height,
            };

            b.Encode(normalizedBarcodeType, input);
        }
        catch (Exception e)
        {
            error = e.Message;
            return false;
        }

        return true;
    }

    public static void GetTypes(out string[]? names)
    {
        names = null;

        var typeValues = Enum.GetValues<TypeAdapter>();
        names = typeValues
            .Select(type => type.ToString())
            .ToArray();
    }


    public int GetSampleDataForBarcodeStandard(string barcodeStandard, out string? data, out string? error)
    {
        data = null;
        error = null;

        if (!Enum.TryParse(barcodeStandard, out TypeAdapter typeEnum))
        {
            error = "Invalid Barcode Standard";
            return -1;
        }

        string[]? samples = null;

        var normalizedBarcodeType = (BarcodeStandard.Type)(int)typeEnum;

        Regex? inputPattern = null;
        switch (normalizedBarcodeType)
        {
            case Type.Ucc12:
            case Type.UpcA: //Encode_UPCA();
                inputPattern = new Regex(@"^[0-9]{11,12}$");
                break;
            case BarcodeStandard.Type.Ucc13:
            case Type.Ean13: //Encode_EAN13();
                inputPattern = new Regex(@"^[0-9]{10,11}$");
                break;
            case Type.Interleaved2Of5Mod10:
                inputPattern = new Regex(@"^([0-9][0-9]){5,6}[0-9]$");
                break;
            case Type.Interleaved2Of5: //Encode_Interleaved2of5();
                inputPattern = new Regex(@"^([0-9][0-9]){6,7}$");
                break;
            case Type.Industrial2Of5Mod10:
            case Type.Industrial2Of5:
            case Type.Standard2Of5Mod10:
            case Type.Standard2Of5: //Encode_Standard2of5();
                inputPattern = new Regex(@"^[0-9]{10,11}$");
                break;
            case Type.Logmars:
            case Type.Code39Mod43:
            case Type.Code39: //Encode_Code39();
                samples = new string[]
                {
                    "*1234567*",
                    "2351523305",
                    "BCDE12452",
                    "12-1328-716",
                    "PZN-65108940",
                };
                break;
            case Type.Code39Extended:
                //inputPattern = new Regex(@"^[0-9A-Za-z_\!\#\$\%\&\'\(\)\*\+\,\-\.\/\:\;\<\=\>\?\@\[\\\]\^\`][0-9A-Za-z_ \!\#\$\%\&\'\(\)\*\+\,\-\.\/\:\;\<\=\>\?\@\[\\\]\^\`]*$");
                samples = new string[]
                {
                    "*123<32A>457*",
                    "(23)608-4065",
                    "BCDE456452",
                    "1^2198:716",
                    "PZ N65 190150",
                };
                break;
            case Type.Codabar: //Encode_Codabar();
                //inputPattern = new Regex(@"^[A-Da-d][0-9A-Da-d\+\.\/\:\$\-]*[A-Da-d]+$");
                samples = new string[]
                {
                    "AA-65108B",
                    "CV6510+790A",
                    "A4:6507810.450D",
                    "A809$610B",
                    "B5+940:42:948C",
                };
                break;
            case Type.PostNet: //Encode_PostNet();
                inputPattern = new Regex(@"^[0-9]{5}|[0-9]{7}|[0-9]{9}|[0-9]{11}$");
                break;
            case Type.Isbn:
            case Type.Bookland: //Encode_ISBN_Bookland();
                inputPattern = new Regex(@"^(?:978[0-9]{9,10})|(?:[0-9]{9,10})$");
                break;
            case Type.Jan13: //Encode_JAN13();
                inputPattern = new Regex(@"^49[0-9]{10,11}$");
                break;
            case Type.UpcSupplemental2Digit: //Encode_UPCSupplemental_2();
                inputPattern = new Regex(@"^[0-9]{2}$");
                break;
            case Type.MsiMod10:
            case Type.Msi2Mod10:
            case Type.MsiMod11:
            case Type.MsiMod11Mod10:
            case Type.ModifiedPlessey: //Encode_MSI();
                //inputPattern = new Regex(@"^[0-9]+$");
                inputPattern = new Regex(@"^[0-9]{10,11}$");
                break;
            case Type.UpcSupplemental5Digit: //Encode_UPCSupplemental_5();
                inputPattern = new Regex(@"^[0-9]{5}$");
                break;
            case Type.UpcE: //Encode_UPCE();
                inputPattern = new Regex(@"^[01]{6}|[01]{8}|[01]{12}$");
                break;
            case Type.Ean8: //Encode_EAN8();
                inputPattern = new Regex(@"^[0-9]{7,8}$");
                break;
            case Type.Usd8:
            case Type.Code11: //Encode_Code11();
                inputPattern = new Regex(@"^[0-9][0-9\-]{8,10}$");
                break;
            case Type.Code128: //Encode_Code128();
            case Type.Code128A:
            case Type.Code128B:
                samples = new string[]
                {
                    "1234567890",
                    "(10)123(650165)10604",
                    "(50)605651(150)56016",
                    "AE 1234 5678 5V",
                    "16501(9809):504",
                };
                break;
            case Type.Code128C:
                inputPattern = new Regex(@"^[0-9]{10,12}$");
                break;
            case Type.Itf14:
                inputPattern = new Regex(@"^[0-9]{13,14}$");
                break;
            case Type.Code93:
                //inputPattern = new Regex(@"^[0-9A-B\-\.\$\/\+\%][0-9A-B\-\.\$\/\+\% ]*$");
                samples = new string[]
                {
                    "CODE 93",
                    "ABCD+510609",
                    "ITEM+5489:1354",
                    "(ABC) 650165/12/531",
                    "2023/12/19:650A%054",
                };
                break;
            case Type.Telepen:
                //inputPattern = new Regex(@"^[\x00-\x7F]+$");
                samples = new string[]
                {
                    "1234567890",
                    "(10)12$3(05)04",
                    "(50)60(150)566",
                    "AE 12458 5V",
                    "16501(9809):504",
                };
                break;
            case Type.Fim:
                inputPattern = new Regex(@"^[A-Ea-e]$");
                break;
            case Type.Pharmacode:
                inputPattern = new Regex(@"^[0-9]{5,6}$");
                break;
            default:
                error = "Invalid barcode type";
                return -1;
        }//switch

        if (inputPattern is not null)
        {
            samples = new string[BarcodeSamples];
            for (int i = 0; i < BarcodeSamples; ++i)
            {
                var xeger = new Xeger(inputPattern.ToString());
                data = xeger.Generate();
                samples[i] = data;
            }
        }

        do
        {
            data = samples![(int)(Random.Shared.NextDouble() * BarcodeSamples)];
        } while (data == lastBarcode);
        lastBarcode = data;
        return 1;

    }
}
