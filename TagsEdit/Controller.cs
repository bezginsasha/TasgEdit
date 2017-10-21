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
                    case "remove":
                        Remove(words[1]);
                        break;
                    case "help":
                        Console.WriteLine("Help");
                        break;
                    case "config":
                        Config();
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Error");
                        break;
                }
            }
        }

        private void Add(string s)
        {
            //var list = new List<string>();
            //using (var reader = new StreamReader("config.txt", Encoding.UTF8))
            //{
            //    while (!reader.EndOfStream)
            //        list.Add(reader.ReadLine());
            //}
            //for (var i = 0; i < list.Count; i++)
            //    if (list[i] == "//" + s)
            //    {
            //        list[i] = list[i].Substring(2, list[i].Length - 3);
            //        Console.Clear();
            //        Console.WriteLine("Готово!");
            //        using (var writer = new StreamWriter("config.txt", false, Encoding.UTF8))
            //        {
            //            foreach (var j in list)
            //                writer.WriteLine(j);
            //        }
            //        return;
            //    }
            //Console.Clear();
            //Console.WriteLine("Свойство не найдено");
        }

        private void Remove(string s)
        {
            //var list = new List<string>();
            //using (var reader = new StreamReader("config.txt", Encoding.UTF8))
            //{
            //    while (!reader.EndOfStream)
            //        list.Add(reader.ReadLine());
            //}
            //for (var i = 0; i < list.Count; i++)
            //{
            //    if (list[i] == s)
            //    {
            //        list[i] = "//" + list[i];
            //        Console.Clear();
            //        Console.WriteLine("Всё готово!");
            //        using (var writer = new StreamWriter("config.txt", false, Encoding.UTF8))
            //        {
            //            foreach (var j in list)
            //                writer.WriteLine(j);
            //        }
            //        return;
            //    }
            //    if (list[i] == "//" + s)
            //    {
            //        Console.Clear();
            //        Console.WriteLine("Свойство и так удалено");
            //        return;
            //    }
            //}
            //Console.Clear();
            //Console.WriteLine("Свойство не найдено");
        }

        private void Config()
        {
            Console.Clear();

            //Получение списка свойств
            var list = new List<string>();
            using (var reader = new StreamReader("config.txt", Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                    list.Add(reader.ReadLine());
            }
            var path = list[list.Count - 1];
            list.Remove(path);
            list[0] += "<";
            foreach (var i in list)
                Console.WriteLine(i);

            //Главный цикл
            var current = 0;
            while (1 < 2)
            {
                //Перемещение курсора
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.DownArrow)
                    if (current < list.Count - 1)
                    {
                        current++;
                        list[current] += "<";
                        if (current > 0)
                            if (list[current - 1][list[current - 1].Length - 1] == '<')
                                list[current - 1] = list[current - 1].Substring(0, list[current - 1].Length - 1);
                    }
                if (key.Key == ConsoleKey.UpArrow)
                    if (current > 0)
                    {
                        current--;
                        list[current] += "<";
                        if (current < list.Count - 1)
                            if (list[current + 1][list[current + 1].Length - 1] == '<')
                                list[current + 1] = list[current + 1].Substring(0, list[current + 1].Length - 1);
                    }

                //Комментируется свойство
                if (key.Key == ConsoleKey.RightArrow)
                    if (list[current][0] != '/')
                        list[current] = "//" + list[current];

                //Разкомментируется свойство
                if (key.Key == ConsoleKey.LeftArrow)
                    if (list[current][0] == '/')
                    {
                        list[current] = list[current].Substring(2, list[current].Length - 3);
                        list[current] += "<";
                    }

                //Выход
                if (key.Key == ConsoleKey.Enter)
                {
                    list[current] = list[current].Substring(0, list[current].Length - 1);
                    break;
                }

                //Обновление
                Console.Clear();
                foreach (var i in list)
                    Console.WriteLine(i);
            }

            //Запись в файл и завершение
            list.Add(path);
            using (var writer = new StreamWriter("config.txt", false, Encoding.UTF8))
            {
                foreach (var i in list)
                    writer.WriteLine(i);
            }
            Console.Clear();
            Console.WriteLine("Всё готово!");
        }

        private void Smart(string s)
        {
            //Дирректория
            var dir = GetDir(s);

            //Консоль
            Console.Clear();
            Console.WriteLine("Умный режим");

            //Задаётся основа тэгов
            var music = new Music();
            var author = SetAuthor(music);
            SetAlbum(music);
            SetYear(music);
            SetCover(music, dir);

            //Задаются индиидуальные теги и сохраняются изменения
            foreach (var i in dir.GetFiles())
                if (new Regex(".mp3").IsMatch(i.Name))
                {
                    var name = SetName(music, GetName(i.Name));
                    music.SetPath(i.FullName);
                    music.Save();

                    var newPath = i.Directory + "/" + author + " - " + name + ".mp3";

                    File.Move(i.FullName, newPath);
                }

            //Консоль
            Console.Clear();
            Console.WriteLine("Всё готово!");
        }

        private void SetDir(string s)
        {
            var list = new List<string>();
            using (var reader = new StreamReader("config.txt", Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                    list.Add(reader.ReadLine());
            }
            directory = s;
            list[list.Count - 1] = s;
            using (var writer = new StreamWriter("config.txt", false, Encoding.UTF8))
            {
                foreach (var i in list)
                    writer.WriteLine(i);
            }
            Console.Clear();
            Console.WriteLine("Директория задана");
        }

        private void All(string s)
        {
            //Дирректория
            var dir = GetDir(s);

            //Консоль
            Console.Clear();
            Console.WriteLine("Режим редактирования папки, общие тэги:");

            //Задаются общие тэги
            var music = new Music();
            var author = SetAuthor(music);
            SetAlbum(music);
            SetYear(music);
            SetCover(music, dir);

            //Задаются индивидуальные тэги
            Console.WriteLine("\nНазвания песен:");
            foreach (var i in dir.GetFiles())
            {
                var l = i.Name.Length;
                if (new Regex(".mp3").IsMatch(i.Name))
                {
                    Console.WriteLine("\nФайл: {0}", i.Name);
                    var name = SetName(music);
                    music.SetPath(i.FullName);
                    music.Save();

                    var newPath = i.Directory + "/" + author + " - " + name + ".mp3";

                    try
                    {
                        File.Move(i.FullName, newPath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Вы хотите сделать 2 одинаковых файла. Точное исключение: {0}", e);
                    }
                }
            }

            //Консоль
            Console.Clear();
            Console.WriteLine("Всё готово!");
        }

        private void One(string s)
        {
            //Поиск файла
            Console.Clear();
            var dir = GetDir(s);
            Console.Write("Название файла: ");
            var name = Console.ReadLine();
            var temp = name;
            var res = "";
            do
            {
                foreach (var i in dir.GetFiles())
                {
                    //var sss = ;
                    if (new Regex(Screen(name.ToLower())).IsMatch(Screen(i.Name.ToLower())))
                    {
                        Console.WriteLine("Найден файл, полное имя: " + i.Name);
                        name = i.FullName;
                        Console.WriteLine("Правильно?");
                        res = Console.ReadLine();
                        break;
                    }
                }
                if (temp == name)
                {
                    Console.WriteLine("Файл не найден");
                    return;
                }
            }
            //while ((new Regex(res.ToLower()).IsMatch("д") || (new Regex(res.ToLower()).IsMatch("y"))));
            while (res!="yes");

            //Редактирование тэгов
            var music = new Music();
            var musicName = SetName(music);
            var author = SetAuthor(music);
            SetAlbum(music);
            SetYear(music);
            SetCover(music, name, s);
            music.SetPath(name);
            music.Save();

            var newPath = dir.FullName + "/" + author + " - " + musicName + ".mp3";

            File.Move(name, newPath);

            Console.Clear();
            Console.WriteLine("Всё готово!");
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

        //Возвращает название песни отбрасывая всё лишнее в имени файла, используется в режиме смарт
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

        public void SetCover(Music m, string fileName, string s)
        {
            //Console.Clear();
            var dir = GetDir(s);
            Console.Write("Название картинки: ");
            var name = Console.ReadLine();
            var temp = name;
            var res = "";
            do
            {
                foreach (var i in dir.GetFiles())
                    if (new Regex(name.ToLower()).IsMatch(Screen(i.Name.ToLower())))
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
            //while ((new Regex(res.ToLower()).IsMatch("д") || (new Regex(res.ToLower()).IsMatch("y"))));
            while (res!="yes") ;

            m.SetCover(name);
        }

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
                m.SetName(s);
                return s;
            }
            else
            {
                m.SetName("");
                return "";
            }
        }

        private string SetName(Music m, string s)
        {
            if (properties["name"])
                m.SetName(s);
            else
                m.SetName("");
            return s;
        }

        private int SetYear(Music m)
        {
            Console.Write("Год:  ");
            var s = Console.ReadLine();
            if (properties["year"])
                m.SetYear(s);
            else
                m.SetYear("");
            return Int32.Parse(s);
        }

        private DirectoryInfo GetDir(string s)
        {
            return directory == "" ? new DirectoryInfo(s) : new DirectoryInfo(directory);
        }
    }
}
