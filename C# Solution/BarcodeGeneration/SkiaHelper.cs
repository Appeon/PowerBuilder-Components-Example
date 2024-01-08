using System.Runtime.InteropServices;

namespace Appeon.ComponentsApp.BarcodeGeneration
{
    public class SkiaHelper
    {
        public static bool LoadNativeLibrary(out string? error)
        {
            error = null;
            var libSkiaSharpPath = Path.GetDirectoryName(typeof(SkiaHelper).Assembly.Location);
            string runtime;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    throw new InvalidOperationException("Unsupported OS");
                default:
                    runtime = IntPtr.Size == 4 ? "win-x86" : "win-x64";
                    libSkiaSharpPath = Path.Combine(libSkiaSharpPath, "runtimes", runtime, "native",
                        "libSkiaSharp.dll");
                    break;
            }

            // for .net core
            var lib = NativeLibrary.Load(libSkiaSharpPath);
            if (lib == IntPtr.Zero)
            {
                error = $"Loading native lib from '{libSkiaSharpPath}' failed";
                return false;
            }
            return true;
        }
    }
}
