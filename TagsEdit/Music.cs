using TagLib;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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

        //  ОБЛОЖКА
        public void SetCover(bool b, System.IO.DirectoryInfo dir)
        {
            if (b)
            {
                var cover = "";
                foreach (var i in dir.GetFiles())
                    if ((new Regex(".jpg").IsMatch(i.Name)) || (new Regex(".png").IsMatch(i.Name)))
                    {
                        cover = i.FullName;
                        break;
                    }
                if (cover != "")
                    this.cover = cover;
            }
            else
                cover = "";
        }
        public void SetCover(System.IO.DirectoryInfo dir, bool b)
        {
            if (b)
            {
                Console.Write("Название картинки: ");
                var name = Console.ReadLine();
                var temp = name;
                var res = "";
                do
                {
                    foreach (var i in dir.GetFiles())
                        if (new Regex(Screen(name.ToLower())).IsMatch(Screen(i.Name.ToLower())))
                        {
                            Console.WriteLine("Найдена картинка, полное имя: " + i.Name);
                            name = i.FullName;
                            Console.WriteLine("Правильно?");
                            res = Console.ReadLine();
                            break;
                        }
                    if (temp == name)
                    {
                        Console.WriteLine("Картинка не найдена");
                        return;
                    }
                }
                while (res != "yes");

                this.cover = name;
            }
            else
                this.cover = "";
        }

        //  АВТОР
        public string SetAuthor(bool b)
        {
            var s = "";
            if (b)
            {
                Console.Write("Испонитель: ");
                s = Console.ReadLine();
            }
            this.author = s;
            return s;
        }

        //  АЛЬБОМ
        public string SetAlbum(bool b)
        {
            var s = "";
            if (b)
            {
                Console.Write("Альбом: ");
                s = Console.ReadLine();
            }
            this.album = s;
            return s;
        }

        //  ГОД
        public int SetYear(bool b)
        {
            var s = "0";
            if (b)
            {
                Console.Write("Год:  ");
                s = Console.ReadLine();
                if (!new Regex(@"^[1|2][0|9|8|7][0-9]{2}$").IsMatch(s))
                    return -1;
            }
            this.year = Int32.Parse(s);
            return Int32.Parse(s);
        }

        //  НАЗВАНИЕ
        public string SetName(string s, bool b)
        {
            if (!b)
                s = "";
            this.name = s;
            return s;
        }
        public string SetName(bool b)
        {
            var s = "";
            if (b)
            {
                Console.Write("Название: ");
                s = Console.ReadLine();
            }
            this.name = s;
            return s;
        }

        // Задаётся путь, то есть пересоздаётся новый объект музыки
        public void SetPath(string s)
        {
            music = File.Create(s);
        }

        public void Save()
        {
            music.Tag.Title = this.name;
            music.Tag.Year = (uint)this.year;
            music.Tag.Album = this.album;
            music.Tag.Performers = new string[] { this.author };
            if (this.cover == "") music.Tag.Pictures = new Picture[0]; else music.Tag.Pictures = new Picture[] { new Picture(this.cover) };

            music.Save();
        }

        //Меняет \ на /
        private string Screen(string s)
        {
            var list = new List<char>();
            for (var i = 0; i < s.Length; i++)
                if (s[i] == '\\')
                    list.Add('/');
                else
                    list.Add(s[i]);
            return new string(list.ToArray());
        }
    }
}
