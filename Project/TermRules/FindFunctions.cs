using System.Collections.Generic;
using System.Drawing;

namespace TermRules
{
    public static class FindFunctions
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
        public static int FindInList(List<Pair<string, string>> v, Pair<string, string> str)
        {
            if (v == null || str == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if (v[i].First == str.First && v[i].Second == str.Second)
                    return i;
            }
            return -1;
        }
        public static int FindInList(List<Term> v, string str, string pos)
        {
            if (v == null || str == null || pos == null) return -1;
	        for (var i=0; i<v.Count;i++)
	        {
		        if (v[i].TermWord==str && v[i].PoSstr == pos)
			        return i;
	        }
	        return -1;
        }
        public static int FindInList(List<Term> v, string str)
        {
            if (v == null || str == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if (v[i].TermWord == str)
                    return i;
            }
            return -1;
        }
        public static int FindInList(List<NonDictTerm> v, string word, string pos)
        {
            if (v == null || word == null || pos == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if (v[i].TermWord == word && v[i].PoSstr == pos)
                    return i;
            }
            return -1;
        }
        public static int FindInList(List<NonDictTerm> v, string str)
        {
            if (v == null || str == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if (v[i].TermWord == str)
                    return i;
            }
            return -1;
        }
        public static int FindInList(List<CombTerm> v, string word, string pos)
        {
            if (v == null || word == null || pos == null) return -1;
	        for (var i=0; i<v.Count;i++)
	        {
		        if (v[i].TermWord==word && v[i].PoSstr == pos)
			        return i;
	        }
	        return -1;
        }
        public static int FindInList(List<CombTerm> v, string str)
        {
            if (v == null || str == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if (v[i].TermWord == str)
                    return i;
            }
            return -1;
        }

        public static int FindInList(List<CombComponent> v, string str)
        {
            if (v == null || str == null) return -1;
	        for (var i = 0; i<v.Count; i++)
	        {
		        if (v[i].TermWord == str)
			        return i;
	        }
	        return -1;
        }
        public static int FindInList(List<SynTerm> v, Pair<SynTermAlternative, SynTermAlternative> str)
        {
            if (v == null || str == null) return -1;
	        for (var i = 0; i<v.Count; i++)
	        {
		        if ((v[i].Alternatives.First.Alternative == str.First.Alternative   && 
                    v[i].Alternatives.Second.Alternative == str.Second.Alternative) || 
                    (v[i].Alternatives.First.Alternative == str.Second.Alternative  && 
                    v[i].Alternatives.Second.Alternative == str.First.Alternative))
			        return i;
	        }
	        return -1;
        }
        public static int FindInListPos(List<SynTerm> v, Pair<SynTermAlternative, SynTermAlternative> str)
        {
            if (v == null || str == null) return -1;
            return v.FindIndex(item => 
                (item.Alternatives.First.Alternative == str.First.Alternative && item.Alternatives.Second.Alternative == str.Second.Alternative &&
                 item.Alternatives.First.PoSstr == str.First.PoSstr && item.Alternatives.Second.PoSstr == str.Second.PoSstr) ||
                (item.Alternatives.First.Alternative == str.Second.Alternative && item.Alternatives.Second.Alternative == str.First.Alternative &&
                 item.Alternatives.First.PoSstr == str.Second.PoSstr && item.Alternatives.Second.PoSstr == str.First.PoSstr));
            //if (v == null || str == null) return -1;
            //for (int i = 0; i < v.Count; i++)
            //{
            //    if ((v[i].alternatives.first.alternative == str.first.alternative &&
            //        v[i].alternatives.second.alternative == str.second.alternative &&
            //        v[i].alternatives.first.POSstr == str.first.POSstr &&
            //        v[i].alternatives.second.POSstr == str.second.POSstr) ||
            //        (v[i].alternatives.first.alternative == str.second.alternative &&
            //        v[i].alternatives.second.alternative == str.first.alternative &&
            //        v[i].alternatives.first.POSstr == str.second.POSstr &&
            //        v[i].alternatives.second.POSstr == str.first.POSstr))
            //        return i;
            //}
            //return -1;
        }
        public static int FindPattern(List<Term> v, HashSet<int> ind, string str)
        {
            if (v == null || ind == null || str == null) return -1;
            foreach (var i in ind)
            {
                if (v[i].Pattern == str) return i;
            }
            //for (var i=0; i<ind.Count;i++)
            //{
            //    if (v[ind[i]].Pattern == str) return ind[i];
            //}
            return -1;
        }
        public static int FindPattern(List<NonDictTerm> v, HashSet<int> ind, string str)
        {
            if (v == null || ind == null || str == null) return -1;
            foreach (var i in ind)
            {
                if (v[i].Pattern == str) return i;
            }
            return -1;
        }
        public static int FindPattern(List<CombTerm> v, HashSet<int> ind, string str)
        {
            if (v == null || ind == null || str == null) return -1;
            foreach (var i in ind)
            {
                if (v[i].Pattern == str) return i;
            }
            return -1;
        }
        public static int FindPattern(List<SynTerm> v, HashSet<int> ind, string str)
        {
            if (v == null || ind == null || str == null) return -1;
            foreach (var i in ind)
            {
                if (v[i].Pattern == str) return i;
            }
            return -1;
        }
        public static int FindPattern(List<CombComponent> v, string str)
        {
            if (v == null || str == null) return -1;
            return v.FindIndex(item => item.Pattern == str);
            //for (int i = 0; i < v.Count; i++)
            //{
            //    if (v[i].Pattern == str) return i;
            //}
            //return -1;
        }
        public static int FindPattern(List<NonDictComponent> v, string str)
        {
            if (v == null || str == null) return -1;
            return v.FindIndex(item => item.Pattern == str);
            //for (int i = 0; i < v.Count; i++)
            //{
            //    if (v[i].Pattern == str) return i;
            //}
            //return -1;
        }
        public static int FindBlock(List<NonDictBlock> v, string str)
        {
            for (var i=0; i<v.Count; i++)
            {
                if (v[i].Block == str) return i; 
            }
            return -1;
        }
        public static int FindFragmentInList(List<Term> v, string str)
        {
            if (v == null || str == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if (v[i].TermFragment == str)
                    return i;
            }
            return -1;
        }
        public static int FindFragmentInList(List<NonDictTerm> v, string str)
        {
            if (v == null || str == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if (v[i].TermFragment == str)
                    return i;
            }
            return -1;
        }
        public static int FindFragmentInList(List<CombTerm> v, string str)
        {
            if (v == null || str == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if (v[i].TermFragment== str)
                    return i;
            }
            return -1;
        }
        public static int FindFragmentInList(List<CombComponent> v, string str)
        {
            if (v == null || str == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if (v[i].TermFragment== str)
                    return i;
            }
            return -1;
        }
        public static int FindFragmentInListPos(List<Term> v, Point p)
        {
            if (v == null || p == null) return -1;
	        var sizeV=v.Count;
	        for (var i=0; i<sizeV ;i++)
	        {
		        var sizeP=v[i].Pos.Count;
		        for (var j=0; j<sizeP; j++)
		        {
			        if (v[i].Pos[j].Range.Inf.X==p.X && v[i].Pos[j].Range.Inf.Y==p.Y)
				        return i;
		        }
	        }
	        return -1;
        }
        public static int FindFragmentInListPos(List<NonDictTerm> v, Point p)
        {
            if (v == null || p == null) return -1;
	        var sizeV=v.Count;
	        for (var i=0; i<sizeV ;i++)
	        {
		        var sizeP=v[i].Pos.Count;
		        for (var j=0; j<sizeP; j++)
		        {
			        if (v[i].Pos[j].Range.Inf.X==p.X && v[i].Pos[j].Range.Inf.Y==p.Y)
				        return i;
		        }
	        }
	        return -1;
        }
        public static int FindFragmentInListPos(List<CombTerm> v, Point p)
        {
            if (v == null || p == null) return -1;
	        var sizeV=v.Count;
	        for (var i=0; i<sizeV ;i++)
	        {
		        var sizeP=v[i].Pos.Count;
		        for (var j=0; j<sizeP; j++)
		        {
			        if (v[i].Pos[j].Range.Inf.X==p.X && v[i].Pos[j].Range.Inf.Y==p.Y)
				        return i;
		        }
	        }
	        return -1;
        }
        public static ComponentInElement FindInListComponents(List<NonDictTerm> v, string str)
        {
            if (v == null || str == null) return null;
	        var result  = new ComponentInElement();	        
	        result.Element =-1;
            result.Block = -1;
            result.Component = -1;	        
	        for (var i=0; i<v.Count;i++)
	        {
		        for (var k=0; k<v[i].Blocks.Count; k++)
		        {
			        var s=v[i].Blocks[k].Components.Count;
			        for(var j=0; j<s; j++)
			        {
				        if (v[i].Blocks[k].Components[j].Component == str)
				        {
					        result.Element=i;
					        result.Block=k;
                            result.Component = j;
					        return result;
				        }
			        }
		        }
	        }	
	        return result;
        }
        public static ComponentInElement FindInListComponents(List<CombTerm> v, string str)
        {
            if (v == null || str == null) return null;
	        var result = new ComponentInElement();
            result.Element = -1;
            result.Block = -1;
            result.Component = -1;	       
	        for (var i=0; i<v.Count;i++)
	        {
		        var s=v[i].Components.Count;
		        for(var j=0; j<s; j++)
		        {
			        if (v[i].Components[j].TermWord == str)
			        {
				        result.Element=i;
				        result.Component=j;
				        return result;
			        }
		        }
	        }
	        return result;
        }
        public static int FindEqualVariants(NonDictTerm v, List<string> var)
        {
            if (v == null || var == null) return -1;
	        for (var i=0 ; i<v.Blocks.Count ; i++)
	        {
		        var f=false;
		        if (v.Blocks[i].Components.Count != var.Count)
			        continue;
		        for (var j=0 ; j<v.Blocks[i].Components.Count; j++)
		        {
			        f=false;
			        if(v.Blocks[i].Components[j].Component == var[j])
				        f=true;
			        if(f == false) break;
		        }
		        if(f) return i;
	        }
	        return -1;
        }
        public static int FindEqualVariants(NonDictTerm v, List<NonDictComponent> var)
        {
            if (v == null || var == null) return -1;
            for (var i = 0; i < v.Blocks.Count; i++)
            {
                var f = false;
                if (v.Blocks[i].Components.Count != var.Count)
                    continue;
                for (var j = 0; j < v.Blocks[i].Components.Count; j++)
                {
                    f = false;
                    if (v.Blocks[i].Components[j].Component == var[j].Component)
                        f = true;
                    if (f == false) break;
                }
                if (f) return i;
            }
            return -1;
        }
        public static int FindInListStr(List<string> v, string str)
        {
            if (v == null || str == null) return -1;
	        for (var i=0; i<v.Count;i++)
	        {
		        if (v[i]==str)
			        return i;
	        }
	        return -1;
        }
        public static int FindInListStr(List<NonDictComponent> v, string str)
        {
            if (v == null || str == null) return -1;
            for (var i = 0; i < v.Count; i++)
            {
                if (v[i].Component == str)
                    return i;
            }
            return -1;
        }
        public static int FindPos(List<Point> pos, Point p)
        {
            if (pos == null || p == null) return -1;
            return pos.FindIndex(item => item.X == p.X && item.Y == p.Y);
            //int size=pos.Count;
            //for (int i=0; i<size; i++)
            //    if (pos[i].X == p.X && pos[i].Y == p.Y)
            //        return i;
            //return -1;
        }
        public static string GetLargestCommonSubstring(string s1, string s2)
        {
            if (s1 == null || s2 == null) return null;
            var a = new int[s1.Length + 1, s2.Length + 1];
            int u = 0, v = 0;

            for (var i = 0; i < s1.Length; i++)
                for (var j = 0; j < s2.Length; j++)
                    if (s1[i] == s2[j])
                    {
                        a[i + 1, j + 1] = a[i, j] + 1;
                        if (a[i + 1, j + 1] > a[u, v])
                        {
                            u = i + 1;
                            v = j + 1;
                        }
                    }

            return s1.Substring(u - a[u, v], a[u, v]);
        }
        public static int num_spaces(string str)
        {
            if (str == null) return -1;
	        var res = 0;
	        for (var i = 0; i < str.Length; i++)
	        {
		        if (i + 1 < str.Length && str[i] == ' ' && str[i + 1] != ' ')
			        res++;
	        }
	        return res;
        }
        }
}
