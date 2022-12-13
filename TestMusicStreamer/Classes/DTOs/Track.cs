using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMusicStreamer.Classes.DTOs
{
    internal class Track : ITrack
    {
        public int id { get; set; }
        public string title { get; set; }
        public string artist { get; set; }
        public IAlbum album { get; set; }

        public Track(int id = -1,
            string title = "Unknown Title",
            string artist = "Unknown Artist",
            IAlbum album = null)
        {
            this.id = id;
            this.title = title;
            this.artist = artist;
            if (album == null)
            {
                this.album = new Album();
            }
            else
            {
                this.album = album;
            }
        }
    }
}
