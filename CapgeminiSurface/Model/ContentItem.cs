using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapgeminiSurface.Model
{
    public class ContentItem
    {
        private string name;

        private string fileName;

        public enum Type { VideoItem, PictureItem, LinkItem, AgendaItem, VisitItem }

        public string FileName 
        {
            get { return "Resources/" + fileName; }
            set { fileName = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #region visit card
        private string job;
        public string Job
        {
            get { return job; }
            set { job = value; }
        }

        private string tlf;
        public string Tlf
        {
            get { return tlf; }
            set { tlf = value; }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string FullName
        {
            get { return Name + " " + fileName; } 
        }
        #endregion

        public Type ContentType { get; set; }

        public bool IsVideoItem { get { return ContentType == Type.VideoItem; } }

        public bool IsPictureItem { get { return ContentType == Type.PictureItem; } }

        public bool IsLinkItem { get { return ContentType == Type.LinkItem; } }

        public bool IsAgendaItem { get { return ContentType == Type.AgendaItem; } }

        public bool IsVisitItem { get { return ContentType == Type.VisitItem; } }
    }
}
