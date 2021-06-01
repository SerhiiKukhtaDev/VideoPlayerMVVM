using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VideoPlayer.Models;
using VideoPlayer.Utils;
using Application = System.Windows.Application;

namespace VideoPlayer.Services
{
    public class VideoService
    {
        private static readonly string[] Exts = {"mp4", "mkv"};

        public static List<Video> GetVideos(string path)
        {
            List<Video> videos = Directory
                .EnumerateFiles(path, "*.*")
                .Where(file => Exts.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(str => str, new NaturalComparer())
                .Select(str => new Video
                {
                    Path = new Uri(str),
                    CurrentPosition = default,
                    Name = Path.GetFileName(str)
                })
                .ToList();

            return videos;
        }
    }
}
