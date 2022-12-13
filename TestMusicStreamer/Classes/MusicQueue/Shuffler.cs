using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMusicStreamer.Classes.MusicQueue
{
    internal class Shuffler<T>
    {
        CachedRng rng;
        ObservableCollection<T> items;
        List<T> blacklist;

        protected List<T> whitelist
        {
            get
            {
                //blacklist being too full checking all happens here
                var output = items.Where(x => !blacklist.Contains(x)).ToList();
                if (output.Count < 1)
                {
                    if (blacklist.Count > 1)
                    {
                        blacklist.RemoveRange(0, output.Count - 1); //Remove all except last
                        output = items.Where(x => !blacklist.Contains(x)).ToList();
                    }
                    else
                    {
                        output = items.ToList(); //TODO: check if this is a copy or not (pretty sure it is)
                    }
                }
                return output;
            }
        }

        public Shuffler(ObservableCollection<T> items, CachedRng rng)
        {
            this.rng = rng;
            this.items = items;
            blacklist = new List<T>();
        }

        public T Peek()
        {
            return whitelist[rng.Peek() % whitelist.Count];
        }

        public T Next()
        {
            var output = whitelist[rng.Next() % whitelist.Count];
            blacklist.Add(output);
            return output;
        }

        public void Update(T item)
        {
            if (!this.blacklist.Contains(item))
            {
                this.blacklist.Add(item);
            }
        }
    }
}
