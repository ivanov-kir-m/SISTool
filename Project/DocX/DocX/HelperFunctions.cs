using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;

namespace Novacode
{
    internal static class HelperFunctions
    {
        public const string DocumentDocumenttype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml";
        public const string TemplateDocumenttype = "application/vnd.openxmlformats-officedocument.wordprocessingml.template.main+xml";

            public static bool IsNullOrWhiteSpace(this string value)
            {
                if (value == null) return true;
                return string.IsNullOrEmpty(value.Trim());
            }

        /// <summary>
        /// Checks whether 'toCheck' has all children that 'desired' has and values of 'val' attributes are the same
        /// </summary>
        /// <param name="desired"></param>
        /// <param name="toCheck"></param>
        /// <param name="fo">Matching options whether check if desired attributes are inder a, or a has exactly and only these attributes as b has.</param>
        /// <returns></returns>
        internal static bool ContainsEveryChildOf(XElement desired, XElement toCheck, MatchFormattingOptions fo)
        {
            foreach (XElement e in desired.Elements())
            {
                // If a formatting property has the same name and 'val' attribute's value, its considered to be equivalent.
                if (!toCheck.Elements(e.Name).Where(bElement => bElement.GetAttribute(XName.Get("val", DocX.W.NamespaceName)) == e.GetAttribute(XName.Get("val", DocX.W.NamespaceName))).Any())
                    return false;
            }

            // If the formatting has to be exact, no additionaly formatting must exist.
            if (fo == MatchFormattingOptions.ExactMatch)
                return desired.Elements().Count() == toCheck.Elements().Count();

            return true;
        }
        internal static void CreateRelsPackagePart(DocX document, Uri uri)
        {
            PackagePart pp = document.Package.CreatePart(uri, DocX.ContentTypeApplicationRelationShipXml, CompressionOption.Maximum);
            using (TextWriter tw = new StreamWriter(new PackagePartStream(pp.GetStream())))
            {
                XDocument d = new XDocument
                (
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement(XName.Get("Relationships", DocX.Rel.NamespaceName))
                );
                var root = d.Root;
                d.Save(tw);
            }
        }

        internal static int GetSize(XElement xml)
        {
            switch (xml.Name.LocalName)
            {
                case "tab":
                    return 1;
                case "br":
                    return 1;
                case "t":
                    goto case "delText";
                case "delText":
                    return xml.Value.Length;
                case "tr":
                    goto case "br";
                case "tc":
                    goto case "br";
                default:
                    return 0;
            }
        }

        internal static string GetText(XElement e)
        {
            StringBuilder sb = new StringBuilder();
            GetTextRecursive(e, ref sb);
            return sb.ToString();
        }

        internal static void GetTextRecursive(XElement xml, ref StringBuilder sb)
        {
            sb.Append(ToText(xml));

            if (xml.HasElements)
                foreach (XElement e in xml.Elements())
                    GetTextRecursive(e, ref sb);
        }

        internal static List<FormattedText> GetFormattedText(XElement e)
        {
            List<FormattedText> alist = new List<FormattedText>();
            GetFormattedTextRecursive(e, ref alist);
            return alist;
        }

        internal static void GetFormattedTextRecursive(XElement xml, ref List<FormattedText> alist)
        {
            FormattedText ft = ToFormattedText(xml);
            FormattedText last = null;

            if (ft != null)
            {
                if (alist.Count() > 0)
                    last = alist.Last();

                if (last != null && last.CompareTo(ft) == 0)
                {
                    // Update text of last entry.
                    last.Text += ft.Text;
                }
                else
                {
                    if (last != null)
                        ft.Index = last.Index + last.Text.Length;

                    alist.Add(ft);
                }
            }

            if (xml.HasElements)
                foreach (XElement e in xml.Elements())
                    GetFormattedTextRecursive(e, ref alist);
        }

        internal static FormattedText ToFormattedText(XElement e)
        {
            // The text representation of e.
            String text = ToText(e);
            if (text == String.Empty)
                return null;

            // e is a w:t element, it must exist inside a w:r element or a w:tabs, lets climb until we find it.
            while (!e.Name.Equals(XName.Get("r", DocX.W.NamespaceName)) &&
                   !e.Name.Equals(XName.Get("tabs", DocX.W.NamespaceName)))
                e = e.Parent;

            // e is a w:r element, lets find the rPr element.
            XElement rPr = e.Element(XName.Get("rPr", DocX.W.NamespaceName));

            FormattedText ft = new FormattedText();
            ft.Text = text;
            ft.Index = 0;
            ft.Formatting = null;

            // Return text with formatting.
            if (rPr != null)
                ft.Formatting = Formatting.Parse(rPr);

            return ft;
        }

        internal static string ToText(XElement e)
        {
            switch (e.Name.LocalName)
            {
                case "tab":
                    return "\t";
                case "br":
                    return "\n";
                case "t":
                    goto case "delText";
                case "delText":
                    {
                        if (e.Parent != null && e.Parent.Name.LocalName == "r")
                        {
                            XElement run = e.Parent;
                            var rPr = run.Elements().FirstOrDefault(a => a.Name.LocalName == "rPr");
                            if (rPr != null)
                            {
                                var caps = rPr.Elements().FirstOrDefault(a => a.Name.LocalName == "caps");

                                if (caps != null)
                                    return e.Value.ToUpper();
                            }
                        }

                        return e.Value;
                    }
                case "tr":
                    goto case "br";
                case "tc":
                    goto case "tab";
                default: return "";
            }
        }

        internal static XElement CloneElement(XElement element)
        {
            return new XElement
            (
                element.Name,
                element.Attributes(),
                element.Nodes().Select
                (
                    n =>
                    {
                        XElement e = n as XElement;
                        if (e != null)
                            return CloneElement(e);
                        return n;
                    }
                )
            );
        }

        internal static PackagePart CreateOrGetSettingsPart(Package package)
        {
            PackagePart settingsPart;

            Uri settingsUri = new Uri("/word/settings.xml", UriKind.Relative);
            if (!package.PartExists(settingsUri))
            {
                settingsPart = package.CreatePart(settingsUri, "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml", CompressionOption.Maximum);

                PackagePart mainDocumentPart = package.GetParts().Single(p => p.ContentType.Equals(DocumentDocumenttype, StringComparison.CurrentCultureIgnoreCase) ||
                                                                              p.ContentType.Equals(TemplateDocumenttype, StringComparison.CurrentCultureIgnoreCase));

                mainDocumentPart.CreateRelationship(settingsUri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings");

                XDocument settings = XDocument.Parse
                (@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
                <w:settings xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:r='http://schemas.openxmlformats.org/officeDocument/2006/relationships' xmlns:m='http://schemas.openxmlformats.org/officeDocument/2006/math' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w10='urn:schemas-microsoft-com:office:word' xmlns:w='http://schemas.openxmlformats.org/wordprocessingml/2006/main' xmlns:sl='http://schemas.openxmlformats.org/schemaLibrary/2006/main'>
                  <w:zoom w:percent='100' />
                  <w:defaultTabStop w:val='720' />
                  <w:characterSpacingControl w:val='doNotCompress' />
                  <w:compat />
                  <w:rsids>
                    <w:rsidRoot w:val='00217F62' />
                    <w:rsid w:val='001915A3' />
                    <w:rsid w:val='00217F62' />
                    <w:rsid w:val='00A906D8' />
                    <w:rsid w:val='00AB5A74' />
                    <w:rsid w:val='00F071AE' />
                  </w:rsids>
                  <m:mathPr>
                    <m:mathFont m:val='Cambria Math' />
                    <m:brkBin m:val='before' />
                    <m:brkBinSub m:val='--' />
                    <m:smallFrac m:val='off' />
                    <m:dispDef />
                    <m:lMargin m:val='0' />
                    <m:rMargin m:val='0' />
                    <m:defJc m:val='centerGroup' />
                    <m:wrapIndent m:val='1440' />
                    <m:intLim m:val='subSup' />
                    <m:naryLim m:val='undOvr' />
                  </m:mathPr>
                  <w:themeFontLang w:val='en-IE' w:bidi='ar-SA' />
                  <w:clrSchemeMapping w:bg1='light1' w:t1='dark1' w:bg2='light2' w:t2='dark2' w:accent1='accent1' w:accent2='accent2' w:accent3='accent3' w:accent4='accent4' w:accent5='accent5' w:accent6='accent6' w:hyperlink='hyperlink' w:followedHyperlink='followedHyperlink' />
                  <w:shapeDefaults>
                    <o:shapedefaults v:ext='edit' spidmax='2050' />
                    <o:shapelayout v:ext='edit'>
                      <o:idmap v:ext='edit' data='1' />
                    </o:shapelayout>
                  </w:shapeDefaults>
                  <w:decimalSymbol w:val='.' />
                  <w:listSeparator w:val=',' />
                </w:settings>"
                );

                XElement themeFontLang = settings.Root.Element(XName.Get("themeFontLang", DocX.W.NamespaceName));
                themeFontLang.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), CultureInfo.CurrentCulture);

                // Save the settings document.
                using (TextWriter tw = new StreamWriter(new PackagePartStream(settingsPart.GetStream())))
                    settings.Save(tw);
            }
            else
                settingsPart = package.GetPart(settingsUri);
            return settingsPart;
        }

        internal static void CreateCustomPropertiesPart(DocX document)
        {
            PackagePart customPropertiesPart = document.Package.CreatePart(new Uri("/docProps/custom.xml", UriKind.Relative), "application/vnd.openxmlformats-officedocument.custom-properties+xml", CompressionOption.Maximum);

            XDocument customPropDoc = new XDocument
            (
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement
                (
                    XName.Get("Properties", DocX.CustomPropertiesSchema.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "vt", DocX.CustomVTypesSchema)
                )
            );

            using (TextWriter tw = new StreamWriter(new PackagePartStream(customPropertiesPart.GetStream(FileMode.Create, FileAccess.Write))))
                customPropDoc.Save(tw, SaveOptions.None);

            document.Package.CreateRelationship(customPropertiesPart.Uri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties");
        }

        internal static XDocument DecompressXmlResource(string manifestResourceName)
        {
            // XDocument to load the compressed Xml resource into.
            XDocument document;

            // Get a reference to the executing assembly.
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Open a Stream to the embedded resource.
            Stream stream = assembly.GetManifestResourceStream(manifestResourceName);

            // Decompress the embedded resource.
            using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
            {
                // Load this decompressed embedded resource into an XDocument using a TextReader.
                using (TextReader sr = new StreamReader(zip))
                {
                    document = XDocument.Load(sr);
                }
            }

            // Return the decompressed Xml as an XDocument.
            return document;
        }


        /// <summary>
        /// If this document does not contain a /word/numbering.xml add the default one generated by Microsoft Word 
        /// when the default bullet, numbered and multilevel lists are added to a blank document
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        internal static XDocument AddDefaultNumberingXml(Package package)
        {
            XDocument numberingDoc;
            // Create the main document part for this package
            PackagePart wordNumbering = package.CreatePart(new Uri("/word/numbering.xml", UriKind.Relative), "application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml", CompressionOption.Maximum);

            numberingDoc = DecompressXmlResource("Novacode.Resources.numbering.xml.gz");

            // Save /word/numbering.xml
            using (TextWriter tw = new StreamWriter(new PackagePartStream(wordNumbering.GetStream(FileMode.Create, FileAccess.Write))))
                numberingDoc.Save(tw, SaveOptions.None);

            PackagePart mainDocumentPart = package.GetParts().Single(p => p.ContentType.Equals(DocumentDocumenttype, StringComparison.CurrentCultureIgnoreCase) ||
                                                                          p.ContentType.Equals(TemplateDocumenttype, StringComparison.CurrentCultureIgnoreCase));

            mainDocumentPart.CreateRelationship(wordNumbering.Uri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/numbering");
            return numberingDoc;
        }



        /// <summary>
        /// If this document does not contain a /word/styles.xml add the default one generated by Microsoft Word.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        internal static XDocument AddDefaultStylesXml(Package package)
        {
            XDocument stylesDoc;
            // Create the main document part for this package
            PackagePart wordStyles = package.CreatePart(new Uri("/word/styles.xml", UriKind.Relative), "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml", CompressionOption.Maximum);

            stylesDoc = HelperFunctions.DecompressXmlResource("Novacode.Resources.default_styles.xml.gz");
            XElement lang = stylesDoc.Root.Element(XName.Get("docDefaults", DocX.W.NamespaceName)).Element(XName.Get("rPrDefault", DocX.W.NamespaceName)).Element(XName.Get("rPr", DocX.W.NamespaceName)).Element(XName.Get("lang", DocX.W.NamespaceName));
            lang.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), CultureInfo.CurrentCulture);

            // Save /word/styles.xml
            using (TextWriter tw = new StreamWriter(new PackagePartStream(wordStyles.GetStream(FileMode.Create, FileAccess.Write))))
                stylesDoc.Save(tw, SaveOptions.None);

            PackagePart mainDocumentPart = package.GetParts().Where
            (
                p => p.ContentType.Equals(DocumentDocumenttype, StringComparison.CurrentCultureIgnoreCase)||p.ContentType.Equals(TemplateDocumenttype, StringComparison.CurrentCultureIgnoreCase)
            ).Single();

            mainDocumentPart.CreateRelationship(wordStyles.Uri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles");
            return stylesDoc;
        }

        internal static XElement CreateEdit(EditType t, DateTime editTime, object content)
        {
            if (t == EditType.Del)
            {
                foreach (object o in (IEnumerable<XElement>)content)
                {
                    if (o is XElement)
                    {
                        XElement e = (o as XElement);
                        IEnumerable<XElement> ts = e.DescendantsAndSelf(XName.Get("t", DocX.W.NamespaceName));

                        for (int i = 0; i < ts.Count(); i++)
                        {
                            XElement text = ts.ElementAt(i);
                            text.ReplaceWith(new XElement(DocX.W + "delText", text.Attributes(), text.Value));
                        }
                    }
                }
            }

            return
            (
                new XElement(DocX.W + t.ToString(),
                    new XAttribute(DocX.W + "id", 0),
                    new XAttribute(DocX.W + "author", WindowsIdentity.GetCurrent().Name),
                    new XAttribute(DocX.W + "date", editTime),
                content)
            );
        }

        internal static XElement CreateTable(int rowCount, int columnCount)
		{
			int[] columnWidths = new int[columnCount];
			for (int i = 0; i < columnCount; i++)
			{
				columnWidths[i] = 2310;
			}
			return CreateTable(rowCount, columnWidths);
		}

		internal static XElement CreateTable(int rowCount, int[] columnWidths)
        {
            XElement newTable =
            new XElement
            (
                XName.Get("tbl", DocX.W.NamespaceName),
                new XElement
                (
                    XName.Get("tblPr", DocX.W.NamespaceName),
                        new XElement(XName.Get("tblStyle", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), "TableGrid")),
                        new XElement(XName.Get("tblW", DocX.W.NamespaceName), new XAttribute(XName.Get("w", DocX.W.NamespaceName), "5000"), new XAttribute(XName.Get("type", DocX.W.NamespaceName), "auto")),
                        new XElement(XName.Get("tblLook", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), "04A0"))
                )
            );

            /*XElement tableGrid = new XElement(XName.Get("tblGrid", DocX.w.NamespaceName));
            for (int i = 0; i < columnWidths.Length; i++)
                tableGrid.Add(new XElement(XName.Get("gridCol", DocX.w.NamespaceName), new XAttribute(XName.Get("w", DocX.w.NamespaceName), XmlConvert.ToString(columnWidths[i]))));

            newTable.Add(tableGrid);*/

            for (int i = 0; i < rowCount; i++)
            {
                XElement row = new XElement(XName.Get("tr", DocX.W.NamespaceName));

                for (int j = 0; j < columnWidths.Length; j++)
                {
                    XElement cell = CreateTableCell();
                    row.Add(cell);
                }

                newTable.Add(row);
            }
            return newTable;
        }

        /// <summary>
        /// Create and return a cell of a table        
        /// </summary>        
        internal static XElement CreateTableCell(double w = 2310)
        {
            return new XElement
                    (
                        XName.Get("tc", DocX.W.NamespaceName),
                            new XElement(XName.Get("tcPr", DocX.W.NamespaceName),
                            new XElement(XName.Get("tcW", DocX.W.NamespaceName),
                                    new XAttribute(XName.Get("w", DocX.W.NamespaceName), w),
                                    new XAttribute(XName.Get("type", DocX.W.NamespaceName), "dxa"))),
                            new XElement(XName.Get("p", DocX.W.NamespaceName),
                                new XElement(XName.Get("pPr", DocX.W.NamespaceName)))
                    );
        }

        internal static List CreateItemInList(List list, string listText, int level = 0, ListItemType listType = ListItemType.Numbered, int? startNumber = null, bool trackChanges = false, bool continueNumbering = false)
        {
            if (list.NumId == 0)
            {
                list.CreateNewNumberingNumId(level, listType, startNumber, continueNumbering);
            }

            if (listText != null) //I see no reason why you shouldn't be able to insert an empty element. It simplifies tasks such as populating an item from html.
            {
                var newParagraphSection = new XElement
                    (
                    XName.Get("p", DocX.W.NamespaceName),
                    new XElement(XName.Get("pPr", DocX.W.NamespaceName),
                                 new XElement(XName.Get("numPr", DocX.W.NamespaceName),
                                              new XElement(XName.Get("ilvl", DocX.W.NamespaceName), new XAttribute(DocX.W + "val", level)),
                                              new XElement(XName.Get("numId", DocX.W.NamespaceName), new XAttribute(DocX.W + "val", list.NumId)))),
                    new XElement(XName.Get("r", DocX.W.NamespaceName), new XElement(XName.Get("t", DocX.W.NamespaceName), listText))
                    );

                if (trackChanges)
                    newParagraphSection = CreateEdit(EditType.Ins, DateTime.Now, newParagraphSection);

                if (startNumber == null)
                {
                    list.AddItem(new Paragraph(list.Document, newParagraphSection, 0, ContainerType.Paragraph));
                }
                else
                {
                    list.AddItemWithStartValue(new Paragraph(list.Document, newParagraphSection, 0, ContainerType.Paragraph), (int)startNumber);
                }
            }

            return list;
        }

        internal static void RenumberIDs(DocX document)
        {
            IEnumerable<XAttribute> trackerIDs =
                            (from d in document.MainDoc.Descendants()
                             where d.Name.LocalName == "ins" || d.Name.LocalName == "del"
                             select d.Attribute(XName.Get("id", "http://schemas.openxmlformats.org/wordprocessingml/2006/main")));

            for (int i = 0; i < trackerIDs.Count(); i++)
                trackerIDs.ElementAt(i).Value = i.ToString();
        }

        internal static Paragraph GetFirstParagraphEffectedByInsert(DocX document, int index)
        {
            // This document contains no Paragraphs and insertion is at index 0
            if (document.Paragraphs.Count() == 0 && index == 0)
                return null;

            foreach (Paragraph p in document.Paragraphs)
            {
                if (p.EndIndex >= index)
                    return p;
            }

            throw new ArgumentOutOfRangeException();
        }

        internal static List<XElement> FormatInput(string text, XElement rPr)
        {
            List<XElement> newRuns = new List<XElement>();
            XElement tabRun = new XElement(DocX.W + "tab");
            XElement breakRun = new XElement(DocX.W + "br");

            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(text))
            {
                return newRuns; //I dont wanna get an exception if text == null, so just return empy list
            }
            
            char lastChar = '\0';

            foreach (char c in text)
            {
                switch (c)
                {
                    case '\t':
                        if (sb.Length > 0)
                        {
                            XElement t = new XElement(DocX.W + "t", sb.ToString());
                            Novacode.Text.PreserveSpace(t);
                            newRuns.Add(new XElement(DocX.W + "r", rPr, t));
                            sb = new StringBuilder();
                        }
                        newRuns.Add(new XElement(DocX.W + "r", rPr, tabRun));
                        break;
                    case '\r':
                    	if (sb.Length > 0)
                        {
                            XElement t = new XElement(DocX.W + "t", sb.ToString());
                            Novacode.Text.PreserveSpace(t);
                            newRuns.Add(new XElement(DocX.W + "r", rPr, t));
                            sb = new StringBuilder();
                        }
                        newRuns.Add(new XElement(DocX.W + "r", rPr, breakRun));
                        break;
                    case '\n':
                    	if (lastChar == '\r') break;
                    	
                        if (sb.Length > 0)
                        {
                            XElement t = new XElement(DocX.W + "t", sb.ToString());
                            Novacode.Text.PreserveSpace(t);
                            newRuns.Add(new XElement(DocX.W + "r", rPr, t));
                            sb = new StringBuilder();
                        }
                        newRuns.Add(new XElement(DocX.W + "r", rPr, breakRun));
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
                
                lastChar = c;
            }

            if (sb.Length > 0)
            {
                XElement t = new XElement(DocX.W + "t", sb.ToString());
                Novacode.Text.PreserveSpace(t);
                newRuns.Add(new XElement(DocX.W + "r", rPr, t));
            }

            return newRuns;
        }

        internal static XElement[] SplitParagraph(Paragraph p, int index)
        {
            // In this case edit dosent really matter, you have a choice.
            Run r = p.GetFirstRunEffectedByEdit(index, EditType.Ins);

            XElement[] split;
            XElement before, after;

            if (r.Xml.Parent.Name.LocalName == "ins")
            {
                split = p.SplitEdit(r.Xml.Parent, index, EditType.Ins);
                before = new XElement(p.Xml.Name, p.Xml.Attributes(), r.Xml.Parent.ElementsBeforeSelf(), split[0]);
                after = new XElement(p.Xml.Name, p.Xml.Attributes(), r.Xml.Parent.ElementsAfterSelf(), split[1]);
            }
            else if (r.Xml.Parent.Name.LocalName == "del")
            {
                split = p.SplitEdit(r.Xml.Parent, index, EditType.Del);

                before = new XElement(p.Xml.Name, p.Xml.Attributes(), r.Xml.Parent.ElementsBeforeSelf(), split[0]);
                after = new XElement(p.Xml.Name, p.Xml.Attributes(), r.Xml.Parent.ElementsAfterSelf(), split[1]);
            }
            else
            {
                split = Run.SplitRun(r, index);

                before = new XElement(p.Xml.Name, p.Xml.Attributes(), r.Xml.ElementsBeforeSelf(), split[0]);
                after = new XElement(p.Xml.Name, p.Xml.Attributes(), split[1], r.Xml.ElementsAfterSelf());
            }

            if (before.Elements().Count() == 0)
                before = null;

            if (after.Elements().Count() == 0)
                after = null;

            return new XElement[] { before, after };
        }

        /// <!-- 
        /// Bug found and fixed by trnilse. To see the change, 
        /// please compare this release to the previous release using TFS compare.
        /// -->
        internal static bool IsSameFile(Stream streamOne, Stream streamTwo)
        {
            int file1Byte, file2Byte;

            if (streamOne.Length != streamTwo.Length)
            {
                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1Byte = streamOne.ReadByte();
                file2Byte = streamTwo.ReadByte();
            }
            while ((file1Byte == file2Byte) && (file1Byte != -1));

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.

            streamOne.Position = 0;
            streamTwo.Position = 0;

            return ((file1Byte - file2Byte) == 0);
        }

      internal static UnderlineStyle GetUnderlineStyle(string underlineStyle)
      {
        switch (underlineStyle)
        {
          case "single":
            return UnderlineStyle.SingleLine;
          case "double": 
            return UnderlineStyle.DoubleLine;
          case "thick":
            return UnderlineStyle.Thick;
          case "dotted":
            return UnderlineStyle.Dotted;
          case "dottedHeavy":
            return UnderlineStyle.DottedHeavy;
          case "dash":
            return UnderlineStyle.Dash;
          case "dashedHeavy":
            return UnderlineStyle.DashedHeavy;
          case "dashLong":
            return UnderlineStyle.DashLong;
          case "dashLongHeavy":
            return UnderlineStyle.DashLongHeavy;
          case "dotDash":
            return UnderlineStyle.DotDash;
          case "dashDotHeavy":
            return UnderlineStyle.DashDotHeavy;
          case "dotDotDash":
            return UnderlineStyle.DotDotDash;
          case "dashDotDotHeavy":
            return UnderlineStyle.DashDotDotHeavy;
          case "wave":
            return UnderlineStyle.Wave;
          case "wavyHeavy":
            return UnderlineStyle.WavyHeavy;
          case "wavyDouble":
            return UnderlineStyle.WavyDouble;
          case "words":
            return UnderlineStyle.Words;
          default: 
            return UnderlineStyle.None;
        }
      }



    }
}
