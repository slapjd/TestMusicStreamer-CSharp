using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Windows.Storage.Streams;

namespace TestMusicStreamer.Classes.MusicQueue
{
    internal class CachedRng: Random
    {
        protected int? cachedInt;
        public override int Next()
        {
            if (cachedInt != null)
            {
                int output = cachedInt.Value;
                cachedInt = null;
                return output;
            } else
            {
                return base.Next();
            }
        }

        public int Peek()
        {
            if (cachedInt == null)
            {
                cachedInt = base.Next();
            }
            return cachedInt.Value;
        }
    }
}
