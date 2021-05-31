using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VideoPlayer.Models.Interfaces;

namespace VideoPlayer.Models
{
    public class Folder : IFolder
    {
        [XmlIgnore]
        public IEnumerable<SimpleFolder> Folders { get; set; }

        public List<Video> Videos { get; set; }

        [XmlIgnore]
        public string Name { get; set; }

        public string Path { get; set; }
    }
}