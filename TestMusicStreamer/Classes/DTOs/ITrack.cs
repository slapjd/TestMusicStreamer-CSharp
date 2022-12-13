using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMusicStreamer.Classes.DTOs
{
    internal interface ITrack
    {
        int id { get; set; }
        string title { get; set; }
        string artist { get; set; }
        IAlbum album { get; set; }
    }
}
