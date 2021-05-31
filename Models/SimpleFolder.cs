using VideoPlayer.Models.Interfaces;

namespace VideoPlayer.Models
{
    public class SimpleFolder : IFolder
    {
        public string Path { get; set; }

        public string Name { get; set; }
    }
}
