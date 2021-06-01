using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using VideoPlayer.Models;

namespace VideoPlayer.Services
{
    public class VideoDurationXmlSaver
    {
        private readonly string _path;
        private readonly XDocument _xDocument;
        private readonly XElement _xRoot;
        private readonly long _maxSize;

        public VideoDurationXmlSaver(string path, long maxSize)
        {
            _path = path;
            try
            {
                _xDocument = XDocument.Load(_path);
            }
            catch (Exception)
            {
                RestoreDefault();
                _xDocument = XDocument.Load(_path);
            }
            
            _xRoot = _xDocument.Root;
            _maxSize = maxSize;

            if (_xRoot == null || new FileInfo(_path).Length / 1024 > maxSize) RestoreDefault();
        }

        public void SaveToXml(Folder folder)
        {
            if (folder.Videos == null) return;
            if (folder.Videos.Count <= 0) return;


            if (!FolderExist(folder.Path))
            {
                AddFolder(folder);
            }
            else
            {
                Replace(folder.Videos.Where(v => v.CurrentPosition != null), folder.Path);
            }

            if(new FileInfo(_path).Length / 1024 > _maxSize) RestoreDefault();
        }

        public TimeSpan? GetDurationFromXml(string path, string fileName)
        {
            string folderPath = Path.GetDirectoryName(path);
            XElement xFolder = FindNode(folderPath);

            var xVideos = xFolder?.Element("Videos")?.Elements();

            if (xVideos == null) return null;

            foreach (var xElement in xVideos)
            {
                var xVideoName = xElement.Element("Name")?.Value;
                var xVideoDuration = xElement.Element("Duration")?.Value;

                if(xVideoName == null || xVideoDuration == null || xVideoName != fileName) continue;

                return TimeSpan.TryParse(xVideoDuration, out var duration) ? duration : (TimeSpan?) null;
            }

            return null;
        }

        public void Replace(IEnumerable<Video> videos, string path)
        {
            XElement folder = FindNode(path);
            XElement xVideos = folder.Element("Videos");
            var enumerable = videos as Video[] ?? videos.ToArray();

            var existedVideos = (from video in enumerable
                join xVideo in xVideos?.Elements() on video.Name equals xVideo.Element("Name")?.Value
                select new {xDuration = xVideo.Element("Duration"), Video = video}).ToList();

            foreach (var existedVideo in existedVideos)
            {
                if(existedVideo.xDuration == null) continue;
                existedVideo.xDuration.Value = existedVideo.Video.CurrentPosition.ToString();
            }

            var notExistedVideos = enumerable.Where(v => existedVideos.Find(vi => vi.Video.Equals(v)) == null);

            xVideos?.Add(notExistedVideos.Select(v => new XElement("Video",
                new XElement("Name", v.Name),
                new XElement("Duration", v.CurrentPosition.ToString()))));

            _xDocument.Save(_path);
        } 

        public void AddFolder(Folder currentFolder)
        {

            XElement xFolder = new XElement("Folder");
            XElement xPath = new XElement("Path") {Value = currentFolder.Path};
            XElement xVideos = new XElement("Videos");

            foreach (Video currentFolderVideo in currentFolder.Videos)
            {
                if (currentFolderVideo.CurrentPosition == null) continue;

                XElement xVideo = new XElement("Video");
                XElement xName = new XElement("Name") { Value = currentFolderVideo.Name };
                XElement xDuration = new XElement("Duration") { Value = currentFolderVideo.CurrentPosition.ToString() };

                xVideo.Add(xName, xDuration);

                xVideos.Add(xVideo);
            }

            xFolder.Add(xPath, xVideos);

            _xRoot.Add(xFolder);

            _xDocument.Save(_path);
        }

        public bool FolderExist(string path)
        {
            var penCategory = from folder in _xDocument.Element("Folders")?.Elements()
                where folder.Element("Path")?.Value == path
                select folder;

            return penCategory.Any();
        }

        public XElement FindNode(string path)
        {
            var penCategory = (from folder in _xDocument.Element("Folders")?.Elements()
                where folder.Element("Path")?.Value == path
                select folder).ToList();

            if (penCategory.Count <= 0) return null;

            return penCategory.First();
        }

        private void RestoreDefault()
        {
            string defaultXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" + 
                                "<Folders xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                                "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  \r\n</Folders>";

            File.WriteAllText(_path, defaultXml);
        }
    }
}
