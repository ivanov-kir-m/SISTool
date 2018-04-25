using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TermRules
{
    public static class AuxiliaryFunctions
    {
        public static List<string> ConstantPatterns = new List<string> {"AP = A1 (A1) =text> A1 | Pa1 (Pa1) =text> Pa1", "NE = \"не\" =text> \"не\""};
        public static void getFrequency_(Terms mr)
        {
            var size = mr.TermsAr.Count;
            var change = true;
            while (change)
            {
                change = false;
                for (var i = 0; i < mr.TermsAr.Count; i++)
                {
                    for (var j = 0; j < mr.TermsAr[i].Pos.Count; j++)
                    {
                        if (mr?.TermsAr != null && mr.TermsAr.Count > i && mr.TermsAr[i].Pos != null &&
                            mr.TermsAr[i].Pos.Count > j && mr.TermsAr[i].Pos[j]?.Range?.Inf != null)
                        {
                            var range = new Range(mr.TermsAr[i].Pos[j].Range.Inf);
                            var e = mr.RootTermsTree.FindRangeExtension(range);
                            if (e != null)
                            {
                                var curTreeEl = mr.TermsAr[i].Pos[j];
                                List<int> toDel = new List<int>();
                                foreach (var index in curTreeEl.IndexElement)
                                {
                                    if (index < mr.TermsAr.Count)
                                    {
                                        var indDelPos = mr.TermsAr[index].Pos.FindIndex(item => item == curTreeEl);
                                        if (indDelPos != -1)
                                            mr.TermsAr[index].Pos.RemoveAt(indDelPos);
                                    }
                                    else
                                    { toDel.Add(index);}
                                }
                                foreach (var index in toDel)
                                {
                                    curTreeEl.IndexElement.Remove(index);
                                }
                                mr.RootTermsTree.DeleteRange(range);
                                int u =
                                    mr.TermsAr[i].Pos.FindIndex(
                                        item => item.Range.Inf.X == range.Inf.X && item.Range.Inf.Y == range.Inf.Y);
                                if (u != -1)
                                    mr.TermsAr[i].Pos.RemoveAt(u);
                                j--;
                            }
                        }
                    }
                    if (mr.TermsAr[i].Pos.Count == 0)
                    {
                        mr.TermsAr.RemoveAt(i);
                        ChangeIdexesInTree(mr, i);
                        i--;
                    }
                    else
                    {
                        mr.TermsAr[i].Frequency = mr.TermsAr[i].Pos.Count;
                    }
                }
                if (size != mr.TermsAr.Count)
                {
                    change = true;
                    size = mr.TermsAr.Count;
                }
            }
        }
        public static void getFrequency_(NonDictTerms mr)
        {
            var size = mr.TermsAr.Count;
            var change = true;
            while (change)
            {
                change = false;
                for (var i = 0; i < mr.TermsAr.Count; i++)
                {
                    for (var j = 0; j < mr.TermsAr[i].Pos.Count; j++)
                    {
                        if (mr?.TermsAr != null && mr.TermsAr.Count > i && mr.TermsAr[i].Pos != null &&
                            mr.TermsAr[i].Pos.Count > j && mr.TermsAr[i].Pos[j]?.Range?.Inf != null)
                        {
                            var range = new Range(mr.TermsAr[i].Pos[j].Range.Inf);
                            var e = mr.RootTermsTree.FindRangeExtension(range);
                            if (e != null)
                            {
                                var curTreeEl = mr.TermsAr[i].Pos[j];
                                List<int> toDel = new List<int>();
                                foreach (var k in curTreeEl.IndexElement)
                                {
                                    if (k < mr.TermsAr.Count)
                                    {
                                        var indDelPos = mr.TermsAr[k].Pos.FindIndex(item => item == curTreeEl);
                                    if (indDelPos != -1)
                                        mr.TermsAr[k].Pos.RemoveAt(indDelPos);
                                    }
                                    else
                                    { toDel.Add(k); }
                                }
                                foreach (var index in toDel)
                                {
                                    curTreeEl.IndexElement.Remove(index);
                                }
                                //for (var k = 0; k < curTreeEl.IndexElement.Count; k++)
                                //{
                                //    var indDelPos = mr.TermsAr[curTreeEl.IndexElement[k]].Pos.FindIndex(item => item == curTreeEl);
                                //    if (indDelPos != -1)
                                //        mr.TermsAr[curTreeEl.IndexElement[k]].Pos.RemoveAt(indDelPos);
                                //}
                                mr.RootTermsTree.DeleteRange(range);
                                int u =
                                    mr.TermsAr[i].Pos.FindIndex(
                                        item => item.Range.Inf.X == range.Inf.X && item.Range.Inf.Y == range.Inf.Y);
                                if (u != -1)
                                    mr.TermsAr[i].Pos.RemoveAt(u);
                                j--;
                            }
                        }
                    }
                    if (mr.TermsAr[i].Pos.Count == 0)
                    {
                        mr.TermsAr.RemoveAt(i);
                        ChangeIdexesInTree(mr, i);
                        i--;
                    }
                    else
                    {
                        mr.TermsAr[i].Frequency = mr.TermsAr[i].Pos.Count;
                    }
                }
                if (size != mr.TermsAr.Count)
                {
                    change = true;
                    size = mr.TermsAr.Count;
                }
            }
        }
        public static void getFrequency_(SynTerms mr)
        {
            var size = mr.TermsAr.Count;
            var change = true;
            while (change)
            {
                change = false;
                for (var i = 0; i < mr.TermsAr.Count; i++)
                {
                    for (var j = 0; j < mr.TermsAr[i].Pos.Count; j++)
                    {
                        if (mr?.TermsAr != null && mr.TermsAr.Count > i && mr.TermsAr[i].Pos != null &&
                            mr.TermsAr[i].Pos.Count > j && mr.TermsAr[i].Pos[j]?.Range?.Inf != null)
                        {
                            var range = new Range(mr.TermsAr[i].Pos[j].Range.Inf);
                            var e = mr.RootTermsTree.FindRangeExtension(range);
                            if (e != null)
                            {
                                var curTreeEl = mr.TermsAr[i].Pos[j];
                                List<int> toDel = new List<int>();
                                foreach (var k in curTreeEl.IndexElement)
                                {
                                    if (k < mr.TermsAr.Count)
                                    {
                                        var indDelPos = mr.TermsAr[k].Pos.FindIndex(item => item == curTreeEl);
                                    if (indDelPos != -1)
                                        mr.TermsAr[k].Pos.RemoveAt(indDelPos);
                                    }
                                    else
                                    { toDel.Add(k); }
                                }
                                foreach (var index in toDel)
                                {
                                    curTreeEl.IndexElement.Remove(index);
                                }
                                //for (var k = 0; k < curTreeEl.IndexElement.Count; k++)
                                //{
                                //    var indDelPos = mr.TermsAr[curTreeEl.IndexElement[k]].Pos.FindIndex(item => item == curTreeEl);
                                //    if (indDelPos != -1)
                                //        mr.TermsAr[curTreeEl.IndexElement[k]].Pos.RemoveAt(indDelPos);
                                //}
                                mr.RootTermsTree.DeleteRange(range);
                                int u =
                                    mr.TermsAr[i].Pos.FindIndex(
                                        item => item.Range.Inf.X == range.Inf.X && item.Range.Inf.Y == range.Inf.Y);
                                if (u != -1)
                                    mr.TermsAr[i].Pos.RemoveAt(u);
                                j--;
                            }
                        }
                    }
                    if (mr.TermsAr[i].Pos.Count == 0)
                    {
                        mr.TermsAr.RemoveAt(i);
                        ChangeIdexesInTree(mr, i);
                        i--;
                    }
                    else
                    {
                        mr.TermsAr[i].Frequency = mr.TermsAr[i].Pos.Count;
                    }
                }
                if (size != mr.TermsAr.Count)
                {
                    change = true;
                    size = mr.TermsAr.Count;
                }
            }
        }
        public static void DelElementsWhichSetToDel(Terms mr)
        {
            var sizeMr = mr.TermsAr.Count;
            for (var i = 0; i < sizeMr; i++)
            {
                if (mr.TermsAr[i].SetToDel)
                {
                    for (var j=0; j<mr.TermsAr[i].Pos.Count; j++)
                    {
                        if (mr.TermsAr[i].Pos[j].IndexElement.Contains(i))
                            mr.TermsAr[i].Pos[j].IndexElement.Remove(i);
                        //var indToDel = mr.TermsAr[i].Pos[j].IndexElement.FindIndex(item => item == i);
                        //if (indToDel != -1)
                        //{
                        //    mr.TermsAr[i].Pos[j].IndexElement.RemoveAt(indToDel);
                        //}
                        if (mr.TermsAr[i].Pos[j].IndexElement.Count == 0)
                        {
                            var delRange = new Range(mr.TermsAr[i].Pos[j].Range.Inf);
                            mr.RootTermsTree.DeleteRange(delRange);
                        }
                    }
                    mr.TermsAr.RemoveAt(i);
                    ChangeIdexesInTree(mr, i);
                    sizeMr = mr.TermsAr.Count;
                    i--;
                }
            }
        }
        public static void DelElementsWhichSetToDel(SynTerms mr)
        {
            var sizeMr = mr.TermsAr.Count;
            for (var i = 0; i < sizeMr; i++)
            {
                if (mr.TermsAr[i].SetToDel)
                {
                    for (var j = 0; j < mr.TermsAr[i].Pos.Count; j++)
                    {
                        if (mr.TermsAr[i].Pos[j].IndexElement.Contains(i))
                            mr.TermsAr[i].Pos[j].IndexElement.Remove(i);
                        //var indToDel = mr.TermsAr[i].Pos[j].IndexElement.FindIndex(item => item == i);
                        //if (indToDel != -1)
                        //{
                        //    mr.TermsAr[i].Pos[j].IndexElement.RemoveAt(indToDel);
                        //}
                        if (mr.TermsAr[i].Pos[j].IndexElement.Count == 0)
                        {
                            var delRange = new Range(mr.TermsAr[i].Pos[j].Range.Inf);
                            mr.RootTermsTree.DeleteRange(delRange);
                        }
                    }
                    mr.TermsAr.RemoveAt(i);
                    ChangeIdexesInTree(mr, i);
                    sizeMr = mr.TermsAr.Count;
                    i--;
                }
            }            
        }
        public static Point GetRealPos(string fragment, string term, Point pos)
        {
            var lowTerm = term;
            lowTerm = lowTerm.ToLower();
            fragment = fragment.ToLower();
            var space = term.IndexOf(' ');
            if (space != -1)
            {
                var curWord = term.Substring(0, space);
                var largestFragment = FindFunctions.GetLargestCommonSubstring(curWord, fragment);
                var start = fragment.IndexOf(largestFragment);
                if (start != -1)
                {
                    var numberSpaces = FindFunctions.num_spaces(lowTerm);
                    var end = start;
                    while (numberSpaces > -1)
                    {
                        if (end + 1 < fragment.Length && fragment[end] == ' ' && fragment[end + 1] != ' ')
                        {
                            numberSpaces--;
                        }
                        else if (end + 1 >= fragment.Length)
                            numberSpaces--;
                        end++;
                    }
                    end--;
                    pos.X = pos.X + start - 1;
                    pos.Y = pos.X + end - 1;
                }
            }
            else
            {
                var largestFragment = FindFunctions.GetLargestCommonSubstring(lowTerm, fragment);
                var start = fragment.IndexOf(largestFragment);
                if (start != -1)
                {
                    var numberSpaces = 0;
                    var end = start;
                    while (numberSpaces > -1)
                    {
                        if (end + 1 < fragment.Length && fragment[end] == ' ' && fragment[end + 1] != ' ')
                        {
                            numberSpaces--;
                        }
                        else if (end + 1 >= fragment.Length)
                            numberSpaces--;
                        end++;
                    }
                    end--;
                    pos.Y = pos.X + end - 1;
                    pos.X = pos.X + start - 1;
                }
            }
            return pos;
        }

        public static string NormalizeNPattern(string nPattern)
        {
            nPattern = nPattern.Replace('[', '<');
            nPattern = nPattern.Replace(']', '>');
            var newPattern = Normalization(nPattern);
            while(newPattern != nPattern)
            {
                nPattern = newPattern;
                newPattern = Normalization(nPattern);
            }
            return nPattern.Replace("=TEXT>", "=text>").Replace("PA","Pa").Replace("NUM","Num");//.Replace("ё","е");
        }
        private static string Normalization(string nPattern)//Костыль, разобраться позже
        {
            var pattern = @"(<\s+)";
            foreach (Match match in Regex.Matches(nPattern, pattern, RegexOptions.IgnoreCase))
                nPattern = nPattern.Replace(match.Groups[1].Value, match.Groups[1].Value.Replace(" ", ""));
            pattern = @"(\s+>)";
            foreach (Match match in Regex.Matches(nPattern, pattern, RegexOptions.IgnoreCase))
                nPattern = nPattern.Replace(match.Groups[1].Value, match.Groups[1].Value.Replace(" ", ""));
            pattern = @"(\s*,\s*)";
            foreach (Match match in Regex.Matches(nPattern, pattern, RegexOptions.IgnoreCase))
                nPattern = nPattern.Replace(match.Groups[1].Value, match.Groups[1].Value.Replace(" ", ""));
            pattern = @"(\.\w[=,>])";
            foreach (Match match in Regex.Matches(nPattern, pattern, RegexOptions.IgnoreCase))
                nPattern = nPattern.Replace(match.Groups[1].Value, match.Groups[1].Value.ToLower());
            pattern = @"([,<](C|N|G|DOC|T|A|F|M|P|R)=\w*[,>])";
            //MatchCollection matches = Regex.Matches(NPattern, pattern);
            foreach (Match match in Regex.Matches(nPattern, pattern))
                nPattern = nPattern.Replace(match.Groups[1].Value, match.Groups[1].Value.ToLower());
            pattern = @"([0-9]<[\w-0-9]*[,>])";
            foreach (Match match in Regex.Matches(nPattern, pattern))
                nPattern = nPattern.Replace(match.Groups[1].Value, match.Groups[1].Value.ToLower());
            return nPattern;
        }
        public static void PrintConstantPatterns(StreamWriter sw)
        {
            foreach (var str in ConstantPatterns)
                sw.WriteLine(str);
        }
        public static void ChangeIdexesInTree(Terms mr, int delEl) // Костыль, по возможности убрать
        {
            var changed = new List<TermTree>();
            for (var i=0; i<mr.TermsAr.Count; i++)
            {
                for (var j=0; j<mr.TermsAr[i].Pos.Count; j++)
                {
                    if (!changed.Contains(mr.TermsAr[i].Pos[j]))
                    {
                        if (mr != null && mr.TermsAr != null && mr.TermsAr.Count > i && mr.TermsAr[i].Pos != null && mr.TermsAr[i].Pos.Count > j &&
                            mr.TermsAr[i].Pos[j]?.IndexElement != null)
                        {
                            var indexSetList = mr.TermsAr[i].Pos[j].IndexElement.ToList();
                            for (var k = 0; k < indexSetList.Count; k++)
                            {
                                if (indexSetList[k] > delEl)
                                {
                                    indexSetList[k]--;
                                }
                            }
                            mr.TermsAr[i].Pos[j].IndexElement = new HashSet<int>(indexSetList);
                            changed.Add(mr.TermsAr[i].Pos[j]);
                        }
                    }
                }
            }
        }
        public static void ChangeIdexesInTree(SynTerms mr, int delEl) // Костыль, по возможности убрать
        {
            var changed = new List<TermTree>();
            for (var i = 0; i < mr.TermsAr.Count; i++)
            {
                for (var j = 0; j < mr.TermsAr[i].Pos.Count; j++)
                {
                    if (!changed.Contains(mr.TermsAr[i].Pos[j]))
                    {
                        if (mr?.TermsAr != null && mr.TermsAr.Count > i && mr.TermsAr[i].Pos != null && mr.TermsAr[i].Pos.Count > j && mr.TermsAr[i].Pos[j]?.IndexElement != null)
                        {
                            var indexSetList = mr.TermsAr[i].Pos[j].IndexElement.ToList();
                            for (var k = 0; k < indexSetList.Count; k++)
                            {
                                if (indexSetList[k] > delEl)
                                {
                                    indexSetList[k]--;
                                }
                            }
                            mr.TermsAr[i].Pos[j].IndexElement = new HashSet<int>(indexSetList);
                            changed.Add(mr.TermsAr[i].Pos[j]);
                        }
                    }
                }
            }
        }
        public static void ChangeIdexesInTree(NonDictTerms mr, int delEl) // Костыль, по возможности убрать
        {
            var changed = new List<TermTree>();
            for (var i = 0; i < mr.TermsAr.Count; i++)
            {
                for (var j = 0; j < mr.TermsAr[i].Pos.Count; j++)
                {
                    if (!changed.Contains(mr.TermsAr[i].Pos[j]))
                    {
                        if (mr?.TermsAr != null && mr.TermsAr.Count > i && mr.TermsAr[i].Pos != null && mr.TermsAr[i].Pos.Count > j && mr.TermsAr[i].Pos[j]?.IndexElement != null)
                        {
                            var indexSetList = mr.TermsAr[i].Pos[j].IndexElement.ToList();
                            for (var k = 0; k < indexSetList.Count; k++)
                            {
                                if (indexSetList[k] > delEl)
                                {
                                    indexSetList[k]--;
                                }
                            }
                            mr.TermsAr[i].Pos[j].IndexElement = new HashSet<int>(indexSetList);
                            changed.Add(mr.TermsAr[i].Pos[j]);
                        }
                    }
                }
            }
        }
        public static void ChangeIdexesInTree(CombTerms mr, int delEl) // Костыль, по возможности убрать
        {
            var changed = new List<TermTree>();
            for (var i = 0; i < mr.TermsAr.Count; i++)
            {
                for (var j = 0; j < mr.TermsAr[i].Pos.Count; j++)
                {
                    if (!changed.Contains(mr.TermsAr[i].Pos[j]))
                    {
                        if (mr?.TermsAr != null && mr.TermsAr.Count > i && mr.TermsAr[i].Pos != null && mr.TermsAr[i].Pos.Count > j && mr.TermsAr[i].Pos[j]?.IndexElement != null)
                        {
                            var indexSetList = mr.TermsAr[i].Pos[j].IndexElement.ToList();
                            for (var k = 0; k < indexSetList.Count; k++)
                            {
                                if (indexSetList[k] > delEl)
                                {
                                    indexSetList[k]--;
                                }
                            }
                            mr.TermsAr[i].Pos[j].IndexElement = new HashSet<int>(indexSetList);
                            changed.Add(mr.TermsAr[i].Pos[j]);
                        }
                    }
                }
            }
        }
        private static string IntegerToRoman4K(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("Value must be between 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + IntegerToRoman(number - 1000);
            if (number >= 900) return "CM" + IntegerToRoman(number - 900); //EDIT: i've typed 400 instead 900
            if (number >= 500) return "D" + IntegerToRoman(number - 500);
            if (number >= 400) return "CD" + IntegerToRoman(number - 400);
            if (number >= 100) return "C" + IntegerToRoman(number - 100);
            if (number >= 90) return "XC" + IntegerToRoman(number - 90);
            if (number >= 50) return "L" + IntegerToRoman(number - 50);
            if (number >= 40) return "XL" + IntegerToRoman(number - 40);
            if (number >= 10) return "X" + IntegerToRoman(number - 10);
            if (number >= 9) return "IX" + IntegerToRoman(number - 9);
            if (number >= 5) return "V" + IntegerToRoman(number - 5);
            if (number >= 4) return "IV" + IntegerToRoman(number - 4);
            if (number >= 1) return "I" + IntegerToRoman(number - 1);
            throw new ArgumentOutOfRangeException("Value must be between 1 and 3999");
        }

        public static string IntegerToRoman(int number)
        {
            int num = number;
            string result = "";
            bool start = true;
            while (num > 3999)
            {
                if (!start) result += "plus";
                start = false;
                result += IntegerToRoman4K(3999);
                num = num - 3999;
            }
            result += IntegerToRoman4K(num);
            return result;
        }
        private static Dictionary<char, int> _romanMap = new Dictionary<char, int>
        {
            {'I', 1},
            {'V', 5},
            {'X', 10},
            {'L', 50},
            {'C', 100},
            {'D', 500},
            {'M', 1000}
        };
        private static int RomanToInteger4K(string roman)
        {
            var number = 0;
            for (var i = 0; i < roman.Length; i++)
            {
                if (i + 1 < roman.Length && _romanMap[roman[i]] < _romanMap[roman[i + 1]])
                {
                    number -= _romanMap[roman[i]];
                }
                else
                {
                    number += _romanMap[roman[i]];
                }
            }
            return number;
        }

        public static int RomanToInteger(string roman)
        {
            string[] parts = roman.Replace("plus","+").Split('+');
            return parts.Sum(RomanToInteger4K);
        }
    }
}




//while (MR.TermsAr[i].Pos.Count > 0)
//                    {
//                        for (int k = 0; k < MR.TermsAr[i].Pos[0].indexElement.Count; k++)
//                            if (MR.TermsAr[i].Pos[0].indexElement[k] != i)
//                            {
//                                for (int p=0; p<MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos.Count; p++)
//                                {
//                                    if (MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos[p] == MR.TermsAr[i].Pos[0])
//                                    {
//                                        MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos.RemoveAt(p);
//                                        p--;
//                                    }
//                                }
//                            }
//                        MR.TermsAr[i].Pos.RemoveAt(0);
//                        MR.rootTermsTree.DeleteRange(MR.TermsAr[i].Pos[0].range);
//                        //MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin() );
//                        //vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);                        
//                    }
//                    //MR.TermsAr.erase(MR.TermsAr.begin() + i);
//                    //vector<Term>(MR.TermsAr).swap(MR.TermsAr);
//                    MR.TermsAr.RemoveAt(i);
//                    i--;
//                    sizeMR = MR.TermsAr.Count;

//int sizeMR = MR.TermsAr.Count;
//            for (int i = 0; i < sizeMR; i++)
//            {
//                if (MR.TermsAr[i].setToDel)
//                {
//                    while (MR.TermsAr[i].Pos.Count > 0)
//                    {
//                        for (int k = 0; k < MR.TermsAr[i].Pos[0].indexElement.Count; k++)
//                            if (MR.TermsAr[i].Pos[0].indexElement[k] != i)
//                            {
//                                for (int p = 0; p < MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos.Count; p++)
//                                {
//                                    if (MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos[p] == MR.TermsAr[i].Pos[0])
//                                    {
//                                        MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos.RemoveAt(p);
//                                        p--;
//                                    }
//                                }
//                            }
//                        MR.TermsAr[i].Pos.RemoveAt(0);
//                        MR.rootTermsTree.DeleteRange(MR.TermsAr[i].Pos[0].range);
//                        //MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin());
//                        //vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);
                        
//                    }
//                    //MR.TermsAr.erase(MR.TermsAr.begin() + i);
//                    //vector<SynTerm>(MR.TermsAr).swap(MR.TermsAr);
//                    MR.TermsAr.RemoveAt(i);
//                    i--;
//                    sizeMR = MR.TermsAr.Count;
//                }
//            }
//            for (int i = 0; i < MR.TermsAr.Count; i++)
//            {
//                for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
//                    if (!MR.TermsAr[i].Pos[j].indexElement.Contains(i)) MR.TermsAr[i].Pos[j].indexElement.Add(i);
//            }






// List<string> atributes = new List<string>({ "c", "n", "g", "doc", "t", "a", "f", "m", "p", "r" });            
//bool change = true;
//int open_t = -1;
//int close_t = 0;
//string newNP = "";
//while (change)
//{
//    change = false;
//    open_t = NPattern.IndexOf("<", open_t + 1);
//    if (open_t != -1 &&
//        (NPattern[open_t - 1] >= '0' &&
//         NPattern[open_t - 1] <= '9'))
//    {
//        newNP = newNP + NPattern.Substring(close_t, open_t - close_t + 1);
//        //newNP = newNP + NPattern.Substring(close_t, close_t - open_t + 1);
//        //prev_close_t = close_t;
//        close_t = NPattern.IndexOf(">", open_t + 1);
//        newNP = newNP + NPattern.Substring(open_t + 1, close_t - (open_t + 1)).Trim().ToLower().Replace(" ", "");
//        change = true;
//    }
//    else
//    {
//        int p = open_t;
//        while (p != -1) // Костыль, можно изящнее
//        {
//            p = NPattern.IndexOf("<", p + 1);
//            if (p != -1 &&
//                (NPattern[p - 1] >= '0' &&
//                 NPattern[p - 1] <= '9'))
//                break;
//        }
//        if (p != -1)
//        {
//            open_t = p;
//            newNP = newNP + NPattern.Substring(close_t, open_t - close_t + 1);
//            close_t = NPattern.IndexOf(">", open_t + 1);
//            newNP = newNP + NPattern.Substring(open_t + 1, close_t - (open_t + 1)).Trim().ToLower().Replace(" ", "");
//            change = true;
//        }
//        else
//        {
//            newNP = newNP + NPattern.Substring(close_t);
//        }

//    }

//}