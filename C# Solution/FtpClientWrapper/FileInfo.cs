using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appeon.ComponentsApp.FtpClientWrapper
{
    public class FileInfo
    {
        public FileType Type { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public long Size { get; set; }
        public string? Permissions { get; set; }
    }

    public enum FileType
    {
        Directory = 1,
        File = 2,
    }
}
