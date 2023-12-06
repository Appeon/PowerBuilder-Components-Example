using System;
using System.IO;
using System.Linq;

namespace Appeon.ComponentsApp.FilesystemTools
{
    public class DirectoryTools
    {
        public static int GetContents(string path, string pattern, out string[]? files, out string? error)
        {
            files = null;
            error = null;

            try
            {
                var res = Directory.EnumerateFiles(path, pattern);

                files = res.ToArray();
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }
    }
}