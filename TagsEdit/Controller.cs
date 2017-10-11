using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TagsEdit
{
    class Controller
    {
        private string directory = "";

        public void Start()
        {
            Console.WriteLine("Добро пожаловать в TagsEdit");
            while (true)
            {
                var words = SelectWords(Console.ReadLine());
                if (words.Count == 1)
                    words.Add("");
                switch (words[0])
                {
                    case "smart":
                        Smart(words[1]);
                        break;
                    case "setdir":
                        SetDir(words[1]);
                        break;
                    case "all":
                        All(words[1]);
                        break;
                    case "one":
                        One(words[1]);
                        break;
                    case "help":
                        Console.WriteLine("Help");
                        break;
                    case "config":
                        Console.WriteLine("Config");
                        break;
                    case "exit":
                        return; // Это не точно, но здесь может появиться ошибка
                    default:
                        Console.WriteLine("Error");
                        break;
                }
            }
        }

        private void Smart(string s)
        {
            DirectoryInfo dir;
            if (directory == "")
                dir = new DirectoryInfo(s);
            else
                dir = new DirectoryInfo(directory);

            Console.Clear();
            Console.Write("Умный режим\nИсполнитель: ");
            string author = Console.ReadLine();
            Console.Write("Альбом: ");
            string album = Console.ReadLine();

            var cover = "";
            foreach (var i in dir.GetFiles())
                if ((new Regex(".jpg").IsMatch(i.Name)) || (new Regex(".png").IsMatch(i.Name)))
                {
                    cover = i.FullName;
                    break;
                }

            var music = new Music();
            music.SetAuthor(author);
            music.SetAlbum(album);
            if (cover != "")
                music.SetCover(cover);
            else
                return;

            foreach (var i in dir.GetFiles())
                if (new Regex(".mp3").IsMatch(i.Name))
                {
                    var name = GetName(i.Name);
                    music.SetName(name);
                    music.SetPath(i.FullName);
                    music.Save();

                    var newPath = i.Directory + "/" + author + " - " + name + ".mp3";

                    File.Move(i.FullName, newPath);
                }

            //foreach (var i in new DirectoryInfo("D:/musictest").GetFiles())
            //    Console.WriteLine(GetName(i.Name));
        }

        private void SetDir(string s)
        {
            directory = s;
        }

        private void All(string s)
        {
            Console.Clear();
            Console.Write("Режим редактирования папки\nНазвание: ");
            var name = Console.ReadLine();
            Console.Write("Альбом: ");
            var album = Console.ReadLine();
            Console.Write("Исполнитель: ");
            var author = Console.ReadLine();
            Console.Write("Путь обложки: ");
            var cover = Console.ReadLine();
            DirectoryInfo dir;
            if (directory == "")
                dir = new DirectoryInfo(s);
            else
                dir = new DirectoryInfo(directory);


            var music = new Music();
            music.SetName(name);
            music.SetAuthor(author);
            music.SetCover(cover);
            music.SetAlbum(album);
            foreach (var i in dir.GetFiles())
                if (new Regex(".mp3").IsMatch(i.Name))
                {
                    music.SetPath(i.FullName);
                    music.Save();
                }

            Console.Clear();
            Console.WriteLine("Всё готово!");
        }

        private void One(string s)
        {
            Console.Clear();
            Console.Write("Режим редактирования одной песни\nНазвание: ");
            var name = Console.ReadLine();
            Console.Write("Альбом: ");
            var album = Console.ReadLine();
            Console.Write("Исполнитель: ");
            var author = Console.ReadLine();
            Console.Write("Путь обложки: ");
            var cover = Console.ReadLine();

            Music music;
            if (directory == "")
                music = new Music(s);
            else
                music = new Music(directory);
            music.SetName(name);
            music.SetAuthor(author);
            music.SetCover(cover);
            music.SetAlbum(album);
            music.Save();

            Console.Clear();
            Console.WriteLine("Готово!");
        }

        private List<string> SelectWords(string s)
        {
            var words = new List<string>();
            s += ' ';
            var start = 0;
            var end = 0;
            for (var i = 0; i < s.Length; i++)
                if (s[i] == ' ')
                {
                    end = i - 1;
                    words.Add(s.Substring(start, end - start + 1).ToLower());
                    start = i + 1;
                }
            return words;
        }

        private string GetName(string s)
        {
            var start = 0;
            var end = 0;
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == '—')
                    start = i + 1;
                if (s[i] == '-')
                    start = i + 1;
                if (s[i] == '.')
                    if (s[i + 1] == 'm')
                        if (s[i + 2] == 'p')
                            if (s[i + 3] == '3')
                                end = i;
            }
            var res = s.Substring(start, end - start);

            if (res.Length > 1)
                while ((res[0] == ' ') || (res[res.Length - 1] == ' '))
                {
                    if (res[0] == ' ')
                        res = res.Substring(1, res.Length - 1);
                    if (res[res.Length - 1] == ' ')
                        res = res.Substring(0, res.Length - 1);
                }

            return res;
        }
    }
}
