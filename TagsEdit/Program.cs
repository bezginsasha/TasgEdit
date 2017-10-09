using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagsEdit
{
    class Program
    {
        static void Main(string[] args)
        {
            var neval = TagLib.File.Create("D:/MusicTest/neval.mp3");
            Console.WriteLine(neval.Tag.Album);
            Console.ReadKey();
        }
    }
}
