using System;
using System.ComponentModel;

namespace Novacode
{

  public enum ListItemType
  {
    Bulleted,
    Numbered
  }

  public enum SectionBreakType
  {
    DefaultNextPage,
    EvenPage,
    OddPage,
    Continuous
  }


  public enum ContainerType
  {
    None,
    Toc,
    Section,
    Cell,
    Table,
    Header,
    Footer,
    Paragraph,
    Body
  }

  public enum PageNumberFormat
  {
    Normal,
    Roman
  }

  public enum BorderSize
  {
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine
  }

  public enum EditRestrictions
  {
    None,
    ReadOnly,
    Forms,
    Comments,
    TrackedChanges
  }

  /// <summary>
  /// Table Cell Border styles
  /// Added by lckuiper @ 20101117
  /// source: http://msdn.microsoft.com/en-us/library/documentformat.openxml.wordprocessing.tablecellborders.aspx
  /// </summary>
  public enum BorderStyle
  {
    TcbsNone = 0,
    TcbsSingle,
    TcbsThick,
    TcbsDouble,
    TcbsDotted,
    TcbsDashed,
    TcbsDotDash,
    TcbsDotDotDash,
    TcbsTriple,
    TcbsThinThickSmallGap,
    TcbsThickThinSmallGap,
    TcbsThinThickThinSmallGap,
    TcbsThinThickMediumGap,
    TcbsThickThinMediumGap,
    TcbsThinThickThinMediumGap,
    TcbsThinThickLargeGap,
    TcbsThickThinLargeGap,
    TcbsThinThickThinLargeGap,
    TcbsWave,
    TcbsDoubleWave,
    TcbsDashSmallGap,
    TcbsDashDotStroked,
    TcbsThreeDEmboss,
    TcbsThreeDEngrave,
    TcbsOutset,
    TcbsInset,
	TcbsNil
  }

  /// <summary>
  /// Table Cell Border Types
  /// Added by lckuiper @ 20101117
  /// source: http://msdn.microsoft.com/en-us/library/documentformat.openxml.wordprocessing.tablecellborders.aspx
  /// </summary>
  public enum TableCellBorderType
  {
    Top,
    Bottom,
    Left,
    Right,
    InsideH,
    InsideV,
    TopLeftToBottomRight,
    TopRightToBottomLeft
  }

  /// <summary>
  /// Table Border Types
  /// Added by lckuiper @ 20101117
  /// source: http://msdn.microsoft.com/en-us/library/documentformat.openxml.wordprocessing.tableborders.aspx
  /// </summary>
  public enum TableBorderType
  {
    Top,
    Bottom,
    Left,
    Right,
    InsideH,
    InsideV
  }

  // Patch 7398 added by lckuiper on Nov 16th 2010 @ 2:23 PM
  public enum VerticalAlignment
  {
    Top,
    Center,
    Bottom
  };

  public enum Orientation
  {
    Portrait,
    Landscape
  };

  public enum XmlDocument
  {
    Main,
    HeaderOdd,
    HeaderEven,
    HeaderFirst,
    FooterOdd,
    FooterEven,
    FooterFirst
  };

  public enum MatchFormattingOptions
  {
    ExactMatch,
    SubsetMatch
  };

  public enum Script
  {
    Superscript,
    Subscript,
    None
  }

  public enum Highlight
  {
    Yellow,
    Green,
    Cyan,
    Magenta,
    Blue,
    Red,
    DarkBlue,
    DarkCyan,
    DarkGreen,
    DarkMagenta,
    DarkRed,
    DarkYellow,
    DarkGray,
    LightGray,
    Black,
    None
  };

  public enum UnderlineStyle
  {
      None = 0,
      SingleLine = 1,
      Words = 2,
      DoubleLine = 3,
      Dotted = 4,
      Thick = 6,
      Dash = 7,
      DotDash = 9,
      DotDotDash = 10,
      Wave = 11,
      DottedHeavy = 20,
      DashedHeavy = 23,
      DashDotHeavy = 25,
      DashDotDotHeavy = 26,
      DashLongHeavy = 27,
      DashLong = 39,
      WavyDouble = 43,
      WavyHeavy = 55,
  };

  public enum StrikeThrough
  {
    None,
    Strike,
    DoubleStrike
  };

  public enum Misc
  {
    None,
    Shadow,
    Outline,
    OutlineShadow,
    Emboss,
    Engrave
  };

  /// <summary>
  /// Change the caps style of text, for use with Append and AppendLine.
  /// </summary>
  public enum CapsStyle
  {
    /// <summary>
    /// No caps, make all characters are lowercase.
    /// </summary>
    None,

    /// <summary>
    /// All caps, make every character uppercase.
    /// </summary>
    Caps,

    /// <summary>
    /// Small caps, make all characters capital but with a small font size.
    /// </summary>
    SmallCaps
  };

  /// <summary>
  /// Designs\Styles that can be applied to a table.
  /// </summary>
  public enum TableDesign
  {
    Custom,
    TableNormal,
    TableGrid,
    LightShading,
    LightShadingAccent1,
    LightShadingAccent2,
    LightShadingAccent3,
    LightShadingAccent4,
    LightShadingAccent5,
    LightShadingAccent6,
    LightList,
    LightListAccent1,
    LightListAccent2,
    LightListAccent3,
    LightListAccent4,
    LightListAccent5,
    LightListAccent6,
    LightGrid,
    LightGridAccent1,
    LightGridAccent2,
    LightGridAccent3,
    LightGridAccent4,
    LightGridAccent5,
    LightGridAccent6,
    MediumShading1,
    MediumShading1Accent1,
    MediumShading1Accent2,
    MediumShading1Accent3,
    MediumShading1Accent4,
    MediumShading1Accent5,
    MediumShading1Accent6,
    MediumShading2,
    MediumShading2Accent1,
    MediumShading2Accent2,
    MediumShading2Accent3,
    MediumShading2Accent4,
    MediumShading2Accent5,
    MediumShading2Accent6,
    MediumList1,
    MediumList1Accent1,
    MediumList1Accent2,
    MediumList1Accent3,
    MediumList1Accent4,
    MediumList1Accent5,
    MediumList1Accent6,
    MediumList2,
    MediumList2Accent1,
    MediumList2Accent2,
    MediumList2Accent3,
    MediumList2Accent4,
    MediumList2Accent5,
    MediumList2Accent6,
    MediumGrid1,
    MediumGrid1Accent1,
    MediumGrid1Accent2,
    MediumGrid1Accent3,
    MediumGrid1Accent4,
    MediumGrid1Accent5,
    MediumGrid1Accent6,
    MediumGrid2,
    MediumGrid2Accent1,
    MediumGrid2Accent2,
    MediumGrid2Accent3,
    MediumGrid2Accent4,
    MediumGrid2Accent5,
    MediumGrid2Accent6,
    MediumGrid3,
    MediumGrid3Accent1,
    MediumGrid3Accent2,
    MediumGrid3Accent3,
    MediumGrid3Accent4,
    MediumGrid3Accent5,
    MediumGrid3Accent6,
    DarkList,
    DarkListAccent1,
    DarkListAccent2,
    DarkListAccent3,
    DarkListAccent4,
    DarkListAccent5,
    DarkListAccent6,
    ColorfulShading,
    ColorfulShadingAccent1,
    ColorfulShadingAccent2,
    ColorfulShadingAccent3,
    ColorfulShadingAccent4,
    ColorfulShadingAccent5,
    ColorfulShadingAccent6,
    ColorfulList,
    ColorfulListAccent1,
    ColorfulListAccent2,
    ColorfulListAccent3,
    ColorfulListAccent4,
    ColorfulListAccent5,
    ColorfulListAccent6,
    ColorfulGrid,
    ColorfulGridAccent1,
    ColorfulGridAccent2,
    ColorfulGridAccent3,
    ColorfulGridAccent4,
    ColorfulGridAccent5,
    ColorfulGridAccent6,
    None
  };

  /// <summary>
  /// How a Table should auto resize.
  /// </summary>
  public enum AutoFit
  {
    /// <summary>
    /// Autofit to Table contents.
    /// </summary>
    Contents,

    /// <summary>
    /// Autofit to Window.
    /// </summary>
    Window,

    /// <summary>
    /// Autofit to Column width.
    /// </summary>
    ColumnWidth,
    /// <summary>
    ///  Autofit to Fixed column width
    /// </summary>
    Fixed
  };

  public enum RectangleShapes
  {
    Rect,
    RoundRect,
    Snip1Rect,
    Snip2SameRect,
    Snip2DiagRect,
    SnipRoundRect,
    Round1Rect,
    Round2SameRect,
    Round2DiagRect
  };

  public enum BasicShapes
  {
    Ellipse,
    Triangle,
    RtTriangle,
    Parallelogram,
    Trapezoid,
    Diamond,
    Pentagon,
    Hexagon,
    Heptagon,
    Octagon,
    Decagon,
    Dodecagon,
    Pie,
    Chord,
    Teardrop,
    Frame,
    HalfFrame,
    Corner,
    DiagStripe,
    Plus,
    Plaque,
    Can,
    Cube,
    Bevel,
    Donut,
    NoSmoking,
    BlockArc,
    FoldedCorner,
    SmileyFace,
    Heart,
    LightningBolt,
    Sun,
    Moon,
    Cloud,
    Arc,
    BacketPair,
    BracePair,
    LeftBracket,
    RightBracket,
    LeftBrace,
    RightBrace
  };

  public enum BlockArrowShapes
  {
    RightArrow,
    LeftArrow,
    UpArrow,
    DownArrow,
    LeftRightArrow,
    UpDownArrow,
    QuadArrow,
    LeftRightUpArrow,
    BentArrow,
    UturnArrow,
    LeftUpArrow,
    BentUpArrow,
    CurvedRightArrow,
    CurvedLeftArrow,
    CurvedUpArrow,
    CurvedDownArrow,
    StripedRightArrow,
    NotchedRightArrow,
    HomePlate,
    Chevron,
    RightArrowCallout,
    DownArrowCallout,
    LeftArrowCallout,
    UpArrowCallout,
    LeftRightArrowCallout,
    QuadArrowCallout,
    CircularArrow
  };

  public enum EquationShapes
  {
    MathPlus,
    MathMinus,
    MathMultiply,
    MathDivide,
    MathEqual,
    MathNotEqual
  };

  public enum FlowchartShapes
  {
    FlowChartProcess,
    FlowChartAlternateProcess,
    FlowChartDecision,
    FlowChartInputOutput,
    FlowChartPredefinedProcess,
    FlowChartInternalStorage,
    FlowChartDocument,
    FlowChartMultidocument,
    FlowChartTerminator,
    FlowChartPreparation,
    FlowChartManualInput,
    FlowChartManualOperation,
    FlowChartConnector,
    FlowChartOffpageConnector,
    FlowChartPunchedCard,
    FlowChartPunchedTape,
    FlowChartSummingJunction,
    FlowChartOr,
    FlowChartCollate,
    FlowChartSort,
    FlowChartExtract,
    FlowChartMerge,
    FlowChartOnlineStorage,
    FlowChartDelay,
    FlowChartMagneticTape,
    FlowChartMagneticDisk,
    FlowChartMagneticDrum,
    FlowChartDisplay
  };

  public enum StarAndBannerShapes
  {
    IrregularSeal1,
    IrregularSeal2,
    Star4,
    Star5,
    Star6,
    Star7,
    Star8,
    Star10,
    Star12,
    Star16,
    Star24,
    Star32,
    Ribbon,
    Ribbon2,
    EllipseRibbon,
    EllipseRibbon2,
    VerticalScroll,
    HorizontalScroll,
    Wave,
    DoubleWave
  };

  public enum CalloutShapes
  {
    WedgeRectCallout,
    WedgeRoundRectCallout,
    WedgeEllipseCallout,
    CloudCallout,
    BorderCallout1,
    BorderCallout2,
    BorderCallout3,
    AccentCallout1,
    AccentCallout2,
    AccentCallout3,
    Callout1,
    Callout2,
    Callout3,
    AccentBorderCallout1,
    AccentBorderCallout2,
    AccentBorderCallout3
  };

  /// <summary>
  /// Text alignment of a Paragraph.
  /// </summary>
  public enum Alignment
  {
    /// <summary>
    /// Align Paragraph to the left.
    /// </summary>
    Left,

    /// <summary>
    /// Align Paragraph as centered.
    /// </summary>
    Center,

    /// <summary>
    /// Align Paragraph to the right.
    /// </summary>
    Right,

    /// <summary>
    /// (Justified) Align Paragraph to both the left and right margins, adding extra space between content as necessary.
    /// </summary>
    Both
  };

  public enum Direction
  {
    LeftToRight,
    RightToLeft
  };

  /// <summary>
  /// Paragraph edit types
  /// </summary>
  internal enum EditType
  {
    /// <summary>
    /// A ins is a tracked insertion
    /// </summary>
    Ins,

    /// <summary>
    /// A del is  tracked deletion
    /// </summary>
    Del
  }

  /// <summary>
  /// Custom property types.
  /// </summary>
  internal enum CustomPropertyType
  {
    /// <summary>
    /// System.String
    /// </summary>
    Text,

    /// <summary>
    /// System.DateTime
    /// </summary>
    Date,

    /// <summary>
    /// System.Int32
    /// </summary>
    NumberInteger,

    /// <summary>
    /// System.Double
    /// </summary>
    NumberDecimal,

    /// <summary>
    /// System.Boolean
    /// </summary>
    YesOrNo
  }

  /// <summary>
  /// Text types in a Run
  /// </summary>
  public enum RunTextType
  {
    /// <summary>
    /// System.String
    /// </summary>
    Text,

    /// <summary>
    /// System.String
    /// </summary>
    DelText,
  }
  public enum LineSpacingType
  {
  	Line,
  	Before,
  	After
  }
  
  public enum LineSpacingTypeAuto
  {
  	AutoBefore,
  	AutoAfter,
  	Auto,
  	None
  }

  /// <summary>
  /// Cell margin for all sides of the table cell.
  /// </summary>
  public enum TableCellMarginType
  {
      /// <summary>
      /// The left cell margin.
      /// </summary>
      Left,
      /// <summary>
      /// The right cell margin.
      /// </summary>
      Right,
      /// <summary>
      /// The bottom cell margin.
      /// </summary>
      Bottom,
      /// <summary>
      /// The top cell margin.
      /// </summary>
      Top
  }

  public enum HeadingType
  {
      [Description("Heading1")]
      Heading1,

      [Description("Heading2")]
      Heading2,

      [Description("Heading3")]
      Heading3,

      [Description("Heading4")]
      Heading4,

      [Description("Heading5")]
      Heading5,

      [Description("Heading6")]
      Heading6,

      [Description("Heading7")]
      Heading7,

      [Description("Heading8")]
      Heading8,

      [Description("Heading9")]
      Heading9,


      /*		 
       * The Character Based Headings below do not work in the same way as the headings 1-9 above, but appear on the same list in word. 
       * I have kept them here for reference in case somebody else things its just a matter of adding them in to gain extra headings
       */
      #region Other character (NOT paragraph) based Headings
      //[Description("NoSpacing")]
      //NoSpacing,

      //[Description("Title")]
      //Title,

      //[Description("Subtitle")]
      //Subtitle,

      //[Description("Quote")]
      //Quote,

      //[Description("IntenseQuote")]
      //IntenseQuote,

      //[Description("Emphasis")]
      //Emphasis,

      //[Description("IntenseEmphasis")]
      //IntenseEmphasis,

      //[Description("Strong")]
      //Strong,

      //[Description("ListParagraph")]
      //ListParagraph,

      //[Description("SubtleReference")]
      //SubtleReference,

      //[Description("IntenseReference")]
      //IntenseReference,

      //[Description("BookTitle")]
      //BookTitle, 
      #endregion


  }
  public enum TextDirection
  {
      BtLr,
      Right
  };

    /// <summary>
    /// Represents the switches set on a TOC.
    /// </summary>
    [Flags]
    public enum TableOfContentsSwitches
    {
        None = 0 << 0,
        [Description("\\a")]
        A = 1 << 0,
        [Description("\\b")]
        B = 1 << 1,
        [Description("\\c")]
        C = 1 << 2,
        [Description("\\d")]
        D = 1 << 3,
        [Description("\\f")]
        F = 1 << 4,
        [Description("\\h")]
        H = 1 << 5,
        [Description("\\l")]
        L = 1 << 6,
        [Description("\\n")]
        N = 1 << 7,
        [Description("\\o")]
        O = 1 << 8,
        [Description("\\p")]
        P = 1 << 9,
        [Description("\\s")]
        S = 1 << 10,
        [Description("\\t")]
        T = 1 << 11,
        [Description("\\u")]
        U = 1 << 12,
        [Description("\\w")]
        W = 1 << 13,
        [Description("\\x")]
        X = 1 << 14,
        [Description("\\z")]
        Z = 1 << 15,
    }

}