using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapgeminiSurface.Model
{
    public class ContentItem
    {
        private string fileName;

        public enum Type { VideoItem, PictureItem }

        public string FileName 
        {
            get { return "Resources/" + fileName; }
            set { fileName = value; }
        }

        public Type ContentType { get; set; }

        public bool IsVideoItem { get { return ContentType == Type.VideoItem; } }

        public bool IsPictureItem { get { return ContentType == Type.PictureItem; } }
    }
}
