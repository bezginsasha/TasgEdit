using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace TagsEdit
{
    class Controller
    {
        private string directory = "";
        private Dictionary<string, bool> properties;

        public Controller()
        {
            properties = new Dictionary<string, bool>();
            List<string> options;
            using (var reader = new StreamReader("config.txt", System.Text.Encoding.UTF8))
            {
                var list = new List<string>();
                while (!reader.EndOfStream)
                    list.Add(reader.ReadLine());
                directory = list[list.Count - 1];
                list.Remove(directory);
                options = list;
            }
            foreach (var i in options)
                if (i[0] == '/')
                    properties.Add(i.Substring(2, i.Length - 2), false);
                else
                    properties.Add(i, true);
        }

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
                    case "all":
                        All(words[1]);
                        break;
                    case "one":
                        One(words[1]);
                        break;
                    case "setdir":
                        SetDir(words[1]);
                        break;
                    case "add":
                        Add(words[1]);
                        break;
                    case "help":
                        Console.WriteLine("Help");
                        break;
                    case "config":
                        Config();
                        break;
                    case "exit":
                        return; // Это не точно, но здесь может появиться ошибка
                    default:
                        Console.WriteLine("Error");
                        break;
                }
            }
        }

        private void Add(string s)
        {
            using (var reader=new StreamReader("config.txt", Encoding.UTF8))
            {
                var list = new List<string>();
                while (!reader.EndOfStream)
                    if (reader.ReadLine() == s)
                    {
                        Console.WriteLine("Такой параметр уже есть");
                        return;
                    }                
            }            
        }

        private void Config()
        {
            List<string> options;
            string path;
            using (var reader = new StreamReader("config.txt", System.Text.Encoding.UTF8))
            {
                var list = new List<string>();
                while (!reader.EndOfStream)
                    list.Add(reader.ReadLine());
                path = list[list.Count - 1];
                list.Remove(path);
                options = list;
            }
            Console.Clear();
            Console.WriteLine("Список тэгов:");
            foreach (var i in options)
                Console.WriteLine(i);
            Console.WriteLine();
            Console.WriteLine("path\n{0}", path);
        }

        private void Smart(string s)
        {
            DirectoryInfo dir;
            if (directory == "")
                dir = new DirectoryInfo(s);
            else
                dir = new DirectoryInfo(directory);

            Console.Clear();
            Console.WriteLine("Умный режим");

            var music = new Music();
            var author=SetAuthor(music);
            SetAlbum(music);
            SetCover(music, dir);

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
            Console.WriteLine("Всё готово!");
        }

        private void SetDir(string s)
        {
            directory = s;
            Console.WriteLine("Директория задана");
        }

        private void All(string s)
        {
            Console.Clear();
            Console.Write("Режим редактирования папки");
            var name = Console.ReadLine();
            Console.Write("Альбом: ");
            var album = Console.ReadLine();
            //Console.Write("Исполнитель: ");
            //var author = Console.ReadLine();
            
            Console.Write("Путь обложки: ");
            var cover = Console.ReadLine();
            DirectoryInfo dir;
            if (directory == "")
                dir = new DirectoryInfo(s);
            else
                dir = new DirectoryInfo(directory);


            var music = new Music();
            music.SetName(name);
            //music.SetAuthor(author);
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

        public void SetCover(Music m, DirectoryInfo dir)
        {
            var cover = "";
            foreach (var i in dir.GetFiles())
                if ((new Regex(".jpg").IsMatch(i.Name)) || (new Regex(".png").IsMatch(i.Name)))
                {
                    cover = i.FullName;
                    break;
                }
            if (cover != "")
                m.SetCover(cover);
        }

        private string SetAuthor(Music m)
        {
            if (properties["author"])
            {
                Console.Write("Испонитель: ");
                var s = Console.ReadLine();
                m.SetAuthor(s);
                return s;
            }
            else
            {
                m.SetAuthor("");
                return "";
            }
        }

        private string SetAlbum(Music m)
        {
            if (properties["album"])
            {
                Console.Write("Альбом: ");
                var s = Console.ReadLine();
                m.SetAlbum(s);
                return s;
            }
            else
            {
                m.SetAlbum("");
                return "";
            }
        }

        private string SetName(Music m)
        {
            if (properties["name"])
            {
                Console.Write("Название: ");
                var s = Console.ReadLine();
                m.SetAlbum(s);
                return s;
            }
            else
            {
                m.SetAlbum("");
                return "";
            }
        }

        private string SetName(Music m, string s)
        {
            if (properties["name"])
                m.SetAlbum(s);
            else
                m.SetAlbum("");
            return s;
        }
    }
}
