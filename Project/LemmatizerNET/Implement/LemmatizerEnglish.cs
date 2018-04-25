using System;
using System.Collections.Generic;
using System.Text;

namespace LemmatizerNET.Implement {
	internal class LemmatizerEnglish : Lemmatizer {
		public LemmatizerEnglish()
			: base(InternalMorphLanguage.MorphEnglish) {
			Registry = "Software\\Dialing\\Lemmatizer\\English\\DictPath";
		}
		protected override string FilterSrc(string src) {
			return src;
		}
	}
}
