using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static TermRules.AuxiliaryFunctions;
using static TermRules.KindOfTerm;
using DbscanImplementation;
using ForelImplimentation;
using Newtonsoft.Json.Linq;
using Accord.MachineLearning;
using Accord.Math;
//using Accord.Statistics.Distributions.DensityKernels;
//using Accord.Math.Distances;

namespace TermRules
{
    public class Rules
    {
        //private string InputFile { get; }

        public Rules(string inputfile, DictionaryF dict, int startPage, int endPage, bool clearSupportLists)
        {
            //InputFile = inputfile;
            _proc = new TermsProcessing(inputfile, dict, startPage, endPage, clearSupportLists);
        }

        private readonly TermsProcessing _proc;
        // Сопутствующие функции
        // TODO заменить на Generic Function
        private void GetXmlTermsFrequencies(Terms curTermsAr, string type)
        {
            if (curTermsAr == null) throw new ArgumentNullException(nameof(curTermsAr));
            var tmpPath = Path.GetTempPath();
            var programmPath = Application.StartupPath;
            const string folderPath = "TermsProcessingF";
            var batOutput = tmpPath + folderPath + "\\TermsArFrequencies" + type + ".bat";
            var lsplPatterns = tmpPath + folderPath + "\\TermsArFrequenciesPatterns" + type + ".txt";
            var targetPatternsOutput = tmpPath + folderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false,
                Encoding.GetEncoding("Windows-1251"));
            var pAtternsStreamWriter = new StreamWriter(lsplPatterns, false, Encoding.GetEncoding("Windows-1251"));
            for (var i = 0; i < curTermsAr.TermsAr.Count; i++)
            {
                if (string.IsNullOrEmpty(curTermsAr.TermsAr[i].NPattern)) continue;
                if (!curTermsAr.TermsAr[i].TermFragment.Contains('+'))
                {
                    pAtternsStreamWriter.WriteLine("Term-" + IntegerToRoman(i + 1) + "-full = " +
                                               curTermsAr.TermsAr[i].NPattern);
                    targetPatternsWriter.WriteLine("Term-" + IntegerToRoman(i + 1) + "-full");
                }
            }
            pAtternsStreamWriter.Close();
            targetPatternsWriter.Close();
            var lsplExe = programmPath + "\\bin\\lspl-find.exe";
            var lsplOutputText = tmpPath + folderPath + "\\TermsArFrequenciesOutputText" + type + ".xml";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            sw.WriteLine("\"" + lsplExe
                         + "\" -i \"" + _proc.InputFile
                         + "\" -p \"" + lsplPatterns
                         + "\" -t \"" + lsplOutputText
                         + "\" -s \"" + targetPatternsOutput + "\" ");
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput) { WindowStyle = ProcessWindowStyle.Hidden };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            GetTermsFrequencies(curTermsAr, type);
            if (type == "_auth_terms" || type == "_main_terms")
            {
                //GetPagesSets(curTermsAr, 3); //DBSCAN
                //GetPagesSetsAlt1(curTermsAr, 3); //FOREL
                GetPagesSetsAlt2(curTermsAr, 5); //KMEANS
            }
        }

        private static void GetTermsFrequencies(Terms curTermsAr, string type)
        {
            var tmpPath = Path.GetTempPath();
            const string folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var lastNodeName = "";
            var newMatch = false;
            var processedPos = new List<Point>();
            var curItemIndex = -1;
            TermTree cvalueTermsTree = new TermTree();
            Dictionary<int, List<Point>> nondictTermsPositionsMap = new Dictionary<int, List<Point>>();
            using (var xml = XmlReader.Create(tmpPath + folderPath + "\\TermsArFrequenciesOutputText" + type + ".xml"))
            {
                while (xml.Read())
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                switch (xml.Name)
                                {
                                    case "goal":
                                        if (xml.HasAttributes)
                                            while (xml.MoveToNextAttribute())
                                                if (xml.Name == "name")
                                                {
                                                    curPat = xml.Value.Trim();
                                                    curPat = curPat.Trim();
                                                }
                                        if (curItemIndex != -1)
                                        {
                                            nondictTermsPositionsMap[curItemIndex] = processedPos;
                                            processedPos = new List<Point>();
                                        }
                                        break;
                                    case "match":
                                        if (xml.HasAttributes)
                                            while (xml.MoveToNextAttribute())
                                            {
                                                if (xml.Name == "startPos")
                                                    curPos.X = Convert.ToInt32(xml.Value.Trim());
                                                if (xml.Name == "endPos")
                                                    curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        newMatch = true;
                                        break;
                                    case "result":
                                        lastNodeName = xml.Name;
                                        continue;
                                    case "fragment":
                                        lastNodeName = xml.Name;
                                        break;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                }
                                if (lastNodeName == "result")
                                {
                                    var partPattern = curPat.Split('-').ToList();
                                    if (partPattern[2] == "full")
                                    {
                                        var cPos = processedPos.FindIndex(item => item.X == curPos.X && item.Y == curPos.Y);
                                        if (cPos == -1)
                                        {
                                            curItemIndex = RomanToInteger(partPattern[1]) - 1;
                                            if (curItemIndex != -1)
                                                if (newMatch)
                                                {
                                                    newMatch = false;
                                                    if (curTermsAr.TermsAr[curItemIndex].FreqStart)
                                                    {
                                                        curTermsAr.TermsAr[curItemIndex].FreqStart = false;
                                                        curTermsAr.TermsAr[curItemIndex].Frequency = 0;
                                                    }
                                                    curTermsAr.TermsAr[curItemIndex].Frequency++;
                                                }
                                            processedPos.Add(curPos);
                                            var curRange = new Range(curPos);
                                            var e = curTermsAr.RootTermsTree.FindRange(curRange);
                                            var eExtension =
                                                curTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                            if (e == null && eExtension == null)
                                            {
                                                e = curTermsAr.RootTermsTree.AddRange(curRange);
                                                e.IndexElement.Add(curItemIndex);
                                                curTermsAr.TermsAr[curItemIndex].Pos.Add(e);
                                                curTermsAr.TermsAr[curItemIndex].Frequency++;
                                            }
                                            else
                                            {
                                                if (e != null && !e.IndexElement.Contains(curItemIndex))
                                                    e.IndexElement.Add(curItemIndex);
                                            }
                                            e = cvalueTermsTree.FindRange(curRange);
                                            if (e == null)
                                            {
                                                e = cvalueTermsTree.AddRange(curRange);
                                                e.IndexElement.Add(curItemIndex);
                                            }
                                            else
                                                e.IndexElement.Add(curItemIndex);
                                        }
                                    }
                                }
                                break;
                            }
                        default:
                            continue;
                            //throw new ArgumentOutOfRangeException();
                    }
            }
            for (int term = 0; term < curTermsAr.TermsAr.Count; term++)
            {
                if (nondictTermsPositionsMap.ContainsKey(term))
                {
                    Dictionary<int, int> termsContainsCur = new Dictionary<int, int>();
                    foreach (var curPosition in nondictTermsPositionsMap[term])
                    {
                        var e = cvalueTermsTree.FindRangeExtension(new Range(curPosition));
                        if (e != null)
                        {
                            int maxFreqTerm = -1;
                            int maxFreq = 0;
                            foreach (var extTerm in e.IndexElement)
                            {
                                if (curTermsAr.TermsAr[extTerm].Frequency > maxFreq)
                                {
                                    maxFreq = curTermsAr.TermsAr[extTerm].Frequency;
                                    maxFreqTerm = extTerm;
                                }
                            }
                            if (!termsContainsCur.ContainsKey(maxFreqTerm))
                            {
                                termsContainsCur[maxFreqTerm] = maxFreq;
                            }
                        }
                    }
                    curTermsAr.TermsAr[term].CValue = 1;
                    if (termsContainsCur.Count == 0)
                    {
                        curTermsAr.TermsAr[term].CValue =
                            Math.Log(curTermsAr.TermsAr[term].TermWord.Trim().Split().Length + 1, 2) *
                            curTermsAr.TermsAr[term].Frequency;
                    }
                    else
                    {
                        var sumExtTermsFreqs = 0;
                        foreach (var val in termsContainsCur.Values)
                        {
                            sumExtTermsFreqs += val;
                        }
                        curTermsAr.TermsAr[term].CValue =
                            Math.Log(curTermsAr.TermsAr[term].TermWord.Trim().Split().Length + 1, 2) *
                            (curTermsAr.TermsAr[term].Frequency -
                             (1 / termsContainsCur.Count) * sumExtTermsFreqs);
                    }
                }
                else
                {
                    curTermsAr.TermsAr[term].CValue =
                            Math.Log(curTermsAr.TermsAr[term].TermWord.Trim().Split().Length + 1, 2) *
                            curTermsAr.TermsAr[term].Frequency;
                }
            }
            if(type != "_main_terms")
                getFrequency_(curTermsAr);
        }

        private static void PrintTermsFrequencies(Terms curTermsAr, string termsType)
        {
            var programmPath = Application.StartupPath;
            var serializeWriter = new StreamWriter(programmPath + "\\backup_docs\\" + termsType + ".csv", false,
                Encoding.GetEncoding("Windows-1251"));
            if (termsType == "_auth_terms" || termsType == "_main_terms")
            {
                serializeWriter.WriteLine("Term;MainPage;Pages;TermFrequency;CValue;inHeader;TermKind;TermType;TermSynonimTo;TermRule;TermRepeatRule;TermFragment;TermFullNormForm");
            }
            else
                serializeWriter.WriteLine("Term;MainPage;TermFrequency;CValue;inHeader;TermKind;TermType;TermSynonimTo;TermRule;TermRepeatRule;TermFragment;TermFullNormForm");
            foreach (var term in curTermsAr.TermsAr)
            {
                var termFragment = term.TermFragment;
                var termKind = "";
                switch (term.Kind)
                {
                    case AuthTerm:
                        termKind = "AuthTerm";
                        break;
                    case KindOfTerm.CombTerm:
                        termKind = "CombTerm";
                        break;
                    case DictTerm:
                        termKind = "DictTerm";
                        break;
                    case KindOfTerm.NonDictTerm:
                        termKind = "NonDictTerm";
                        break;
                    case KindOfTerm.SynTerm:
                        termKind = "SynTerm";
                        break;
                    case Null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var termType = "";
                switch (term.type)
                {
                    case TypeOfTerm.Trusted:
                        termType = "Trusted";
                        break;
                    case TypeOfTerm.Untrusted:
                        termType = "Untrusted";
                        break;
                    case TypeOfTerm.UntrastedByFrequency:
                        termType = "UntrustedByFrequency";
                        break;
                    case TypeOfTerm.Null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                string pagesString = "";
                if (termsType == "_auth_terms" || termsType == "_main_terms")
                {
                    foreach (var cluster in term.pages)
                    {
                        if (cluster.Length == 1)
                            pagesString += cluster[0].number.ToString() + ", ";
                        else
                        {
                            List<MyCustomDatasetItem> pages = new List<MyCustomDatasetItem>();
                            pages = cluster.ToList().OrderBy(item => item.number).ToList();
                            if(pages[0].number == pages[pages.Count() - 1].number)
                                pagesString += pages[0].number.ToString() + ", ";
                            else
                                pagesString += pages[0].number.ToString() + "-" + pages[pages.Count()-1].number.ToString() + ", ";
                        }
                    }
                    if (pagesString.Trim().Length >= 2)
                        pagesString = pagesString.Trim().Substring(0,pagesString.Length-2).Trim();
                }
                var termRule = term.Rule.ToString();
                var termRepeatRule = term.RepeatRule.ToString();
                var termFrequency = term.Frequency.ToString();
                var termCValue = term.CValue.ToString();
                if (termsType == "_auth_terms" || termsType == "_main_terms")
                {
                    string termSynonimTo = term.synonimTo;
                    if (termSynonimTo == term.TermWord)
                        termSynonimTo = "-";
                    serializeWriter.WriteLine(term.TermWord + ";" + term.mainPage.ToString() + ";" + pagesString + ";" + termFrequency + ";" + termCValue + ";" + term.inHeader.ToString() + ";" + termKind + ";" + termType + ";" + termSynonimTo + ";" +
                                          termRule + ";" + termRepeatRule + ";" + termFragment + ";" + term.TermFullNormForm);
                }
                else
                    serializeWriter.WriteLine(term.TermWord + ";" + term.mainPage.ToString() + ";" + termFrequency + ";" + termCValue +";" + term.inHeader.ToString() + ";" + termKind + ";" + termType + ";" + term.synonimTo + ";" +
                                          termRule + ";" + termRepeatRule + ";" + termFragment+";"+term.TermFullNormForm);
            }
            serializeWriter.Close();
        }

        private void GetXmlCombTermsFrequencies(CombTerms combTermsAr)
        {
            var tmpPath = Path.GetTempPath();
            var programmPath = Application.StartupPath;
            var folderPath = "TermsProcessingF";
            var batOutput = tmpPath + folderPath + "\\CombTermsFrequencies.bat";
            var lsplPatterns = tmpPath + folderPath + "\\CombTermsFrequenciesPatterns.txt";
            var targetPatternsOutput = tmpPath + folderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false,
                Encoding.GetEncoding("Windows-1251"));
            var pAtternsStreamWriter = new StreamWriter(lsplPatterns, false, Encoding.GetEncoding("Windows-1251"));
            for (var i = 0; i < combTermsAr.TermsAr.Count; i++)
                if (!string.IsNullOrEmpty(combTermsAr.TermsAr[i].NPattern))
                {
                    if (!combTermsAr.TermsAr[i].TermFragment.Contains('+'))
                    {
                        pAtternsStreamWriter.WriteLine("Term-" + IntegerToRoman(i + 1) + "-full = " +
                                                   combTermsAr.TermsAr[i].NPattern);
                        targetPatternsWriter.WriteLine("Term-" + IntegerToRoman(i + 1) + "-full");
                    }
                }
            pAtternsStreamWriter.Close();
            targetPatternsWriter.Close();
            var lsplExe = programmPath + "\\bin\\lspl-find.exe";
            var lsplOutputText = tmpPath + folderPath + "\\CombTermsFrequenciesOutputText.xml";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            sw.WriteLine("\"" + lsplExe
                         + "\" -i \"" + _proc.InputFile
                         + "\" -p \"" + lsplPatterns
                         + "\" -t \"" + lsplOutputText
                         + "\" -s \"" + targetPatternsOutput + "\" ");
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput) { WindowStyle = ProcessWindowStyle.Hidden };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            GetCombTermsFrequencies(combTermsAr);
        }

        private static void GetCombTermsFrequencies(CombTerms combTermsAr)
        {
            var tmpPath = Path.GetTempPath();
            const string folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var lastNodeName = "";
            var newMatch = false;
            var processedPos = new List<Point>();
            var curItemIndex = -1;
            TermTree cvalueTermsTree = new TermTree();
            Dictionary<int, List<Point>> nondictTermsPositionsMap = new Dictionary<int, List<Point>>();
            using (var xml = XmlReader.Create(tmpPath + folderPath + "\\CombTermsFrequenciesOutputText.xml"))
            {
                while (xml.Read())
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                        while (xml.MoveToNextAttribute())
                                            if (xml.Name == "name")
                                            {
                                                curPat = xml.Value.Trim();
                                                curPat = curPat.Trim();
                                            }
                                    if (curItemIndex != -1)
                                    {
                                        nondictTermsPositionsMap[curItemIndex] = processedPos;
                                        processedPos = new List<Point>();
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                                curPos.X = Convert.ToInt32(xml.Value.Trim());
                                            if (xml.Name == "endPos")
                                                curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                        }
                                    newMatch = true;
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                switch (lastNodeName)
                                {
                                    case "fragment":
                                        continue;
                                    case "result":
                                        var partPattern = curPat.Split('-').ToList();
                                        if (partPattern[2] == "full")
                                        {
                                            var cPos = processedPos.FindIndex(item => item.X == curPos.X && item.Y == curPos.Y);
                                            if (cPos == -1)
                                            {
                                                curItemIndex = RomanToInteger(partPattern[1]) - 1;
                                                if (curItemIndex != -1)
                                                    if (newMatch)
                                                    {
                                                        newMatch = false;
                                                        if (combTermsAr.TermsAr[curItemIndex].FreqStart)
                                                        {
                                                            combTermsAr.TermsAr[curItemIndex].FreqStart = false;
                                                            combTermsAr.TermsAr[curItemIndex].Frequency = 0;
                                                        }
                                                        combTermsAr.TermsAr[curItemIndex].Frequency++;
                                                        var curRange = new Range(curPos);
                                                        var e = combTermsAr.RootTermsTree.FindRange(curRange);
                                                        var eExtension =
                                                            combTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                        if (e == null && eExtension == null)
                                                        {
                                                            e = combTermsAr.RootTermsTree.AddRange(curRange);
                                                            e.IndexElement.Add(curItemIndex);
                                                            combTermsAr.TermsAr[curItemIndex].Pos.Add(e);
                                                            combTermsAr.TermsAr[curItemIndex].Frequency++;
                                                        }
                                                        else
                                                        {
                                                            if (e != null && !e.IndexElement.Contains(curItemIndex))
                                                                e.IndexElement.Add(curItemIndex);
                                                        }
                                                        e = cvalueTermsTree.FindRange(curRange);
                                                        if (e == null)
                                                        {
                                                            e = cvalueTermsTree.AddRange(curRange);
                                                            e.IndexElement.Add(curItemIndex);
                                                        }
                                                        else
                                                            e.IndexElement.Add(curItemIndex);
                                                    }
                                                processedPos.Add(curPos);
                                            }
                                        }
                                        break;
                                }
                                break;
                            }
                        default:
                            continue;
                            //throw new ArgumentOutOfRangeException();
                    }
                
            }
            for (int term = 0; term < combTermsAr.TermsAr.Count; term++)
            {
                if (nondictTermsPositionsMap.ContainsKey(term))
                {
                    Dictionary<int, int> termsContainsCur = new Dictionary<int, int>();
                    foreach (var curPosition in nondictTermsPositionsMap[term])
                    {
                        var e = cvalueTermsTree.FindRangeExtension(new Range(curPosition));
                        if (e != null)
                        {
                            int maxFreqTerm = -1;
                            int maxFreq = 0;
                            foreach (var extTerm in e.IndexElement)
                            {
                                if (combTermsAr.TermsAr[extTerm].Frequency > maxFreq)
                                {
                                    maxFreq = combTermsAr.TermsAr[extTerm].Frequency;
                                    maxFreqTerm = extTerm;
                                }
                            }
                            if (!termsContainsCur.ContainsKey(maxFreqTerm))
                            {
                                termsContainsCur[maxFreqTerm] = maxFreq;
                            }
                        }
                    }
                    combTermsAr.TermsAr[term].CValue = 1;
                    if (termsContainsCur.Count == 0)
                    {
                        combTermsAr.TermsAr[term].CValue =
                            Math.Log(combTermsAr.TermsAr[term].TermWord.Trim().Split().Length + 1, 2) *
                            combTermsAr.TermsAr[term].Frequency;
                    }
                    else
                    {
                        var sumExtTermsFreqs = 0;
                        foreach (var val in termsContainsCur.Values)
                        {
                            sumExtTermsFreqs += val;
                        }
                        combTermsAr.TermsAr[term].CValue =
                            Math.Log(combTermsAr.TermsAr[term].TermWord.Trim().Split().Length + 1, 2) *
                            (combTermsAr.TermsAr[term].Frequency -
                             (1 / termsContainsCur.Count) * sumExtTermsFreqs);
                    }
                }
                else
                {
                    combTermsAr.TermsAr[term].CValue =
                            Math.Log(combTermsAr.TermsAr[term].TermWord.Trim().Split().Length + 1, 2) *
                            combTermsAr.TermsAr[term].Frequency;
                }
            }
        }

        private static void PrintCombTermsFrequencies(CombTerms curTermsAr, string termsType)
        {
            var programmPath = Application.StartupPath;
            var serializeWriter = new StreamWriter(programmPath + "\\backup_docs\\" + termsType + ".csv", false,
                Encoding.GetEncoding("Windows-1251"));
            serializeWriter.WriteLine("Term;TermFragment;MainPage;TermType;TermFrequency;CValue;inHeader;TermComponents");
            foreach (var term in curTermsAr.TermsAr)
            {
                var termFragment = term.TermFragment;
                var termType = "";
                switch (term.Kind)
                {
                    case AuthTerm:
                        termType = "AuthTerm";
                        break;
                    case KindOfTerm.CombTerm:
                        termType = "CombTerm";
                        break;
                    case DictTerm:
                        termType = "DictTerm";
                        break;
                    case KindOfTerm.NonDictTerm:
                        termType = "NonDictTerm";
                        break;
                    case KindOfTerm.SynTerm:
                        termType = "SynTerm";
                        break;
                    case Null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var termFrequency = term.Frequency.ToString();
                string components = "";
                foreach (var curComponent in term.Components)
                {
                    components += curComponent.TermWord + "(" + curComponent.Frequency + "),  ";
                }
                var termCValue = term.CValue;
                serializeWriter.WriteLine(term.TermWord + ";" + termFragment + ";" + term.mainPage.ToString() + ";" + termType + ";" + termFrequency + ";" + termCValue + ";" + term.inHeader.ToString() + ";" +
                                          components);
            }
            serializeWriter.Close();
        }

        private static void PrintCombTermsComponentsFrequencies(CombTerms curTermsAr, string termsType)
        {
            var programmPath = Application.StartupPath;
            var serializeWriter = new StreamWriter(programmPath + "\\backup_docs\\" + termsType + ".csv", false,
                Encoding.GetEncoding("Windows-1251"));
            serializeWriter.WriteLine("Term;TermFragment;TermType;TermFrequency;TermRule;TermRepeatRule");
            foreach (var term in curTermsAr.TermsAr)
            {
                foreach (var curComponent in term.Components)
                {
                    var termFragment = curComponent.TermFragment;
                    const string termType = "CombTermComponent";
                    const int termRule = 0;
                    const int termRepeatRule = 0;
                    var termFrequency = curComponent.Frequency.ToString();
                    serializeWriter.WriteLine(curComponent.TermWord + ";" + termFragment + ";" + termType + ";" + termFrequency + ";" +
                                              termRule + ";" + termRepeatRule);
                }
            }
            serializeWriter.Close();
        }

        private void GetXmlSynTermsFrequencies(SynTerms synTermsAr)
        {
            var tmpPath = Path.GetTempPath();
            var programmPath = Application.StartupPath;
            const string folderPath = "TermsProcessingF";
            var batOutput = tmpPath + folderPath + "\\SynTermsFrequencies.bat";
            var lsplPatterns = tmpPath + folderPath + "\\SynTermsFrequenciesPatterns.txt";
            var targetPatternsOutput = tmpPath + folderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false,
                Encoding.GetEncoding("Windows-1251"));
            var pAtternsStreamWriter = new StreamWriter(lsplPatterns, false, Encoding.GetEncoding("Windows-1251"));
            for (var i = 0; i < synTermsAr.TermsAr.Count; i++)
            {
                if (!string.IsNullOrEmpty(synTermsAr.TermsAr[i].Alternatives.First.NPattern))
                {
                    if (!synTermsAr.TermsAr[i].TermFragment.Contains('+'))
                    {
                        pAtternsStreamWriter.WriteLine("Term-" + IntegerToRoman(i + 1) + "-first-full = " +
                                                   synTermsAr.TermsAr[i].Alternatives.First.NPattern);
                        targetPatternsWriter.WriteLine("Term-" + IntegerToRoman(i + 1) + "-first-full");
                    }
                }
                if (!string.IsNullOrEmpty(synTermsAr.TermsAr[i].Alternatives.Second.NPattern))
                {
                    if (!synTermsAr.TermsAr[i].TermFragment.Contains('+'))
                    {
                        pAtternsStreamWriter.WriteLine("Term-" + IntegerToRoman(i + 1) +
                                                   "-second-full = " +
                                                   synTermsAr.TermsAr[i].Alternatives.Second.NPattern);
                        targetPatternsWriter.WriteLine("Term-" + IntegerToRoman(i + 1) + "-second-full");
                    }
                }
            }
            pAtternsStreamWriter.Close();
            targetPatternsWriter.Close();
            var lsplExe = programmPath + "\\bin\\lspl-find.exe";
            var lsplOutputText = tmpPath + folderPath + "\\SynTermsFrequenciesOutputText.xml";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            sw.WriteLine("\"" + lsplExe
                         + "\" -i \"" + _proc.InputFile
                         + "\" -p \"" + lsplPatterns
                         + "\" -t \"" + lsplOutputText
                         + "\" -s \"" + targetPatternsOutput + "\" ");
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput) { WindowStyle = ProcessWindowStyle.Hidden };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            GetSynTermsFrequencies(synTermsAr);
        }

        private static void GetSynTermsFrequencies(SynTerms synTermsAr)
        {
            var tmpPath = Path.GetTempPath();
            const string folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var lastNodeName = "";
            var newMatch = false;
            var processedPos = new List<Point>();
            using (var xml = XmlReader.Create(tmpPath + folderPath + "\\SynTermsFrequenciesOutputText.xml"))
            {
                while (xml.Read())
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                switch (xml.Name)
                                {
                                    case "goal":
                                        if (xml.HasAttributes)
                                            while (xml.MoveToNextAttribute())
                                                if (xml.Name == "name")
                                                {
                                                    curPat = xml.Value.Trim();
                                                    curPat = curPat.Trim();
                                                }
                                        processedPos.Clear();
                                        break;
                                    case "match":
                                        if (xml.HasAttributes)
                                            while (xml.MoveToNextAttribute())
                                            {
                                                if (xml.Name == "startPos")
                                                    curPos.X = Convert.ToInt32(xml.Value.Trim());
                                                if (xml.Name == "endPos")
                                                    curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        newMatch = true;
                                        break;
                                    case "result":
                                        lastNodeName = xml.Name;
                                        break;
                                    case "fragment":
                                        lastNodeName = xml.Name;
                                        break;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                switch (lastNodeName)
                                {
                                    case "fragment":
                                        continue;
                                    case "result":
                                        var partPattern = curPat.Split('-').ToList();
                                        if (partPattern[3] == "full")
                                        {
                                            var cPos = processedPos.FindIndex(item => item.X == curPos.X && item.Y == curPos.Y);
                                            if (cPos == -1)
                                            {
                                                var itemIndex = RomanToInteger(partPattern[1]) - 1;
                                                var alt = partPattern[2].Trim();
                                                if (itemIndex != -1)
                                                {
                                                    if (newMatch)
                                                    {
                                                        newMatch = false;
                                                        switch (alt)
                                                        {
                                                            case "first":
                                                                {
                                                                    if (
                                                                        synTermsAr.TermsAr[itemIndex].Alternatives.First
                                                                            .FreqStart)
                                                                    {
                                                                        synTermsAr.TermsAr[itemIndex].Alternatives.First
                                                                                .FreqStart =
                                                                            false;
                                                                        synTermsAr.TermsAr[itemIndex].Alternatives.First
                                                                                .Frequency =
                                                                            0;
                                                                    }
                                                                    synTermsAr.TermsAr[itemIndex].Alternatives.First.Frequency++;
                                                                    break;
                                                                }
                                                            case "second":
                                                                {
                                                                    if (
                                                                        synTermsAr.TermsAr[itemIndex].Alternatives.Second
                                                                            .FreqStart)
                                                                    {
                                                                        synTermsAr.TermsAr[itemIndex].Alternatives.Second
                                                                                .FreqStart
                                                                            = false;
                                                                        synTermsAr.TermsAr[itemIndex].Alternatives.Second
                                                                                .Frequency
                                                                            = 0;
                                                                    }
                                                                    synTermsAr.TermsAr[itemIndex].Alternatives.Second.Frequency
                                                                        ++;
                                                                    break;
                                                                }
                                                            default:
                                                                {
                                                                    break;
                                                                }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    throw new ArgumentException(
                                                        "Error in GetSynTermsFrequencies. Error romain number.");
                                                }
                                            }
                                        }
                                        break;
                                }
                                break;
                            }
                    }
            }
            for (int i = 0; i < synTermsAr.TermsAr.Count; i++)
            {
                if (synTermsAr.TermsAr[i].Alternatives.First.Frequency != 0 &&
                    synTermsAr.TermsAr[i].Alternatives.Second.Frequency != 0) continue;
                foreach (var curDelPos in synTermsAr.TermsAr[i].Pos)
                {
                    var e = synTermsAr.RootTermsTree.FindRange(curDelPos.Range);
                    if (e != null && e.IndexElement.Count == 1)
                        synTermsAr.RootTermsTree.DeleteRange(curDelPos.Range);
                    else
                        e?.IndexElement.Remove(i);
                }
                synTermsAr.TermsAr.RemoveAt(i);
                i--;
            }
        }

        private static void PrinSynTermsFrequencies(SynTerms nonDictTermsAr, string termsType)
        {
            var programmPath = Application.StartupPath;
            var serializeWriter = new StreamWriter(programmPath + "\\backup_docs\\" + termsType + ".csv", false,
                Encoding.GetEncoding("Windows-1251"));
            serializeWriter.WriteLine("Term1;Term2;TermFragment;TermType;TermFrequency1;TermFrequency2;inHeader1;inHeader2;Pattern");
            var termType = "SynTerm";
            foreach (var term in nonDictTermsAr.TermsAr)
            {
                var termFragment = term.TermFragment;
                var termWord1 = term.Alternatives.First.Alternative;
                var termWord2 = term.Alternatives.Second.Alternative;
                int termFrequency1 = term.Alternatives.First.Frequency;
                int termFrequency2 = term.Alternatives.Second.Frequency;
                serializeWriter.WriteLine(termWord1 + ";" + termWord2 + ";" + termFragment + ";" + termType + ";" +
                                          termFrequency1 + ";" + termFrequency2 + ";" + term.Alternatives.First.inHeader.ToString() + ";" + term.Alternatives.Second.inHeader.ToString() + ";" + term.Pattern);
            }
            serializeWriter.Close();
        }

        private void GetXmlNonDictTermsArFrequencies(NonDictTerms nonDictTermsAr)
        {
            var tmpPath = Path.GetTempPath();
            var programmPath = Application.StartupPath;
            var folderPath = "TermsProcessingF";
            var batOutput = tmpPath + folderPath + "\\NonDictTermsFrequencies.bat";
            var targetPatternsOutput = tmpPath + folderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false,
                Encoding.GetEncoding("Windows-1251"));
            var pAtternsStreamWriter = new StreamWriter(programmPath + "\\Patterns\\NonDictMachedPatterns.txt", false,
                Encoding.GetEncoding("Windows-1251"));
            for (var i = 0; i < nonDictTermsAr.TermsAr.Count; i++)
                if (!string.IsNullOrEmpty(nonDictTermsAr.TermsAr[i].NPattern))
                {
                    if (!nonDictTermsAr.TermsAr[i].TermFragment.Contains('+'))
                    {
                        pAtternsStreamWriter.WriteLine("Term-" + IntegerToRoman(i + 1) + "-full = " +
                                                   nonDictTermsAr.TermsAr[i].NPattern);
                        targetPatternsWriter.WriteLine("Term-" + IntegerToRoman(i + 1) + "-full");
                    }
                }
            pAtternsStreamWriter.Close();
            targetPatternsWriter.Close();
            var lsplExe = programmPath + "\\bin\\lspl-find.exe";
            var lsplPatterns = programmPath + "\\Patterns\\NonDictMachedPatterns.txt";
            var lsplOutputText = tmpPath + folderPath + "\\NonDictTermsFrequencies.xml";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            sw.WriteLine("\"" + lsplExe
                         + "\" -i \"" + _proc.InputFile
                         + "\" -p \"" + lsplPatterns
                         + "\" -t \"" + lsplOutputText
                         + "\" -s \"" + targetPatternsOutput + "\" ");
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput) { WindowStyle = ProcessWindowStyle.Hidden };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            GetNonDictTermsFrequencies(nonDictTermsAr);
        }

        private static void GetNonDictTermsFrequencies(NonDictTerms nonDictTermsAr)
        {
            var tmpPath = Path.GetTempPath();
            const string folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var lastNodeName = "";
            var newMatch = false;
            var processedPos = new List<Point>();
            var curItemIndex = -1;
            TermTree cvalueTermsTree = new TermTree();
            Dictionary<int, List<Point>> nondictTermsPositionsMap = new Dictionary<int, List<Point>>();
            using (var xml = XmlReader.Create(tmpPath + folderPath + "\\NonDictTermsFrequencies.xml"))
            {
                while (xml.Read())
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                switch (xml.Name)
                                {
                                    case "goal":
                                        if (xml.HasAttributes)
                                            while (xml.MoveToNextAttribute())
                                                if (xml.Name == "name")
                                                {
                                                    curPat = xml.Value.Trim();
                                                    curPat = curPat.Trim();
                                                }
                                        if (curItemIndex != -1)
                                        {
                                            nondictTermsPositionsMap[curItemIndex] = processedPos;
                                            processedPos = new List<Point>();
                                        }
                                        break;
                                    case "match":
                                        if (xml.HasAttributes)
                                            while (xml.MoveToNextAttribute())
                                            {
                                                if (xml.Name == "startPos")
                                                    curPos.X = Convert.ToInt32(xml.Value.Trim());
                                                if (xml.Name == "endPos")
                                                    curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        newMatch = true;
                                        break;
                                    case "result":
                                        lastNodeName = xml.Name;
                                        continue;
                                    case "fragment":
                                        lastNodeName = xml.Name;
                                        break;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                switch (lastNodeName)
                                {
                                    case "fragment":
                                        continue;
                                    case "result":
                                        var partPattern = curPat.Split('-').ToList();
                                        if (partPattern[2] == "full")
                                        {
                                            var cPos = processedPos.FindIndex(item => item.X == curPos.X && item.Y == curPos.Y);
                                            if (cPos == -1)
                                            {
                                                curItemIndex = RomanToInteger(partPattern[1]) - 1;
                                                if (curItemIndex != -1)
                                                    if (newMatch)
                                                    {
                                                        newMatch = false;
                                                        if (nonDictTermsAr.TermsAr[curItemIndex].FreqStart)
                                                        {
                                                            nonDictTermsAr.TermsAr[curItemIndex].FreqStart = false;
                                                            nonDictTermsAr.TermsAr[curItemIndex].Frequency = 0;
                                                        }
                                                        nonDictTermsAr.TermsAr[curItemIndex].Frequency++;
                                                    }
                                                processedPos.Add(curPos);
                                                var curRange = new Range(curPos);
                                                var e = nonDictTermsAr.RootTermsTree.FindRange(curRange);
                                                var eExtension =
                                                    nonDictTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                if (e == null && eExtension == null)
                                                {
                                                    e = nonDictTermsAr.RootTermsTree.AddRange(curRange);
                                                    e.IndexElement.Add(curItemIndex);
                                                    nonDictTermsAr.TermsAr[curItemIndex].Pos.Add(e);
                                                    nonDictTermsAr.TermsAr[curItemIndex].Frequency++;
                                                }
                                                else
                                                {
                                                    if (e != null && !e.IndexElement.Contains(curItemIndex))
                                                        e.IndexElement.Add(curItemIndex);
                                                }
                                                e = cvalueTermsTree.FindRange(curRange);
                                                if (e == null)
                                                {
                                                    e = cvalueTermsTree.AddRange(curRange);
                                                    e.IndexElement.Add(curItemIndex);
                                                }
                                                else
                                                    e.IndexElement.Add(curItemIndex);
                                            }
                                        }
                                        break;
                                }
                                break;
                            }
                        default:
                            continue;
                            //throw new ArgumentOutOfRangeException();
                    }
            }
            for (int term = 0; term < nonDictTermsAr.TermsAr.Count; term++)
            {
                if (nondictTermsPositionsMap.ContainsKey(term))
                {
                    Dictionary<int, int> termsContainsCur = new Dictionary<int, int>();
                    foreach (var curPosition in nondictTermsPositionsMap[term])
                    {
                        var e = cvalueTermsTree.FindRangeExtension(new Range(curPosition));
                        if (e != null)
                        {
                            int maxFreqTerm = -1;
                            int maxFreq = 0;
                            foreach (var extTerm in e.IndexElement)
                            {
                                if (nonDictTermsAr.TermsAr[extTerm].Frequency > maxFreq)
                                {
                                    maxFreq = nonDictTermsAr.TermsAr[extTerm].Frequency;
                                    maxFreqTerm = extTerm;
                                }
                            }
                            if (!termsContainsCur.ContainsKey(maxFreqTerm))
                            {
                                termsContainsCur[maxFreqTerm] = maxFreq;
                            }
                        }
                    }
                    nonDictTermsAr.TermsAr[term].CValue = 1;
                    if (termsContainsCur.Count == 0)
                    {
                        nonDictTermsAr.TermsAr[term].CValue =
                            Math.Log(nonDictTermsAr.TermsAr[term].TermWord.Trim().Split().Length + 1, 2) *
                            nonDictTermsAr.TermsAr[term].Frequency;
                    }
                    else
                    {
                        var sumExtTermsFreqs = 0;
                        foreach (var val in termsContainsCur.Values)
                        {
                            sumExtTermsFreqs += val;
                        }
                        nonDictTermsAr.TermsAr[term].CValue =
                            Math.Log(nonDictTermsAr.TermsAr[term].TermWord.Trim().Split().Length + 1, 2) *
                            (nonDictTermsAr.TermsAr[term].Frequency -
                             (1 / termsContainsCur.Count) * sumExtTermsFreqs);
                    }
                }
                else
                {
                    nonDictTermsAr.TermsAr[term].CValue =
                            Math.Log(nonDictTermsAr.TermsAr[term].TermWord.Trim().Split().Length + 1, 2) *
                            nonDictTermsAr.TermsAr[term].Frequency;
                }
            }
        }

        private static void PrinNonDictTermsFrequencies(NonDictTerms nonDictTermsAr, string termsType)
        {
            var programmPath = Application.StartupPath;
            var serializeWriter = new StreamWriter(programmPath + "\\backup_docs\\" + termsType + ".csv", false,
                Encoding.GetEncoding("Windows-1251"));
            serializeWriter.WriteLine("Term;TermFragment;MainPage;TermType;TermFrequency;CValue;inHeader;TermRule;TermRepeatRule");
            foreach (var term in nonDictTermsAr.TermsAr)
            {
                var termFragment = term.TermFragment;
                var termType = "";
                switch (term.Kind)
                {
                    case AuthTerm:
                        termType = "AuthTerm";
                        break;
                    case KindOfTerm.CombTerm:
                        termType = "CombTerm";
                        break;
                    case DictTerm:
                        termType = "DictTerm";
                        break;
                    case KindOfTerm.NonDictTerm:
                        termType = "NonDictTerm";
                        break;
                    case KindOfTerm.SynTerm:
                        termType = "SynTerm";
                        break;
                    case Null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var termRule = term.Rule.ToString();
                var termRepeatRule = term.RepeatRule.ToString();
                var termFrequency = term.Frequency.ToString();
                var termCValue = term.CValue.ToString();
                serializeWriter.WriteLine(term.TermWord + ";" + termFragment + ";" + term.mainPage.ToString() + ";" + termType + ";" + termFrequency + ";" + termCValue + ";" + term.inHeader.ToString() + ";" +
                                          termRule + ";" + termRepeatRule);
            }
            serializeWriter.Close();
        }

        // Правила фильтрации
        private static void Rule1_Mauth_to_M(Terms authTermsAr, Terms mainArrayTermsAr, int rule = 1, int repeatRule = 0)
        {
            for (var i = 0; i < authTermsAr.TermsAr.Count; i++)
            {
                //int k = FindFunctions.findINList(MainArrayTermsAr.TermsAr, AuthTermsAr.TermsAr[i].TermWord);
                var k =
                    mainArrayTermsAr.TermsAr.FindIndex(
                        item =>
                            item.TermWord == authTermsAr.TermsAr[i].TermWord &&
                            item.PoSstr == authTermsAr.TermsAr[i].PoSstr);
                if (k != -1) continue;
                if (authTermsAr.TermsAr[i].Pos.Count <= 0) continue;
                authTermsAr.TermsAr[i].SetToDel = true;
                var newEl = new Term
                {
                    Frequency = authTermsAr.TermsAr[i].Frequency,
                    Kind = AuthTerm,
                    NPattern = authTermsAr.TermsAr[i].NPattern,
                    PatCounter = 0,
                    Pattern = authTermsAr.TermsAr[i].Pattern,
                    SetToDel = false,
                    TermFragment = authTermsAr.TermsAr[i].TermFragment,
                    TermWord = authTermsAr.TermsAr[i].TermWord,
                    PoSstr = authTermsAr.TermsAr[i].PoSstr
                };
                newEl.Pos.Add(null);
                newEl.Rule = rule;
                newEl.RepeatRule = repeatRule;
                var e = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[0].Range);
                if (e == null)
                {
                    mainArrayTermsAr.RootTermsTree.AddRange(authTermsAr.TermsAr[i].Pos[0].Range);
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    e = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[0].Range);
                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }
                else
                {
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }


                for (var j = 1; j < authTermsAr.TermsAr[i].Pos.Count; j++)
                {
                    var node = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[j].Range);
                    if (node == null)
                    {
                        mainArrayTermsAr.RootTermsTree.AddRange(authTermsAr.TermsAr[i].Pos[j].Range);
                        node = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[j].Range);
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                    else
                    {
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                }
            }
            DelElementsWhichSetToDel(authTermsAr);
            getFrequency_(mainArrayTermsAr);
        } //Work

        private static void Rule2_Mdict_to_M(Terms dictTermsAr, NonDictTerms nonDictTermsAr, Terms mainArrayTermsAr,
            int rule = 2, int repeatRule = 0)
        {
            var sizeDict = dictTermsAr.TermsAr.Count;
            for (var i = 0; i < sizeDict; i++)
            {
                //int k = FindFunctions.findINList(MainArrayTermsAr.TermsAr, DictTermsAr.TermsAr[i].TermWord);
                var k =
                    mainArrayTermsAr.TermsAr.FindIndex(
                        item =>
                            item.TermWord == dictTermsAr.TermsAr[i].TermWord &&
                            item.PoSstr == dictTermsAr.TermsAr[i].PoSstr);
                if (k != -1) continue;
                var noComp = false;
                foreach (var curTermTreePos in dictTermsAr.TermsAr[i].Pos)
                {
                    var e = nonDictTermsAr.RootTermsTree.FindRangeExtension(curTermTreePos.Range);
                    if (e == null)
                        noComp = true;
                    if (noComp) break;
                }
                if (!noComp) continue;
                {
                    dictTermsAr.TermsAr[i].SetToDel = true;
                    var newEl = new Term
                    {
                        Frequency = dictTermsAr.TermsAr[i].Frequency,
                        Kind = DictTerm,
                        NPattern = dictTermsAr.TermsAr[i].NPattern,
                        PatCounter = 0,
                        Pattern = dictTermsAr.TermsAr[i].Pattern
                    };
                    newEl.Pos.Add(null);
                    newEl.SetToDel = false;
                    newEl.TermFragment = dictTermsAr.TermsAr[i].TermFragment;
                    newEl.TermWord = dictTermsAr.TermsAr[i].TermWord;
                    newEl.TermWord = dictTermsAr.TermsAr[i].TermWord;
                    newEl.PoSstr = dictTermsAr.TermsAr[i].PoSstr;
                    newEl.Rule = rule;
                    newEl.RepeatRule = repeatRule;
                    var e = mainArrayTermsAr.RootTermsTree.FindRange(dictTermsAr.TermsAr[i].Pos[0].Range);
                    if (e == null)
                    {
                        mainArrayTermsAr.RootTermsTree.AddRange(dictTermsAr.TermsAr[i].Pos[0].Range);
                        e = mainArrayTermsAr.RootTermsTree.FindRange(dictTermsAr.TermsAr[i].Pos[0].Range);
                        mainArrayTermsAr.TermsAr.Add(newEl);
                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                            mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                    }
                    else
                    {
                        mainArrayTermsAr.TermsAr.Add(newEl);
                        if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                            mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                    }
                    for (var j = 1; j < dictTermsAr.TermsAr[i].Pos.Count; j++)
                    {
                        var node = mainArrayTermsAr.RootTermsTree.FindRange(dictTermsAr.TermsAr[i].Pos[j].Range);
                        if (node == null)
                        {
                            mainArrayTermsAr.RootTermsTree.AddRange(dictTermsAr.TermsAr[i].Pos[j].Range);
                            node = mainArrayTermsAr.RootTermsTree.FindRange(dictTermsAr.TermsAr[i].Pos[j].Range);
                            if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                            mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                        }
                        else
                        {
                            if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                            mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                        }
                    }
                }
            }
            /*for(int i=0; i<sizeDict; i++)
            {
            string str=DictTermsAr.TermsAr[i].TermWord;
            ComponentInElement find=FindFunctions.findINListComponents(NonDictTermsAr.TermsAr, str);
            if (FindFunctions.Component != -1)
            {
            if (FindFunctions.findINList(MainArrayTermsAr.TermsAr,DictTermsAr.TermsAr[i].TermWord) == -1)
            MainArrayTermsAr.TermsAr.Add(DictTermsAr.TermsAr[i]);
            }
            else if (FindFunctions.findINList(NonDictTermsAr.TermsAr,str) != -1)
            {
            if (FindFunctions.findINList(MainArrayTermsAr.TermsAr,DictTermsAr.TermsAr[i].TermWord) == -1)
            MainArrayTermsAr.TermsAr.Add(DictTermsAr.TermsAr[i]);
            }
            }*/
            getFrequency_(mainArrayTermsAr);
        }

        private static void Rule3_Mnondict_to_M(Terms dictTermsAr, NonDictTerms nonDictTermsAr, CombTerms combTermsAr,
            Terms mainArrayTermsAr, int rule = 3, int repeatRule = 0) //Переписать!!!
        {
            var sNd = nonDictTermsAr.TermsAr.Count;
            //vector<int> DictTermsToDel;
            for (var i = 0; i < sNd; i++)
            {
                //int res = FindFunctions.findINList(CombTermsAr.TermsAr, NonDictTermsAr.TermsAr[i].TermWord);
                var res = combTermsAr.TermsAr.FindIndex(item => item.TermWord == nonDictTermsAr.TermsAr[i].TermWord);
                if (res == -1) continue;
                {
                    var del = false;
                    foreach (var curNonDictBlock in nonDictTermsAr.TermsAr[i].Blocks)
                    {
                        var countDictComponents = new List<int>();
                        foreach (var curNonDictComponent in curNonDictBlock.Components)
                        {
                            //int r = FindFunctions.findINList(DictTermsAr.TermsAr, NonDictTermsAr.TermsAr[i].Blocks[j].Components[k].Component);
                            var r =
                                dictTermsAr.TermsAr.FindIndex(
                                    item =>
                                        item.TermWord == curNonDictComponent.Component &&
                                        item.PoSstr == curNonDictComponent.PoSstr);
                            if (r != -1)
                                countDictComponents.Add(r);
                            //if (!isDictComponents) break;
                        }
                        //if (isDictComponents)
                        if (countDictComponents.Count == curNonDictBlock.Components.Count)
                        {
                            del = true;
                            while (countDictComponents.Count > 0)
                            {
                                dictTermsAr.TermsAr[countDictComponents[0]].SetToDel = true;
                                var index = countDictComponents[0];
                                //DictTermsToDel.insert(DictTermsToDel.end(), countDictComponents.begin(), countDictComponents.end());
                                //if (FindFunctions.findINList(MainArrayTermsAr.TermsAr, DictTermsAr.TermsAr[countDictComponents[0]].TermWord) == -1)
                                if (mainArrayTermsAr.TermsAr.FindIndex(
                                        item => item.TermWord == dictTermsAr.TermsAr[index].TermWord &&
                                                item.PoSstr == dictTermsAr.TermsAr[index].PoSstr) == -1)
                                {
                                    var newEl = new Term
                                    {
                                        Frequency = dictTermsAr.TermsAr[countDictComponents[0]].Frequency,
                                        Kind = DictTerm,
                                        NPattern = dictTermsAr.TermsAr[countDictComponents[0]].NPattern,
                                        PatCounter = 0,
                                        Pattern = dictTermsAr.TermsAr[countDictComponents[0]].Pattern
                                    };
                                    newEl.Pos.Add(null);
                                    newEl.SetToDel = false;
                                    newEl.TermFragment = dictTermsAr.TermsAr[countDictComponents[0]].TermFragment;
                                    newEl.PoSstr = dictTermsAr.TermsAr[countDictComponents[0]].PoSstr;
                                    newEl.TermWord = dictTermsAr.TermsAr[countDictComponents[0]].TermWord;
                                    newEl.Rule = rule;
                                    newEl.RepeatRule = repeatRule;
                                    var e =
                                        mainArrayTermsAr.RootTermsTree.FindRange(
                                            dictTermsAr.TermsAr[countDictComponents[0]].Pos[0].Range);
                                    if (e == null)
                                    {
                                        mainArrayTermsAr.RootTermsTree.AddRange(
                                            dictTermsAr.TermsAr[countDictComponents[0]].Pos[0].Range);
                                        e =
                                            mainArrayTermsAr.RootTermsTree.FindRange(
                                                dictTermsAr.TermsAr[countDictComponents[0]].Pos[0].Range);
                                        mainArrayTermsAr.TermsAr.Add(newEl);
                                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                                                mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1]
                                            = e;
                                    }
                                    else
                                    {
                                        mainArrayTermsAr.TermsAr.Add(newEl);
                                        if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                            e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                                                mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1]
                                            = e;
                                    }
                                    for (var k = 1; k < dictTermsAr.TermsAr[countDictComponents[0]].Pos.Count; k++)
                                    {
                                        var node =
                                            mainArrayTermsAr.RootTermsTree.FindRange(
                                                dictTermsAr.TermsAr[countDictComponents[0]].Pos[k].Range);
                                        if (node == null)
                                        {
                                            mainArrayTermsAr.RootTermsTree.AddRange(
                                                dictTermsAr.TermsAr[countDictComponents[0]].Pos[k].Range);
                                            node =
                                                mainArrayTermsAr.RootTermsTree.FindRange(
                                                    dictTermsAr.TermsAr[countDictComponents[0]].Pos[k].Range);
                                            if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                                node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                            mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                                        }
                                        else
                                        {
                                            if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                                node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                            mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                                        }
                                    }
                                    countDictComponents.RemoveAt(0);
                                }
                                else
                                {
                                    countDictComponents.RemoveAt(0);
                                }
                            }
                        }
                        else if (countDictComponents.Count == 1)
                        {
                            var isDictComponents = false;
                            foreach (var corPosTermTree in dictTermsAr.TermsAr[countDictComponents[0]].Pos)
                            {
                                isDictComponents = false;
                                var e =
                                    nonDictTermsAr.RootTermsTree.FindRangeExtension(
                                        corPosTermTree.Range);
                                if (e != null && e.IndexElement.Contains(i))
                                    isDictComponents = true;
                                if (!isDictComponents) break;
                            }
                            if (!isDictComponents) continue;
                            {
                                dictTermsAr.TermsAr[countDictComponents[0]].SetToDel = true;
                                //DictTermsToDel.Add(countDictComponents[0]);
                                //if (FindFunctions.findINList(MainArrayTermsAr.TermsAr, NonDictTermsAr.TermsAr[i].TermWord) == -1)
                                if (mainArrayTermsAr.TermsAr.FindIndex(
                                        item => item.TermWord == nonDictTermsAr.TermsAr[i].TermWord &&
                                                item.PoSstr == nonDictTermsAr.TermsAr[i].PoSstr) != -1) continue;
                                var newEl = new Term
                                {
                                    Frequency = nonDictTermsAr.TermsAr[i].Frequency,
                                    Kind = KindOfTerm.NonDictTerm,
                                    NPattern = nonDictTermsAr.TermsAr[i].NPattern,
                                    PatCounter = 0,
                                    Pattern = nonDictTermsAr.TermsAr[i].Pattern
                                };
                                newEl.Pos.Add(null);
                                newEl.SetToDel = false;
                                newEl.TermFragment = nonDictTermsAr.TermsAr[i].TermFragment;
                                newEl.TermWord = nonDictTermsAr.TermsAr[i].TermWord;
                                newEl.PoSstr = nonDictTermsAr.TermsAr[i].PoSstr;
                                newEl.Rule = rule;
                                newEl.RepeatRule = repeatRule;
                                var e =
                                    mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                                if (e == null)
                                {
                                    mainArrayTermsAr.RootTermsTree.AddRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                                    e =
                                        mainArrayTermsAr.RootTermsTree.FindRange(
                                            nonDictTermsAr.TermsAr[i].Pos[0].Range);
                                    mainArrayTermsAr.TermsAr.Add(newEl);
                                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                                            mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1]
                                        = e;
                                }
                                else
                                {
                                    mainArrayTermsAr.TermsAr.Add(newEl);
                                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                                            mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1]
                                        = e;
                                }
                                for (var k = 1; k < nonDictTermsAr.TermsAr[i].Pos.Count; k++)
                                {
                                    var node =
                                        mainArrayTermsAr.RootTermsTree.FindRange(
                                            nonDictTermsAr.TermsAr[i].Pos[k].Range);
                                    if (node == null)
                                    {
                                        mainArrayTermsAr.RootTermsTree.AddRange(
                                            nonDictTermsAr.TermsAr[i].Pos[k].Range);
                                        node =
                                            mainArrayTermsAr.RootTermsTree.FindRange(
                                                nonDictTermsAr.TermsAr[i].Pos[k].Range);
                                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                                    }
                                    else
                                    {
                                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                                    }
                                }
                                countDictComponents.RemoveAt(0);
                            }
                        }
                    }
                    if (!del) continue;
                    {
                        while (nonDictTermsAr.TermsAr[i].Pos.Count > 0)
                        {
                            if (nonDictTermsAr.TermsAr[i].Pos[0].IndexElement.Contains(i))
                                nonDictTermsAr.TermsAr[i].Pos[0].IndexElement.Remove(i);
                            //var l = nonDictTermsAr.TermsAr[i].Pos[0].IndexElement.FindIndex(item => item == i);
                            //nonDictTermsAr.TermsAr[i].Pos[0].IndexElement.RemoveAt(l);
                            if (nonDictTermsAr.TermsAr[i].Pos[0].IndexElement.Count == 0)
                            {
                                var delRange = new Range(nonDictTermsAr.TermsAr[i].Pos[0].Range.Inf);
                                nonDictTermsAr.RootTermsTree.DeleteRange(delRange);
                            }
                            nonDictTermsAr.TermsAr[i].Pos.RemoveAt(0);
                        }
                        nonDictTermsAr.TermsAr.RemoveAt(i);
                        ChangeIdexesInTree(nonDictTermsAr, i);
                        i--;
                        sNd = nonDictTermsAr.TermsAr.Count;
                        while (combTermsAr.TermsAr[res].Pos.Count > 0)
                        {
                            if (combTermsAr.TermsAr[res].Pos[0].IndexElement.Contains(res))
                                combTermsAr.TermsAr[res].Pos[0].IndexElement.Remove(res);
                            //var l = combTermsAr.TermsAr[res].Pos[0].IndexElement.FindIndex(item => item == res);
                            //combTermsAr.TermsAr[res].Pos[0].IndexElement.RemoveAt(l);
                            if (combTermsAr.TermsAr[res].Pos[0].IndexElement.Count == 0)
                            {
                                var delRange = new Range(combTermsAr.TermsAr[res].Pos[0].Range.Inf);
                                combTermsAr.RootTermsTree.DeleteRange(delRange);
                            }
                            combTermsAr.TermsAr[res].Pos.RemoveAt(0);
                        }
                        combTermsAr.TermsAr.RemoveAt(res);
                        ChangeIdexesInTree(combTermsAr, res);
                        //===========Заглушка============
                        //for (int p = 0; p < NonDictTermsAr.TermsAr.Count; p++)
                        //{
                        //    for (int j = 0; j < NonDictTermsAr.TermsAr[p].Pos.Count; j++)
                        //        if (!NonDictTermsAr.TermsAr[p].Pos[j].indexElement.Contains(p)) NonDictTermsAr.TermsAr[p].Pos[j].indexElement.Add(p);
                        //}
                        //for (int p = 0; p < CombTermsAr.TermsAr.Count; p++)
                        //{
                        //    for (int j = 0; j < CombTermsAr.TermsAr[p].Pos.Count; j++)
                        //        if (!CombTermsAr.TermsAr[p].Pos[j].indexElement.Contains(p)) CombTermsAr.TermsAr[p].Pos[j].indexElement.Add(p);
                        //}
                        //==================================
                    }
                }
            }
            DelElementsWhichSetToDel(dictTermsAr);
            getFrequency_(mainArrayTermsAr);
        }

        private static void Rule4_Msyn_to_M(Terms mainTermsAr, SynTerms synTermsAr, int rule = 4, int repeatRule = 0)
        {
            //FindFunctions find = new FindFunctions();
            foreach (var curSynTermPair in synTermsAr.TermsAr)
            {
                var firstAlt =
                    mainTermsAr.TermsAr.FindIndex(
                        item => item.TermWord == curSynTermPair.Alternatives.First.Alternative &&
                                item.PoSstr == curSynTermPair.Alternatives.First.PoSstr);
                var secondAlt =
                    mainTermsAr.TermsAr.FindIndex(
                        item => item.TermWord == curSynTermPair.Alternatives.Second.Alternative &&
                                item.PoSstr == curSynTermPair.Alternatives.Second.PoSstr);
                if (firstAlt != -1 && secondAlt == -1)
                {
                    curSynTermPair.SetToDel = true;
                    var newEl = new Term
                    {
                        Frequency = curSynTermPair.Alternatives.Second.Frequency,
                        Kind = KindOfTerm.SynTerm,
                        NPattern = curSynTermPair.Alternatives.Second.NPattern,
                        PatCounter = 0,
                        Pattern = curSynTermPair.Alternatives.Second.Pattern,
                        SetToDel = false,
                        PoSstr = curSynTermPair.Alternatives.Second.PoSstr,
                        TermFragment = curSynTermPair.TermFragment,
                        TermWord = curSynTermPair.Alternatives.Second.Alternative,
                        Rule = rule,
                        RepeatRule = repeatRule
                    };
                    //MainTermsAr.TermsAr[first_alt].frequency;
                    //newEl.Pos.Add(null);
                    mainTermsAr.TermsAr.Add(newEl);
                    for (var j = 1; j < mainTermsAr.TermsAr[firstAlt].Pos.Count; j++)
                    {
                        mainTermsAr.TermsAr[mainTermsAr.TermsAr.Count - 1].Pos.Add(mainTermsAr.TermsAr[firstAlt].Pos[j]);
                        if (!mainTermsAr.TermsAr[firstAlt].Pos[j].IndexElement.Contains(mainTermsAr.TermsAr.Count - 1))
                            mainTermsAr.TermsAr[firstAlt].Pos[j].IndexElement.Add(mainTermsAr.TermsAr.Count - 1);
                    }
                }
                else if (firstAlt == -1 && secondAlt != -1)
                {
                    curSynTermPair.SetToDel = true;
                    var newEl = new Term
                    {
                        Frequency = curSynTermPair.Alternatives.First.Frequency,
                        Kind = KindOfTerm.SynTerm,
                        NPattern = curSynTermPair.Alternatives.First.NPattern,
                        PatCounter = 0,
                        Pattern = curSynTermPair.Alternatives.First.Pattern,
                        SetToDel = false,
                        TermFragment = curSynTermPair.TermFragment,
                        TermWord = curSynTermPair.Alternatives.First.Alternative,
                        PoSstr = curSynTermPair.Alternatives.First.PoSstr,
                        Rule = rule,
                        RepeatRule = repeatRule
                    };
                    //MainTermsAr.TermsAr[second_alt].frequency;
                    //newEl.Pos.Add(null);
                    mainTermsAr.TermsAr.Add(newEl);
                    for (var j = 1; j < mainTermsAr.TermsAr[secondAlt].Pos.Count; j++)
                    {
                        mainTermsAr.TermsAr[mainTermsAr.TermsAr.Count - 1].Pos.Add(mainTermsAr.TermsAr[secondAlt].Pos[j]);
                        if (!mainTermsAr.TermsAr[secondAlt].Pos[j].IndexElement.Contains(mainTermsAr.TermsAr.Count - 1))
                            mainTermsAr.TermsAr[secondAlt].Pos[j].IndexElement.Add(mainTermsAr.TermsAr.Count - 1);
                    }
                }
            }
            DelElementsWhichSetToDel(synTermsAr);
            getFrequency_(mainTermsAr);
        }

        private static void Rule5_Mcomb_to_M(Terms dictTermsAr, NonDictTerms nonDictTermsAr, CombTerms combTermsAr,
            Terms mainArrayTermsAr, int rule = 5, int repeatRule = 0)
        {
            var sComb = combTermsAr.TermsAr.Count;
            for (var i = 0; i < sComb; i++)
            {
                var resNonDict = nonDictTermsAr.TermsAr.FindIndex(item =>
                    combTermsAr.TermsAr[i].Components.FindIndex(compItem =>
                        compItem.TermWord == item.TermWord && compItem.PoSstr == item.PoSstr) != -1);
                if (resNonDict != -1)
                {
                    foreach (CombComponent curCombComponent in combTermsAr.TermsAr[i].Components)
                    {
                        var mT =
                            mainArrayTermsAr.TermsAr.FindIndex(
                                item => item.TermWord == curCombComponent.TermWord &&
                                        item.PoSstr == curCombComponent.PoSstr);
                        if (mT != -1) continue;
                        var newEl = new Term
                        {
                            Frequency = curCombComponent.Frequency,
                            Kind = KindOfTerm.CombTerm,
                            NPattern = curCombComponent.NPattern,
                            PatCounter = 0,
                            Pattern = curCombComponent.Pattern
                        };
                        newEl.Pos.Add(null);
                        newEl.SetToDel = false;
                        newEl.TermFragment = curCombComponent.TermFragment;
                        newEl.PoSstr = curCombComponent.PoSstr;
                        newEl.TermWord = curCombComponent.TermWord;
                        mainArrayTermsAr.TermsAr.Add(newEl);
                        newEl.Rule = rule;
                        newEl.RepeatRule = repeatRule;
                        for (var j = 0; j < combTermsAr.TermsAr[i].Pos.Count; j++)
                        {
                            var e = mainArrayTermsAr.RootTermsTree.FindRange(combTermsAr.TermsAr[i].Pos[j].Range);
                            if (e == null)
                            {
                                mainArrayTermsAr.RootTermsTree.AddRange(combTermsAr.TermsAr[i].Pos[j].Range);
                                e = mainArrayTermsAr.RootTermsTree.FindRange(combTermsAr.TermsAr[i].Pos[j].Range);
                                e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                            }
                            else
                            {
                                if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                            }
                        }
                    }
                    for (var j = 0; j < combTermsAr.TermsAr[i].Pos.Count; j++)
                    {
                        combTermsAr.TermsAr[i].Pos[j].IndexElement.Remove(i);
                        if (combTermsAr.TermsAr[i].Pos[j].IndexElement.Count == 0)
                            combTermsAr.RootTermsTree.DeleteRange(combTermsAr.TermsAr[i].Pos[j].Range);
                        combTermsAr.TermsAr[i].Pos.RemoveAt(j);
                    }
                    combTermsAr.TermsAr.RemoveAt(i);
                    for (var j = 0; j < nonDictTermsAr.TermsAr[resNonDict].Pos.Count; j++)
                    {
                        nonDictTermsAr.TermsAr[resNonDict].Pos[j].IndexElement.Remove(resNonDict);
                        if (nonDictTermsAr.TermsAr[resNonDict].Pos[j].IndexElement.Count == 0)
                            nonDictTermsAr.RootTermsTree.DeleteRange(nonDictTermsAr.TermsAr[resNonDict].Pos[j].Range);
                        nonDictTermsAr.TermsAr[resNonDict].Pos.RemoveAt(j);
                    }
                    nonDictTermsAr.TermsAr.RemoveAt(resNonDict);
                    i--;
                    sComb = combTermsAr.TermsAr.Count;
                    continue;
                }
                var resDict = dictTermsAr.TermsAr.FindIndex(item =>
                    combTermsAr.TermsAr[i].Components.FindIndex(compItem =>
                        compItem.TermWord == item.TermWord && compItem.PoSstr == item.PoSstr) != -1);
                if (resDict != -1)
                {
                    foreach (var curCombComponent in combTermsAr.TermsAr[i].Components)
                        if (mainArrayTermsAr.TermsAr.FindIndex(
                                item => item.TermWord == curCombComponent.TermWord &&
                                        item.PoSstr == curCombComponent.PoSstr) == -1)
                        {
                            var newEl = new Term
                            {
                                Frequency = curCombComponent.Frequency,
                                Kind = KindOfTerm.CombTerm,
                                NPattern = curCombComponent.NPattern,
                                PatCounter = 0,
                                Pattern = curCombComponent.Pattern
                            };
                            newEl.Pos.Add(null);
                            newEl.SetToDel = false;
                            newEl.TermFragment = curCombComponent.TermFragment;
                            newEl.PoSstr = curCombComponent.PoSstr;
                            newEl.TermWord = curCombComponent.TermWord;
                            mainArrayTermsAr.TermsAr.Add(newEl);
                            newEl.Rule = rule;
                            newEl.RepeatRule = repeatRule;
                            for (var j = 0; j < combTermsAr.TermsAr[i].Pos.Count; j++)
                            {
                                var e = mainArrayTermsAr.RootTermsTree.FindRange(combTermsAr.TermsAr[i].Pos[j].Range);
                                if (e == null)
                                {
                                    mainArrayTermsAr.RootTermsTree.AddRange(combTermsAr.TermsAr[i].Pos[j].Range);
                                    e = mainArrayTermsAr.RootTermsTree.FindRange(combTermsAr.TermsAr[i].Pos[j].Range);
                                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                }
                                else
                                {
                                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                }
                            }
                        }
                    for (var j = 0; j < combTermsAr.TermsAr[i].Pos.Count; j++)
                    {
                        combTermsAr.TermsAr[i].Pos[j].IndexElement.Remove(i);
                        if (combTermsAr.TermsAr[i].Pos[j].IndexElement.Count == 0)
                            combTermsAr.RootTermsTree.DeleteRange(combTermsAr.TermsAr[i].Pos[j].Range);
                        combTermsAr.TermsAr[i].Pos.RemoveAt(j);
                    }
                    combTermsAr.TermsAr.RemoveAt(i);
                    for (var j = 0; j < nonDictTermsAr.TermsAr[resDict].Pos.Count; j++)
                    {
                        nonDictTermsAr.TermsAr[resDict].Pos[j].IndexElement.Remove(resNonDict);
                        if (nonDictTermsAr.TermsAr[resDict].Pos[j].IndexElement.Count == 0)
                            nonDictTermsAr.RootTermsTree.DeleteRange(nonDictTermsAr.TermsAr[resDict].Pos[j].Range);
                        nonDictTermsAr.TermsAr[resDict].Pos.RemoveAt(j);
                    }
                    dictTermsAr.TermsAr.RemoveAt(resDict);
                    i--;
                    sComb = combTermsAr.TermsAr.Count;
                    continue;
                }
                if (resDict == -1) continue;
                {
                    foreach (var curCombComponent in combTermsAr.TermsAr[i].Components)
                        if (mainArrayTermsAr.TermsAr.FindIndex(
                                item => item.TermWord == curCombComponent.TermWord &&
                                        item.PoSstr == curCombComponent.PoSstr) == -1)
                        {
                            var newEl = new Term
                            {
                                Frequency = curCombComponent.Frequency,
                                Kind = KindOfTerm.CombTerm,
                                NPattern = curCombComponent.NPattern,
                                PatCounter = 0,
                                Pattern = curCombComponent.Pattern
                            };
                            newEl.Pos.Add(null);
                            newEl.SetToDel = false;
                            newEl.TermFragment = curCombComponent.TermFragment;
                            newEl.PoSstr = curCombComponent.PoSstr;
                            newEl.TermWord = curCombComponent.TermWord;
                            mainArrayTermsAr.TermsAr.Add(newEl);
                            newEl.Rule = rule;
                            newEl.RepeatRule = repeatRule;
                            for (var j = 0; j < combTermsAr.TermsAr[i].Pos.Count; j++)
                            {
                                var e = mainArrayTermsAr.RootTermsTree.FindRange(combTermsAr.TermsAr[i].Pos[j].Range);
                                if (e == null)
                                {
                                    mainArrayTermsAr.RootTermsTree.AddRange(combTermsAr.TermsAr[i].Pos[j].Range);
                                    e = mainArrayTermsAr.RootTermsTree.FindRange(combTermsAr.TermsAr[i].Pos[j].Range);
                                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                }
                                else
                                {
                                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                }
                            }
                        }
                    for (var j = 0; j < combTermsAr.TermsAr[i].Pos.Count; j++)
                    {
                        combTermsAr.TermsAr[i].Pos[j].IndexElement.Remove(i);
                        if (combTermsAr.TermsAr[i].Pos[j].IndexElement.Count == 0)
                            combTermsAr.RootTermsTree.DeleteRange(combTermsAr.TermsAr[i].Pos[j].Range);
                        combTermsAr.TermsAr[i].Pos.RemoveAt(j);
                    }
                    combTermsAr.TermsAr.RemoveAt(i);
                    i--;
                    sComb = combTermsAr.TermsAr.Count;
                }
            }
        }

        private static void Rule6_repeat_Rule5(Terms dictTermsAr, NonDictTerms nonDictTermsAr, CombTerms combTermsAr,
            Terms mainArrayTermsAr, int repeatRule = 6)
        {
            var oldSizeM = mainArrayTermsAr.TermsAr.Count;
            Rule5_Mcomb_to_M(dictTermsAr, nonDictTermsAr, combTermsAr, mainArrayTermsAr, 5, repeatRule);
            var newSizwM = mainArrayTermsAr.TermsAr.Count;
            while (oldSizeM != newSizwM)
            {
                Rule5_Mcomb_to_M(dictTermsAr, nonDictTermsAr, combTermsAr, mainArrayTermsAr, 5, repeatRule);
                oldSizeM = newSizwM;
                newSizwM = mainArrayTermsAr.TermsAr.Count;
            }
        }

        private static double GetAverageFrequency(NonDictTerms nonDictTermsAr)
        {
            double avrfrequency = 0;
            double count = nonDictTermsAr.TermsAr.Count;
            for (var i = 0; i < count; i++)
                avrfrequency += nonDictTermsAr.TermsAr[i].Frequency;
            avrfrequency = avrfrequency / count;
            return Math.Floor(avrfrequency);
        }

        private static void Rule7_Mnondict_to_M(NonDictTerms nonDictTermsAr, CombTerms combTermsAr,
            Terms mainArrayTermsAr, int rule = 7, int repeatRule = 0)
        {
            var avrFrequency = GetAverageFrequency(nonDictTermsAr);
            //foreach (NonDictTerm term in NonDictTermsAr.TermsAr)
            for (var i = 0; i < nonDictTermsAr.TermsAr.Count; i++)
            {
                var p =
                    mainArrayTermsAr.TermsAr.FindIndex(
                        mainTerm => mainTerm.TermWord == nonDictTermsAr.TermsAr[i].TermWord &&
                                    mainTerm.PoSstr == nonDictTermsAr.TermsAr[i].PoSstr);
                if (p != -1 || !(nonDictTermsAr.TermsAr[i].Frequency >= avrFrequency)) continue;
                var s =
                    combTermsAr.TermsAr.FindIndex(
                        combTerm => combTerm.TermWord == nonDictTermsAr.TermsAr[i].TermWord &&
                                    combTerm.PoSstr == nonDictTermsAr.TermsAr[i].PoSstr);
                if (s != -1 || nonDictTermsAr.TermsAr[i].Pos.Count == 0) continue;
                var newEl = new Term
                {
                    Frequency = nonDictTermsAr.TermsAr[i].Frequency,
                    Kind = KindOfTerm.NonDictTerm,
                    NPattern = nonDictTermsAr.TermsAr[i].NPattern,
                    PatCounter = 0,
                    Pattern = nonDictTermsAr.TermsAr[i].Pattern
                };
                newEl.Pos.Add(null);
                newEl.SetToDel = false;
                newEl.TermFragment = nonDictTermsAr.TermsAr[i].TermFragment;
                newEl.TermWord = nonDictTermsAr.TermsAr[i].TermWord;
                newEl.PoSstr = nonDictTermsAr.TermsAr[i].PoSstr;
                newEl.Rule = rule;
                newEl.RepeatRule = repeatRule;
                var e = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                if (e == null)
                {
                    mainArrayTermsAr.RootTermsTree.AddRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                    e = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }
                else
                {
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }
                for (var k = 1; k < nonDictTermsAr.TermsAr[i].Pos.Count; k++)
                {
                    var node = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[k].Range);
                    if (node == null)
                    {
                        mainArrayTermsAr.RootTermsTree.AddRange(nonDictTermsAr.TermsAr[i].Pos[k].Range);
                        node = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[k].Range);
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                    else
                    {
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                }
                nonDictTermsAr.TermsAr.RemoveAt(i);
                i--;
            }
        }

        private void Rule8_from_4_to_7(Terms mainArrayTermsAr)
        {
            var oldSizeM = mainArrayTermsAr.TermsAr.Count;
            Rule7_Mnondict_to_M(_proc.NonDictTermsAr, _proc.CombTermsAr, _proc.MainTermsAr);
            var newSizwM = mainArrayTermsAr.TermsAr.Count;
            while (oldSizeM != newSizwM)
            {
                var synTermsAr = new SynTerms();
                _proc.GetXmlSynTerms(synTermsAr);
                Rule4_Msyn_to_M(_proc.MainTermsAr, synTermsAr, 4, 8);
                Rule6_repeat_Rule5(_proc.DictTermsAr, _proc.NonDictTermsAr, _proc.CombTermsAr, _proc.MainTermsAr, 48);
                Rule7_Mnondict_to_M(_proc.NonDictTermsAr, _proc.CombTermsAr, _proc.MainTermsAr, 7, 8);
                oldSizeM = newSizwM;
                newSizwM = mainArrayTermsAr.TermsAr.Count;
            }
            ////TermsProcessing proc = new TermsProcessing();
            //SynTerms SynTermsAr = new SynTerms();
            //proc.GetXMLSynTerms(SynTermsAr);
            //Rule4_Msyn_to_M(MainTermsAr, SynTermsAr);
        }

        public Terms ApplyRules()
        {
            // Инициализируем массивы терминов
            _proc.GetXmlAuthTerms(_proc.AuthTermsAr);
            _proc.GetXmlCombTerms(_proc.CombTermsAr);
            _proc.GetXmlDictTerms(_proc.DictTermsAr);
            _proc.GetXmlNonDictTerms(_proc.NonDictTermsAr);
            var synTermsAr = new SynTerms();
            //_proc.GetXmlSynTermsFromTermsAr(synTermsAr, _proc.AuthTermsAr);
            _proc.GetXmlSynTerms(synTermsAr);

            _proc.CheckTermsInHeaders(synTermsAr);

            //Получаем частоты 
            GetXmlTermsFrequencies(_proc.AuthTermsAr, "_auth_terms");
            GetXmlTermsFrequencies(_proc.DictTermsAr, "_dict_terms");
            GetXmlCombTermsFrequencies(_proc.CombTermsAr);
            GetXmlNonDictTermsArFrequencies(_proc.NonDictTermsAr);
            GetXmlSynTermsFrequencies(synTermsAr);
            Rule1(_proc.AuthTermsAr, _proc.MainTermsAr); // Mauth_to_M
            Rule2(_proc.NonDictTermsAr, _proc.MainTermsAr);
            Rule3(_proc.NonDictTermsAr, _proc.AuthTermsAr, _proc.MainTermsAr);
            Rule4(_proc.AuthTermsAr, _proc.MainTermsAr);
            
            Rule5(_proc.AuthTermsAr, _proc.MainTermsAr);
            Rule6(_proc.MainTermsAr, synTermsAr);
            Rule7(synTermsAr, _proc.MainTermsAr);

            Rule8(_proc.NonDictTermsAr, _proc.MainTermsAr);
            //Rule2(_proc.NonDictTermsAr,_proc.MainTermsAr,2,0);
            PrintTermsFrequencies(_proc.AuthTermsAr, "_auth_terms");
            PrintTermsFrequencies(_proc.DictTermsAr, "_dict_terms");
            PrintCombTermsFrequencies(_proc.CombTermsAr, "_full_comb_terms");
            PrintCombTermsComponentsFrequencies(_proc.CombTermsAr, "_comp_comb_terms");
            PrinNonDictTermsFrequencies(_proc.NonDictTermsAr, "_nondict_terms_ar");
            PrinSynTermsFrequencies(synTermsAr, "_syn_terms");


            ////Применение правила 1
            //Rule1_Mauth_to_M(_proc.AuthTermsAr, _proc.MainTermsAr);
            
            //Rule2(_proc.NonDictTermsAr, _proc.MainTermsAr); // Mnondict_to_M
            //Правило 2
            //Rule2_Mdict_to_M(_proc.DictTermsAr, _proc.NonDictTermsAr, _proc.MainTermsAr);
            //Правило 3
            //Rule3_Mnondict_to_M(_proc.DictTermsAr, _proc.NonDictTermsAr, _proc.CombTermsAr, _proc.MainTermsAr);
            //Правило 4
            //Rule4_Msyn_to_M(_proc.MainTermsAr, synTermsAr);
            //Правило 5 и 6
            //Rule6_repeat_Rule5(_proc.DictTermsAr, _proc.NonDictTermsAr, _proc.CombTermsAr, _proc.MainTermsAr);
            //Правила 7 и 8
            //Rule8_from_4_to_7(_proc.MainTermsAr);
            foreach (var term in _proc.MainTermsAr.TermsAr)
                term.FreqStart = true;
            ClearDublicates(_proc.MainTermsAr);
            //DeleteWhichHaveExtended(_proc.MainTermsAr);
            GetXmlTermsFrequencies(_proc.MainTermsAr, "_main_terms");
            PrintTermsFrequencies(_proc.MainTermsAr, "_main_terms");
            return _proc.MainTermsAr;
        }

        public void GetPagesSets(Terms curTermsAr, int c_num = 3) //DBScan
        {
            foreach (var term in curTermsAr.TermsAr)
            {
                int clusters_num = c_num;
                List<MyCustomDatasetItem> pages = new List<MyCustomDatasetItem>();
                foreach (var curPos in term.Pos)
                {
                    int page = _proc.GetPageNumberByPositon(curPos.Range);
                    //if(!pages.Any(item => item.number == page))
                    pages.Add(new MyCustomDatasetItem(page));
                }

                MyCustomDatasetItem[] featureData = { };
                featureData = pages.ToArray();
                HashSet<MyCustomDatasetItem[]> clusters;

                var dbs = new DbscanAlgorithm<MyCustomDatasetItem>((x, y) => Math.Abs(x.number - y.number));
                dbs.ComputeClusterDbscan(allPoints: featureData, epsilon: 3, minPts: 1, clusters: out clusters);
                List<MyCustomDatasetItem[]> pageClusters = clusters.ToList().OrderBy(item => item.OrderBy(item2 => item2.number).ToList()[0].number).ThenByDescending(item => item.Length).ToList();
                term.pages = pageClusters.GetRange(0, Math.Min(clusters_num, pageClusters.Count));
            }
        }

        public void GetPagesSetsAlt1(Terms curTermsAr, int c_num = 3) //Forel
        {
            foreach (var term in curTermsAr.TermsAr)
            {
                int clusters_num = c_num;
                List<double> pages = new List<double>();
                foreach (var curPos in term.Pos)
                {
                    int page = _proc.GetPageNumberByPositon(curPos.Range);
                    //if(!pages.Any(item => item.number == page))
                    pages.Add(page);
                }
                //if (pages.Count > 10)
                //{
                    double[][] clusters;
                    //for (double r = -10; r <= 10; r += 1)
                    //{
                        clusters = Forel.Solve(pages.ToArray(), 4, (x, y) => Math.Abs(x - y), a =>
                        {
                            double s = 0.0;
                            foreach (double v in a)
                                s += v;
                            return s / a.Length;
                        });
                    List<MyCustomDatasetItem[]> pageClusters = new List<MyCustomDatasetItem[]>();
                    foreach (var cluster in clusters)
                    {
                        List<MyCustomDatasetItem> new_cluster = new List<MyCustomDatasetItem>();
                        foreach (var page in cluster)
                        {
                            new_cluster.Add(new MyCustomDatasetItem(page));
                        }
                        pageClusters.Add(new_cluster.ToArray());
                    }

                pageClusters = pageClusters.OrderBy(item => item.OrderBy(item2 => item2.number).ToList()[0].number).ThenByDescending(item => item.Length).ToList();
                term.pages = pageClusters.GetRange(0, Math.Min(clusters_num, pageClusters.Count));

                //}
                //}

                //MyCustomDatasetItem[] featureData = { };
                //featureData = pages.ToArray();
                //HashSet<MyCustomDatasetItem[]> clusters;

                //var dbs = new DbscanAlgorithm<MyCustomDatasetItem>((x, y) => Math.Abs(x.number - y.number));
                //dbs.ComputeClusterDbscan(allPoints: featureData, epsilon: 3, minPts: 1, clusters: out clusters);
                //List<MyCustomDatasetItem[]> pageClusters = clusters.ToList().OrderBy(item => item.OrderBy(item2 => item2.number).ToList()[0].number).ThenByDescending(item => item.Length).ToList();
                //term.pages = pageClusters.GetRange(0, Math.Min(4, pageClusters.Count));
            }
        }

        public void GetPagesSetsAlt2(Terms curTermsAr, int c_num=3) //Kmeans
        {
            foreach (var term in curTermsAr.TermsAr)
            {
                int clusters_num = c_num;
                List<double[]> pages = new List<double[]>();
                foreach (var curPos in term.Pos)
                {
                    int page = _proc.GetPageNumberByPositon(curPos.Range);
                    pages.Add(new double[] {page, 0});
                }
                if (pages.Count > 0)
                {
                    int cn = clusters_num;
                    if (clusters_num > pages.Count)
                    {
                        clusters_num = pages.Count;
                        cn = clusters_num;
                    }

                    KMeans kmeans = new KMeans(k: cn); //, distance: (x, y) => Math.Abs(x[0] - y[0]));

                    // Compute and retrieve the data centroids
                    var clusters_centroinds = kmeans.Learn(pages.ToArray());

                    // Use the centroids to parition all the data
                    int[] clusters_labels = clusters_centroinds.Decide(pages.ToArray());
                    List<List<double>> clusters = new List<List<double>>();
                    for (int i = 0; i < clusters_labels.Length; i++)
                    {
                        int cur_label = clusters_labels[i];
                        if (cur_label + 1 > clusters.Count)
                        {
                            for (int j = clusters.Count; j < cur_label + 1; j++)
                            {
                                clusters.Add(new List<double>());
                            }
                        }
                        clusters[cur_label].Add(pages[i][0]);
                    }

                    //double[][] clusters;

                    //clusters = Forel.Solve(pages.ToArray(), 4, (x, y) => Math.Abs(x - y), a =>
                    //{
                    //    double s = 0.0;
                    //    foreach (double v in a)
                    //        s += v;
                    //    return s / a.Length;
                    //});
                    List<MyCustomDatasetItem[]> pageClusters = new List<MyCustomDatasetItem[]>();
                    foreach (var cluster in clusters)
                    {
                        List<MyCustomDatasetItem> new_cluster = new List<MyCustomDatasetItem>();
                        foreach (var page in cluster)
                        {
                            new_cluster.Add(new MyCustomDatasetItem(page));
                        }
                        pageClusters.Add(new_cluster.ToArray());
                    }

                    pageClusters = pageClusters.OrderBy(item => item.OrderBy(item2 => item2.number).ToList()[0].number).ThenByDescending(item => item.Length).ToList();
                    term.pages = pageClusters.GetRange(0, Math.Min(clusters_num, pageClusters.Count));
                }
                else
                {
                    term.pages = (new List<MyCustomDatasetItem[]>()).GetRange(0, 0);
                }
            }
           
        }

        public static T[,] ToMultidimensional<T>(T[][] arr, int maxSize)
        {
            T[,] md = (T[,])Array.CreateInstance(typeof(double), arr.Length, maxSize);

            for (int i = 0; i < arr.Length; i++)
                for (int j = 0; j < arr[i].Length; j++)
                    md[i, j] = arr[i][j];

            return md;
        }

        private double percentileBottom = 0;
        private double percentileTop = 0;
        private void Rule1(Terms authTermsAr, Terms mainArrayTermsAr, int rule = 1, int repeatRule = 0)
        {
            List<double> frequencyList = new List<double>();
            for (var i = 0; i < authTermsAr.TermsAr.Count; i++)
            {
                frequencyList.Add(authTermsAr.TermsAr[i].Frequency);
            }
            percentileBottom = Math.Floor(Percentile(frequencyList, 0.4));
            percentileTop = Math.Ceiling(Percentile(frequencyList, 0.95));
            for (var i = 0; i < authTermsAr.TermsAr.Count; i++)
            {
                var k =
                    mainArrayTermsAr.TermsAr.FindIndex(
                        item =>
                            item.TermFullNormForm == authTermsAr.TermsAr[i].TermFullNormForm &&
                            item.PoSstr == authTermsAr.TermsAr[i].PoSstr);
                if (k != -1) continue;
                if (authTermsAr.TermsAr[i].Pos.Count <= 0) continue;
                if (authTermsAr.TermsAr[i].Frequency < percentileBottom || authTermsAr.TermsAr[i].Frequency > percentileTop)
                {
                    if (authTermsAr.TermsAr[i].type == TypeOfTerm.Trusted)
                        authTermsAr.TermsAr[i].type = TypeOfTerm.UntrastedByFrequency;
                    continue;
                }
                if (authTermsAr.TermsAr[i].type == TypeOfTerm.Untrusted || authTermsAr.TermsAr[i].type == TypeOfTerm.UntrastedByFrequency) continue;
                var newEl = new Term
                {
                    Frequency = authTermsAr.TermsAr[i].Frequency,
                    Kind = AuthTerm,
                    NPattern = authTermsAr.TermsAr[i].NPattern,
                    PatCounter = 0,
                    Pattern = authTermsAr.TermsAr[i].Pattern,
                    SetToDel = false,
                    TermFragment = authTermsAr.TermsAr[i].TermFragment,
                    TermWord = authTermsAr.TermsAr[i].TermWord,
                    PoSstr = authTermsAr.TermsAr[i].PoSstr,
                    TermFullNormForm = authTermsAr.TermsAr[i].TermFullNormForm,
                    CValue = authTermsAr.TermsAr[i].CValue,
                    mainPage = authTermsAr.TermsAr[i].mainPage
                };
                newEl.Pos.Add(null);
                newEl.Rule = rule;
                newEl.RepeatRule = repeatRule;
                var e = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[0].Range);
                if (e == null)
                {
                    mainArrayTermsAr.RootTermsTree.AddRange(authTermsAr.TermsAr[i].Pos[0].Range);
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    e = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[0].Range);
                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }
                else
                {
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }


                for (var j = 1; j < authTermsAr.TermsAr[i].Pos.Count; j++)
                {
                    var node = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[j].Range);
                    if (node == null)
                    {
                        mainArrayTermsAr.RootTermsTree.AddRange(authTermsAr.TermsAr[i].Pos[j].Range);
                        node = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[j].Range);
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                    else
                    {
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                }
            }
            //getFrequency_(mainArrayTermsAr);
        } //Work
        private void Rule2(NonDictTerms nonDictTermsAr, Terms mainArrayTermsAr, int rule = 2, int repeatRule = 0)
        {
            for (var i = 0; i < nonDictTermsAr.TermsAr.Count; i++)
            {
                var k =
                    mainArrayTermsAr.TermsAr.FindIndex(
                        item =>
                            item.TermFullNormForm == nonDictTermsAr.TermsAr[i].TermFullNormForm &&
                            item.PoSstr == nonDictTermsAr.TermsAr[i].PoSstr);
                if (k != -1) continue;
                if (nonDictTermsAr.TermsAr[i].Pos.Count <= 0) continue;
                if (!nonDictTermsAr.TermsAr[i].inHeader) continue;
                if (nonDictTermsAr.TermsAr[i].Frequency < percentileBottom || nonDictTermsAr.TermsAr[i].Frequency > percentileTop)
                    continue;
                var newEl = new Term
                {
                    Frequency = nonDictTermsAr.TermsAr[i].Frequency,
                    Kind = KindOfTerm.NonDictTerm,
                    NPattern = nonDictTermsAr.TermsAr[i].NPattern,
                    PatCounter = 0,
                    Pattern = nonDictTermsAr.TermsAr[i].Pattern,
                    SetToDel = false,
                    TermFragment = nonDictTermsAr.TermsAr[i].TermFragment,
                    TermWord = nonDictTermsAr.TermsAr[i].TermWord,
                    PoSstr = nonDictTermsAr.TermsAr[i].PoSstr,
                    TermFullNormForm = nonDictTermsAr.TermsAr[i].TermFullNormForm
                };
                newEl.Pos.Add(null);
                newEl.Rule = rule;
                newEl.RepeatRule = repeatRule;
                var e = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                if (e == null)
                {
                    mainArrayTermsAr.RootTermsTree.AddRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    e = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }
                else
                {
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }


                for (var j = 1; j < nonDictTermsAr.TermsAr[i].Pos.Count; j++)
                {
                    var node = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[j].Range);
                    if (node == null)
                    {
                        mainArrayTermsAr.RootTermsTree.AddRange(nonDictTermsAr.TermsAr[i].Pos[j].Range);
                        node = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[j].Range);
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                    else
                    {
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                }
            }
            //getFrequency_(mainArrayTermsAr);
        }
        private void Rule3(NonDictTerms nonDictTermsAr, Terms authTermsAr, Terms mainArrayTermsAr, int rule = 3, int repeatRule = 0)
        {
            for (var i = 0; i < nonDictTermsAr.TermsAr.Count; i++)
            {
                var k =
                    mainArrayTermsAr.TermsAr.FindIndex(
                        item =>
                            item.TermFullNormForm == nonDictTermsAr.TermsAr[i].TermFullNormForm &&
                            item.PoSstr == nonDictTermsAr.TermsAr[i].PoSstr);
                if (k != -1) continue;
                if (nonDictTermsAr.TermsAr[i].Pos.Count <= 0) continue;
                if (nonDictTermsAr.TermsAr[i].Frequency < percentileBottom || nonDictTermsAr.TermsAr[i].Frequency > percentileTop)
                    continue;
                bool containWord = false;
                List<string> curTermSplt = nonDictTermsAr.TermsAr[i].TermFullNormForm.Split().ToList();
                foreach (var term in authTermsAr.TermsAr)
                {
                    if (term.type == TypeOfTerm.Untrusted) continue;
                    List<string> termSplt = term.TermFullNormForm?.Split().ToList();
                    foreach (var word in curTermSplt)
                    {
                        if (termSplt.Contains(word))
                        {
                            containWord = true;
                            break;
                        }
                    }
                    if (containWord)
                        break;
                }
                if (!containWord) continue;
                if (_proc.WordFilter(nonDictTermsAr.TermsAr[i].TermFullNormForm)) continue;
                var newEl = new Term
                {
                    Frequency = nonDictTermsAr.TermsAr[i].Frequency,
                    Kind = KindOfTerm.NonDictTerm,
                    NPattern = nonDictTermsAr.TermsAr[i].NPattern,
                    PatCounter = 0,
                    Pattern = nonDictTermsAr.TermsAr[i].Pattern,
                    SetToDel = false,
                    TermFragment = nonDictTermsAr.TermsAr[i].TermFragment,
                    TermWord = nonDictTermsAr.TermsAr[i].TermWord,
                    PoSstr = nonDictTermsAr.TermsAr[i].PoSstr,
                    TermFullNormForm = nonDictTermsAr.TermsAr[i].TermFullNormForm
                };
                newEl.Pos.Add(null);
                newEl.Rule = rule;
                newEl.RepeatRule = repeatRule;
                var e = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                if (e == null)
                {
                    mainArrayTermsAr.RootTermsTree.AddRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    e = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }
                else
                {
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }


                for (var j = 1; j < nonDictTermsAr.TermsAr[i].Pos.Count; j++)
                {
                    var node = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[j].Range);
                    if (node == null)
                    {
                        mainArrayTermsAr.RootTermsTree.AddRange(nonDictTermsAr.TermsAr[i].Pos[j].Range);
                        node = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[j].Range);
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                    else
                    {
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                }
            }
            //getFrequency_(mainArrayTermsAr);
        }
        private void Rule4(Terms authTermsAr, Terms mainArrayTermsAr, int rule = 4, int repeatRule = 0)
        {
            for (var i = 0; i < authTermsAr.TermsAr.Count; i++)
            {
                var k =
                    mainArrayTermsAr.TermsAr.FindIndex(
                        item =>
                            item.TermFullNormForm == authTermsAr.TermsAr[i].TermFullNormForm &&
                            item.PoSstr == authTermsAr.TermsAr[i].PoSstr);
                if (k != -1) continue;
                if (authTermsAr.TermsAr[i].Pos.Count <= 0) continue;
                bool containWord = false;
                List<string> curTermSplt = authTermsAr.TermsAr[i].TermFullNormForm.Split().ToList();
                if (curTermSplt.Count == 1)
                    curTermSplt = authTermsAr.TermsAr[i].TermFullNormForm.Split('-').ToList();
                foreach (var term in mainArrayTermsAr.TermsAr)
                {
                    List<string> termSplt = term.TermFullNormForm?.Split().ToList();
                    if (termSplt.Count == 1)
                        termSplt = term.TermFullNormForm?.Split('-').ToList();
                    foreach (var word in curTermSplt)
                    {
                        if (termSplt.Contains(word))
                        {
                            containWord = true;
                            break;
                        }
                    }
                    if (containWord)
                        break;
                }
                if (!containWord) continue;
                authTermsAr.TermsAr[i].SetToDel = true;
                var newEl = new Term
                {
                    Frequency = authTermsAr.TermsAr[i].Frequency,
                    Kind = AuthTerm,
                    NPattern = authTermsAr.TermsAr[i].NPattern,
                    PatCounter = 0,
                    Pattern = authTermsAr.TermsAr[i].Pattern,
                    SetToDel = false,
                    TermFragment = authTermsAr.TermsAr[i].TermFragment,
                    TermWord = authTermsAr.TermsAr[i].TermWord,
                    PoSstr = authTermsAr.TermsAr[i].PoSstr,
                    TermFullNormForm = authTermsAr.TermsAr[i].TermFullNormForm,
                    CValue = authTermsAr.TermsAr[i].CValue,
                    mainPage = authTermsAr.TermsAr[i].mainPage
                };
                newEl.Pos.Add(null);
                newEl.Rule = rule;
                newEl.RepeatRule = repeatRule;
                var e = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[0].Range);
                if (e == null)
                {
                    mainArrayTermsAr.RootTermsTree.AddRange(authTermsAr.TermsAr[i].Pos[0].Range);
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    e = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[0].Range);
                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }
                else
                {
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }


                for (var j = 1; j < authTermsAr.TermsAr[i].Pos.Count; j++)
                {
                    var node = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[j].Range);
                    if (node == null)
                    {
                        mainArrayTermsAr.RootTermsTree.AddRange(authTermsAr.TermsAr[i].Pos[j].Range);
                        node = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[j].Range);
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                    else
                    {
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                }
            }
        }
        private void Rule5(Terms authTermsAr, Terms mainArrayTermsAr, int rule = 5, int repeatRule = 0)
        {
            for (var i = 0; i < authTermsAr.TermsAr.Count; i++)
            {
                var k =
                    mainArrayTermsAr.TermsAr.FindIndex(
                        item =>
                            item.TermFullNormForm == authTermsAr.TermsAr[i].TermFullNormForm &&
                            item.PoSstr == authTermsAr.TermsAr[i].PoSstr);
                if (k != -1) continue;
                if (authTermsAr.TermsAr[i].Pos.Count <= 0) continue;
                bool containWord = false;
                List<string> curTermSplt = authTermsAr.TermsAr[i].TermFullNormForm.Split().ToList();
                if (curTermSplt.Count == 1)
                    curTermSplt = authTermsAr.TermsAr[i].TermFullNormForm.Split('-').ToList();
                foreach (var term in mainArrayTermsAr.TermsAr)
                {
                    if (authTermsAr.TermsAr[i].synonimTo == term.TermWord)
                    {
                        containWord = true;
                        break;
                    }
                }
                if (!containWord) continue;
                authTermsAr.TermsAr[i].SetToDel = true;
                var newEl = new Term
                {
                    Frequency = authTermsAr.TermsAr[i].Frequency,
                    Kind = AuthTerm,
                    NPattern = authTermsAr.TermsAr[i].NPattern,
                    PatCounter = 0,
                    Pattern = authTermsAr.TermsAr[i].Pattern,
                    SetToDel = false,
                    TermFragment = authTermsAr.TermsAr[i].TermFragment,
                    TermWord = authTermsAr.TermsAr[i].TermWord,
                    PoSstr = authTermsAr.TermsAr[i].PoSstr,
                    TermFullNormForm = authTermsAr.TermsAr[i].TermFullNormForm,
                    CValue = authTermsAr.TermsAr[i].CValue,
                    mainPage = authTermsAr.TermsAr[i].mainPage
                };
                newEl.Pos.Add(null);
                newEl.Rule = rule;
                newEl.RepeatRule = repeatRule;
                var e = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[0].Range);
                if (e == null)
                {
                    mainArrayTermsAr.RootTermsTree.AddRange(authTermsAr.TermsAr[i].Pos[0].Range);
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    e = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[0].Range);
                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }
                else
                {
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }


                for (var j = 1; j < authTermsAr.TermsAr[i].Pos.Count; j++)
                {
                    var node = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[j].Range);
                    if (node == null)
                    {
                        mainArrayTermsAr.RootTermsTree.AddRange(authTermsAr.TermsAr[i].Pos[j].Range);
                        node = mainArrayTermsAr.RootTermsTree.FindRange(authTermsAr.TermsAr[i].Pos[j].Range);
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                    else
                    {
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                }
            }
        }
        private void Rule6(Terms mainTermsAr, SynTerms synTermsAr, int rule = 6, int repeatRule = 0)
        {
            //FindFunctions find = new FindFunctions();
            foreach (var curSynTermPair in synTermsAr.TermsAr)
            {
                var firstAlt =
                    mainTermsAr.TermsAr.FindIndex(
                        item => item.TermFullNormForm == curSynTermPair.Alternatives.First.TermFullNormForm &&
                                item.PoSstr == curSynTermPair.Alternatives.First.PoSstr);
                var secondAlt =
                    mainTermsAr.TermsAr.FindIndex(
                        item => item.TermFullNormForm == curSynTermPair.Alternatives.Second.TermFullNormForm &&
                                item.PoSstr == curSynTermPair.Alternatives.Second.PoSstr);
                if (firstAlt != -1 && secondAlt == -1)
                {
                    curSynTermPair.SetToDel = true;
                    var newEl = new Term
                    {
                        Frequency = curSynTermPair.Alternatives.Second.Frequency,
                        Kind = KindOfTerm.SynTerm,
                        NPattern = curSynTermPair.Alternatives.Second.NPattern,
                        PatCounter = 0,
                        Pattern = curSynTermPair.Alternatives.Second.Pattern,
                        SetToDel = false,
                        PoSstr = curSynTermPair.Alternatives.Second.PoSstr,
                        TermFragment = curSynTermPair.TermFragment,
                        TermWord = curSynTermPair.Alternatives.Second.Alternative,
                        TermFullNormForm = curSynTermPair.Alternatives.Second.TermFullNormForm,
                        Rule = rule,
                        RepeatRule = repeatRule
                    };
                    //MainTermsAr.TermsAr[first_alt].frequency;
                    //newEl.Pos.Add(null);
                    mainTermsAr.TermsAr.Add(newEl);
                    for (var j = 1; j < mainTermsAr.TermsAr[firstAlt].Pos.Count; j++)
                    {
                        mainTermsAr.TermsAr[mainTermsAr.TermsAr.Count - 1].Pos.Add(mainTermsAr.TermsAr[firstAlt].Pos[j]);
                        if (!mainTermsAr.TermsAr[firstAlt].Pos[j].IndexElement.Contains(mainTermsAr.TermsAr.Count - 1))
                            mainTermsAr.TermsAr[firstAlt].Pos[j].IndexElement.Add(mainTermsAr.TermsAr.Count - 1);
                    }
                }
                else if (firstAlt == -1 && secondAlt != -1)
                {
                    curSynTermPair.SetToDel = true;
                    var newEl = new Term
                    {
                        Frequency = curSynTermPair.Alternatives.First.Frequency,
                        Kind = KindOfTerm.SynTerm,
                        NPattern = curSynTermPair.Alternatives.First.NPattern,
                        PatCounter = 0,
                        Pattern = curSynTermPair.Alternatives.First.Pattern,
                        SetToDel = false,
                        TermFragment = curSynTermPair.TermFragment,
                        TermWord = curSynTermPair.Alternatives.First.Alternative,
                        PoSstr = curSynTermPair.Alternatives.First.PoSstr,
                        TermFullNormForm = curSynTermPair.Alternatives.First.TermFullNormForm,
                        Rule = rule,
                        RepeatRule = repeatRule
                    };
                    //MainTermsAr.TermsAr[second_alt].frequency;
                    //newEl.Pos.Add(null);
                    mainTermsAr.TermsAr.Add(newEl);
                    for (var j = 1; j < mainTermsAr.TermsAr[secondAlt].Pos.Count; j++)
                    {
                        mainTermsAr.TermsAr[mainTermsAr.TermsAr.Count - 1].Pos.Add(mainTermsAr.TermsAr[secondAlt].Pos[j]);
                        if (!mainTermsAr.TermsAr[secondAlt].Pos[j].IndexElement.Contains(mainTermsAr.TermsAr.Count - 1))
                            mainTermsAr.TermsAr[secondAlt].Pos[j].IndexElement.Add(mainTermsAr.TermsAr.Count - 1);
                    }
                }
            }
            DelElementsWhichSetToDel(synTermsAr);
            //getFrequency_(mainTermsAr);
        }
        private void Rule7(SynTerms synTermsAr, Terms mainTermsAr, int rule = 7, int repeatRule = 0)
        {
            List<double> frequencyList = new List<double>();
            for (var i = 0; i < synTermsAr.TermsAr.Count; i++)
            {
                frequencyList.Add(synTermsAr.TermsAr[i].Alternatives.First.Frequency + synTermsAr.TermsAr[i].Alternatives.Second.Frequency);
            }
            double percentileBottom = Math.Floor(Percentile(frequencyList, 0.35));
            double percentileTop = Math.Ceiling(Percentile(frequencyList, 0.95));
            foreach (var curSynTermPair in synTermsAr.TermsAr)
            {
                int sumFreq = curSynTermPair.Alternatives.First.Frequency + curSynTermPair.Alternatives.Second.Frequency;
                if (sumFreq < percentileBottom || sumFreq > percentileTop)
                {
                    continue;
                }
                 if (_proc.WordFilter(curSynTermPair.Alternatives.First.TermFullNormForm) || _proc.WordFilter(curSynTermPair.Alternatives.Second.TermFullNormForm))
                    continue;
                var firstAlt =
                    mainTermsAr.TermsAr.FindIndex(
                        item => item.TermFullNormForm == curSynTermPair.Alternatives.First.TermFullNormForm &&
                                item.PoSstr == curSynTermPair.Alternatives.First.PoSstr);
                var secondAlt =
                    mainTermsAr.TermsAr.FindIndex(
                        item => item.TermFullNormForm == curSynTermPair.Alternatives.Second.TermFullNormForm &&
                                item.PoSstr == curSynTermPair.Alternatives.Second.PoSstr);
                if (firstAlt != -1 && secondAlt == -1)
                {
                    curSynTermPair.SetToDel = true;
                    var newEl = new Term
                    {
                        Frequency = curSynTermPair.Alternatives.Second.Frequency,
                        Kind = KindOfTerm.SynTerm,
                        NPattern = curSynTermPair.Alternatives.Second.NPattern,
                        PatCounter = 0,
                        Pattern = curSynTermPair.Alternatives.Second.Pattern,
                        SetToDel = false,
                        PoSstr = curSynTermPair.Alternatives.Second.PoSstr,
                        TermFragment = curSynTermPair.TermFragment,
                        TermWord = curSynTermPair.Alternatives.Second.Alternative,
                        TermFullNormForm = curSynTermPair.Alternatives.Second.TermFullNormForm,
                        Rule = rule,
                        RepeatRule = repeatRule
                    };
                    //MainTermsAr.TermsAr[first_alt].frequency;
                    //newEl.Pos.Add(null);
                    mainTermsAr.TermsAr.Add(newEl);
                    for (var j = 1; j < mainTermsAr.TermsAr[firstAlt].Pos.Count; j++)
                    {
                        mainTermsAr.TermsAr[mainTermsAr.TermsAr.Count - 1].Pos.Add(mainTermsAr.TermsAr[firstAlt].Pos[j]);
                        if (!mainTermsAr.TermsAr[firstAlt].Pos[j].IndexElement.Contains(mainTermsAr.TermsAr.Count - 1))
                            mainTermsAr.TermsAr[firstAlt].Pos[j].IndexElement.Add(mainTermsAr.TermsAr.Count - 1);
                    }
                }
                else if (firstAlt == -1 && secondAlt != -1)
                {
                    curSynTermPair.SetToDel = true;
                    var newEl = new Term
                    {
                        Frequency = curSynTermPair.Alternatives.First.Frequency,
                        Kind = KindOfTerm.SynTerm,
                        NPattern = curSynTermPair.Alternatives.First.NPattern,
                        PatCounter = 0,
                        Pattern = curSynTermPair.Alternatives.First.Pattern,
                        SetToDel = false,
                        TermFragment = curSynTermPair.TermFragment,
                        TermWord = curSynTermPair.Alternatives.First.Alternative,
                        TermFullNormForm = curSynTermPair.Alternatives.First.TermFullNormForm,
                        PoSstr = curSynTermPair.Alternatives.First.PoSstr,
                        Rule = rule,
                        RepeatRule = repeatRule
                    };
                    //MainTermsAr.TermsAr[second_alt].frequency;
                    //newEl.Pos.Add(null);
                    mainTermsAr.TermsAr.Add(newEl);
                    for (var j = 1; j < mainTermsAr.TermsAr[secondAlt].Pos.Count; j++)
                    {
                        mainTermsAr.TermsAr[mainTermsAr.TermsAr.Count - 1].Pos.Add(mainTermsAr.TermsAr[secondAlt].Pos[j]);
                        if (!mainTermsAr.TermsAr[secondAlt].Pos[j].IndexElement.Contains(mainTermsAr.TermsAr.Count - 1))
                            mainTermsAr.TermsAr[secondAlt].Pos[j].IndexElement.Add(mainTermsAr.TermsAr.Count - 1);
                    }
                }
                else
                {
                    curSynTermPair.SetToDel = true;
                    var newEl1 = new Term
                    {
                        Frequency = curSynTermPair.Alternatives.Second.Frequency,
                        Kind = KindOfTerm.SynTerm,
                        NPattern = curSynTermPair.Alternatives.Second.NPattern,
                        PatCounter = 0,
                        Pattern = curSynTermPair.Alternatives.Second.Pattern,
                        SetToDel = false,
                        PoSstr = curSynTermPair.Alternatives.Second.PoSstr,
                        TermFragment = curSynTermPair.TermFragment,
                        TermWord = curSynTermPair.Alternatives.Second.Alternative,
                        TermFullNormForm = curSynTermPair.Alternatives.Second.TermFullNormForm,
                        Rule = rule,
                        RepeatRule = repeatRule
                    };
                    //MainTermsAr.TermsAr[first_alt].frequency;
                    //newEl.Pos.Add(null);
                    mainTermsAr.TermsAr.Add(newEl1);
                    //for (var j = 1; j < mainTermsAr.TermsAr[firstAlt].Pos.Count; j++)
                    //{
                    //    mainTermsAr.TermsAr[mainTermsAr.TermsAr.Count - 1].Pos.Add(mainTermsAr.TermsAr[firstAlt].Pos[j]);
                    //    if (!mainTermsAr.TermsAr[firstAlt].Pos[j].IndexElement.Contains(mainTermsAr.TermsAr.Count - 1))
                    //        mainTermsAr.TermsAr[firstAlt].Pos[j].IndexElement.Add(mainTermsAr.TermsAr.Count - 1);
                    //}
                    curSynTermPair.SetToDel = true;
                    var newEl2 = new Term
                    {
                        Frequency = curSynTermPair.Alternatives.First.Frequency,
                        Kind = KindOfTerm.SynTerm,
                        NPattern = curSynTermPair.Alternatives.First.NPattern,
                        PatCounter = 0,
                        Pattern = curSynTermPair.Alternatives.First.Pattern,
                        SetToDel = false,
                        TermFragment = curSynTermPair.TermFragment,
                        TermWord = curSynTermPair.Alternatives.First.Alternative,
                        TermFullNormForm = curSynTermPair.Alternatives.First.TermFullNormForm,
                        PoSstr = curSynTermPair.Alternatives.First.PoSstr,
                        Rule = rule,
                        RepeatRule = repeatRule
                    };
                    //MainTermsAr.TermsAr[second_alt].frequency;
                    //newEl.Pos.Add(null);
                    mainTermsAr.TermsAr.Add(newEl2);
                    //for (var j = 1; j < mainTermsAr.TermsAr[secondAlt].Pos.Count; j++)
                    //{
                    //    mainTermsAr.TermsAr[mainTermsAr.TermsAr.Count - 1].Pos.Add(mainTermsAr.TermsAr[secondAlt].Pos[j]);
                    //    if (!mainTermsAr.TermsAr[secondAlt].Pos[j].IndexElement.Contains(mainTermsAr.TermsAr.Count - 1))
                    //        mainTermsAr.TermsAr[secondAlt].Pos[j].IndexElement.Add(mainTermsAr.TermsAr.Count - 1);
                    //}
                }
            }
        }
        private void Rule8(NonDictTerms nonDictTermsAr, Terms mainArrayTermsAr, int rule = 8, int repeatRule = 0)
        {
            for (var i = 0; i < nonDictTermsAr.TermsAr.Count; i++)
            {
                var k =
                    mainArrayTermsAr.TermsAr.FindIndex(
                        item =>
                            item.TermFullNormForm == nonDictTermsAr.TermsAr[i].TermFullNormForm &&
                            item.PoSstr == nonDictTermsAr.TermsAr[i].PoSstr);
                if (k != -1) continue;
                if (nonDictTermsAr.TermsAr[i].Pos.Count <= 0) continue;
                if (nonDictTermsAr.TermsAr[i].Frequency < percentileBottom || nonDictTermsAr.TermsAr[i].Frequency > percentileTop)
                    continue;
                bool containWord = false;
                List<string> curTermSplt = nonDictTermsAr.TermsAr[i].TermFullNormForm.Split().ToList();
                foreach (var term in mainArrayTermsAr.TermsAr)
                {
                    if (term.type == TypeOfTerm.Trusted) continue;
                    List<string> termSplt = term.TermFullNormForm?.Split().ToList();
                    foreach (var word in curTermSplt)
                    {
                        if (termSplt.Contains(word))
                        {
                            containWord = true;
                            break;
                        }
                    }
                    if (containWord)
                        break;
                }
                if (!containWord) continue;
                var newEl = new Term
                {
                    Frequency = nonDictTermsAr.TermsAr[i].Frequency,
                    Kind = KindOfTerm.NonDictTerm,
                    NPattern = nonDictTermsAr.TermsAr[i].NPattern,
                    PatCounter = 0,
                    Pattern = nonDictTermsAr.TermsAr[i].Pattern,
                    SetToDel = false,
                    TermFragment = nonDictTermsAr.TermsAr[i].TermFragment,
                    TermWord = nonDictTermsAr.TermsAr[i].TermWord,
                    PoSstr = nonDictTermsAr.TermsAr[i].PoSstr,
                    TermFullNormForm = nonDictTermsAr.TermsAr[i].TermFullNormForm
                };
                newEl.Pos.Add(null);
                newEl.Rule = rule;
                newEl.RepeatRule = repeatRule;
                var e = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                if (e == null)
                {
                    mainArrayTermsAr.RootTermsTree.AddRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    e = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[0].Range);
                    e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }
                else
                {
                    mainArrayTermsAr.TermsAr.Add(newEl);
                    if (!e.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                        e.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                    mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos[
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                }


                for (var j = 1; j < nonDictTermsAr.TermsAr[i].Pos.Count; j++)
                {
                    var node = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[j].Range);
                    if (node == null)
                    {
                        mainArrayTermsAr.RootTermsTree.AddRange(nonDictTermsAr.TermsAr[i].Pos[j].Range);
                        node = mainArrayTermsAr.RootTermsTree.FindRange(nonDictTermsAr.TermsAr[i].Pos[j].Range);
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                    else
                    {
                        if (!node.IndexElement.Contains(mainArrayTermsAr.TermsAr.Count - 1))
                            node.IndexElement.Add(mainArrayTermsAr.TermsAr.Count - 1);
                        mainArrayTermsAr.TermsAr[mainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                    }
                }
            }
            //getFrequency_(mainArrayTermsAr);
        }
        public static double Percentile(List<double> sequence, double excelPercentile)
        {
            sequence.Sort();
            int N = sequence.Count;
            double n = (N - 1) * excelPercentile + 1;
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1d) return sequence[0];
            else if (n == N) return sequence[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }

        private void ClearDublicates(Terms mainTermsAr)
        {
            for (int i = 8; i > 1; i--)
            {
                for(int j =0; j < mainTermsAr.TermsAr.Count; j++)
                {
                    if (mainTermsAr.TermsAr[j].Rule == i)
                    {
                        int k = mainTermsAr.TermsAr.FindIndex(
                            item => item.TermFullNormForm == mainTermsAr.TermsAr[j].TermFullNormForm && item.Rule <= mainTermsAr.TermsAr[j].Rule);
                        if (k != -1 && j != k)
                        {
                            mainTermsAr.TermsAr[j].SetToDel = true;
                        }
                    }
                }
            }
            DelElementsWhichSetToDel(mainTermsAr);
        }

        private void DeleteWhichHaveExtended(Terms mainTermsAr)
        {
            for (int i = 0; i < mainTermsAr.TermsAr.Count; i++)
            {
                var curTermSplt = mainTermsAr.TermsAr[i].TermFullNormForm.Split();
                bool match = false;
                for (int j = 0; j < mainTermsAr.TermsAr.Count; j++)
                {
                    if (i != j)
                    {
                        var potentialExtendSplt = mainTermsAr.TermsAr[j].TermFullNormForm.Split();
                        int count = 0;
                        foreach (var word in curTermSplt)
                        {
                            if (potentialExtendSplt.Contains(word))
                                count++;
                        }
                        if (count == curTermSplt.Length)
                        {
                            match = true;
                            break;
                        }

                    }
                }
                if (match)
                    mainTermsAr.TermsAr[i].SetToDel = true;
            }
            DelElementsWhichSetToDel(mainTermsAr);
        }
    }
}
