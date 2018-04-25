﻿using System;
using System.IO.Packaging;
using System.IO;

namespace Novacode
{
    /// <summary>
    /// Represents an Image embedded in a document.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// A unique id which identifies this Image.
        /// </summary>
        private string _id;
        private DocX _document;
        internal PackageRelationship Pr;

        public Stream GetStream(FileMode mode, FileAccess access)
        {
            string temp = Pr.SourceUri.OriginalString;
            string start = temp.Remove(temp.LastIndexOf('/'));
            string end = Pr.TargetUri.OriginalString;
            string full = end.Contains(start) ? end : start + "/" + end;

            return (new PackagePartStream(_document.Package.GetPart(new Uri(full, UriKind.Relative)).GetStream(mode, access)));
        }

        /// <summary>
        /// Returns the id of this Image.
        /// </summary>
        public string Id 
        { 
            get {return _id;} 
        }

        internal Image(DocX document, PackageRelationship pr)
        {
            this._document = document;
            this.Pr = pr;
            this._id = pr.Id;
        }

        /// <summary>
        /// Add an image to a document, create a custom view of that image (picture) and then insert it into a Paragraph using append.
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// Add an image to a document, create a custom view of that image (picture) and then insert it into a Paragraph using append.
        /// <code>
        /// using (DocX document = DocX.Create("Test.docx"))
        /// {
        ///    // Add an image to the document. 
        ///    Image     i = document.AddImage(@"Image.jpg");
        ///    
        ///    // Create a picture i.e. (A custom view of an image)
        ///    Picture   p = i.CreatePicture();
        ///    p.FlipHorizontal = true;
        ///    p.Rotation = 10;
        ///
        ///    // Create a new Paragraph.
        ///    Paragraph par = document.InsertParagraph();
        ///    
        ///    // Append content to the Paragraph.
        ///    par.Append("Here is a cool picture")
        ///       .AppendPicture(p)
        ///       .Append(" don't you think so?");
        ///
        ///    // Save all changes made to this document.
        ///    document.Save();
        /// }
        /// </code>
        /// </example>
        public Picture CreatePicture()
        {
            return Paragraph.CreatePicture(_document, _id, string.Empty, string.Empty);
        }
        public Picture CreatePicture(int height, int width) {
            Picture picture = Paragraph.CreatePicture(_document, _id, string.Empty, string.Empty);
            picture.Height = height;
            picture.Width = width;
            return picture;
        }

      ///<summary>
      /// Returns the name of the image file.
      ///</summary>
      public string FileName
      {
        get
        {
          return Path.GetFileName(this.Pr.TargetUri.ToString());
        }
      }
    }
}
