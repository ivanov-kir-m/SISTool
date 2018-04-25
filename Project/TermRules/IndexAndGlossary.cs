using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace TermRules
{
    //public class IndexBlock
    //{
    //    public IndexBlock(string letter_)
    //    {
    //        letter = letter_;
    //        terms = new List<IndexItem>();
    //    }
    //    public string letter { get; set; }
    //    public List<IndexItem> terms { get; set; }
    //}
    public class IndexItem
    {
        public string Term { get; set; }
        public List<string> SupportTerms { get; set; }
        public List<int> Pages { get; set; }
        public IndexItem(string term)
        {
            Term = term;
            SupportTerms = new List<string>();
        }
    }
    public class PartOfIndexItem
    {
        public string NPattern { get; set; }
        public int Frequency { get; set; }
        public string PartWord { get; set; }
        public string PartNormWord { get; set; }
        public Point CurPos { get; set; }

    }

    public class PreparatoryIndexItem
    {
        public int TermsArIndex { get; set; }
        public int StartPos { get; set; }
        public List<PartOfIndexItem> PosibleParts { get; set; }
        public int Frequency { get; set; }
    }

    public class GlossaryItem
    {
        public string Term { get; set; }
        public string Definition { get; set; }
        public GlossaryItem(string term, string description)
        {
            Term = term;
            Definition = description;
        }
    }
    public class IndexAndGlossary
    {
        private Rules _rules;
        private Terms _mainTermsAr;
        public IndexAndGlossary(string inputfile, DictionaryF dict, int startPage, int endPage, bool clearSupportLists)
        {
            _rules = new Rules(inputfile, dict, startPage, endPage, clearSupportLists);
            _mainTermsAr = _rules.ApplyRules();
        }
        public List<IndexItem> GetIndex()
        {
            var index = new List<IndexItem>();
            foreach (var term in _mainTermsAr.TermsAr)
            {
                var separatedTerm = new List<string>();
                separatedTerm = GetSeparatedTerm(term.TermWord, term.PoSstr);
                var curItem = index.Find(r => r.Term == separatedTerm[0]);
                if (curItem == null)
                {
                    curItem = new IndexItem(separatedTerm[0]);
                    for (var i = 1; i < separatedTerm.Count; i++)
                        curItem.SupportTerms.Add(separatedTerm[i]);
                    index.Add(curItem);
                }
                else
                {
                    for (var i = 1; i < separatedTerm.Count; i++)
                    {
                        var supportTerm = curItem.SupportTerms.Find(r => r == separatedTerm[i]);
                        if (supportTerm == null || supportTerm == "")
                        {
                            curItem.SupportTerms.Add(separatedTerm[i]);
                        }
                    }
                }
                //IndexItem item = new IndexItem(term.TermWord, term.TermFragment);
                //index.Add(item);
            }
            index.Sort(delegate (IndexItem item1, IndexItem item2) { return item1.Term.CompareTo(item2.Term); });
            foreach (var item in index)
            {
                if (item.SupportTerms != null)
                    item.SupportTerms.Sort(delegate (string term1, string term2) { return term1.CompareTo(term2); });
            }
            return index;
        }

        public Terms GetMainTermsAr()
        {
            return _mainTermsAr;
        }
        public void GetXmlIndexTermsPartsPatterns(string inputFile)
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
            var lsplOutput = tmpPath + folderPath + "\\IndexTermsPatternsOutput.xml";
            var lsplOutputText = tmpPath + folderPath + "\\IndexTermsOutputPatternsText.xml";
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
        public bool GetIndexTermsPartsPatterns(List<PreparatoryIndexItem> index, List<Term> termsAr)
        {
            var tmpPath = Path.GetTempPath();
            var programmPath = Application.StartupPath;
            var folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var curFragment = "";
            var lastNodeName = "";
            var lastPos = "";
            using (var xml = XmlReader.Create(tmpPath + folderPath + "\\IndexTermsOutputPatternsText.xml"))
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
                                        var k = index.FindIndex(item => (curPos.X >= item.StartPos &&
                                        curPos.Y <= item.StartPos + termsAr[item.TermsArIndex].TermWord.Length + 1));
                                        if (k != -1)
                                        {
                                            var l = index[k].PosibleParts.FindIndex(item => (item.CurPos.X == curPos.X && item.CurPos.Y == curPos.Y));
                                            if (l == -1)
                                            {
                                                var newPart = new PartOfIndexItem();
                                                newPart.NPattern = AuxiliaryFunctions.NormalizeNPattern(word.Trim());
                                                newPart.Frequency = 0;
                                                newPart.CurPos = curPos;
                                                index[k].PosibleParts.Add(newPart);

                                            }
                                        }
                                    }
                                    if (curPat == "PARTWORD")
                                    {
                                        foreach (var item in index)
                                        {
                                            var k = item.PosibleParts.FindIndex(part => part.CurPos.X == curPos.X && part.CurPos.Y == curPos.Y);
                                            if (k != -1)
                                            {
                                                var word = xml.Value.Trim();
                                                item.PosibleParts[k].PartWord = word;
                                                break;
                                            }
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
        public void GetXmlIndexTermsParts(string inputFile, List<PreparatoryIndexItem> index, List<Term> termsAr)
        {
            var tmpPath = Path.GetTempPath();
            var programmPath = Application.StartupPath;
            var folderPath = "TermsProcessingF";
            var batOutput = tmpPath + folderPath + "\\IndexItemsParts.bat";
            var targetPatternsOutput = tmpPath + folderPath + "\\targets";
            var targetPatternsWriter = new StreamWriter(targetPatternsOutput, false, Encoding.GetEncoding("Windows-1251"));
            var pAtternsStreamWriter = new StreamWriter(programmPath + "\\Patterns\\IndexItemsPartsPatterns.txt", false, Encoding.GetEncoding("Windows-1251"));
            var curPattern = "";
            for (var i = 0; i < index.Count; i++)
            {
                if (termsAr[index[i].TermsArIndex].NPattern != null && termsAr[index[i].TermsArIndex].NPattern != "")
                {
                    pAtternsStreamWriter.WriteLine("Part-" + AuxiliaryFunctions.IntegerToRoman(i + 1) + "-full = " + termsAr[index[i].TermsArIndex].NPattern);
                    targetPatternsWriter.WriteLine("Part-" + AuxiliaryFunctions.IntegerToRoman(i + 1) + "-full");
                    for (var j = 0; j < index[i].PosibleParts.Count; j++)
                    {
                        pAtternsStreamWriter.WriteLine("Part-" + AuxiliaryFunctions.IntegerToRoman(i + 1) + "-" + AuxiliaryFunctions.IntegerToRoman(j + 1) + " = " + index[i].PosibleParts[j].NPattern);
                        if (index[i].PosibleParts[j].NPattern.IndexOf("AP1") != -1)
                            Console.Write("Error!");
                        targetPatternsWriter.WriteLine("Part-" + AuxiliaryFunctions.IntegerToRoman(i + 1) + "-" + AuxiliaryFunctions.IntegerToRoman(j + 1));
                    }
                }
            }
            pAtternsStreamWriter.Close();
            targetPatternsWriter.Close();
            var lsplExe = programmPath + "\\bin\\lspl-find.exe";
            var lsplPatterns = programmPath + "\\Patterns\\IndexItemsPartsPatterns.txt";
            var lsplOutput = tmpPath + folderPath + "\\IndexItemsPartsOutput.xml";
            var lsplOutputText = tmpPath + folderPath + "\\IndexItemsPartsOutputText.xml";
            //string LSPL_output_patterns = tmpPath + folderPath + "\\AuthTermsOutputPatterns.xml";
            var sw = new StreamWriter(batOutput, false, Encoding.GetEncoding("cp866"));
            //bin\lspl-find -i example\text.txt -p example\patterns.txt -o example\output.xml -t example\output_text.xml -r example\output_patterns.xml -s example\targets.txt
            sw.WriteLine("\"" + lsplExe
                + "\" -i \"" + inputFile
                + "\" -p \"" + lsplPatterns
                //+ "\" -o \"" + LSPL_output
                + "\" -t \"" + lsplOutputText
                //+ "\" -r \"" + LSPL_output_patterns
                + "\" -s \"" + targetPatternsOutput + "\" ");
            sw.Close();
            var startInfo = new ProcessStartInfo(batOutput);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(startInfo).WaitForExit();
        }
        public bool GetIndexTermsParts(List<PreparatoryIndexItem> index, List<Term> termsAr)
        {
            var tmpPath = Path.GetTempPath();
            var programmPath = Application.StartupPath;
            var folderPath = "TermsProcessingF";
            var curPos = new Point();
            var curPat = "";
            var curFragment = "";
            var lastNodeName = "";
            var lastPos = "";
            var newMatch = false;
            using (var xml = XmlReader.Create(tmpPath + folderPath + "\\IndexItemsPartsOutputText.xml"))
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
                                    newMatch = true;
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
                                    var partPattern = curPat.Split('-').ToList();
                                    if (partPattern[2] == "full")
                                    {
                                        var itemIndex = AuxiliaryFunctions.RomanToInteger(partPattern[1])-1;
                                        if (itemIndex != -1)
                                        {
                                            if (newMatch)
                                            {
                                                newMatch = false;
                                                index[itemIndex].Frequency++;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        var word = xml.Value.Trim();
                                        var itemIndex = AuxiliaryFunctions.RomanToInteger(partPattern[1])-1;
                                        var partIndex = AuxiliaryFunctions.RomanToInteger(partPattern[2])-1;
                                        if (itemIndex != -1 && partIndex != -1)
                                        {
                                            if (newMatch)
                                            {
                                                newMatch = false;
                                                index[itemIndex].PosibleParts[partIndex].Frequency++;
                                                index[itemIndex].PosibleParts[partIndex].PartNormWord = word;
                                            }
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
        private int FindMaxFrequency(List<PartOfIndexItem> posibleParts)
        {
            var maxInd = 0;
            var max = -10;
            for(var i =0; i<posibleParts.Count; i++)
            {
                if (posibleParts[i].Frequency > max)
                {
                    max = posibleParts[i].Frequency;
                    maxInd = i;
                }
            }
            return maxInd;
        }

        public List<IndexItem> GetIndexAr()
        {
            var index = new List<IndexItem>();
            var prepareIndex = new List<PreparatoryIndexItem>();
            var tmpPath = Path.GetTempPath();
            var folderPath = "TermsProcessingF";
            var indexInput = tmpPath + folderPath + "\\MainTermsAr.txt";
            var testInput = tmpPath + folderPath + "\\MainTermsArTest.txt";
            var serializeWriter = new StreamWriter(indexInput, false, Encoding.GetEncoding("Windows-1251"));
            var testWriter = new StreamWriter(testInput, false, Encoding.GetEncoding("Windows-1251"));
            var curStartPos = 0;
            for (var i = 0; i< _mainTermsAr.TermsAr.Count; i++)
            {
                var newItem = new PreparatoryIndexItem();
                newItem.TermsArIndex = i;
                newItem.StartPos = curStartPos;
                newItem.PosibleParts = new List<PartOfIndexItem>();
                prepareIndex.Add(newItem);
                serializeWriter.WriteLine(_mainTermsAr.TermsAr[i].TermWord);
                curStartPos = curStartPos + _mainTermsAr.TermsAr[i].TermWord.Length + 1;

                testWriter.WriteLine(_mainTermsAr.TermsAr[i].TermWord);
                testWriter.WriteLine(_mainTermsAr.TermsAr[i].NPattern);
                testWriter.WriteLine(_mainTermsAr.TermsAr[i].PoSstr);
                testWriter.WriteLine(_mainTermsAr.TermsAr[i].Frequency);
                testWriter.WriteLine(_mainTermsAr.TermsAr[i].TermFragment);
            }
            testWriter.Close();
            serializeWriter.Close();
            GetXmlIndexTermsPartsPatterns(indexInput);
            GetIndexTermsPartsPatterns(prepareIndex, _mainTermsAr.TermsAr);
            GetXmlIndexTermsParts(indexInput, prepareIndex, _mainTermsAr.TermsAr);
            GetIndexTermsParts(prepareIndex, _mainTermsAr.TermsAr);
            for (var i = 0; i < prepareIndex.Count; i++)
            {
                var maxFreqIndex = FindMaxFrequency(prepareIndex[i].PosibleParts);
                if (maxFreqIndex > prepareIndex[i].Frequency)
                {
                    var curItem = index.Find(r => r.Term == prepareIndex[i].PosibleParts[maxFreqIndex].PartNormWord);
                    if (curItem == null)
                    {
                        curItem = new IndexItem(prepareIndex[i].PosibleParts[maxFreqIndex].PartNormWord);
                    }

                    var fullTermWord = _mainTermsAr.TermsAr[prepareIndex[i].TermsArIndex].TermWord;
                    var shortTermWord = fullTermWord.Replace(prepareIndex[i].PosibleParts[maxFreqIndex].PartWord.Trim(), ",");
                    if (shortTermWord.Length != fullTermWord.Length)
                    {
                        List<string> splittedShortTermWord = new List<string>(shortTermWord.Split());                  
                        var match = true;
                        while (match)
                        {
                            match = false;
                            if (splittedShortTermWord.Count>0 && splittedShortTermWord[0] == ",")
                            {
                                splittedShortTermWord.RemoveAt(0);
                                match = true;
                            }
                            else if (splittedShortTermWord.Count > 0 && splittedShortTermWord[splittedShortTermWord.Count - 1] == ",")
                            {
                                splittedShortTermWord.RemoveAt(0);
                                match = true;
                            }
                        }
                        shortTermWord = string.Join(" ", splittedShortTermWord);
                        shortTermWord = shortTermWord.Trim();
                        var curLen = shortTermWord.Length;
                        shortTermWord = shortTermWord.Replace(", ,", ",");
                        while (shortTermWord.Length != curLen)
                        {
                            curLen = shortTermWord.Length;
                            shortTermWord = shortTermWord.Replace(", ,", ",");
                        }
                        var k = curItem.SupportTerms.FindIndex(supterm => supterm == shortTermWord);
                        if (k == -1)
                            curItem.SupportTerms.Add(shortTermWord);
                    }
                    else
                    {
                        var splitedTerm = _mainTermsAr.TermsAr[prepareIndex[i].TermsArIndex].TermWord.Split(' ').ToList();
                        splitedTerm.Reverse();
                        var splitedMaxFrqPart = prepareIndex[i].PosibleParts[maxFreqIndex].PartWord.Split(' ').ToList();
                        foreach (var part in splitedMaxFrqPart)
                        {
                            for (var j = 0; j < splitedTerm.Count; j++)
                            {
                                if (splitedTerm[j] == part)
                                {
                                    if (j == 0 || j == splitedTerm.Count - 1)
                                        splitedTerm.RemoveAt(j);
                                    else
                                        splitedTerm[j] = ",";
                                    break;
                                }
                            }
                        }
                        //TODO: Добавить запятные в supportTerm DONE need test
                        splitedTerm.Reverse();
                        var supportTerm = string.Join(" ", splitedTerm);
                        var curLen = supportTerm.Length;
                        supportTerm = supportTerm.Replace(", ,", ",");
                        while (supportTerm.Length != curLen)
                        {
                            curLen = supportTerm.Length;
                            supportTerm = supportTerm.Replace(", ,", ",");
                        }
                        supportTerm = supportTerm.Trim();
                        var k = curItem.SupportTerms.FindIndex(supterm => supterm == shortTermWord);
                        if (k == -1 && !string.IsNullOrEmpty(supportTerm))
                            curItem.SupportTerms.Add(supportTerm);
                    }
                }
                else
                {
                    var curItem = index.Find(r => r.Term == _mainTermsAr.TermsAr[prepareIndex[i].TermsArIndex].TermWord);
                    if (curItem == null)
                    {
                        curItem = new IndexItem(_mainTermsAr.TermsAr[prepareIndex[i].TermsArIndex].TermWord);
                        index.Add(curItem);
                    }
                    
                }
            }
            index.Sort(delegate (IndexItem item1, IndexItem item2) { return item1.Term.CompareTo(item2.Term); });
            foreach (var item in index)
            {
                if (item.SupportTerms != null)
                    item.SupportTerms.Sort(delegate (string term1, string term2) { return term1.CompareTo(term2); });
            }

            for (var i = 0; i < index.Count; i++)
            {
                var added = false;
                var splittedTerm = index[i].Term.Split(' ').ToList(); 
                for (var j = i+1; j < index.Count; j++)
                {
                    var splittedSupTerm = index[j].Term.Split(' ').ToList();
                    if (splittedTerm[0] == splittedSupTerm[0] && index[j].SupportTerms.Count == 0)
                    {                        
                        var newSupTerm = "";
                        for (var n = 1; n < splittedSupTerm.Count; n++)
                            newSupTerm += " " + splittedSupTerm[n];
                        newSupTerm = newSupTerm.Trim();
                        var k = index[i].SupportTerms.FindIndex(item => item == newSupTerm);
                        if (k == -1 && newSupTerm != null && newSupTerm != "")
                            index[i].SupportTerms.Add(newSupTerm);
                        index.RemoveAt(j);
                        j--;
                        added = true;
                    }
                    else
                        break;
                }
                if (added)
                {
                    var newSupTerm = "";
                    for (var n = 1; n < splittedTerm.Count; n++)
                        newSupTerm += " " + splittedTerm[n];
                    newSupTerm = newSupTerm.Trim();
                    index[i].Term = splittedTerm[0];
                    if (newSupTerm != null && newSupTerm != "")
                        index[i].SupportTerms.Insert(0, newSupTerm);
                }
            }

            //----------------------------------------------------------------------------
            var indexWriter = new StreamWriter("index_backup.txt", false, Encoding.GetEncoding("Windows-1251"));
            var curLetterBlock = index[0].Term[0].ToString().ToUpper();
            indexWriter.WriteLine("==" + curLetterBlock + "==");
            foreach (var item in index)
            {
                if (item.Term[0].ToString().ToUpper().CompareTo(curLetterBlock) != 0)
                {
                    curLetterBlock = item.Term[0].ToString().ToUpper();
                    indexWriter.WriteLine("");
                    indexWriter.WriteLine("=="+curLetterBlock+"==");
                }
                indexWriter.WriteLine(item.Term);
                foreach(var supterm in item.SupportTerms)
                {
                    indexWriter.WriteLine("------ " + supterm);
                }
            }
            indexWriter.Close();
            //----------------------------------------------------------------------------
            return index;
        }
        
        public List<GlossaryItem> GetGlossary()
        {
            var glossary = new List<GlossaryItem>();
            foreach (var term in _mainTermsAr.TermsAr)
            {
                if (term.Kind == KindOfTerm.AuthTerm)
                {
                    var item = new GlossaryItem(term.TermWord, term.TermFragment);
                    glossary.Add(item);
                }
            }
            return glossary;
        }        
        public List<string> GetSeparatedTerm(string term, string partsOfSpeech)
        {
            var sepTerm = new List<string>();
            partsOfSpeech = partsOfSpeech.Replace("Pa", "A");
            partsOfSpeech = partsOfSpeech.Replace("L", "N");
            switch (partsOfSpeech)
            {
                case "N":
                    {
                        sepTerm.Add(term.Trim());
                        break;
                    }
                case "N N":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        break;
                    }
                case "A N":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    }
                case "A A N":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        var tmpStr = sepTerm[1];
                        sepTerm[2] = tmpStr + " " + sepTerm[2];
                        sepTerm.RemoveAt(1);
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    } 
                case "A N N":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        var tmpStr = sepTerm[1];
                        sepTerm[2] = tmpStr + " " + sepTerm[2];
                        sepTerm.RemoveAt(1);
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    }            
                case "N A N":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        var tmpStr = sepTerm[1];
                        sepTerm[2] = tmpStr + " " + sepTerm[2];
                        sepTerm.RemoveAt(1);
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    }
                case "N N N":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        var tmpStr = sepTerm[1];
                        sepTerm[2] = tmpStr + " " + sepTerm[2];
                        sepTerm.RemoveAt(1);
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    }
                case "N N N N":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        var tmpStr = sepTerm[1];
                        sepTerm[2] = tmpStr + " " + sepTerm[2];
                        sepTerm.RemoveAt(1);
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    }
            }

            return sepTerm;
        }

    }
}
