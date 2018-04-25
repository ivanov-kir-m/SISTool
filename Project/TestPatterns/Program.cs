using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TestPatterns
{
    internal class Program
    {
        public static int FindInList(List<Pair<string, string>> v, string str, int alt)
        {
            if (v == null || str == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if ((alt == 1 && v[i].First == str) || (alt == 2 && v[i].Second == str))
                    return i;
            }
            return -1;
        }
        public class Pair<T, TU>
        {
            public Pair(T first, TU second)
            {
                First = first;
                Second = second;
            }
            public Pair() { }
            public T First { get; set; }
            public TU Second { get; set; }
        }

        private static void Main(string[] args)
        {
            TestAuthPatterns();
        }

        public static void TestAuthPatterns()
        {
            var patternsModel = new List<Pair<string, string>>();
            var programmPath = Application.StartupPath;
            var batOutput = programmPath + "\\AuthTerms.bat";
            var patternsName = "";
            var curPattern = "";
            var fs = new StreamReader(programmPath + "\\Patterns\\AUTH_TERM.txt", Encoding.GetEncoding("Windows-1251"));
            curPattern = fs.ReadLine();
            if (curPattern != null)
            {
                curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                curPattern = curPattern.Trim();
                if (curPattern.IndexOf("DefIns") == -1 && curPattern.IndexOf("DefIns") == -1)
                {
                    var len = 0;
                    if (curPattern.IndexOf("Def") != -1 && curPattern.Length > 3)
                    {
                        len = curPattern.IndexOf("Def") + "Def".Length;
                    }
                    if (len > 0)
                    {
                        var k = FindInList(patternsModel, curPattern, 1);
                        if (k == -1)
                        {
                            var newP = new Pair<string, string>();
                            patternsName = patternsName + " " + curPattern;
                            newP.First = curPattern;
                            newP.Second = curPattern.Substring(len).Trim();
                            patternsModel.Add(newP);
                        }
                        len = 0;
                    }
                }
                while (true)
                {
                    curPattern = fs.ReadLine();
                    if (curPattern == null) break;
                    if (curPattern != "")
                    {
                        curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                        curPattern = curPattern.Trim();
                        if (curPattern.IndexOf("DefIns") == -1 && curPattern.IndexOf("DefXXX") == -1)
                        {
                            var len = 0;
                            if (curPattern.IndexOf("Def") != -1 && curPattern.Length > 3)
                            {
                                len = curPattern.IndexOf("Def") + "Def".Length;
                            }
                            if (len > 0)
                            {
                                var k = FindInList(patternsModel, curPattern, 1);
                                if (k == -1)
                                {
                                    var newP = new Pair<string, string>();
                                    patternsName = patternsName + " " + curPattern;
                                    newP.First = curPattern;
                                    newP.Second = curPattern.Substring(len).Trim();
                                    patternsModel.Add(newP);
                                }
                                len = 0;
                            }
                        }
                    }
                }
                var inputFile = programmPath + "\\inputText.txt";
                var lsplExe = programmPath + "\\bin\\lspl-find.exe";
                var lsplPatterns = programmPath + "\\Patterns\\AUTH_TERM.txt";
                var lsplOutput = programmPath + "\\AuthTermsOutput.xml";
                var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
                sw.WriteLine("\"" + lsplExe + "\" -i \"" + inputFile + "\" -p \"" + lsplPatterns + "\" -o \"" + lsplOutput + "\" " + patternsName);
                sw.Close();
                //ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //System.Diagnostics.Process.Start(startInfo).WaitForExit();
                //GetAuthTerms(AuthTermsAr);

            }
            else
            {
                MessageBox.Show("Ошибка! Некорректный файл с шаблонами авторских терминов!");
                Application.Exit();
            }
        }
        public static void TestNonDictPatterns()
        {
            var patternsModel = new List<Pair<string, string>>();
            var programmPath = Application.StartupPath;
            var lsplPatterns = programmPath + "\\Patterns\\NONDICT_TERM.txt";
            var fs = new StreamReader(lsplPatterns, Encoding.GetEncoding("Windows-1251"));
            var patternsName = "";
            var curPattern = "";
            while (true)
            {
                curPattern = fs.ReadLine();
                if (curPattern == null) break;
                if (curPattern != "")
                {
                    curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                    var len = 0;
                    switch (curPattern[0])
                    {
                        case 'F':
                            {
                                len = "F".Length;
                                break;
                            }
                        case 'C':
                            {
                                len = "Ca".Length;
                                break;
                            }
                        case 'N':
                            {
                                if (curPattern.IndexOf("F") != -1) len = "NPF".Length;
                                else len = "NPCa".Length;
                                break;
                            }
                    }
                    var k = FindInList(patternsModel, curPattern, 1);
                    if (k == -1 && len != 0)
                    {
                        var newP = new Pair<string, string>();
                        patternsName = patternsName + " " + curPattern.Trim();
                        newP.First = curPattern;
                        newP.Second = curPattern.Substring(len).Trim();
                        patternsModel.Add(newP);
                    }
                }
            }
            //--------------------------------
            var inputFile = programmPath + "\\inputText.txt";
            var lsplExe = programmPath + "\\bin\\lspl-find.exe";
            var lsplOutput = programmPath + "\\NontDictTermsOutput.xml";
            var batOutput = programmPath  + "\\NontDictTerms.bat";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + lsplExe + "\" -i \"" + inputFile + "\" -p \"" + lsplPatterns + "\" -o \"" + lsplOutput + "\" " + patternsName);
            //Close the file
            sw.Close();
            //ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //System.Diagnostics.Process.Start(startInfo).WaitForExit();
            //GetNonDictTerms(NonDictTermsAr);
            //---------------------------------
        }
        public static void TestSynPatterns()
        {
            var patternsModel = new List<Pair<string, string>>();
            var programmPath = Application.StartupPath;
            var lsplPatterns = programmPath + "\\Patterns\\SYN_TERM.txt";
            var patternsName = "";
            var curPattern = "";
            var fs = new StreamReader(lsplPatterns, Encoding.GetEncoding("Windows-1251"));
            curPattern = fs.ReadLine();
            while (true)
            {
                curPattern = fs.ReadLine();
                if (curPattern == null) break;
                if (curPattern != "")
                {
                    curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                    if (curPattern.IndexOf("SYN") != -1)
                    {
                        var len = curPattern.IndexOf("SYN") + "SYN".Length;
                        var k = FindInList(patternsModel, curPattern, 1);
                        if (k == -1)
                        {
                            var newP = new Pair<string, string>();
                            patternsName = patternsName + " " + curPattern;
                            newP.First = curPattern;
                            newP.Second = curPattern.Substring(len).Trim();
                            patternsModel.Add(newP);
                        }
                        len = 0;
                    }
                }
            }
            //--------------------------------
            var inputFile = programmPath + "\\inputText.txt";
            var lsplExe = programmPath + "\\bin\\lspl-find.exe";
            var lsplOutput = programmPath + "\\SynTermsOutput.xml";
            var batOutput = programmPath + "\\SynTerms.bat";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + lsplExe + "\" -i \"" + inputFile + "\" -p \"" + lsplPatterns + "\" -o \"" + lsplOutput + "\" " + patternsName);
            //Close the file
            sw.Close();
            //ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //System.Diagnostics.Process.Start(startInfo).WaitForExit();
            //GetSynTerms(SynTermsAr);
            //---------------------------------
        }
        public static void TestDictPatterns()
        {
            var dictPatterns = new List<Pair<string, string>>();
            var programmPath = Application.StartupPath;
            var lsplPatterns = "";
            StreamReader fs = null;
            lsplPatterns = programmPath + "\\Patterns\\F_TERM.txt";
            fs = new StreamReader(lsplPatterns, Encoding.GetEncoding(1251));
            var patternsName = "";
            var curPattern = "";
            var inputFile = programmPath + "\\inputText.txt";
            var lsplExe = programmPath + "\\bin\\lspl-find.exe";
            var lsplOutput = programmPath + "\\DictTermsOutput.xml";
            var batOutput = programmPath + "\\DictTerms.bat";
            var batCommand = "\"" + lsplExe + "\" -i \"" + inputFile + "\" -p \"" + lsplPatterns + "\" -o \"" + lsplOutput + "\" ";
            var callUtilit = false;
            curPattern = fs.ReadLine();
            if (curPattern != "")
            {
                curPattern = curPattern.Trim();
                patternsName = patternsName + " " + curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                var curPat = new Pair<string, string>();
                curPat.First = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                curPat.Second = curPattern.Substring(curPattern.IndexOf("=") + 1).Trim();
                dictPatterns.Add(curPat);
            }
            //string prevPattern = curPattern.Substring(0, curPattern.IndexOf('='));
            while (true)
            {
                curPattern = fs.ReadLine();
                if (curPattern == null) break;
                //if (curPattern != "" && curPattern.Substring(0, curPattern.IndexOf('=')) != prevPattern)
                if (curPattern != "")
                {
                    var curPatternName = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                    if (batCommand.Length + patternsName.Length + curPatternName.Length < 8000)
                    {
                        patternsName = patternsName + " " + curPatternName;
                        var curPat = new Pair<string, string>();
                        curPat.First = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                        curPat.Second = curPattern.Substring(curPattern.IndexOf("=") + 1).Trim();
                        dictPatterns.Add(curPat);
                        callUtilit = false;
                    }
                    else
                    {
                        var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
                        sw.WriteLine(batCommand + patternsName);
                        sw.Close();
                        //ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
                        //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //System.Diagnostics.Process.Start(startInfo).WaitForExit();
                       // GetDictTerms(DictTermsAr);
                        patternsName = curPatternName;
                        var curPat = new Pair<string, string>();
                        curPat.First = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                        curPat.Second = curPattern.Substring(curPattern.IndexOf("=") + 1).Trim();
                        dictPatterns.Add(curPat);
                        callUtilit = true;
                    }
                }
            }
            if (!callUtilit)
            {
                var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
                sw.WriteLine(batCommand + patternsName);
                sw.Close();
                //ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //System.Diagnostics.Process.Start(startInfo).WaitForExit();
                //GetDictTerms(DictTermsAr);
            }
            /*switch (dictionary)
            //{
            //    case DictionaryF.IT_TERM:
            //        {
            //            LSPL_patterns = programmPath + "\\Patterns\\IT_TERMNP.txt";
            //            fs = new StreamReader(LSPL_patterns, Encoding.GetEncoding("Windows-1251"));
            //            break;
            //        }
            //    case DictionaryF.F_TERM:
            //        {
            //            LSPL_patterns = programmPath + "\\Patterns\\F_TERMNP.txt";
            //            fs = new StreamReader(LSPL_patterns, Encoding.GetEncoding("Windows-1251"));
            //            break;
            //        }
            //}
            //patternsName = "";
            //curPattern = "";            
            //while (true)
            //{
            //    curPattern = fs.ReadLine();
            //    if (curPattern == null) break;
            //    //if (curPattern != "" && curPattern.Substring(0, curPattern.IndexOf('=')) != prevPattern)
            //    if (curPattern != "")
            //    {
            //        int k = FindFunctions.findINList(DictPatterns, curPattern.Substring("NP".Length, curPattern.IndexOf('=') - "NP".Length), 1);
            //        DictPatterns[k].second = curPattern.Substring(curPattern.IndexOf('=')); 
            //    }
            //}
            //--------------------------------*/
        }
    }
}
