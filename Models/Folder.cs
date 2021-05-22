using System.Collections.Generic;

namespace VideoPlayer.Models
{
    public class Folder
    {
        public IEnumerable<Folder> Folders { get; set; }

        public IEnumerable<Video> Videos { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }
    }
}