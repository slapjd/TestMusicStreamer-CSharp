using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMusicStreamer.Classes.DTOs
{
    internal class Album : IAlbum
    {
        public string title { get; set; }

        public Album(string title = "Unknown Title")
        {
            this.title = title;
        }
    }
}
