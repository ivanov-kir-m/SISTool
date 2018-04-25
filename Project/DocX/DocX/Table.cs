﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO.Packaging;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Novacode
{
    /// <summary>
    /// Represents a Table in a document.
    /// </summary>
    public class Table : InsertBeforeOrAfter
    {
        private Alignment _alignment;
        private AutoFit _autofit;
        private float[] _columnWidthsValue;
        /// <summary>
        /// Merge cells in given column starting with startRow and ending with endRow.
        /// </summary>
        /// <remarks>
        /// Added by arudoy patch: 11608
        /// </remarks>
        public void MergeCellsInColumn(int columnIndex, int startRow, int endRow)
        {
            // Check for valid start and end indexes.
            if (columnIndex < 0 || columnIndex >= ColumnCount)
                throw new IndexOutOfRangeException();

            if (startRow < 0 || endRow <= startRow || endRow >= Rows.Count)
                throw new IndexOutOfRangeException();
            // Foreach each Cell between startIndex and endIndex inclusive.
            foreach (Row row in Rows.Where((z, i) => i > startRow && i <= endRow))
            {
                Cell c = row.Cells[columnIndex];
                XElement tcPr = c.Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                if (tcPr == null)
                {
                    c.Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                    tcPr = c.Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                }

                XElement vMerge = tcPr.Element(XName.Get("vMerge", DocX.W.NamespaceName));
                if (vMerge == null)
                {
                    tcPr.SetElementValue(XName.Get("vMerge", DocX.W.NamespaceName), string.Empty);
                    vMerge = tcPr.Element(XName.Get("vMerge", DocX.W.NamespaceName));
                }
            }

            /* 
             * Get the tcPr (table cell properties) element for the first cell in this merge,
            * null will be returned if no such element exists.
             */
            XElement startTcPr;
            if (columnIndex > Rows[startRow].Cells.Count)
                startTcPr = Rows[startRow].Cells[Rows[startRow].Cells.Count - 1].Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
            else
                startTcPr = Rows[startRow].Cells[columnIndex].Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
            if (startTcPr == null)
            {
                Rows[startRow].Cells[columnIndex].Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                startTcPr = Rows[startRow].Cells[columnIndex].Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
            }

            /* 
              * Get the gridSpan element of this row,
              * null will be returned if no such element exists.
              */
            XElement startVMerge = startTcPr.Element(XName.Get("vMerge", DocX.W.NamespaceName));
            if (startVMerge == null)
            {
                startTcPr.SetElementValue(XName.Get("vMerge", DocX.W.NamespaceName), string.Empty);
                startVMerge = startTcPr.Element(XName.Get("vMerge", DocX.W.NamespaceName));
            }

            startVMerge.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), "restart");
        }

        /// <summary>
        /// Returns a list of all Paragraphs inside this container.
        /// </summary>
        /// 
        public virtual List<Paragraph> Paragraphs
        {
            get
            {
                var paragraphs = new List<Paragraph>();

                foreach (Row r in Rows)
                    paragraphs.AddRange(r.Paragraphs);

                return paragraphs;
            }
        }

        /// <summary>
        /// Returns a list of all Pictures in a Table.
        /// </summary>
        /// <example>
        /// Returns a list of all Pictures in a Table.
        /// <code>
        /// <![CDATA[
        /// // Create a document.
        /// using (DocX document = DocX.Load(@"Test.docx"))
        /// {
        ///     // Get the first Table in a document.
        ///     Table t = document.Tables[0];
        ///
        ///     // Get all of the Pictures in this Table.
        ///     List<Picture> pictures = t.Pictures;
        ///
        ///     // Save this document.
        ///     document.Save();
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public List<Picture> Pictures
        {
            get
            {
                var pictures = new List<Picture>();

                foreach (Row r in Rows)
                    pictures.AddRange(r.Pictures);

                return pictures;
            }
        }

        /// <summary>
        /// Set the direction of all content in this Table.
        /// </summary>
        /// <param name="direction">(Left to Right) or (Right to Left)</param>
        /// <example>
        /// Set the content direction for all content in a table to RightToLeft.
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"Test.docx"))
        /// {
        ///     // Get the first table in a document.
        ///     Table table = document.Tables[0];
        ///
        ///     // Set the content direction for all content in this table to RightToLeft.
        ///     table.SetDirection(Direction.RightToLeft);
        ///    
        ///     // Save all changes made to this document.
        ///     document.Save();
        /// }
        /// </code>
        /// </example>
        public void SetDirection(Direction direction)
        {
            XElement tblPr = GetOrCreate_tblPr();
            tblPr.Add(new XElement(DocX.W + "bidiVisual"));

            foreach (Row r in Rows)
                r.SetDirection(direction);
        }

        /// <summary>
        /// Get all of the Hyperlinks in this Table.
        /// </summary>
        /// <example>
        /// Get all of the Hyperlinks in this Table.
        /// <code>
        /// // Create a document.
        /// using (DocX document = DocX.Load(@"Test.docx"))
        /// {
        ///     // Get the first Table in this document.
        ///     Table t = document.Tables[0];
        ///
        ///     // Get a list of all Hyperlinks in this Table.
        ///     List&lt;Hyperlink&gt; hyperlinks = t.Hyperlinks;
        ///
        ///     // Save this document.
        ///     document.Save();
        /// }
        /// </code>
        /// </example>
        public List<Hyperlink> Hyperlinks
        {
            get
            {
                var hyperlinks = new List<Hyperlink>();

                foreach (Row r in Rows)
                    hyperlinks.AddRange(r.Hyperlinks);

                return hyperlinks;
            }
        }

        public void SetWidths(float[] widths)
        {
            this._columnWidthsValue = widths;
            //set widths for existing rows
            foreach (var r in Rows)
            {
                for (var c = 0; c < widths.Length; c++)
                {
                    if (r.Cells.Count > c)
                        r.Cells[c].Width = widths[c];
                }

            }
        }

        /// <summary> 
        /// Set Table column width by prescribing percent 
        /// </summary> 
        /// <param name="widthsPercentage">column width % list</param> 
        /// <param name="totalWidth">Total table width. Will be calculated if null sent.</param>
        public void SetWidthsPercentage(float[] widthsPercentage, float? totalWidth)
        {
            if (totalWidth == null) totalWidth = this.Document.PageWidth - this.Document.MarginLeft - this.Document.MarginRight; // calculate total table width 
            List<float> widths = new List<float>(widthsPercentage.Length); // empty list, will hold actual width 
            widthsPercentage.ToList().ForEach(pWidth => { widths.Add((pWidth * totalWidth.Value / 100) * (96 / 72)); }); // convert percentage to actual width for all values in array 
            SetWidths(widths.ToArray()); // set actual column width
        }


        /// <summary>
        /// If the tblPr element doesent exist it is created, either way it is returned by this function.
        /// </summary>
        /// <returns>The tblPr element for this Table.</returns>
        internal XElement GetOrCreate_tblPr()
        {
            // Get the element.
            XElement tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));

            // If it dosen't exist, create it.
            if (tblPr == null)
            {
                Xml.AddFirst(new XElement(XName.Get("tblPr", DocX.W.NamespaceName)));
                tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
            }

            // Return the pPr element for this Paragraph.
            return tblPr;
        }

#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
        /// <summary>
        /// Set the specified cell margin for the table-level.
        /// </summary>
        /// <param name="type">The side of the cell margin.</param>
        /// <param name="margin">The value for the specified cell margin.</param>
        /// <remarks>More information can be found <see cref="http://msdn.microsoft.com/en-us/library/documentformat.openxml.wordprocessing.tablecellmargindefault.aspx">here</see></remarks>
        public void SetTableCellMargin(TableCellMarginType type, double margin)
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
        {
            XElement tblPr = GetOrCreate_tblPr();

            // find (or create) the element with the cell margins 
            XElement tblCellMar = tblPr.Element(XName.Get("tblCellMar", DocX.W.NamespaceName));
            if (tblCellMar == null)
            {
                tblPr.AddFirst(new XElement(XName.Get("tblCellMar", DocX.W.NamespaceName)));
                tblCellMar = tblPr.Element(XName.Get("tblCellMar", DocX.W.NamespaceName));
            }

            // find (or create) the element with cell margin for the specified side
            XElement tblMargin = tblCellMar.Element(XName.Get(type.ToString(), DocX.W.NamespaceName));
            if (tblMargin == null)
            {
                tblCellMar.AddFirst(new XElement(XName.Get(type.ToString(), DocX.W.NamespaceName)));
                tblMargin = tblCellMar.Element(XName.Get(type.ToString(), DocX.W.NamespaceName));
            }

            tblMargin.RemoveAttributes();
            // set the value for the cell margin
            tblMargin.Add(new XAttribute(XName.Get("w", DocX.W.NamespaceName), margin));
            // set the side of cell margin
            tblMargin.Add(new XAttribute(XName.Get("type", DocX.W.NamespaceName), "dxa"));
        }

        /// <summary>
        /// Gets the column width for a given column index.
        /// </summary>
        /// <param name="index"></param>
        public Double GetColumnWidth(Int32 index)
        {
            List<Double> widths = ColumnWidths;
            if (widths == null || index > widths.Count - 1) return Double.NaN;

            return widths[index];
        }

        /// <summary>
        /// Sets the column width for the given index.
        /// </summary>
        /// <param name="index">Column index</param>
        /// <param name="width">Colum width</param>
        public void SetColumnWidth(Int32 index, Double width)
        {
            List<Double> widths = ColumnWidths;
            if (widths == null || index > widths.Count - 1)
            {
                if (Rows.Count == 0) throw new Exception("There is at least one row required to detect the existing columns.");
                // use width of last row cells
                // may not work for merged cell! 
                widths = new List<Double>();
                foreach (Cell c in Rows[Rows.Count - 1].Cells)
                {
                    widths.Add(c.Width);
                }
            }

            // check if index is matching table columns
            if (index > widths.Count - 1) throw new Exception("The index is greather than the available table columns.");

            // get the table grid props
            XElement grid = Xml.Element(XName.Get("tblGrid", DocX.W.NamespaceName));
            // if null; append a new grid below tblPr
            if (grid == null)
            {
                XElement tblPr = GetOrCreate_tblPr();
                tblPr.AddAfterSelf(new XElement(XName.Get("tblGrid", DocX.W.NamespaceName)));
                grid = Xml.Element(XName.Get("tblGrid", DocX.W.NamespaceName));
            }

            // remove all existing values
            grid?.RemoveAll();

            // append new column widths
            Int32 i = 0;
            foreach (var w in widths)
            {
                double value = w;
                if (i == index) value = width;
                var gridCol = new XElement(XName.Get("gridCol", DocX.W.NamespaceName),
                    new XAttribute(XName.Get("w", DocX.W.NamespaceName), value));
                grid?.Add(gridCol);
                i += 1;
            }

            // remove cell widths
            foreach (Row r in Rows)
                foreach (Cell c in r.Cells)
                    c.Width = -1;

            // set fitting to fixed; this will add/set additional table properties
            this.AutoFit = AutoFit.Fixed;
        }


        /// <summary>
        /// Gets a list of all column widths for this table.
        /// </summary>
        public List<Double> ColumnWidths
        {
            get
            {
                var widths = new List<Double>();
                // get the table grid props
                XElement grid = Xml.Element(XName.Get("tblGrid", DocX.W.NamespaceName));

                // get col properties
                var cols = grid?.Elements(XName.Get("gridCol", DocX.W.NamespaceName));
                if (cols == null) return null;

                foreach (var col in cols)
                {
                    string value = col.GetAttribute(XName.Get("w", DocX.W.NamespaceName));
                    widths.Add(Convert.ToDouble(value));
                }
                return widths;
            }
        }


        /// <summary>
        /// Returns the number of rows in this table.
        /// </summary>
        public Int32 RowCount
        {
            get
            {
                return Xml.Elements(XName.Get("tr", DocX.W.NamespaceName)).Count();
            }
        }

        private int _cachedColCount = -1;
        /// <summary>
        /// Returns the number of columns in this table.
        /// </summary>
        public Int32 ColumnCount
        {
            get
            {
                if (RowCount == 0)
                    return 0;
                if (_cachedColCount == -1)
                    _cachedColCount = Rows.First().ColumnCount;
                return _cachedColCount;
            }
        }

        /// <summary>
        /// Returns a list of rows in this table.
        /// </summary>
        public List<Row> Rows
        {
            get
            {
                List<Row> rows =
                (
                    from r in Xml.Elements(XName.Get("tr", DocX.W.NamespaceName))
                    select new Row(this, Document, r)
                ).ToList();

                return rows;
            }
        }

        private TableDesign _design;


        internal Table(DocX document, XElement xml)
            : base(document, xml)
        {
            _autofit = AutoFit.ColumnWidth;
            this.Xml = xml;
            this.MainPart = document.MainPart;

            XElement properties = xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));

            XElement style = properties?.Element(XName.Get("tblStyle", DocX.W.NamespaceName));
            if (style != null)
            {
                XAttribute val = style.Attribute(XName.Get("val", DocX.W.NamespaceName));

                if (val != null)
                {
                    String cleanValue = val.Value.Replace("-", string.Empty);
                    if (Enum.IsDefined(typeof(TableDesign), cleanValue))
                    {
                        _design = (TableDesign)Enum.Parse(typeof(TableDesign), cleanValue);
                    }
                    else
                    {
                        _design = TableDesign.Custom;
                    }
                }
                else
                    _design = TableDesign.None;
            }

            else
                _design = TableDesign.None;

            XElement tableLook = properties?.Element(XName.Get("tblLook", DocX.W.NamespaceName));
            if (tableLook != null)
            {
                TableLook = new TableLook
                {
                    FirstRow = tableLook.GetAttribute(XName.Get("firstRow", DocX.W.NamespaceName)) == "1",
                    LastRow = tableLook.GetAttribute(XName.Get("lastRow", DocX.W.NamespaceName)) == "1",
                    FirstColumn = tableLook.GetAttribute(XName.Get("firstColumn", DocX.W.NamespaceName)) == "1",
                    LastColumn = tableLook.GetAttribute(XName.Get("lastColumn", DocX.W.NamespaceName)) == "1",
                    NoHorizontalBanding = tableLook.GetAttribute(XName.Get("noHBand", DocX.W.NamespaceName)) == "1",
                    NoVerticalBanding = tableLook.GetAttribute(XName.Get("noVBand", DocX.W.NamespaceName)) == "1"
                };
            }

        }
        /// <summary>
        /// Extra property for Custom Table Style provided by carpfisher - Thanks
        /// </summary>
        private string _customTableDesignName;
        /// <summary>
        /// Extra property for Custom Table Style provided by carpfisher - Thanks
        /// </summary>
        public string CustomTableDesignName
        {
            set
            {
                _customTableDesignName = value;
                this.Design = TableDesign.Custom;
            }

            get
            {
                return _customTableDesignName;
            }
        }

        /// <summary>
        /// String containing the Table Caption value (the table's Alternate Text Title)
        /// </summary>
        private string _tableCaption;
        /// <summary>
        /// Gets or Sets the value of the Table Caption (Alternate Text Title) of this table. 
        /// </summary>
        public string TableCaption
        {
            set
            {
                XElement tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
                if (tblPr != null)
                {
                    XElement tblCaption =
                        tblPr.Descendants(XName.Get("tblCaption", DocX.W.NamespaceName)).FirstOrDefault();

                    if (tblCaption != null)
                        tblCaption.Remove();

                    tblCaption = new XElement(XName.Get("tblCaption", DocX.W.NamespaceName),
                        new XAttribute(XName.Get("val", DocX.W.NamespaceName), value));
                    tblPr.Add(tblCaption);
                }
            }

            get
            {
                XElement tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
                XElement caption = tblPr?.Element(XName.Get("tblCaption", DocX.W.NamespaceName));
                if (caption != null)
                {
                    _tableCaption = caption.GetAttribute(XName.Get("val", DocX.W.NamespaceName));
                }
                return _tableCaption;
            }
        }

        /// <summary>
        /// String containing the Table Description (the table's Alternate Text Description).
        /// </summary>
        private string _tableDescription;
        /// <summary>
        /// Gets or Sets the value of the Table Description (Alternate Text Description) of this table. 
        /// </summary>
        public string TableDescription
        {
            set
            {
                XElement tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
                if (tblPr != null)
                {
                    XElement tblDescription =
                        tblPr.Descendants(XName.Get("tblDescription", DocX.W.NamespaceName)).FirstOrDefault();

                    tblDescription?.Remove();

                    tblDescription = new XElement(XName.Get("tblDescription", DocX.W.NamespaceName),
                       new XAttribute(XName.Get("val", DocX.W.NamespaceName), value));
                    tblPr.Add(tblDescription);
                }
            }

            get
            {
                XElement tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
                XElement caption = tblPr?.Element(XName.Get("tblDescription", DocX.W.NamespaceName));
                if (caption != null)
                {
                    _tableDescription = caption.GetAttribute(XName.Get("val", DocX.W.NamespaceName));
                }
                return _tableDescription;
            }
        }


        public TableLook TableLook { get; set; }

        public Alignment Alignment
        {
            get { return _alignment; }
            set
            {
                string alignmentString = string.Empty;
                switch (value)
                {
                    case Alignment.Left:
                        {
                            alignmentString = "left";
                            break;
                        }

                    case Alignment.Both:
                        {
                            alignmentString = "both";
                            break;
                        }


                    case Alignment.Right:
                        {
                            alignmentString = "right";
                            break;
                        }

                    case Alignment.Center:
                        {
                            alignmentString = "center";
                            break;
                        }
                }

                XElement tblPr = Xml.Descendants(XName.Get("tblPr", DocX.W.NamespaceName)).First();
                XElement jc = tblPr.Descendants(XName.Get("jc", DocX.W.NamespaceName)).FirstOrDefault();

                jc?.Remove();

                jc = new XElement(XName.Get("jc", DocX.W.NamespaceName), new XAttribute(XName.Get("val", DocX.W.NamespaceName), alignmentString));
                tblPr.Add(jc);
                _alignment = value;
            }
        }

        /// <summary>
        /// Auto size this table according to some rule.
        /// </summary>
        /// <remarks>Added by Roger Saele, April 2012. Thank you for your contribution Roger.</remarks>
        public AutoFit AutoFit
        {
            get { return _autofit; }

            set
            {
                string tableAttributeValue = string.Empty;
                string columnAttributeValue = string.Empty;
                switch (value)
                {
                    case AutoFit.ColumnWidth:
                        {
                            tableAttributeValue = "auto";
                            columnAttributeValue = "dxa";

                            // Disable "Automatically resize to fit contents" option
                            XElement tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
                            if (tblPr != null)
                            {
                                XElement layout = tblPr.Element(XName.Get("tblLayout", DocX.W.NamespaceName));
                                if (layout == null)
                                {
                                    tblPr.Add(new XElement(XName.Get("tblLayout", DocX.W.NamespaceName)));
                                    layout = tblPr.Element(XName.Get("tblLayout", DocX.W.NamespaceName));
                                }

                                XAttribute type = layout.Attribute(XName.Get("type", DocX.W.NamespaceName));
                                if (type == null)
                                {
                                    layout.Add(new XAttribute(XName.Get("type", DocX.W.NamespaceName), String.Empty));
                                    type = layout.Attribute(XName.Get("type", DocX.W.NamespaceName));
                                }

                                type.Value = "fixed";
                            }

                            break;
                        }

                    case AutoFit.Contents:
                        {
                            tableAttributeValue = columnAttributeValue = "auto";
                            break;
                        }

                    case AutoFit.Window:
                        {
                            tableAttributeValue = columnAttributeValue = "pct";
                            break;
                        }

                    case AutoFit.Fixed:
                        // DL added - 20150816:
                        // Set fixed width for the whole table; columns width is definied in the node: tblGrid
                        {
                            tableAttributeValue = columnAttributeValue = "dxa";

                            XElement tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
                            XElement tblLayout = tblPr.Element(XName.Get("tblLayout", DocX.W.NamespaceName));

                            if (tblLayout == null)
                            {
                                XElement tmp = tblPr.Element(XName.Get("tblInd", DocX.W.NamespaceName)) ??
                                               tblPr.Element(XName.Get("tblW", DocX.W.NamespaceName));

                                tmp.AddAfterSelf(new XElement(XName.Get("tblLayout", DocX.W.NamespaceName)));
                                tmp = tblPr.Element(XName.Get("tblLayout", DocX.W.NamespaceName));
                                tmp.SetAttributeValue(XName.Get("type", DocX.W.NamespaceName), "fixed");

                                tmp = tblPr.Element(XName.Get("tblW", DocX.W.NamespaceName));
                                Double i = 0;
                                foreach (Double w in ColumnWidths)
                                    i += w;

                                tmp.SetAttributeValue(XName.Get("w", DocX.W.NamespaceName), i.ToString());


                                break;

                            }
                            else
                            {
                                var qry = from d in Xml.Descendants()
                                          let type = d.Attribute(XName.Get("type", DocX.W.NamespaceName))
                                          where (d.Name.LocalName == "tblLayout") && type != null
                                          select type;

                                foreach (XAttribute type in qry)
                                    type.Value = "fixed";


                                XElement tmp = tblPr.Element(XName.Get("tblW", DocX.W.NamespaceName));
                                Double i = 0;
                                foreach (Double w in ColumnWidths)
                                    i += w;

                                tmp.SetAttributeValue(XName.Get("w", DocX.W.NamespaceName), i.ToString());
                                break;
                            }

                        }
                }

                // Set table attributes
                var query = from d in Xml.Descendants()
                            let type = d.Attribute(XName.Get("type", DocX.W.NamespaceName))
                            where (d.Name.LocalName == "tblW") && type != null
                            select type;

                foreach (XAttribute type in query)
                    type.Value = tableAttributeValue;

                // Set column attributes
                query = from d in Xml.Descendants()
                        let type = d.Attribute(XName.Get("type", DocX.W.NamespaceName))
                        where (d.Name.LocalName == "tcW") && type != null
                        select type;

                foreach (XAttribute type in query)
                    type.Value = columnAttributeValue;

                _autofit = value;
            }
        }
        /// <summary>
        /// The design\style to apply to this table.
        /// 
        /// Patch1. Patch to code for Custom Table Style support by carpfisher
        /// </summary>
        /// <example>
        /// Example code for custom table style usage 
        /// 
        /// <code> 
        /// Novacode.DocX document = Novacode.DocX.Load(“DOC01.doc”); // load document with custom table style defined
        /// Novacode.Table t = document.AddTable(2, 2); // adds table 
        /// t.CustomTableDesignName = “MyStyle01”; // assigns Custom Table Design style to newly created table
        /// </code>
        /// </example>
        /// 
        /// 
        /// 
        public TableDesign Design
        {
            get { return _design; }
            set
            {
                XElement tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
                XElement style = tblPr.Element(XName.Get("tblStyle", DocX.W.NamespaceName));
                if (style == null)
                {
                    tblPr.Add(new XElement(XName.Get("tblStyle", DocX.W.NamespaceName)));
                    style = tblPr.Element(XName.Get("tblStyle", DocX.W.NamespaceName));
                }

                XAttribute val = style.Attribute(XName.Get("val", DocX.W.NamespaceName));
                if (val == null)
                {
                    style.Add(new XAttribute(XName.Get("val", DocX.W.NamespaceName), ""));
                    val = style.Attribute(XName.Get("val", DocX.W.NamespaceName));
                }

                _design = value;

                if (_design == TableDesign.None)
                    style.Remove();

                if (_design == TableDesign.Custom)
                {
                    if (string.IsNullOrEmpty(_customTableDesignName))
                    {
                        _design = TableDesign.None;
                        if (style != null)
                            style.Remove();

                    }
                    else
                    {
                        val.Value = _customTableDesignName;
                    }
                }
                else
                {
                    switch (_design)
                    {
                        case TableDesign.TableNormal:
                            val.Value = "TableNormal";
                            break;
                        case TableDesign.TableGrid:
                            val.Value = "TableGrid";
                            break;
                        case TableDesign.LightShading:
                            val.Value = "LightShading";
                            break;
                        case TableDesign.LightShadingAccent1:
                            val.Value = "LightShading-Accent1";
                            break;
                        case TableDesign.LightShadingAccent2:
                            val.Value = "LightShading-Accent2";
                            break;
                        case TableDesign.LightShadingAccent3:
                            val.Value = "LightShading-Accent3";
                            break;
                        case TableDesign.LightShadingAccent4:
                            val.Value = "LightShading-Accent4";
                            break;
                        case TableDesign.LightShadingAccent5:
                            val.Value = "LightShading-Accent5";
                            break;
                        case TableDesign.LightShadingAccent6:
                            val.Value = "LightShading-Accent6";
                            break;
                        case TableDesign.LightList:
                            val.Value = "LightList";
                            break;
                        case TableDesign.LightListAccent1:
                            val.Value = "LightList-Accent1";
                            break;
                        case TableDesign.LightListAccent2:
                            val.Value = "LightList-Accent2";
                            break;
                        case TableDesign.LightListAccent3:
                            val.Value = "LightList-Accent3";
                            break;
                        case TableDesign.LightListAccent4:
                            val.Value = "LightList-Accent4";
                            break;
                        case TableDesign.LightListAccent5:
                            val.Value = "LightList-Accent5";
                            break;
                        case TableDesign.LightListAccent6:
                            val.Value = "LightList-Accent6";
                            break;
                        case TableDesign.LightGrid:
                            val.Value = "LightGrid";
                            break;
                        case TableDesign.LightGridAccent1:
                            val.Value = "LightGrid-Accent1";
                            break;
                        case TableDesign.LightGridAccent2:
                            val.Value = "LightGrid-Accent2";
                            break;
                        case TableDesign.LightGridAccent3:
                            val.Value = "LightGrid-Accent3";
                            break;
                        case TableDesign.LightGridAccent4:
                            val.Value = "LightGrid-Accent4";
                            break;
                        case TableDesign.LightGridAccent5:
                            val.Value = "LightGrid-Accent5";
                            break;
                        case TableDesign.LightGridAccent6:
                            val.Value = "LightGrid-Accent6";
                            break;
                        case TableDesign.MediumShading1:
                            val.Value = "MediumShading1";
                            break;
                        case TableDesign.MediumShading1Accent1:
                            val.Value = "MediumShading1-Accent1";
                            break;
                        case TableDesign.MediumShading1Accent2:
                            val.Value = "MediumShading1-Accent2";
                            break;
                        case TableDesign.MediumShading1Accent3:
                            val.Value = "MediumShading1-Accent3";
                            break;
                        case TableDesign.MediumShading1Accent4:
                            val.Value = "MediumShading1-Accent4";
                            break;
                        case TableDesign.MediumShading1Accent5:
                            val.Value = "MediumShading1-Accent5";
                            break;
                        case TableDesign.MediumShading1Accent6:
                            val.Value = "MediumShading1-Accent6";
                            break;
                        case TableDesign.MediumShading2:
                            val.Value = "MediumShading2";
                            break;
                        case TableDesign.MediumShading2Accent1:
                            val.Value = "MediumShading2-Accent1";
                            break;
                        case TableDesign.MediumShading2Accent2:
                            val.Value = "MediumShading2-Accent2";
                            break;
                        case TableDesign.MediumShading2Accent3:
                            val.Value = "MediumShading2-Accent3";
                            break;
                        case TableDesign.MediumShading2Accent4:
                            val.Value = "MediumShading2-Accent4";
                            break;
                        case TableDesign.MediumShading2Accent5:
                            val.Value = "MediumShading2-Accent5";
                            break;
                        case TableDesign.MediumShading2Accent6:
                            val.Value = "MediumShading2-Accent6";
                            break;
                        case TableDesign.MediumList1:
                            val.Value = "MediumList1";
                            break;
                        case TableDesign.MediumList1Accent1:
                            val.Value = "MediumList1-Accent1";
                            break;
                        case TableDesign.MediumList1Accent2:
                            val.Value = "MediumList1-Accent2";
                            break;
                        case TableDesign.MediumList1Accent3:
                            val.Value = "MediumList1-Accent3";
                            break;
                        case TableDesign.MediumList1Accent4:
                            val.Value = "MediumList1-Accent4";
                            break;
                        case TableDesign.MediumList1Accent5:
                            val.Value = "MediumList1-Accent5";
                            break;
                        case TableDesign.MediumList1Accent6:
                            val.Value = "MediumList1-Accent6";
                            break;
                        case TableDesign.MediumList2:
                            val.Value = "MediumList2";
                            break;
                        case TableDesign.MediumList2Accent1:
                            val.Value = "MediumList2-Accent1";
                            break;
                        case TableDesign.MediumList2Accent2:
                            val.Value = "MediumList2-Accent2";
                            break;
                        case TableDesign.MediumList2Accent3:
                            val.Value = "MediumList2-Accent3";
                            break;
                        case TableDesign.MediumList2Accent4:
                            val.Value = "MediumList2-Accent4";
                            break;
                        case TableDesign.MediumList2Accent5:
                            val.Value = "MediumList2-Accent5";
                            break;
                        case TableDesign.MediumList2Accent6:
                            val.Value = "MediumList2-Accent6";
                            break;
                        case TableDesign.MediumGrid1:
                            val.Value = "MediumGrid1";
                            break;
                        case TableDesign.MediumGrid1Accent1:
                            val.Value = "MediumGrid1-Accent1";
                            break;
                        case TableDesign.MediumGrid1Accent2:
                            val.Value = "MediumGrid1-Accent2";
                            break;
                        case TableDesign.MediumGrid1Accent3:
                            val.Value = "MediumGrid1-Accent3";
                            break;
                        case TableDesign.MediumGrid1Accent4:
                            val.Value = "MediumGrid1-Accent4";
                            break;
                        case TableDesign.MediumGrid1Accent5:
                            val.Value = "MediumGrid1-Accent5";
                            break;
                        case TableDesign.MediumGrid1Accent6:
                            val.Value = "MediumGrid1-Accent6";
                            break;
                        case TableDesign.MediumGrid2:
                            val.Value = "MediumGrid2";
                            break;
                        case TableDesign.MediumGrid2Accent1:
                            val.Value = "MediumGrid2-Accent1";
                            break;
                        case TableDesign.MediumGrid2Accent2:
                            val.Value = "MediumGrid2-Accent2";
                            break;
                        case TableDesign.MediumGrid2Accent3:
                            val.Value = "MediumGrid2-Accent3";
                            break;
                        case TableDesign.MediumGrid2Accent4:
                            val.Value = "MediumGrid2-Accent4";
                            break;
                        case TableDesign.MediumGrid2Accent5:
                            val.Value = "MediumGrid2-Accent5";
                            break;
                        case TableDesign.MediumGrid2Accent6:
                            val.Value = "MediumGrid2-Accent6";
                            break;
                        case TableDesign.MediumGrid3:
                            val.Value = "MediumGrid3";
                            break;
                        case TableDesign.MediumGrid3Accent1:
                            val.Value = "MediumGrid3-Accent1";
                            break;
                        case TableDesign.MediumGrid3Accent2:
                            val.Value = "MediumGrid3-Accent2";
                            break;
                        case TableDesign.MediumGrid3Accent3:
                            val.Value = "MediumGrid3-Accent3";
                            break;
                        case TableDesign.MediumGrid3Accent4:
                            val.Value = "MediumGrid3-Accent4";
                            break;
                        case TableDesign.MediumGrid3Accent5:
                            val.Value = "MediumGrid3-Accent5";
                            break;
                        case TableDesign.MediumGrid3Accent6:
                            val.Value = "MediumGrid3-Accent6";
                            break;

                        case TableDesign.DarkList:
                            val.Value = "DarkList";
                            break;
                        case TableDesign.DarkListAccent1:
                            val.Value = "DarkList-Accent1";
                            break;
                        case TableDesign.DarkListAccent2:
                            val.Value = "DarkList-Accent2";
                            break;
                        case TableDesign.DarkListAccent3:
                            val.Value = "DarkList-Accent3";
                            break;
                        case TableDesign.DarkListAccent4:
                            val.Value = "DarkList-Accent4";
                            break;
                        case TableDesign.DarkListAccent5:
                            val.Value = "DarkList-Accent5";
                            break;
                        case TableDesign.DarkListAccent6:
                            val.Value = "DarkList-Accent6";
                            break;

                        case TableDesign.ColorfulShading:
                            val.Value = "ColorfulShading";
                            break;
                        case TableDesign.ColorfulShadingAccent1:
                            val.Value = "ColorfulShading-Accent1";
                            break;
                        case TableDesign.ColorfulShadingAccent2:
                            val.Value = "ColorfulShading-Accent2";
                            break;
                        case TableDesign.ColorfulShadingAccent3:
                            val.Value = "ColorfulShading-Accent3";
                            break;
                        case TableDesign.ColorfulShadingAccent4:
                            val.Value = "ColorfulShading-Accent4";
                            break;
                        case TableDesign.ColorfulShadingAccent5:
                            val.Value = "ColorfulShading-Accent5";
                            break;
                        case TableDesign.ColorfulShadingAccent6:
                            val.Value = "ColorfulShading-Accent6";
                            break;

                        case TableDesign.ColorfulList:
                            val.Value = "ColorfulList";
                            break;
                        case TableDesign.ColorfulListAccent1:
                            val.Value = "ColorfulList-Accent1";
                            break;
                        case TableDesign.ColorfulListAccent2:
                            val.Value = "ColorfulList-Accent2";
                            break;
                        case TableDesign.ColorfulListAccent3:
                            val.Value = "ColorfulList-Accent3";
                            break;
                        case TableDesign.ColorfulListAccent4:
                            val.Value = "ColorfulList-Accent4";
                            break;
                        case TableDesign.ColorfulListAccent5:
                            val.Value = "ColorfulList-Accent5";
                            break;
                        case TableDesign.ColorfulListAccent6:
                            val.Value = "ColorfulList-Accent6";
                            break;

                        case TableDesign.ColorfulGrid:
                            val.Value = "ColorfulGrid";
                            break;
                        case TableDesign.ColorfulGridAccent1:
                            val.Value = "ColorfulGrid-Accent1";
                            break;
                        case TableDesign.ColorfulGridAccent2:
                            val.Value = "ColorfulGrid-Accent2";
                            break;
                        case TableDesign.ColorfulGridAccent3:
                            val.Value = "ColorfulGrid-Accent3";
                            break;
                        case TableDesign.ColorfulGridAccent4:
                            val.Value = "ColorfulGrid-Accent4";
                            break;
                        case TableDesign.ColorfulGridAccent5:
                            val.Value = "ColorfulGrid-Accent5";
                            break;
                        case TableDesign.ColorfulGridAccent6:
                            val.Value = "ColorfulGrid-Accent6";
                            break;
                    }
                }
                if (Document.Styles == null)
                {
                    PackagePart wordStyles = Document.Package.GetPart(new Uri("/word/styles.xml", UriKind.Relative));
                    using (TextReader tr = new StreamReader(wordStyles.GetStream()))
                        Document.Styles = XDocument.Load(tr);
                }

                var tableStyle =
                (
                    from e in Document.Styles.Descendants()
                    let styleId = e.Attribute(XName.Get("styleId", DocX.W.NamespaceName))
                    where (styleId != null && styleId.Value == val.Value)
                    select e
                ).FirstOrDefault();

                if (tableStyle == null)
                {
                    XDocument externalStyleDoc = HelperFunctions.DecompressXmlResource("Novacode.Resources.styles.xml.gz");

                    var styleElement =
                    (
                        from e in externalStyleDoc.Descendants()
                        let styleId = e.Attribute(XName.Get("styleId", DocX.W.NamespaceName))
                        where (styleId != null && styleId.Value == val.Value)
                        select e
                    ).First();

                    Document.Styles.Element(XName.Get("styles", DocX.W.NamespaceName)).Add(styleElement);
                }
            }
        }

        /// <summary>
        /// Returns the index of this Table.
        /// </summary>
        /// <example>
        /// Replace the first table in this document with a new Table.
        /// <code>
        /// // Load a document into memory.
        /// using (DocX document = DocX.Load(@"Test.docx"))
        /// {
        ///     // Get the first Table in this document.
        ///     Table t = document.Tables[0];
        ///
        ///     // Get the character index of Table t in this document.
        ///     int index = t.Index;
        ///
        ///     // Remove Table t.
        ///     t.Remove();
        ///
        ///     // Insert a new Table at the original index of Table t.
        ///     Table newTable = document.InsertTable(index, 4, 4);
        ///
        ///     // Set the design of this new Table, so that we can see it.
        ///     newTable.Design = TableDesign.LightShadingAccent1;
        ///
        ///     // Save all changes made to the document.
        ///     document.Save();
        /// } // Release this document from memory.
        /// </code>
        /// </example>
        public int Index
        {
            get
            {
                int index = 0;
                IEnumerable<XElement> previous = Xml.ElementsBeforeSelf();

                foreach (XElement e in previous)
                    index += Paragraph.GetElementTextLength(e);

                return index;
            }
        }

        /// <summary>
        /// Remove this Table from this document.
        /// </summary>
        /// <example>
        /// Remove the first Table from this document.
        /// <code>
        /// // Load a document into memory.
        /// using (DocX document = DocX.Load(@"Test.docx"))
        /// {
        ///     // Get the first Table in this document.
        ///     Table t = d.Tables[0];
        ///        
        ///     // Remove this Table.
        ///     t.Remove();
        ///
        ///     // Save all changes made to the document.
        ///     document.Save();
        /// } // Release this document from memory.
        /// </code>
        /// </example>
        public void Remove()
        {
            Xml.Remove();
        }

        /// <summary>
        /// Insert a row at the end of this table.
        /// </summary>
        /// <example>
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Get the first table in this document.
        ///     Table table = document.Tables[0];
        ///        
        ///     // Insert a new row at the end of this table.
        ///     Row row = table.InsertRow();
        ///
        ///     // Loop through each cell in this new row.
        ///     foreach (Cell c in row.Cells)
        ///     {
        ///         // Set the text of each new cell to "Hello".
        ///         c.Paragraphs[0].InsertText("Hello", false);
        ///     }
        ///
        ///     // Save the document to a new file.
        ///     document.SaveAs(@"C:\Example\Test2.docx");
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        /// <returns>A new row.</returns>
        public Row InsertRow()
        {
            return InsertRow(RowCount);
        }

        /// <summary>
        /// Insert a copy of a row at the end of this table.
        /// </summary>      
        /// <returns>A new row.</returns>
        public Row InsertRow(Row row)
        {
            return InsertRow(row, RowCount);
        }

        /// <summary>
        /// Insert a column to the right of a Table.
        /// </summary>
        /// <example>
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Get the first Table in this document.
        ///     Table table = document.Tables[0];
        ///
        ///     // Insert a new column to this right of this table.
        ///     table.InsertColumn();
        ///
        ///     // Set the new columns text to "Row no."
        ///     table.Rows[0].Cells[table.ColumnCount - 1].Paragraph.InsertText("Row no.", false);
        ///
        ///     // Loop through each row in the table.
        ///     for (int i = 1; i &lt; table.Rows.Count; i++)
        ///     {
        ///         // The current row.
        ///         Row row = table.Rows[i];
        ///
        ///         // The cell in this row that belongs to the new column.
        ///         Cell cell = row.Cells[table.ColumnCount - 1];
        ///
        ///         // The first Paragraph that this cell houses.
        ///         Paragraph p = cell.Paragraphs[0];
        ///
        ///         // Insert this rows index.
        ///         p.InsertText(i.ToString(), false);
        ///     }
        ///
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public void InsertColumn()
        {
            InsertColumn(ColumnCount, true);
        }

        /// <summary>
        /// Remove the last row from this Table.
        /// </summary>
        /// <example>
        /// Remove the last row from a Table.
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Get the first table in this document.
        ///     Table table = document.Tables[0];
        ///
        ///     // Remove the last row from this table.
        ///     table.RemoveRow();
        ///
        ///     // Save the document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public void RemoveRow()
        {
            RemoveRow(RowCount - 1);
        }

        /// <summary>
        /// Remove a row from this Table.
        /// </summary>
        /// <param name="index">The row to remove.</param>
        /// <example>
        /// Remove the first row from a Table.
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Get the first table in this document.
        ///     Table table = document.Tables[0];
        ///
        ///     // Remove the first row from this table.
        ///     table.RemoveRow(0);
        ///
        ///     // Save the document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public void RemoveRow(int index)
        {
            if (index < 0 || index > RowCount - 1)
                throw new IndexOutOfRangeException();

            Rows[index].Xml.Remove();
            if (Rows.Count == 0)
                Remove();
        }

        /// <summary>
        /// Remove the last column for this Table.
        /// </summary>
        /// <example>
        /// Remove the last column from a Table.
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Get the first table in this document.
        ///     Table table = document.Tables[0];
        ///
        ///     // Remove the last column from this table.
        ///     table.RemoveColumn();
        ///
        ///     // Save the document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public void RemoveColumn()
        {
            RemoveColumn(ColumnCount - 1);
        }

        /// <summary>
        /// Remove a column from this Table.
        /// </summary>
        /// <param name="index">The column to remove.</param>
        /// <example>
        /// Remove the first column from a Table.
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Get the first table in this document.
        ///     Table table = document.Tables[0];
        ///
        ///     // Remove the first column from this table.
        ///     table.RemoveColumn(0);
        ///
        ///     // Save the document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public void RemoveColumn(int index)
        {
            if (index < 0 || index > ColumnCount - 1)
                throw new IndexOutOfRangeException();

            foreach (Row r in Rows)
                if (r.Cells.Count < ColumnCount)
                {
                    var positionIndex = 0;
                    var actualPosition = 0;
                    var gridAfterVal = 0;
                    // checks to see if there is a deleted cell                    
                    gridAfterVal = r.GridAfter;

                    // goes through iteration of cells to find the one the that contains the index number
                    foreach (Cell rowCell in r.Cells)
                    {
                        // checks if the cell has a gridspan
                        var gridSpanVal = 0;

                        if (rowCell.GridSpan != 0)
                        {
                            gridSpanVal = rowCell.GridSpan - 1;
                        }

                        // checks to see if the index is within its lowest and highest cell value
                        if ((index - gridAfterVal) >= actualPosition
                            && (index - gridAfterVal) <= (actualPosition + gridSpanVal))
                        {
                            r.Cells[positionIndex].Xml.Remove();
                            break;
                        }
                        positionIndex += 1;
                        actualPosition += gridSpanVal + 1;
                    }
                }
                else
                {
                    r.Cells[index].Xml.Remove();
                }

            _cachedColCount = -1;
        }

        /// <summary>
        /// Insert a row into this table.
        /// </summary>
        /// <example>
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Get the first table in this document.
        ///     Table table = document.Tables[0];
        ///        
        ///     // Insert a new row at index 1 in this table.
        ///     Row row = table.InsertRow(1);
        ///
        ///     // Loop through each cell in this new row.
        ///     foreach (Cell c in row.Cells)
        ///     {
        ///         // Set the text of each new cell to "Hello".
        ///         c.Paragraphs[0].InsertText("Hello", false);
        ///     }
        ///
        ///     // Save the document to a new file.
        ///     document.SaveAs(@"C:\Example\Test2.docx");
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        /// <param name="index">Index to insert row at.</param>
        /// <returns>A new Row</returns>
        public Row InsertRow(int index)
        {
            if (index < 0 || index > RowCount)
                throw new IndexOutOfRangeException();

            List<XElement> content = new List<XElement>();
            for (int i = 0; i < ColumnCount; i++)
            {
                var w = 2310d;
                if (_columnWidthsValue != null && _columnWidthsValue.Length > i)
                    w = _columnWidthsValue[i] * 15;
                XElement cell = HelperFunctions.CreateTableCell(w);
                content.Add(cell);
            }

            return InsertRow(content, index);
        }

        /// <summary>
        /// Insert a copy of a row into this table.
        /// </summary>
        /// <param name="row">Row to copy and insert.</param>
        /// <param name="index">Index to insert row at.</param>
        /// <returns>A new Row</returns>
        public Row InsertRow(Row row, int index)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            if (index < 0 || index > RowCount)
                throw new IndexOutOfRangeException();

            List<XElement> content = row.Xml.Elements(XName.Get("tc", DocX.W.NamespaceName)).Select(element => HelperFunctions.CloneElement(element)).ToList();
            return InsertRow(content, index);
        }

        private Row InsertRow(List<XElement> content, Int32 index)
        {
            Row newRow = new Row(this, Document, new XElement(XName.Get("tr", DocX.W.NamespaceName), content));

            XElement rowXml;
            if (index == Rows.Count)
            {
                rowXml = Rows.Last().Xml;
                rowXml.AddAfterSelf(newRow.Xml);
            }

            else
            {
                rowXml = Rows[index].Xml;
                rowXml.AddBeforeSelf(newRow.Xml);
            }

            return newRow;
        }

        /// <summary>
        /// Insert a column into a table.
        /// </summary>
        /// <param name="index">The index to insert the column at.</param>
        /// <param name="direction">The side in which you wish to place the colum(True right, false left)</param>
        /// <example>
        /// Insert a column to the left of a table.
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Get the first Table in this document.
        ///     Table table = document.Tables[0];
        ///
        ///     // Insert a new column to this left of this table.
        ///     table.InsertColumn(0, false);
        ///
        ///     // Set the new columns text to "Row no."
        ///     table.Rows[0].Cells[table.ColumnCount - 1].Paragraph.InsertText("Row no.", false);
        ///
        ///     // Loop through each row in the table.
        ///     for (int i = 1; i &lt; table.Rows.Count; i++)
        ///     {
        ///         // The current row.
        ///         Row row = table.Rows[i];
        ///
        ///         // The cell in this row that belongs to the new column.
        ///         Cell cell = row.Cells[table.ColumnCount - 1];
        ///
        ///         // The first Paragraph that this cell houses.
        ///         Paragraph p = cell.Paragraphs[0];
        ///
        ///         // Insert this rows index.
        ///         p.InsertText(i.ToString(), false);
        ///     }
        ///
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public void InsertColumn(int index, bool direction)
        {
            var columnCount = ColumnCount;
            if (RowCount > 0)
            {
                if (index > 0 && index <= columnCount)
                {
                    _cachedColCount = -1;
                    foreach (Row r in Rows)
                    {
                        // create cell
                        XElement cell = HelperFunctions.CreateTableCell();

                        // insert cell 
                        // checks if it is in bounds of index
                        if (r.Cells.Count < columnCount)
                        {
                            if (index >= columnCount)
                            {
                                AddCellToRow(r, cell, r.Cells.Count, direction);
                            }
                            else
                            {
                                var positionIndex = 1;
                                var actualPosition = 1;
                                var gridAfterVal = 0;
                                // checks to see if there is a deleted cell

                                gridAfterVal = r.GridAfter;

                                // goes through iteration of cells to find the one the that contains the index number
                                foreach (Cell rowCell in r.Cells)
                                {
                                    // checks if the cell has a gridspan
                                    var gridSpanVal = 0;

                                    if (rowCell.GridSpan != 0)
                                    {
                                        gridSpanVal = rowCell.GridSpan - 1;
                                    }

                                    // checks to see if the index is within its lowest and highest cell value
                                    if ((index - gridAfterVal) >= actualPosition
                                        && (index - gridAfterVal) <= (actualPosition + gridSpanVal))
                                    {
                                        bool directionTest;
                                        if (index == (actualPosition + gridSpanVal) && direction)
                                        {
                                            directionTest = true;
                                        }
                                        else
                                        {
                                            directionTest = false;
                                        }
                                        AddCellToRow(r, cell, positionIndex, directionTest);
                                        break;
                                    }
                                    positionIndex += 1;
                                    actualPosition += gridSpanVal + 1;
                                }
                            }
                        }
                        else if (r.Cells.Count == index)
                        {
                            AddCellToRow(r, cell, index, direction);
                        }
                        else
                            AddCellToRow(r, cell, index, direction);
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException("Out of index bounds, column count is " + columnCount + " you input " + index);
                }
            }
        }

        /// <summary>
        /// Adds a cell to the right or left of a cell
        /// </summary>
        /// <param name="row">is the row you are adding</param>
        /// <param name="cell">is the cell you are adding</param>
        /// <param name="index">the cell index position you are refferencing from</param>
        /// <param name="direction">which side of the cell you wish to add cell</param>

        private void AddCellToRow(Row row, XElement cell, int index, bool direction)
        {
            index -= 1;
            if (direction)
            {
                row.Cells[index].Xml.AddAfterSelf(cell);
            }
            else
            {
                row.Cells[index].Xml.AddBeforeSelf(cell);
            }
        }
        /// <summary>
        /// Deletes a cell in a row
        /// </summary>
        /// <param name="rowIndex">index of the row you want to remove the cell</param>
        /// <param name="celIndex">index of the cell you want to remove</param>
        public void DeleteAndShiftCellsLeft(int rowIndex, int celIndex)
        {
            
            var trPr = Rows[rowIndex].Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));
            if (trPr != null)
            {
                var gridAfter = trPr.Element(XName.Get("gridAfter", DocX.W.NamespaceName));
                if (gridAfter != null)
                {
                    var val = gridAfter.Attribute(XName.Get("val", DocX.W.NamespaceName));
                    val.Value = (int.Parse(val.Value) + 1).ToString();
                }
                else
                {
                    gridAfter.SetAttributeValue("val", 1);
                }
            }
            else
            {
                XElement trPrXElement = new XElement(XName.Get("trPr", DocX.W.NamespaceName));
                XElement gridAfterElement = new XElement(XName.Get("gridAfter", DocX.W.NamespaceName));
                XAttribute gridAfterValAttribute = new XAttribute(XName.Get("val", DocX.W.NamespaceName), 1);
                gridAfterElement.Add(gridAfterValAttribute);
                trPrXElement.Add(gridAfterElement);
                Rows[rowIndex].Xml.AddFirst(trPrXElement);
            }
            var columnCount = this.ColumnCount;
            if (celIndex <= this.ColumnCount && this.Rows[rowIndex].ColumnCount <= this.ColumnCount)
            {
                Rows[rowIndex].Cells[celIndex].Xml.Remove();
            }
        }

        /// <summary>
        /// Insert a page break before a Table.
        /// </summary>
        /// <example>
        /// Insert a Table and a Paragraph into a document with a page break between them.
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {              
        ///     // Insert a new Paragraph.
        ///     Paragraph p1 = document.InsertParagraph("Paragraph", false);
        ///
        ///     // Insert a new Table.
        ///     Table t1 = document.InsertTable(2, 2);
        ///     t1.Design = TableDesign.LightShadingAccent1;
        ///     
        ///     // Insert a page break before this Table.
        ///     t1.InsertPageBreakBeforeSelf();
        ///     
        ///     // Save this document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public override void InsertPageBreakBeforeSelf()
        {
            base.InsertPageBreakBeforeSelf();
        }


        /// <summary>
        /// Insert a page break after a Table.
        /// </summary>
        /// <example>
        /// Insert a Table and a Paragraph into a document with a page break between them.
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Insert a new Table.
        ///     Table t1 = document.InsertTable(2, 2);
        ///     t1.Design = TableDesign.LightShadingAccent1;
        ///        
        ///     // Insert a page break after this Table.
        ///     t1.InsertPageBreakAfterSelf();
        ///        
        ///     // Insert a new Paragraph.
        ///     Paragraph p1 = document.InsertParagraph("Paragraph", false);
        ///
        ///     // Save this document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public override void InsertPageBreakAfterSelf()
        {
            base.InsertPageBreakAfterSelf();
        }

        /// <summary>
        /// Insert a new Table before this Table, this Table can be from this document or another document.
        /// </summary>
        /// <param name="t">The Table t to be inserted</param>
        /// <returns>A new Table inserted before this Table.</returns>
        /// <example>
        /// Insert a new Table before this Table.
        /// <code>
        /// // Place holder for a Table.
        /// Table t;
        ///
        /// // Load document a.
        /// using (DocX documentA = DocX.Load(@"a.docx"))
        /// {
        ///     // Get the first Table from this document.
        ///     t = documentA.Tables[0];
        /// }
        ///
        /// // Load document b.
        /// using (DocX documentB = DocX.Load(@"b.docx"))
        /// {
        ///     // Get the first Table in document b.
        ///     Table t2 = documentB.Tables[0];
        ///
        ///     // Insert the Table from document a before this Table.
        ///     Table newTable = t2.InsertTableBeforeSelf(t);
        ///
        ///     // Save all changes made to document b.
        ///     documentB.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public override Table InsertTableBeforeSelf(Table t)
        {
            return base.InsertTableBeforeSelf(t);
        }

        /// <summary>
        /// Insert a new Table into this document before this Table.
        /// </summary>
        /// <param name="rowCount">The number of rows this Table should have.</param>
        /// <param name="columnCount">The number of columns this Table should have.</param>
        /// <returns>A new Table inserted before this Table.</returns>
        /// <example>
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     //Insert a Table into this document.
        ///     Table t = document.InsertTable(2, 2);
        ///     t.Design = TableDesign.LightShadingAccent1;
        ///     t.Alignment = Alignment.center;
        ///     
        ///     // Insert a new Table before this Table.
        ///     Table newTable = t.InsertTableBeforeSelf(2, 2);
        ///     newTable.Design = TableDesign.LightShadingAccent2;
        ///     newTable.Alignment = Alignment.center;
        ///
        ///     // Save all changes made to this document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public override Table InsertTableBeforeSelf(int rowCount, int columnCount)
        {
            return base.InsertTableBeforeSelf(rowCount, columnCount);
        }

        /// <summary>
        /// Insert a new Table after this Table, this Table can be from this document or another document.
        /// </summary>
        /// <param name="t">The Table t to be inserted</param>
        /// <returns>A new Table inserted after this Table.</returns>
        /// <example>
        /// Insert a new Table after this Table.
        /// <code>
        /// // Place holder for a Table.
        /// Table t;
        ///
        /// // Load document a.
        /// using (DocX documentA = DocX.Load(@"a.docx"))
        /// {
        ///     // Get the first Table from this document.
        ///     t = documentA.Tables[0];
        /// }
        ///
        /// // Load document b.
        /// using (DocX documentB = DocX.Load(@"b.docx"))
        /// {
        ///     // Get the first Table in document b.
        ///     Table t2 = documentB.Tables[0];
        ///
        ///     // Insert the Table from document a after this Table.
        ///     Table newTable = t2.InsertTableAfterSelf(t);
        ///
        ///     // Save all changes made to document b.
        ///     documentB.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public override Table InsertTableAfterSelf(Table t)
        {
            return base.InsertTableAfterSelf(t);
        }

        /// <summary>
        /// Insert a new Table into this document after this Table.
        /// </summary>
        /// <param name="rowCount">The number of rows this Table should have.</param>
        /// <param name="columnCount">The number of columns this Table should have.</param>
        /// <returns>A new Table inserted before this Table.</returns>
        /// <example>
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     //Insert a Table into this document.
        ///     Table t = document.InsertTable(2, 2);
        ///     t.Design = TableDesign.LightShadingAccent1;
        ///     t.Alignment = Alignment.center;
        ///     
        ///     // Insert a new Table after this Table.
        ///     Table newTable = t.InsertTableAfterSelf(2, 2);
        ///     newTable.Design = TableDesign.LightShadingAccent2;
        ///     newTable.Alignment = Alignment.center;
        ///
        ///     // Save all changes made to this document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public override Table InsertTableAfterSelf(int rowCount, int columnCount)
        {
            return base.InsertTableAfterSelf(rowCount, columnCount);
        }

        /// <summary>
        /// Insert a Paragraph before this Table, this Paragraph may have come from the same or another document.
        /// </summary>
        /// <param name="p">The Paragraph to insert.</param>
        /// <returns>The Paragraph now associated with this document.</returns>
        /// <example>
        /// Take a Paragraph from document a, and insert it into document b before this Table.
        /// <code>
        /// // Place holder for a Paragraph.
        /// Paragraph p;
        ///
        /// // Load document a.
        /// using (DocX documentA = DocX.Load(@"a.docx"))
        /// {
        ///     // Get the first paragraph from this document.
        ///     p = documentA.Paragraphs[0];
        /// }
        ///
        /// // Load document b.
        /// using (DocX documentB = DocX.Load(@"b.docx"))
        /// {
        ///     // Get the first Table in document b.
        ///     Table t = documentB.Tables[0];
        ///
        ///     // Insert the Paragraph from document a before this Table.
        ///     Paragraph newParagraph = t.InsertParagraphBeforeSelf(p);
        ///
        ///     // Save all changes made to document b.
        ///     documentB.Save();
        /// }// Release this document from memory.
        /// </code> 
        /// </example>
        public override Paragraph InsertParagraphBeforeSelf(Paragraph p)
        {
            return base.InsertParagraphBeforeSelf(p);
        }

        /// <summary>
        /// Insert a new Paragraph before this Table.
        /// </summary>
        /// <param name="text">The initial text for this new Paragraph.</param>
        /// <returns>A new Paragraph inserted before this Table.</returns>
        /// <example>
        /// Insert a new Paragraph before the first Table in this document.
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Insert a Table into this document.
        ///     Table t = document.InsertTable(2, 2);
        ///
        ///     t.InsertParagraphBeforeSelf("I was inserted before the next Table.");
        ///
        ///     // Save all changes made to this new document.
        ///     document.Save();
        ///    }// Release this new document form memory.
        /// </code>
        /// </example>
        public override Paragraph InsertParagraphBeforeSelf(string text)
        {
            return base.InsertParagraphBeforeSelf(text);
        }

        /// <summary>
        /// Insert a new Paragraph before this Table.
        /// </summary>
        /// <param name="text">The initial text for this new Paragraph.</param>
        /// <param name="trackChanges">Should this insertion be tracked as a change?</param>
        /// <returns>A new Paragraph inserted before this Table.</returns>
        /// <example>
        /// Insert a new paragraph before the first Table in this document.
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Insert a Table into this document.
        ///     Table t = document.InsertTable(2, 2);
        ///
        ///     t.InsertParagraphBeforeSelf("I was inserted before the next Table.", false);
        ///
        ///     // Save all changes made to this new document.
        ///     document.Save();
        ///    }// Release this new document form memory.
        /// </code>
        /// </example>
        public override Paragraph InsertParagraphBeforeSelf(string text, bool trackChanges)
        {
            return base.InsertParagraphBeforeSelf(text, trackChanges);
        }

        /// <summary>
        /// Insert a new Paragraph before this Table.
        /// </summary>
        /// <param name="text">The initial text for this new Paragraph.</param>
        /// <param name="trackChanges">Should this insertion be tracked as a change?</param>
        /// <param name="formatting">The formatting to apply to this insertion.</param>
        /// <returns>A new Paragraph inserted before this Table.</returns>
        /// <example>
        /// Insert a new paragraph before the first Table in this document.
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Insert a Table into this document.
        ///     Table t = document.InsertTable(2, 2);
        ///
        ///     Formatting boldFormatting = new Formatting();
        ///     boldFormatting.Bold = true;
        ///
        ///     t.InsertParagraphBeforeSelf("I was inserted before the next Table.", false, boldFormatting);
        ///
        ///     // Save all changes made to this new document.
        ///     document.Save();
        ///    }// Release this new document form memory.
        /// </code>
        /// </example>
        public override Paragraph InsertParagraphBeforeSelf(string text, bool trackChanges, Formatting formatting)
        {
            return base.InsertParagraphBeforeSelf(text, trackChanges, formatting);
        }

        /// <summary>
        /// Insert a Paragraph after this Table, this Paragraph may have come from the same or another document.
        /// </summary>
        /// <param name="p">The Paragraph to insert.</param>
        /// <returns>The Paragraph now associated with this document.</returns>
        /// <example>
        /// Take a Paragraph from document a, and insert it into document b after this Table.
        /// <code>
        /// // Place holder for a Paragraph.
        /// Paragraph p;
        ///
        /// // Load document a.
        /// using (DocX documentA = DocX.Load(@"a.docx"))
        /// {
        ///     // Get the first paragraph from this document.
        ///     p = documentA.Paragraphs[0];
        /// }
        ///
        /// // Load document b.
        /// using (DocX documentB = DocX.Load(@"b.docx"))
        /// {
        ///     // Get the first Table in document b.
        ///     Table t = documentB.Tables[0];
        ///
        ///     // Insert the Paragraph from document a after this Table.
        ///     Paragraph newParagraph = t.InsertParagraphAfterSelf(p);
        ///
        ///     // Save all changes made to document b.
        ///     documentB.Save();
        /// }// Release this document from memory.
        /// </code> 
        /// </example>
        public override Paragraph InsertParagraphAfterSelf(Paragraph p)
        {
            return base.InsertParagraphAfterSelf(p);
        }

        /// <summary>
        /// Insert a new Paragraph after this Table.
        /// </summary>
        /// <param name="text">The initial text for this new Paragraph.</param>
        /// <param name="trackChanges">Should this insertion be tracked as a change?</param>
        /// <param name="formatting">The formatting to apply to this insertion.</param>
        /// <returns>A new Paragraph inserted after this Table.</returns>
        /// <example>
        /// Insert a new paragraph after the first Table in this document.
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Insert a Table into this document.
        ///     Table t = document.InsertTable(2, 2);
        ///
        ///     Formatting boldFormatting = new Formatting();
        ///     boldFormatting.Bold = true;
        ///
        ///     t.InsertParagraphAfterSelf("I was inserted after the previous Table.", false, boldFormatting);
        ///
        ///     // Save all changes made to this new document.
        ///     document.Save();
        ///    }// Release this new document form memory.
        /// </code>
        /// </example>
        public override Paragraph InsertParagraphAfterSelf(string text, bool trackChanges, Formatting formatting)
        {
            return base.InsertParagraphAfterSelf(text, trackChanges, formatting);
        }

        /// <summary>
        /// Insert a new Paragraph after this Table.
        /// </summary>
        /// <param name="text">The initial text for this new Paragraph.</param>
        /// <param name="trackChanges">Should this insertion be tracked as a change?</param>
        /// <returns>A new Paragraph inserted after this Table.</returns>
        /// <example>
        /// Insert a new paragraph after the first Table in this document.
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Insert a Table into this document.
        ///     Table t = document.InsertTable(2, 2);
        ///
        ///     t.InsertParagraphAfterSelf("I was inserted after the previous Table.", false);
        ///
        ///     // Save all changes made to this new document.
        ///     document.Save();
        ///    }// Release this new document form memory.
        /// </code>
        /// </example>
        public override Paragraph InsertParagraphAfterSelf(string text, bool trackChanges)
        {
            return base.InsertParagraphAfterSelf(text, trackChanges);
        }

        /// <summary>
        /// Insert a new Paragraph after this Table.
        /// </summary>
        /// <param name="text">The initial text for this new Paragraph.</param>
        /// <returns>A new Paragraph inserted after this Table.</returns>
        /// <example>
        /// Insert a new Paragraph after the first Table in this document.
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Insert a Table into this document.
        ///     Table t = document.InsertTable(2, 2);
        ///
        ///     t.InsertParagraphAfterSelf("I was inserted after the previous Table.");
        ///
        ///     // Save all changes made to this new document.
        ///     document.Save();
        ///    }// Release this new document form memory.
        /// </code>
        /// </example>
        public override Paragraph InsertParagraphAfterSelf(string text)
        {
            return base.InsertParagraphAfterSelf(text);
        }

        /// <summary>
        /// Set a table border
        /// Added by lckuiper @ 20101117
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a new document.
        ///using (DocX document = DocX.Create("Test.docx"))
        ///{
        ///    // Insert a table into this document.
        ///    Table t = document.InsertTable(3, 3);
        ///
        ///    // Create a large blue border.
        ///    Border b = new Border(BorderStyle.Tcbs_single, BorderSize.seven, 0, Color.Blue);
        ///
        ///    // Set the tables Top, Bottom, Left and Right Borders to b.
        ///    t.SetBorder(TableBorderType.Top, b);
        ///    t.SetBorder(TableBorderType.Bottom, b);
        ///    t.SetBorder(TableBorderType.Left, b);
        ///    t.SetBorder(TableBorderType.Right, b);
        ///
        ///    // Save the document.
        ///    document.Save();
        ///}
        /// </code>
        /// </example>
        /// <param name="borderType">The table border to set</param>
        /// <param name="border">Border object to set the table border</param>
 		public void SetBorder(TableBorderType borderType, Border border)
        {
            /*
			 * Get the tblPr (table properties) element for this Table,
			 * null will be return if no such element exists.
			 */
            XElement tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
            if (tblPr == null)
            {
                Xml.SetElementValue(XName.Get("tblPr", DocX.W.NamespaceName), string.Empty);
                tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
            }

            /*
			 * Get the tblBorders (table borders) element for this Table,
			 * null will be return if no such element exists.
			 */
            XElement tblBorders = tblPr.Element(XName.Get("tblBorders", DocX.W.NamespaceName));
            if (tblBorders == null)
            {
                tblPr.SetElementValue(XName.Get("tblBorders", DocX.W.NamespaceName), string.Empty);
                tblBorders = tblPr.Element(XName.Get("tblBorders", DocX.W.NamespaceName));
            }

            /*
			 * Get the 'borderType' (table border) element for this Table,
			 * null will be return if no such element exists.
			 */
            var tbordertype = borderType.ToString();
            // only lower the first char of string (because of insideH and insideV)
            tbordertype = tbordertype.Substring(0, 1).ToLower() + tbordertype.Substring(1);

            XElement tblBorderType = tblBorders.Element(XName.Get(borderType.ToString(), DocX.W.NamespaceName));
            if (tblBorderType == null)
            {
                tblBorders.SetElementValue(XName.Get(tbordertype, DocX.W.NamespaceName), string.Empty);
                tblBorderType = tblBorders.Element(XName.Get(tbordertype, DocX.W.NamespaceName));
            }

            // get string value of border style
            string borderstyle = border.Tcbs.ToString().Substring(5);
            borderstyle = borderstyle.Substring(0, 1).ToLower() + borderstyle.Substring(1);

            // The val attribute is used for the border style
            tblBorderType.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), borderstyle);

            if (border.Tcbs != BorderStyle.TcbsNil)
            {
                int size;
                switch (border.Size)
                {
                    case BorderSize.One: size = 2; break;
                    case BorderSize.Two: size = 4; break;
                    case BorderSize.Three: size = 6; break;
                    case BorderSize.Four: size = 8; break;
                    case BorderSize.Five: size = 12; break;
                    case BorderSize.Six: size = 18; break;
                    case BorderSize.Seven: size = 24; break;
                    case BorderSize.Eight: size = 36; break;
                    case BorderSize.Nine: size = 48; break;
                    default: size = 2; break;
                }

                // The sz attribute is used for the border size
                tblBorderType.SetAttributeValue(XName.Get("sz", DocX.W.NamespaceName), (size).ToString());

                // The space attribute is used for the cell spacing (probably '0')
                tblBorderType.SetAttributeValue(XName.Get("space", DocX.W.NamespaceName), (border.Space).ToString());

                // The color attribute is used for the border color
                tblBorderType.SetAttributeValue(XName.Get("color", DocX.W.NamespaceName), border.Color.ToHex());
            }
        }

        /// <summary>
        /// Get a table border
        /// Added by lckuiper @ 20101117
        /// </summary>
        /// <param name="borderType">The table border to get</param>
        public Border GetBorder(TableBorderType borderType)
        {
            // instance with default border values
            Border b = new Border();

            // Get the tblPr (table properties) element for this Table,
            // null will be return if no such element exists.
            XElement tblPr = Xml.Element(XName.Get("tblPr", DocX.W.NamespaceName));
            if (tblPr == null)
            {
                // uses default border style
            }

            /*
             * Get the tblBorders (table borders) element for this Table,
             * null will be return if no such element exists.
             */
            XElement tblBorders = tblPr.Element(XName.Get("tblBorders", DocX.W.NamespaceName));
            if (tblBorders == null)
            {
                // uses default border style
            }

            /*
             * Get the 'borderType' (table border) element for this Table,
             * null will be return if no such element exists.
             */
            var tbordertype = borderType.ToString();
            // only lower the first char of string (because of insideH and insideV)
            tbordertype = tbordertype.Substring(0, 1).ToLower() + tbordertype.Substring(1);

            XElement tblBorderType = tblBorders.Element(XName.Get(tbordertype, DocX.W.NamespaceName));
            if (tblBorderType == null)
            {
                // uses default border style
            }

            // The val attribute is used for the border style
            XAttribute val = tblBorderType.Attribute(XName.Get("val", DocX.W.NamespaceName));
            // If val is null, this table contains no border information.
            if (val == null)
            {
                // uses default border style
            }
            else
            {
                try
                {
                    string bordertype = "Tcbs_" + val.Value;
                    b.Tcbs = (BorderStyle)Enum.Parse(typeof(BorderStyle), bordertype);
                }
                catch
                {
                    val.Remove();
                    // uses default border style
                }
            }

            // The sz attribute is used for the border size
            XAttribute sz = tblBorderType.Attribute(XName.Get("sz", DocX.W.NamespaceName));
            // If sz is null, this border contains no size information.
            if (sz == null)
            {
                // uses default border style
            }
            else
            {
                // If sz is not an int, something is wrong with this attributes value, so remove it
                int numericalSize;
                if (!int.TryParse(sz.Value, out numericalSize))
                    sz.Remove();
                else
                {
                    switch (numericalSize)
                    {
                        case 2: b.Size = BorderSize.One; break;
                        case 4: b.Size = BorderSize.Two; break;
                        case 6: b.Size = BorderSize.Three; break;
                        case 8: b.Size = BorderSize.Four; break;
                        case 12: b.Size = BorderSize.Five; break;
                        case 18: b.Size = BorderSize.Six; break;
                        case 24: b.Size = BorderSize.Seven; break;
                        case 36: b.Size = BorderSize.Eight; break;
                        case 48: b.Size = BorderSize.Nine; break;
                        default: b.Size = BorderSize.One; break;
                    }
                }
            }

            // The space attribute is used for the border spacing (probably '0')
            XAttribute space = tblBorderType.Attribute(XName.Get("space", DocX.W.NamespaceName));
            // If space is null, this border contains no space information.
            if (space == null)
            {
                // uses default border style
            }
            else
            {
                // If space is not an int, something is wrong with this attributes value, so remove it
                int borderspace;
                if (!int.TryParse(space.Value, out borderspace))
                {
                    space.Remove();
                    // uses default border style
                }
                else
                {
                    b.Space = borderspace;
                }
            }

            // The color attribute is used for the border color
            XAttribute color = tblBorderType.Attribute(XName.Get("color", DocX.W.NamespaceName));
            if (color == null)
            {
                // uses default border style
            }
            else
            {
                // If color is not a Color, something is wrong with this attributes value, so remove it
                try
                {
                    b.Color = ColorTranslator.FromHtml(string.Format("#{0}", color.Value));
                }
                catch
                {
                    color.Remove();
                    // uses default border style
                }
            }
            return b;
        }

    }

    /// <summary>
    /// Represents a single row in a Table.
    /// </summary>
    public class Row : Container
    {
        /// <summary>
        /// Calculates columns count in the row, taking spanned cells into account
        /// </summary>
        public Int32 ColumnCount
        {
            get
            {
                int gridSpanSum = 0;

                gridSpanSum += GridAfter;

                // Foreach each Cell between startIndex and endIndex inclusive.
                foreach (Cell c in Cells)
                {
                    if (c.GridSpan != 0)
                    {
                        gridSpanSum += c.GridSpan - 1;
                    }
                }

                // return cells count + count of spanned cells
                return Cells.Count + gridSpanSum;
            }
        }

        /// <summary>
        /// Returns the GridAfter of a row ie. The amount of cells that are deleted
        /// </summary>
        public int GridAfter
        {
            get
            {
                var gridAfterValue = 0;
                var trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));
                if (trPr != null)
                {
                    var gridAfter = trPr.Element(XName.Get("gridAfter", DocX.W.NamespaceName));
                    var val = gridAfter?.Attribute(XName.Get("val", DocX.W.NamespaceName));
                    if (val != null)
                    {
                        gridAfterValue += int.Parse(val.Value);
                    }
                }
                return gridAfterValue;
            }
        }

        /// <summary>
        /// A list of Cells in this Row.
        /// </summary>
        public List<Cell> Cells
        {
            get
            {
                List<Cell> cells =
                (
                    from c in Xml.Elements(XName.Get("tc", DocX.W.NamespaceName))
                    select new Cell(this, Document, c)
                ).ToList();

                return cells;
            }
        }

        public void Remove()
        {
            XElement table = Xml.Parent;

            Xml.Remove();
            if (!table.Elements(XName.Get("tr", DocX.W.NamespaceName)).Any())
                table.Remove();
        }

        public override ReadOnlyCollection<Paragraph> Paragraphs
        {
            get
            {
                List<Paragraph> paragraphs =
                (
                    from p in Xml.Descendants(DocX.W + "p")
                    select new Paragraph(Document, p, 0)
                ).ToList();

                foreach (Paragraph p in paragraphs)
                    p.PackagePart = Table.MainPart;

                return paragraphs.AsReadOnly();
            }
        }

        internal Table Table;
        internal Row(Table table, DocX document, XElement xml)
            : base(document, xml)
        {
            this.Table = table;
            this.MainPart = table.MainPart;
        }

        /// <summary>
        /// The property name to set when specifiying an exact height
        /// </summary>
        /// <created>Nick Kusters</created>
        private const string HRuleExact = "exact";
        /// <summary>
        /// The property name to set when specifying a minimum height
        /// </summary>
        /// <created>Nick Kusters</created>
        private const string HRuleAtLeast = "atLeast";
        /// <summary>
        /// Height in pixels. // Added by Joel, refactored by Cathal.
        /// </summary>
        public double Height
        {
            get
            {
                // Get the trPr (table row properties) element for this Row,
                // null will be return if no such element exists.
                XElement trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));

                // If trPr is null, this row contains no height information.
                // Get the trHeight element for this Row,
                // null will be return if no such element exists.
                XElement trHeight = trPr?.Element(XName.Get("trHeight", DocX.W.NamespaceName));

                // If trHeight is null, this row contains no height information.
                // Get the val attribute for this trHeight element.
                XAttribute val = trHeight?.Attribute(XName.Get("val", DocX.W.NamespaceName));

                // If w is null, this cell contains no width information.
                if (val == null)
                    return double.NaN;

                // If val is not a double, something is wrong with this attributes value, so remove it and return double.NaN;
                double heightInWordUnits;
                if (!double.TryParse(val.Value, out heightInWordUnits))
                {
                    val.Remove();
                    return double.NaN;
                }

                // 15 "word units" in one pixel
                return (heightInWordUnits / 15);
            }
            set
            {
                SetHeight(value, true);
            }
        }
        /// <summary>
        /// Helper method to set either the exact height or the min-height
        /// </summary>
        /// <param name="height">The height value to set (in pixels)</param>
        /// <param name="exact">
        /// If true, the height will be forced. 
        /// If false, it will be treated as a minimum height, auto growing past it if need be.
        /// </param>
        /// <created>Nick Kusters</created>
        private void SetHeight(double height, bool exact)
        {
            /*
             * Get the trPr (table row properties) element for this Row,
             * null will be return if no such element exists.
             */
            XElement trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));
            if (trPr == null)
            {
                Xml.SetElementValue(XName.Get("trPr", DocX.W.NamespaceName), string.Empty);
                trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));
            }

            /*
             * Get the trHeight element for this Row,
             * null will be return if no such element exists.
             */
            XElement trHeight = trPr.Element(XName.Get("trHeight", DocX.W.NamespaceName));
            if (trHeight == null)
            {
                trPr.SetElementValue(XName.Get("trHeight", DocX.W.NamespaceName), string.Empty);
                trHeight = trPr.Element(XName.Get("trHeight", DocX.W.NamespaceName));
            }

            // The hRule attribute needs to be set to exact.
            trHeight.SetAttributeValue(XName.Get("hRule", DocX.W.NamespaceName), exact ? HRuleExact : HRuleAtLeast);

            // 15 "word units" is equal to one pixel. 
            trHeight.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), (height * 15).ToString());
        }
        /// <summary>
        /// Min-Height in pixels. // Added by Nick Kusters.
        /// </summary>
        /// <remarks>
        /// Value will be treated as a minimum height, auto growing past it if need be.
        /// </remarks>
        /// <created>Nick Kusters</created>
        public double MinHeight
        {
            get
            {
                // Just return the value from the normal height property since it doesn't care if you've set an exact or minimum height.
                return Height;
            }
            set
            {
                SetHeight(value, false);
            }
        }


        /// <summary>
		/// Set to true to make this row the table header row that will be repeated on each page
		/// </summary>
		public bool TableHeader
        {
            get
            {
                XElement trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));
                if (trPr == null)
                    return false;
                XElement tblHeader = trPr.Element(XName.Get("tblHeader", DocX.W.NamespaceName));
                return tblHeader != null;
            }
            set
            {
                XElement trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));
                if (trPr == null)
                {
                    Xml.SetElementValue(XName.Get("trPr", DocX.W.NamespaceName), string.Empty);
                    trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));
                }
                XElement tblHeader = trPr.Element(XName.Get("tblHeader", DocX.W.NamespaceName));
                if (tblHeader == null && value)
                {
                    trPr.SetElementValue(XName.Get("tblHeader", DocX.W.NamespaceName), string.Empty);
                }
                if (tblHeader != null && !value)
                {
                    tblHeader.Remove();
                }
            }
        }


        /// <summary>
        /// Allow row to break across pages. 
        /// The default value is true: Word will break the contents of the row across pages. 
        /// If set to false, the contents of the row will not be split across pages, the entire row will be moved to the next page instead.
        /// </summary>
        public bool BreakAcrossPages
        {
            get
            {
                XElement trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));

                XElement trCantSplit = trPr?.Element(XName.Get("cantSplit", DocX.W.NamespaceName));

                return trCantSplit == null;
            }

            set
            {
                if (value == false)
                {
                    XElement trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));
                    if (trPr == null)
                    {
                        Xml.SetElementValue(XName.Get("trPr", DocX.W.NamespaceName), string.Empty);
                        trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));
                    }

                    XElement trCantSplit = trPr.Element(XName.Get("cantSplit", DocX.W.NamespaceName));
                    if (trCantSplit == null)
                        trPr.SetElementValue(XName.Get("cantSplit", DocX.W.NamespaceName), string.Empty);
                }
                else
                {
                    XElement trPr = Xml.Element(XName.Get("trPr", DocX.W.NamespaceName));
                    XElement trCantSplit = trPr?.Element(XName.Get("cantSplit", DocX.W.NamespaceName));
                    trCantSplit?.Remove();
                }
            }
        }

        /// <summary>
        /// Merge cells starting with startIndex and ending with endIndex.
        /// </summary>
        public void MergeCells(int startIndex, int endIndex)
        {
            // Check for valid start and end indexes.
            if (startIndex < 0 || endIndex <= startIndex || endIndex > Cells.Count + 1)
                throw new IndexOutOfRangeException();

            // The sum of all merged gridSpans.
            int gridSpanSum = 0;

            // Foreach each Cell between startIndex and endIndex inclusive.
            foreach (Cell c in Cells.Where((z, i) => i > startIndex && i <= endIndex))
            {
                XElement tcPr = c.Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                XElement gridSpan = tcPr?.Element(XName.Get("gridSpan", DocX.W.NamespaceName));
                if (gridSpan != null)
                {
                    XAttribute val = gridSpan.Attribute(XName.Get("val", DocX.W.NamespaceName));

                    int value;
                    if (val != null && int.TryParse(val.Value, out value))
                            gridSpanSum += value - 1;
                }

                // Add this cells Pragraph to the merge start Cell.
                Cells[startIndex].Xml.Add(c.Xml.Elements(XName.Get("p", DocX.W.NamespaceName)));

                // Remove this Cell.
                c.Xml.Remove();
            }

            /* 
             * Get the tcPr (table cell properties) element for the first cell in this merge,
             * null will be returned if no such element exists.
             */
            XElement startTcPr = Cells[startIndex].Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
            if (startTcPr == null)
            {
                Cells[startIndex].Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                startTcPr = Cells[startIndex].Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
            }

            /* 
             * Get the gridSpan element of this row,
             * null will be returned if no such element exists.
             */
            XElement startGridSpan = startTcPr.Element(XName.Get("gridSpan", DocX.W.NamespaceName));
            if (startGridSpan == null)
            {
                startTcPr.SetElementValue(XName.Get("gridSpan", DocX.W.NamespaceName), string.Empty);
                startGridSpan = startTcPr.Element(XName.Get("gridSpan", DocX.W.NamespaceName));
            }

            /* 
             * Get the val attribute of this row,
             * null will be returned if no such element exists.
             */
            XAttribute startVal = startGridSpan.Attribute(XName.Get("val", DocX.W.NamespaceName));

            int startValue = 0;
            if (startVal != null)
                if (int.TryParse(startVal.Value, out startValue))
                    gridSpanSum += startValue - 1;

            // Set the val attribute to the number of merged cells.
            startGridSpan.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), (gridSpanSum + (endIndex - startIndex + 1)).ToString());
        }
    }

    public class Cell : Container
    {
        internal Row Row;

        internal Cell(Row row, DocX document, XElement xml)
            : base(document, xml)
        {
            this.Row = row;
            this.MainPart = row.MainPart;
        }

        public override ReadOnlyCollection<Paragraph> Paragraphs
        {
            get
            {
                ReadOnlyCollection<Paragraph> paragraphs = base.Paragraphs;

                foreach (Paragraph p in paragraphs)
                    p.PackagePart = Row.Table.MainPart;

                return paragraphs;
            }
        }
        /// <summary>
        /// Returns the GridSpan of a specific Cell ie. How many cells are merged
        /// </summary>
        public int GridSpan
        {
            get
            {
                var gridSpanVal = 0;
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                XElement gridSpan = tcPr?.Element(XName.Get("gridSpan", DocX.W.NamespaceName));
                if (gridSpan != null)
                {
                    XAttribute val = gridSpan.Attribute(XName.Get("val", DocX.W.NamespaceName));

                    int value;
                    if (val != null && int.TryParse(val.Value, out value))
                            gridSpanVal = value;
                }
                return gridSpanVal;
            }
        }

        /// <summary>
        /// Gets or Sets this Cells vertical alignment.
        /// </summary>
        /// <!--Patch 7398 added by lckuiper on Nov 16th 2010 @ 2:23 PM-->
        /// <example>
        /// Creates a table with 3 cells and sets the vertical alignment of each to 1 of the 3 available options.
        /// <code>
        /// // Create a new document.
        ///using(DocX document = DocX.Create("Test.docx"))
        ///{
        ///    // Insert a Table into this document.
        ///    Table t = document.InsertTable(3, 1);
        ///
        ///    // Set the design of the Table such that we can easily identify cell boundaries.
        ///    t.Design = TableDesign.TableGrid;
        ///
        ///    // Set the height of the row bigger than default.
        ///    // We need to be able to see the difference in vertical cell alignment options.
        ///    t.Rows[0].Height = 100;
        ///
        ///    // Set the vertical alignment of cell0 to top.
        ///    Cell c0 = t.Rows[0].Cells[0];
        ///    c0.InsertParagraph("VerticalAlignment.Top");
        ///    c0.VerticalAlignment = VerticalAlignment.Top;
        ///
        ///    // Set the vertical alignment of cell1 to center.
        ///    Cell c1 = t.Rows[0].Cells[1];
        ///    c1.InsertParagraph("VerticalAlignment.Center");
        ///    c1.VerticalAlignment = VerticalAlignment.Center;
        ///
        ///    // Set the vertical alignment of cell2 to bottom.
        ///    Cell c2 = t.Rows[0].Cells[2];
        ///    c2.InsertParagraph("VerticalAlignment.Bottom");
        ///    c2.VerticalAlignment = VerticalAlignment.Bottom;
        ///
        ///    // Save the document.
        ///    document.Save();
        ///}
        /// </code>
        /// </example>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));

                // If tcPr is null, this cell contains no width information.
                // Get the vAlign (table cell vertical alignment) element for this Cell,
                // null will be return if no such element exists.
                XElement vAlign = tcPr?.Element(XName.Get("vAlign", DocX.W.NamespaceName));

                // If vAlign is null, this cell contains no vertical alignment information.
                // Get the val attribute of the vAlign element.
                XAttribute val = vAlign?.Attribute(XName.Get("val", DocX.W.NamespaceName));

                // If val is null, this cell contains no vAlign information.
                if (val == null)
                    return VerticalAlignment.Center;

                // If val is not a VerticalAlign enum, something is wrong with this attributes value, so remove it and return VerticalAlignment.Center;
                try
                {
                    return (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), val.Value, true);
                }

                catch
                {
                    val.Remove();
                    return VerticalAlignment.Center;
                }
            }

            set
            {
                // Get the tcPr (table cell properties) element for this Cell,
                // null will be return if no such element exists.
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                if (tcPr == null)
                {
                    Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                    tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                }

                
                // Get the vAlign (table cell vertical alignment) element for this Cell,
                // null will be return if no such element exists.
                XElement vAlign = tcPr.Element(XName.Get("vAlign", DocX.W.NamespaceName));
                if (vAlign == null)
                {
                    tcPr.SetElementValue(XName.Get("vAlign", DocX.W.NamespaceName), string.Empty);
                    vAlign = tcPr.Element(XName.Get("vAlign", DocX.W.NamespaceName));
                }

                // Set the VerticalAlignment in 'val'
                vAlign.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), value.ToString().ToLower());
            }
        }

        public Color Shading
        {
            get
            {
                // Get the tcPr (table cell properties) element for this Cell,
                // null will be return if no such element exists.
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));

                // If tcPr is null, this cell contains no Color information.
                // Get the shd (table shade) element for this Cell,
                // null will be return if no such element exists.
                XElement shd = tcPr?.Element(XName.Get("shd", DocX.W.NamespaceName));

                // If shd is null, this cell contains no Color information.
                // Get the w attribute of the tcW element.
                XAttribute fill = shd?.Attribute(XName.Get("fill", DocX.W.NamespaceName));

                // If fill is null, this cell contains no Color information.
                if (fill == null)
                    return Color.White;

                return ColorTranslator.FromHtml(string.Format("#{0}", fill.Value));
            }

            set
            {
                // Get the tcPr (table cell properties) element for this Cell,
                // null will be return if no such element exists.
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                if (tcPr == null)
                {
                    Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                    tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                }

                /*
                 * Get the shd (table shade) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement shd = tcPr.Element(XName.Get("shd", DocX.W.NamespaceName));
                if (shd == null)
                {
                    tcPr.SetElementValue(XName.Get("shd", DocX.W.NamespaceName), string.Empty);
                    shd = tcPr.Element(XName.Get("shd", DocX.W.NamespaceName));
                }

                // The val attribute needs to be set to clear
                shd.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), "clear");

                // The color attribute needs to be set to auto
                shd.SetAttributeValue(XName.Get("color", DocX.W.NamespaceName), "auto");

                // The fill attribute needs to be set to the hex for this Color.
                shd.SetAttributeValue(XName.Get("fill", DocX.W.NamespaceName), value.ToHex());
            }
        }

        /// <summary>
        /// Width in pixels. // Added by Joel, refactored by Cathal
        /// </summary>
        public double Width
        {
            get
            {
                // Get the tcPr (table cell properties) element for this Cell,
                // null will be return if no such element exists.
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));

                // If tcPr is null, this cell contains no width information.
                // Get the tcW (table cell width) element for this Cell,
                // null will be return if no such element exists.
                XElement tcW = tcPr?.Element(XName.Get("tcW", DocX.W.NamespaceName));

                // If tcW is null, this cell contains no width information.
                // Get the w attribute of the tcW element.
                XAttribute w = tcW?.Attribute(XName.Get("w", DocX.W.NamespaceName));

                // If w is null, this cell contains no width information.
                if (w == null)
                    return double.NaN;

                // If w is not a double, something is wrong with this attributes value, so remove it and return double.NaN;
                double widthInWordUnits;
                if (!double.TryParse(w.Value, out widthInWordUnits))
                {
                    w.Remove();
                    return double.NaN;
                }

                // 15 "word units" is equal to one pixel.
                return (widthInWordUnits / 15);
            }

            set
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                if (tcPr == null)
                {
                    Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                    tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                }

                /*
                 * Get the tcW (table cell width) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcW = tcPr.Element(XName.Get("tcW", DocX.W.NamespaceName));
                if (tcW == null)
                {
                    tcPr.SetElementValue(XName.Get("tcW", DocX.W.NamespaceName), string.Empty);
                    tcW = tcPr.Element(XName.Get("tcW", DocX.W.NamespaceName));
                }

                if (value == -1)
                {
                    // remove cell width; due to set on table prop.
                    tcW.Remove();
                    return;
                }

                // The type attribute needs to be set to dxa which represents "twips" or twentieths of a point. In other words, 1/1440th of an inch.
                tcW.SetAttributeValue(XName.Get("type", DocX.W.NamespaceName), "dxa");

                // 15 "word units" is equal to one pixel. 
                tcW.SetAttributeValue(XName.Get("w", DocX.W.NamespaceName), (value * 15).ToString());
            }
        }

        /// <summary>
        /// LeftMargin in pixels. // Added by lckuiper
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a new document.
        ///using (DocX document = DocX.Create("Test.docx"))
        ///{
        ///    // Insert table into this document.
        ///    Table t = document.InsertTable(3, 3);
        ///    t.Design = TableDesign.TableGrid;
        ///
        ///    // Get the center cell.
        ///    Cell center = t.Rows[1].Cells[1];
        ///
        ///    // Insert some text so that we can see the effect of the Margins.
        ///    center.Paragraphs[0].Append("Center Cell");
        ///
        ///    // Set the center cells Left, Margin to 10.
        ///    center.MarginLeft = 25;
        ///
        ///    // Save the document.
        ///    document.Save();
        ///}
        /// </code>
        /// </example>
        public double MarginLeft
        {
            get
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));

                // If tcPr is null, this cell contains no width information.
                if (tcPr == null)
                    return double.NaN;

                /*
                 * Get the tcMar
                 * 
                 */
                XElement tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));

                // If tcMar is null, this cell contains no margin information.
                // Get the left (LeftMargin) element
                XElement tcMarLeft = tcMar?.Element(XName.Get("left", DocX.W.NamespaceName));

                // If tcMarLeft is null, this cell contains no left margin information.
                // Get the w attribute of the tcMarLeft element.
                XAttribute w = tcMarLeft?.Attribute(XName.Get("w", DocX.W.NamespaceName));

                // If w is null, this cell contains no width information.
                if (w == null)
                    return double.NaN;

                // If w is not a double, something is wrong with this attributes value, so remove it and return double.NaN;
                double leftMarginInWordUnits;
                if (!double.TryParse(w.Value, out leftMarginInWordUnits))
                {
                    w.Remove();
                    return double.NaN;
                }

                // 15 "word units" is equal to one pixel.
                return (leftMarginInWordUnits / 15);
            }

            set
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                if (tcPr == null)
                {
                    Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                    tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                }

                /*
                 * Get the tcMar (table cell margin) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));
                if (tcMar == null)
                {
                    tcPr.SetElementValue(XName.Get("tcMar", DocX.W.NamespaceName), string.Empty);
                    tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));
                }

                /*
                 * Get the left (table cell left margin) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcMarLeft = tcMar.Element(XName.Get("left", DocX.W.NamespaceName));
                if (tcMarLeft == null)
                {
                    tcMar.SetElementValue(XName.Get("left", DocX.W.NamespaceName), string.Empty);
                    tcMarLeft = tcMar.Element(XName.Get("left", DocX.W.NamespaceName));
                }

                // The type attribute needs to be set to dxa which represents "twips" or twentieths of a point. In other words, 1/1440th of an inch.
                tcMarLeft.SetAttributeValue(XName.Get("type", DocX.W.NamespaceName), "dxa");

                // 15 "word units" is equal to one pixel. 
                tcMarLeft.SetAttributeValue(XName.Get("w", DocX.W.NamespaceName), (value * 15).ToString());
            }
        }

        /// <summary>
        /// RightMargin in pixels. // Added by lckuiper
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a new document.
        ///using (DocX document = DocX.Create("Test.docx"))
        ///{
        ///    // Insert table into this document.
        ///    Table t = document.InsertTable(3, 3);
        ///    t.Design = TableDesign.TableGrid;
        ///
        ///    // Get the center cell.
        ///    Cell center = t.Rows[1].Cells[1];
        ///
        ///    // Insert some text so that we can see the effect of the Margins.
        ///    center.Paragraphs[0].Append("Center Cell");
        ///
        ///    // Set the center cells Right, Margin to 10.
        ///    center.MarginRight = 25;
        ///
        ///    // Save the document.
        ///    document.Save();
        ///}
        /// </code>
        /// </example>
        public double MarginRight
        {
            get
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));

                // If tcPr is null, this cell contains no width information.
                if (tcPr == null)
                    return double.NaN;

                /*
                 * Get the tcMar
                 * 
                 */
                XElement tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));

                // If tcMar is null, this cell contains no margin information.
                // Get the right (RightMargin) element
                XElement tcMarRight = tcMar?.Element(XName.Get("right", DocX.W.NamespaceName));

                // If tcMarRight is null, this cell contains no right margin information.
                // Get the w attribute of the tcMarRight element.
                XAttribute w = tcMarRight?.Attribute(XName.Get("w", DocX.W.NamespaceName));

                // If w is null, this cell contains no width information.
                if (w == null)
                    return double.NaN;

                // If w is not a double, something is wrong with this attributes value, so remove it and return double.NaN;
                double rightMarginInWordUnits;
                if (!double.TryParse(w.Value, out rightMarginInWordUnits))
                {
                    w.Remove();
                    return double.NaN;
                }

                // 15 "word units" is equal to one pixel.
                return (rightMarginInWordUnits / 15);
            }

            set
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                if (tcPr == null)
                {
                    Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                    tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                }

                /*
                 * Get the tcMar (table cell margin) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));
                if (tcMar == null)
                {
                    tcPr.SetElementValue(XName.Get("tcMar", DocX.W.NamespaceName), string.Empty);
                    tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));
                }

                /*
                 * Get the right (table cell right margin) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcMarRight = tcMar.Element(XName.Get("right", DocX.W.NamespaceName));
                if (tcMarRight == null)
                {
                    tcMar.SetElementValue(XName.Get("right", DocX.W.NamespaceName), string.Empty);
                    tcMarRight = tcMar.Element(XName.Get("right", DocX.W.NamespaceName));
                }

                // The type attribute needs to be set to dxa which represents "twips" or twentieths of a point. In other words, 1/1440th of an inch.
                tcMarRight.SetAttributeValue(XName.Get("type", DocX.W.NamespaceName), "dxa");

                // 15 "word units" is equal to one pixel. 
                tcMarRight.SetAttributeValue(XName.Get("w", DocX.W.NamespaceName), (value * 15).ToString());
            }
        }

        /// <summary>
        /// TopMargin in pixels. // Added by lckuiper
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a new document.
        ///using (DocX document = DocX.Create("Test.docx"))
        ///{
        ///    // Insert table into this document.
        ///    Table t = document.InsertTable(3, 3);
        ///    t.Design = TableDesign.TableGrid;
        ///
        ///    // Get the center cell.
        ///    Cell center = t.Rows[1].Cells[1];
        ///
        ///    // Insert some text so that we can see the effect of the Margins.
        ///    center.Paragraphs[0].Append("Center Cell");
        ///
        ///    // Set the center cells Top, Margin to 10.
        ///    center.MarginTop = 25;
        ///
        ///    // Save the document.
        ///    document.Save();
        ///}
        /// </code>
        /// </example>
        public double MarginTop
        {
            get
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));

                // If tcPr is null, this cell contains no width information.
                if (tcPr == null)
                    return double.NaN;

                /*
                 * Get the tcMar
                 * 
                 */
                XElement tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));

                // If tcMar is null, this cell contains no margin information.
                // Get the top (TopMargin) element
                XElement tcMarTop = tcMar?.Element(XName.Get("top", DocX.W.NamespaceName));

                // If tcMarTop is null, this cell contains no top margin information.
                // Get the w attribute of the tcMarTop element.
                XAttribute w = tcMarTop?.Attribute(XName.Get("w", DocX.W.NamespaceName));

                // If w is null, this cell contains no width information.
                if (w == null)
                    return double.NaN;

                // If w is not a double, something is wrong with this attributes value, so remove it and return double.NaN;
                double topMarginInWordUnits;
                if (!double.TryParse(w.Value, out topMarginInWordUnits))
                {
                    w.Remove();
                    return double.NaN;
                }

                // 15 "word units" is equal to one pixel.
                return (topMarginInWordUnits / 15);
            }

            set
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                if (tcPr == null)
                {
                    Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                    tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                }

                /*
                 * Get the tcMar (table cell margin) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));
                if (tcMar == null)
                {
                    tcPr.SetElementValue(XName.Get("tcMar", DocX.W.NamespaceName), string.Empty);
                    tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));
                }

                /*
                 * Get the top (table cell top margin) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcMarTop = tcMar.Element(XName.Get("top", DocX.W.NamespaceName));
                if (tcMarTop == null)
                {
                    tcMar.SetElementValue(XName.Get("top", DocX.W.NamespaceName), string.Empty);
                    tcMarTop = tcMar.Element(XName.Get("top", DocX.W.NamespaceName));
                }

                // The type attribute needs to be set to dxa which represents "twips" or twentieths of a point. In other words, 1/1440th of an inch.
                tcMarTop.SetAttributeValue(XName.Get("type", DocX.W.NamespaceName), "dxa");

                // 15 "word units" is equal to one pixel. 
                tcMarTop.SetAttributeValue(XName.Get("w", DocX.W.NamespaceName), (value * 15).ToString());
            }
        }

        /// <summary>
        /// BottomMargin in pixels. // Added by lckuiper
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a new document.
        ///using (DocX document = DocX.Create("Test.docx"))
        ///{
        ///    // Insert table into this document.
        ///    Table t = document.InsertTable(3, 3);
        ///    t.Design = TableDesign.TableGrid;
        ///
        ///    // Get the center cell.
        ///    Cell center = t.Rows[1].Cells[1];
        ///
        ///    // Insert some text so that we can see the effect of the Margins.
        ///    center.Paragraphs[0].Append("Center Cell");
        ///
        ///    // Set the center cells Top, Margin to 10.
        ///    center.MarginBottom = 25;
        ///
        ///    // Save the document.
        ///    document.Save();
        ///}
        /// </code>
        /// </example>
        public double MarginBottom
        {
            get
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));

                // If tcPr is null, this cell contains no width information.

                /*
                 * Get the tcMar
                 * 
                 */
                XElement tcMar = tcPr?.Element(XName.Get("tcMar", DocX.W.NamespaceName));

                // If tcMar is null, this cell contains no margin information.
                if (tcMar == null)
                    return double.NaN;

                // Get the bottom (BottomMargin) element
                XElement tcMarBottom = tcMar.Element(XName.Get("bottom", DocX.W.NamespaceName));

                // If tcMarBottom is null, this cell contains no bottom margin information.

                // Get the w attribute of the tcMarBottom element.
                XAttribute w = tcMarBottom?.Attribute(XName.Get("w", DocX.W.NamespaceName));

                // If w is null, this cell contains no width information.
                if (w == null)
                    return double.NaN;

                // If w is not a double, something is wrong with this attributes value, so remove it and return double.NaN;
                double bottomMarginInWordUnits;
                if (!double.TryParse(w.Value, out bottomMarginInWordUnits))
                {
                    w.Remove();
                    return double.NaN;
                }

                // 15 "word units" is equal to one pixel.
                return (bottomMarginInWordUnits / 15);
            }

            set
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                if (tcPr == null)
                {
                    Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                    tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                }

                /*
                 * Get the tcMar (table cell margin) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));
                if (tcMar == null)
                {
                    tcPr.SetElementValue(XName.Get("tcMar", DocX.W.NamespaceName), string.Empty);
                    tcMar = tcPr.Element(XName.Get("tcMar", DocX.W.NamespaceName));
                }

                /*
                 * Get the bottom (table cell bottom margin) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcMarBottom = tcMar.Element(XName.Get("bottom", DocX.W.NamespaceName));
                if (tcMarBottom == null)
                {
                    tcMar.SetElementValue(XName.Get("bottom", DocX.W.NamespaceName), string.Empty);
                    tcMarBottom = tcMar.Element(XName.Get("bottom", DocX.W.NamespaceName));
                }

                // The type attribute needs to be set to dxa which represents "twips" or twentieths of a point. In other words, 1/1440th of an inch.
                tcMarBottom.SetAttributeValue(XName.Get("type", DocX.W.NamespaceName), "dxa");

                // 15 "word units" is equal to one pixel. 
                tcMarBottom.SetAttributeValue(XName.Get("w", DocX.W.NamespaceName), (value * 15).ToString());
            }
        }

        /// <summary>
        /// Set the table cell border
        /// Added by lckuiper @ 20101117
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a new document.
        ///using (DocX document = DocX.Create("Test.docx"))
        ///{
        ///    // Insert a table into this document.
        ///    Table t = document.InsertTable(3, 3);
        ///
        ///    // Get the center cell.
        ///    Cell center = t.Rows[1].Cells[1];
        ///
        ///    // Create a large blue border.
        ///    Border b = new Border(BorderStyle.Tcbs_single, BorderSize.seven, 0, Color.Blue);
        ///
        ///    // Set the center cells Top, Bottom, Left and Right Borders to b.
        ///    center.SetBorder(TableCellBorderType.Top, b);
        ///    center.SetBorder(TableCellBorderType.Bottom, b);
        ///    center.SetBorder(TableCellBorderType.Left, b);
        ///    center.SetBorder(TableCellBorderType.Right, b);
        ///
        ///    // Save the document.
        ///    document.Save();
        ///}
        /// </code>
        /// </example>
        /// <param name="borderType">Table Cell border to set</param>
        /// <param name="border">Border object to set the table cell border</param>
        public void SetBorder(TableCellBorderType borderType, Border border)
        {
            /*
             * Get the tcPr (table cell properties) element for this Cell,
             * null will be return if no such element exists.
             */
            XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
            if (tcPr == null)
            {
                Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
            }

            /*
             * Get the tblBorders (table cell borders) element for this Cell,
             * null will be return if no such element exists.
             */
            XElement tcBorders = tcPr.Element(XName.Get("tcBorders", DocX.W.NamespaceName));
            if (tcBorders == null)
            {
                tcPr.SetElementValue(XName.Get("tcBorders", DocX.W.NamespaceName), string.Empty);
                tcBorders = tcPr.Element(XName.Get("tcBorders", DocX.W.NamespaceName));
            }

            /*
             * Get the 'borderType' (table cell border) element for this Cell,
             * null will be return if no such element exists.
             */
            string tcbordertype;
            switch (borderType)
            {
                case TableCellBorderType.TopLeftToBottomRight:
                    tcbordertype = "tl2br";
                    break;
                case TableCellBorderType.TopRightToBottomLeft:
                    tcbordertype = "tr2bl";
                    break;
                default:
                    // enum to string
                    tcbordertype = borderType.ToString();
                    // only lower the first char of string (because of insideH and insideV)
                    tcbordertype = tcbordertype.Substring(0, 1).ToLower() + tcbordertype.Substring(1);
                    break;
            }

            XElement tcBorderType = tcBorders.Element(XName.Get(borderType.ToString(), DocX.W.NamespaceName));
            if (tcBorderType == null)
            {
                tcBorders.SetElementValue(XName.Get(tcbordertype, DocX.W.NamespaceName), string.Empty);
                tcBorderType = tcBorders.Element(XName.Get(tcbordertype, DocX.W.NamespaceName));
            }

            // get string value of border style
            string borderstyle = border.Tcbs.ToString().Substring(5);
            borderstyle = borderstyle.Substring(0, 1).ToLower() + borderstyle.Substring(1);

            // The val attribute is used for the border style
            tcBorderType.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), borderstyle);

            int size;
            switch (border.Size)
            {
                case BorderSize.One: size = 2; break;
                case BorderSize.Two: size = 4; break;
                case BorderSize.Three: size = 6; break;
                case BorderSize.Four: size = 8; break;
                case BorderSize.Five: size = 12; break;
                case BorderSize.Six: size = 18; break;
                case BorderSize.Seven: size = 24; break;
                case BorderSize.Eight: size = 36; break;
                case BorderSize.Nine: size = 48; break;
                default: size = 2; break;
            }

            // The sz attribute is used for the border size
            tcBorderType.SetAttributeValue(XName.Get("sz", DocX.W.NamespaceName), (size).ToString());

            // The space attribute is used for the cell spacing (probably '0')
            tcBorderType.SetAttributeValue(XName.Get("space", DocX.W.NamespaceName), (border.Space).ToString());

            // The color attribute is used for the border color
            tcBorderType.SetAttributeValue(XName.Get("color", DocX.W.NamespaceName), border.Color.ToHex());
        }


        /// <summary>
        /// Get a table cell border
        /// Added by lckuiper @ 20101117
        /// </summary>
        /// <param name="borderType">The table cell border to get</param>
        public Border GetBorder(TableCellBorderType borderType)
        {
            // instance with default border values
            var b = new Border();

            /*
             * Get the tcPr (table cell properties) element for this Cell,
             * null will be return if no such element exists.
             */
            XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
            if (tcPr == null)
            {
                // uses default border style
            }

            /*
             * Get the tcBorders (table cell borders) element for this Cell,
             * null will be return if no such element exists.
             */
            XElement tcBorders = tcPr.Element(XName.Get("tcBorders", DocX.W.NamespaceName));
            if (tcBorders == null)
            {
                // uses default border style
            }

            /*
             * Get the 'borderType' (cell border) element for this Cell,
             * null will be return if no such element exists.
             */
            var tcbordertype = borderType.ToString();

            switch (tcbordertype)
            {
                case "TopLeftToBottomRight":
                    tcbordertype = "tl2br";
                    break;
                case "TopRightToBottomLeft":
                    tcbordertype = "tr2bl";
                    break;
                default:
                    // only lower the first char of string (because of insideH and insideV)
                    tcbordertype = tcbordertype.Substring(0, 1).ToLower() + tcbordertype.Substring(1);
                    break;
            }

            XElement tcBorderType = tcBorders.Element(XName.Get(tcbordertype, DocX.W.NamespaceName));
            if (tcBorderType == null)
            {
                // uses default border style
            }

            // The val attribute is used for the border style
            XAttribute val = tcBorderType.Attribute(XName.Get("val", DocX.W.NamespaceName));
            // If val is null, this cell contains no border information.
            if (val == null)
            {
                // uses default border style
            }
            else
            {
                try
                {
                    string bordertype = "Tcbs_" + val.Value;
                    b.Tcbs = (BorderStyle)Enum.Parse(typeof(BorderStyle), bordertype);
                }

                catch
                {
                    val.Remove();
                    // uses default border style
                }
            }

            // The sz attribute is used for the border size
            XAttribute sz = tcBorderType.Attribute(XName.Get("sz", DocX.W.NamespaceName));
            // If sz is null, this border contains no size information.
            if (sz == null)
            {
                // uses default border style
            }
            else
            {
                // If sz is not an int, something is wrong with this attributes value, so remove it
                int numericalSize;
                if (!int.TryParse(sz.Value, out numericalSize))
                    sz.Remove();
                else
                {
                    switch (numericalSize)
                    {
                        case 2: b.Size = BorderSize.One; break;
                        case 4: b.Size = BorderSize.Two; break;
                        case 6: b.Size = BorderSize.Three; break;
                        case 8: b.Size = BorderSize.Four; break;
                        case 12: b.Size = BorderSize.Five; break;
                        case 18: b.Size = BorderSize.Six; break;
                        case 24: b.Size = BorderSize.Seven; break;
                        case 36: b.Size = BorderSize.Eight; break;
                        case 48: b.Size = BorderSize.Nine; break;
                        default: b.Size = BorderSize.One; break;
                    }
                }
            }

            // The space attribute is used for the border spacing (probably '0')
            XAttribute space = tcBorderType.Attribute(XName.Get("space", DocX.W.NamespaceName));
            // If space is null, this border contains no space information.
            if (space == null)
            {
                // uses default border style
            }
            else
            {
                // If space is not an int, something is wrong with this attributes value, so remove it
                int borderspace;
                if (!int.TryParse(space.Value, out borderspace))
                {
                    space.Remove();
                    // uses default border style
                }
                else
                {
                    b.Space = borderspace;
                }
            }

            // The color attribute is used for the border color
            XAttribute color = tcBorderType.Attribute(XName.Get("color", DocX.W.NamespaceName));
            if (color == null)
            {
                // uses default border style
            }
            else
            {
                // If color is not a Color, something is wrong with this attributes value, so remove it
                try
                {
                    b.Color = ColorTranslator.FromHtml(string.Format("#{0}", color.Value));
                }
                catch
                {
                    color.Remove();
                    // uses default border style
                }
            }
            return b;
        }

        /// <summary>
        /// Gets or Sets the fill color of this Cell.
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create("Test.docx"))
        /// {
        ///    // Insert a table into this document.
        ///    Table t = document.InsertTable(3, 3);
        ///
        ///    // Fill the first cell as Blue.
        ///    t.Rows[0].Cells[0].FillColor = Color.Blue;
        ///    // Fill the middle cell as Red.
        ///    t.Rows[1].Cells[1].FillColor = Color.Red;
        ///    // Fill the last cell as Green.
        ///    t.Rows[2].Cells[2].FillColor = Color.Green;
        ///
        ///    // Save the document.
        ///    document.Save();
        /// }
        /// </code>
        /// </example>
        public Color FillColor
        {
            get
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                XElement shd = tcPr?.Element(XName.Get("shd", DocX.W.NamespaceName));
                XAttribute fill = shd?.Attribute(XName.Get("fill", DocX.W.NamespaceName));
                if (fill == null)
                    return Color.Empty;
                int argb = Int32.Parse(fill.Value.Replace("#", ""), NumberStyles.HexNumber);
                return Color.FromArgb(argb);
            }

            set
            {
                /*
                 * Get the tcPr (table cell properties) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                if (tcPr == null)
                {
                    Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                    tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                }

                /*
                 * Get the tcW (table cell width) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement shd = tcPr.Element(XName.Get("shd", DocX.W.NamespaceName));
                if (shd == null)
                {
                    tcPr.SetElementValue(XName.Get("shd", DocX.W.NamespaceName), string.Empty);
                    shd = tcPr.Element(XName.Get("shd", DocX.W.NamespaceName));
                }

                shd.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), "clear");
                shd.SetAttributeValue(XName.Get("color", DocX.W.NamespaceName), "auto");
                shd.SetAttributeValue(XName.Get("fill", DocX.W.NamespaceName), value.ToHex());
            }
        }

        public override Table InsertTable(int rowCount, int columnCount)
        {
            Table table = base.InsertTable(rowCount, columnCount);
            table.MainPart = MainPart;
            InsertParagraph(); //Dmitchern, It is necessary to put paragraph in the end of the cell, without it MS-Word will say that the document is corrupted
            //IMPORTANT: It will be better to check all methods that work with adding anything to cells
            return table;
        }

        public TextDirection TextDirection
        {
            get
            {
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));

                // If tcPr is null, this cell contains no width information.
                XElement textDirection = tcPr?.Element(XName.Get("textDirection", DocX.W.NamespaceName));
                XAttribute val = textDirection?.Attribute(XName.Get("val", DocX.W.NamespaceName));
                if (val == null)
                    return TextDirection.Right;

                // If val is not a VerticalAlign enum, something is wrong with this attributes value, so remove it and return VerticalAlignment.Center;
                try
                {
                    return (TextDirection)Enum.Parse(typeof(TextDirection), val.Value, true);
                }
                catch
                {
                    val.Remove();
                    return TextDirection.Right;
                }
            }
            set
            {
                /*
                    * Get the tcPr (table cell properties) element for this Cell,
                    * null will be return if no such element exists.
                    */
                XElement tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                if (tcPr == null)
                {
                    Xml.SetElementValue(XName.Get("tcPr", DocX.W.NamespaceName), string.Empty);
                    tcPr = Xml.Element(XName.Get("tcPr", DocX.W.NamespaceName));
                }

                /*
                 * Get the vAlign (table cell vertical alignment) element for this Cell,
                 * null will be return if no such element exists.
                 */
                XElement textDirection = tcPr.Element(XName.Get("textDirection", DocX.W.NamespaceName));
                if (textDirection == null)
                {
                    tcPr.SetElementValue(XName.Get("textDirection", DocX.W.NamespaceName), string.Empty);
                    textDirection = tcPr.Element(XName.Get("textDirection", DocX.W.NamespaceName));
                }

                // Set the VerticalAlignment in 'val'
                textDirection.SetAttributeValue(XName.Get("val", DocX.W.NamespaceName), value.ToString());

            }
        }
    }


    public class TableLook
    {
        public bool FirstRow { get; set; }
        public bool LastRow { get; set; }
        public bool FirstColumn { get; set; }
        public bool LastColumn { get; set; }
        public bool NoHorizontalBanding { get; set; }
        public bool NoVerticalBanding { get; set; }
    }

}
