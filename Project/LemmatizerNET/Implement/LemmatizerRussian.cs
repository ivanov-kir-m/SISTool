using System;
using System.Collections.Generic;
using System.Text;

namespace LemmatizerNET.Implement {
	internal class LemmatizerRussian:Lemmatizer {
		public LemmatizerRussian()
			: base(InternalMorphLanguage.MorphRussian) {
			Registry = "Software\\Dialing\\Lemmatizer\\Russian\\DictPath";
			
			HyphenPostfixes.Add("КА");
			HyphenPostfixes.Add("ТО");
			HyphenPostfixes.Add("С");

			HyphenPrefixes.Add("ПОЛУ");
			HyphenPrefixes.Add("ПОЛ");
			HyphenPrefixes.Add("ВИЦЕ");
			HyphenPrefixes.Add("МИНИ");
			HyphenPrefixes.Add("КИК");
		}
		private static string ConvertJo2Je(string src) {
			return src.Replace('Ё', 'Е').Replace('ё', 'е');
		}
		protected override string FilterSrc(string src) {
			if (!AllowRussianJo) {
				src=ConvertJo2Je(src);
			}
			return src.Replace('\'', 'ъ');
		}
	}
}
