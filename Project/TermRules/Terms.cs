using System.Collections.Generic;
using System.Drawing;
using Novacode;
using DbscanImplementation;
namespace TermRules
{
    public class Pair<T, TU>
    {
        public Pair(T first, TU second)
        {
            First = first;
            Second = second;
        }
        public Pair() {}
        public T First { get; set; }
        public TU Second { get; set; }
    }
    public enum KindOfTerm
    {
        Null,
        AuthTerm,
        DictTerm,
        NonDictTerm,
        CombTerm,
        SynTerm
    }
    public enum TypeOfTerm
    {
        Null,
        Trusted,
        Untrusted,
        UntrastedByFrequency

    }
    public class Term
    {        
        //public List<Point> Positions; //необходимо, особенно для словарных терминов
        public KindOfTerm Kind { get; set;}
        public TypeOfTerm type { get; set; }
        public string PoSstr {get; set;}
        public string TermWord { get; set; }
        public string TermFragment { get; set; }
        public string TermFullNormForm { get; set; }
        public string Pattern { get; set; }
        public string NPattern { get; set; }
        public int PatCounter { get; set; }
        public bool SetToDel { get; set; }
        public int Frequency { get; set; }
        public bool FreqStart { get; set; }
        public int Rule { get; set; }
        public int RepeatRule { get; set; }
        public double CValue { get; set; }
        public string synonimTo { get; set; }
        public List<TermTree> Pos { get; set; }
        public bool inHeader { get; set; }
        public int mainPage { get; set; }
        public List<MyCustomDatasetItem[]> pages { get; set; }
        public Term() 
        {
            Kind = KindOfTerm.Null;
            type = TypeOfTerm.Null;
            TermWord = "";
            TermFragment = "";
            Pattern = "";
            NPattern = "";
            PatCounter = 0;
            SetToDel = false;
            FreqStart = true;
            Frequency = 0;
            CValue = 0;
            synonimTo = "";
            inHeader = false;
            Pos = new List<TermTree>();
            mainPage = 0;
            pages = new List<MyCustomDatasetItem[]>();
        }        
    }
    public class NonDictBlock
    {
        public string Block { get; set; }
        public List<NonDictComponent> Components { get; set; }
        public NonDictBlock() 
        {
            Block = "";
            Components = new List<NonDictComponent>();
        }  
    }
    public class NonDictComponent
    {
        public string Component { get; set; }
        public string PoSstr { get; set; }
        public string Pattern { get; set; }
        public string NPattern { get; set; }
        public NonDictComponent()
        {
            Component = "";
            Pattern = "";
            NPattern = "";
        }
    }
    public class NonDictTerm : Term
    {
        public List<NonDictBlock> Blocks { get; set; }
        public bool FreqStart { get; set; }
        public NonDictTerm()
        {
            Kind = KindOfTerm.NonDictTerm;
            Blocks = new List<NonDictBlock>();
            FreqStart = true;
        }
        ~NonDictTerm() { }
    }
    // TODO: Возможно стоит заменить на Term
    public class CombComponent
    {
        public string TermWord { get; set; }
        public string TermFragment { get; set; }
        public string Pattern { get; set; }
        public string NPattern { get; set; }
        public int PatCounter { get; set; }
        public int Frequency { get; set; }
        public string PoSstr { get; set; }
        public bool FreqStart { get; set; }
        public List<Point> Pos { get; set; }
        public string TermFullNormForm { get; set; }
        public CombComponent() 
        {
            TermWord = "";
            TermFragment = "";
            Pattern = "";
            NPattern = "";
            PatCounter = 0;
            Frequency = 0;
            FreqStart = true;
            Pos = new List<Point>();
        }
	    ~CombComponent() {}
    }
    public class CombTerm : Term
    {
        public List<CombComponent> Components { get; set; }
        public CombTerm() 
        {
            Kind = KindOfTerm.CombTerm;
            Components = new List<CombComponent>();
        }
        ~CombTerm() { }
    }
    public class SynTermAlternative
    {
        public string Alternative { get; set; }
        public string PoSstr { get; set; }
        public string PatternPart { get; set; }
        public string NPattern { get; set; }
        public string Pattern { get; set; }
        public bool FreqStart { get; set; }
        public int Frequency { get; set; }
        public bool inHeader { get; set; }
        public string TermFullNormForm { get; set; }

        public SynTermAlternative()
        {
            Alternative = "";
            PatternPart = "";
            NPattern = "";
            Pattern = "";
            FreqStart = true;
            Frequency = 0;
            inHeader = false;
            TermFullNormForm = "";
        }
    }
    public class SynTerm
    {
        public KindOfTerm Kind { get; set; }
        public bool SetToDel { get; set; }
        public int Frequency { get; set; }
        public List<TermTree> Pos { get; set; }
        public Pair<SynTermAlternative, SynTermAlternative> Alternatives { get; set; }
        public string Pattern { get; set; }
        public string TermFragment { get; set; }
        public SynTerm()
        {
            Kind = KindOfTerm.SynTerm;
            SetToDel = false;
            Frequency = 0;
            Pos = new List<TermTree>();
            Alternatives = new Pair<SynTermAlternative, SynTermAlternative>();
            Pattern = "";
            TermFragment = "";
        }
    }
    public class Terms
    {
        public List<Term> TermsAr { get; set;}
        public TermTree RootTermsTree { get; set;}
        public Terms()
        {
            TermsAr = new List<Term>();
            RootTermsTree = new TermTree();
        }
        ~Terms() { }
    }
    public class SynTerms
    {
            public List<SynTerm> TermsAr { get; set; }
            public TermTree RootTermsTree { get; set; }
            public SynTerms()
            {
                TermsAr = new List<SynTerm>();
                RootTermsTree = new TermTree();
            }
            ~SynTerms() { }
    }
    public class CombTerms
    {
        public List<CombTerm> TermsAr { get; set; }
        public TermTree RootTermsTree { get; set; }
        public CombTerms()
            {
                TermsAr = new List<CombTerm>();
                RootTermsTree = new TermTree();
            }
    }
    public class NonDictTerms
    {
        public List<NonDictTerm> TermsAr { get; set; }
        public TermTree RootTermsTree { get; set; }
        public NonDictTerms()
            {
                TermsAr = new List<NonDictTerm>();
                RootTermsTree = new TermTree();
            }
    }
    public class ComponentInElement
    {
        public int Element { get; set; }
        public int Block { get; set; }
        public int Component { get; set; }
        public ComponentInElement()
        {
            Element = 0;
            Block = 0;
            Component = 0;
        }
    }
}
