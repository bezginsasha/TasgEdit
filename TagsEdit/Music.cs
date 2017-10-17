using TagLib;

namespace TagsEdit
{
    class Music
    {
        private string name = "";
        private string author = "";
        private string album = "";
        private string cover = "";
        private int year = 1900;
        private File music;

        public Music(string s)
        {
            music = File.Create(s);           
        }
        public Music()
        {
        }

        public void SetName(string s)
        {
            name = s;
        }

        public void SetAuthor(string s)
        {
            author = s;
        }

        public void SetAlbum(string s)
        {
            album = s;
        }

        public void SetPath(string s)
        {
            music = File.Create(s);
        }

        public void SetCover(string s)
        {
            cover = s;
        }

        public void SetYear(string s)
        {
            bool res = System.Int32.TryParse(s, out year);
        }

        public void Save()
        {
            Picture[] pictures = new Picture[1];
            pictures[0] = new Picture(cover);
            music.Tag.Pictures = pictures;

            music.Tag.Album = album;

            string[] performers = new string[1];
            performers[0] = author;
            music.Tag.Performers = performers;

            music.Tag.Title = name;

            music.Tag.Year = (uint)year;

            music.Save();
        }
    }
}
