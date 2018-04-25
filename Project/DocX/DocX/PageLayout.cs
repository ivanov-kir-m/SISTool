using System;
using System.Xml.Linq;

namespace Novacode
{
    public class PageLayout: DocXElement
    {
        internal PageLayout(DocX document, XElement xml):base(document, xml)
        {
        
        }

        
        public Orientation Orientation 
        {
            get
            {
                /*
                 * Get the pgSz (page size) element for this Section,
                 * null will be return if no such element exists.
                 */
                XElement pgSz = Xml.Element(XName.Get("pgSz", DocX.W.NamespaceName));

                if (pgSz == null)
                    return Orientation.Portrait;

                // Get the attribute of the pgSz element.
                XAttribute val = pgSz.Attribute(XName.Get("orient", DocX.W.NamespaceName));

                // If val is null, this cell contains no information.
                if (val == null)
                    return Orientation.Portrait;

                if (val.Value.Equals("Landscape", StringComparison.CurrentCultureIgnoreCase))
                    return Orientation.Landscape;
                else
                    return Orientation.Portrait;
            }

            set
            {
                // Check if already correct value.
                if (Orientation == value)
                    return;

                /*
                 * Get the pgSz (page size) element for this Section,
                 * null will be return if no such element exists.
                 */
                XElement pgSz = Xml.Element(XName.Get("pgSz", DocX.W.NamespaceName));

                if (pgSz == null)
                {
                    Xml.SetElementValue(XName.Get("pgSz", DocX.W.NamespaceName), string.Empty);
                    pgSz = Xml.Element(XName.Get("pgSz", DocX.W.NamespaceName));
                }

                pgSz.SetAttributeValue(XName.Get("orient", DocX.W.NamespaceName), value.ToString().ToLower());

                if(value == Novacode.Orientation.Landscape)
                {
                    pgSz.SetAttributeValue(XName.Get("w", DocX.W.NamespaceName), "16838");
                    pgSz.SetAttributeValue(XName.Get("h", DocX.W.NamespaceName), "11906");
                }

                else if (value == Novacode.Orientation.Portrait)
                {
                    pgSz.SetAttributeValue(XName.Get("w", DocX.W.NamespaceName), "11906");
                    pgSz.SetAttributeValue(XName.Get("h", DocX.W.NamespaceName), "16838");
                }
            }
        }
    }
}
