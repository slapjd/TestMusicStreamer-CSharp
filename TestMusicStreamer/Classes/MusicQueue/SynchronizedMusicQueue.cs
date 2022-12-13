using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMusicStreamer.Classes.DTOs;

namespace TestMusicStreamer.Classes.MusicQueue
{
    internal class SynchronizedMusicQueue : MusicQueue
    {
        protected readonly SocketIO socket;

        public event PropertyChangedEventHandler? PropertyChanged;

        public override ITrack currentTrack { 
            get => base.currentTrack;
            set
            {
                base.currentTrack = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(currentTrack)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(nextTrack)));
            }
        }

        public SynchronizedMusicQueue(SocketIO socket): base()
        {
            this.socket = socket;

            this._tracks.CollectionChanged += EmitOnSocket_QueueChanged;
            socket.On("queueUpdate", (response) =>
            {
                var newQueue = response.GetValue<ICollection<ITrack>>();
                _tracks.CollectionChanged -= EmitOnSocket_QueueChanged;
                _tracks.ReplaceAll(newQueue);
                _tracks.CollectionChanged += EmitOnSocket_QueueChanged;
            });

            PropertyChanged += EmitOnSocket_PropertyChanged;
            socket.On("changeTrack", (response) =>
            {
                PropertyChanged -= EmitOnSocket_PropertyChanged;

                var next = response.GetValue<ITrack>(1);
                var nextStack = response.GetValue<Stack<ITrack>>(2);
                if (nextStack != null && nextStack.Count > 1)
                {
                    //NextStack was sent to us, just use it as ours
                    this._nextStack = nextStack;
                }
                else if (next != null)
                {
                    //if nextstack wasn't sent, next *should* always be sent but whatever
                    //no nextstack received, so push the next track to the nextstack for UI purposes
                    _nextStack.Push(next);
                }

                var previousStack = response.GetValue<Stack<ITrack>>(3);
                if (previousStack != null && previousStack.Count > 1)
                {
                    //Previous stack was sent to us, just use it as ours
                    this._previousStack = previousStack;
                }

                //next track is prepared, change the track
                currentTrack = response.GetValue<ITrack>();

                PropertyChanged += EmitOnSocket_PropertyChanged; //Readd socket stuff for future changes
            });
        }

        private async void EmitOnSocket_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName== nameof(currentTrack))
            {
                await socket.EmitAsync("changeTrack", new object[] { currentTrack, this.Peek(), _nextStack, _previousStack });
            }
        }

        private async void EmitOnSocket_QueueChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await socket.EmitAsync("queueUpdate", new object[] {_tracks});
        }
    }
}
