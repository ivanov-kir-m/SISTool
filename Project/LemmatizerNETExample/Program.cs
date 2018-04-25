using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LemmaExample
{
    class Program
    {
        static void Main(string[] args)
        {
            LemmatizerNET.MorphLanguage lang = LemmatizerNET.MorphLanguage.Russian;         // Русский словарь
            LemmatizerNET.ILemmatizer lem = LemmatizerNET.LemmatizerFactory.Create(lang);   // объект лемматизера
            LemmatizerNET.FileManager manager = LemmatizerNET.FileManager.GetFileManager(@"c:\RML\");   // путь к словарям
            lem.LoadDictionariesRegistry(manager);                      // инициализация словарей
            LemmatizerNET.IParadigmCollection paradigmList = lem.CreateParadigmCollectionFromForm("ЗВЕЗДЫ", false, true); // создание парадигмы по словоформе
            for (var i = 0; i < paradigmList.Count; i++)          // вывод данных
            {
                var paradigm = paradigmList[i];
                Console.Write("Paradigm: ");
                Console.WriteLine(paradigm.Norm);
                Console.Write("\tFounded: ");
                Console.WriteLine(paradigm.Founded);
                Console.Write("\tParadigmID: ");
                Console.WriteLine(paradigm.ParadigmID);
                for (var j = 0; j < paradigm.Count; j++)
                {
                    Console.Write("\t\t");
                    Console.Write(paradigm.GetForm(j));
                    Console.Write("\t");
                    Console.WriteLine(paradigm.GetAncode(j));
                }
            }
            Console.ReadLine();

        }
    }
}
