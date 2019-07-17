using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ZennMusic.Model;

namespace ZennMusic.Managers
{
    static class SongManager
    {
        public static readonly ObservableCollection<Song> SongList = new ObservableCollection<Song>();
        public static readonly ObservableCollection<Song> RemovedSongList = new ObservableCollection<Song>();

        public static bool IsRequestAvailable { get; set; }
    }
}
