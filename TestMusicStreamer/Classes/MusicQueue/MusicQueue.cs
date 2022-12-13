using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestMusicStreamer.Classes.BetterObservables;
using TestMusicStreamer.Classes.DTOs;
using Windows.Security.Cryptography.Core;

namespace TestMusicStreamer.Classes.MusicQueue
{
    internal class MusicQueue
    {
        private ITrack _currentTrack;

        protected CachedRng _rng;
        protected Shuffler<ITrack> _shuffler;
        protected Stack<ITrack> _nextStack;
        protected Stack<ITrack> _previousStack;
        protected readonly ExtendedObservableCollection<ITrack> _tracks;

        public bool shuffle;
        public virtual ITrack currentTrack
        {
            get { return _currentTrack; }
            set
            {
                _currentTrack = value;
                _shuffler.Update(value);
            }
        }
        public ITrack nextTrack
        {
            get => Peek();
        }

        protected ITrack InternalNext(bool peek)
        {
            if (_nextStack.Count > 0)
            {
                if (peek) return _nextStack.Peek();
                else return _nextStack.Pop();
            }
            else if (this._tracks.Count < 1) return new Track();
            else if (this.shuffle)
            {
                if (peek) return this._shuffler.Peek();
                else return this._shuffler.Next();
            }
            else return _tracks[_tracks.IndexOf(currentTrack) + 1];
        }

        public void Select(ITrack track)
        {
            currentTrack = track;
        }

        public ITrack Peek()
        {
            return InternalNext(true);
        }

        public ITrack Next()
        {
            this.Select(InternalNext(false));
            _previousStack.Push(currentTrack);
            return currentTrack;
        }

        public ITrack Previous()
        {
            if (_previousStack.Count > 0) this.Select(_previousStack.Pop());
            else if (this._tracks.Count < 1) this.Select(new Track());
            else if (this.shuffle) this.Select(this._shuffler.Next()); //No previous songs to pull from, so just shuffle a random one
            else this.Select(_tracks[(_tracks.IndexOf(currentTrack) + _tracks.Count - 1) % _tracks.Count]);

            return currentTrack;
        }


        public MusicQueue()
        {
            _rng= new CachedRng();
            _tracks = new ExtendedObservableCollection<ITrack>();
            _previousStack = new Stack<ITrack>();
            _nextStack = new Stack<ITrack>();
            _currentTrack = new Track();
            shuffle = false;
        }
    }
}
