using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CheckPatterns
{
    internal class Program
    {
        //public List<string> PatternLines;
        public class Pair<T, TU>
        {
            public Pair() { }
            public Pair(T first, TU second)
            {
                First = first;
                Second = second;
            }

            private T First { get; set; }
            private TU Second { get; set; }
            //public T first;
            //public U second;
        }
        public void CheckS(List<string> patternLines)
        {
            var wrongPatterns = new List<int>();
            for (var i = 0; i < patternLines.Count; i++)
            {
                if (patternLines[i] != "")
                {
                    if (patternLines[i].IndexOf("=text>") == -1)
                    {
                        //pair<string, int> new_p = new pair<string, int>();
                        //new_p.first = PatternLines[i];
                        //new_p.second = i+1;
                        //wrongPatterns.Add(new_p);
                        //wrongPatterns.Add(i + 1);
                    }
                    else
                    {
                        //string SecondPart = PatternLines[i].Substring(PatternLines[i].IndexOf("=text>") + "=text>".Length);
                        var secondPart = patternLines[i];
                        var countO = 0;
                        var countC = 0;
                        var ind = -1;
                        while (true)
                        {
                            ind = secondPart.IndexOf("<", ind + 1);
                            if (ind == -1) break;
                            countO++;
                        }
                        ind = -1;
                        while (true)
                        {
                            ind = secondPart.IndexOf(">", ind + 1);
                            if (ind == -1) break;
                            if (secondPart[ind - 1] != '~') countC++;
                        }
                        if (countO != countC - 1)
                            wrongPatterns.Add(i + 1);
                        //if (SecondPart.ind.IndexOf("с=") != -1)
                        //    wrongPatterns.Add(SecondPart);
                    }
                }
            }
        }
        public void CheckK(List<string> patternLines)
        {
            var wrongPatterns = new List<int>();
            for (var i = 0; i < patternLines.Count; i++)
            {
                if (patternLines[i] != "")
                {

                    //string SecondPart = PatternLines[i].Substring(PatternLines[i].IndexOf("=text>") + "=text>".Length);
                    var secondPart = patternLines[i];
                    var countS = 0;
                    var ind = -1;
                    while (true)
                    {
                        ind = secondPart.IndexOf("\"", ind + 1);
                        if (ind == -1) break;
                        countS++;
                    }
                    if (countS % 2 != 0)
                        wrongPatterns.Add(i + 1);
                    //if (SecondPart.ind.IndexOf("с=") != -1)
                    //    wrongPatterns.Add(SecondPart);
                }
            }
        }
        public void CheckP(List<string> patternLines)
        {
            var wrongPatterns = new List<int>();
            for (var i = 0; i < patternLines.Count; i++)
            {
                if (patternLines[i] != "")
                {
                    if (patternLines[i].IndexOf("=text>") != -1)
                    {
                        var firstPart = patternLines[i].Substring(0, patternLines[i].IndexOf("=text>"));
                        var secondPart = patternLines[i].Substring(patternLines[i].IndexOf("=text>") + "=text>".Length);
                        if (firstPart.IndexOf("N") != -1 && secondPart.IndexOf("N") == -1)
                            wrongPatterns.Add(i + 1);
                        if (firstPart.IndexOf("N") == -1 && secondPart.IndexOf("N") != -1)
                            wrongPatterns.Add(i + 1);
                        if (firstPart.IndexOf("A") != -1 && secondPart.IndexOf("A") == -1)
                            wrongPatterns.Add(i + 1);
                        if (firstPart.IndexOf("A") == -1 && secondPart.IndexOf("A") != -1)
                            wrongPatterns.Add(i + 1);
                    }
                }
            }
        }

        private void CheckParts(List<string> patternLines)
        {
            var wrongPatterns = new List<int>();
            for (var i = 0; i < patternLines.Count; i++)
            {
                if (patternLines[i] != "")
                {
                    if (patternLines[i].IndexOf("=text>") != -1)
                    {
                        var firstPart = patternLines[i].Substring(0, patternLines[i].IndexOf("=text>"));
                        var secondPart = patternLines[i].Substring(patternLines[i].IndexOf("=text>") + "=text>".Length);
                        var firstPartElements = new List<string>();
                        var secondPartElements = new List<string>();
                        var pattern = "((([AN]|Pa)[0-9]+)|\"(\\w+)\")";
                        foreach (Match match in Regex.Matches(firstPart, pattern, RegexOptions.IgnoreCase))
                            firstPartElements.Add(match.Groups[1].Value);
                        foreach (Match match in Regex.Matches(secondPart, pattern, RegexOptions.IgnoreCase))
                            secondPartElements.Add(match.Groups[1].Value);
                        foreach (var el in firstPartElements)
                            secondPartElements.Remove(el);
                        if (secondPartElements.Count != 0)
                            wrongPatterns.Add(i + 1);
                        //if (firstPart.IndexOf("N") != -1 && SecondPart.IndexOf("N") == -1)
                        //    wrongPatterns.Add(i + 1);
                        //if (firstPart.IndexOf("N") == -1 && SecondPart.IndexOf("N") != -1)
                        //    wrongPatterns.Add(i + 1);
                        //if (firstPart.IndexOf("A") != -1 && SecondPart.IndexOf("A") == -1)
                        //    wrongPatterns.Add(i + 1);
                        //if (firstPart.IndexOf("A") == -1 && SecondPart.IndexOf("A") != -1)
                        //    wrongPatterns.Add(i + 1);
                    }
                    else
                    {
                        wrongPatterns.Add(i + 1);
                    }
                }
            }
        }
        public void CheckThroughLsplUtilite(List<string> patternLines, string patternsFile)
        {
            var count = patternLines.Count;
            var ind = 0;
            var rewrite = false;
            while (count != 0)
            {
                if (count > 50)
                {
                    count = count - 50;
                    var sw = new StreamWriter(patternsFile, rewrite, Encoding.GetEncoding("Windows-1251"));
                    for (var i = 0; i < 50; i++)
                    {
                        sw.WriteLine(patternLines[ind + i]);
                    }
                    rewrite = true;
                    sw.Close();
                    ind = ind + 50;
                }
                else
                {
                    count = 0;
                    var sw = new StreamWriter(patternsFile, true, Encoding.GetEncoding("Windows-1251"));
                    for (var i = 0; i < patternLines.Count - ind; i++)
                    {
                        sw.WriteLine(patternLines[ind + i]);
                    }
                    sw.Close();
                }
            }
        }
        public void SetName(List<string> patternLines)
        {
            var sw = new StreamWriter("NEW_TERM_IT.txt", false, Encoding.GetEncoding("Windows-1251"));
            var priviousWord = "";
            var curLat = 65;
            var counter = 0;
            foreach (var currentPat in patternLines)
            {
                var pat = currentPat.Trim();
                if (pat != "")
                {
                    var indS = pat.IndexOf("<");
                    var indK = pat.IndexOf("\"");
                    if (indS != -1 && indK != -1)
                    {
                        if (indS < indK)
                        {
                            var indSc = pat.IndexOf(">", indS + 1);
                            var indSz = pat.IndexOf(",", indS + 1);
                            if ((indSz != -1 && indSc < indSz) || (indSz == -1))
                            {
                                var curPat = pat.Substring(indS + 1, indSc - (indS + 1));
                                if (curPat == priviousWord)
                                {
                                    curLat++;
                                    sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                                    priviousWord = curPat;
                                }
                                else
                                {
                                    curLat = 65;
                                    sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                                    priviousWord = curPat;
                                }
                            }
                            else if (indSz != -1 && indSz < indSc)
                            {
                                var curPat = pat.Substring(indS + 1, indSz - (indS + 1));
                                if (curPat == priviousWord)
                                {
                                    curLat++;
                                    sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                                    priviousWord = curPat;
                                }
                                else
                                {
                                    curLat = 65;
                                    sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                                    priviousWord = curPat;
                                }
                            }
                        }
                        else
                        {
                            var indKc = pat.IndexOf("\"", indK + 1);
                            var curPat = pat.Substring(indK + 1, indKc - (indK + 1));

                            if (curPat == priviousWord)
                            {
                                curLat++;
                                sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                                priviousWord = curPat;
                            }
                            else
                            {
                                curLat = 65;
                                sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                                priviousWord = curPat;
                            }
                        }
                    }
                    else if (indS != -1)
                    {
                        var indSc = pat.IndexOf(">", indS + 1);
                        var indSz = pat.IndexOf(",", indS + 1);
                        if ((indSz != -1 && indSc < indSz) || (indSz == -1))
                        {
                            var curPat = pat.Substring(indS + 1, indSc - (indS + 1));
                            if (curPat == priviousWord)
                            {
                                curLat++;
                                sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                                priviousWord = curPat;
                            }
                            else
                            {
                                curLat = 65;
                                sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                                priviousWord = curPat;
                            }
                        }
                        else if (indSz != -1 && indSz < indSc)
                        {
                            var curPat = pat.Substring(indS + 1, indSz - (indS + 1));
                            if (curPat == priviousWord)
                            {
                                curLat++;
                                sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                                priviousWord = curPat;
                            }
                            else
                            {
                                curLat = 65;
                                sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                                priviousWord = curPat;
                            }
                        }
                    }
                    else if (indK != -1)
                    {
                        var indKc = pat.IndexOf("\"", indK + 1);
                        var curPat = pat.Substring(indK + 1, indKc - (indK + 1));

                        if (curPat == priviousWord)
                        {
                            curLat++;
                            sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                            priviousWord = curPat;
                        }
                        else
                        {
                            curLat = 65;
                            sw.WriteLine(curPat + "-" + ((char)curLat).ToString().ToUpper() + " = " + pat);
                            priviousWord = curPat;
                        }
                    }
                }
                else
                {
                    sw.WriteLine("\n");
                }
                counter++;
            }
            sw.Close();
        }
        public void SetNameFix(List<string> patternLines)
        {
            var sw = new StreamWriter("NEW_TERM_IT.txt", false, Encoding.GetEncoding("Windows-1251"));
            var priviousWord = "";
            var curLat = 65;
            var counter = 0;
            foreach (var currentPat in patternLines)
            {
                if (currentPat != "")
                {
                    var pat = currentPat.Trim();
                    string[] delimiterChars = { " = " };
                    var words = pat.Split(delimiterChars, StringSplitOptions.None);
                    var newPat = words[0] + " = ";
                    for (var i = 2; i < words.Count() - 2; i++)
                        newPat += words[i] + " = ";
                    newPat += words[words.Count() - 1];
                    sw.WriteLine(newPat + "\n");
                }
                else
                {
                    sw.WriteLine("\n");
                }
            }
        }

        private static void Main(string[] args)
        {
            var pr = new Program();
            var patternLines = new List<string>();
            //string programmPath = Application.StartupPath.ToString();
            //string programmPath = "C:\\Users\\Kir\\Documents\\Visual Studio 2013\\Projects\\TermsStrategy\\Debug\\Patterns";
            var programmPath = "C:\\Users\\Kir\\Documents\\Visual Studio 2015\\Projects\\SWStool\\CheckPatterns\\bin\\Debug";
            //string PatternsFile = programmPath + "\\TERM_F2.txt";
            var patternsFile = programmPath + "\\NEW_TERM_IT.txt";
            var curPattern = "";
            //List<int> wrongPatterns = new List<int>();
            //List<string> wrongPatterns = new List<string>();
            var fs = new StreamReader(patternsFile, Encoding.GetEncoding("Windows-1251"));

            while (true)
            {
                curPattern = fs.ReadLine();
                if (curPattern == null) break;
                //if (curPattern != "")
                    patternLines.Add(curPattern);
            }
            fs.Close();
            pr.CheckParts(patternLines);
            //pr.checkS(PatternLines);
        }
    }
}









//for (int i = 0; i < PatternLines.Count; i++)
//            {
//                if (PatternLines[i] != "")
//                {
//                    if (PatternLines[i].IndexOf("=text>") != -1)
//                    {
//                        string firstPart = PatternLines[i].Substring(0, PatternLines[i].IndexOf("=text>"));
//                        string SecondPart = PatternLines[i].Substring(PatternLines[i].IndexOf("=text>") + "=text>".Length);
//                        if (firstPart.IndexOf("N") != -1 && SecondPart.IndexOf("N") == -1)
//                            wrongPatterns.Add(i + 1);
//                        if (firstPart.IndexOf("N") == -1 && SecondPart.IndexOf("N") != -1)
//                            wrongPatterns.Add(i + 1);
//                        if (firstPart.IndexOf("A") != -1 && SecondPart.IndexOf("A") == -1)
//                            wrongPatterns.Add(i + 1);
//                        if (firstPart.IndexOf("A") == -1 && SecondPart.IndexOf("A") != -1)
//                            wrongPatterns.Add(i + 1);
//                    }                   
//                }
//            }