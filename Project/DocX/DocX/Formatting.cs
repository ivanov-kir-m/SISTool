using System;
using System.Linq;
using System.Xml.Linq;
using System.Drawing;
using System.Globalization;
namespace Novacode
{
    /// <summary>
    /// A text formatting.
    /// </summary>
    public class Formatting : IComparable
    {
		private XElement _rPr;
		private bool? _hidden;
		private bool? _bold;
		private bool? _italic;
		private StrikeThrough? _strikethrough;
		private Script? _script;
		private Highlight? _highlight;
		private double? _size;
		private Color? _fontColor;
		private Color? _underlineColor;
		private UnderlineStyle? _underlineStyle;
		private Misc? _misc;
		private CapsStyle? _capsStyle;
		private Font _fontFamily;
		private int? _percentageScale;
		private int? _kerning;
		private int? _position;
		private double? _spacing;

		private CultureInfo _language;

        /// <summary>
        /// A text formatting.
        /// </summary>
        public Formatting()
        {
            _capsStyle = Novacode.CapsStyle.None;
            _strikethrough = Novacode.StrikeThrough.None;
            _script = Novacode.Script.None;
            _highlight = Novacode.Highlight.None;
            _underlineStyle = Novacode.UnderlineStyle.None;
            _misc = Novacode.Misc.None;

            // Use current culture by default
            _language = CultureInfo.CurrentCulture;

            _rPr = new XElement(XName.Get("rPr", DocX.W.NamespaceName));
        }

        /// <summary>
        /// Text language
        /// </summary>
        public CultureInfo Language 
        { 
            get 
            { 
                return _language; 
            } 
            
            set 
            { 
                _language = value; 
            } 
        }

		/// <summary>
		/// Returns a new identical instance of Formatting.
		/// </summary>
		/// <returns></returns>
		public Formatting Clone()
		{
			Formatting newf = new Formatting();
			newf.Bold = _bold;
			newf.CapsStyle = _capsStyle;
			newf.FontColor = _fontColor;
			newf.FontFamily = _fontFamily;
			newf.Hidden = _hidden;
			newf.Highlight = _highlight;
			newf.Italic = _italic;
			if (_kerning.HasValue) { newf.Kerning = _kerning; }
			newf.Language = _language;
			newf.Misc = _misc;
			if (_percentageScale.HasValue) { newf.PercentageScale = _percentageScale; }
			if (_position.HasValue) { newf.Position = _position; }
			newf.Script = _script;
			if (_size.HasValue) { newf.Size = _size; }
			if (_spacing.HasValue) { newf.Spacing = _spacing; }
			newf.StrikeThrough = _strikethrough;
			newf.UnderlineColor = _underlineColor;
			newf.UnderlineStyle = _underlineStyle;
			return newf;
		}

		public static Formatting Parse(XElement rPr)
        {
            Formatting formatting = new Formatting();

            // Build up the Formatting object.
            foreach (XElement option in rPr.Elements())
            {
                switch (option.Name.LocalName)
                {
                    case "lang": 
                        formatting.Language = new CultureInfo(
                            option.GetAttribute(XName.Get("val", DocX.W.NamespaceName), null) ?? 
                            option.GetAttribute(XName.Get("eastAsia", DocX.W.NamespaceName), null) ?? 
                            option.GetAttribute(XName.Get("bidi", DocX.W.NamespaceName))); 
                        break;
                    case "spacing": 
                        formatting.Spacing = Double.Parse(
                            option.GetAttribute(XName.Get("val", DocX.W.NamespaceName))) / 20.0; 
                        break;
                    case "position": 
                        formatting.Position = Int32.Parse(
                            option.GetAttribute(XName.Get("val", DocX.W.NamespaceName))) / 2; 
                        break;
                    case "kern": 
                        formatting.Position = Int32.Parse(
                            option.GetAttribute(XName.Get("val", DocX.W.NamespaceName))) / 2; 
                        break;
                    case "w": 
                        formatting.PercentageScale = Int32.Parse(
                            option.GetAttribute(XName.Get("val", DocX.W.NamespaceName))); 
                        break;
                    // <w:sz w:val="20"/><w:szCs w:val="20"/>
                    case "sz":
                        formatting.Size = Int32.Parse(
                            option.GetAttribute(XName.Get("val", DocX.W.NamespaceName))) / 2; 
                        break;
                   

                    case "rFonts": 
                        formatting.FontFamily = 
                            new Font(
                                option.GetAttribute(XName.Get("cs", DocX.W.NamespaceName), null) ??
                                option.GetAttribute(XName.Get("ascii", DocX.W.NamespaceName), null) ??
                                option.GetAttribute(XName.Get("hAnsi", DocX.W.NamespaceName), null) ??
                                option.GetAttribute(XName.Get("eastAsia", DocX.W.NamespaceName))); 
                        break;
                    case "color" :
                        try
                        {
                            string color = option.GetAttribute(XName.Get("val", DocX.W.NamespaceName));
                            formatting.FontColor = System.Drawing.ColorTranslator.FromHtml(string.Format("#{0}", color));
                        }
                        catch { }
                        break;
                    case "vanish": formatting._hidden = true; break;
                    case "b": formatting.Bold = true; break;
                    case "i": formatting.Italic = true; break;
                    case "u": formatting.UnderlineStyle = HelperFunctions.GetUnderlineStyle(option.GetAttribute(XName.Get("val", DocX.W.NamespaceName))); break;
                    case "vertAlign":
                        var script = option.GetAttribute(XName.Get("val", DocX.W.NamespaceName), null);
                        formatting.Script = (Script)Enum.Parse(typeof(Script), script);
                        break;
                    default: break;
                }
            }


            return formatting;
        }

        internal XElement Xml
        {
            get
            {
                _rPr = new XElement(XName.Get("rPr", DocX.W.NamespaceName));

                if (_language != null)
                    _rPr.Add(new XElement(XName.Get("lang", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), _language.Name)));
                
                if(_spacing.HasValue)
                    _rPr.Add(new XElement(XName.Get("spacing", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), _spacing.Value * 20)));

                if(_position.HasValue)
                    _rPr.Add(new XElement(XName.Get("position", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), _position.Value * 2)));                   

                if (_kerning.HasValue)
                    _rPr.Add(new XElement(XName.Get("kern", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), _kerning.Value * 2)));                   

                if (_percentageScale.HasValue)
                    _rPr.Add(new XElement(XName.Get("w", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), _percentageScale)));

                if (_fontFamily != null)
                {
                    _rPr.Add
                    (
                        new XElement
                        (
                            XName.Get("rFonts", DocX.W.NamespaceName), 
                            new XAttribute(XName.Get("ascii", DocX.W.NamespaceName), _fontFamily.Name),
                            new XAttribute(XName.Get("hAnsi", DocX.W.NamespaceName), _fontFamily.Name), // Added by Maurits Elbers to support non-standard characters. See http://docx.codeplex.com/Thread/View.aspx?ThreadId=70097&ANCHOR#Post453865
                            new XAttribute(XName.Get("cs", DocX.W.NamespaceName), _fontFamily.Name)    // Added by Maurits Elbers to support non-standard characters. See http://docx.codeplex.com/Thread/View.aspx?ThreadId=70097&ANCHOR#Post453865
                        )
                    );
                }

				if (_hidden.HasValue && _hidden.Value)
					_rPr.Add(new XElement(XName.Get("vanish", DocX.W.NamespaceName)));

				if (_bold.HasValue && _bold.Value)
					_rPr.Add(new XElement(XName.Get("b", DocX.W.NamespaceName)));

				if (_italic.HasValue && _italic.Value)
					_rPr.Add(new XElement(XName.Get("i", DocX.W.NamespaceName)));

				if (_underlineStyle.HasValue)
				{
					switch (_underlineStyle)
					{
					case Novacode.UnderlineStyle.None:
						break;
					case Novacode.UnderlineStyle.SingleLine:
						_rPr.Add(new XElement(XName.Get("u", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), "single")));
						break;
					case Novacode.UnderlineStyle.DoubleLine:
						_rPr.Add(new XElement(XName.Get("u", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), "double")));
						break;
					default:
						_rPr.Add(new XElement(XName.Get("u", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), _underlineStyle.ToString())));
						break;
					}
				}

                if(_underlineColor.HasValue)
                {
                    // If an underlineColor has been set but no underlineStyle has been set
                    if (_underlineStyle == Novacode.UnderlineStyle.None)
                    {
                        // Set the underlineStyle to the default
                        _underlineStyle = Novacode.UnderlineStyle.SingleLine;
                        _rPr.Add(new XElement(XName.Get("u", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), "single")));
                    }

                    _rPr.Element(XName.Get("u", DocX.W.NamespaceName)).Add(new XAttribute(XName.Get("color", DocX.W.NamespaceName), _underlineColor.Value.ToHex()));
                }

				if (_strikethrough.HasValue)
				{
					switch (_strikethrough)
					{
					case Novacode.StrikeThrough.None:
						break;
					case Novacode.StrikeThrough.Strike:
						_rPr.Add(new XElement(XName.Get("strike", DocX.W.NamespaceName)));
						break;
					case Novacode.StrikeThrough.DoubleStrike:
						_rPr.Add(new XElement(XName.Get("dstrike", DocX.W.NamespaceName)));
						break;
					default:
						break;
					}
				}

				if (_script.HasValue)
				{
					switch (_script)
					{
					case Novacode.Script.None:
						break;
					default:
						_rPr.Add(new XElement(XName.Get("vertAlign", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), _script.ToString())));
						break;
					}
				}

				if (_size.HasValue)
                {
                    _rPr.Add(new XElement(XName.Get("sz", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), (_size * 2).ToString())));
                    _rPr.Add(new XElement(XName.Get("szCs", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), (_size * 2).ToString())));
                }

                if(_fontColor.HasValue)
                    _rPr.Add(new XElement(XName.Get("color", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), _fontColor.Value.ToHex())));

				if (_highlight.HasValue)
				{
					switch (_highlight)
					{
					case Novacode.Highlight.None:
						break;
					default:
						_rPr.Add(new XElement(XName.Get("highlight", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), _highlight.ToString())));
						break;
					}
				}

				if (_capsStyle.HasValue)
				{
					switch (_capsStyle)
					{
					case Novacode.CapsStyle.None:
						break;
					default:
						_rPr.Add(new XElement(XName.Get(_capsStyle.ToString(), DocX.W.NamespaceName)));
						break;
					}
				}

				if (_misc.HasValue)
				{
					switch (_misc)
					{
					case Novacode.Misc.None:
						break;
					case Novacode.Misc.OutlineShadow:
						_rPr.Add(new XElement(XName.Get("outline", DocX.W.NamespaceName)));
						_rPr.Add(new XElement(XName.Get("shadow", DocX.W.NamespaceName)));
						break;
					case Novacode.Misc.Engrave:
						_rPr.Add(new XElement(XName.Get("imprint", DocX.W.NamespaceName)));
						break;
					default:
						_rPr.Add(new XElement(XName.Get(_misc.ToString(), DocX.W.NamespaceName)));
						break;
					}
				}

				return _rPr;
            }
        }

        /// <summary>
        /// This formatting will apply Bold.
        /// </summary>
        public bool? Bold { get { return _bold; } set { _bold = value;} }

        /// <summary>
        /// This formatting will apply Italic.
        /// </summary>
        public bool? Italic { get { return _italic; } set { _italic = value; } }

        /// <summary>
        /// This formatting will apply StrickThrough.
        /// </summary>
        public StrikeThrough? StrikeThrough { get { return _strikethrough; } set { _strikethrough = value; } }

        /// <summary>
        /// The script that this formatting should be, normal, superscript or subscript.
        /// </summary>
        public Script? Script { get { return _script; } set { _script = value; } }
        
        /// <summary>
        /// The Size of this text, must be between 0 and 1638.
        /// </summary>
        public double? Size 
        { 
            get { return _size; } 
            
            set 
            { 
                double? temp = value * 2;

                if (temp - (int)temp == 0)
                {
                    if(value > 0 && value < 1639)
                        _size = value;
                    else
                        throw new ArgumentException("Size", "Value must be in the range 0 - 1638");
                }

                else
                    throw new ArgumentException("Size", "Value must be either a whole or half number, examples: 32, 32.5");
            } 
        }

        /// <summary>
        /// Percentage scale must be one of the following values 200, 150, 100, 90, 80, 66, 50 or 33.
        /// </summary>
        public int? PercentageScale
        { 
            get { return _percentageScale; } 
            
            set 
            {
                if ((new int?[] { 200, 150, 100, 90, 80, 66, 50, 33 }).Contains(value))
                    _percentageScale = value; 
                else
                    throw new ArgumentOutOfRangeException("PercentageScale", "Value must be one of the following: 200, 150, 100, 90, 80, 66, 50 or 33");
            } 
        }

        /// <summary>
        /// The Kerning to apply to this text must be one of the following values 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72.
        /// </summary>
        public int? Kerning 
        { 
            get { return _kerning; } 
            
            set 
            { 
                if(new int?[] {8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72}.Contains(value))
                    _kerning = value; 
                else
                    throw new ArgumentOutOfRangeException("Kerning", "Value must be one of the following: 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48 or 72");
            } 
        }

        /// <summary>
        /// Text position must be in the range (-1585 - 1585).
        /// </summary>
        public int? Position
        {
            get { return _position; }

            set
            {
                if (value > -1585 && value < 1585)
                    _position = value;
                else
                    throw new ArgumentOutOfRangeException("Position", "Value must be in the range -1585 - 1585");
            }
        }

        /// <summary>
        /// Text spacing must be in the range (-1585 - 1585).
        /// </summary>
        public double? Spacing
        {
            get { return _spacing; }

            set
            {
                double? temp = value * 20;

                if (temp - (int)temp == 0)
                {
                    if (value > -1585 && value < 1585)
                        _spacing = value;
                    else
                        throw new ArgumentException("Spacing", "Value must be in the range: -1584 - 1584");
                }

                else
                    throw new ArgumentException("Spacing", "Value must be either a whole or acurate to one decimal, examples: 32, 32.1, 32.2, 32.9");
            } 
        }

        /// <summary>
        /// The colour of the text.
        /// </summary>
        public Color? FontColor { get { return _fontColor; } set { _fontColor = value; } }

        /// <summary>
        /// Highlight colour.
        /// </summary>
        public Highlight? Highlight { get { return _highlight; } set { _highlight = value; } }
       
        /// <summary>
        /// The Underline style that this formatting applies.
        /// </summary>
        public UnderlineStyle? UnderlineStyle { get { return _underlineStyle; } set { _underlineStyle = value; } }
        
        /// <summary>
        /// The underline colour.
        /// </summary>
        public Color? UnderlineColor { get { return _underlineColor; } set { _underlineColor = value; } }
        
        /// <summary>
        /// Misc settings.
        /// </summary>
        public Misc? Misc { get { return _misc; } set { _misc = value; } }
        
        /// <summary>
        /// Is this text hidden or visible.
        /// </summary>
        public bool? Hidden { get { return _hidden; } set { _hidden = value; } }
        
        /// <summary>
        /// Capitalization style.
        /// </summary>
        public CapsStyle? CapsStyle { get { return _capsStyle; } set { _capsStyle = value; } }
        
        /// <summary>
        /// The font family of this formatting.
        /// </summary>
        /// <!-- 
        /// Bug found and fixed by krugs525 on August 12 2009.
        /// Use TFS compare to see exact code change.
        /// -->
        public Font FontFamily { get { return _fontFamily; } set { _fontFamily = value; } }

        public int CompareTo(object obj)
        {
            Formatting other = (Formatting)obj;

            if(other._hidden != this._hidden)
                return -1;

            if(other._bold != this._bold)
                return -1;

            if(other._italic != this._italic)
                return -1;

            if(other._strikethrough != this._strikethrough)
                return -1;

            if(other._script != this._script)
                return -1;

            if(other._highlight != this._highlight)
                return -1;

            if(other._size != this._size)
                return -1;

            if(other._fontColor != this._fontColor)
                return -1;

            if(other._underlineColor != this._underlineColor)
                return -1;

            if(other._underlineStyle != this._underlineStyle)
                return -1;

            if(other._misc != this._misc)
                return -1;

            if(other._capsStyle != this._capsStyle)
                return -1;

            if(other._fontFamily != this._fontFamily)
                return -1;

            if(other._percentageScale != this._percentageScale)
                return -1;

            if(other._kerning != this._kerning)
                return -1;

            if(other._position != this._position)
                return -1;

            if(other._spacing != this._spacing)
                return -1;

            if (!other._language.Equals(this._language))
                return -1;

            return 0;
        }
    }
}
