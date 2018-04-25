using System;
using System.Collections.Generic;
using System.Text;
using LemmatizerNET.Implement.Agramtab;

namespace LemmatizerNET.Implement {
	internal class AbcEncoder {
		private Lemmatizer _lemmatizer;
        private InternalMorphLanguage _language;
		private char _annotChar;
		private int _alphabetSize;
		protected int[] Alphabet2Code=new int[Constants.AlphabetSize];
		private int[] _code2Alphabet = new int[Constants.MaxAlphabetSize];
		private int _alphabetSizeWithoutAnnotator;
		private int[] _alphabet2CodeWithoutAnnotator = new int[Constants.AlphabetSize];
		private int[] _code2AlphabetWithoutAnnotator = new int[Constants.MaxAlphabetSize];

		public string CriticalNounLetterPack {
			get {
				return new string(_annotChar, Constants.MinimalPredictionSuffix);
			}
		}
		public InternalMorphLanguage Language {
			get {
                return _language;
			}
		}

		private static int InitAlphabet(InternalMorphLanguage language, int[] pCode2Alphabet, int[] pAlphabet2Code, char annotChar) {
			if (char.IsUpper(annotChar)) {
				throw new MorphException("annotChar is not upper");
			}
			var additionalEnglishChars = "'1234567890";
			var additionalGermanChars = "";
			var alphabetSize = 0;
			for (var i = 0; i < Constants.AlphabetSize; i++) {
				var ch = Convert.ToChar(i);
				if (Lang.is_upper_alpha((byte)i, language)
				|| (ch == '-')
				|| (ch == annotChar)
				|| ((language == InternalMorphLanguage.MorphEnglish)
						&& (additionalEnglishChars.IndexOf(ch) >= 0)
					)
				|| ((language == InternalMorphLanguage.MorphGerman)
						&& (additionalGermanChars.IndexOf(ch) >= 0)
					)
				|| ((language == InternalMorphLanguage.MorphUrl)
					  && Lang.is_alpha((byte)i, language)
					 )) {


					pCode2Alphabet[alphabetSize] = i;
					pAlphabet2Code[i] = alphabetSize;
					alphabetSize++;
				} else {
					pAlphabet2Code[i] = -1;
				}
			}
			if (alphabetSize > Constants.MaxAlphabetSize) {
				throw new MorphException("Error! The  ABC is too large");
			}
			return alphabetSize;
		}
		public AbcEncoder(Lemmatizer lemmatizer,InternalMorphLanguage language, char annotChar) {
            _lemmatizer = lemmatizer;
            _language = language;
			_annotChar=annotChar;
			_alphabetSize = InitAlphabet(language, _code2Alphabet, Alphabet2Code, _annotChar);
			_alphabetSizeWithoutAnnotator = InitAlphabet(language,_code2AlphabetWithoutAnnotator,_alphabet2CodeWithoutAnnotator,(char)257/* non-exeting symbol */);
			if (_alphabetSizeWithoutAnnotator + 1 != _alphabetSize) {
				throw new MorphException("_alphabetSizeWithoutAnnotator + 1 != _alphabetSize");
			}
		}
		public Lemmatizer Lemmatizer {
			get {
				return _lemmatizer;
			}
		}
		public char AnnotChar {
			get {
				return _annotChar;
			}
		}
		public bool CheckAbcWithAnnotator(string wordForm) {
			var len = wordForm.Length;
			for (var i = 0; i < len; i++) {
				if (Alphabet2Code[Tools.GetByte(wordForm[i])] == -1) {
					return false;
				}
			}
			return true;
		}
		public bool CheckAbcWithoutAnnotator(string wordForm) {
			var len = wordForm.Length;
			for (var i = 0; i < len; i++)
				if (_alphabet2CodeWithoutAnnotator[Tools.GetByte(wordForm[i])] == -1)
					return false;
			return true;
		}
		public int DecodeFromAlphabet(string v) {
			var len = v.Length;
			var c = 1;
			var result = 0;
			for (var i = 0; i < len; i++) {
				result += _alphabet2CodeWithoutAnnotator[Tools.GetByte(v[i])] * c;
				c *= _alphabetSizeWithoutAnnotator;
			};
			return result;
		}
	}
}
