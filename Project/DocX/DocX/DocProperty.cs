﻿using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Novacode
{
    /// <summary>
    /// Represents a field of type document property. This field displays the value stored in a custom property.
    /// </summary>
    public class DocProperty: DocXElement
    {
        internal Regex ExtractName = new Regex(@"DOCPROPERTY  (?<name>.*)  ");

        /// <summary>
        /// The custom property to display.
        /// </summary>
        public string Name { get; }

        internal DocProperty(DocX document, XElement xml):base(document, xml)
        {           
            string instr = Xml.Attribute(XName.Get("instr", "http://schemas.openxmlformats.org/wordprocessingml/2006/main")).Value;
            Name = ExtractName.Match(instr.Trim()).Groups["name"].Value;
        }
    }
}
