using System;
using System.Collections.Generic;
using System.Text;

namespace LemmatizerNET.Implement {
    internal class LemmatizerGerman : Lemmatizer {
		public LemmatizerGerman()
			: base(InternalMorphLanguage.MorphGerman) {
			Registry = "Software\\Dialing\\Lemmatizer\\German\\DictPath";
		}
		protected override string FilterSrc(string src) {
			return src;
		}
	}
}
