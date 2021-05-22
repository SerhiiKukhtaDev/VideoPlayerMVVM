using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VideoPlayer.Models;
using VideoPlayer.Utils;

namespace VideoPlayer.Services
{
    public class VideoService
    {
        public static IEnumerable<Video> GetVideos(string path, params string[] exts)
        {
            IEnumerable<string> videos = Directory
                .EnumerateFiles(path, "*.*")
                .Where(file => exts.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)));

            List<string> list = new List<string>(videos);
            list.Sort(new NaturalComparer());

            return list.Select(str => new Video()
            {
                Path = new Uri(str),
                CurrentPosition = default,
                Name = Path.GetFileName(str)
            });
        }
    }
}