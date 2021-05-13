using System;

namespace VideoPlayer.Models
{
    public class Video
    {
        public string Name { get; set; }

        public Uri Path { get; set; }

        public string CurrentPosition { get; set; }
    }
}
