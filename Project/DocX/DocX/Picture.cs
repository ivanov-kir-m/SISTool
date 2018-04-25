using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO.Packaging;

namespace Novacode
{
    /// <summary>
    /// Represents a Picture in this document, a Picture is a customized view of an Image.
    /// </summary>
    public class Picture: DocXElement
    {
        private const int EmusInPixel = 9525;

        internal Dictionary<PackagePart, PackageRelationship> PictureRels;
        
        internal Image Img;
        private string _id;
        private string _name;
        private string _descr;
        private int _cx, _cy;
        //private string fileName;
        private uint _rotation;
        private bool _hFlip, _vFlip;
        private object _pictureShape;
        private XElement _xfrm;
        private XElement _prstGeom;

        /// <summary>
        /// Remove this Picture from this document.
        /// </summary>
        public void Remove()
        {
            Xml.Remove();
        }

        /// <summary>
        /// Wraps an XElement as an Image
        /// </summary>
        /// <param name="document"></param>
        /// <param name="i">The XElement i to wrap</param>
        /// <param name="img"></param>
        internal Picture(DocX document, XElement i, Image img):base(document, i)
		{
		    PictureRels = new Dictionary<PackagePart, PackageRelationship>();
		    
		    this.Img = img;
		
		    this._id =
		    (
		        from e in Xml.Descendants()
		        where e.Name.LocalName.Equals("blip")
		        select e.Attribute(XName.Get("embed", "http://schemas.openxmlformats.org/officeDocument/2006/relationships")).Value
		    ).SingleOrDefault();
		
		    if (this._id == null)
		    {
		        this._id =
		        (
		            from e in Xml.Descendants()
		            where e.Name.LocalName.Equals("imagedata")
		            select e.Attribute(XName.Get("id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships")).Value
		        ).SingleOrDefault();
		    }
		
		    this._name = 
		    (
		        from e in Xml.Descendants()
		        let a = e.Attribute(XName.Get("name"))
		        where (a != null)
		        select a.Value
		    ).FirstOrDefault();
		    
		    if (this._name == null)
		    {
		        this._name = 
		        (
		            from e in Xml.Descendants()
		            let a = e.Attribute(XName.Get("title"))
		            where (a != null)
		            select a.Value
		        ).FirstOrDefault();
		    }
		  
		    this._descr =
		    (
		        from e in Xml.Descendants()
		        let a = e.Attribute(XName.Get("descr"))
		        where (a != null)
		        select a.Value
		    ).FirstOrDefault();
		
		    this._cx = 
		    (
		        from e in Xml.Descendants()
		        let a = e.Attribute(XName.Get("cx"))
		        where (a != null)
		        select int.Parse(a.Value)
		    ).FirstOrDefault();
		    
		    if (this._cx == 0)
		    {
		        XAttribute style = 
		        (
		            from e in Xml.Descendants()
            let a = e.Attribute(XName.Get("style"))
		            where (a != null)
		            select a
		        ).FirstOrDefault();
		        
		        string fromWidth = style.Value.Substring(style.Value.IndexOf("width:") + 6);
		        var widthInt = ((double.Parse((fromWidth.Substring(0, fromWidth.IndexOf("pt"))).Replace(".", ","))) / 72.0) * 914400;
		        _cx = System.Convert.ToInt32(widthInt);
		    }
		
		    this._cy = 
		    (
		        from e in Xml.Descendants()
		        let a = e.Attribute(XName.Get("cy"))
		        where (a != null)
		        select int.Parse(a.Value)
		    ).FirstOrDefault();
		    
		    if (this._cy == 0)
		    {
		        XAttribute style = 
		        (
		            from e in Xml.Descendants()
		            let a = e.Attribute(XName.Get("style"))
		            where (a != null)
		            select a
		        ).FirstOrDefault();
		        
		        string fromHeight = style.Value.Substring(style.Value.IndexOf("height:") + 7);
		        var heightInt = ((double.Parse((fromHeight.Substring(0, fromHeight.IndexOf("pt"))).Replace(".", ","))) / 72.0) * 914400;
		        _cy = System.Convert.ToInt32(heightInt);
		    }
		
		    this._xfrm =
		    (
		        from d in Xml.Descendants()
		        where d.Name.LocalName.Equals("xfrm")
		        select d
		    ).SingleOrDefault();
		
		    this._prstGeom =
		    (
		        from d in Xml.Descendants()
		        where d.Name.LocalName.Equals("prstGeom")
		        select d
		    ).SingleOrDefault();
		
		    if (_xfrm != null)
		        this._rotation = _xfrm.Attribute(XName.Get("rot")) == null ? 0 : uint.Parse(_xfrm.Attribute(XName.Get("rot")).Value);
		}

        private void SetPictureShape(object shape)
        {
            this._pictureShape = shape;

            XAttribute prst = _prstGeom.Attribute(XName.Get("prst"));
            if (prst == null)
                _prstGeom.Add(new XAttribute(XName.Get("prst"), "rectangle"));

            _prstGeom.Attribute(XName.Get("prst")).Value = shape.ToString();
        }

        /// <summary>
        /// Set the shape of this Picture to one in the BasicShapes enumeration.
        /// </summary>
        /// <param name="shape">A shape from the BasicShapes enumeration.</param>
        public void SetPictureShape(BasicShapes shape)
        {
            SetPictureShape((object)shape);
        }

        /// <summary>
        /// Set the shape of this Picture to one in the RectangleShapes enumeration.
        /// </summary>
        /// <param name="shape">A shape from the RectangleShapes enumeration.</param>
        public void SetPictureShape(RectangleShapes shape)
        {
            SetPictureShape((object)shape);
        }

        /// <summary>
        /// Set the shape of this Picture to one in the BlockArrowShapes enumeration.
        /// </summary>
        /// <param name="shape">A shape from the BlockArrowShapes enumeration.</param>
        public void SetPictureShape(BlockArrowShapes shape)
        {
            SetPictureShape((object)shape);
        }

        /// <summary>
        /// Set the shape of this Picture to one in the EquationShapes enumeration.
        /// </summary>
        /// <param name="shape">A shape from the EquationShapes enumeration.</param>
        public void SetPictureShape(EquationShapes shape)
        {
            SetPictureShape((object)shape);
        }

        /// <summary>
        /// Set the shape of this Picture to one in the FlowchartShapes enumeration.
        /// </summary>
        /// <param name="shape">A shape from the FlowchartShapes enumeration.</param>
        public void SetPictureShape(FlowchartShapes shape)
        {
            SetPictureShape((object)shape);
        }

        /// <summary>
        /// Set the shape of this Picture to one in the StarAndBannerShapes enumeration.
        /// </summary>
        /// <param name="shape">A shape from the StarAndBannerShapes enumeration.</param>
        public void SetPictureShape(StarAndBannerShapes shape)
        {
            SetPictureShape((object)shape);
        }

        /// <summary>
        /// Set the shape of this Picture to one in the CalloutShapes enumeration.
        /// </summary>
        /// <param name="shape">A shape from the CalloutShapes enumeration.</param>
        public void SetPictureShape(CalloutShapes shape)
        {
            SetPictureShape((object)shape);
        }

        /// <summary>
        /// A unique id that identifies an Image embedded in this document.
        /// </summary>
        public string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Flip this Picture Horizontally.
        /// </summary>
        public bool FlipHorizontal
        {
            get { return _hFlip; }

            set
            {
                _hFlip = value;

                XAttribute flipH = _xfrm.Attribute(XName.Get("flipH"));
                if (flipH == null)
                    _xfrm.Add(new XAttribute(XName.Get("flipH"), "0"));

                _xfrm.Attribute(XName.Get("flipH")).Value = _hFlip ? "1" : "0";
            }
        }

        /// <summary>
        /// Flip this Picture Vertically.
        /// </summary>
        public bool FlipVertical
        {
            get { return _vFlip; }

            set
            {
                _vFlip = value;

                XAttribute flipV = _xfrm.Attribute(XName.Get("flipV"));
                if (flipV == null)
                    _xfrm.Add(new XAttribute(XName.Get("flipV"), "0"));

                _xfrm.Attribute(XName.Get("flipV")).Value = _vFlip ? "1" : "0";
            }
        }

        /// <summary>
        /// The rotation in degrees of this image, actual value = value % 360
        /// </summary>
        public uint Rotation
        {
            get { return _rotation / 60000; }

            set
            {
                _rotation = (value % 360) * 60000;
                XElement xfrm =
                    (from d in Xml.Descendants()
                    where d.Name.LocalName.Equals("xfrm")
                    select d).Single();

                XAttribute rot = xfrm.Attribute(XName.Get("rot"));
                if(rot == null)
                    xfrm.Add(new XAttribute(XName.Get("rot"), 0));

                xfrm.Attribute(XName.Get("rot")).Value = _rotation.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the name of this Image.
        /// </summary>
        public string Name 
        { 
            get { return _name; } 
            
            set 
            { 
                _name = value;

                foreach (XAttribute a in Xml.Descendants().Attributes(XName.Get("name")))
                    a.Value = _name;
            } 
        }

        /// <summary>
        /// Gets or sets the description for this Image.
        /// </summary>
        public string Description 
        { 
            get { return _descr; } 
            
            set 
            { 
                _descr = value;

                foreach (XAttribute a in Xml.Descendants().Attributes(XName.Get("descr")))
                    a.Value = _descr;
            } 
        }

        ///<summary>
        /// Returns the name of the image file for the picture.
        ///</summary>
        public string FileName
        {
          get
          {
            return Img.FileName;
          }
        }

        /// <summary>
        /// Get or sets the Width of this Image.
        /// </summary>
        public int Width 
        { 
            get { return _cx / EmusInPixel; }
            
            set 
            {
                _cx = value * EmusInPixel;

                foreach (XAttribute a in Xml.Descendants().Attributes(XName.Get("cx")))
                    a.Value = (_cx).ToString();
            } 
        }

        /// <summary>
        /// Get or sets the height of this Image.
        /// </summary>
        public int Height 
        { 
            get { return _cy / EmusInPixel; }
            
            set 
            {
                _cy = value * EmusInPixel;

                foreach (XAttribute a in Xml.Descendants().Attributes(XName.Get("cy")))
                    a.Value = (_cy).ToString();
            } 
        }

        //public void Delete()
        //{
        //    // Remove xml
        //    i.Remove();
   
        //    // Rebuild the image collection for this paragraph
        //    // Requires that every Image have a link to its paragraph

        //}
    }
}
