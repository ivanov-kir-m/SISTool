#pragma warning disable CS0436 // Тип конфликтует с импортированным типом
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using LemmatizerNET;
using Microsoft.Office.Interop.Word;
using static TermRules.AuxiliaryFunctions;
using static TermRules.FindFunctions;
using static TermRules.KindOfTerm;
using Application = System.Windows.Forms.Application;
using Point = System.Drawing.Point;
using DawgSharp;
using Novacode;

namespace TermRules
{

    public enum DictionaryF
    {
        ItTerm,
        FTerm
    }
    public class PatternGenerationAssistItem
    {
        public int TermsArIndex { get; set; }
        public int StartPos { get; set; }
        public int Frequency { get; set; }
    }
    public class TermsProcessing
    {
        //private const int MAX_COMMAND = 8000;
        public Terms MainTermsAr { get; set; }
        public Terms AuthTermsAr { get; set; }
        public Terms DictTermsAr { get; set; }
        public NonDictTerms NonDictTermsAr { get; set; }
        public CombTerms CombTermsAr { get; set; }
        public List<Pair<string, string>> PatternsModel { get; set; }
        public List<Pair<string, string>> DictPatterns { get; set; }
        public string TmpPath { get; set; }
        public string ProgrammPath { get; set; }
        public string FolderPath { get; set; }
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public List<string> Pages { get; set; }
        public MorphLanguage Lang { get; set; }
        public ILemmatizer Lem { get; set; }
        public FileManager Manager { get; set; }
        private DictionaryF Dictionary { get; set; }
        public DawgBuilder<bool> StopWordsCombinedBuilder { get; set; }
        public DawgBuilder<bool> StopWordsUnaryBuilder { get; set; }
        public Dawg<bool> StopWordsCombined { get; set; }
        public Dawg<bool> StopWordsUnary { get; set; }
        public List<string> Headers { get; set; }

        public TermsProcessing(string inputfile, DictionaryF dict, int startPage, int endPage, bool clearSupportLists)
        {
            FolderPath = "TermsProcessingF";
            OutputFile = "";
            InputFile = inputfile;
            var extension = Path.GetExtension(inputfile).ToLower();
            //string pattern = ".\u002E[dD][oO][cC][xX]$";
            //if (Regex.IsMatch(inputFile, pattern))
            if (extension == ".docx")
            {
                Lang = MorphLanguage.Russian;         // Русский словарь
                Lem = LemmatizerFactory.Create(Lang);   // объект лемматизера
                Manager = FileManager.GetFileManager(Environment.GetEnvironmentVariable("RMLLematizer", EnvironmentVariableTarget.Machine));   // путь к словарям
                Lem.LoadDictionariesRegistry(Manager);                      // инициализация словарей
                StartPage = startPage;
                EndPage = endPage;
                Pages = ReadTextFromDocxByPage(inputfile);
                Headers = new List<string>();
                var doc = DocX.Load(inputfile);
                if (doc?.Bookmarks != null)
                    foreach (var docBookmark in doc?.Bookmarks)
                        Headers.Add(docBookmark.Paragraph.Text.ToUpper().Replace("\t","").Replace("\n",""));
                doc.Save();
                Dictionary = dict;
                TmpPath = Path.GetTempPath();
                ProgrammPath = Application.StartupPath;
                MainTermsAr = new Terms();
                AuthTermsAr = new Terms();
                DictTermsAr = new Terms();
                NonDictTermsAr = new NonDictTerms();
                CombTermsAr = new CombTerms();
                Directory.CreateDirectory(TmpPath + FolderPath);
                InputFile = TmpPath + FolderPath + "\\InputText.txt";
                var outfile = new StreamWriter(File.Open(InputFile, FileMode.Create), Encoding.GetEncoding("Windows-1251"));
                foreach (var page in Pages)
                {
                    outfile.Write(CleanString(page));
                }
                outfile.Close();
                InputFile = TmpPath + FolderPath + "\\InputTextPages.txt";
                outfile = new StreamWriter(File.Open(InputFile, FileMode.Create), Encoding.GetEncoding("Windows-1251"));
                foreach (var page in Pages)
                {
                    outfile.Write(CleanString(page)+"\n<<<PAGE>>>\n");
                }
                outfile.Close();
                PatternsModel = new List<Pair<string, string>>();
                DictPatterns = new List<Pair<string, string>>();
                GetStopwords();
            }
            else
            {
                MessageBox.Show("ERROR: Неверный формат файла, поддерживается только *.docx");
                Application.Exit();
            }
        }
        public static string CleanString(string s)
        {
            if (s != null && s.Length > 0)
            {
                var sb = new StringBuilder(s.Length);
                foreach (var c in s)
                {
                    if (c != '\x0A' && c != '\n' && c != '\r' && char.IsControl(c))
                    {
                        if (c == '\x1E')
                            sb.Append('-');
                        else sb.Append(' ');
                        continue;
                    }
                    if (c == 'ё')
                        sb.Append('е');
                    else
                        sb.Append(c);
                }
                s = sb.ToString().Replace("/", " / ").Replace("|", " | ").Replace("\\", " \\ ").Replace("\r", "\n");
            }
            return s;
        }

        private static bool WordPatternEng(string word)
        {
            List<char> delims = new List<char>(new[] { '-', '\'', '`' });
            Regex engLetter = new Regex("([a-zA-Z])");
            bool q = false;
            bool start = true;
            int i = 1;
            int end = word.Length;
            foreach (var letter in word)
            {
                if (start)
                    if (!engLetter.IsMatch(letter.ToString()))
                        return false;
                    else
                    {
                        start = false;
                        continue;
                    }
                if (!(engLetter.IsMatch(letter.ToString()) || delims.Contains(letter)))
                    return false;
                if ((letter == '\'' || letter == '`') && q)
                    return false;
                if (letter == '\'' || letter == '`')
                    q = true;
                if (i == end && delims.Contains(letter))
                    return false;
                i++;
            }
            return true;
        }

        private void GetStopwords()
        {
            StopWordsCombinedBuilder = new DawgBuilder<bool>();
            var programmPath = Application.StartupPath;
            var lsplPatterns = programmPath + "\\stopwords_combined.txt";
            var targetPatternsWriter = new StreamReader(lsplPatterns, Encoding.GetEncoding("Windows-1251"));
            string line;
            while ((line = targetPatternsWriter.ReadLine()) != null)
            {
                StopWordsCombinedBuilder.Insert(line.Replace("\n", ""),true);
            }
            StopWordsCombined = StopWordsCombinedBuilder.BuildDawg();
            StopWordsUnaryBuilder = new DawgBuilder<bool>();
            lsplPatterns = programmPath + "\\stopwords_unary.txt";
            targetPatternsWriter = new StreamReader(lsplPatterns, Encoding.GetEncoding("Windows-1251"));
            line = "";
            while ((line = targetPatternsWriter.ReadLine()) != null)
            {
                StopWordsUnaryBuilder.Insert(line.Replace("\n", ""),true);
            }
            StopWordsUnary = StopWordsUnaryBuilder.BuildDawg();
        }

        public bool WordFilter(string word)
        {
            List<string> wordSplt = new List<string>(word.Trim().Split(' '));
            bool matchEng = false;
            foreach (var wordPart in wordSplt)
            {
                if (WordPatternEng(wordPart) || StopWordsCombined[wordPart.Trim()])
                {
                    matchEng = true;
                    break;
                }
            }
            if (matchEng) return matchEng;
            int count = 0;
            foreach (var wordPart in wordSplt)
            {
                if (StopWordsUnary[wordPart.Trim()])
                {
                    count++;
                }
            }
            if (count == wordSplt.Count) return true;
            return false;
        }

        //AuthTerms
        public void GetNewPatternsXml(string inputFile)
        {
            var tmpPath = Path.GetTempPath();
            var programmPath = Application.StartupPath;
            var folderPath = "TermsProcessingF";
            var batOutput = tmpPath + folderPath + "\\IndexItemsPatterns.bat";
            var targetPatternsOutput = tmpPath + folderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false, Encoding.GetEncoding("Windows-1251"));
            targetPatternsWriter.WriteLine("PART");
            targetPatternsWriter.WriteLine("PARTWORD");
            targetPatternsWriter.Close();
            var lsplExe = programmPath + "\\bin\\lspl-find.exe";
            var lsplPatterns = programmPath + "\\Patterns\\INDEX_PARTS.txt";
            var lsplOutput = tmpPath + folderPath + "\\AuthNewPatternsOutput.xml";
            var lsplOutputText = tmpPath + folderPath + "\\AuthNewPatternsOutputPatternsText.xml";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            sw.WriteLine("\"" + lsplExe
                + "\" -i \"" + inputFile
                + "\" -p \"" + lsplPatterns
                + "\" -t \"" + lsplOutputText
                + "\" -s \"" + targetPatternsOutput + "\" ");
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(startInfo).WaitForExit();
        }
        public bool GetNewPatterns(List<PatternGenerationAssistItem> index, List<Term> termsAr)
        {
            var tmpPath = Path.GetTempPath();
            var programmPath = Application.StartupPath;
            var folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var curFragment = "";
            var lastNodeName = "";
            var lastPos = "";
            using (var xml = XmlReader.Create(tmpPath + folderPath + "\\AuthNewPatternsOutputPatternsText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                curPat = xml.Value.Trim();
                                                curPat = curPat.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                curPos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPos = xml.Value.Trim();
                                        }
                                    }

                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    curFragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    if (curPat == "PART")
                                    {
                                        var word = xml.Value.Trim();
                                        var k = index.FindIndex(item => (curPos.X == item.StartPos &&
                                        curPos.Y == item.StartPos + termsAr[item.TermsArIndex].TermWord.Length));
                                        if (k != -1)
                                        {
                                            termsAr[index[k].TermsArIndex].NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            return true;
        }
        public void GetXmlAuthTerms(Terms authTermsAr)
        {
            var batOutput = TmpPath + FolderPath + "\\AuthTerms.bat";
            var targetPatternsOutput = TmpPath + FolderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false, Encoding.GetEncoding("Windows-1251"));
            var curPattern = "";
            var fs = new StreamReader(ProgrammPath + "\\Patterns\\AUTH_TERM_NEW.txt", Encoding.GetEncoding("Windows-1251"));
            curPattern = fs.ReadLine();
            if (curPattern != null)
            {
                curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                curPattern = curPattern.Trim();
                if (curPattern.IndexOf("Def", StringComparison.Ordinal) != -1 || curPattern.IndexOf("Sdef", StringComparison.Ordinal) != -1)
                {
                    targetPatternsWriter.WriteLine(curPattern);
                }
                while (true)
                {
                    curPattern = fs.ReadLine();
                    if (curPattern == null) break;
                    curPattern = curPattern.Trim();
                    if (curPattern != "" && curPattern != " ")
                    {
                        curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                        curPattern = curPattern.Trim();

                        if ((curPattern.IndexOf("Def", StringComparison.Ordinal) != -1 || curPattern.IndexOf("Sdef", StringComparison.Ordinal) != -1) && curPattern.Length > 3)
                        {
                            targetPatternsWriter.WriteLine(curPattern);
                        }
                    }
                }
                targetPatternsWriter.Close();
                var lsplExe = ProgrammPath + "\\bin\\lspl-find.exe";
                var lsplPatterns = ProgrammPath + "\\Patterns\\AUTH_TERM_NEW.txt";
                var lsplOutput = TmpPath + FolderPath + "\\AuthTermsOutput.xml";
                var lsplOutputText = TmpPath + FolderPath + "\\AuthTermsOutputText.xml";
                //string LSPL_output_patterns = tmpPath + folderPath + "\\AuthTermsOutputPatterns.xml";
                var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
                //bin\lspl-find -i example\text.txt -p example\patterns.txt -o example\output.xml -t example\output_text.xml -r example\output_patterns.xml -s example\targets.txt
                sw.WriteLine("\"" + lsplExe
                    + "\" -i \"" + InputFile
                    + "\" -p \"" + lsplPatterns
                    //+ "\" -o \"" + LSPL_output
                    + "\" -t \"" + lsplOutputText
                    //+ "\" -r \"" + LSPL_output_patterns
                    + "\" -s \"" + targetPatternsOutput + "\" ");
                sw.Close();
                var startInfo = new ProcessStartInfo(batOutput);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(startInfo).WaitForExit();
                GetAuthTerms(authTermsAr);

            }
            else
            {
                MessageBox.Show("Ошибка! Некорректный файл с шаблонами авторских терминов!");
                Application.Exit();
            }
        }
        public bool GetAuthTerms(Terms authTermsAr)
        {
            var curPos = new Point();
            var curPat = "";
            var curFragment = "";
            var lastNodeName = "";
            var curLastPos = "";
            using (var xml = XmlReader.Create(TmpPath + FolderPath + "\\AuthTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                curPat = xml.Value.Trim();
                                                curPat = curPat.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                curPos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            curLastPos = xml.Value.Trim();
                                        }
                                    }

                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    curFragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    if (curPat.IndexOf("Sdef", StringComparison.Ordinal) == -1)
                                    {
                                        string words = xml.Value.Trim();
                                        var wordsSplt = words.Split(new string[] { "@@" }, StringSplitOptions.None).ToList();
                                        var lastPosSplt = curLastPos.Split('L').ToList();
                                        string firstSynonim = "";
                                        bool firstSyn = true;
                                        for (int t = 0; t < wordsSplt.Count; t++)
                                        {
                                            var word = wordsSplt[t].Trim();
                                            var lastPos = lastPosSplt[t].Trim();
                                            var check = word.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                            if (check != -1)
                                                continue;
                                            var let = word.ToList().FindIndex(sym => char.IsDigit(sym));
                                            if (let != -1)
                                                continue;
                                            if (word.Length > 1)
                                            {
                                                string fullNormWord = "";
                                                List<string> wordSplt = new List<string>(word.Trim().Split(' '));
                                                foreach (var wordPart in wordSplt)
                                                {
                                                    fullNormWord += " " +
                                                                    Lem.CreateParadigmCollectionFromForm(wordPart, false,
                                                                        true)[0].Norm;
                                                }
                                                fullNormWord = fullNormWord.Trim();
                                                bool matchEng = WordFilter(fullNormWord) || WordFilter(word);
                                                if (matchEng)
                                                    continue;
                                                if (firstSyn)
                                                {
                                                    firstSynonim = word;
                                                    firstSyn = false;
                                                }
                                                //int k = FindFunctions.findINList(AuthTermsAr.TermsAr, word, lastPOS);
                                                var k =
                                                    authTermsAr.TermsAr.FindIndex(
                                                        item =>
                                                            item.TermFullNormForm == fullNormWord &&
                                                            item.PoSstr == lastPos);
                                                if (k == -1)
                                                {
                                                    curPos = GetRealPos(curFragment, word, curPos);
                                                    var curRange = new Range(curPos);
                                                    var e = authTermsAr.RootTermsTree.FindRange(curRange);
                                                    var eExtension =
                                                        authTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                    var newEl = new Term();
                                                    newEl.mainPage = GetPageNumberByPositon(curRange);
                                                    newEl.TermWord = word.Trim();
                                                    newEl.Frequency = 1;
                                                    newEl.PoSstr = lastPos;
                                                    newEl.SetToDel = false;
                                                    newEl.Pos.Add(null);
                                                    newEl.TermFragment = curFragment;
                                                    newEl.Pattern = curPat;
                                                    newEl.TermFullNormForm = fullNormWord.Trim();
                                                    newEl.Kind = AuthTerm;
                                                    newEl.synonimTo = firstSynonim;
                                                    if (curPat.IndexOf("TrustedDef", StringComparison.Ordinal) == 0)
                                                        newEl.type = TypeOfTerm.Trusted;
                                                    else if (curPat.IndexOf("UntrustedDef", StringComparison.Ordinal) == 0)
                                                        newEl.type = TypeOfTerm.Untrusted;
                                                    if (e == null && eExtension == null)
                                                    {
                                                        e = authTermsAr.RootTermsTree.AddRange(curRange);
                                                        authTermsAr.TermsAr.Add(newEl);
                                                        //e = authTermsAr.RootTermsTree.FindRange(curRange);
                                                        e.IndexElement.Add(authTermsAr.TermsAr.Count - 1);
                                                        authTermsAr.TermsAr[authTermsAr.TermsAr.Count - 1].Pos[
                                                            authTermsAr.TermsAr[authTermsAr.TermsAr.Count - 1].Pos.Count -
                                                            1] = e;
                                                    }
                                                    else
                                                    {
                                                        if (e != null && eExtension == null)
                                                        {
                                                            authTermsAr.TermsAr.Add(newEl);
                                                            e.IndexElement.Add(authTermsAr.TermsAr.Count - 1);
                                                            authTermsAr.TermsAr[authTermsAr.TermsAr.Count - 1].Pos[
                                                                authTermsAr.TermsAr[authTermsAr.TermsAr.Count - 1].Pos
                                                                    .Count - 1] = e;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    curPos = GetRealPos(curFragment, word, curPos);
                                                    var curRange = new Range(curPos);
                                                    var e = authTermsAr.RootTermsTree.FindRange(curRange);
                                                    var eExtension =
                                                        authTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                    if (e == null && eExtension == null)
                                                    {
                                                        e = authTermsAr.RootTermsTree.AddRange(curRange);
                                                        //e = authTermsAr.RootTermsTree.FindRange(curRange);
                                                        e.IndexElement.Add(k);
                                                        authTermsAr.TermsAr[k].Pos.Add(e);
                                                        authTermsAr.TermsAr[k].Frequency++;
                                                    }
                                                    else
                                                    {
                                                        if (e != null && !e.IndexElement.Contains(k))
                                                            e.IndexElement.Add(k);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (curPat.IndexOf("Sdef", StringComparison.Ordinal) != -1)
                                    {
                                        string words = xml.Value.Trim();
                                        var wordsSplt = words.Split(new string[] { "@@" }, StringSplitOptions.None).ToList();
                                        var lastPosSplt = curLastPos.Split('L').ToList();
                                        string firstSynonim = "";
                                        bool firstSyn = true;
                                        for (int t = 0; t < wordsSplt.Count - 1; t++)
                                        {
                                            var word = wordsSplt[t].Trim() + " " + wordsSplt[wordsSplt.Count - 1].Trim();
                                            var lastPos = lastPosSplt[t].Trim() + " " + lastPosSplt[wordsSplt.Count - 1].Trim();
                                            var check = word.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                            if (check != -1)
                                                continue;
                                            var let = word.ToList().FindIndex(sym => char.IsDigit(sym));
                                            if (let != -1)
                                                continue;

                                            if (word.Length > 1)
                                            {
                                                string fullNormWord = "";
                                                List<string> wordSplt = new List<string>(word.Trim().Split(' '));
                                                foreach (var wordPart in wordSplt)
                                                {
                                                    fullNormWord += " " +
                                                                    Lem.CreateParadigmCollectionFromForm(wordPart, false,
                                                                        true)[0].Norm;
                                                }
                                                fullNormWord = fullNormWord.Trim();
                                                bool matchEng = WordFilter(fullNormWord) || WordFilter(word);
                                                if (matchEng)
                                                    continue;
                                                if (firstSyn)
                                                {
                                                    firstSynonim = word;
                                                    firstSyn = false;
                                                }
                                                //int k = FindFunctions.findINList(AuthTermsAr.TermsAr, word, lastPOS);
                                                var k =
                                                    authTermsAr.TermsAr.FindIndex(
                                                        item =>
                                                            item.TermFullNormForm == fullNormWord &&
                                                            item.PoSstr == lastPos);
                                                if (k == -1)
                                                {
                                                    var curRange = new Range(curPos);
                                                    var e = authTermsAr.RootTermsTree.FindRange(curRange);
                                                    var eExtension =
                                                        authTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                    var newEl = new Term();
                                                    newEl.mainPage = GetPageNumberByPositon(curRange);
                                                    newEl.TermWord = word.Trim();
                                                    newEl.PoSstr = lastPos;
                                                    newEl.Frequency = 1;
                                                    newEl.SetToDel = false;
                                                    newEl.Pos.Add(null);
                                                    newEl.TermFragment = curFragment;
                                                    newEl.Pattern = curPat;
                                                    newEl.Kind = AuthTerm;
                                                    newEl.TermFullNormForm = fullNormWord.Trim();
                                                    newEl.synonimTo = firstSynonim;
                                                    newEl.type = TypeOfTerm.Trusted;
                                                    if (e == null && eExtension == null)
                                                    {
                                                        e = authTermsAr.RootTermsTree.AddRange(curRange);
                                                        authTermsAr.TermsAr.Add(newEl);
                                                        //e = authTermsAr.RootTermsTree.FindRange(curRange);
                                                        e.IndexElement.Add(authTermsAr.TermsAr.Count - 1);
                                                        authTermsAr.TermsAr[authTermsAr.TermsAr.Count - 1].Pos[
                                                            authTermsAr.TermsAr[authTermsAr.TermsAr.Count - 1].Pos.Count -
                                                            1] = e;
                                                    }
                                                    else
                                                    {
                                                        if (e != null && eExtension == null)
                                                        {
                                                            authTermsAr.TermsAr.Add(newEl);
                                                            e.IndexElement.Add(authTermsAr.TermsAr.Count - 1);
                                                            authTermsAr.TermsAr[authTermsAr.TermsAr.Count - 1].Pos[
                                                                authTermsAr.TermsAr[authTermsAr.TermsAr.Count - 1].Pos
                                                                    .Count - 1] = e;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var curRange = new Range(curPos);
                                                    var e = authTermsAr.RootTermsTree.FindRange(curRange);
                                                    var eExtension =
                                                        authTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                    if (e == null && eExtension == null)
                                                    {
                                                        e = authTermsAr.RootTermsTree.AddRange(curRange);
                                                        //e = authTermsAr.RootTermsTree.FindRange(curRange);
                                                        e.IndexElement.Add(k);
                                                        authTermsAr.TermsAr[k].Pos.Add(e);
                                                        authTermsAr.TermsAr[k].Frequency++;
                                                    }
                                                    else
                                                    {
                                                        if (e != null && !e.IndexElement.Contains(k))
                                                            e.IndexElement.Add(k);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //else if (curPat.IndexOf("NPDef") == 0)
                                    //{
                                    //    var word = xml.Value.Trim();

                                    //    //Range cur_range = new Range(cur_pos);
                                    //    //TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                    //    //if (e != null)
                                    //    //{
                                    //    //    //int k = e.indexElement;
                                    //    //    int k = FindFunctions.findPattern(AuthTermsAr.TermsAr, e.indexElement, cur_pat.Substring("NPDef".Length).Trim());
                                    //    //    AuthTermsAr.TermsAr[k].NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                    //    //}
                                    //    //int f = FindFunctions.findFragmentINList(AuthTermsAr.TermsAr, cur_fragment);
                                    //    var f = authTermsAr.TermsAr.FindIndex(item => item.TermFragment == curFragment);
                                    //    if (f != -1)
                                    //        authTermsAr.TermsAr[f].NPattern = NormalizeNPattern(word);
                                    //}
                                    //else if (curPat.IndexOf("NPSDef") == 0)
                                    //{
                                    //    var word = xml.Value.Trim();
                                    //    var curRange = new Range(curPos);
                                    //    var e = authTermsAr.RootTermsTree.FindRange(curRange);
                                    //    if (e != null)
                                    //    {
                                    //        //int k = e.indexElement;
                                    //        var k = FindPattern(authTermsAr.TermsAr, e.IndexElement, curPat.Substring("NPSDef".Length).Trim());
                                    //        if (k != -1)
                                    //            authTermsAr.TermsAr[k].NPattern = NormalizeNPattern(word);
                                    //    }
                                    //}
                                }
                                break;
                            }
                    }
                }
            }
            for (int i = 0; i < authTermsAr.TermsAr.Count; i++)
            {
                int cont = authTermsAr.TermsAr.FindIndex(
                                item =>
                                    item.TermFullNormForm.Split(' ')
                                        .Contains(authTermsAr.TermsAr[i].TermFullNormForm) && authTermsAr.TermsAr[i].TermFullNormForm != item.TermFullNormForm);
                if (cont != -1)
                {
                    foreach (var curPosition in authTermsAr.TermsAr[i].Pos)
                    {
                        if (curPosition.IndexElement.Contains(i))
                            curPosition.IndexElement.Remove(i);
                        if(curPosition.IndexElement.Count == 0)
                            authTermsAr.RootTermsTree.DeleteRange(curPosition.Range);
                    }
                    authTermsAr.TermsAr.RemoveAt(i);
                    i--;
                }
            }
            var prepareIndex = new List<PatternGenerationAssistItem>();
            var indexInput = TmpPath + FolderPath + "\\AuthTermsAr.txt";
            var serializeWriter = new StreamWriter(indexInput, false, Encoding.GetEncoding("Windows-1251"));
            var curStartPos = 0;
            for (var i = 0; i < authTermsAr.TermsAr.Count; i++)
            {
                var newItem = new PatternGenerationAssistItem();
                newItem.TermsArIndex = i;
                newItem.StartPos = curStartPos;
                prepareIndex.Add(newItem);
                serializeWriter.WriteLine(authTermsAr.TermsAr[i].TermWord);
                curStartPos = curStartPos + authTermsAr.TermsAr[i].TermWord.Length + 1;
            }
            serializeWriter.Close();
            GetNewPatternsXml(indexInput);
            GetNewPatterns(prepareIndex, authTermsAr.TermsAr);
            return true;
        }

        //CombTerms        
        public void GetXmlCombTerms(CombTerms combTermsAr)
        {
            var batOutput = TmpPath + FolderPath + "\\CombTerms.bat";
            var targetPatternsOutput = TmpPath + FolderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false, Encoding.GetEncoding("Windows-1251"));
            var curPattern = "";
            var fs = new StreamReader(ProgrammPath + "\\Patterns\\COMBNS_TERM.txt", Encoding.GetEncoding("Windows-1251"));
            curPattern = fs.ReadLine();
            if (curPattern != null)
            {
                curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                curPattern = curPattern.Trim();
                if (curPattern.IndexOf("CT") != -1)
                {
                    var len = curPattern.IndexOf("CT") + "CT".Length;
                    //int k = FindFunctions.findINList(PatternsModel, curPattern, 1);
                    var k = PatternsModel.FindIndex(item => item.First == curPattern);
                    if (k == -1)
                    {
                        var newP = new Pair<string, string>();
                        targetPatternsWriter.WriteLine(curPattern);
                        newP.First = curPattern;
                        newP.Second = curPattern.Substring(len).Trim();
                        PatternsModel.Add(newP);
                    }
                    len = 0;
                }
                while (true)
                {
                    curPattern = fs.ReadLine();
                    if (curPattern == null) break;
                    if (curPattern != "")
                    {
                        curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                        curPattern = curPattern.Trim();
                        if (curPattern.IndexOf("CT") != -1)
                        {
                            var len = curPattern.IndexOf("CT") + "CT".Length;
                            //int k = FindFunctions.findINList(PatternsModel, curPattern, 1);
                            var k = PatternsModel.FindIndex(item => item.First == curPattern);
                            if (k == -1)
                            {
                                var newP = new Pair<string, string>();
                                targetPatternsWriter.WriteLine(curPattern);
                                newP.First = curPattern;
                                newP.Second = curPattern.Substring(len).Trim();
                                PatternsModel.Add(newP);
                            }
                            len = 0;
                        }
                    }
                }
                targetPatternsWriter.Close();
                var lsplExe = ProgrammPath + "\\bin\\lspl-find.exe";
                var lsplPatterns = ProgrammPath + "\\Patterns\\COMBNS_TERM.txt";
                //string LSPL_output = tmpPath + folderPath + "\\CombTermsOutput.xml";
                var lsplOutputText = TmpPath + FolderPath + "\\CombTermsOutputText.xml";
                var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
                sw.WriteLine("\"" + lsplExe
                     + "\" -i \"" + InputFile
                     + "\" -p \"" + lsplPatterns
                     //+ "\" -o \"" + LSPL_output
                     + "\" -t \"" + lsplOutputText
                     //+ "\" -r \"" + LSPL_output_patterns
                     + "\" -s \"" + targetPatternsOutput + "\" ");
                sw.Close();
                var startInfo = new ProcessStartInfo(batOutput);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(startInfo).WaitForExit();
                GetCombTerms(combTermsAr);
                var patternsName = FormCombComponentsPatterns(combTermsAr);
                GetXmlCombComponentsTerms(combTermsAr, patternsName);
            }
            else
            {
                MessageBox.Show("Ошибка! Некорректный файл с шаблонами бессоюзных терминов!");
                Application.Exit();
            }
        }
        public string FormCombComponentsPatterns(CombTerms combTermsAr)
        {
            var combComponentsPatterns = TmpPath + FolderPath + "\\COMP_COMB_TERM.txt";
            var patterns = "";
            var sw = new StreamWriter(combComponentsPatterns, false, Encoding.GetEncoding("Windows-1251"));
            //AuxiliaryFunctions.PrintConstantPatterns(sw);
            for (var i = 0; i < combTermsAr.TermsAr.Count; i++)
            {
                for (var j = 0; j < combTermsAr.TermsAr[i].Components.Count; j++)
                {
                    //CombTermsAr.TermsAr[i].Components[j].NPattern = AuxiliaryFunctions.NormalizeNPattern(CombTermsAr.TermsAr[i].Components[j].NPattern);
                    var patternName = "CCP-" + IntegerToRoman(i + 1) + "-" + IntegerToRoman(j + 1);
                    if (combTermsAr.TermsAr[i].Components[j].NPattern != "")
                    {
                        if (!combTermsAr.TermsAr[i].Components[j].TermFragment.Contains('+'))
                        {
                            sw.WriteLine(patternName + " = " + combTermsAr.TermsAr[i].Components[j].NPattern);
                            patterns = patterns + patternName + "\n";
                        }
                    }
                }
            }
            sw.Close();
            return patterns;

        }
        public void GetXmlCombComponentsTerms(CombTerms combTermsAr, string patterns)
        {
            //--------------------------------
            var lsplExe = ProgrammPath + "\\bin\\lspl-find.exe";
            var lsplPatterns = TmpPath + FolderPath + "\\COMP_COMB_TERM.txt";
            var lsplOutputText = TmpPath + FolderPath + "\\CombComponentsTermsOutputText.xml";
            var batOutput = TmpPath + FolderPath + "\\CombComponentsTerms.bat";
            var targetPatternsOutput = TmpPath + FolderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false, Encoding.GetEncoding("Windows-1251"));
            targetPatternsWriter.Write(patterns);
            targetPatternsWriter.Close();
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            sw.WriteLine("\"" + lsplExe
                     + "\" -i \"" + InputFile
                     + "\" -p \"" + lsplPatterns
                     //+ "\" -o \"" + LSPL_output
                     + "\" -t \"" + lsplOutputText
                     //+ "\" -r \"" + LSPL_output_patterns
                     + "\" -s \"" + targetPatternsOutput + "\" ");
            //Write a line of text
            //sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            //sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" " + patterns);
            //Close the file
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(startInfo).WaitForExit();
            GetCombComponentsTerms(combTermsAr);
            //---------------------------------
        }
        public bool GetCombComponentsTerms(CombTerms combTermsAr)
        {
            var curPos = new Point();
            var curPat = "";
            var curFragment = "";
            var lastNodeName = "";
            var lastPos = "";
            var compV = new List<CombComponent>();
            using (var xml = XmlReader.Create(TmpPath + FolderPath + "\\CombComponentsTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                switch (xml.Name)
                                {
                                    case "goal":
                                        if (xml.HasAttributes)
                                        {
                                            while (xml.MoveToNextAttribute())
                                            {
                                                if (xml.Name == "name")
                                                {
                                                    curPat = xml.Value.Trim();
                                                }
                                            }
                                        }
                                        break;
                                    case "match":
                                        if (xml.HasAttributes)
                                        {
                                            while (xml.MoveToNextAttribute())
                                            {
                                                if (xml.Name == "startPos")
                                                {
                                                    curPos.X = Convert.ToInt32(xml.Value.Trim());
                                                }
                                                if (xml.Name == "endPos")
                                                {
                                                    curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                                }
                                            }
                                        }
                                        break;
                                    case "result":
                                        lastNodeName = xml.Name;
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "pos")
                                            {
                                                lastPos = xml.Value.Trim();
                                            }
                                        }
                                        break;
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
                                    curFragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    if (curPat.IndexOf("CCP", StringComparison.Ordinal) != -1)
                                    {
                                        var word = xml.Value.Trim();
                                        var check = word.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                        if (check != -1)
                                            continue;
                                        //int k = FindFunctions.findINList(CompV, word);
                                        var let = word.ToList().FindIndex(char.IsDigit);
                                        if (let != -1)
                                            continue;
                                        if (word.Length > 1)
                                        {
                                            string fullNormWord = "";
                                            List<string> wordSplt = new List<string>(word.Trim().Split(' '));
                                            foreach (var wordPart in wordSplt)
                                            {
                                                fullNormWord += " " + Lem.CreateParadigmCollectionFromForm(wordPart, false, true)[0].Norm;
                                            }
                                            fullNormWord = fullNormWord.Trim();
                                            bool matchEng = WordFilter(fullNormWord) || WordFilter(word);
                                            if (matchEng)
                                                continue;
                                            var k = compV.FindIndex(item => item.TermFullNormForm == fullNormWord && item.Pattern == curPat.Substring("CCP-".Length).Trim());
                                            if (k == -1)
                                            {
                                                var newEl = new CombComponent();
                                                newEl.Pos.Add(curPos);
                                                newEl.TermWord = word.Trim();
                                                newEl.PoSstr = lastPos;
                                                newEl.TermFullNormForm = fullNormWord.Trim();
                                                newEl.TermFragment = curFragment;
                                                newEl.Pattern = curPat.Substring("CCP-".Length).Trim();
                                                newEl.TermFullNormForm = fullNormWord.Trim();
                                                compV.Add(newEl);

                                            }
                                            else
                                            {
                                                if (FindPos(compV[k].Pos, curPos) == -1)
                                                {
                                                    compV[k].Pos.Add(curPos);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            while (compV.Count > 0)
            {
                for (var i = 0; i < compV[0].Pos.Count; i++)
                {
                    int combTermIndex = RomanToInteger(compV[0].Pattern.Substring(0, compV[0].Pattern.IndexOf('-')).Trim()) - 1;
                    int combTermComponentIndex =
                        RomanToInteger(compV[0].Pattern.Substring(compV[0].Pattern.IndexOf('-') + 1).Trim()) - 1;
                    combTermsAr.TermsAr[combTermIndex].Components[combTermComponentIndex].Pos.InsertRange(combTermsAr.TermsAr[combTermIndex].Components[combTermComponentIndex].Pos.Count, compV[0].Pos);
                    combTermsAr.TermsAr[combTermIndex].Components[combTermComponentIndex].Frequency = combTermsAr.TermsAr[combTermIndex].Components[combTermComponentIndex].Pos.Count;
                }
                compV.RemoveAt(0);
            }
            return true;
        }
        public bool GetCombTerms(CombTerms combTermsAr)
        {
            var curPos = new Point();
            var curPat = "";
            var curFragment = "";
            var lastNodeName = "";
            var lastPos = "";
            using (var xml = XmlReader.Create(TmpPath + FolderPath + "\\CombTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                curPat = xml.Value.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                curPos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPos = xml.Value.Trim();
                                        }
                                    }

                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    curFragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    if (curPat.IndexOf("FCT", StringComparison.Ordinal) == 0)
                                    {
                                        var word = xml.Value.Trim();
                                        var let = word.ToList().FindIndex(sym => char.IsDigit(sym));
                                        if (let != -1)
                                            continue;
                                        if (word.Length > 1)
                                        {
                                            var check = word.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                            if (check != -1)
                                                continue;
                                            //int k = FindFunctions.findINList(CombTermsAr.TermsAr, word, lastPOS);
                                            string fullNormWord = "";
                                            List<string> wordSplt = new List<string>(word.Trim().Split(' '));
                                            foreach (var wordPart in wordSplt)
                                            {
                                                fullNormWord += " " + Lem.CreateParadigmCollectionFromForm(wordPart, false, true)[0].Norm;
                                            }
                                            fullNormWord = fullNormWord.Trim();
                                            bool matchEng = WordFilter(fullNormWord) || WordFilter(word);
                                            if (matchEng)
                                                continue;
                                            var k = combTermsAr.TermsAr.FindIndex(item => item.TermFullNormForm == fullNormWord && item.PoSstr == lastPos);
                                            if (k == -1)
                                            {
                                                var curRange = new Range(curPos);
                                                var e = combTermsAr.RootTermsTree.FindRange(curRange);
                                                var eExtension = combTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                var newEl = new CombTerm();
                                                newEl.mainPage = GetPageNumberByPositon(curRange);
                                                newEl.Pos.Add(null);
                                                newEl.TermWord = word.Trim();
                                                newEl.PoSstr = lastPos;
                                                newEl.Frequency = 1;
                                                newEl.TermFullNormForm = fullNormWord.Trim();
                                                newEl.TermFragment = curFragment;
                                                newEl.Pattern = curPat.Substring("FCT".Length);
                                                newEl.PatCounter = 0;
                                                newEl.Kind = KindOfTerm.CombTerm;
                                                if (e == null && eExtension == null)
                                                {
                                                    e = combTermsAr.RootTermsTree.AddRange(curRange);
                                                    combTermsAr.TermsAr.Add(newEl);
                                                    //e = combTermsAr.RootTermsTree.FindRange(curRange);
                                                    e.IndexElement.Add(combTermsAr.TermsAr.Count - 1);
                                                    combTermsAr.TermsAr[combTermsAr.TermsAr.Count - 1].Pos[combTermsAr.TermsAr[combTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                }
                                                else
                                                {
                                                    if (e != null && eExtension == null)
                                                    {
                                                        combTermsAr.TermsAr.Add(newEl);
                                                        e.IndexElement.Add(combTermsAr.TermsAr.Count - 1);
                                                        combTermsAr.TermsAr[combTermsAr.TermsAr.Count - 1].Pos[combTermsAr.TermsAr[combTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var curRange = new Range(curPos);
                                                var e = combTermsAr.RootTermsTree.FindRange(curRange);
                                                var eExtension = combTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                if (e == null && eExtension == null)
                                                {
                                                    e = combTermsAr.RootTermsTree.AddRange(curRange);
                                                    //e = combTermsAr.RootTermsTree.FindRange(curRange);
                                                    e.IndexElement.Add(k);
                                                    combTermsAr.TermsAr[k].Pos.Add(e);
                                                    combTermsAr.TermsAr[k].Frequency++;
                                                }
                                                else
                                                {
                                                    if (e != null && !e.IndexElement.Contains(k))
                                                    {
                                                        e.IndexElement.Add(k);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (curPat.IndexOf("CT") == 0)
                                    {
                                        var word = xml.Value.Trim();
                                        var check = word.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                        if (check != -1)
                                            continue;
                                        var let = word.ToList().FindIndex(sym => char.IsDigit(sym));
                                        if (let != -1)
                                            continue;
                                        if (word.Length > 1)
                                        {
                                            var curMainPat = curPat.Substring("CT".Length, curPat.IndexOf('-') - "CT".Length).Trim();
                                            var curRange = new Range(curPos);
                                            var e = combTermsAr.RootTermsTree.FindRange(curRange);
                                            if (e != null)
                                            {
                                                string fullNormWord = "";
                                                List<string> wordSplt = new List<string>(word.Trim().Split(' '));
                                                foreach (var wordPart in wordSplt)
                                                {
                                                    fullNormWord += " " + Lem.CreateParadigmCollectionFromForm(wordPart, false, true)[0].Norm;
                                                }
                                                fullNormWord = fullNormWord.Trim();
                                                bool matchEng = WordFilter(fullNormWord) || WordFilter(word);
                                                if (matchEng)
                                                    continue;
                                                var curTerm = FindPattern(combTermsAr.TermsAr, e.IndexElement, curMainPat);
                                                if (curTerm != -1 && combTermsAr.TermsAr[curTerm].Components.FindIndex(item => item.TermFullNormForm == fullNormWord) == -1)
                                                {
                                                    var newEl = new CombComponent();
                                                    newEl.TermWord = word.Trim();
                                                    newEl.PoSstr = lastPos;
                                                    newEl.TermFragment = curFragment;
                                                    newEl.Frequency = 1;
                                                    newEl.TermFullNormForm = fullNormWord.Trim();
                                                    newEl.Pattern = curPat.Substring(curPat.IndexOf('-') + 1).Trim();
                                                    combTermsAr.TermsAr[curTerm].Components.Add(newEl);
                                                }
                                            }
                                        }
                                    }
                                    else if (curPat.IndexOf("NPFCT", StringComparison.Ordinal) == 0)
                                    {
                                        var word = xml.Value.Trim();
                                        var curRange = new Range(curPos);
                                        var e = combTermsAr.RootTermsTree.FindRange(curRange);
                                        if (e != null)
                                        {
                                            var k = FindPattern(combTermsAr.TermsAr, e.IndexElement, curPat.Substring("NPFCT".Length).Trim());
                                            if (k != -1)
                                                combTermsAr.TermsAr[k].NPattern = NormalizeNPattern(word);
                                        }
                                    }
                                    else if (curPat.IndexOf("NPCT", StringComparison.Ordinal) == 0)
                                    {
                                        var word = xml.Value.Trim();
                                        var curMainPat = curPat.Substring("NPCT".Length, curPat.IndexOf('-') - "NPCT".Length).Trim();
                                        var curRange = new Range(curPos);
                                        var e = combTermsAr.RootTermsTree.FindRange(curRange);
                                        if (e != null)
                                        {
                                            var k = FindPattern(combTermsAr.TermsAr, e.IndexElement, curMainPat);
                                            if (k != -1)
                                            {
                                                var a = FindPattern(combTermsAr.TermsAr[k].Components, curPat.Substring(curPat.IndexOf('-') + 1).Trim());
                                                if (a != -1)
                                                    combTermsAr.TermsAr[k].Components[a].NPattern = NormalizeNPattern(word);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            //getFrequency(CombTerms);
            return true;
        }

        //DictTerms
        public void GetXmlDictTerms(Terms dictTermsAr)
        {
            var lsplPatterns = "";
            StreamReader fs = null;
            switch (Dictionary)
            {
                case DictionaryF.ItTerm:
                    {
                        lsplPatterns = ProgrammPath + "\\Patterns\\NEW_TERM_IT.txt";
                        fs = new StreamReader(lsplPatterns, Encoding.GetEncoding(1251));
                        break;
                    }
                case DictionaryF.FTerm:
                    {
                        lsplPatterns = ProgrammPath + "\\Patterns\\F_TERM.txt";
                        fs = new StreamReader(lsplPatterns, Encoding.GetEncoding(1251));
                        break;
                    }
            }
            var batOutput = TmpPath + FolderPath + "\\DictTerms.bat";
            var targetPatternsOutput = TmpPath + FolderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false, Encoding.GetEncoding("Windows-1251"));
            var patternsName = "";
            var lsplExe = ProgrammPath + "\\bin\\lspl-find.exe";
            var lsplOutputText = TmpPath + FolderPath + "\\DictTermsOutputText.xml";
            var batCommand = "\"" + lsplExe
                     + "\" -i \"" + InputFile
                     + "\" -p \"" + lsplPatterns
                     //+ "\" -o \"" + LSPL_output
                     + "\" -t \"" + lsplOutputText
                     //+ "\" -r \"" + LSPL_output_patterns
                     + "\" -s \"" + targetPatternsOutput + "\" ";
            var callUtilit = false;
            var curPattern = fs.ReadLine();
            if (!string.IsNullOrEmpty(curPattern))
            {
                curPattern = curPattern.Trim();
                //curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                targetPatternsWriter.WriteLine(curPattern);
                var curPat = new Pair<string, string>
                {
                    First = curPattern.Substring(0, curPattern.IndexOf('=')).Trim(),
                    Second = curPattern.Substring(curPattern.IndexOf("=", StringComparison.Ordinal) + 1).Trim()
                };
                DictPatterns.Add(curPat);
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
                    //if (BAT_command.Length + patternsName.Length + curPatternName.Length < MAX_COMMAND)
                    //{
                    targetPatternsWriter.WriteLine(curPatternName);
                    var curPat = new Pair<string, string>();
                    curPat.First = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                    curPat.Second = curPattern.Substring(curPattern.IndexOf("=") + 1).Trim();
                    DictPatterns.Add(curPat);
                    //callUtilit = false;
                    //}
                    //else
                    //{
                    //    StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
                    //    sw.WriteLine(BAT_command + patternsName);
                    //    sw.Close();
                    //    ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
                    //    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //    ////System.Diagnostics.Process.Start(startInfo).WaitForExit();
                    //    GetDictTerms(DictTermsAr);
                    //    patternsName = curPatternName;
                    //    pair<string, string> cur_pat = new pair<string, string>();
                    //    cur_pat.first = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                    //    cur_pat.second = curPattern.Substring(curPattern.IndexOf("=") + 1).Trim();
                    //    DictPatterns.Add(cur_pat);
                    //    callUtilit = true;
                    //}
                }
            }
            //if (!callUtilit)
            //{
            targetPatternsWriter.Close();
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            sw.WriteLine(batCommand);
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(startInfo).WaitForExit();
            GetDictTerms(dictTermsAr);
            //}
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
        public bool GetDictTerms(Terms dictTermsAr)
        {
            var curPos = new Point();
            var curPat = "";
            var curFragment = "";
            var lastNodeName = "";
            var lastPos = "";
            using (var xml = XmlReader.Create(TmpPath + FolderPath + "\\DictTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                curPat = xml.Value.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                curPos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPos = xml.Value.Trim();
                                        }
                                    }

                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    curFragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    var word = xml.Value.Trim();
                                    var check = word.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                    if (check != -1)
                                        continue;
                                    var let = word.ToList().FindIndex(sym => char.IsDigit(sym));
                                    if (let != -1)
                                        continue;
                                    if (word.Length > 1)
                                    {
                                        string fullNormWord = "";
                                        List<string> wordSplt = new List<string>(word.Trim().Split(' '));
                                        foreach (var wordPart in wordSplt)
                                        {
                                            fullNormWord += " " + Lem.CreateParadigmCollectionFromForm(wordPart, false, true)[0].Norm;
                                        }
                                        fullNormWord = fullNormWord.Trim();
                                        bool matchEng = WordFilter(fullNormWord) || WordFilter(word);
                                        if (matchEng)
                                            continue;
                                        //int k = FindFunctions.findINList(DictTermsAr.TermsAr, word);
                                        var k = dictTermsAr.TermsAr.FindIndex(item => item.TermFullNormForm == fullNormWord);
                                        if (k == -1)
                                        {
                                            var curRange = new Range(curPos);
                                            var newEl = new Term();
                                            newEl.mainPage = GetPageNumberByPositon(curRange);
                                            newEl.Frequency = 0;
                                            newEl.Kind = DictTerm;
                                            //int element = FindFunctions.findINList(DictPatterns, cur_pat, 1);
                                            var element = DictPatterns.FindIndex(item => item.First == curPat);
                                            newEl.NPattern = DictPatterns[element].Second;//<-------
                                            newEl.TermFullNormForm = fullNormWord.Trim();
                                            newEl.PatCounter = 1;
                                            newEl.Pattern = curPat;
                                            newEl.Pos.Add(null);
                                            newEl.SetToDel = false;
                                            newEl.TermFragment = curFragment;
                                            newEl.TermWord = word.Trim();
                                            newEl.PoSstr = lastPos;
                                            var e = dictTermsAr.RootTermsTree.FindRange(curRange);
                                            var eExtension = dictTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                            if (e == null && eExtension == null)
                                            {
                                                e = dictTermsAr.RootTermsTree.AddRange(curRange);
                                                //e = dictTermsAr.RootTermsTree.FindRange(curRange);
                                                dictTermsAr.TermsAr.Add(newEl);
                                                e.IndexElement.Add(dictTermsAr.TermsAr.Count - 1);
                                                dictTermsAr.TermsAr[dictTermsAr.TermsAr.Count - 1].Pos[dictTermsAr.TermsAr[dictTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                            }
                                            else
                                            {
                                                if (e != null && eExtension == null)
                                                {
                                                    dictTermsAr.TermsAr.Add(newEl);
                                                    e.IndexElement.Add(dictTermsAr.TermsAr.Count - 1);
                                                    dictTermsAr.TermsAr[dictTermsAr.TermsAr.Count - 1].Pos[dictTermsAr.TermsAr[dictTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                }
                                            }
                                        }

                                        else
                                        {
                                            var curRange = new Range(curPos);
                                            var e = dictTermsAr.RootTermsTree.FindRange(curRange);
                                            var eExtension = dictTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                            if (e == null && eExtension == null)
                                            {
                                                e = dictTermsAr.RootTermsTree.AddRange(curRange);
                                                //e = dictTermsAr.RootTermsTree.FindRange(curRange);
                                                e.IndexElement.Add(k);
                                                dictTermsAr.TermsAr[k].Pos.Add(e);
                                                dictTermsAr.TermsAr[k].Frequency++;
                                            }
                                            else
                                            {
                                                if (e != null && !e.IndexElement.Contains(k))
                                                    e.IndexElement.Add(k);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            getFrequency_(dictTermsAr);
            return true;
        }

        //NonDictTerms
        public void GetXmlNonDictTerms(NonDictTerms nonDictTermsAr)
        {
            var batOutput = TmpPath + FolderPath + "\\NontDictTerms.bat";
            var targetPatternsOutput = TmpPath + FolderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false, Encoding.GetEncoding("Windows-1251"));
            var curPattern = "";
            var fs = new StreamReader(ProgrammPath + "\\Patterns\\NONDICT_TERM_NEW.txt", Encoding.GetEncoding("Windows-1251"));
            var lsplPatterns = ProgrammPath + "\\Patterns\\NONDICT_TERM_NEW.txt";
            //string patternsName = "";
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
                    //int k = FindFunctions.findINList(PatternsModel, curPattern, 1);
                    var k = PatternsModel.FindIndex(item => item.First == curPattern);
                    if (k == -1 && len != 0)
                    {
                        var newP = new Pair<string, string>();
                        //patternsName = patternsName + " " + curPattern.Trim();
                        targetPatternsWriter.WriteLine(curPattern.Trim());
                        newP.First = curPattern;
                        newP.Second = curPattern.Substring(len).Trim();
                        PatternsModel.Add(newP);
                    }
                }
            }
            targetPatternsWriter.Close();
            //--------------------------------
            var lsplExe = ProgrammPath + "\\bin\\lspl-find.exe";
            var lsplOutputText = TmpPath + FolderPath + "\\NontDictTermsOutputText.xml";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + ProgrammPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + lsplExe
                     + "\" -i \"" + InputFile
                     + "\" -p \"" + lsplPatterns
                     //+ "\" -o \"" + LSPL_output
                     + "\" -t \"" + lsplOutputText
                     //+ "\" -r \"" + LSPL_output_patterns
                     + "\" -s \"" + targetPatternsOutput + "\" ");
            //Close the file
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(startInfo).WaitForExit();
            GetNonDictTerms(nonDictTermsAr);
            //---------------------------------
        }
        public bool GetNonDictTerms(NonDictTerms nonDictTermsAr)
        {
            var curPos = new Point();
            var curPat = "";
            var curFragment = "";
            var lastNodeName = "";
            var lastPos = "";
            using (var xml = XmlReader.Create(TmpPath + FolderPath + "\\NontDictTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                curPat = xml.Value.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                curPos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPos = xml.Value.Trim();
                                        }
                                    }

                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    curFragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    switch (curPat[0])
                                    {
                                        case 'F':
                                            {

                                                var word = xml.Value.Trim();
                                                var check = word.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                                if (check != -1)
                                                    continue;
                                                var let = word.ToList().FindIndex(sym => char.IsDigit(sym));
                                                if (let != -1)
                                                    continue;
                                                if (word.Length > 1)
                                                {
                                                    string fullNormWord = "";
                                                    List<string> wordSplt = new List<string>(word.Trim().Split(' '));
                                                    foreach (var wordPart in wordSplt)
                                                    {
                                                        fullNormWord += " " + Lem.CreateParadigmCollectionFromForm(wordPart, false, true)[0].Norm;
                                                    }
                                                    fullNormWord = fullNormWord.Trim();
                                                    bool matchEng = WordFilter(fullNormWord) || WordFilter(word);
                                                    if (matchEng)
                                                        continue;
                                                    //int k = FindFunctions.findINList(NonDictTermsAr.TermsAr, word, lastPOS);
                                                    var k = nonDictTermsAr.TermsAr.FindIndex(item => item.TermFullNormForm == fullNormWord && item.PoSstr == lastPos);
                                                    if (k == -1)
                                                    {
                                                        var curRange = new Range(curPos);
                                                        var newEl = new NonDictTerm();
                                                        newEl.mainPage = GetPageNumberByPositon(curRange);
                                                        newEl.Pos.Add(null);
                                                        newEl.TermWord = word.Trim();
                                                        newEl.TermFullNormForm = fullNormWord.Trim();
                                                        newEl.Frequency = 1;
                                                        newEl.TermFragment = curFragment;
                                                        newEl.Pattern = curPat.Substring("F".Length).Trim();
                                                        newEl.PoSstr = lastPos;
                                                        newEl.Kind = KindOfTerm.NonDictTerm;
                                                        var e = nonDictTermsAr.RootTermsTree.FindRange(curRange);
                                                        var eExtension = nonDictTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                        if (e == null && eExtension == null)
                                                        {
                                                            e = nonDictTermsAr.RootTermsTree.AddRange(curRange);
                                                            //e = nonDictTermsAr.RootTermsTree.FindRange(curRange);
                                                            nonDictTermsAr.TermsAr.Add(newEl);
                                                            e.IndexElement.Add(nonDictTermsAr.TermsAr.Count - 1);
                                                            nonDictTermsAr.TermsAr[nonDictTermsAr.TermsAr.Count - 1].Pos[nonDictTermsAr.TermsAr[nonDictTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                        }
                                                        else
                                                        {
                                                            if (e != null && eExtension == null)
                                                            {
                                                                nonDictTermsAr.TermsAr.Add(newEl);
                                                                e.IndexElement.Add(nonDictTermsAr.TermsAr.Count - 1);
                                                                nonDictTermsAr.TermsAr[nonDictTermsAr.TermsAr.Count - 1].Pos[nonDictTermsAr.TermsAr[nonDictTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        var curRange = new Range(curPos);
                                                        var e = nonDictTermsAr.RootTermsTree.FindRange(curRange);
                                                        var eExtension = nonDictTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                        if (e == null && eExtension == null)
                                                        {
                                                            e = nonDictTermsAr.RootTermsTree.AddRange(curRange);
                                                            //e = nonDictTermsAr.RootTermsTree.FindRange(curRange);
                                                            e.IndexElement.Add(k);
                                                            nonDictTermsAr.TermsAr[k].Pos.Add(e);
                                                            nonDictTermsAr.TermsAr[k].Frequency++;
                                                        }
                                                        else
                                                        {
                                                            if (e != null && !e.IndexElement.Contains(k))
                                                                e.IndexElement.Add(k);
                                                        }

                                                    }
                                                }
                                                break;
                                            }
                                        case 'C':
                                            {
                                                var word = xml.Value.Trim();
                                                var check = word.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                                if (check != -1)
                                                    continue;
                                                var let = word.ToList().FindIndex(sym => char.IsDigit(sym));
                                                if (let != -1)
                                                    continue;
                                                if (word.Length > 1)
                                                {
                                                    var curRange = new Range(curPos);
                                                    var e = nonDictTermsAr.RootTermsTree.FindRange(curRange);
                                                    if (e != null)
                                                    {
                                                        var curTerm = FindPattern(nonDictTermsAr.TermsAr, e.IndexElement, curPat.Substring("Ca".Length, curPat.IndexOf('-') - "Ca".Length).Trim());
                                                        if (curTerm != -1)
                                                        {
                                                            //int block = FindFunctions.findBlock(NonDictTermsAr.TermsAr[cur_term].Blocks, cur_pat[1].ToString());
                                                            var block = nonDictTermsAr.TermsAr[curTerm].Blocks.FindIndex(item => item.Block == curPat[1].ToString());
                                                            if (block != -1)
                                                            {
                                                                var curComp = FindPattern(nonDictTermsAr.TermsAr[curTerm].Blocks[block].Components, curPat.Substring(curPat.IndexOf('-') + 1).Trim());
                                                                if (curComp == -1)
                                                                {
                                                                    var newEl = new NonDictComponent();
                                                                    newEl.Component = word;
                                                                    newEl.PoSstr = lastPos;
                                                                    newEl.Pattern = curPat.Substring(curPat.IndexOf('-') + 1).Trim();
                                                                    nonDictTermsAr.TermsAr[curTerm].Blocks[block].Components.Add(newEl);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var newBl = new NonDictBlock();
                                                                newBl.Block = curPat[1].ToString();
                                                                var newEl = new NonDictComponent();
                                                                newEl.Component = word;
                                                                newEl.PoSstr = lastPos;
                                                                newEl.Pattern = curPat.Substring(curPat.IndexOf('-') + 1).Trim();
                                                                newBl.Components.Add(newEl);
                                                                nonDictTermsAr.TermsAr[curTerm].Blocks.Add(newBl);
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 'N':
                                            {
                                                var word = xml.Value.Trim();
                                                var curRange = new Range(curPos);
                                                var e = nonDictTermsAr.RootTermsTree.FindRange(curRange);
                                                if (e != null)
                                                {
                                                    switch (curPat["NP".Length])
                                                    {
                                                        case 'F':
                                                            {
                                                                var k = FindPattern(nonDictTermsAr.TermsAr, e.IndexElement, curPat.Substring("NPF".Length).Trim());
                                                                if (k != -1)
                                                                {
                                                                    nonDictTermsAr.TermsAr[k].NPattern = NormalizeNPattern(word);
                                                                }
                                                                break;
                                                            }
                                                        case 'C':
                                                            {
                                                                var curTerm = FindPattern(nonDictTermsAr.TermsAr, e.IndexElement, curPat.Substring("NPCa".Length, curPat.IndexOf('-') - "NPCa".Length).Trim());
                                                                if (curTerm != -1)
                                                                {
                                                                    //int block = FindFunctions.findBlock(NonDictTermsAr.TermsAr[cur_term].Blocks, cur_pat[4].ToString());
                                                                    var block = nonDictTermsAr.TermsAr[curTerm].Blocks.FindIndex(item => item.Block == curPat[4].ToString());
                                                                    if (block != -1)
                                                                    {
                                                                        var curComp = FindPattern(nonDictTermsAr.TermsAr[curTerm].Blocks[block].Components, curPat.Substring(curPat.IndexOf('-') + 1).Trim());
                                                                        if (curComp != -1)
                                                                        {
                                                                            nonDictTermsAr.TermsAr[curTerm].Blocks[block].Components[curComp].NPattern = NormalizeNPattern(word);
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            }
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                }

                            }
                            break;
                    }
                }
            }
            getFrequency_(nonDictTermsAr);
            return true;
        }

        //SynTerms
        public void GetXmlSynTerms(SynTerms synTermsAr)
        {
            var batOutput = TmpPath + FolderPath + "\\SynTerms.bat";
            var targetPatternsOutput = TmpPath + FolderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false, Encoding.GetEncoding("Windows-1251"));
            var curPattern = "";
            var fs = new StreamReader(ProgrammPath + "\\Patterns\\SYN_TERM.txt", Encoding.GetEncoding("Windows-1251"));
            var lsplPatterns = ProgrammPath + "\\Patterns\\SYN_TERM.txt";
            var patternsName = "";
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
                        //int k = FindFunctions.findINList(PatternsModel, curPattern, 1);
                        var k = PatternsModel.FindIndex(item => item.First == curPattern);
                        if (k == -1)
                        {
                            var newP = new Pair<string, string>();
                            //patternsName = patternsName + " " + curPattern;
                            targetPatternsWriter.WriteLine(curPattern);
                            newP.First = curPattern;
                            newP.Second = curPattern.Substring(len).Trim();
                            PatternsModel.Add(newP);
                        }
                        len = 0;
                    }
                }
            }
            targetPatternsWriter.Close();
            //--------------------------------
            var lsplExe = ProgrammPath + "\\bin\\lspl-find.exe";
            var lsplOutputText = TmpPath + FolderPath + "\\SynTermsOutputText.xml";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + ProgrammPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + lsplExe
                    + "\" -i \"" + InputFile
                    + "\" -p \"" + lsplPatterns
                    //+ "\" -o \"" + LSPL_output
                    + "\" -t \"" + lsplOutputText
                    //+ "\" -r \"" + LSPL_output_patterns
                    + "\" -s \"" + targetPatternsOutput + "\" ");
            //Close the file
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput) { WindowStyle = ProcessWindowStyle.Hidden };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            GetSynTerms(synTermsAr);
            //---------------------------------
        }
        public bool GetSynTerms(SynTerms synTermsAr)
        #pragma warning restore CS0436 // Тип конфликтует с импортированным типом
        {
            var curPos = new Point();
            var curPat = "";
            var curFragment = "";
            var lastNodeName = "";
            var lastPos = "";
            using (var xml = XmlReader.Create(TmpPath + FolderPath + "\\SynTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                curPat = xml.Value.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                curPos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPos = xml.Value.Trim();
                                        }
                                    }

                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    curFragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    if (curPat.IndexOf("NP", StringComparison.Ordinal) != 0)
                                    {
                                        var word = xml.Value.Trim();
                                        var let = word.ToList().FindIndex(char.IsDigit);
                                        if (let != -1)
                                            continue;
                                        if (word.Length > 1)
                                        {
                                            var alt = new Pair<SynTermAlternative, SynTermAlternative>();
                                            alt.First = new SynTermAlternative();
                                            alt.Second = new SynTermAlternative();
                                            var y = word.IndexOf(" - ", StringComparison.Ordinal);
                                            if (y != -1)
                                            {
                                                var partsPatterns = curPat.Substring(curPat.IndexOf('-') + 1).Trim();
                                                alt.First.Alternative = word.Substring(0, y);
                                                var firstAltWords = alt.First.Alternative.Split(' ').ToList();
                                                var numWords = firstAltWords.Count;
                                                var poSlist = lastPos.Split(' ').ToList();
                                                var firstAltPos = "";
                                                var secondAltPos = "";
                                                var lDelimeter = false;
                                                foreach (var pos in poSlist)
                                                {
                                                    if (pos == "L")
                                                    {
                                                        lDelimeter = true;
                                                        continue;
                                                    }
                                                    if (lDelimeter)
                                                        secondAltPos += " " + pos;
                                                    else
                                                        firstAltPos += " " + pos;
                                                }
                                                var check = alt.First.Alternative.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                                if (check != -1)
                                                    continue;

                                                List<string> wordSplt = new List<string>(alt.First.Alternative.Split(' '));
                                                string fullNormWord = "";
                                                foreach (var wordPart in wordSplt)
                                                {
                                                    fullNormWord += " " + Lem.CreateParadigmCollectionFromForm(wordPart, false, true)[0].Norm;
                                                }
                                                bool matchEng = WordFilter(fullNormWord) || WordFilter(word);
                                                if (matchEng)
                                                    continue;

                                                alt.First.PoSstr = firstAltPos.Trim();
                                                alt.First.PatternPart = "A";
                                                alt.First.TermFullNormForm = fullNormWord.Trim();
                                                alt.First.Pattern = partsPatterns.Substring(0, partsPatterns.IndexOf('-')).Trim();
 

                                                alt.Second.Alternative = word.Substring(y + 3, word.Length - (y + 3)).Trim();
                                                check = alt.Second.Alternative.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                                if (check != -1)
                                                    continue;
                                                matchEng = false;
                                                wordSplt = new List<string>(alt.Second.Alternative.Split(' '));
                                                fullNormWord = "";
                                                foreach (var wordPart in wordSplt)
                                                {
                                                    fullNormWord += " " + Lem.CreateParadigmCollectionFromForm(wordPart, false, true)[0].Norm;
                                                }
                                                matchEng = WordFilter(fullNormWord);
                                                if (matchEng)
                                                    continue;


                                                alt.Second.PatternPart = "B";
                                                alt.Second.Pattern = partsPatterns.Substring(partsPatterns.IndexOf('-') + 1).Trim();
                                                alt.Second.PoSstr = secondAltPos.Trim();
                                                alt.Second.TermFullNormForm = fullNormWord.Trim();

                                                var k = FindInListPos(synTermsAr.TermsAr, alt);
                                                if (k == -1)
                                                {
                                                    var newEl = new SynTerm
                                                    {
                                                        Alternatives =
                                                        {
                                                            First = alt.First,
                                                            Second = alt.Second
                                                        },
                                                        Frequency = 1,
                                                        SetToDel = false
                                                    };
                                                    //добавляем алтернативы в список
                                                    //TODO: вычисляем точные координаты альтернатив пары ------> нужно ли? если да дописать
                                                    newEl.Pos.Add(null);
                                                    newEl.TermFragment = curFragment;
                                                    newEl.Pattern = partsPatterns;
                                                    newEl.Kind = KindOfTerm.SynTerm;
                                                    var curRange = new Range(curPos);
                                                    var e = synTermsAr.RootTermsTree.FindRange(curRange);
                                                    var eExtension = synTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                    if (e == null && eExtension == null)
                                                    {
                                                        e = synTermsAr.RootTermsTree.AddRange(curRange);
                                                        //e = synTermsAr.RootTermsTree.FindRange(curRange);
                                                        synTermsAr.TermsAr.Add(newEl);
                                                        e.IndexElement.Add(synTermsAr.TermsAr.Count - 1);
                                                        synTermsAr.TermsAr[synTermsAr.TermsAr.Count - 1].Pos[synTermsAr.TermsAr[synTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                    }
                                                    else
                                                    {
                                                        if (e != null && eExtension == null)
                                                        {
                                                            synTermsAr.TermsAr.Add(newEl);
                                                            e.IndexElement.Add(synTermsAr.TermsAr.Count - 1);
                                                            synTermsAr.TermsAr[synTermsAr.TermsAr.Count - 1].Pos[synTermsAr.TermsAr[synTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var curRange = new Range(curPos);
                                                    var e = synTermsAr.RootTermsTree.FindRange(curRange);
                                                    var eExtension = synTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                    if (e == null && eExtension == null)
                                                    {
                                                        e = synTermsAr.RootTermsTree.AddRange(curRange);
                                                        //e = synTermsAr.RootTermsTree.FindRange(curRange);
                                                        e.IndexElement.Add(k);
                                                        synTermsAr.TermsAr[k].Pos.Add(e);
                                                        synTermsAr.TermsAr[k].Frequency++;
                                                    }
                                                    else
                                                    {
                                                        if (e != null && !e.IndexElement.Contains(k))
                                                            e.IndexElement.Add(k);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var word = xml.Value.Trim();
                                        if (word.Length > 1)
                                        {
                                            var curRange = new Range(curPos);
                                            var e = synTermsAr.RootTermsTree.FindRange(curRange);
                                            if (e != null)
                                            {
                                                var lastDef = curPat.LastIndexOf('-');
                                                var mainPat = curPat.Substring(curPat.IndexOf('-') + 1, lastDef - (curPat.IndexOf('-') + 1)).Trim();
                                                var mainSyn = FindPattern(synTermsAr.TermsAr, e.IndexElement, mainPat);
                                                var patternPart = curPat.Substring(lastDef + 1).Trim();
                                                if (mainSyn != -1)
                                                {
                                                    switch (patternPart)
                                                    {
                                                        case "A":
                                                            {
                                                                synTermsAr.TermsAr[mainSyn].Alternatives.First.NPattern = NormalizeNPattern(word);
                                                                break;
                                                            }
                                                        case "B":
                                                            {
                                                                synTermsAr.TermsAr[mainSyn].Alternatives.Second.NPattern = NormalizeNPattern(word);
                                                                break;
                                                            }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            getFrequency_(synTermsAr);
            return true;

        }

        public void GetXmlSynTermsFromTermsAr(SynTerms synTermsAr, Terms curTermsAr)
        {
            var targetPatternsOutput = TmpPath + FolderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false, Encoding.GetEncoding("Windows-1251"));
            StringBuilder patterns = new StringBuilder();
            List<string> patternsParts = new List<string>();
            List<string> nPatternsParts = new List<string>();
            var patternsBlankPath = ProgrammPath + "\\Patterns\\SYN_TERM_BLANK.txt";
            var blankReader = new StreamReader(patternsBlankPath, Encoding.GetEncoding("Windows-1251"));
            while (true)
            {
                var curLine = blankReader.ReadLine();
                if (curLine == null) break;
                curLine = curLine.Trim();
                if (curLine == "") continue;
                List<string> curLineSplt = curLine.Split('\t').ToList();
                switch (curLineSplt[0])
                {
                    case "PATTERN":
                        {
                            patterns.AppendLine(curLineSplt[1]);
                            break;
                        }
                    case "PART":
                        {
                            patternsParts.Add(curLineSplt[1]);
                            break;
                        }
                    case "NPPART":
                        {
                            nPatternsParts.Add(curLineSplt[1]);
                            break;
                        }
                }
            }
            blankReader.Close();
            for (int i = 0; i < curTermsAr.TermsAr.Count; i++)
            {
                try
                {
                    if (curTermsAr.TermsAr[i].NPattern == null || curTermsAr.TermsAr[i].NPattern.Trim() == "")
                    {
                        throw new ArgumentException("Generated pattern is null! For term " + curTermsAr.TermsAr[i].TermWord);
                    }
                    else
                    {
                        var filteredNewPattern = curTermsAr.TermsAr[i].NPattern.Split(new string[] { "=text>" }, StringSplitOptions.None)[0];
                        Regex patternOptions = new Regex(@"\(\w\d\)");
                        filteredNewPattern = patternOptions.Split(filteredNewPattern)[0].Trim();
                        foreach (var pat in patternsParts)
                        {
                            patterns.AppendLine("SYN-" + IntegerToRoman(i + 1) + "=" + pat.Replace("PASTEHERE", filteredNewPattern));
                            targetPatternsWriter.WriteLine("SYN-" + IntegerToRoman(i + 1));
                        }
                        foreach (var pat in nPatternsParts)
                        {
                            patterns.AppendLine("NPSYN-" + IntegerToRoman(i + 1) + "=" + pat.Replace("PASTEHERE", filteredNewPattern));
                            targetPatternsWriter.WriteLine("SYN-" + IntegerToRoman(i + 1));
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    continue;
                }
            }
            var batOutput = TmpPath + FolderPath + "\\SynTerms.bat";
            var lsplPatterns = TmpPath + FolderPath + "\\SYN_TERM.txt";
            var patternsWriter = new StreamWriter(lsplPatterns, false, Encoding.GetEncoding("Windows-1251"));
            patternsWriter.Write(patterns.ToString());
            patternsWriter.Close();
            //var curPattern = "";
            //var fs = new StreamReader(ProgrammPath + "\\Patterns\\SYN_TERM.txt", Encoding.GetEncoding("Windows-1251"));
            //var lsplPatterns = ProgrammPath + "\\Patterns\\SYN_TERM.txt";
            //while (true)
            //{
            //    curPattern = fs.ReadLine();
            //    if (curPattern == null) break;
            //    if (curPattern != "")
            //    {
            //        curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
            //        if (curPattern.IndexOf("SYN", StringComparison.Ordinal) != -1)
            //        {
            //            var len = curPattern.IndexOf("SYN", StringComparison.Ordinal) + "SYN".Length;
            //            //int k = FindFunctions.findINList(PatternsModel, curPattern, 1);
            //            var k = PatternsModel.FindIndex(item => item.First == curPattern);
            //            if (k == -1)
            //            {
            //                var newP = new Pair<string, string>();
            //                //patternsName = patternsName + " " + curPattern;
            //                targetPatternsWriter.WriteLine(curPattern);
            //                newP.First = curPattern;
            //                newP.Second = curPattern.Substring(len).Trim();
            //                PatternsModel.Add(newP);
            //            }
            //            len = 0;
            //        }
            //    }
            //}
            targetPatternsWriter.Close();
            //--------------------------------
            var lsplExe = ProgrammPath + "\\bin\\lspl-find.exe";
            var lsplOutputText = TmpPath + FolderPath + "\\SynTermsOutputText.xml";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + ProgrammPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + lsplExe
                    + "\" -i \"" + InputFile
                    + "\" -p \"" + lsplPatterns
                    //+ "\" -o \"" + LSPL_output
                    + "\" -t \"" + lsplOutputText
                    //+ "\" -r \"" + LSPL_output_patterns
                    + "\" -s \"" + targetPatternsOutput + "\" ");
            //Close the file
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput) { WindowStyle = ProcessWindowStyle.Hidden };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            //GetSynTermsFromTermsAr(synTermsAr, curTermsAr);
            //---------------------------------
        }
        public bool GetSynTermsFromTermsAr(SynTerms synTermsAr, Terms curTermsAr)
        {
            var curPos = new Point();
            var curPat = "";
            var curFragment = "";
            var lastNodeName = "";
            var lastPos = "";
            using (var xml = XmlReader.Create(TmpPath + FolderPath + "\\SynTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                curPat = xml.Value.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                curPos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                curPos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPos = xml.Value.Trim();
                                        }
                                    }

                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    curFragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    if (curPat.IndexOf("NP", StringComparison.Ordinal) != 0)
                                    {
                                        var word = xml.Value.Trim();
                                        var let = word.ToList().FindIndex(char.IsDigit);
                                        if (let != -1)
                                            continue;
                                        if (word.Length > 1)
                                        {
                                            var alt = new Pair<SynTermAlternative, SynTermAlternative>();
                                            alt.First = new SynTermAlternative();
                                            alt.Second = new SynTermAlternative();
                                            var y = word.IndexOf(" - ", StringComparison.Ordinal);
                                            if (y != -1)
                                            {
                                                var partsPatterns = curPat.Substring(curPat.IndexOf('-') + 1).Trim();
                                                alt.First.Alternative = word.Substring(0, y);
                                                var firstAltWords = alt.First.Alternative.Split(' ').ToList();
                                                var numWords = firstAltWords.Count;
                                                var poSlist = lastPos.Split(' ').ToList();
                                                var firstAltPos = "";
                                                var secondAltPos = "";
                                                var lDelimeter = false;
                                                foreach (var pos in poSlist)
                                                {
                                                    if (pos == "L")
                                                    {
                                                        lDelimeter = true;
                                                        continue;
                                                    }
                                                    if (lDelimeter)
                                                        firstAltPos += " " + pos;
                                                    else
                                                        secondAltPos += " " + pos;
                                                }
                                                var check = alt.First.Alternative.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                                if (check != -1)
                                                    continue;
                                                alt.First.PoSstr = firstAltPos.Trim();
                                                alt.First.PatternPart = "A";
                                                alt.First.Pattern = partsPatterns.Substring(0, partsPatterns.IndexOf('-')).Trim();

                                                alt.Second.Alternative = word.Substring(y + 3, word.Length - (y + 3)).Trim();
                                                check = alt.Second.Alternative.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray());
                                                if (check != -1)
                                                    continue;
                                                alt.Second.PatternPart = "B";
                                                alt.Second.Pattern = partsPatterns.Substring(partsPatterns.IndexOf('-') + 1).Trim();
                                                alt.Second.PoSstr = secondAltPos.Trim();

                                                var k = FindInListPos(synTermsAr.TermsAr, alt);
                                                if (k == -1)
                                                {
                                                    var newEl = new SynTerm
                                                    {
                                                        Alternatives =
                                                        {
                                                            First = alt.First,
                                                            Second = alt.Second
                                                        },
                                                        Frequency = 1,
                                                        SetToDel = false
                                                    };
                                                    //добавляем алтернативы в список
                                                    //TODO: вычисляем точные координаты альтернатив пары ------> нужно ли? если да дописать
                                                    newEl.Pos.Add(null);
                                                    newEl.TermFragment = curFragment;
                                                    newEl.Pattern = partsPatterns;
                                                    newEl.Kind = KindOfTerm.SynTerm;
                                                    var curRange = new Range(curPos);
                                                    var e = synTermsAr.RootTermsTree.FindRange(curRange);
                                                    var eExtension = synTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                    if (e == null && eExtension == null)
                                                    {
                                                        e = synTermsAr.RootTermsTree.AddRange(curRange);
                                                        //e = synTermsAr.RootTermsTree.FindRange(curRange);
                                                        synTermsAr.TermsAr.Add(newEl);
                                                        e.IndexElement.Add(synTermsAr.TermsAr.Count - 1);
                                                        synTermsAr.TermsAr[synTermsAr.TermsAr.Count - 1].Pos[synTermsAr.TermsAr[synTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                    }
                                                    else
                                                    {
                                                        if (e != null && eExtension == null)
                                                        {
                                                            synTermsAr.TermsAr.Add(newEl);
                                                            e.IndexElement.Add(synTermsAr.TermsAr.Count - 1);
                                                            synTermsAr.TermsAr[synTermsAr.TermsAr.Count - 1].Pos[synTermsAr.TermsAr[synTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var curRange = new Range(curPos);
                                                    var e = synTermsAr.RootTermsTree.FindRange(curRange);
                                                    var eExtension = synTermsAr.RootTermsTree.FindRangeExtension(curRange);
                                                    if (e == null && eExtension == null)
                                                    {
                                                        e = synTermsAr.RootTermsTree.AddRange(curRange);
                                                        //e = synTermsAr.RootTermsTree.FindRange(curRange);
                                                        e.IndexElement.Add(k);
                                                        synTermsAr.TermsAr[k].Pos.Add(e);
                                                        synTermsAr.TermsAr[k].Frequency++;
                                                    }
                                                    else
                                                    {
                                                        if (e != null && !e.IndexElement.Contains(k))
                                                            e.IndexElement.Add(k);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var word = xml.Value.Trim();
                                        if (word.Length > 1)
                                        {
                                            var curRange = new Range(curPos);
                                            var e = synTermsAr.RootTermsTree.FindRange(curRange);
                                            if (e != null)
                                            {
                                                var lastDef = curPat.LastIndexOf('-');
                                                var mainPat = curPat.Substring(curPat.IndexOf('-') + 1, lastDef - (curPat.IndexOf('-') + 1)).Trim();
                                                var mainSyn = FindPattern(synTermsAr.TermsAr, e.IndexElement, mainPat);
                                                var patternPart = curPat.Substring(lastDef + 1).Trim();
                                                if (mainSyn != -1)
                                                {
                                                    switch (patternPart)
                                                    {
                                                        case "A":
                                                            {
                                                                synTermsAr.TermsAr[mainSyn].Alternatives.First.NPattern = NormalizeNPattern(word);
                                                                break;
                                                            }
                                                        case "B":
                                                            {
                                                                synTermsAr.TermsAr[mainSyn].Alternatives.Second.NPattern = NormalizeNPattern(word);
                                                                break;
                                                            }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            getFrequency_(synTermsAr);
            return true;

        }

        // Check terms in headers
        // TODO заменить на Generic Function

        public void CheckTermsInHeaders(SynTerms synTermsAr)
        {
            var tmpPath = Path.GetTempPath();
            const string folderPath = "TermsProcessingF";
            var headersOutput = tmpPath + folderPath + "\\Headers.txt";
            var headersWriter = new StreamWriter(headersOutput, false, Encoding.GetEncoding("Windows-1251"));
            for (var i = 0; i < Headers.Count; i++)
            {
                headersWriter.WriteLine(Headers[i]);
            }
            headersWriter.Close();
            GetXmlTermsHeaders(AuthTermsAr, "_auth_terms", headersOutput);
            GetXmlTermsHeaders(DictTermsAr, "_dict_terms", headersOutput);
            GetXmlCombTermsHeaders(CombTermsAr, headersOutput);
            GetXmlNonDictTermsArHeaders(NonDictTermsAr, headersOutput);
            GetXmlSynTermsHeaders(synTermsAr, headersOutput);
        }
        private void GetXmlTermsHeaders(Terms curTermsAr, string type, string InputFile)
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
                         + "\" -i \"" + InputFile
                         + "\" -p \"" + lsplPatterns
                         + "\" -t \"" + lsplOutputText
                         + "\" -s \"" + targetPatternsOutput + "\" ");
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput) { WindowStyle = ProcessWindowStyle.Hidden };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            GetTermsHeaders(curTermsAr, type);
        }
        private static void GetTermsHeaders(Terms curTermsAr, string type)
        {
            var tmpPath = Path.GetTempPath();
            const string folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var lastNodeName = "";
            var newMatch = false;
            var curItemIndex = -1;
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
                                        curItemIndex = RomanToInteger(partPattern[1]) - 1;
                                        if (curItemIndex != -1)
                                            if (newMatch)
                                            {
                                                newMatch = false;
                                                curTermsAr.TermsAr[curItemIndex].inHeader = true;
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
        }

        private void GetXmlCombTermsHeaders(CombTerms combTermsAr, string InputFile)
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
                         + "\" -i \"" + InputFile
                         + "\" -p \"" + lsplPatterns
                         + "\" -t \"" + lsplOutputText
                         + "\" -s \"" + targetPatternsOutput + "\" ");
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput) { WindowStyle = ProcessWindowStyle.Hidden };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            GetCombTermsHeaders(combTermsAr);
        }
        private static void GetCombTermsHeaders(CombTerms combTermsAr)
        {
            var tmpPath = Path.GetTempPath();
            const string folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var lastNodeName = "";
            var newMatch = false;
            var processedPos = new List<Point>();
            var curItemIndex = -1;
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

                                            curItemIndex = RomanToInteger(partPattern[1]) - 1;
                                            if (curItemIndex != -1)
                                                if (newMatch)
                                                {
                                                    newMatch = false;
                                                    combTermsAr.TermsAr[curItemIndex].inHeader = true;
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
        }

        private void GetXmlSynTermsHeaders(SynTerms synTermsAr, string InputFile)
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
                         + "\" -i \"" + InputFile
                         + "\" -p \"" + lsplPatterns
                         + "\" -t \"" + lsplOutputText
                         + "\" -s \"" + targetPatternsOutput + "\" ");
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput) { WindowStyle = ProcessWindowStyle.Hidden };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            GetSynTermsHeaders(synTermsAr);
        }
        private static void GetSynTermsHeaders(SynTerms synTermsAr)
        {
            var tmpPath = Path.GetTempPath();
            const string folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var lastNodeName = "";
            var newMatch = false;
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
                                                                synTermsAr.TermsAr[itemIndex].Alternatives.First.inHeader = true;
                                                                break;
                                                            }
                                                        case "second":
                                                        {
                                                            synTermsAr.TermsAr[itemIndex].Alternatives.Second.inHeader =
                                                                true;
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
                                        break;
                                }
                                break;
                            }
                    }
            }
        }

        private void GetXmlNonDictTermsArHeaders(NonDictTerms nonDictTermsAr, string InputFile)
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
                         + "\" -i \"" + InputFile
                         + "\" -p \"" + lsplPatterns
                         + "\" -t \"" + lsplOutputText
                         + "\" -s \"" + targetPatternsOutput + "\" ");
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput) { WindowStyle = ProcessWindowStyle.Hidden };
            var process = Process.Start(startInfo);
            process?.WaitForExit();
            GetNonDictTermsHeaders(nonDictTermsAr);
        }
        private static void GetNonDictTermsHeaders(NonDictTerms nonDictTermsAr)
        {
            var tmpPath = Path.GetTempPath();
            const string folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var lastNodeName = "";
            var newMatch = false;
            var processedPos = new List<Point>();
            var curItemIndex = -1;
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
                                            curItemIndex = RomanToInteger(partPattern[1]) - 1;
                                            if (curItemIndex != -1)
                                                if (newMatch)
                                                {
                                                    nonDictTermsAr.TermsAr[curItemIndex].inHeader = true;
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
        }


        // Постраничное извлечение текста из Docx документа -------------------------------------
        private List<string> ReadTextFromDocxByPage(string pathToFile)
        {
            var pages = new List<string>();
            object path = pathToFile;
            var word = new Microsoft.Office.Interop.Word.Application();
            object miss = Missing.Value;
            object readOnly = false;
            var docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
            var totaltext = "";
            object obj = "\\page";
            var numberOfPages = docs.ComputeStatistics(WdStatistic.wdStatisticPages, false);
            var rng = word.Selection.Bookmarks.get_Item(ref obj).Range;
            for (var i = 1; i <= numberOfPages; i++)
            {
                pages.Add(rng.Text);
                word.Selection.GoToNext(WdGoToItem.wdGoToPage);
                rng = word.Selection.Bookmarks.get_Item(ref obj).Range;
            }
            docs.Close();
            word.Quit();
            pages = pages.GetRange(StartPage - 1, EndPage - StartPage + 1);
            return pages;
        }

        public int GetPageNumberByPositon(Range pos)
        {
            int pageNum = StartPage;
            int curStart = 0;
            for (int i = 0; i < Pages.Count; i++)
            {
                if (Pages[i] == null)
                    Pages[i] = "";

                if (pos.Inf.X >= curStart && pos.Inf.Y <= curStart + Pages[i].Length + 1)
                {
                    pageNum = pageNum + i;
                    break;
                }
                else
                {
                    if (Pages[i] != null)
                        curStart = curStart + Pages[i].Length;
                }
            }
            return pageNum;
        }
        //---------------------------------------------------------------------------------------
    }
}


////string cmdCommand = " -i \"" + file + "\" -p  -o  Definition SeparateDefinition";


////ofstream out_bat(outFile);
////if (!out_bat) {
////    cout << "Cannot open file.\n";
////    return;
////}

//out_bat<<\n"<<cmdCommand;
//out_bat.close();
//cmdCommand="\""+outFile+"\"";
////CreateWaitChildProcess(cmdCommand);
