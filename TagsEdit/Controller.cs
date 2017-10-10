using System;
using System.Collections.Generic;

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
                switch (words[0])
                {
                    case "all":
                        Console.WriteLine("All");
                        break;
                    case "one":
                        One();
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

        private void One()
        {
            var music = new Music(@"D:\MusicTest\neval.mp3");
            music.SetName("Неваляшка");
            music.SetAuthor("Oxxxymiron");
            music.SetCover(@"D:\MusicTest\cover.jpg");
            music.SetAlbum("Долгий путь домой");
            music.Save();
        }

        private List<string> SelectWords(string s)
        {
            var words = new List<string>();
            s += ' ';
            var start = 0;
            var end = 0;
            for (var i = 0; i < s.Length; i++)
                if (s[i]==' ')
                {
                    end = i - 1;
                    words.Add(s.Substring(start, end - start + 1).ToLower());
                    start = i + 1;
                }
            return words;
        }
    }
}
