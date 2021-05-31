using System.Collections.Generic;
using System.IO;
using System.Linq;
using VideoPlayer.Models;

namespace VideoPlayer.Services
{
    public class FolderService
    {
        public static IEnumerable<SimpleFolder> GetFolders(string path)
        {
            return from str in Directory.EnumerateDirectories(path)
                   where FileAttributes.System != (File.GetAttributes(str) & FileAttributes.System) &&
                         FileAttributes.Hidden != (File.GetAttributes(str) & FileAttributes.Hidden)
                   select new SimpleFolder { Name = Path.GetFileName(str), Path = str };

        }
    }
}
