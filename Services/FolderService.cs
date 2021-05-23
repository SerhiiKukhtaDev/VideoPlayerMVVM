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
            return Directory.EnumerateDirectories(path)
                .Select(str => new SimpleFolder()
                {
                    Name = Path.GetFileName(str),
                    Path = str
                });
        } 
    }
}
