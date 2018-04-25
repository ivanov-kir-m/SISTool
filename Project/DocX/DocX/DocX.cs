using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Novacode
{
    /// <summary>
    /// Represents a document.
    /// </summary>
    public class DocX : Container, IDisposable
    {
        #region Namespaces
        static internal XNamespace W = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        static internal XNamespace Rel = "http://schemas.openxmlformats.org/package/2006/relationships";

        static internal XNamespace R = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        static internal XNamespace M = "http://schemas.openxmlformats.org/officeDocument/2006/math";
        static internal XNamespace CustomPropertiesSchema = "http://schemas.openxmlformats.org/officeDocument/2006/custom-properties";
        static internal XNamespace CustomVTypesSchema = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";

        static internal XNamespace Wp = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing";
        static internal XNamespace A = "http://schemas.openxmlformats.org/drawingml/2006/main";
        static internal XNamespace C = "http://schemas.openxmlformats.org/drawingml/2006/chart";

        static internal XNamespace V = "urn:schemas-microsoft-com:vml";

        internal static XNamespace N = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/numbering";
        #endregion

        internal const string RelationshipImage = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
        internal const string ContentTypeApplicationRelationShipXml = "application/vnd.openxmlformats-package.relationships+xml";

        internal float GetMarginAttribute(XName name)
        {
            XElement body = MainDoc.Root.Element(XName.Get("body", W.NamespaceName));
            XElement sectPr = body.Element(XName.Get("sectPr", W.NamespaceName));
            XElement pgMar = sectPr?.Element(XName.Get("pgMar", W.NamespaceName));
            XAttribute top = pgMar?.Attribute(name);
            if (top != null)
            {
                float f;
                if (float.TryParse(top.Value, out f))
                    return (int)(f / 20.0f);
            }

            return 0;
        }

        internal void SetMarginAttribute(XName xName, float value)
        {
            XElement body = MainDoc.Root.Element(XName.Get("body", W.NamespaceName));
            XElement sectPr = body.Element(XName.Get("sectPr", W.NamespaceName));
            XElement pgMar = sectPr?.Element(XName.Get("pgMar", W.NamespaceName));
            XAttribute top = pgMar?.Attribute(xName);
            top?.SetValue(value * 20);
        }

        public BookmarkCollection Bookmarks
        {
            get
            {
                BookmarkCollection bookmarks = new BookmarkCollection();
                foreach (Paragraph paragraph in Paragraphs)
                    bookmarks.AddRange(paragraph.GetBookmarks());
                return bookmarks;
            }
        }

        /// <summary>
		/// Top margin value in points. 1pt = 1/72 of an inch. Word internally writes docx using units = 1/20th of a point.
		/// </summary>
		public float MarginTop
        {
            get
            {
                return GetMarginAttribute(XName.Get("top", W.NamespaceName));
            }

            set
            {
                SetMarginAttribute(XName.Get("top", W.NamespaceName), value);
            }
        }

        /// <summary>
        /// Bottom margin value in points. 1pt = 1/72 of an inch. Word internally writes docx using units = 1/20th of a point.
        /// </summary>
        public float MarginBottom
        {
            get
            {
                return GetMarginAttribute(XName.Get("bottom", W.NamespaceName));
            }

            set
            {
                SetMarginAttribute(XName.Get("bottom", W.NamespaceName), value);
            }
        }

        /// <summary>
        /// Left margin value in points. 1pt = 1/72 of an inch. Word internally writes docx using units = 1/20th of a point.
        /// </summary>
        public float MarginLeft
        {
            get
            {
                return GetMarginAttribute(XName.Get("left", W.NamespaceName));
            }

            set
            {
                SetMarginAttribute(XName.Get("left", W.NamespaceName), value);
            }
        }

        /// <summary>
        /// Right margin value in points. 1pt = 1/72 of an inch. Word internally writes docx using units = 1/20th of a point.
        /// </summary>
        public float MarginRight
        {
            get
            {
                return GetMarginAttribute(XName.Get("right", W.NamespaceName));
            }

            set
            {
                SetMarginAttribute(XName.Get("right", W.NamespaceName), value);
            }
        }

        /// <summary>
        /// Header margin value in points. 1pt = 1/72 of an inch. Word internally writes docx using units = 1/20th of a point.
        /// </summary>
        public float MarginHeader
        {
            get
            {
                return GetMarginAttribute(XName.Get("header", W.NamespaceName));
            }

            set
            {
                SetMarginAttribute(XName.Get("header", W.NamespaceName), value);
            }
        }

        /// <summary>
        /// Footer margin value in points. 1pt = 1/72 of an inch. Word internally writes docx using units = 1/20th of a point.
        /// </summary>
        public float MarginFooter
        {
            get
            {
                return GetMarginAttribute(XName.Get("footer", W.NamespaceName));
            }

            set
            {
                SetMarginAttribute(XName.Get("footer", W.NamespaceName), value);
            }
        }

        /// <summary>
        /// Mirror Margins boolean value. True when margins has to be mirrored.
        /// </summary>
        internal bool GetMirrorMargins(XName name)
        {
            XElement body = MainDoc.Root.Element(XName.Get("body", DocX.W.NamespaceName));
            XElement sectPr = body.Element(XName.Get("sectPr", DocX.W.NamespaceName));
            if (sectPr != null)
            {
                XElement marMirror = sectPr.Element(XName.Get("mirrorMargins", DocX.W.NamespaceName));
                if (marMirror != null)
                {
                    return true;
                }
            }
            return false;

        }

        internal void SetMirrorMargins(XName name, bool value)
        {
            XElement body = MainDoc.Root.Element(XName.Get("body", DocX.W.NamespaceName));
            XElement sectPr = body.Element(XName.Get("sectPr", DocX.W.NamespaceName));
            if (sectPr != null)
            {
                XElement marMirror = sectPr.Element(XName.Get("mirrorMargins", DocX.W.NamespaceName));
                if (marMirror != null)
                {
                    if (!value)
                    {
                        marMirror.Remove();
                    }
                }
                else
                {
                    sectPr.Add(new XElement(W + "mirrorMargins", string.Empty));
                }
            }
        }

        public bool MirrorMargins
        {
            get
            {
                return GetMirrorMargins(XName.Get("mirrorMargins", DocX.W.NamespaceName));
            }

            set
            {
                SetMirrorMargins(XName.Get("mirrorMargins", DocX.W.NamespaceName), value);
            }
        }

        /// <summary>
        /// Page width value in points. 1pt = 1/72 of an inch. Word internally writes docx using units = 1/20th of a point.
        /// </summary>
        public float PageWidth
        {
            get
            {
                XElement body = MainDoc.Root.Element(XName.Get("body", W.NamespaceName));
                XElement sectPr = body.Element(XName.Get("sectPr", W.NamespaceName));
                XElement pgSz = sectPr?.Element(XName.Get("pgSz", W.NamespaceName));

                if (pgSz != null)
                {
                    XAttribute w = pgSz.Attribute(XName.Get("w", DocX.W.NamespaceName));
                    if (w != null)
                    {
                        float f;
                        if (float.TryParse(w.Value, out f))
                            return (int)(f / 20.0f);
                    }
                }

                return (12240.0f / 20.0f);
            }

            set
            {
                XElement body = MainDoc.Root.Element(XName.Get("body", W.NamespaceName));
                XElement sectPr = body?.Element(XName.Get("sectPr", W.NamespaceName));
                XElement pgSz = sectPr?.Element(XName.Get("pgSz", W.NamespaceName));
                pgSz?.SetAttributeValue(XName.Get("w", W.NamespaceName), value * 20);
            }
        }

        /// <summary>
        /// Page height value in points. 1pt = 1/72 of an inch. Word internally writes docx using units = 1/20th of a point.
        /// </summary>
        public float PageHeight
        {
            get
            {
                XElement body = MainDoc.Root.Element(XName.Get("body", W.NamespaceName));
                XElement sectPr = body.Element(XName.Get("sectPr", W.NamespaceName));
                if (sectPr != null)
                {
                    XElement pgSz = sectPr.Element(XName.Get("pgSz", W.NamespaceName));

                    if (pgSz != null)
                    {
                        XAttribute w = pgSz.Attribute(XName.Get("h", DocX.W.NamespaceName));
                        if (w != null)
                        {
                            float f;
                            if (float.TryParse(w.Value, out f))
                                return (int)(f / 20.0f);
                        }
                    }
                }

                return (15840.0f / 20.0f);
            }

            set
            {
                XElement body = MainDoc.Root.Element(XName.Get("body", W.NamespaceName));

                if (body != null)
                {
                    XElement sectPr = body.Element(XName.Get("sectPr", W.NamespaceName));

                    if (sectPr != null)
                    {
                        XElement pgSz = sectPr.Element(XName.Get("pgSz", W.NamespaceName));

                        if (pgSz != null)
                        {
                            pgSz.SetAttributeValue(XName.Get("h", W.NamespaceName), value * 20);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Returns true if any editing restrictions are imposed on this document.
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     if(document.isProtected)
        ///         Console.WriteLine("Protected");
        ///     else
        ///         Console.WriteLine("Not protected");
        ///
        ///     // Save the document.
        ///     document.Save();
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="AddProtection(EditRestrictions)"/>
        /// <seealso cref="RemoveProtection"/>
        /// <seealso cref="GetProtectionType"/>
        public bool IsProtected
        {
            get
            {
                return Settings.Descendants(XName.Get("documentProtection", W.NamespaceName)).Count() > 0;
            }
        }

        /// <summary>
        /// Returns the type of editing protection imposed on this document.
        /// </summary>
        /// <returns>The type of editing protection imposed on this document.</returns>
        /// <example>
        /// <code>
        /// Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Make sure the document is protected before checking the protection type.
        ///     if (document.isProtected)
        ///     {
        ///         EditRestrictions protection = document.GetProtectionType();
        ///         Console.WriteLine("Document is protected using " + protection.ToString());
        ///     }
        ///
        ///     else
        ///         Console.WriteLine("Document is not protected.");
        ///
        ///     // Save the document.
        ///     document.Save();
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="AddProtection(EditRestrictions)"/>
        /// <seealso cref="RemoveProtection"/>
        /// <seealso cref="IsProtected"/>
        public EditRestrictions GetProtectionType()
        {
            if (IsProtected)
            {
                XElement documentProtection = Settings.Descendants(XName.Get("documentProtection", W.NamespaceName)).FirstOrDefault();
                string editType = documentProtection.Attribute(XName.Get("edit", W.NamespaceName)).Value;
                return (EditRestrictions)Enum.Parse(typeof(EditRestrictions), editType);
            }

            return EditRestrictions.None;
        }

        /// <summary>
        /// Add editing protection to this document.
        /// </summary>
        /// <param name="er">The type of protection to add to this document.</param>
        /// <example>
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Allow no editing, only the adding of comment.
        ///     document.AddProtection(EditRestrictions.comments);
        ///
        ///     // Save the document.
        ///     document.Save();
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="RemoveProtection"/>
        /// <seealso cref="GetProtectionType"/>
        /// <seealso cref="IsProtected"/>
        public void AddProtection(EditRestrictions er)
        {
            // Call remove protection before adding a new protection element.
            RemoveProtection();

            if (er == EditRestrictions.None)
                return;

            XElement documentProtection = new XElement(XName.Get("documentProtection", W.NamespaceName));
            documentProtection.Add(new XAttribute(XName.Get("edit", W.NamespaceName), er.ToString()));
            documentProtection.Add(new XAttribute(XName.Get("enforcement", W.NamespaceName), "1"));

            Settings.Root.AddFirst(documentProtection);
        }

        public void AddProtection(EditRestrictions er, string strPassword)
        {
            // http://blogs.msdn.com/b/vsod/archive/2010/04/05/how-to-set-the-editing-restrictions-in-word-using-open-xml-sdk-2-0.aspx
            // Call remove protection before adding a new protection element.
            RemoveProtection();

            if (er == EditRestrictions.None)
                return;

            XElement documentProtection = new XElement(XName.Get("documentProtection", W.NamespaceName));
            documentProtection.Add(new XAttribute(XName.Get("edit", W.NamespaceName), er.ToString()));
            documentProtection.Add(new XAttribute(XName.Get("enforcement", W.NamespaceName), "1"));

            int[] initialCodeArray = { 0xE1F0, 0x1D0F, 0xCC9C, 0x84C0, 0x110C, 0x0E10, 0xF1CE, 0x313E, 0x1872, 0xE139, 0xD40F, 0x84F9, 0x280C, 0xA96A, 0x4EC3 };
            int[,] encryptionMatrix = new int[15, 7]
            {

        /* char 1  */ {0xAEFC, 0x4DD9, 0x9BB2, 0x2745, 0x4E8A, 0x9D14, 0x2A09},
        /* char 2  */ {0x7B61, 0xF6C2, 0xFDA5, 0xEB6B, 0xC6F7, 0x9DCF, 0x2BBF},
        /* char 3  */ {0x4563, 0x8AC6, 0x05AD, 0x0B5A, 0x16B4, 0x2D68, 0x5AD0},
        /* char 4  */ {0x0375, 0x06EA, 0x0DD4, 0x1BA8, 0x3750, 0x6EA0, 0xDD40},
        /* char 5  */ {0xD849, 0xA0B3, 0x5147, 0xA28E, 0x553D, 0xAA7A, 0x44D5},
        /* char 6  */ {0x6F45, 0xDE8A, 0xAD35, 0x4A4B, 0x9496, 0x390D, 0x721A},
        /* char 7  */ {0xEB23, 0xC667, 0x9CEF, 0x29FF, 0x53FE, 0xA7FC, 0x5FD9},
        /* char 8  */ {0x47D3, 0x8FA6, 0x0F6D, 0x1EDA, 0x3DB4, 0x7B68, 0xF6D0},
        /* char 9  */ {0xB861, 0x60E3, 0xC1C6, 0x93AD, 0x377B, 0x6EF6, 0xDDEC},
        /* char 10 */ {0x45A0, 0x8B40, 0x06A1, 0x0D42, 0x1A84, 0x3508, 0x6A10},
        /* char 11 */ {0xAA51, 0x4483, 0x8906, 0x022D, 0x045A, 0x08B4, 0x1168},
        /* char 12 */ {0x76B4, 0xED68, 0xCAF1, 0x85C3, 0x1BA7, 0x374E, 0x6E9C},
        /* char 13 */ {0x3730, 0x6E60, 0xDCC0, 0xA9A1, 0x4363, 0x86C6, 0x1DAD},
        /* char 14 */ {0x3331, 0x6662, 0xCCC4, 0x89A9, 0x0373, 0x06E6, 0x0DCC},
        /* char 15 */ {0x1021, 0x2042, 0x4084, 0x8108, 0x1231, 0x2462, 0x48C4}
            };

            // Generate the Salt
            byte[] arrSalt = new byte[16];
            RandomNumberGenerator rand = new RNGCryptoServiceProvider();
            rand.GetNonZeroBytes(arrSalt);

            //Array to hold Key Values
            byte[] generatedKey = new byte[4];

            //Maximum length of the password is 15 chars.
            int intMaxPasswordLength = 15;

            if (!String.IsNullOrEmpty(strPassword))
            {
                strPassword = strPassword.Substring(0, Math.Min(strPassword.Length, intMaxPasswordLength));

                byte[] arrByteChars = new byte[strPassword.Length];

                for (int intLoop = 0; intLoop < strPassword.Length; intLoop++)
                {
                    int intTemp = Convert.ToInt32(strPassword[intLoop]);
                    arrByteChars[intLoop] = Convert.ToByte(intTemp & 0x00FF);
                    if (arrByteChars[intLoop] == 0)
                        arrByteChars[intLoop] = Convert.ToByte((intTemp & 0xFF00) >> 8);
                }

                int intHighOrderWord = initialCodeArray[arrByteChars.Length - 1];

                for (int intLoop = 0; intLoop < arrByteChars.Length; intLoop++)
                {
                    int tmp = intMaxPasswordLength - arrByteChars.Length + intLoop;
                    for (int intBit = 0; intBit < 7; intBit++)
                    {
                        if ((arrByteChars[intLoop] & (0x0001 << intBit)) != 0)
                        {
                            intHighOrderWord ^= encryptionMatrix[tmp, intBit];
                        }
                    }
                }

                int intLowOrderWord = 0;

                // For each character in the strPassword, going backwards
                for (int intLoopChar = arrByteChars.Length - 1; intLoopChar >= 0; intLoopChar--)
                {
                    intLowOrderWord = (((intLowOrderWord >> 14) & 0x0001) | ((intLowOrderWord << 1) & 0x7FFF)) ^ arrByteChars[intLoopChar];
                }

                intLowOrderWord = (((intLowOrderWord >> 14) & 0x0001) | ((intLowOrderWord << 1) & 0x7FFF)) ^ arrByteChars.Length ^ 0xCE4B;

                // Combine the Low and High Order Word
                int intCombinedkey = (intHighOrderWord << 16) + intLowOrderWord;

                // The byte order of the result shall be reversed [Example: 0x64CEED7E becomes 7EEDCE64. end example],
                // and that value shall be hashed as defined by the attribute values.

                for (int intTemp = 0; intTemp < 4; intTemp++)
                {
                    generatedKey[intTemp] = Convert.ToByte(((uint)(intCombinedkey & (0x000000FF << (intTemp * 8)))) >> (intTemp * 8));
                }
            }

            StringBuilder sb = new StringBuilder();
            for (int intTemp = 0; intTemp < 4; intTemp++)
            {
                sb.Append(Convert.ToString(generatedKey[intTemp], 16));
            }
            generatedKey = Encoding.Unicode.GetBytes(sb.ToString().ToUpper());

            byte[] tmpArray1 = generatedKey;
            byte[] tmpArray2 = arrSalt;
            byte[] tempKey = new byte[tmpArray1.Length + tmpArray2.Length];
            Buffer.BlockCopy(tmpArray2, 0, tempKey, 0, tmpArray2.Length);
            Buffer.BlockCopy(tmpArray1, 0, tempKey, tmpArray2.Length, tmpArray1.Length);
            generatedKey = tempKey;


            int iterations = 100000;

            HashAlgorithm sha1 = new SHA1Managed();
            generatedKey = sha1.ComputeHash(generatedKey);
            byte[] iterator = new byte[4];
            for (int intTmp = 0; intTmp < iterations; intTmp++)
            {

                iterator[0] = Convert.ToByte((intTmp & 0x000000FF) >> 0);
                iterator[1] = Convert.ToByte((intTmp & 0x0000FF00) >> 8);
                iterator[2] = Convert.ToByte((intTmp & 0x00FF0000) >> 16);
                iterator[3] = Convert.ToByte((intTmp & 0xFF000000) >> 24);

                generatedKey = ConcatByteArrays(iterator, generatedKey);
                generatedKey = sha1.ComputeHash(generatedKey);
            }

            documentProtection.Add(new XAttribute(XName.Get("cryptProviderType", W.NamespaceName), "rsaFull"));
            documentProtection.Add(new XAttribute(XName.Get("cryptAlgorithmClass", W.NamespaceName), "hash"));
            documentProtection.Add(new XAttribute(XName.Get("cryptAlgorithmType", W.NamespaceName), "typeAny"));
            documentProtection.Add(new XAttribute(XName.Get("cryptAlgorithmSid", W.NamespaceName), "4"));          // SHA1
            documentProtection.Add(new XAttribute(XName.Get("cryptSpinCount", W.NamespaceName), iterations.ToString()));
            documentProtection.Add(new XAttribute(XName.Get("hash", W.NamespaceName), Convert.ToBase64String(generatedKey)));
            documentProtection.Add(new XAttribute(XName.Get("salt", W.NamespaceName), Convert.ToBase64String(arrSalt)));

            Settings.Root.AddFirst(documentProtection);
        }

        private byte[] ConcatByteArrays(byte[] array1, byte[] array2)
        {
            byte[] result = new byte[array1.Length + array2.Length];
            Buffer.BlockCopy(array2, 0, result, 0, array2.Length);
            Buffer.BlockCopy(array1, 0, result, array2.Length, array1.Length);
            return result;
        }

        /// <summary>
        /// Remove editing protection from this document.
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a new document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Remove any editing restrictions that are imposed on this document.
        ///     document.RemoveProtection();
        ///
        ///     // Save the document.
        ///     document.Save();
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="AddProtection(EditRestrictions)"/>
        /// <seealso cref="GetProtectionType"/>
        /// <seealso cref="IsProtected"/>
        public void RemoveProtection()
        {
            // Remove every node of type documentProtection.
            Settings.Descendants(XName.Get("documentProtection", W.NamespaceName)).Remove();
        }

        public PageLayout PageLayout
        {
            get
            {
                XElement sectPr = Xml.Element(XName.Get("sectPr", W.NamespaceName));
                if (sectPr == null)
                {
                    Xml.SetElementValue(XName.Get("sectPr", W.NamespaceName), string.Empty);
                    sectPr = Xml.Element(XName.Get("sectPr", W.NamespaceName));
                }

                return new PageLayout(this, sectPr);
            }
        }

        /// <summary>
        /// Returns a collection of Headers in this Document.
        /// A document typically contains three Headers.
        /// A default one (odd), one for the first page and one for even pages.
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///    // Add header support to this document.
        ///    document.AddHeaders();
        ///
        ///    // Get a collection of all headers in this document.
        ///    Headers headers = document.Headers;
        ///
        ///    // The header used for the first page of this document.
        ///    Header first = headers.first;
        ///
        ///    // The header used for odd pages of this document.
        ///    Header odd = headers.odd;
        ///
        ///    // The header used for even pages of this document.
        ///    Header even = headers.even;
        /// }
        /// </code>
        /// </example>
        public Headers Headers
        {
            get
            {
                return _headers;
            }
        }
        private Headers _headers;

        /// <summary>
        /// Returns a collection of Footers in this Document.
        /// A document typically contains three Footers.
        /// A default one (odd), one for the first page and one for even pages.
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///    // Add footer support to this document.
        ///    document.AddFooters();
        ///
        ///    // Get a collection of all footers in this document.
        ///    Footers footers = document.Footers;
        ///
        ///    // The footer used for the first page of this document.
        ///    Footer first = footers.first;
        ///
        ///    // The footer used for odd pages of this document.
        ///    Footer odd = footers.odd;
        ///
        ///    // The footer used for even pages of this document.
        ///    Footer even = footers.even;
        /// }
        /// </code>
        /// </example>
        public Footers Footers
        {
            get
            {
                return _footers;
            }
        }

        private Footers _footers;

        /// <summary>
        /// Should the Document use different Headers and Footers for odd and even pages?
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Add header support to this document.
        ///     document.AddHeaders();
        ///
        ///     // Get a collection of all headers in this document.
        ///     Headers headers = document.Headers;
        ///
        ///     // The header used for odd pages of this document.
        ///     Header odd = headers.odd;
        ///
        ///     // The header used for even pages of this document.
        ///     Header even = headers.even;
        ///
        ///     // Force the document to use a different header for odd and even pages.
        ///     document.DifferentOddAndEvenPages = true;
        ///
        ///     // Content can be added to the Headers in the same manor that it would be added to the main document.
        ///     Paragraph p1 = odd.InsertParagraph();
        ///     p1.Append("This is the odd pages header.");
        ///
        ///     Paragraph p2 = even.InsertParagraph();
        ///     p2.Append("This is the even pages header.");
        ///
        ///     // Save all changes to this document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public bool DifferentOddAndEvenPages
        {
            get
            {
                XDocument settings;
                using (TextReader tr = new StreamReader(SettingsPart.GetStream()))
                    settings = XDocument.Load(tr);

                XElement evenAndOddHeaders = settings.Root.Element(W + "evenAndOddHeaders");

                return evenAndOddHeaders != null;
            }

            set
            {
                XDocument settings;
                using (TextReader tr = new StreamReader(SettingsPart.GetStream()))
                    settings = XDocument.Load(tr);

                XElement evenAndOddHeaders = settings.Root.Element(W + "evenAndOddHeaders");
                if (evenAndOddHeaders == null)
                {
                    if (value)
                        settings.Root.AddFirst(new XElement(W + "evenAndOddHeaders"));
                }
                else
                {
                    if (!value)
                        evenAndOddHeaders.Remove();
                }

                using (TextWriter tw = new StreamWriter(new PackagePartStream(SettingsPart.GetStream())))
                    settings.Save(tw);
            }
        }

        /// <summary>
        /// Should the Document use an independent Header and Footer for the first page?
        /// </summary>
        /// <example>
        /// // Create a document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Add header support to this document.
        ///     document.AddHeaders();
        ///
        ///     // The header used for the first page of this document.
        ///     Header first = document.Headers.first;
        ///
        ///     // Force the document to use a different header for first page.
        ///     document.DifferentFirstPage = true;
        ///
        ///     // Content can be added to the Headers in the same manor that it would be added to the main document.
        ///     Paragraph p = first.InsertParagraph();
        ///     p.Append("This is the first pages header.");
        ///
        ///     // Save all changes to this document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </example>
        public bool DifferentFirstPage
        {
            get
            {
                XElement body = MainDoc.Root.Element(W + "body");
                XElement sectPr = body.Element(W + "sectPr");

                XElement titlePg = sectPr?.Element(W + "titlePg");
                return titlePg != null;
            }

            set
            {
                XElement body = MainDoc.Root.Element(W + "body");

                body.Add(new XElement(W + "sectPr", string.Empty));

                var sectPr = body.Element(W + "sectPr");

                var titlePg = sectPr.Element(W + "titlePg");
                if (titlePg == null)
                {
                    if (value)
                        sectPr.Add(new XElement(W + "titlePg", string.Empty));
                }
                else
                {
                    if (!value)
                        titlePg.Remove();
                }
            }
        }

        private Header GetHeaderByType(string type)
        {
            return (Header)GetHeaderOrFooterByType(type, true);
        }

        private Footer GetFooterByType(string type)
        {
            return (Footer)GetHeaderOrFooterByType(type, false);
        }

        private object GetHeaderOrFooterByType(string type, bool isHeader)
        {
            // Switch which handles either case Header\Footer, this just cuts down on code duplication.
            string reference = "footerReference";
            if (isHeader)
                reference = "headerReference";

            // Get the Id of the [default, even or first] [Header or Footer]

            string id =
            (
                from e in MainDoc.Descendants(XName.Get("body", W.NamespaceName)).Descendants()
                where (e.Name.LocalName == reference) && (e.Attribute(W + "type").Value == type)
                select e.Attribute(R + "id").Value
            ).LastOrDefault();

            if (id != null)
            {
                // Get the Xml file for this Header or Footer.
                Uri partUri = MainPart.GetRelationship(id).TargetUri;

                // Weird problem with PackaePart API.
                if (!partUri.OriginalString.StartsWith("/word/"))
                    partUri = new Uri("/word/" + partUri.OriginalString, UriKind.Relative);

                // Get the Part and open a stream to get the Xml file.
                PackagePart part = Package.GetPart(partUri);

                using (TextReader tr = new StreamReader(part.GetStream()))
                {
                    var doc = XDocument.Load(tr);

                    // Header and Footer extend Container.
                    Container c;
                    if (isHeader)
                        c = new Header(this, doc.Element(W + "hdr"), part);
                    else
                        c = new Footer(this, doc.Element(W + "ftr"), part);

                    return c;
                }
            }

            // If we got this far something went wrong.
            return null;
        }



        public List<Section> GetSections()
        {

            var allParas = Paragraphs;

            var parasInASection = new List<Paragraph>();
            var sections = new List<Section>();

            foreach (var para in allParas)
            {

                var sectionInPara = para.Xml.Descendants().FirstOrDefault(s => s.Name.LocalName == "sectPr");

                if (sectionInPara == null)
                {
                    parasInASection.Add(para);
                }
                else
                {
                    parasInASection.Add(para);
                    var section = new Section(Document, sectionInPara) { SectionParagraphs = parasInASection };
                    sections.Add(section);
                    parasInASection = new List<Paragraph>();
                }

            }

            XElement body = MainDoc.Root.Element(XName.Get("body", W.NamespaceName));
            XElement baseSectionXml = body.Element(XName.Get("sectPr", W.NamespaceName));
            var baseSection = new Section(Document, baseSectionXml) { SectionParagraphs = parasInASection };
            sections.Add(baseSection);

            return sections;
        }


        // Get the word\settings.xml part
        internal PackagePart SettingsPart;
        internal PackagePart EndnotesPart;
        internal PackagePart FootnotesPart;
        internal PackagePart StylesPart;
        internal PackagePart StylesWithEffectsPart;
        internal PackagePart NumberingPart;
        internal PackagePart FontTablePart;

        #region Internal variables defined foreach DocX object
        // Object representation of the .docx
        internal Package Package;

        // The mainDocument is loaded into a XDocument object for easy querying and editing
        internal XDocument MainDoc;
        internal XDocument Settings;
        internal XDocument Endnotes;
        internal XDocument Footnotes;
        internal XDocument Styles;
        internal XDocument StylesWithEffects;
        internal XDocument Numbering;
        internal XDocument FontTable;
        internal XDocument Header1;
        internal XDocument Header2;
        internal XDocument Header3;

        // A lookup for the Paragraphs in this document.
        internal Dictionary<int, Paragraph> ParagraphLookup = new Dictionary<int, Paragraph>();
        // Every document is stored in a MemoryStream, all edits made to a document are done in memory.
        internal MemoryStream MemoryStream;
        // The filename that this document was loaded from
        internal string Filename;
        // The stream that this document was loaded from
        internal Stream Stream;
        #endregion

        internal DocX(DocX document, XElement xml)
            : base(document, xml)
        {

        }

        /// <summary>
        /// Returns a list of Images in this document.
        /// </summary>
        /// <example>
        /// Get the unique Id of every Image in this document.
        /// <code>
        /// // Load a document.
        /// DocX document = DocX.Load(@"C:\Example\Test.docx");
        ///
        /// // Loop through each Image in this document.
        /// foreach (Novacode.Image i in document.Images)
        /// {
        ///     // Get the unique Id which identifies this Image.
        ///     string uniqueId = i.Id;
        /// }
        ///
        /// </code>
        /// </example>
        /// <seealso cref="AddImage(string, string)"/>
        /// <seealso cref="AddImage(System.IO.Stream, string)"/>
        /// <seealso cref="Paragraph.Pictures"/>
        /// <seealso cref="Paragraph.InsertPicture"/>
        public List<Image> Images
        {
            get
            {
                PackageRelationshipCollection imageRelationships = MainPart.GetRelationshipsByType(RelationshipImage);
                if (imageRelationships.Any())
                {
                    return
                    (
                        from i in imageRelationships
                        select new Image(this, i)
                    ).ToList();
                }

                return new List<Image>();
            }
        }

        /// <summary>
        /// Returns a list of custom properties in this document.
        /// </summary>
        /// <example>
        /// Method 1: Get the name, type and value of each CustomProperty in this document.
        /// <code>
        /// // Load Example.docx
        /// DocX document = DocX.Load(@"C:\Example\Test.docx");
        ///
        /// /*
        ///  * No two custom properties can have the same name,
        ///  * so a Dictionary is the perfect data structure to store them in.
        ///  * Each custom property can be accessed using its name.
        ///  */
        /// foreach (string name in document.CustomProperties.Keys)
        /// {
        ///     // Grab a custom property using its name.
        ///     CustomProperty cp = document.CustomProperties[name];
        ///
        ///     // Write this custom properties details to Console.
        ///     Console.WriteLine(string.Format("Name: '{0}', Value: {1}", cp.Name, cp.Value));
        /// }
        ///
        /// Console.WriteLine("Press any key...");
        ///
        /// // Wait for the user to press a key before closing the Console.
        /// Console.ReadKey();
        /// </code>
        /// </example>
        /// <example>
        /// Method 2: Get the name, type and value of each CustomProperty in this document.
        /// <code>
        /// // Load Example.docx
        /// DocX document = DocX.Load(@"C:\Example\Test.docx");
        ///
        /// /*
        ///  * No two custom properties can have the same name,
        ///  * so a Dictionary is the perfect data structure to store them in.
        ///  * The values of this Dictionary are CustomProperties.
        ///  */
        /// foreach (CustomProperty cp in document.CustomProperties.Values)
        /// {
        ///     // Write this custom properties details to Console.
        ///     Console.WriteLine(string.Format("Name: '{0}', Value: {1}", cp.Name, cp.Value));
        /// }
        ///
        /// Console.WriteLine("Press any key...");
        ///
        /// // Wait for the user to press a key before closing the Console.
        /// Console.ReadKey();
        /// </code>
        /// </example>
        /// <seealso cref="AddCustomProperty"/>
        public Dictionary<string, CustomProperty> CustomProperties
        {
            get
            {
                if (Package.PartExists(new Uri("/docProps/custom.xml", UriKind.Relative)))
                {
                    PackagePart docPropsCustom = Package.GetPart(new Uri("/docProps/custom.xml", UriKind.Relative));
                    XDocument customPropDoc;
                    using (TextReader tr = new StreamReader(docPropsCustom.GetStream(FileMode.Open, FileAccess.Read)))
                        customPropDoc = XDocument.Load(tr, LoadOptions.PreserveWhitespace);

                    // Get all of the custom properties in this document
                    return
                    (
                        from p in customPropDoc.Descendants(XName.Get("property", CustomPropertiesSchema.NamespaceName))
                        let name = p.Attribute(XName.Get("name")).Value
                        let type = p.Descendants().Single().Name.LocalName
                        let value = p.Descendants().Single().Value
                        select new CustomProperty(name, type, value)
                    ).ToDictionary(p => p.Name, StringComparer.CurrentCultureIgnoreCase);
                }

                return new Dictionary<string, CustomProperty>();
            }
        }

        ///<summary>
        /// Returns the list of document core properties with corresponding values.
        ///</summary>
        public Dictionary<string, string> CoreProperties
        {
            get
            {
                if (Package.PartExists(new Uri("/docProps/core.xml", UriKind.Relative)))
                {
                    PackagePart docPropsCore = Package.GetPart(new Uri("/docProps/core.xml", UriKind.Relative));
                    XDocument corePropDoc;
                    using (TextReader tr = new StreamReader(docPropsCore.GetStream(FileMode.Open, FileAccess.Read)))
                        corePropDoc = XDocument.Load(tr, LoadOptions.PreserveWhitespace);

                    // Get all of the core properties in this document
                    return (from docProperty in corePropDoc.Root.Elements()
                            select
                              new KeyValuePair<string, string>(
                              string.Format(
                                "{0}:{1}",
                                corePropDoc.Root.GetPrefixOfNamespace(docProperty.Name.Namespace),
                                docProperty.Name.LocalName),
                              docProperty.Value)).ToDictionary(p => p.Key, v => v.Value);
                }

                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Get the Text of this document.
        /// </summary>
        /// <example>
        /// Write to Console the Text from this document.
        /// <code>
        /// // Load a document
        /// DocX document = DocX.Load(@"C:\Example\Test.docx");
        ///
        /// // Get the text of this document.
        /// string text = document.Text;
        ///
        /// // Write the text of this document to Console.
        /// Console.Write(text);
        ///
        /// // Wait for the user to press a key before closing the console window.
        /// Console.ReadKey();
        /// </code>
        /// </example>
        public string Text
        {
            get
            {
                return HelperFunctions.GetText(Xml);
            }
        }
        /// <summary>
        /// Get the text of each footnote from this document
        /// </summary>
        public IEnumerable<string> FootnotesText
        {
            get
            {
                foreach (XElement footnote in Footnotes.Root.Elements(W + "footnote"))
                {
                    yield return HelperFunctions.GetText(footnote);
                }
            }
        }

        /// <summary>
        /// Get the text of each endnote from this document
        /// </summary>
        public IEnumerable<string> EndnotesText
        {
            get
            {
                foreach (XElement endnote in Endnotes.Root.Elements(W + "endnote"))
                {
                    yield return HelperFunctions.GetText(endnote);
                }
            }
        }



        internal string GetCollectiveText(List<PackagePart> list)
        {
            string text = string.Empty;

            foreach (var hp in list)
            {
                using (TextReader tr = new StreamReader(hp.GetStream()))
                {
                    XDocument d = XDocument.Load(tr);

                    StringBuilder sb = new StringBuilder();

                    // Loop through each text item in this run
                    foreach (XElement descendant in d.Descendants())
                    {
                        switch (descendant.Name.LocalName)
                        {
                            case "tab":
                                sb.Append("\t");
                                break;
                            case "br":
                                sb.Append("\n");
                                break;
                            case "t":
                                goto case "delText";
                            case "delText":
                                sb.Append(descendant.Value);
                                break;
                            default: break;
                        }
                    }

                    text += "\n" + sb;
                }
            }

            return text;
        }

        /// <summary>
        /// Insert the contents of another document at the end of this document.
        /// </summary>
        /// <param name="remoteDocument">The document to insert at the end of this document.</param>
		/// <param name="append">If true, document is inserted at the end, otherwise document is inserted at the beginning.</param>
        /// <example>
        /// Create a new document and insert an old document into it.
        /// <code>
        /// // Create a new document.
        /// using (DocX newDocument = DocX.Create(@"NewDocument.docx"))
        /// {
        ///     // Load an old document.
        ///     using (DocX oldDocument = DocX.Load(@"OldDocument.docx"))
        ///     {
        ///         // Insert the old document into the new document.
        ///         newDocument.InsertDocument(oldDocument);
        ///
        ///         // Save the new document.
        ///         newDocument.Save();
        ///     }// Release the old document from memory.
        /// }// Release the new document from memory.
        /// </code>
        /// <remarks>
        /// If the document being inserted contains Images, CustomProperties and or custom styles, these will be correctly inserted into the new document. In the case of Images, new ID's are generated for the Images being inserted to avoid ID conflicts. CustomProperties with the same name will be ignored not replaced.
        /// </remarks>
        /// </example>
        public void InsertDocument(DocX remoteDocument, bool append = true)
        {
            // We don't want to effect the origional XDocument, so create a new one from the old one.
            XDocument remoteMainDoc = new XDocument(remoteDocument.MainDoc);

            XDocument remoteFootnotes = null;
            if (remoteDocument.Footnotes != null)
                remoteFootnotes = new XDocument(remoteDocument.Footnotes);

            XDocument remoteEndnotes = null;
            if (remoteDocument.Endnotes != null)
                remoteEndnotes = new XDocument(remoteDocument.Endnotes);

            // Remove all header and footer references.
            remoteMainDoc.Descendants(XName.Get("headerReference", W.NamespaceName)).Remove();
            remoteMainDoc.Descendants(XName.Get("footerReference", W.NamespaceName)).Remove();

            // Get the body of the remote document.
            XElement remoteBody = remoteMainDoc.Root.Element(XName.Get("body", W.NamespaceName));

            // Every file that is missing from the local document will have to be copied, every file that already exists will have to be merged.
            PackagePartCollection ppc = remoteDocument.Package.GetParts();

            List<String> ignoreContentTypes = new List<string>
            {
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml",
                "application/vnd.openxmlformats-package.core-properties+xml",
                "application/vnd.openxmlformats-officedocument.extended-properties+xml",
                ContentTypeApplicationRelationShipXml
            };

            List<String> imageContentTypes = new List<string>
            {
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/bmp",
                "image/gif",
                "image/tiff",
                "image/icon",
                "image/pcx",
                "image/emf",
                "image/wmf"
            };
            // Check if each PackagePart pp exists in this document.
            foreach (PackagePart remotePp in ppc)
            {
                if (ignoreContentTypes.Contains(remotePp.ContentType) || imageContentTypes.Contains(remotePp.ContentType))
                    continue;

                // If this external PackagePart already exits then we must merge them.
                if (Package.PartExists(remotePp.Uri))
                {
                    PackagePart localPp = Package.GetPart(remotePp.Uri);
                    switch (remotePp.ContentType)
                    {
                        case "application/vnd.openxmlformats-officedocument.custom-properties+xml":
                            merge_customs(remotePp, localPp, remoteMainDoc);
                            break;

                        // Merge footnotes (and endnotes) before merging styles, then set the remote_footnotes to the just updated footnotes
                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml":
                            merge_footnotes(remotePp, localPp, remoteMainDoc, remoteDocument, remoteFootnotes);
                            remoteFootnotes = Footnotes;
                            break;

                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml":
                            merge_endnotes(remotePp, localPp, remoteMainDoc, remoteDocument, remoteEndnotes);
                            remoteEndnotes = Endnotes;
                            break;

                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml":
                            merge_styles(remotePp, localPp, remoteMainDoc, remoteDocument, remoteFootnotes, remoteEndnotes);
                            break;

                        // Merge styles after merging the footnotes, so the changes will be applied to the correct document/footnotes
                        case "application/vnd.ms-word.stylesWithEffects+xml":
                            merge_styles(remotePp, localPp, remoteMainDoc, remoteDocument, remoteFootnotes, remoteEndnotes);
                            break;

                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.fontTable+xml":
                            merge_fonts(remotePp, localPp, remoteMainDoc, remoteDocument);
                            break;

                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml":
                            merge_numbering(remotePp, localPp, remoteMainDoc, remoteDocument);
                            break;
                    }
                }

                // If this external PackagePart does not exits in the internal document then we can simply copy it.
                else
                {
                    var packagePart = ClonePackagePart(remotePp);
                    switch (remotePp.ContentType)
                    {
                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml":
                            EndnotesPart = packagePart;
                            Endnotes = remoteEndnotes;
                            break;

                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml":
                            FootnotesPart = packagePart;
                            Footnotes = remoteFootnotes;
                            break;

                        case "application/vnd.openxmlformats-officedocument.custom-properties+xml":
                            break;

                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml":
                            StylesPart = packagePart;
                            using (TextReader tr = new StreamReader(StylesPart.GetStream()))
                                Styles = XDocument.Load(tr);
                            break;

                        case "application/vnd.ms-word.stylesWithEffects+xml":
                            StylesWithEffectsPart = packagePart;
                            using (TextReader tr = new StreamReader(StylesWithEffectsPart.GetStream()))
                                StylesWithEffects = XDocument.Load(tr);
                            break;

                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.fontTable+xml":
                            FontTablePart = packagePart;
                            using (TextReader tr = new StreamReader(FontTablePart.GetStream()))
                                FontTable = XDocument.Load(tr);
                            break;

                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml":
                            NumberingPart = packagePart;
                            using (TextReader tr = new StreamReader(NumberingPart.GetStream()))
                                Numbering = XDocument.Load(tr);
                            break;

                    }

                    ClonePackageRelationship(remoteDocument, remotePp, remoteMainDoc);
                }
            }

            foreach (var hyperlinkRel in remoteDocument.MainPart.GetRelationshipsByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink"))
            {
                var oldRelId = hyperlinkRel.Id;
                var newRelId = MainPart.CreateRelationship(hyperlinkRel.TargetUri, hyperlinkRel.TargetMode, hyperlinkRel.RelationshipType).Id;
                var hyperlinkRefs = remoteMainDoc.Descendants(XName.Get("hyperlink", W.NamespaceName));
                foreach (var hyperlinkRef in hyperlinkRefs)
                {
                    XAttribute a0 = hyperlinkRef.Attribute(XName.Get("id", R.NamespaceName));
                    if (a0 != null && a0.Value == oldRelId)
                    {
                        a0.SetValue(newRelId);
                    }
                }
            }

            ////ole object links
            foreach (var oleObjectRel in remoteDocument.MainPart.GetRelationshipsByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/oleObject"))
            {
                var oldRelId = oleObjectRel.Id;
                var newRelId = MainPart.CreateRelationship(oleObjectRel.TargetUri, oleObjectRel.TargetMode, oleObjectRel.RelationshipType).Id;
                var oleObjectRefs = remoteMainDoc.Descendants(XName.Get("OLEObject", "urn:schemas-microsoft-com:office:office"));
                foreach (var oleObjectRef in oleObjectRefs)
                {
                    XAttribute a0 = oleObjectRef.Attribute(XName.Get("id", R.NamespaceName));
                    if (a0 != null && a0.Value == oldRelId)
                    {
                        a0.SetValue(newRelId);
                    }
                }
            }


            foreach (PackagePart remotePp in ppc)
            {
                if (imageContentTypes.Contains(remotePp.ContentType))
                {
                    merge_images(remotePp, remoteDocument, remoteMainDoc, remotePp.ContentType);
                }
            }

            int id = 0;
            var localDocPrs = MainDoc.Root.Descendants(XName.Get("docPr", Wp.NamespaceName));
            foreach (var localDocPr in localDocPrs)
            {
                XAttribute aId = localDocPr.Attribute(XName.Get("id"));
                int aIdValue;
                if (aId != null && int.TryParse(aId.Value, out aIdValue))
                    if (aIdValue > id)
                        id = aIdValue;
            }
            id++;

            // docPr must be sequential
            var docPrs = remoteBody.Descendants(XName.Get("docPr", Wp.NamespaceName));
            foreach (var docPr in docPrs)
            {
                docPr.SetAttributeValue(XName.Get("id"), id);
                id++;
            }

            // Add the remote documents contents to this document.
            XElement localBody = MainDoc.Root.Element(XName.Get("body", W.NamespaceName));
            if (append)
                localBody.Add(remoteBody.Elements());
            else
                localBody.AddFirst(remoteBody.Elements());

            // Copy any missing root attributes to the local document.
            foreach (XAttribute a in remoteMainDoc.Root.Attributes())
            {
                if (MainDoc.Root.Attribute(a.Name) == null)
                {
                    MainDoc.Root.SetAttributeValue(a.Name, a.Value);
                }
            }

        }

        private void merge_images(PackagePart remotePp, DocX remoteDocument, XDocument remoteMainDoc, String contentType)
        {
            // Before doing any other work, check to see if this image is actually referenced in the document.
            // In my testing I have found cases of Images inside documents that are not referenced
            var remoteRel = remoteDocument.MainPart.GetRelationships().Where(r => r.TargetUri.OriginalString.Equals(remotePp.Uri.OriginalString.Replace("/word/", ""))).FirstOrDefault();
            if (remoteRel == null) {
                remoteRel = remoteDocument.MainPart.GetRelationships().Where(r => r.TargetUri.OriginalString.Equals(remotePp.Uri.OriginalString)).FirstOrDefault();
                if (remoteRel == null)
                    return;
            }
            String remoteId = remoteRel.Id;

            String remoteHash = ComputeMd5HashString(remotePp.GetStream());
            var imageParts = Package.GetParts().Where(pp => pp.ContentType.Equals(contentType));

            bool found = false;
            foreach (var part in imageParts)
            {
                String localHash = ComputeMd5HashString(part.GetStream());
                if (localHash.Equals(remoteHash))
                {
                    // This image already exists in this document.
                    found = true;

                    var localRel = MainPart.GetRelationships().Where(r => r.TargetUri.OriginalString.Equals(part.Uri.OriginalString.Replace("/word/", ""))).FirstOrDefault();
                    if (localRel == null)
                    {
                        localRel = MainPart.GetRelationships().Where(r => r.TargetUri.OriginalString.Equals(part.Uri.OriginalString)).FirstOrDefault();
                    }
                    if (localRel != null)
                    {
                        String newId = localRel.Id;

                        // Replace all instances of remote_Id in the local document with local_Id
                        var elems = remoteMainDoc.Descendants(XName.Get("blip", A.NamespaceName));
                        foreach (var elem in elems)
                        {
                            XAttribute embed = elem.Attribute(XName.Get("embed", R.NamespaceName));
                            if (embed != null && embed.Value == remoteId)
                            {
                                embed.SetValue(newId);
                            }
                        }

                        // Replace all instances of remote_Id in the local document with local_Id (for shapes as well)
                        var vElems = remoteMainDoc.Descendants(XName.Get("imagedata", V.NamespaceName));
                        foreach (var elem in vElems)
                        {
                            XAttribute id = elem.Attribute(XName.Get("id", R.NamespaceName));
                            if (id != null && id.Value == remoteId)
                            {
                                id.SetValue(newId);
                            }
                        }
                    }

                    break;
                }
            }

            // This image does not exist in this document.
            if (!found)
            {
                String newUri = remotePp.Uri.OriginalString;
                newUri = newUri.Remove(newUri.LastIndexOf("/"));
                //new_uri = new_uri.Replace("word/", "");
                newUri += "/" + Guid.NewGuid() + contentType.Replace("image/", ".");
                if (!newUri.StartsWith("/"))
                    newUri = "/" + newUri;

                PackagePart newPp = Package.CreatePart(new Uri(newUri, UriKind.Relative), remotePp.ContentType, CompressionOption.Normal);

                using (Stream sRead = remotePp.GetStream())
                {
                    using (Stream sWrite = new PackagePartStream(newPp.GetStream(FileMode.Create)))
                    {
                        CopyStream(sRead, sWrite);
                    }
                }

                PackageRelationship pr = MainPart.CreateRelationship(new Uri(newUri, UriKind.Relative), TargetMode.Internal, RelationshipImage);

                String newId = pr.Id;

                //Check if the remote relationship id is a default rId from Word
                Match defRelId = Regex.Match(remoteId, @"rId\d+", RegexOptions.IgnoreCase);

                // Replace all instances of remote_Id in the local document with local_Id
                var elems = remoteMainDoc.Descendants(XName.Get("blip", A.NamespaceName));
                foreach (var elem in elems)
                {
                    XAttribute embed = elem.Attribute(XName.Get("embed", R.NamespaceName));
                    if (embed != null && embed.Value == remoteId)
                    {
                        embed.SetValue(newId);
                    }
                }

                if (!defRelId.Success)
                {
                    // Replace all instances of remote_Id in the local document with local_Id
                    var elemsLocal = MainDoc.Descendants(XName.Get("blip", A.NamespaceName));
                    foreach (var elem in elemsLocal)
                    {
                        XAttribute embed = elem.Attribute(XName.Get("embed", R.NamespaceName));
                        if (embed != null && embed.Value == remoteId)
                        {
                            embed.SetValue(newId);
                        }
                    }


                    // Replace all instances of remote_Id in the local document with local_Id
                    var vElemsLocal = MainDoc.Descendants(XName.Get("imagedata", V.NamespaceName));
                    foreach (var elem in vElemsLocal)
                    {
                        XAttribute id = elem.Attribute(XName.Get("id", R.NamespaceName));
                        if (id != null && id.Value == remoteId)
                        {
                            id.SetValue(newId);
                        }
                    }
                }


                // Replace all instances of remote_Id in the local document with local_Id (for shapes as well)
                var vElems = remoteMainDoc.Descendants(XName.Get("imagedata", V.NamespaceName));
                foreach (var elem in vElems)
                {
                    XAttribute id = elem.Attribute(XName.Get("id", R.NamespaceName));
                    if (id != null && id.Value == remoteId)
                    {
                        id.SetValue(newId);
                    }
                }
            }
        }

        private string ComputeMd5HashString(Stream stream)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        private void merge_endnotes(PackagePart remotePp, PackagePart localPp, XDocument remoteMainDoc, DocX remote, XDocument remoteEndnotes)
        {
            IEnumerable<int> ids =
            (
                from d in Endnotes.Root.Descendants()
                where d.Name.LocalName == "endnote"
                select int.Parse(d.Attribute(XName.Get("id", W.NamespaceName)).Value)
            );

            int maxId = ids.Max() + 1;
            var endnoteReferences = remoteMainDoc.Descendants(XName.Get("endnoteReference", W.NamespaceName));

            foreach (var endnote in remoteEndnotes.Root.Elements().OrderBy(fr => fr.Attribute(XName.Get("id", R.NamespaceName))).Reverse())
            {
                XAttribute id = endnote.Attribute(XName.Get("id", W.NamespaceName));
                int i;
                if (id != null && int.TryParse(id.Value, out i))
                {
                    if (i > 0)
                    {
                        foreach (var endnoteRef in endnoteReferences)
                        {
                            XAttribute a = endnoteRef.Attribute(XName.Get("id", W.NamespaceName));
                            if (a != null && int.Parse(a.Value).Equals(i))
                            {
                                a.SetValue(maxId);
                            }
                        }

                        // We care about copying this footnote.
                        endnote.SetAttributeValue(XName.Get("id", W.NamespaceName), maxId);
                        Endnotes.Root.Add(endnote);
                        maxId++;
                    }
                }
            }
        }

        private void merge_footnotes(PackagePart remotePp, PackagePart localPp, XDocument remoteMainDoc, DocX remote, XDocument remoteFootnotes)
        {
            IEnumerable<int> ids =
            (
                from d in Footnotes.Root.Descendants()
                where d.Name.LocalName == "footnote"
                select int.Parse(d.Attribute(XName.Get("id", W.NamespaceName)).Value)
            );

            int maxId = ids.Max() + 1;
            var footnoteReferences = remoteMainDoc.Descendants(XName.Get("footnoteReference", W.NamespaceName));

            foreach (var footnote in remoteFootnotes.Root.Elements().OrderBy(fr => fr.Attribute(XName.Get("id", R.NamespaceName))).Reverse())
            {
                XAttribute id = footnote.Attribute(XName.Get("id", W.NamespaceName));
                int i;
                if (id != null && int.TryParse(id.Value, out i))
                {
                    if (i > 0)
                    {
                        foreach (var footnoteRef in footnoteReferences)
                        {
                            XAttribute a = footnoteRef.Attribute(XName.Get("id", W.NamespaceName));
                            if (a != null && int.Parse(a.Value).Equals(i))
                            {
                                a.SetValue(maxId);
                            }
                        }

                        // We care about copying this footnote.
                        footnote.SetAttributeValue(XName.Get("id", W.NamespaceName), maxId);
                        Footnotes.Root.Add(footnote);
                        maxId++;
                    }
                }
            }
        }

        private void merge_customs(PackagePart remotePp, PackagePart localPp, XDocument remoteMainDoc)
        {
            // Get the remote documents custom.xml file.
            XDocument remoteCustomDocument;
            using (TextReader tr = new StreamReader(remotePp.GetStream()))
                remoteCustomDocument = XDocument.Load(tr);

            // Get the local documents custom.xml file.
            XDocument localCustomDocument;
            using (TextReader tr = new StreamReader(localPp.GetStream()))
                localCustomDocument = XDocument.Load(tr);

            IEnumerable<int> pids =
            (
                from d in remoteCustomDocument.Root.Descendants()
                where d.Name.LocalName == "property"
                select int.Parse(d.Attribute(XName.Get("pid")).Value)
            );

            int pid = pids.Max() + 1;

            foreach (XElement remoteProperty in remoteCustomDocument.Root.Elements())
            {
                bool found = false;
                foreach (XElement localProperty in localCustomDocument.Root.Elements())
                {
                    XAttribute remotePropertyName = remoteProperty.Attribute(XName.Get("name"));
                    XAttribute localPropertyName = localProperty.Attribute(XName.Get("name"));

                    if (remoteProperty != null && localPropertyName != null && remotePropertyName.Value.Equals(localPropertyName.Value))
                        found = true;
                }

                if (!found)
                {
                    remoteProperty.SetAttributeValue(XName.Get("pid"), pid);
                    localCustomDocument.Root.Add(remoteProperty);

                    pid++;
                }
            }

            // Save the modified local custom styles.xml file.
            using (TextWriter tw = new StreamWriter(new PackagePartStream(localPp.GetStream(FileMode.Create, FileAccess.Write))))
                localCustomDocument.Save(tw, SaveOptions.None);
        }

        private void merge_numbering(PackagePart remotePp, PackagePart localPp, XDocument remoteMainDoc, DocX remote)
        {
            // Add each remote numbering to this document.
            IEnumerable<XElement> remoteAbstractNums = remote.Numbering.Root.Elements(XName.Get("abstractNum", W.NamespaceName));
            int guidd = 0;
            foreach (var an in remoteAbstractNums)
            {
                XAttribute a = an.Attribute(XName.Get("abstractNumId", W.NamespaceName));
                if (a != null)
                {
                    int i;
                    if (int.TryParse(a.Value, out i))
                    {
                        if (i > guidd)
                            guidd = i;
                    }
                }
            }
            guidd++;

            IEnumerable<XElement> remoteNums = remote.Numbering.Root.Elements(XName.Get("num", W.NamespaceName));
            int guidd2 = 0;
            foreach (var an in remoteNums)
            {
                XAttribute a = an.Attribute(XName.Get("numId", W.NamespaceName));
                if (a != null)
                {
                    int i;
                    if (int.TryParse(a.Value, out i))
                    {
                        if (i > guidd2)
                            guidd2 = i;
                    }
                }
            }
            guidd2++;

            foreach (XElement remoteAbstractNum in remoteAbstractNums)
            {
                XAttribute abstractNumId = remoteAbstractNum.Attribute(XName.Get("abstractNumId", W.NamespaceName));
                if (abstractNumId != null)
                {
                    String abstractNumIdValue = abstractNumId.Value;
                    abstractNumId.SetValue(guidd);

                    foreach (XElement remoteNum in remoteNums)
                    {
                        var numIds = remoteMainDoc.Descendants(XName.Get("numId", W.NamespaceName));
                        foreach (var numId in numIds)
                        {
                            XAttribute attr = numId.Attribute(XName.Get("val", W.NamespaceName));
                            if (attr != null && attr.Value.Equals(remoteNum.Attribute(XName.Get("numId", W.NamespaceName)).Value))
                            {
                                attr.SetValue(guidd2);
                            }

                        }
                        remoteNum.SetAttributeValue(XName.Get("numId", W.NamespaceName), guidd2);

                        XElement e = remoteNum.Element(XName.Get("abstractNumId", W.NamespaceName));
                        XAttribute a2 = e?.Attribute(XName.Get("val", W.NamespaceName));
                        if (a2 != null && a2.Value.Equals(abstractNumIdValue))
                            a2.SetValue(guidd);

                        guidd2++;
                    }
                }

                guidd++;
            }

            // Checking whether there were more than 0 elements, helped me get rid of exceptions thrown while using InsertDocument
            if (Numbering.Root.Elements(XName.Get("abstractNum", W.NamespaceName)).Count() > 0)
                Numbering.Root.Elements(XName.Get("abstractNum", W.NamespaceName)).Last().AddAfterSelf(remoteAbstractNums);

            if (Numbering.Root.Elements(XName.Get("num", W.NamespaceName)).Count() > 0)
                Numbering.Root.Elements(XName.Get("num", W.NamespaceName)).Last().AddAfterSelf(remoteNums);
        }

        private void merge_fonts(PackagePart remotePp, PackagePart localPp, XDocument remoteMainDoc, DocX remote)
        {
            // Add each remote font to this document.
            IEnumerable<XElement> remoteFonts = remote.FontTable.Root.Elements(XName.Get("font", W.NamespaceName));
            IEnumerable<XElement> localFonts = FontTable.Root.Elements(XName.Get("font", W.NamespaceName));

            foreach (XElement remoteFont in remoteFonts)
            {
                bool flagAddFont = true;
                foreach (XElement localFont in localFonts)
                {
                    if (localFont.Attribute(XName.Get("name", W.NamespaceName)).Value == remoteFont.Attribute(XName.Get("name", W.NamespaceName)).Value)
                    {
                        flagAddFont = false;
                        break;
                    }
                }

                if (flagAddFont)
                {
                    FontTable.Root.Add(remoteFont);
                }
            }
        }

        private void merge_styles(PackagePart remotePp, PackagePart localPp, XDocument remoteMainDoc, DocX remote, XDocument remoteFootnotes, XDocument remoteEndnotes)
        {
            var localStyles = new Dictionary<string, string>();
            foreach (XElement localStyle in Styles.Root.Elements(XName.Get("style", W.NamespaceName)))
            {
                XElement temp = new XElement(localStyle);
                XAttribute styleId = temp.Attribute(XName.Get("styleId", W.NamespaceName));
                String value = styleId.Value;
                styleId.Remove();
                String key = Regex.Replace(temp.ToString(), @"\s+", "");
                if (!localStyles.ContainsKey(key)) localStyles.Add(key, value);
            }

            // Add each remote style to this document.
            IEnumerable<XElement> remoteStyles = remote.Styles.Root.Elements(XName.Get("style", W.NamespaceName));
            foreach (XElement remoteStyle in remoteStyles)
            {
                XElement temp = new XElement(remoteStyle);
                XAttribute styleId = temp.Attribute(XName.Get("styleId", W.NamespaceName));
                String value = styleId.Value;
                styleId.Remove();
                String key = Regex.Replace(temp.ToString(), @"\s+", "");
                String guuid;

                // Check to see if the local document already contains the remote style.
                if (localStyles.ContainsKey(key))
                {
                    String localValue;
                    localStyles.TryGetValue(key, out localValue);

                    // If the styleIds are the same then nothing needs to be done.
                    if (localValue == value)
                        continue;

                    // All we need to do is update the styleId.
                    guuid = localValue;
                }
                else
                {
                    guuid = Guid.NewGuid().ToString();
                    // Set the styleId in the remote_style to this new Guid
                    // [Fixed the issue that my document referred to a new Guid while my styles still had the old value ("Titel")]
                    remoteStyle.SetAttributeValue(XName.Get("styleId", W.NamespaceName), guuid);
                }

                foreach (XElement e in remoteMainDoc.Root.Descendants(XName.Get("pStyle", W.NamespaceName)))
                {
                    XAttribute eStyleId = e.Attribute(XName.Get("val", W.NamespaceName));
                    if (eStyleId != null && eStyleId.Value.Equals(styleId.Value))
                    {
                        eStyleId.SetValue(guuid);
                    }
                }

                foreach (XElement e in remoteMainDoc.Root.Descendants(XName.Get("rStyle", W.NamespaceName)))
                {
                    XAttribute eStyleId = e.Attribute(XName.Get("val", W.NamespaceName));
                    if (eStyleId != null && eStyleId.Value.Equals(styleId.Value))
                    {
                        eStyleId.SetValue(guuid);
                    }
                }

                foreach (XElement e in remoteMainDoc.Root.Descendants(XName.Get("tblStyle", W.NamespaceName)))
                {
                    XAttribute eStyleId = e.Attribute(XName.Get("val", W.NamespaceName));
                    if (eStyleId != null && eStyleId.Value.Equals(styleId.Value))
                    {
                        eStyleId.SetValue(guuid);
                    }
                }

                if (remoteEndnotes != null)
                {
                    foreach (XElement e in remoteEndnotes.Root.Descendants(XName.Get("rStyle", W.NamespaceName)))
                    {
                        XAttribute eStyleId = e.Attribute(XName.Get("val", W.NamespaceName));
                        if (eStyleId != null && eStyleId.Value.Equals(styleId.Value))
                        {
                            eStyleId.SetValue(guuid);
                        }
                    }

                    foreach (XElement e in remoteEndnotes.Root.Descendants(XName.Get("pStyle", W.NamespaceName)))
                    {
                        XAttribute eStyleId = e.Attribute(XName.Get("val", W.NamespaceName));
                        if (eStyleId != null && eStyleId.Value.Equals(styleId.Value))
                        {
                            eStyleId.SetValue(guuid);
                        }
                    }
                }

                if (remoteFootnotes != null)
                {
                    foreach (XElement e in remoteFootnotes.Root.Descendants(XName.Get("rStyle", W.NamespaceName)))
                    {
                        XAttribute eStyleId = e.Attribute(XName.Get("val", W.NamespaceName));
                        if (eStyleId != null && eStyleId.Value.Equals(styleId.Value))
                        {
                            eStyleId.SetValue(guuid);
                        }
                    }

                    foreach (XElement e in remoteFootnotes.Root.Descendants(XName.Get("pStyle", W.NamespaceName)))
                    {
                        XAttribute eStyleId = e.Attribute(XName.Get("val", W.NamespaceName));
                        if (eStyleId != null && eStyleId.Value.Equals(styleId.Value))
                        {
                            eStyleId.SetValue(guuid);
                        }
                    }
                }

                // Make sure they don't clash by using a uuid.
                styleId.SetValue(guuid);
                Styles.Root.Add(remoteStyle);
            }
        }

        protected void ClonePackageRelationship(DocX remoteDocument, PackagePart pp, XDocument remoteMainDoc)
        {
            string url = pp.Uri.OriginalString.Replace("/", "");
            var remoteRels = remoteDocument.MainPart.GetRelationships();
            foreach (var remoteRel in remoteRels)
            {
                if (url.Equals("word" + remoteRel.TargetUri.OriginalString.Replace("/", "")))
                {
                    String remoteId = remoteRel.Id;
                    String localId = MainPart.CreateRelationship(remoteRel.TargetUri, remoteRel.TargetMode, remoteRel.RelationshipType).Id;

                    // Replace all instances of remote_Id in the local document with local_Id
                    var elems = remoteMainDoc.Descendants(XName.Get("blip", A.NamespaceName));
                    foreach (var elem in elems)
                    {
                        XAttribute embed = elem.Attribute(XName.Get("embed", R.NamespaceName));
                        if (embed != null && embed.Value == remoteId)
                        {
                            embed.SetValue(localId);
                        }
                    }

                    // Replace all instances of remote_Id in the local document with local_Id (for shapes as well)
                    var vElems = remoteMainDoc.Descendants(XName.Get("imagedata", V.NamespaceName));
                    foreach (var elem in vElems)
                    {
                        XAttribute id = elem.Attribute(XName.Get("id", R.NamespaceName));
                        if (id != null && id.Value == remoteId)
                        {
                            id.SetValue(localId);
                        }
                    }
                    break;
                }
            }
        }

        protected PackagePart ClonePackagePart(PackagePart pp)
        {
            PackagePart newPp = Package.CreatePart(pp.Uri, pp.ContentType, CompressionOption.Normal);

            using (Stream sRead = pp.GetStream())
            {
                using (Stream sWrite = new PackagePartStream(newPp.GetStream(FileMode.Create)))
                {
                    CopyStream(sRead, sWrite);
                }
            }

            return newPp;
        }

        protected string GetMd5HashFromStream(Stream stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(stream);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Insert a new Table at the end of this document.
        /// </summary>
        /// <param name="columnCount">The number of columns to create.</param>
        /// <param name="rowCount">The number of rows to create.</param>
        /// <returns>A new Table.</returns>
        /// <example>
        /// Insert a new Table with 2 columns and 3 rows, at the end of a document.
        /// <code>
        /// // Create a document.
        /// using (DocX document = DocX.Create(@"C:\Example\Test.docx"))
        /// {
        ///     // Create a new Table with 2 columns and 3 rows.
        ///     Table newTable = document.InsertTable(2, 3);
        ///
        ///     // Set the design of this Table.
        ///     newTable.Design = TableDesign.LightShadingAccent2;
        ///
        ///     // Set the column names.
        ///     newTable.Rows[0].Cells[0].Paragraph.InsertText("Ice Cream", false);
        ///     newTable.Rows[0].Cells[1].Paragraph.InsertText("Price", false);
        ///
        ///     // Fill row 1
        ///     newTable.Rows[1].Cells[0].Paragraph.InsertText("Chocolate", false);
        ///     newTable.Rows[1].Cells[1].Paragraph.InsertText("€3:50", false);
        ///
        ///     // Fill row 2
        ///     newTable.Rows[2].Cells[0].Paragraph.InsertText("Vanilla", false);
        ///     newTable.Rows[2].Cells[1].Paragraph.InsertText("€3:00", false);
        ///
        ///     // Save all changes made to document b.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public new Table InsertTable(int rowCount, int columnCount)
        {
            if (rowCount < 1 || columnCount < 1)
                throw new ArgumentOutOfRangeException("Row and Column count must be greater than zero.");

            Table t = base.InsertTable(rowCount, columnCount);
            t.MainPart = MainPart;
            return t;
        }

        public Table AddTable(int rowCount, int columnCount)
        {
            if (rowCount < 1 || columnCount < 1)
                throw new ArgumentOutOfRangeException("Row and Column count must be greater than zero.");

            Table t = new Table(this, HelperFunctions.CreateTable(rowCount, columnCount));
            t.MainPart = MainPart;
            return t;
        }

        /// <summary>
        /// Create a new list with a list item.
        /// </summary>
        /// <param name="listText">The text of the first element in the created list.</param>
        /// <param name="level">The indentation level of the element in the list.</param>
        /// <param name="listType">The type of list to be created: Bulleted or Numbered.</param>
        /// <param name="startNumber">The number start number for the list. </param>
        /// <param name="trackChanges">Enable change tracking</param>
        /// <param name="continueNumbering">Set to true if you want to continue numbering from the previous numbered list</param>
        /// <returns>
        /// The created List. Call AddListItem(...) to add more elements to the list.
        /// Write the list to the Document with InsertList(...) once the list has all the desired
        /// elements, otherwise the list will not be included in the working Document.
        /// </returns>
        public List AddList(string listText = null, int level = 0, ListItemType listType = ListItemType.Numbered, int? startNumber = null, bool trackChanges = false, bool continueNumbering = false)
        {
            return AddListItem(new List(this, null), listText, level, listType, startNumber, trackChanges, continueNumbering);
        }

        /// <summary>
        /// Add a list item to an already existing list.
        /// </summary>
        /// <param name="list">The list to add the new list item to.</param>
        /// <param name="listText">The run text that should be in the new list item.</param>
        /// <param name="level">The indentation level of the new list element.</param>
        /// <param name="startNumber">The number start number for the list. </param>
        /// <param name="trackChanges">Enable change tracking</param>
        /// <param name="listType">Numbered or Bulleted list type. </param>
        /// /// <param name="continueNumbering">Set to true if you want to continue numbering from the previous numbered list</param>
        /// <returns>
        /// The created List. Call AddListItem(...) to add more elements to the list.
        /// Write the list to the Document with InsertList(...) once the list has all the desired
        /// elements, otherwise the list will not be included in the working Document.
        /// </returns>
        public List AddListItem(List list, string listText, int level = 0, ListItemType listType = ListItemType.Numbered, int? startNumber = null, bool trackChanges = false, bool continueNumbering = false)
        {
            if (startNumber.HasValue && continueNumbering) throw new InvalidOperationException("Cannot specify a start number and at the same time continue numbering from another list");
            var listToReturn = HelperFunctions.CreateItemInList(list, listText, level, listType, startNumber, trackChanges, continueNumbering);
            var lastItem = listToReturn.Items.LastOrDefault();
            if (lastItem != null)
            {
                lastItem.PackagePart = MainPart;
            }
            return listToReturn;

        }

        /// <summary>
        /// Insert list into the document.
        /// </summary>
        /// <param name="list">The list to insert into the document.</param>
        /// <returns>The list that was inserted into the document.</returns>
        public new List InsertList(List list)
        {
            base.InsertList(list);
            return list;
        }
        public new List InsertList(List list, Font fontFamily, double fontSize)
        {
            base.InsertList(list, fontFamily, fontSize);
            return list;
        }
        public new List InsertList(List list, double fontSize)
        {
            base.InsertList(list, fontSize);
            return list;
        }

        /// <summary>
        /// Insert a list at an index location in the document.
        /// </summary>
        /// <param name="index">Index in document to insert the list.</param>
        /// <param name="list">The list that was inserted into the document.</param>
        /// <returns></returns>
        public new List InsertList(int index, List list)
        {
            base.InsertList(index, list);
            return list;
        }

        internal XDocument AddStylesForList()
        {
            var wordStylesUri = new Uri("/word/styles.xml", UriKind.Relative);

            // If the internal document contains no /word/styles.xml create one.
            if (!Package.PartExists(wordStylesUri))
                HelperFunctions.AddDefaultStylesXml(Package);

            // Load the styles.xml into memory.
            XDocument wordStyles;
            using (TextReader tr = new StreamReader(Package.GetPart(wordStylesUri).GetStream()))
                wordStyles = XDocument.Load(tr);

            bool listStyleExists =
            (
              from s in wordStyles.Element(W + "styles").Elements()
              let styleId = s.Attribute(XName.Get("styleId", W.NamespaceName))
              where (styleId != null && styleId.Value == "ListParagraph")
              select s
            ).Any();

            if (!listStyleExists)
            {
                var style = new XElement
                (
                    W + "style",
                    new XAttribute(W + "type", "paragraph"),
                    new XAttribute(W + "styleId", "ListParagraph"),
                        new XElement(W + "name", new XAttribute(W + "val", "List Paragraph")),
                        new XElement(W + "basedOn", new XAttribute(W + "val", "Normal")),
                        new XElement(W + "uiPriority", new XAttribute(W + "val", "34")),
                        new XElement(W + "qformat"),
                        new XElement(W + "rsid", new XAttribute(W + "val", "00832EE1")),
                        new XElement
                        (
                            W + "rPr",
                            new XElement(W + "ind", new XAttribute(W + "left", "720")),
                            new XElement
                            (
                                W + "contextualSpacing"
                            )
                        )
                );
                wordStyles.Element(W + "styles").Add(style);

                // Save the styles document.
                using (TextWriter tw = new StreamWriter(new PackagePartStream(Package.GetPart(wordStylesUri).GetStream())))
                    wordStyles.Save(tw);
            }

            return wordStyles;
        }

        /// <summary>
        /// Insert a Table into this document. The Table's source can be a completely different document.
        /// </summary>
        /// <param name="t">The Table to insert.</param>
        /// <param name="index">The index to insert this Table at.</param>
        /// <returns>The Table now associated with this document.</returns>
        /// <example>
        /// Extract a Table from document a and insert it into document b, at index 10.
        /// <code>
        /// // Place holder for a Table.
        /// Table t;
        ///
        /// // Load document a.
        /// using (DocX documentA = DocX.Load(@"C:\Example\a.docx"))
        /// {
        ///     // Get the first Table from this document.
        ///     t = documentA.Tables[0];
        /// }
        ///
        /// // Load document b.
        /// using (DocX documentB = DocX.Load(@"C:\Example\b.docx"))
        /// {
        ///     /*
        ///      * Insert the Table that was extracted from document a, into document b.
        ///      * This creates a new Table that is now associated with document b.
        ///      */
        ///     Table newTable = documentB.InsertTable(10, t);
        ///
        ///     // Save all changes made to document b.
        ///     documentB.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public new Table InsertTable(int index, Table t)
        {
            Table t2 = base.InsertTable(index, t);
            t2.MainPart = MainPart;
            return t2;
        }

        /// <summary>
        /// Insert a Table into this document. The Table's source can be a completely different document.
        /// </summary>
        /// <param name="t">The Table to insert.</param>
        /// <returns>The Table now associated with this document.</returns>
        /// <example>
        /// Extract a Table from document a and insert it at the end of document b.
        /// <code>
        /// // Place holder for a Table.
        /// Table t;
        ///
        /// // Load document a.
        /// using (DocX documentA = DocX.Load(@"C:\Example\a.docx"))
        /// {
        ///     // Get the first Table from this document.
        ///     t = documentA.Tables[0];
        /// }
        ///
        /// // Load document b.
        /// using (DocX documentB = DocX.Load(@"C:\Example\b.docx"))
        /// {
        ///     /*
        ///      * Insert the Table that was extracted from document a, into document b.
        ///      * This creates a new Table that is now associated with document b.
        ///      */
        ///     Table newTable = documentB.InsertTable(t);
        ///
        ///     // Save all changes made to document b.
        ///     documentB.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public new Table InsertTable(Table t)
        {
            t = base.InsertTable(t);
            t.MainPart = MainPart;
            return t;
        }

        /// <summary>
        /// Insert a new Table at the end of this document.
        /// </summary>
        /// <param name="columnCount">The number of columns to create.</param>
        /// <param name="rowCount">The number of rows to create.</param>
        /// <param name="index">The index to insert this Table at.</param>
        /// <returns>A new Table.</returns>
        /// <example>
        /// Insert a new Table with 2 columns and 3 rows, at index 37 in this document.
        /// <code>
        /// // Create a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Create a new Table with 3 rows and 2 columns. Insert this Table at index 37.
        ///     Table newTable = document.InsertTable(37, 3, 2);
        ///
        ///     // Set the design of this Table.
        ///     newTable.Design = TableDesign.LightShadingAccent3;
        ///
        ///     // Set the column names.
        ///     newTable.Rows[0].Cells[0].Paragraph.InsertText("Ice Cream", false);
        ///     newTable.Rows[0].Cells[1].Paragraph.InsertText("Price", false);
        ///
        ///     // Fill row 1
        ///     newTable.Rows[1].Cells[0].Paragraph.InsertText("Chocolate", false);
        ///     newTable.Rows[1].Cells[1].Paragraph.InsertText("€3:50", false);
        ///
        ///     // Fill row 2
        ///     newTable.Rows[2].Cells[0].Paragraph.InsertText("Vanilla", false);
        ///     newTable.Rows[2].Cells[1].Paragraph.InsertText("€3:00", false);
        ///
        ///     // Save all changes made to document b.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public new Table InsertTable(int index, int rowCount, int columnCount)
        {
            if (rowCount < 1 || columnCount < 1)
                throw new ArgumentOutOfRangeException("Row and Column count must be greater than zero.");

            Table t = base.InsertTable(index, rowCount, columnCount);
            t.MainPart = MainPart;
            return t;
        }

        /// <summary>
        /// Creates a document using a Stream.
        /// </summary>
        /// <param name="stream">The Stream to create the document from.</param>
        /// <param name="documentType"></param>
        /// <returns>Returns a DocX object which represents the document.</returns>
        /// <example>
        /// Creating a document from a FileStream.
        /// <code>
        /// // Use a FileStream fs to create a new document.
        /// using(FileStream fs = new FileStream(@"C:\Example\Test.docx", FileMode.Create))
        /// {
        ///     // Load the document using fs
        ///     using (DocX document = DocX.Create(fs))
        ///     {
        ///         // Do something with the document here.
        ///
        ///         // Save all changes made to this document.
        ///         document.Save();
        ///     }// Release this document from memory.
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// Creating a document in a SharePoint site.
        /// <code>
        /// using(SPSite mySite = new SPSite("http://server/sites/site"))
        /// {
        ///     // Open a connection to the SharePoint site
        ///     using(SPWeb myWeb = mySite.OpenWeb())
        ///     {
        ///         // Create a MemoryStream ms.
        ///         using (MemoryStream ms = new MemoryStream())
        ///         {
        ///             // Create a document using ms.
        ///             using (DocX document = DocX.Create(ms))
        ///             {
        ///                 // Do something with the document here.
        ///
        ///                 // Save all changes made to this document.
        ///                 document.Save();
        ///             }// Release this document from memory
        ///
        ///             // Add the document to the SharePoint site
        ///             web.Files.Add("filename", ms.ToArray(), true);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="DocX.Load(System.IO.Stream)"/>
        /// <seealso cref="DocX.Load(string)"/>
        /// <seealso cref="DocX.Save()"/>
        public static DocX Create(Stream stream, DocumentTypes documentType = DocumentTypes.Document)
        {
            // Store this document in memory
            MemoryStream ms = new MemoryStream();

            // Create the docx package
            Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);

            PostCreation(package, documentType);
            DocX document = Load(ms);
            document.Stream = stream;
            return document;
        }

        /// <summary>
        /// Creates a document using a fully qualified or relative filename.
        /// </summary>
        /// <param name="filename">The fully qualified or relative filename.</param>
        /// <param name="documentType"></param>
        /// <returns>Returns a DocX object which represents the document.</returns>
        /// <example>
        /// <code>
        /// // Create a document using a relative filename.
        /// using (DocX document = DocX.Create(@"..\Test.docx"))
        /// {
        ///     // Do something with the document here.
        ///
        ///     // Save all changes made to this document.
        ///     document.Save();
        /// }// Release this document from memory
        /// </code>
        /// <code>
        /// // Create a document using a relative filename.
        /// using (DocX document = DocX.Create(@"..\Test.docx"))
        /// {
        ///     // Do something with the document here.
        ///
        ///     // Save all changes made to this document.
        ///     document.Save();
        /// }// Release this document from memory
        /// </code>
        /// <seealso cref="DocX.Load(System.IO.Stream)"/>
        /// <seealso cref="DocX.Load(string)"/>
        /// <seealso cref="DocX.Save()"/>
        /// </example>
        public static DocX Create(string filename, DocumentTypes documentType = DocumentTypes.Document)
        {
            // Store this document in memory
            MemoryStream ms = new MemoryStream();

            // Create the docx package
            //WordprocessingDocument wdDoc = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
            Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);

            PostCreation(package, documentType);
            DocX document = Load(ms);
            document.Filename = filename;
            return document;
        }

        internal static void PostCreation(Package package, DocumentTypes documentType = DocumentTypes.Document)
        {
            XDocument mainDoc, stylesDoc, numberingDoc;

            #region MainDocumentPart
            // Create the main document part for this package
            PackagePart mainDocumentPart;
            if (documentType == DocumentTypes.Document)
            {
                mainDocumentPart = package.CreatePart(new Uri("/word/document.xml", UriKind.Relative), "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml", CompressionOption.Normal);
            }
            else
            {
                mainDocumentPart = package.CreatePart(new Uri("/word/document.xml", UriKind.Relative), "application/vnd.openxmlformats-officedocument.wordprocessingml.template.main+xml", CompressionOption.Normal);
            }
            package.CreateRelationship(mainDocumentPart.Uri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument");

            // Load the document part into a XDocument object
            using (TextReader tr = new StreamReader(mainDocumentPart.GetStream(FileMode.Create, FileAccess.ReadWrite)))
            {
                mainDoc = XDocument.Parse
                (@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
                   <w:document xmlns:ve=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:m=""http://schemas.openxmlformats.org/officeDocument/2006/math"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:wp=""http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing"" xmlns:w10=""urn:schemas-microsoft-com:office:word"" xmlns:w=""http://schemas.openxmlformats.org/wordprocessingml/2006/main"" xmlns:wne=""http://schemas.microsoft.com/office/word/2006/wordml"" xmlns:a=""http://schemas.openxmlformats.org/drawingml/2006/main"" xmlns:c=""http://schemas.openxmlformats.org/drawingml/2006/chart"">
                   <w:body>
                    <w:sectPr w:rsidR=""003E25F4"" w:rsidSect=""00FC3028"">
                        <w:pgSz w:w=""11906"" w:h=""16838""/>
                        <w:pgMar w:top=""1440"" w:right=""1440"" w:bottom=""1440"" w:left=""1440"" w:header=""708"" w:footer=""708"" w:gutter=""0""/>
                        <w:cols w:space=""708""/>
                        <w:docGrid w:linePitch=""360""/>
                    </w:sectPr>
                   </w:body>
                   </w:document>"
                );
            }

            // Save the main document
            using (TextWriter tw = new StreamWriter(new PackagePartStream(mainDocumentPart.GetStream(FileMode.Create, FileAccess.Write))))
                mainDoc.Save(tw, SaveOptions.None);
            #endregion

            #region StylePart
            stylesDoc = HelperFunctions.AddDefaultStylesXml(package);
            #endregion

            #region NumberingPart
            numberingDoc = HelperFunctions.AddDefaultNumberingXml(package);
            #endregion

            package.Close();
        }

        internal static DocX PostLoad(ref Package package)
        {
            DocX document = new DocX(null, null);
            document.Package = package;
            document.Document = document;

            #region MainDocumentPart
            document.MainPart = package.GetParts().Where
            (
                     p => p.ContentType.Equals(HelperFunctions.DocumentDocumenttype, StringComparison.CurrentCultureIgnoreCase) ||
                     p.ContentType.Equals(HelperFunctions.TemplateDocumenttype, StringComparison.CurrentCultureIgnoreCase)
            ).Single();

            using (TextReader tr = new StreamReader(document.MainPart.GetStream(FileMode.Open, FileAccess.Read)))
                document.MainDoc = XDocument.Load(tr, LoadOptions.PreserveWhitespace);
            #endregion

            PopulateDocument(document, package);

            using (TextReader tr = new StreamReader(document.SettingsPart.GetStream()))
                document.Settings = XDocument.Load(tr);

            document.ParagraphLookup.Clear();
            foreach (var paragraph in document.Paragraphs)
            {
                if (!document.ParagraphLookup.ContainsKey(paragraph.EndIndex))
                    document.ParagraphLookup.Add(paragraph.EndIndex, paragraph);
            }

            return document;
        }

        private static void PopulateDocument(DocX document, Package package)
        {
            Headers headers = new Headers();
            headers.Odd = document.GetHeaderByType("default");
            headers.Even = document.GetHeaderByType("even");
            headers.First = document.GetHeaderByType("first");

            Footers footers = new Footers();
            footers.Odd = document.GetFooterByType("default");
            footers.Even = document.GetFooterByType("even");
            footers.First = document.GetFooterByType("first");

            //// Get the sectPr for this document.
            //XElement sect = document.mainDoc.Descendants(XName.Get("sectPr", DocX.w.NamespaceName)).Single();

            //if (sectPr != null)
            //{
            //    // Extract the even header reference
            //    var header_even_ref = sectPr.Elements().SingleOrDefault(x => x.Name.LocalName == "headerReference" && x.Attribute(XName.Get("type", DocX.w.NamespaceName)) != null && x.Attribute(XName.Get("type", DocX.w.NamespaceName)).Value == "even");
            //    string id = header_even_ref.Attribute(XName.Get("id", DocX.r.NamespaceName)).Value;
            //    var res = document.mainPart.GetRelationship(id);
            //    string ans = res.SourceUri.OriginalString;
            //    headers.even.xml_filename = ans;

            //    // Extract the odd header reference
            //    var header_odd_ref = sectPr.Elements().SingleOrDefault(x => x.Name.LocalName == "headerReference" && x.Attribute(XName.Get("type", DocX.w.NamespaceName)) != null && x.Attribute(XName.Get("type", DocX.w.NamespaceName)).Value == "default");
            //    string id2 = header_odd_ref.Attribute(XName.Get("id", DocX.r.NamespaceName)).Value;
            //    var res2 = document.mainPart.GetRelationship(id2);
            //    string ans2 = res2.SourceUri.OriginalString;
            //    headers.odd.xml_filename = ans2;

            //    // Extract the first header reference
            //    var header_first_ref = sectPr.Elements().SingleOrDefault(x => x.Name.LocalName == "h
            //eaderReference" && x.Attribute(XName.Get("type", DocX.w.NamespaceName)) != null && x.Attribute(XName.Get("type", DocX.w.NamespaceName)).Value == "first");
            //    string id3 = header_first_ref.Attribute(XName.Get("id", DocX.r.NamespaceName)).Value;
            //    var res3 = document.mainPart.GetRelationship(id3);
            //    string ans3 = res3.SourceUri.OriginalString;
            //    headers.first.xml_filename = ans3;

            //    // Extract the even footer reference
            //    var footer_even_ref = sectPr.Elements().SingleOrDefault(x => x.Name.LocalName == "footerReference" && x.Attribute(XName.Get("type", DocX.w.NamespaceName)) != null && x.Attribute(XName.Get("type", DocX.w.NamespaceName)).Value == "even");
            //    string id4 = footer_even_ref.Attribute(XName.Get("id", DocX.r.NamespaceName)).Value;
            //    var res4 = document.mainPart.GetRelationship(id4);
            //    string ans4 = res4.SourceUri.OriginalString;
            //    footers.even.xml_filename = ans4;

            //    // Extract the odd footer reference
            //    var footer_odd_ref = sectPr.Elements().SingleOrDefault(x => x.Name.LocalName == "footerReference" && x.Attribute(XName.Get("type", DocX.w.NamespaceName)) != null && x.Attribute(XName.Get("type", DocX.w.NamespaceName)).Value == "default");
            //    string id5 = footer_odd_ref.Attribute(XName.Get("id", DocX.r.NamespaceName)).Value;
            //    var res5 = document.mainPart.GetRelationship(id5);
            //    string ans5 = res5.SourceUri.OriginalString;
            //    footers.odd.xml_filename = ans5;

            //    // Extract the first footer reference
            //    var footer_first_ref = sectPr.Elements().SingleOrDefault(x => x.Name.LocalName == "footerReference" && x.Attribute(XName.Get("type", DocX.w.NamespaceName)) != null && x.Attribute(XName.Get("type", DocX.w.NamespaceName)).Value == "first");
            //    string id6 = footer_first_ref.Attribute(XName.Get("id", DocX.r.NamespaceName)).Value;
            //    var res6 = document.mainPart.GetRelationship(id6);
            //    string ans6 = res6.SourceUri.OriginalString;
            //    footers.first.xml_filename = ans6;

            //}

            document.Xml = document.MainDoc.Root.Element(W + "body");
            document._headers = headers;
            document._footers = footers;
            document.SettingsPart = HelperFunctions.CreateOrGetSettingsPart(package);

            var ps = package.GetParts();

            //document.endnotesPart = HelperFunctions.GetPart();

            foreach (var rel in document.MainPart.GetRelationships())
            {
                string url = "/word/" + rel.TargetUri.OriginalString.Replace("/word/", "").Replace("file://", "");

                switch (rel.RelationshipType)
                {
                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/endnotes":
                        document.EndnotesPart = package.GetPart(new Uri(url, UriKind.Relative));
                        using (TextReader tr = new StreamReader(document.EndnotesPart.GetStream()))
                            document.Endnotes = XDocument.Load(tr);
                        break;

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footnotes":
                        document.FootnotesPart = package.GetPart(new Uri(url, UriKind.Relative));
                        using (TextReader tr = new StreamReader(document.FootnotesPart.GetStream()))
                            document.Footnotes = XDocument.Load(tr);
                        break;

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles":
                        document.StylesPart = package.GetPart(new Uri(url, UriKind.Relative));
                        using (TextReader tr = new StreamReader(document.StylesPart.GetStream()))
                            document.Styles = XDocument.Load(tr);
                        break;

                    case "http://schemas.microsoft.com/office/2007/relationships/stylesWithEffects":
                        document.StylesWithEffectsPart = package.GetPart(new Uri(url, UriKind.Relative));
                        using (TextReader tr = new StreamReader(document.StylesWithEffectsPart.GetStream()))
                            document.StylesWithEffects = XDocument.Load(tr);
                        break;

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/fontTable":
                        document.FontTablePart = package.GetPart(new Uri(url, UriKind.Relative));
                        using (TextReader tr = new StreamReader(document.FontTablePart.GetStream()))
                            document.FontTable = XDocument.Load(tr);
                        break;

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/numbering":
                        document.NumberingPart = package.GetPart(new Uri(url, UriKind.Relative));
                        using (TextReader tr = new StreamReader(document.NumberingPart.GetStream()))
                            document.Numbering = XDocument.Load(tr);
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Saves and copies the document into a new DocX object
        /// </summary>
        /// <returns>
        /// Returns a new DocX object with an identical document
        /// </returns>
        /// <example>
        /// <seealso cref="DocX.Load(System.IO.Stream)"/>
        /// <seealso cref="DocX.Save()"/>
        /// </example>
        public DocX Copy()
        {
            MemoryStream ms = new MemoryStream();
            SaveAs(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return Load(ms);
        }

        /// <summary>
        /// Loads a document into a DocX object using a Stream.
        /// </summary>
        /// <param name="stream">The Stream to load the document from.</param>
        /// <returns>
        /// Returns a DocX object which represents the document.
        /// </returns>
        /// <example>
        /// Loading a document from a FileStream.
        /// <code>
        /// // Open a FileStream fs to a document.
        /// using (FileStream fs = new FileStream(@"C:\Example\Test.docx", FileMode.Open))
        /// {
        ///     // Load the document using fs.
        ///     using (DocX document = DocX.Load(fs))
        ///     {
        ///         // Do something with the document here.
        ///
        ///         // Save all changes made to the document.
        ///         document.Save();
        ///     }// Release this document from memory.
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// Loading a document from a SharePoint site.
        /// <code>
        /// // Get the SharePoint site that you want to access.
        /// using (SPSite mySite = new SPSite("http://server/sites/site"))
        /// {
        ///     // Open a connection to the SharePoint site
        ///     using (SPWeb myWeb = mySite.OpenWeb())
        ///     {
        ///         // Grab a document stored on this site.
        ///         SPFile file = web.GetFile("Source_Folder_Name/Source_File");
        ///
        ///         // DocX.Load requires a Stream, so open a Stream to this document.
        ///         Stream str = new MemoryStream(file.OpenBinary());
        ///
        ///         // Load the file using the Stream str.
        ///         using (DocX document = DocX.Load(str))
        ///         {
        ///             // Do something with the document here.
        ///
        ///             // Save all changes made to the document.
        ///             document.Save();
        ///         }// Release this document from memory.
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="DocX.Load(string)"/>
        /// <seealso cref="DocX.Save()"/>
        public static DocX Load(Stream stream)
        {
            MemoryStream ms = new MemoryStream();

            stream.Position = 0;
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            ms.Write(data, 0, (int)stream.Length);

            // Open the docx package
            Package package = Package.Open(ms, FileMode.Open, FileAccess.ReadWrite);

            DocX document = PostLoad(ref package);
            document.Package = package;
            document.MemoryStream = ms;
            document.Stream = stream;
            return document;
        }

        /// <summary>
        /// Loads a document into a DocX object using a fully qualified or relative filename.
        /// </summary>
        /// <param name="filename">The fully qualified or relative filename.</param>
        /// <returns>
        /// Returns a DocX object which represents the document.
        /// </returns>
        /// <example>
        /// <code>
        /// // Load a document using its fully qualified filename
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Do something with the document here
        ///
        ///     // Save all changes made to document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// <code>
        /// // Load a document using its relative filename.
        /// using(DocX document = DocX.Load(@"..\..\Test.docx"))
        /// {
        ///     // Do something with the document here.
        ///
        ///     // Save all changes made to document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// <seealso cref="DocX.Load(System.IO.Stream)"/>
        /// <seealso cref="DocX.Save()"/>
        /// </example>
        public static DocX Load(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(string.Format("File could not be found {0}", filename));

            MemoryStream ms = new MemoryStream();

            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                CopyStream(fs, ms);
            }

            // Open the docx package
            Package package = Package.Open(ms, FileMode.Open, FileAccess.ReadWrite);

            DocX document = PostLoad(ref package);
            document.Package = package;
            document.Filename = filename;
            document.MemoryStream = ms;

            return document;
        }

        ///<summary>
        /// Applies document template to the document. Document template may include styles, headers, footers, properties, etc. as well as text content.
        ///</summary>
        ///<param name="templateFilePath">The path to the document template file.</param>
        ///<exception cref="FileNotFoundException">The document template file not found.</exception>
        public void ApplyTemplate(string templateFilePath)
        {
            ApplyTemplate(templateFilePath, true);
        }

        ///<summary>
        /// Applies document template to the document. Document template may include styles, headers, footers, properties, etc. as well as text content.
        ///</summary>
        ///<param name="templateFilePath">The path to the document template file.</param>
        ///<param name="includeContent">Whether to copy the document template text content to document.</param>
        ///<exception cref="FileNotFoundException">The document template file not found.</exception>
        public void ApplyTemplate(string templateFilePath, bool includeContent)
        {
            if (!File.Exists(templateFilePath))
            {
                throw new FileNotFoundException(string.Format("File could not be found {0}", templateFilePath));
            }
            using (FileStream packageStream = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read))
            {
                ApplyTemplate(packageStream, includeContent);
            }
        }

        ///<summary>
        /// Applies document template to the document. Document template may include styles, headers, footers, properties, etc. as well as text content.
        ///</summary>
        ///<param name="templateStream">The stream of the document template file.</param>
        public void ApplyTemplate(Stream templateStream)
        {
            ApplyTemplate(templateStream, true);
        }

        ///<summary>
        /// Applies document template to the document. Document template may include styles, headers, footers, properties, etc. as well as text content.
        ///</summary>
        ///<param name="templateStream">The stream of the document template file.</param>
        ///<param name="includeContent">Whether to copy the document template text content to document.</param>
        public void ApplyTemplate(Stream templateStream, bool includeContent)
        {
            Package templatePackage = Package.Open(templateStream);
            try
            {
                PackagePart documentPart = null;
                XDocument documentDoc = null;
                foreach (PackagePart packagePart in templatePackage.GetParts())
                {
                    switch (packagePart.Uri.ToString())
                    {
                        case "/word/document.xml":
                            documentPart = packagePart;
                            using (XmlReader xr = XmlReader.Create(packagePart.GetStream(FileMode.Open, FileAccess.Read)))
                            {
                                documentDoc = XDocument.Load(xr);
                            }
                            break;
                        case "/_rels/.rels":
                            if (!Package.PartExists(packagePart.Uri))
                            {
                                Package.CreatePart(packagePart.Uri, packagePart.ContentType, packagePart.CompressionOption);
                            }
                            PackagePart globalRelsPart = Package.GetPart(packagePart.Uri);
                            using (
                              StreamReader tr = new StreamReader(
                                packagePart.GetStream(FileMode.Open, FileAccess.Read), Encoding.UTF8))
                            {
                                using (
                                  StreamWriter tw = new StreamWriter(
                                    new PackagePartStream(globalRelsPart.GetStream(FileMode.Create, FileAccess.Write)), Encoding.UTF8))
                                {
                                    tw.Write(tr.ReadToEnd());
                                }
                            }
                            break;
                        case "/word/_rels/document.xml.rels":
                            break;
                        default:
                            if (!Package.PartExists(packagePart.Uri))
                            {
                                Package.CreatePart(packagePart.Uri, packagePart.ContentType, packagePart.CompressionOption);
                            }
                            Encoding packagePartEncoding = Encoding.Default;
                            if (packagePart.Uri.ToString().EndsWith(".xml") || packagePart.Uri.ToString().EndsWith(".rels"))
                            {
                                packagePartEncoding = Encoding.UTF8;
                            }
                            PackagePart nativePart = Package.GetPart(packagePart.Uri);
                            using (
                              StreamReader tr = new StreamReader(
                                packagePart.GetStream(FileMode.Open, FileAccess.Read), packagePartEncoding))
                            {
                                using (
                                  StreamWriter tw = new StreamWriter(
                                    new PackagePartStream(nativePart.GetStream(FileMode.Create, FileAccess.Write)), tr.CurrentEncoding))
                                {
                                    tw.Write(tr.ReadToEnd());
                                }
                            }
                            break;
                    }
                }
                if (documentPart != null)
                {
                    string mainContentType = documentPart.ContentType.Replace("template.main", "document.main");
                    if (Package.PartExists(documentPart.Uri))
                    {
                        Package.DeletePart(documentPart.Uri);
                    }
                    PackagePart documentNewPart = Package.CreatePart(
                      documentPart.Uri, mainContentType, documentPart.CompressionOption);
                    using (XmlWriter xw = XmlWriter.Create(new PackagePartStream(documentNewPart.GetStream(FileMode.Create, FileAccess.Write))))
                    {
                        documentDoc.WriteTo(xw);
                    }
                    foreach (PackageRelationship documentPartRel in documentPart.GetRelationships())
                    {
                        documentNewPart.CreateRelationship(
                          documentPartRel.TargetUri,
                          documentPartRel.TargetMode,
                          documentPartRel.RelationshipType,
                          documentPartRel.Id);
                    }
                    MainPart = documentNewPart;
                    MainDoc = documentDoc;
                    PopulateDocument(this, templatePackage);

                    // DragonFire: I added next line and recovered ApplyTemplate method.
                    // I do it, becouse  PopulateDocument(...) writes into field "settingsPart" the part of Template's package
                    //  and after line "templatePackage.Close();" in finally, field "settingsPart" becomes not available and method "Save" throw an exception...
                    // That's why I recreated settingsParts and unlinked it from Template's package =)
                    SettingsPart = HelperFunctions.CreateOrGetSettingsPart(Package);
                }
                if (!includeContent)
                {
                    foreach (Paragraph paragraph in Paragraphs)
                    {
                        paragraph.Remove(false);
                    }
                }
            }
            finally
            {
                Package.Flush();
                var documentRelsPart = Package.GetPart(new Uri("/word/_rels/document.xml.rels", UriKind.Relative));
                using (TextReader tr = new StreamReader(documentRelsPart.GetStream(FileMode.Open, FileAccess.Read)))
                {
                    tr.Read();
                }
                templatePackage.Close();
                PopulateDocument(Document, Package);
            }
        }

        /// <summary>
        /// Add an Image into this document from a fully qualified or relative filename.
        /// </summary>
        /// <param name="filename">The fully qualified or relative filename.</param>
        /// <param name="contentType">MIME type of image, guessed if not given.</param>
        /// <returns>An Image file.</returns>
        /// <example>
        /// Add an Image into this document from a fully qualified filename.
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Add an Image from a file.
        ///     document.AddImage(@"C:\Example\Image.png");
        ///
        ///     // Save all changes made to this document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        /// <seealso cref="AddImage(System.IO.Stream, string)"/>
        /// <seealso cref="Paragraph.InsertPicture"/>
        public Image AddImage(string filename, string contentType = "image/jpeg")
        {
            if (string.IsNullOrEmpty(contentType))
            {
                // The extension this file has will be taken to be its format.
                switch (Path.GetExtension(filename))
                {
                    case ".tiff": contentType = "image/tif"; break;
                    case ".tif": contentType = "image/tif"; break;
                    case ".png": contentType = "image/png"; break;
                    case ".bmp": contentType = "image/png"; break;
                    case ".gif": contentType = "image/gif"; break;
                    case ".jpg": contentType = "image/jpg"; break;
                    case ".jpeg": contentType = "image/jpeg"; break;
                    default: contentType = "image/jpg"; break;
                }
            }

            return AddImage(filename, contentType);
        }

        /// <summary>
        /// Add an Image into this document from a Stream.
        /// </summary>
        /// <param name="stream">A Stream stream.</param>
        /// <param name="contentType">MIME type of image.</param>
        /// <returns>An Image file.</returns>
        /// <example>
        /// Add an Image into a document using a Stream.
        /// <code>
        /// // Open a FileStream fs to an Image.
        /// using (FileStream fs = new FileStream(@"C:\Example\Image.jpg", FileMode.Open))
        /// {
        ///     // Load a document.
        ///     using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        ///     {
        ///         // Add an Image from a filestream fs.
        ///         document.AddImage(fs);
        ///
        ///         // Save all changes made to this document.
        ///         document.Save();
        ///     }// Release this document from memory.
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="AddImage(string, string)"/>
        /// <seealso cref="Paragraph.InsertPicture"/>
        public Image AddImage(Stream stream, string contentType = "image/jpeg")
        {
            return AddImage(stream as object, contentType);
        }

        /// <summary>
        /// Adds a hyperlink to a document and creates a Paragraph which uses it.
        /// </summary>
        /// <param name="text">The text as displayed by the hyperlink.</param>
        /// <param name="uri">The hyperlink itself.</param>
        /// <returns>Returns a hyperlink that can be inserted into a Paragraph.</returns>
        /// <example>
        /// Adds a hyperlink to a document and creates a Paragraph which uses it.
        /// <code>
        /// // Create a document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///    // Add a hyperlink to this document.
        ///    Hyperlink h = document.AddHyperlink("Google", new Uri("http://www.google.com"));
        ///
        ///    // Add a new Paragraph to this document.
        ///    Paragraph p = document.InsertParagraph();
        ///    p.Append("My favourite search engine is ");
        ///    p.AppendHyperlink(h);
        ///    p.Append(", I think it's great.");
        ///
        ///    // Save all changes made to this document.
        ///    document.Save();
        /// }
        /// </code>
        /// </example>
        public Hyperlink AddHyperlink(string text, Uri uri)
        {
            XElement i = new XElement
            (
                XName.Get("hyperlink", W.NamespaceName),
                new XAttribute(R + "id", string.Empty),
                new XAttribute(W + "history", "1"),
                new XElement(XName.Get("r", W.NamespaceName),
                new XElement(XName.Get("rPr", W.NamespaceName),
                new XElement(XName.Get("rStyle", W.NamespaceName),
                new XAttribute(W + "val", "Hyperlink"))),
                new XElement(XName.Get("t", W.NamespaceName), text))
            );

            Hyperlink h = new Hyperlink(this, MainPart, i);

            h.text = text;
            h.uri = uri;

            AddHyperlinkStyleIfNotPresent();

            return h;
        }

        internal void AddHyperlinkStyleIfNotPresent()
        {
            Uri wordStylesUri = new Uri("/word/styles.xml", UriKind.Relative);

            // If the internal document contains no /word/styles.xml create one.
            if (!Package.PartExists(wordStylesUri))
                HelperFunctions.AddDefaultStylesXml(Package);

            // Load the styles.xml into memory.
            XDocument wordStyles;
            using (TextReader tr = new StreamReader(Package.GetPart(wordStylesUri).GetStream()))
                wordStyles = XDocument.Load(tr);

            bool hyperlinkStyleExists =
            (
                from s in wordStyles.Element(W + "styles").Elements()
                let styleId = s.Attribute(XName.Get("styleId", W.NamespaceName))
                where (styleId != null && styleId.Value == "Hyperlink")
                select s
            ).Count() > 0;

            if (!hyperlinkStyleExists)
            {
                XElement style = new XElement
                (
                    W + "style",
                    new XAttribute(W + "type", "character"),
                    new XAttribute(W + "styleId", "Hyperlink"),
                        new XElement(W + "name", new XAttribute(W + "val", "Hyperlink")),
                        new XElement(W + "basedOn", new XAttribute(W + "val", "DefaultParagraphFont")),
                        new XElement(W + "uiPriority", new XAttribute(W + "val", "99")),
                        new XElement(W + "unhideWhenUsed"),
                        new XElement(W + "rsid", new XAttribute(W + "val", "0005416C")),
                        new XElement
                        (
                            W + "rPr",
                            new XElement(W + "color", new XAttribute(W + "val", "0000FF"), new XAttribute(W + "themeColor", "hyperlink")),
                            new XElement
                            (
                                W + "u",
                                new XAttribute(W + "val", "single")
                            )
                        )
                );
                wordStyles.Element(W + "styles").Add(style);

                // Save the styles document.
                using (TextWriter tw = new StreamWriter(new PackagePartStream(Package.GetPart(wordStylesUri).GetStream())))
                    wordStyles.Save(tw);
            }
        }

        private string GetNextFreeRelationshipId()
        {
            int id = (
                 from r in MainPart.GetRelationships()
                 where r.Id.Substring(0, 3).Equals("rId")
                 select int.Parse(r.Id.Substring(3))
             ).DefaultIfEmpty().Max();

            // The conventiom for ids is rid01, rid02, etc
            string newId = id.ToString();
            int result;
            if (int.TryParse(newId, out result))
                return ("rId" + (result + 1));
            String guid = String.Empty;
            do
            {
                guid = Guid.NewGuid().ToString();
            } while (Char.IsDigit(guid[0]));
            return guid;
        }

        /// <summary>
        /// Adds three new Headers to this document. One for the first page, one for odd pages and one for even pages.
        /// </summary>
        /// <example>
        /// // Create a document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Add header support to this document.
        ///     document.AddHeaders();
        ///
        ///     // Get a collection of all headers in this document.
        ///     Headers headers = document.Headers;
        ///
        ///     // The header used for the first page of this document.
        ///     Header first = headers.first;
        ///
        ///     // The header used for odd pages of this document.
        ///     Header odd = headers.odd;
        ///
        ///     // The header used for even pages of this document.
        ///     Header even = headers.even;
        ///
        ///     // Force the document to use a different header for first, odd and even pages.
        ///     document.DifferentFirstPage = true;
        ///     document.DifferentOddAndEvenPages = true;
        ///
        ///     // Content can be added to the Headers in the same manor that it would be added to the main document.
        ///     Paragraph p = first.InsertParagraph();
        ///     p.Append("This is the first pages header.");
        ///
        ///     // Save all changes to this document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </example>
        public void AddHeaders()
        {
            AddHeadersOrFooters(true);

            _headers.Odd = Document.GetHeaderByType("default");
            _headers.Even = Document.GetHeaderByType("even");
            _headers.First = Document.GetHeaderByType("first");
        }

        /// <summary>
        /// Adds three new Footers to this document. One for the first page, one for odd pages and one for even pages.
        /// </summary>
        /// <example>
        /// // Create a document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///     // Add footer support to this document.
        ///     document.AddFooters();
        ///
        ///     // Get a collection of all footers in this document.
        ///     Footers footers = document.Footers;
        ///
        ///     // The footer used for the first page of this document.
        ///     Footer first = footers.first;
        ///
        ///     // The footer used for odd pages of this document.
        ///     Footer odd = footers.odd;
        ///
        ///     // The footer used for even pages of this document.
        ///     Footer even = footers.even;
        ///
        ///     // Force the document to use a different footer for first, odd and even pages.
        ///     document.DifferentFirstPage = true;
        ///     document.DifferentOddAndEvenPages = true;
        ///
        ///     // Content can be added to the Footers in the same manor that it would be added to the main document.
        ///     Paragraph p = first.InsertParagraph();
        ///     p.Append("This is the first pages footer.");
        ///
        ///     // Save all changes to this document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </example>
        public void AddFooters()
        {
            AddHeadersOrFooters(false);

            _footers.Odd = Document.GetFooterByType("default");
            _footers.Even = Document.GetFooterByType("even");
            _footers.First = Document.GetFooterByType("first");
        }

        /// <summary>
        /// Adds a Header to a document.
        /// If the document already contains a Header it will be replaced.
        /// </summary>
        /// <returns>The Header that was added to the document.</returns>
        internal void AddHeadersOrFooters(bool b)
        {
            string element = "ftr";
            string reference = "footer";
            if (b)
            {
                element = "hdr";
                reference = "header";
            }

            DeleteHeadersOrFooters(b);

            XElement sectPr = MainDoc.Root.Element(W + "body").Element(W + "sectPr");

            for (int i = 1; i < 4; i++)
            {
                string headerUri = string.Format("/word/{0}{1}.xml", reference, i);

                PackagePart headerPart = Package.CreatePart(new Uri(headerUri, UriKind.Relative), string.Format("application/vnd.openxmlformats-officedocument.wordprocessingml.{0}+xml", reference), CompressionOption.Normal);
                PackageRelationship headerRelationship = MainPart.CreateRelationship(headerPart.Uri, TargetMode.Internal, string.Format("http://schemas.openxmlformats.org/officeDocument/2006/relationships/{0}", reference));

                XDocument header;

                // Load the document part into a XDocument object
                using (TextReader tr = new StreamReader(headerPart.GetStream(FileMode.Create, FileAccess.ReadWrite)))
                {
                    header = XDocument.Parse
                    (string.Format(@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                       <w:{0} xmlns:ve=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:m=""http://schemas.openxmlformats.org/officeDocument/2006/math"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:wp=""http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing"" xmlns:w10=""urn:schemas-microsoft-com:office:word"" xmlns:w=""http://schemas.openxmlformats.org/wordprocessingml/2006/main"" xmlns:wne=""http://schemas.microsoft.com/office/word/2006/wordml"">
                         <w:p w:rsidR=""009D472B"" w:rsidRDefault=""009D472B"">
                           <w:pPr>
                             <w:pStyle w:val=""{1}"" />
                           </w:pPr>
                         </w:p>
                       </w:{0}>", element, reference)
                    );
                }

                // Save the main document
                using (TextWriter tw = new StreamWriter(new PackagePartStream(headerPart.GetStream(FileMode.Create, FileAccess.Write))))
                    header.Save(tw, SaveOptions.None);

                string type;
                switch (i)
                {
                    case 1: type = "default"; break;
                    case 2: type = "even"; break;
                    case 3: type = "first"; break;
                    default: throw new ArgumentOutOfRangeException();
                }

                sectPr.Add
                (
                    new XElement
                    (
                        W + string.Format("{0}Reference", reference),
                        new XAttribute(W + "type", type),
                        new XAttribute(R + "id", headerRelationship.Id)
                    )
                );
            }
        }

        internal void DeleteHeadersOrFooters(bool b)
        {
            string reference = "footer";
            if (b)
                reference = "header";

            // Get all header Relationships in this document.
            var headerRelationships = MainPart.GetRelationshipsByType(string.Format("http://schemas.openxmlformats.org/officeDocument/2006/relationships/{0}", reference));

            foreach (PackageRelationship headerRelationship in headerRelationships)
            {
                // Get the TargetUri for this Part.
                Uri headerUri = headerRelationship.TargetUri;

                // Check to see if the document actually contains the Part.
                if (!headerUri.OriginalString.StartsWith("/word/"))
                    headerUri = new Uri("/word/" + headerUri.OriginalString, UriKind.Relative);

                if (Package.PartExists(headerUri))
                {
                    // Delete the Part
                    Package.DeletePart(headerUri);

                    // Get all references to this Relationship in the document.
                    var query =
                    (
                        from e in MainDoc.Descendants(XName.Get("body", W.NamespaceName)).Descendants()
                        where (e.Name.LocalName == string.Format("{0}Reference", reference)) && (e.Attribute(R + "id").Value == headerRelationship.Id)
                        select e
                    );

                    // Remove all references to this Relationship in the document.
                    for (int i = 0; i < query.Count(); i++)
                        query.ElementAt(i).Remove();

                    // Delete the Relationship.
                    Package.DeleteRelationship(headerRelationship.Id);
                }
            }
        }

        internal Image AddImage(object o, string contentType = "image/jpeg")
        {
            // Open a Stream to the new image being added.
            Stream newImageStream;
            if (o is string)
                newImageStream = new FileStream(o as string, FileMode.Open, FileAccess.Read);
            else
                newImageStream = o as Stream;

            // Get all image parts in word\document.xml
            PackagePartCollection packagePartCollection = Package.GetParts();

            // Cache Uri.ToString which is expensive to be used in two loops
            var parts = packagePartCollection.Select(x => new
            {
                UriString = x.Uri.ToString(),
                Part = x
            }).ToList();

            var partLookup = parts.ToDictionary(x => x.UriString, x => x.Part, StringComparer.Ordinal);

            // Gather results manually to minimize closure allocation overhead
            List<PackagePart> imageParts = new List<PackagePart>();
            foreach (var ir in MainPart.GetRelationshipsByType(RelationshipImage))
            {
                var targetUri = ir.TargetUri.ToString();
                PackagePart part;
                if (partLookup.TryGetValue(targetUri, out part))
                {
                    imageParts.Add(part);
                }
            }

            IEnumerable<PackagePart> relsParts = parts
                .Where(
                    part =>
                        part.Part.ContentType.Equals(ContentTypeApplicationRelationShipXml, StringComparison.Ordinal) &&
                        part.UriString.IndexOf("/word/", StringComparison.Ordinal) > -1)
                .Select(part => part.Part);

            XName xNameTarget = XName.Get("Target");
            XName xNameTargetMode = XName.Get("TargetMode");

            foreach (PackagePart relsPart in relsParts)
            {
                XDocument relsPartContent;
                using (TextReader tr = new StreamReader(relsPart.GetStream(FileMode.Open, FileAccess.Read)))
                {
                    relsPartContent = XDocument.Load(tr);
                }

                IEnumerable<XElement> imageRelationships = relsPartContent.Root.Elements()
                    .Where(imageRel => imageRel.Attribute(XName.Get("Type")).Value.Equals(RelationshipImage));

                foreach (XElement imageRelationship in imageRelationships)
                {
                    XAttribute attribute = imageRelationship.Attribute(xNameTarget);
                    if (attribute != null)
                    {
                        string targetMode = string.Empty;

                        XAttribute targetModeAttibute = imageRelationship.Attribute(xNameTargetMode);
                        if (targetModeAttibute != null)
                        {
                            targetMode = targetModeAttibute.Value;
                        }

                        if (!targetMode.Equals("External"))
                        {
                            string imagePartUri = Path.Combine(Path.GetDirectoryName(relsPart.Uri.ToString()),
                                attribute.Value);
                            imagePartUri = Path.GetFullPath(imagePartUri.Replace("\\_rels", string.Empty));
                            imagePartUri = imagePartUri.Replace(Path.GetFullPath("\\"), string.Empty).Replace("\\", "/");

                            if (!imagePartUri.StartsWith("/"))
                            {
                                imagePartUri = "/" + imagePartUri;
                            }

                            PackagePart imagePart = Package.GetPart(new Uri(imagePartUri, UriKind.Relative));
                            imageParts.Add(imagePart);
                        }
                    }
                }
            }

            // Loop through each image part in this document.
            foreach (PackagePart pp in imageParts)
            {
                // Get the image object for this image part
                // Open a tempory Stream to this image part.
                using (Stream tempStream = pp.GetStream(FileMode.Open, FileAccess.Read))
                {
                    // Compare this image to the new image being added.
                    if (HelperFunctions.IsSameFile(tempStream, newImageStream))
                    {
                        // Return the Image object
                        PackageRelationship relationship = MainPart.GetRelationshipsByType(RelationshipImage)
                            .First(x => x.TargetUri == pp.Uri);

                        return new Image(this, relationship);
                    }
                }
            }

            string imgPartUriPath = string.Empty;
            string extension = contentType.Substring(contentType.LastIndexOf("/") + 1);
            do
            {
                // Create a new image part.
                imgPartUriPath = string.Format
                (
                    "/word/media/{0}.{1}",
                    Guid.NewGuid(), // The unique part.
                    extension
                );
            } while (Package.PartExists(new Uri(imgPartUriPath, UriKind.Relative)));

            // We are now guareenteed that imgPartUriPath is unique.
            PackagePart img = Package.CreatePart(new Uri(imgPartUriPath, UriKind.Relative), contentType,
                CompressionOption.Normal);

            // Create a new image relationship
            PackageRelationship rel = MainPart.CreateRelationship(img.Uri, TargetMode.Internal, RelationshipImage);

            // Open a Stream to the newly created Image part.
            using (Stream stream = new PackagePartStream(img.GetStream(FileMode.Create, FileAccess.Write)))
            {
                // Using the Stream to the real image, copy this streams data into the newly create Image part.
                using (newImageStream)
                {
                    CopyStream(newImageStream, stream, bufferSize: 4096);
                } // Close the Stream to the new image.
            } // Close the Stream to the new image part.

            return new Image(this, rel);
        }

        /// <summary>
        /// Save this document back to the location it was loaded from.
        /// </summary>
        /// <example>
        /// <code>
        /// // Load a document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // Add an Image from a file.
        ///     document.AddImage(@"C:\Example\Image.jpg");
        ///
        ///     // Save all changes made to this document.
        ///     document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        /// <seealso cref="DocX.SaveAs(string)"/>
        /// <seealso cref="DocX.Load(System.IO.Stream)"/>
        /// <seealso cref="DocX.Load(string)"/>
        /// <!--
        /// Bug found and fixed by krugs525 on August 12 2009.
        /// Use TFS compare to see exact code change.
        /// -->
        public void Save()
        {
            Headers headers = Headers;

            // Save the main document
            using (TextWriter tw = new StreamWriter(new PackagePartStream(MainPart.GetStream(FileMode.Create, FileAccess.Write))))
                MainDoc.Save(tw, SaveOptions.None);

            if (Settings == null)
            {
                using (TextReader tr = new StreamReader(SettingsPart.GetStream()))
                    Settings = XDocument.Load(tr);
            }

            XElement body = MainDoc.Root.Element(W + "body");
            XElement sectPr = body.Descendants(W + "sectPr").FirstOrDefault();

            if (sectPr != null)
            {
                var evenHeaderRef =
                (
                    from e in MainDoc.Descendants(W + "headerReference")
                    let type = e.Attribute(W + "type")
                    where type != null && type.Value.Equals("even", StringComparison.CurrentCultureIgnoreCase)
                    select e.Attribute(R + "id").Value
                 ).LastOrDefault();

                if (evenHeaderRef != null)
                {
                    XElement even = headers.Even.Xml;

                    Uri target = PackUriHelper.ResolvePartUri
                    (
                        MainPart.Uri,
                        MainPart.GetRelationship(evenHeaderRef).TargetUri
                    );

                    using (TextWriter tw = new StreamWriter(new PackagePartStream(Package.GetPart(target).GetStream(FileMode.Create, FileAccess.Write))))
                    {
                        new XDocument
                        (
                            new XDeclaration("1.0", "UTF-8", "yes"),
                            even
                        ).Save(tw, SaveOptions.None);
                    }
                }

                var oddHeaderRef =
                (
                    from e in MainDoc.Descendants(W + "headerReference")
                    let type = e.Attribute(W + "type")
                    where type != null && type.Value.Equals("default", StringComparison.CurrentCultureIgnoreCase)
                    select e.Attribute(R + "id").Value
                 ).LastOrDefault();

                if (oddHeaderRef != null)
                {
                    XElement odd = headers.Odd.Xml;

                    Uri target = PackUriHelper.ResolvePartUri
                    (
                        MainPart.Uri,
                        MainPart.GetRelationship(oddHeaderRef).TargetUri
                    );

                    // Save header1
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(Package.GetPart(target).GetStream(FileMode.Create, FileAccess.Write))))
                    {
                        new XDocument
                        (
                            new XDeclaration("1.0", "UTF-8", "yes"),
                            odd
                        ).Save(tw, SaveOptions.None);
                    }
                }

                var firstHeaderRef =
                (
                    from e in MainDoc.Descendants(W + "headerReference")
                    let type = e.Attribute(W + "type")
                    where type != null && type.Value.Equals("first", StringComparison.CurrentCultureIgnoreCase)
                    select e.Attribute(R + "id").Value
                 ).LastOrDefault();

                if (firstHeaderRef != null)
                {
                    XElement first = headers.First.Xml;
                    Uri target = PackUriHelper.ResolvePartUri
                    (
                        MainPart.Uri,
                        MainPart.GetRelationship(firstHeaderRef).TargetUri
                    );

                    // Save header3
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(Package.GetPart(target).GetStream(FileMode.Create, FileAccess.Write))))
                    {
                        new XDocument
                        (
                            new XDeclaration("1.0", "UTF-8", "yes"),
                            first
                        ).Save(tw, SaveOptions.None);
                    }
                }

                var oddFooterRef =
                (
                    from e in MainDoc.Descendants(W + "footerReference")
                    let type = e.Attribute(W + "type")
                    where type != null && type.Value.Equals("default", StringComparison.CurrentCultureIgnoreCase)
                    select e.Attribute(R + "id").Value
                 ).LastOrDefault();

                if (oddFooterRef != null)
                {
                    XElement odd = _footers.Odd.Xml;
                    Uri target = PackUriHelper.ResolvePartUri
                    (
                        MainPart.Uri,
                        MainPart.GetRelationship(oddFooterRef).TargetUri
                    );

                    // Save header1
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(Package.GetPart(target).GetStream(FileMode.Create, FileAccess.Write))))
                    {
                        new XDocument
                        (
                            new XDeclaration("1.0", "UTF-8", "yes"),
                            odd
                        ).Save(tw, SaveOptions.None);
                    }
                }

                var evenFooterRef =
                (
                    from e in MainDoc.Descendants(W + "footerReference")
                    let type = e.Attribute(W + "type")
                    where type != null && type.Value.Equals("even", StringComparison.CurrentCultureIgnoreCase)
                    select e.Attribute(R + "id").Value
                 ).LastOrDefault();

                if (evenFooterRef != null)
                {
                    XElement even = _footers.Even.Xml;
                    Uri target = PackUriHelper.ResolvePartUri
                    (
                        MainPart.Uri,
                        MainPart.GetRelationship(evenFooterRef).TargetUri
                    );

                    // Save header2
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(Package.GetPart(target).GetStream(FileMode.Create, FileAccess.Write))))
                    {
                        new XDocument
                        (
                            new XDeclaration("1.0", "UTF-8", "yes"),
                            even
                        ).Save(tw, SaveOptions.None);
                    }
                }

                var firstFooterRef =
                (
                     from e in MainDoc.Descendants(W + "footerReference")
                     let type = e.Attribute(W + "type")
                     where type != null && type.Value.Equals("first", StringComparison.CurrentCultureIgnoreCase)
                     select e.Attribute(R + "id").Value
                ).LastOrDefault();

                if (firstFooterRef != null)
                {
                    XElement first = _footers.First.Xml;
                    Uri target = PackUriHelper.ResolvePartUri
                    (
                        MainPart.Uri,
                        MainPart.GetRelationship(firstFooterRef).TargetUri
                    );

                    // Save header3
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(Package.GetPart(target).GetStream(FileMode.Create, FileAccess.Write))))
                    {
                        new XDocument
                        (
                            new XDeclaration("1.0", "UTF-8", "yes"),
                            first
                        ).Save(tw, SaveOptions.None);
                    }
                }

                // Save the settings document.
                using (TextWriter tw = new StreamWriter(new PackagePartStream(SettingsPart.GetStream(FileMode.Create, FileAccess.Write))))
                    Settings.Save(tw, SaveOptions.None);

                if (EndnotesPart != null)
                {
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(EndnotesPart.GetStream(FileMode.Create, FileAccess.Write))))
                        Endnotes.Save(tw, SaveOptions.None);
                }

                if (FootnotesPart != null)
                {
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(FootnotesPart.GetStream(FileMode.Create, FileAccess.Write))))
                        Footnotes.Save(tw, SaveOptions.None);
                }

                if (StylesPart != null)
                {
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(StylesPart.GetStream(FileMode.Create, FileAccess.Write))))
                        Styles.Save(tw, SaveOptions.None);
                }

                if (StylesWithEffectsPart != null)
                {
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(StylesWithEffectsPart.GetStream(FileMode.Create, FileAccess.Write))))
                        StylesWithEffects.Save(tw, SaveOptions.None);
                }

                if (NumberingPart != null)
                {
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(NumberingPart.GetStream(FileMode.Create, FileAccess.Write))))
                        Numbering.Save(tw, SaveOptions.None);
                }

                if (FontTablePart != null)
                {
                    using (TextWriter tw = new StreamWriter(new PackagePartStream(FontTablePart.GetStream(FileMode.Create, FileAccess.Write))))
                        FontTable.Save(tw, SaveOptions.None);
                }
            }

            // Close the document so that it can be saved.
            Package.Flush();

            #region Save this document back to a file or stream, that was specified by the user at save time.
            if (Filename != null)
            {
                using (FileStream fs = new FileStream(Filename, FileMode.Create))
                {
					// Original code
					// fs.Write( memoryStream.ToArray(), 0, (int)memoryStream.Length );
					// was replaced by save using small buffer
					// CopyStream( memoryStream, fs);
					// Corection is to make position equal to 0
					if(MemoryStream.CanSeek)
					{
						// Write to the beginning of the stream
						MemoryStream.Position = 0;
						CopyStream(MemoryStream, fs);
					}
					else
						fs.Write(MemoryStream.ToArray(), 0, (int)MemoryStream.Length);
				}
            }
            else
            {
                if (Stream.CanSeek) // 2013-05-25: Check if stream can be seeked to support System.Web.HttpResponseStream
                {
                    // Set the length of this stream to 0
                    Stream.SetLength(0);

                    // Write to the beginning of the stream
                    Stream.Position = 0;
                }

                MemoryStream.WriteTo(Stream);
                MemoryStream.Flush();
            }
            #endregion
        }

        /// <summary>
        /// Save this document to a file.
        /// </summary>
        /// <param name="filename">The filename to save this document as.</param>
        /// <example>
        /// Load a document from one file and save it to another.
        /// <code>
        /// // Load a document using its fully qualified filename.
        /// DocX document = DocX.Load(@"C:\Example\Test1.docx");
        ///
        /// // Insert a new Paragraph
        /// document.InsertParagraph("Hello world!", false);
        ///
        /// // Save the document to a new location.
        /// document.SaveAs(@"C:\Example\Test2.docx");
        /// </code>
        /// </example>
        /// <example>
        /// Load a document from a Stream and save it to a file.
        /// <code>
        /// DocX document;
        /// using (FileStream fs1 = new FileStream(@"C:\Example\Test1.docx", FileMode.Open))
        /// {
        ///     // Load a document using a stream.
        ///     document = DocX.Load(fs1);
        ///
        ///     // Insert a new Paragraph
        ///     document.InsertParagraph("Hello world again!", false);
        /// }
        ///
        /// // Save the document to a new location.
        /// document.SaveAs(@"C:\Example\Test2.docx");
        /// </code>
        /// </example>
        /// <seealso cref="DocX.Save()"/>
        /// <seealso cref="DocX.Load(System.IO.Stream)"/>
        /// <seealso cref="DocX.Load(string)"/>
        public void SaveAs(string filename)
        {
            this.Filename = filename;
            Stream = null;
            Save();
        }

        /// <summary>
        /// Save this document to a Stream.
        /// </summary>
        /// <param name="stream">The Stream to save this document to.</param>
        /// <example>
        /// Load a document from a file and save it to a Stream.
        /// <code>
        /// // Place holder for a document.
        /// DocX document;
        ///
        /// using (FileStream fs1 = new FileStream(@"C:\Example\Test1.docx", FileMode.Open))
        /// {
        ///     // Load a document using a stream.
        ///     document = DocX.Load(fs1);
        ///
        ///     // Insert a new Paragraph
        ///     document.InsertParagraph("Hello world again!", false);
        /// }
        ///
        /// using (FileStream fs2 = new FileStream(@"C:\Example\Test2.docx", FileMode.Create))
        /// {
        ///     // Save the document to a different stream.
        ///     document.SaveAs(fs2);
        /// }
        ///
        /// // Release this document from memory.
        /// document.Dispose();
        /// </code>
        /// </example>
        /// <example>
        /// Load a document from one Stream and save it to another.
        /// <code>
        /// DocX document;
        /// using (FileStream fs1 = new FileStream(@"C:\Example\Test1.docx", FileMode.Open))
        /// {
        ///     // Load a document using a stream.
        ///     document = DocX.Load(fs1);
        ///
        ///     // Insert a new Paragraph
        ///     document.InsertParagraph("Hello world again!", false);
        /// }
        ///
        /// using (FileStream fs2 = new FileStream(@"C:\Example\Test2.docx", FileMode.Create))
        /// {
        ///     // Save the document to a different stream.
        ///     document.SaveAs(fs2);
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="DocX.Save()"/>
        /// <seealso cref="DocX.Load(System.IO.Stream)"/>
        /// <seealso cref="DocX.Load(string)"/>
        public void SaveAs(Stream stream)
        {
            Filename = null;
            this.Stream = stream;
            Save();
        }

        /// <summary>
        /// Add a core property to this document. If a core property already exists with the same name it will be replaced. Core property names are case insensitive.
        /// </summary>
        ///<param name="propertyName">The property name.</param>
        ///<param name="propertyValue">The property value.</param>
        ///<example>
        /// Add a core properties of each type to a document.
        /// <code>
        /// // Load Example.docx
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // If this document does not contain a core property called 'forename', create one.
        ///     if (!document.CoreProperties.ContainsKey("forename"))
        ///     {
        ///         // Create a new core property called 'forename' and set its value.
        ///         document.AddCoreProperty("forename", "Cathal");
        ///     }
        ///
        ///     // Get this documents core property called 'forename'.
        ///     string forenameValue = document.CoreProperties["forename"];
        ///
        ///     // Print all of the information about this core property to Console.
        ///     Console.WriteLine(string.Format("Name: '{0}', Value: '{1}'\nPress any key...", "forename", forenameValue));
        ///
        ///     // Save all changes made to this document.
        ///     document.Save();
        /// } // Release this document from memory.
        ///
        /// // Wait for the user to press a key before exiting.
        /// Console.ReadKey();
        /// </code>
        /// </example>
        /// <seealso cref="CoreProperties"/>
        /// <seealso cref="CustomProperty"/>
        /// <seealso cref="CustomProperties"/>
        public void AddCoreProperty(string propertyName, string propertyValue)
        {
            string propertyNamespacePrefix = propertyName.Contains(":") ? propertyName.Split(':')[0] : "cp";
            string propertyLocalName = propertyName.Contains(":") ? propertyName.Split(':')[1] : propertyName;

            // If this document does not contain a coreFilePropertyPart create one.)
            if (!Package.PartExists(new Uri("/docProps/core.xml", UriKind.Relative)))
                throw new Exception("Core properties part doesn't exist.");

            XDocument corePropDoc;
            PackagePart corePropPart = Package.GetPart(new Uri("/docProps/core.xml", UriKind.Relative));
            using (TextReader tr = new StreamReader(corePropPart.GetStream(FileMode.Open, FileAccess.Read)))
            {
                corePropDoc = XDocument.Load(tr);
            }

            XElement corePropElement =
              (from propElement in corePropDoc.Root.Elements()
               where (propElement.Name.LocalName.Equals(propertyLocalName))
               select propElement).SingleOrDefault();
            if (corePropElement != null)
            {
                corePropElement.SetValue(propertyValue);
            }
            else
            {
                var propertyNamespace = corePropDoc.Root.GetNamespaceOfPrefix(propertyNamespacePrefix);
                corePropDoc.Root.Add(new XElement(XName.Get(propertyLocalName, propertyNamespace.NamespaceName), propertyValue));
            }

            using (TextWriter tw = new StreamWriter(new PackagePartStream(corePropPart.GetStream(FileMode.Create, FileAccess.Write))))
            {
                corePropDoc.Save(tw);
            }
            UpdateCorePropertyValue(this, propertyLocalName, propertyValue);
        }

        internal static void UpdateCorePropertyValue(DocX document, string corePropertyName, string corePropertyValue)
        {
            string matchPattern = string.Format(@"(DOCPROPERTY)?{0}\\\*MERGEFORMAT", corePropertyName).ToLower();
            foreach (XElement e in document.MainDoc.Descendants(XName.Get("fldSimple", W.NamespaceName)))
            {
                string attrValue = e.Attribute(XName.Get("instr", W.NamespaceName)).Value.Replace(" ", string.Empty).Trim().ToLower();

                if (Regex.IsMatch(attrValue, matchPattern))
                {
                    XElement firstRun = e.Element(W + "r");
                    XElement firstText = firstRun.Element(W + "t");
                    XElement rPr = firstText.Element(W + "rPr");

                    // Delete everything and insert updated text value
                    e.RemoveNodes();

                    XElement t = new XElement(W + "t", rPr, corePropertyValue);
                    Novacode.Text.PreserveSpace(t);
                    e.Add(new XElement(firstRun.Name, firstRun.Attributes(), firstRun.Element(XName.Get("rPr", W.NamespaceName)), t));
                }
            }

            #region Headers

            IEnumerable<PackagePart> headerParts = from headerPart in document.Package.GetParts()
                                                   where (Regex.IsMatch(headerPart.Uri.ToString(), @"/word/header\d?.xml"))
                                                   select headerPart;
            foreach (PackagePart pp in headerParts)
            {
                XDocument header = XDocument.Load(new StreamReader(pp.GetStream()));

                foreach (XElement e in header.Descendants(XName.Get("fldSimple", W.NamespaceName)))
                {
                    string attrValue = e.Attribute(XName.Get("instr", W.NamespaceName)).Value.Replace(" ", string.Empty).Trim().ToLower();
                    if (Regex.IsMatch(attrValue, matchPattern))
                    {
                        XElement firstRun = e.Element(W + "r");

                        // Delete everything and insert updated text value
                        e.RemoveNodes();

                        XElement t = new XElement(W + "t", corePropertyValue);
                        Novacode.Text.PreserveSpace(t);
                        e.Add(new XElement(firstRun.Name, firstRun.Attributes(), firstRun.Element(XName.Get("rPr", W.NamespaceName)), t));
                    }
                }

                using (TextWriter tw = new StreamWriter(new PackagePartStream(pp.GetStream(FileMode.Create, FileAccess.Write))))
                    header.Save(tw);
            }
            #endregion

            #region Footers
            IEnumerable<PackagePart> footerParts = from footerPart in document.Package.GetParts()
                                                   where (Regex.IsMatch(footerPart.Uri.ToString(), @"/word/footer\d?.xml"))
                                                   select footerPart;
            foreach (PackagePart pp in footerParts)
            {
                XDocument footer = XDocument.Load(new StreamReader(pp.GetStream()));

                foreach (XElement e in footer.Descendants(XName.Get("fldSimple", W.NamespaceName)))
                {
                    string attrValue = e.Attribute(XName.Get("instr", W.NamespaceName)).Value.Replace(" ", string.Empty).Trim().ToLower();
                    if (Regex.IsMatch(attrValue, matchPattern))
                    {
                        XElement firstRun = e.Element(W + "r");

                        // Delete everything and insert updated text value
                        e.RemoveNodes();

                        XElement t = new XElement(W + "t", corePropertyValue);
                        Novacode.Text.PreserveSpace(t);
                        e.Add(new XElement(firstRun.Name, firstRun.Attributes(), firstRun.Element(XName.Get("rPr", W.NamespaceName)), t));
                    }
                }

                using (TextWriter tw = new StreamWriter(new PackagePartStream(pp.GetStream(FileMode.Create, FileAccess.Write))))
                    footer.Save(tw);
            }
            #endregion
            PopulateDocument(document, document.Package);
        }

        /// <summary>
        /// Add a custom property to this document. If a custom property already exists with the same name it will be replace. CustomProperty names are case insensitive.
        /// </summary>
        /// <param name="cp">The CustomProperty to add to this document.</param>
        /// <example>
        /// Add a custom properties of each type to a document.
        /// <code>
        /// // Load Example.docx
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///     // A CustomProperty called forename which stores a string.
        ///     CustomProperty forename;
        ///
        ///     // If this document does not contain a custom property called 'forename', create one.
        ///     if (!document.CustomProperties.ContainsKey("forename"))
        ///     {
        ///         // Create a new custom property called 'forename' and set its value.
        ///         document.AddCustomProperty(new CustomProperty("forename", "Cathal"));
        ///     }
        ///
        ///     // Get this documents custom property called 'forename'.
        ///     forename = document.CustomProperties["forename"];
        ///
        ///     // Print all of the information about this CustomProperty to Console.
        ///     Console.WriteLine(string.Format("Name: '{0}', Value: '{1}'\nPress any key...", forename.Name, forename.Value));
        ///
        ///     // Save all changes made to this document.
        ///     document.Save();
        /// } // Release this document from memory.
        ///
        /// // Wait for the user to press a key before exiting.
        /// Console.ReadKey();
        /// </code>
        /// </example>
        /// <seealso cref="CustomProperty"/>
        /// <seealso cref="CustomProperties"/>
        public void AddCustomProperty(CustomProperty cp)
        {
            // If this document does not contain a customFilePropertyPart create one.
            if (!Package.PartExists(new Uri("/docProps/custom.xml", UriKind.Relative)))
                HelperFunctions.CreateCustomPropertiesPart(this);

            XDocument customPropDoc;
            PackagePart customPropPart = Package.GetPart(new Uri("/docProps/custom.xml", UriKind.Relative));
            using (TextReader tr = new StreamReader(customPropPart.GetStream(FileMode.Open, FileAccess.Read)))
                customPropDoc = XDocument.Load(tr, LoadOptions.PreserveWhitespace);

            // Each custom property has a PID, get the highest PID in this document.
            IEnumerable<int> pids =
            (
                from d in customPropDoc.Descendants()
                where d.Name.LocalName == "property"
                select int.Parse(d.Attribute(XName.Get("pid")).Value)
            );

            int pid = 1;
            if (pids.Count() > 0)
                pid = pids.Max();

            // Check if a custom property already exists with this name
            // 2013-05-25: IgnoreCase while searching for custom property as it would produce a currupted docx.
            var customProperty =
            (
                from d in customPropDoc.Descendants()
                where (d.Name.LocalName == "property") && (d.Attribute(XName.Get("name")).Value.Equals(cp.Name, StringComparison.InvariantCultureIgnoreCase))
                select d
            ).SingleOrDefault();

            // If a custom property with this name already exists remove it.
            if (customProperty != null)
                customProperty.Remove();

            XElement propertiesElement = customPropDoc.Element(XName.Get("Properties", CustomPropertiesSchema.NamespaceName));
            propertiesElement.Add
            (
                new XElement
                (
                    XName.Get("property", CustomPropertiesSchema.NamespaceName),
                    new XAttribute("fmtid", "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}"),
                    new XAttribute("pid", pid + 1),
                    new XAttribute("name", cp.Name),
                        new XElement(CustomVTypesSchema + cp.Type, cp.Value ?? "")
                )
            );

            // Save the custom properties
            using (TextWriter tw = new StreamWriter(new PackagePartStream(customPropPart.GetStream(FileMode.Create, FileAccess.Write))))
                customPropDoc.Save(tw, SaveOptions.None);

            // Refresh all fields in this document which display this custom property.
            UpdateCustomPropertyValue(this, cp.Name, (cp.Value ?? "").ToString());
        }

        /// <summary>
        /// Update the custom properties inside the document
        /// </summary>
        /// <param name="document">The DocX document</param>
        /// <param name="customPropertyName">The property used inside the document</param>
        /// <param name="customPropertyValue">The new value for the property</param>
        /// <remarks>Different version of Word create different Document XML.</remarks>
        internal static void UpdateCustomPropertyValue(DocX document, string customPropertyName, string customPropertyValue)
        {
            // A list of documents, which will contain, The Main Document and if they exist: header1, header2, header3, footer1, footer2, footer3.
            List<XElement> documents = new List<XElement> { document.MainDoc.Root };

            // Check if each header exists and add if if so.
            #region Headers
            Headers headers = document.Headers;
            if (headers.First != null)
                documents.Add(headers.First.Xml);
            if (headers.Odd != null)
                documents.Add(headers.Odd.Xml);
            if (headers.Even != null)
                documents.Add(headers.Even.Xml);
            #endregion

            // Check if each footer exists and add if if so.
            #region Footers
            Footers footers = document.Footers;
            if (footers.First != null)
                documents.Add(footers.First.Xml);
            if (footers.Odd != null)
                documents.Add(footers.Odd.Xml);
            if (footers.Even != null)
                documents.Add(footers.Even.Xml);
            #endregion

            var matchCustomPropertyName = customPropertyName;
            if (customPropertyName.Contains(" ")) matchCustomPropertyName = "\"" + customPropertyName + "\"";
            string matchValue = string.Format(@"DOCPROPERTY  {0}  \* MERGEFORMAT", matchCustomPropertyName).Replace(" ", string.Empty);

            // Process each document in the list.
            foreach (XElement doc in documents)
            {
                #region Word 2010+
                foreach (XElement e in doc.Descendants(XName.Get("instrText", W.NamespaceName)))
                {

                    string attrValue = e.Value.Replace(" ", string.Empty).Trim();

                    if (attrValue.Equals(matchValue, StringComparison.CurrentCultureIgnoreCase))
                    {
                        XNode node = e.Parent.NextNode;
                        bool found = false;
                        while (true)
                        {
                            if (node.NodeType == XmlNodeType.Element)
                            {
                                var ele = node as XElement;
                                var match = ele.Descendants(XName.Get("t", W.NamespaceName));
                                if (match.Any())
                                {
                                    if (!found)
                                    {
                                        match.First().Value = customPropertyValue;
                                        found = true;
                                    }
                                    else
                                    {
                                        ele.RemoveNodes();
                                    }
                                }
                                else
                                {
                                    match = ele.Descendants(XName.Get("fldChar", W.NamespaceName));
                                    if (match.Any())
                                    {
                                        var endMatch = match.First().Attribute(XName.Get("fldCharType", W.NamespaceName));
                                        if (endMatch != null && endMatch.Value == "end")
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            node = node.NextNode;
                        }
                    }
                }
                #endregion

                #region < Word 2010
                foreach (XElement e in doc.Descendants(XName.Get("fldSimple", W.NamespaceName)))
                {
                    string attrValue = e.Attribute(XName.Get("instr", W.NamespaceName)).Value.Replace(" ", string.Empty).Trim();

                    if (attrValue.Equals(matchValue, StringComparison.CurrentCultureIgnoreCase))
                    {
                        XElement firstRun = e.Element(W + "r");
                        XElement firstText = firstRun.Element(W + "t");
                        XElement rPr = firstText.Element(W + "rPr");

                        // Delete everything and insert updated text value
                        e.RemoveNodes();

                        XElement t = new XElement(W + "t", rPr, customPropertyValue);
                        Novacode.Text.PreserveSpace(t);
                        e.Add(new XElement(firstRun.Name, firstRun.Attributes(), firstRun.Element(XName.Get("rPr", W.NamespaceName)), t));
                    }
                }
                #endregion
            }
        }

        public override Paragraph InsertParagraph()
        {
            Paragraph p = base.InsertParagraph();
            p.PackagePart = MainPart;
            return p;
        }

        public override Paragraph InsertParagraph(int index, string text, bool trackChanges)
        {
            Paragraph p = base.InsertParagraph(index, text, trackChanges);
            p.PackagePart = MainPart;
            return p;
        }

        public override Paragraph InsertParagraph(Paragraph p)
        {
            p.PackagePart = MainPart;
            return base.InsertParagraph(p);
        }

        public override Paragraph InsertParagraph(int index, Paragraph p)
        {
            p.PackagePart = MainPart;
            return base.InsertParagraph(index, p);
        }

        public override Paragraph InsertParagraph(int index, string text, bool trackChanges, Formatting formatting)
        {
            Paragraph p = base.InsertParagraph(index, text, trackChanges, formatting);
            p.PackagePart = MainPart;
            return p;
        }

        public override Paragraph InsertParagraph(string text)
        {
            Paragraph p = base.InsertParagraph(text);
            p.PackagePart = MainPart;
            return p;
        }

        public override Paragraph InsertParagraph(string text, bool trackChanges)
        {
            Paragraph p = base.InsertParagraph(text, trackChanges);
            p.PackagePart = MainPart;
            return p;
        }

        public override Paragraph InsertParagraph(string text, bool trackChanges, Formatting formatting)
        {
            Paragraph p = base.InsertParagraph(text, trackChanges, formatting);
            p.PackagePart = MainPart;

            return p;
        }

        public Paragraph[] InsertParagraphs(string text)
        {
            String[] textArray = text.Split('\n');
            List<Paragraph> paragraphs = new List<Paragraph>();
            foreach (var textForParagraph in textArray)
            {
                Paragraph p = base.InsertParagraph(text);
                p.PackagePart = MainPart;
                paragraphs.Add(p);
            }
            return paragraphs.ToArray();
        }

        public override ReadOnlyCollection<Content> Contents
        {
            get
            {
                ReadOnlyCollection<Content> l = base.Contents;
                foreach (var content in l)
                {
                    content.PackagePart = MainPart;
                }
                return l;
            }
        }

        public void SetContent(XElement el)
        {
            foreach (XElement e in el.Elements())
            {
                (from d in Document.Contents
                 where d.Name == e.Name
                 select d).First().SetText(e.Value);
            }
        }

        public void SetContent(Dictionary<string, string> dict)
        {
            foreach (KeyValuePair<string, string> item in dict)
            {
                (from d in Document.Contents
                 where d.Name == item.Key
                 select d).First().SetText(item.Value);
            }
        }

        public void SetContent(string path)
        {
            XDocument doc = XDocument.Load(path);
            SetContent(doc);
        }

        public void SetContent(XDocument xmlDoc)
        {

            foreach (XElement e in xmlDoc.ElementsAfterSelf())
            {
                (from d in Document.Contents
                 where d.Name == e.Name
                 select d).First().SetText(e.Value);
            }
        }

        public override ReadOnlyCollection<Paragraph> Paragraphs
        {
            get
            {
                ReadOnlyCollection<Paragraph> l = base.Paragraphs;
                foreach (var paragraph in l)
                {
                    paragraph.PackagePart = MainPart;
                }
                return l;
            }
        }

        public override List<List> Lists
        {
            get
            {
                List<List> l = base.Lists;
                l.ForEach(x => x.Items.ForEach(i => i.PackagePart = MainPart));
                return l;
            }
        }

        public override List<Table> Tables
        {
            get
            {
                List<Table> l = base.Tables;
                l.ForEach(x => x.MainPart = MainPart);
                return l;
            }
        }


        /// <summary>
        /// Create an equation and insert it in the new paragraph
        /// </summary>
        public override Paragraph InsertEquation(String equation)
        {
            Paragraph p = base.InsertEquation(equation);
            p.PackagePart = MainPart;
            return p;
        }

        /// <summary>
        /// Insert a chart in document
        /// </summary>
        public void InsertChart(Chart chart)
        {
            // Create a new chart part uri.
            String chartPartUriPath = String.Empty;
            Int32 chartIndex = 1;
            do
            {
                chartPartUriPath = String.Format
                (
                    "/word/charts/chart{0}.xml",
                    chartIndex
                );
                chartIndex++;
            } while (Package.PartExists(new Uri(chartPartUriPath, UriKind.Relative)));

            // Create chart part.
            PackagePart chartPackagePart = Package.CreatePart(new Uri(chartPartUriPath, UriKind.Relative), "application/vnd.openxmlformats-officedocument.drawingml.chart+xml", CompressionOption.Normal);

            // Create a new chart relationship
            String relId = GetNextFreeRelationshipId();
            PackageRelationship rel = MainPart.CreateRelationship(chartPackagePart.Uri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart", relId);

            // Save a chart info the chartPackagePart
            using (TextWriter tw = new StreamWriter(new PackagePartStream(chartPackagePart.GetStream(FileMode.Create, FileAccess.Write))))
                chart.Xml.Save(tw);

            // Insert a new chart into a paragraph.
            Paragraph p = InsertParagraph();
            XElement chartElement = new XElement(
                XName.Get("r", W.NamespaceName),
                new XElement(
                    XName.Get("drawing", W.NamespaceName),
                    new XElement(
                        XName.Get("inline", Wp.NamespaceName),
                        new XElement(XName.Get("extent", Wp.NamespaceName), new XAttribute("cx", "5486400"), new XAttribute("cy", "3200400")),
                        new XElement(XName.Get("effectExtent", Wp.NamespaceName), new XAttribute("l", "0"), new XAttribute("t", "0"), new XAttribute("r", "19050"), new XAttribute("b", "19050")),
                        new XElement(XName.Get("docPr", Wp.NamespaceName), new XAttribute("id", "1"), new XAttribute("name", "chart")),
                        new XElement(
                            XName.Get("graphic", A.NamespaceName),
                            new XElement(
                                XName.Get("graphicData", A.NamespaceName),
                                new XAttribute("uri", C.NamespaceName),
                                new XElement(
                                    XName.Get("chart", C.NamespaceName),
                                    new XAttribute(XName.Get("id", R.NamespaceName), relId)
                                )
                            )
                        )
                    )
               ));
            p.Xml.Add(chartElement);
        }

        /// <summary>
        /// Insert a chart in document after paragraph
        /// </summary>
        public void InsertChartAfterParagraph(Chart chart, Paragraph paragraph) {
            // Create a new chart part uri.
            String chartPartUriPath = String.Empty;
            Int32 chartIndex = 1;
            do {
                chartPartUriPath = String.Format
                (
                    "/word/charts/chart{0}.xml",
                    chartIndex
                );
                chartIndex++;
            } while (Package.PartExists(new Uri(chartPartUriPath, UriKind.Relative)));

            // Create chart part.
            PackagePart chartPackagePart = Package.CreatePart(new Uri(chartPartUriPath, UriKind.Relative), "application/vnd.openxmlformats-officedocument.drawingml.chart+xml", CompressionOption.Normal);

            // Create a new chart relationship
            String relId = GetNextFreeRelationshipId();
            PackageRelationship rel = MainPart.CreateRelationship(chartPackagePart.Uri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart", relId);

            // Save a chart info the chartPackagePart
            using (TextWriter tw = new StreamWriter(chartPackagePart.GetStream(FileMode.Create, FileAccess.Write)))
                chart.Xml.Save(tw);

            // Insert a new chart into a paragraph.
            Paragraph p = paragraph;
            XElement chartElement = new XElement(
                XName.Get("r", DocX.W.NamespaceName),
                new XElement(
                    XName.Get("drawing", DocX.W.NamespaceName),
                    new XElement(
                        XName.Get("inline", DocX.Wp.NamespaceName),
                        new XElement(XName.Get("extent", DocX.Wp.NamespaceName), new XAttribute("cx", "5486400"), new XAttribute("cy", "3200400")),
                        new XElement(XName.Get("effectExtent", DocX.Wp.NamespaceName), new XAttribute("l", "0"), new XAttribute("t", "0"), new XAttribute("r", "19050"), new XAttribute("b", "19050")),
                        new XElement(XName.Get("docPr", DocX.Wp.NamespaceName), new XAttribute("id", "1"), new XAttribute("name", "chart")),
                        new XElement(
                            XName.Get("graphic", DocX.A.NamespaceName),
                            new XElement(
                                XName.Get("graphicData", DocX.A.NamespaceName),
                                new XAttribute("uri", DocX.C.NamespaceName),
                                new XElement(
                                    XName.Get("chart", DocX.C.NamespaceName),
                                    new XAttribute(XName.Get("id", DocX.R.NamespaceName), relId)
                                )
                            )
                        )
                    )
               ));
            p.Xml.Add(chartElement);
        }

        /// <summary>
        /// Inserts a default TOC into the current document.
        /// Title: Table of contents
        /// Swithces will be: TOC \h \o '1-3' \u \z
        /// </summary>
        /// <returns>The inserted TableOfContents</returns>
        public TableOfContents InsertDefaultTableOfContents()
        {
            return InsertTableOfContents("Table of contents", TableOfContentsSwitches.O | TableOfContentsSwitches.H | TableOfContentsSwitches.Z | TableOfContentsSwitches.U);
        }

        /// <summary>
        /// Inserts a TOC into the current document.
        /// </summary>
        /// <param name="title">The title of the TOC</param>
        /// <param name="switches">Switches to be applied, see: http://officeopenxml.com/WPtableOfContents.php </param>
        /// <param name="headerStyle">Lets you set the style name of the TOC header</param>
        /// <param name="maxIncludeLevel">Lets you specify how many header levels should be included - default is 1-3</param>
        /// <param name="rightTabPos">Lets you override the right tab position - this is not common</param>
        /// <returns>The inserted TableOfContents</returns>
        public TableOfContents InsertTableOfContents(string title, TableOfContentsSwitches switches, string headerStyle = null, int maxIncludeLevel = 3, int? rightTabPos = null)
        {
            var toc = TableOfContents.CreateTableOfContents(this, title, switches, headerStyle, maxIncludeLevel, rightTabPos);
            Xml.Add(toc.Xml);
            return toc;
        }

        /// <summary>
        /// Inserts at TOC into the current document before the provided <paramref name="reference"/>
        /// </summary>
        /// <param name="reference">The paragraph to use as reference</param>
        /// <param name="title">The title of the TOC</param>
        /// <param name="switches">Switches to be applied, see: http://officeopenxml.com/WPtableOfContents.php </param>
        /// <param name="headerStyle">Lets you set the style name of the TOC header</param>
        /// <param name="maxIncludeLevel">Lets you specify how many header levels should be included - default is 1-3</param>
        /// <param name="rightTabPos">Lets you override the right tab position - this is not common</param>
        /// <returns>The inserted TableOfContents</returns>
        public TableOfContents InsertTableOfContents(Paragraph reference, string title, TableOfContentsSwitches switches, string headerStyle = null, int maxIncludeLevel = 3, int? rightTabPos = null)
        {
            var toc = TableOfContents.CreateTableOfContents(this, title, switches, headerStyle, maxIncludeLevel, rightTabPos);
            reference.Xml.AddBeforeSelf(toc.Xml);
            return toc;
        }

        private static void CopyStream(Stream input, Stream output, int bufferSize = 32768)
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        private readonly object _nextFreeDocPrIdLock = new object();
        private long? _nextFreeDocPrId;

        /// <summary>
        /// Finds next free id for bookmarkStart/docPr.
        /// </summary>
        internal long GetNextFreeDocPrId()
        {
            lock (_nextFreeDocPrIdLock)
            {
                if (_nextFreeDocPrId != null)
                {
                    _nextFreeDocPrId++;
                    return _nextFreeDocPrId.Value;
                }

                // also loop thru all docPr ids
                var xNameBookmarkStart = XName.Get("bookmarkStart", DocX.W.NamespaceName);
                var xNameDocPr = XName.Get("docPr", DocX.Wp.NamespaceName);

                long newDocPrId = 1;
                HashSet<string> existingIds = new HashSet<string>();
                foreach (var bookmarkId in Xml.Descendants())
                {
                    if (bookmarkId.Name != xNameBookmarkStart
                        && bookmarkId.Name != xNameDocPr)
                    {
                        continue;
                    }

                    var idAtt = bookmarkId.Attributes().FirstOrDefault(x => x.Name.LocalName == "id");
                    if (idAtt != null)
                    {
                        existingIds.Add(idAtt.Value);
                    }
                }

                while (existingIds.Contains(newDocPrId.ToString()))
                {
                    newDocPrId++;
                }
                _nextFreeDocPrId = newDocPrId;
                return _nextFreeDocPrId.Value;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by this document.
        /// </summary>
        /// <example>
        /// If you take advantage of the using keyword, Dispose() is automatically called for you.
        /// <code>
        /// // Load document.
        /// using (DocX document = DocX.Load(@"C:\Example\Test.docx"))
        /// {
        ///      // The document is only in memory while in this scope.
        ///
        /// }// Dispose() is automatically called at this point.
        /// </code>
        /// </example>
        /// <example>
        /// This example is equilivant to the one above example.
        /// <code>
        /// // Load document.
        /// DocX document = DocX.Load(@"C:\Example\Test.docx");
        ///
        /// // Do something with the document here.
        ///
        /// // Dispose of the document.
        /// document.Dispose();
        /// </code>
        /// </example>
        public void Dispose()
        {
            Package.Close();
        }

        #endregion
    }
}
