using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TagsEdit
{
    class Controller
    {
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
            DirectoryInfo dir = new DirectoryInfo(s);


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

            var music = new Music(s);
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
    }
}
