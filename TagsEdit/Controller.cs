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
            UpdateConfig();
        }

        private void UpdateConfig()
        {
            properties = new Dictionary<string, bool>();
            var options = new List<string>();
            using (var reader = new StreamReader("config.txt", System.Text.Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                    options.Add(reader.ReadLine());
                directory = options[options.Count - 1];
                options.Remove(directory);
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
                    case "showdir":
                        Console.Clear();
                        Console.WriteLine(directory);
                        break;
                    case "help":
                        Help();
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

        private void Help()
        {
            Console.Clear();
            Console.WriteLine("Список основных команд:\n");
            Console.WriteLine("one - открывает режим редактирования одной песни. Сначала нужно ввести название файла, можно не целиком, а его часть, нажать Enter и вам будет предложен первый вариант из найденных (если их несколько). Если это он, введите «yes» и продолжите редактирование, если файл не найден или это найден не тот (для этого нужно ввести «no»), всё надо начать заново. Если всё хорошо, введите тэги, с обложкой нужно будет повторить то же, что с поиском файла\n");
            Console.WriteLine("all –режим редактирование альбома. Сначала нужно ввести тэги, общие для всех песен альбома, включая обложку. А потом будет перебираться каждый файл в папке, и вам нужно будет ввести названия песен для них\n");
            Console.WriteLine("smart – то же, что и all, только обложка выбирается как первая попавшаяся картинка (в идеале она должна быть одна), а название песни берётся из названия файла, то, что после тире (если оно есть) и перед .mp3. Потому что музыка обычно записывается как «исполнитель – название.mp3»\n");
            Console.WriteLine("Важное замечаение – в любом из режимов файл переименовывается так, как показано в предыдущем пункте\n");
            Console.WriteLine("setdir – задаётся папка, в которой и будет всё происходить\n");
            Console.WriteLine("showdir – показывает папку\n");
            Console.WriteLine("config – режим изменения тэгов. Чтобы перемещаться нажимайте на стрелочки на клавиатуре «вверх» и «вниз». Чтобы закомментировать тэг, нажмите на стрелочку «вправо», разкомментировать – «влево». Закомментированные тэги не будут отображаться при редактировании\n");
            Console.WriteLine("help – открыть справку");
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
            UpdateConfig();
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
            var author = music.SetAuthor(properties["author"]);
            music.SetAlbum(properties["album"]);
            if (music.SetYear(properties["year"]) == -1)
            {
                Console.WriteLine("Ошибка, введён некоректный год");
                return;
            }
            music.SetCover(properties["cover"], dir);

            //Задаются индиидуальные теги и сохраняются изменения
            foreach (var i in dir.GetFiles())
                if (new Regex(".mp3").IsMatch(i.Name))
                {
                    var name = music.SetName(GetName(i.Name), properties["name"]);
                    music.SetPath(i.FullName);
                    music.Save();

                    var newPath = i.Directory + "/" + author + " - " + name + ".mp3";

                    File.Move(i.FullName, newPath);
                }

            //Консоль
            Console.Clear();
            Console.WriteLine("Всё готово!");
        }

        //Задаётся дирректория, в которой будут меняться файлы
        private void SetDir(string s)
        {
            if (!new Regex(@"\w:[/|\\](\w*[\\|/])*\w*$", RegexOptions.Compiled | RegexOptions.IgnoreCase).IsMatch(s))
            {
                Console.WriteLine("Вы ввели некорректный путь");
                return;
            }
            var list = new List<string>();
            using (var reader = new StreamReader("config.txt", Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                    list.Add(reader.ReadLine());
            }
            list[list.Count - 1] = s;
            using (var writer = new StreamWriter("config.txt", false, Encoding.UTF8))
            {
                foreach (var i in list)
                    writer.WriteLine(i);
            }
            UpdateConfig();
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
            var author = music.SetAuthor(properties["author"]);
            music.SetAlbum(properties["album"]);
            music.SetYear(properties["year"]);
            music.SetCover(dir, properties["cover"]);

            //Задаются индивидуальные тэги
            Console.WriteLine("\nНазвания песен:");
            foreach (var i in dir.GetFiles())
            {
                var l = i.Name.Length;
                if (new Regex(".mp3").IsMatch(i.Name))
                {
                    Console.WriteLine("\nФайл: {0}", i.Name);
                    var name = music.SetName(properties["name"]);
                    music.SetPath(i.FullName);
                    music.Save();

                    var newPath = i.Directory + "/" + author + " - " + name + ".mp3";

                    try
                    {
                        File.Move(i.FullName, newPath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Вы хотите сделать 2 одинаковых файла");
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
            foreach (var i in dir.GetFiles())
            {
                if (new Regex(Screen(name.ToLower())).IsMatch(Screen(i.Name.ToLower())))
                {
                    Console.WriteLine("Найден файл, полное имя: " + i.Name);
                    name = i.FullName;
                    Console.WriteLine("Правильно?");
                    res = Console.ReadLine();
                    if (res == "yes")
                        break;
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Попробуйте ещё");
                        return;
                    }
                }
            }
            if (temp == name)
            {
                Console.WriteLine("Файл не найден");
                return;
            }

            //Редактирование тэгов
            var music = new Music();
            var musicName = music.SetName(properties["name"]);
            var author = music.SetAuthor(properties["author"]);
            music.SetAlbum(properties["album"]);
            music.SetYear(properties["year"]);
            music.SetCover(dir, properties["cover"]);
            music.SetPath(name);
            music.Save();

            var newPath = dir.FullName + "/" + author + " - " + musicName + ".mp3";

            File.Move(name, newPath);

            Console.Clear();
            Console.WriteLine("Всё готово!");
        }


        ////////////////////////////////////////////////
        //                                            //
        //         ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ             //
        //                                            //  
        ////////////////////////////////////////////////


        //Меняет "\" на "/", чтобы не было ошибок в регулярках при посике в пути
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

        //Возвращает дирректорию
        private DirectoryInfo GetDir(string s)
        {
            return directory == "" ? new DirectoryInfo(s) : new DirectoryInfo(directory);
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

        //Получается список слов из строки, нужно для главного меню (не меню)
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
