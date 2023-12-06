using System.IO;

namespace Appeon.ComponentsApp.FilesystemTools
{
    public class PathTools
    {
        public static string? ChangeFileExtension(string path, string newExt) => Path.ChangeExtension(path, newExt);

        public static string GetFileName(string path) => Path.GetFileName(path);
    }
}
