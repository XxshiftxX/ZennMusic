using ZennMusic.Enums;

namespace ZennMusic.Model
{
    class Song
    {
        public string SongName { get; private set; }
        public string Requester { get; private set; }
        public RequestType Type { get; private set; }

        public Song(string songName, string requester, RequestType type)
        {
            SongName = songName;
            Requester = requester;
            Type = type;
        }
    }
}
