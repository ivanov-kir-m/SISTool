using System;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Novacode;

namespace TestDllApp
{
    internal static class Program
    {
        private static void Main()
        {
            // Modify to suit your machine:
            const string fileName = @"C:\Users\Kir\Desktop\Test\Пособие Лисп финал.docx";

            // Create a document in memory:
            var doc = DocX.Load(fileName);
            //if (doc?.Bookmarks != null)
            //    foreach (var docBookmark in doc?.Bookmarks)
            //        Console.WriteLine(docBookmark.Paragraph.Text);

            // Insert a paragrpah:
            //doc.InsertParagraph("This is my first paragraph");

            // Save to the output directory:
            doc.Save();

            //ScriptEngine engine = Python.CreateEngine();
            //ScriptScope scope = engine.CreateScope();

            //engine.ExecuteFile("pymorphyNormalization.py", scope);
            //dynamic normalize = scope.GetVariable("normalize");
            //// вызываем функцию и получаем результат
            //string result = normalize("МАШИНЫ");
            //Console.WriteLine(result);

            //Console.Read();

        }
    }
}
