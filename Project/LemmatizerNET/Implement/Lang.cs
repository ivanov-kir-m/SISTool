using System;
using System.Collections.Generic;
using System.Text;

namespace LemmatizerNET.Implement {
	internal static class Lang {
		[Flags]
		private enum LangType : short {
			None = 0,
			FWordDelim = 1,
			RusUpper = 2,
			RusLower = 4,
			GerUpper = 8,
			GerLower = 16,
			EngUpper = 32,
			EngLower = 64,
			OpnBrck = 128,
			ClsBrck = 256,
			UpRomDigits = 512,
			LwRomDigits = 1024,
			LatinVowel = 2048,
			RussianVowel = 4096,
			UrlChar = 8192
		}

	    private static LangType[] _ascii = new[]{
/*null (nul)*/                                     LangType.FWordDelim,
/*start of heading (soh)*/                         LangType.FWordDelim,
/*start of text (stx)*/                            LangType.FWordDelim,
/*end of text (etx)*/                              LangType.FWordDelim,
/*end of transmission (eot)*/                      LangType.FWordDelim,
/*enquiry (enq)*/                                  LangType.FWordDelim,
/*acknowledge (ack)*/                              LangType.FWordDelim,
/*bell (bel)*/                                     LangType.FWordDelim,
/*backspace (bs)*/                                 LangType.FWordDelim,
/*character tabulation (ht)*/                      LangType.FWordDelim,
/*line feed (lf)*/                                 LangType.FWordDelim,
/*line tabulation (vt)*/                           LangType.FWordDelim,
/*form feed (ff)*/                                 LangType.FWordDelim,
/*carriage return (cr)*/                           LangType.FWordDelim,
/*shift out (so)*/                                 LangType.FWordDelim,
/*shift in (si)*/                                  LangType.FWordDelim,
/*datalink escape (dle)*/                          LangType.FWordDelim,
/*device control one (dc1)*/                       LangType.FWordDelim,
/*device control two (dc2)*/                       LangType.FWordDelim,
/*device control three (dc3)*/                     LangType.FWordDelim,
/*device control four (dc4)*/                      LangType.FWordDelim,
/*negative acknowledge (nak)*/                     LangType.FWordDelim,
/*syncronous idle (syn)*/                          LangType.FWordDelim,
/*end of transmission block (etb)*/                LangType.FWordDelim,
/*cancel (can)*/                                   LangType.FWordDelim,
/*end of medium (em)*/                             LangType.FWordDelim,
/*substitute (sub)*/                               LangType.FWordDelim,
/*escape (esc)*/                                   LangType.FWordDelim,
/*file separator (is4)*/                           LangType.FWordDelim,
/*group separator (is3)*/                          LangType.FWordDelim,
/*record separator (is2)*/                         LangType.FWordDelim,
/*unit separator (is1)*/                           LangType.FWordDelim,
/*space ' '*/                                      LangType.FWordDelim,
/*exclamation mark '!'*/                           LangType.FWordDelim|LangType.UrlChar,
/*quotation mark '"'*/                             LangType.FWordDelim,
/*number sign '#'*/                                LangType.FWordDelim,
/*dollar sign '$'*/                                LangType.FWordDelim|LangType.UrlChar,
/*percent sign '%'*/                               LangType.FWordDelim|LangType.UrlChar,
/*ampersand '&'*/                                  LangType.FWordDelim|LangType.UrlChar,
/*apostrophe '''*/                                 LangType.FWordDelim|LangType.RusUpper|LangType.RusLower, // твердый знак в виде апострофа
/*left parenthesis '('*/                           LangType.FWordDelim|LangType.OpnBrck|LangType.UrlChar,
/*right parenthesis ')'*/                          LangType.FWordDelim|LangType.ClsBrck|LangType.UrlChar,
/*asterisk '*'*/                                   LangType.FWordDelim|LangType.UrlChar,
/*plus sign '+'*/                                  LangType.FWordDelim|LangType.UrlChar,
/*comma ','*/                                      LangType.FWordDelim|LangType.UrlChar,
/*hyphen-minus '-'*/                               LangType.FWordDelim|LangType.UrlChar,
/*full stop '.'*/                                  LangType.FWordDelim|LangType.UrlChar,
/*solidus '/'*/                                    LangType.FWordDelim|LangType.UrlChar,
/*digit zero '0'*/                                 LangType.UrlChar,
/*digit one '1'*/                                  LangType.UrlChar,
/*digit two '2'*/                                  LangType.UrlChar,
/*digit three '3'*/                                LangType.UrlChar,
/*digit four '4'*/                                 LangType.UrlChar,
/*digit five '5'*/                                 LangType.UrlChar,
/*digit six '6'*/                                  LangType.UrlChar,
/*digit seven '7'*/                                LangType.UrlChar,
/*digit eight '8'*/                                LangType.UrlChar,
/*digit nine '9'*/                                 LangType.UrlChar,
/*colon ':'*/                                      LangType.FWordDelim|LangType.UrlChar,
/*semicolon ';'*/                                  LangType.FWordDelim|LangType.UrlChar,
/*less-than sign '<'*/                             LangType.FWordDelim|LangType.OpnBrck,
/*equals sign '='*/                                LangType.FWordDelim|LangType.UrlChar,
/*greater-than sign '>'*/                          LangType.FWordDelim|LangType.ClsBrck,
/*question mark '?'*/                              LangType.FWordDelim|LangType.UrlChar,
/*commercial at '@'*/                              LangType.FWordDelim|LangType.UrlChar,
/*latin capital letter a 'A'*/                     LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel,
/*latin capital letter b 'B'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter c 'C'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter d 'D'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter e 'E'*/                     LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel,
/*latin capital letter f 'F'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter g 'G'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter h 'H'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter i 'I'*/                     LangType.GerUpper|LangType.EngUpper|LangType.UpRomDigits|LangType.LatinVowel,
/*latin capital letter j 'J'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter k 'K'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter l 'L'*/                     LangType.GerUpper|LangType.EngUpper|LangType.UpRomDigits,
/*latin capital letter m 'M'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter n 'N'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter o 'O'*/                     LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel,
/*latin capital letter p 'P'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter q 'Q'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter r 'R'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter s 'S'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter t 'T'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter u 'U'*/                     LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel,
/*latin capital letter v 'V'*/                     LangType.GerUpper|LangType.EngUpper|LangType.UpRomDigits,
/*latin capital letter w 'W'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter x 'X'*/                     LangType.GerUpper|LangType.EngUpper|LangType.UpRomDigits,
/*latin capital letter y 'Y'*/                     LangType.GerUpper|LangType.EngUpper,
/*latin capital letter z 'Z'*/                     LangType.GerUpper|LangType.EngUpper,
/*left square bracket '['*/                        LangType.FWordDelim|LangType.OpnBrck,
/*reverse solidus '\'*/                            LangType.FWordDelim,
/*right square bracket ']'*/                       LangType.FWordDelim|LangType.ClsBrck,
/*circumflex accent '^'*/                          LangType.FWordDelim,
/*low line '_'*/                                   LangType.FWordDelim,
/*grave accent '`'*/                               LangType.FWordDelim,
/*latin small letter a 'a'*/                       LangType.GerLower|LangType.EngLower|LangType.LatinVowel|LangType.UrlChar,
/*latin small letter b 'b'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter c 'c'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter d 'd'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter e 'e'*/                       LangType.GerLower|LangType.EngLower|LangType.LatinVowel|LangType.UrlChar,
/*latin small letter f 'f'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter g 'g'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter h 'h'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter i 'i'*/                       LangType.GerLower|LangType.EngLower|LangType.LwRomDigits|LangType.LatinVowel|LangType.UrlChar,
/*latin small letter j 'j'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter k 'k'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter l 'l'*/                       LangType.GerLower|LangType.EngLower|LangType.LwRomDigits|LangType.UrlChar,
/*latin small letter m 'm'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter n 'n'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter o 'o'*/                       LangType.GerLower|LangType.EngLower|LangType.LatinVowel|LangType.UrlChar,
/*latin small letter p 'p'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter q 'q'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter r 'r'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter s 's'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter t 't'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter u 'u'*/                       LangType.GerLower|LangType.EngLower|LangType.LatinVowel|LangType.UrlChar,
/*latin small letter v 'v'*/                       LangType.GerLower|LangType.EngLower|LangType.LwRomDigits|LangType.UrlChar,
/*latin small letter w 'w'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter x 'x'*/                       LangType.GerLower|LangType.EngLower|LangType.LwRomDigits|LangType.UrlChar,
/*latin small letter y 'y'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*latin small letter z 'z'*/                       LangType.GerLower|LangType.EngLower|LangType.UrlChar,
/*left curly bracket '{'*/                         LangType.FWordDelim|LangType.OpnBrck,
/*vertical line '|'*/                              LangType.FWordDelim,
/*right curly bracket '}'*/                        LangType.FWordDelim|LangType.ClsBrck,
/*tilde '~'*/                                      LangType.FWordDelim,
/*delete ''*/                                     LangType.None,
/*padding character (pad) '_'*/                    LangType.FWordDelim,
/*high octet preset (hop) '_'*/                    LangType.None,
/*break permitted here (bph) '''*/                 LangType.None,
/*no break here (nbh) '_'*/                        LangType.FWordDelim,
/*index (ind) '"'*/                                LangType.None,
/*next line (nel) ':'*/                            LangType.FWordDelim,
/*start of selected area (ssa) '+'*/               LangType.FWordDelim,
/*end of selected area (esa) '+'*/                 LangType.FWordDelim,
/*character tabulation set (hts) '_'*/             LangType.FWordDelim,
/*character tabulation with justification (htj) '%'*/ LangType.FWordDelim,
/*line tabulation set (vts) '_'*/                  LangType.None,
/*partial line forward (pld) '<'*/                 LangType.FWordDelim,
/*partial line backward (plu) '_'*/                LangType.FWordDelim,
/*reverse line feed (ri) '_'*/                     LangType.FWordDelim,
/*single-shift two (ss2) '_'*/                     LangType.FWordDelim,
/*single-shift three (ss3) '_'*/                   LangType.FWordDelim,
/*device control string (dcs) '_'*/                LangType.FWordDelim,
/*private use one (pu1) '''*/                      LangType.FWordDelim,
/*private use two (pu2) '''*/                      LangType.FWordDelim,
/*set transmit state (sts) '"'*/                   LangType.FWordDelim,
/*cancel character (cch) '"'*/                     LangType.FWordDelim,
/*message waiting (mw) ''*/                       LangType.FWordDelim,
/*start of guarded area (spa) '-'*/                LangType.FWordDelim,
/*end of guarded area (epa) '-'*/                  LangType.FWordDelim,
/*start of string (sos) '_'*/                      LangType.FWordDelim,
/*single graphic character introducer (sgci) 'T'*/ LangType.FWordDelim,
/*single character introducer (sci) '_'*/          LangType.FWordDelim,
/*control sequence introducer (csi) '>'*/          LangType.FWordDelim,
/*string terminator (st) '_'*/                     LangType.FWordDelim,
/*operating system command (osc) '_'*/             LangType.FWordDelim,
/*privacy message (pm) '_'*/                       LangType.FWordDelim,
/*application program command (apc) '_'*/          LangType.FWordDelim,
/*no-break space ' '*/                             LangType.FWordDelim,
/*inverted exclamation mark 'Ў'*/                  LangType.FWordDelim,
/*cent sign 'ў'*/                                  LangType.FWordDelim,
/*pound sign '_'*/                                 LangType.FWordDelim,
/*currency sign '¤'*/                              LangType.FWordDelim,
/*yen sign '_'*/                                   LangType.FWordDelim,
/*broken bar '¦'*/                                 LangType.FWordDelim,
/*section sign '§'*/                               LangType.FWordDelim,
/*diaeresis 'Ё'*/                                  LangType.FWordDelim|LangType.RusUpper|LangType.RussianVowel,
/*copyright sign 'c'*/                             LangType.FWordDelim,
/*feminine ordinal indicator 'Є'*/                 LangType.FWordDelim,
/*left pointing double angle quotation mark '<'*/  LangType.FWordDelim,
/*not sign '¬'*/                                   LangType.FWordDelim,
/*soft hyphen '-'*/                                LangType.FWordDelim,
/*registered sign 'R'*/                            LangType.FWordDelim,
/*macron 'Ї'*/                                     LangType.FWordDelim,
/*degree sign '°'*/                                LangType.FWordDelim,
/*plus-minus sign '+'*/                            LangType.FWordDelim,
/*superscript two '_'*/                            LangType.FWordDelim,
/*superscript three '_'*/                          LangType.FWordDelim,
/*acute '_'*/                                      LangType.FWordDelim,
/*micro sign 'ч'*/                                 LangType.FWordDelim|LangType.GerLower|LangType.GerUpper,
/*pilcrow sign '¶'*/                               LangType.FWordDelim,
/*middle dot '·'*/                                 LangType.FWordDelim,
/*cedilla 'ё'*/                                    LangType.RusLower|LangType.RussianVowel,
/*superscript one '№'*/                            LangType.FWordDelim,
/*masculine ordinal indicator 'є'*/                LangType.FWordDelim,
/*right pointing double angle quotation mark '>'*/ LangType.FWordDelim,
/*vulgar fraction one quarter '_'*/                LangType.FWordDelim,
/*vulgar fraction one half '_'*/                   LangType.FWordDelim,
/*vulgar fraction three quarters '_'*/             LangType.FWordDelim,
/*inverted question mark 'ї'*/                     LangType.FWordDelim,
/*latin capital letter a with grave 'А'*/          LangType.RusUpper|LangType.RussianVowel,
/*latin capital letter a with acute 'Б'*/          LangType.RusUpper,
/*latin capital letter a with circumflex 'В'*/     LangType.RusUpper|LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel,
/*latin capital letter a with tilde 'Г'*/          LangType.RusUpper,
/*latin capital letter a with diaeresis 'Д'*/      LangType.RusUpper|LangType.GerUpper|LangType.LatinVowel,
/*latin capital letter a with ring above 'Е'*/     LangType.RusUpper|LangType.RussianVowel,
/*latin capital ligature ae 'Ж'*/                  LangType.RusUpper,
/*latin capital letter c with cedilla 'З'*/        LangType.RusUpper|LangType.GerUpper|LangType.EngUpper,
/*latin capital letter e with grave 'И'*/          LangType.RusUpper|LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel|LangType.RussianVowel,
/*latin capital letter e with acute 'Й'*/          LangType.RusUpper|LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel,
/*latin capital letter e with circumflex 'К'*/     LangType.RusUpper|LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel,
/*latin capital letter e with diaeresis 'Л'*/      LangType.RusUpper,
/*latin capital letter i with grave 'М'*/          LangType.RusUpper,
/*latin capital letter i with acute 'Н'*/          LangType.RusUpper,
/*latin capital letter i with circumflex 'О'*/     LangType.RusUpper|LangType.RussianVowel,
/*latin capital letter i with diaeresis 'П'*/      LangType.RusUpper,
/*latin capital letter eth (icelandic) 'Р'*/       LangType.RusUpper,
/*latin capital letter n with tilde 'С'*/          LangType.RusUpper|LangType.GerUpper|LangType.EngUpper,
/*latin capital letter o with grave 'Т'*/          LangType.RusUpper,
/*latin capital letter o with acute 'У'*/          LangType.RusUpper|LangType.RussianVowel,
/*latin capital letter o with circumflex 'Ф'*/     LangType.RusUpper|LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel,
/*latin capital letter o with tilde 'Х'*/          LangType.RusUpper,
/*latin capital letter o with diaeresis 'Ц'*/      LangType.RusUpper|LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel,
/*multiplication sign 'Ч'*/                        LangType.RusUpper,
/*latin capital letter o with stroke 'Ш'*/         LangType.RusUpper|LangType.UpRomDigits,
/*latin capital letter u with grave 'Щ'*/          LangType.RusUpper,
/*latin capital letter u with acute 'Ъ'*/          LangType.RusUpper,
/*latin capital letter u with circumflex 'Ы'*/     LangType.RusUpper|LangType.GerUpper|LangType.EngUpper|LangType.LatinVowel|LangType.RussianVowel,
/*latin capital letter u with diaeresis 'Ь'*/      LangType.RusUpper|LangType.GerUpper|LangType.LatinVowel,
/*latin capital letter y with acute 'Э'*/          LangType.RusUpper|LangType.RussianVowel,
/*latin capital letter thorn (icelandic) 'Ю'*/     LangType.RusUpper|LangType.RussianVowel,
/*latin small letter sharp s (german) 'Я'*/        LangType.RusUpper|LangType.GerLower|LangType.GerUpper|LangType.RussianVowel,
/*latin small letter a with grave 'а'*/            LangType.RusLower|LangType.RussianVowel,
/*latin small letter a with acute 'б'*/            LangType.RusLower,
/*latin small letter a with circumflex 'в'*/       LangType.RusLower|LangType.GerLower|LangType.EngLower|LangType.LatinVowel,
/*latin small letter a with tilde 'г'*/            LangType.RusLower,
/*latin small letter a with diaeresis 'д'*/        LangType.RusLower|LangType.GerLower|LangType.LatinVowel,
/*latin small letter a with ring above 'е'*/       LangType.RusLower|LangType.RussianVowel,
/*latin small ligature ae 'ж'*/                    LangType.RusLower,
/*latin small letter c with cedilla 'з'*/          LangType.RusLower|LangType.GerLower|LangType.EngLower,
/*latin small letter e with grave 'и'*/            LangType.RusLower|LangType.GerLower|LangType.EngLower|LangType.LatinVowel|LangType.RussianVowel,
/*latin small letter e with acute 'й'*/            LangType.RusLower|LangType.GerLower|LangType.EngLower|LangType.LatinVowel,
/*latin small letter e with circumflex 'к'*/       LangType.RusLower|LangType.GerLower|LangType.EngLower|LangType.LatinVowel,
/*latin small letter e with diaeresis 'л'*/        LangType.RusLower,
/*latin small letter i with grave 'м'*/            LangType.RusLower,
/*latin small letter i with acute 'н'*/            LangType.RusLower,
/*latin small letter i with circumflex 'о'*/       LangType.RusLower|LangType.RussianVowel,
/*latin small letter i with diaeresis 'п'*/        LangType.RusLower,
/*latin small letter eth (icelandic) 'р'*/         LangType.RusLower,
/*latin small letter n with tilde 'с'*/            LangType.RusLower|LangType.GerLower|LangType.EngLower,
/*latin small letter o with grave 'т'*/            LangType.RusLower,
/*latin small letter o with acute 'у'*/            LangType.RusLower|LangType.RussianVowel,
/*latin small letter o with circumflex 'ф'*/       LangType.RusLower|LangType.GerLower|LangType.EngLower|LangType.LatinVowel,
/*latin small letter o with tilde 'х'*/            LangType.RusLower,
/*latin small letter o with diaeresis 'ц'*/        LangType.RusLower|LangType.GerLower|LangType.EngLower|LangType.LatinVowel,
/*division sign 'ч'*/                              LangType.RusLower,
/*latin small letter o with stroke 'ш'*/           LangType.RusLower,
/*latin small letter u with grave 'щ'*/            LangType.RusLower,
/*latin small letter u with acute 'ъ'*/            LangType.RusLower,
/*latin small letter u with circumflex 'ы'*/       LangType.RusLower|LangType.GerLower|LangType.EngLower|LangType.LatinVowel|LangType.RussianVowel,
/*latin small letter u with diaeresis 'ь'*/        LangType.RusLower|LangType.GerLower|LangType.LatinVowel,
/*latin small letter y with acute 'э'*/            LangType.RusLower|LangType.RussianVowel,
/*latin small letter thorn (icelandic) 'ю'*/       LangType.RusLower|LangType.RussianVowel,
/*latin small letter y with diaeresis  'я'*/       LangType.RusLower|LangType.RussianVowel
};
		internal static bool is_alpha(byte x) {
			return is_russian_alpha(x) || is_german_alpha(x);
		}
		internal static bool is_alpha(byte x, InternalMorphLanguage langua) {
			switch (langua) {
				case InternalMorphLanguage.MorphRussian: return is_russian_alpha(x);
				case InternalMorphLanguage.MorphEnglish: return is_english_alpha(x);
				case InternalMorphLanguage.MorphGerman: return is_german_alpha(x);
				case InternalMorphLanguage.MorphGeneric: return is_generic_alpha(x);
				case InternalMorphLanguage.MorphUrl: return is_URL_alpha(x);
			}
			throw new MorphException("unknown char x");
		}
		internal static bool is_russian_alpha(byte x) {
			return is_russian_lower(x) || is_russian_upper(x);
		}
		internal static bool is_english_alpha(byte x) {
			return is_english_lower(x) || is_english_upper(x);
		}
		internal static bool is_german_alpha(byte x) {
			return is_german_lower(x) || is_german_upper(x);
		}
		internal static bool is_generic_alpha(byte x) {
			return is_english_alpha(x) || x >= 128;
		}
		internal static bool is_URL_alpha(byte x) {
			return (_ascii[x] & LangType.UrlChar) > 0;
		}
		internal static bool is_russian_lower(byte x) {
			return (_ascii[x] & LangType.RusLower) > 0;
		}
		internal static bool is_russian_upper(byte x) {
			return (_ascii[x] & LangType.RusUpper) > 0;
		}
		internal static bool is_english_upper(byte x) {
			return (_ascii[x] & LangType.EngUpper) > 0;
		}
		internal static bool is_english_lower(byte x) {
			return (_ascii[x] & LangType.EngLower) > 0;
		}
		internal static bool is_german_lower(byte x) {
			return (_ascii[x] & LangType.GerLower) > 0;
		}
		internal static bool is_german_upper(byte x) {
			return (_ascii[x] & LangType.GerUpper) > 0;
		}
		internal static bool is_upper_alpha(byte x, InternalMorphLanguage langua) {
			switch (langua) {
				case InternalMorphLanguage.MorphRussian: return is_russian_upper(x);
				case InternalMorphLanguage.MorphEnglish: return is_english_upper(x);
				case InternalMorphLanguage.MorphGerman: return is_german_upper(x);
				case InternalMorphLanguage.MorphGeneric: return is_generic_upper(x);
				case InternalMorphLanguage.MorphUrl: return false;
			}
			return false;
		}
		internal static bool is_generic_upper(byte x) {
			// why ,,
			return (_ascii[x] & LangType.EngUpper) > 0;
		}
		internal static bool is_upper_vowel(byte x, InternalMorphLanguage langua) {
			switch (langua) {
				case InternalMorphLanguage.MorphRussian: return is_russian_upper_vowel(x);
				case InternalMorphLanguage.MorphEnglish: return is_english_upper_vowel(x);
				case InternalMorphLanguage.MorphGerman: return is_german_upper_vowel(x);
			};
			return false;
		}
		internal static bool is_russian_upper_vowel(byte x) {
			return ((_ascii[x] & LangType.RusUpper) > 0)
				&& ((_ascii[x] & LangType.RussianVowel) > 0);
		}
		internal static bool is_english_upper_vowel(byte x) {
			return ((_ascii[x] & LangType.EngUpper) > 0)
				&& ((_ascii[x] & LangType.LatinVowel) > 0);
		}
		internal static bool is_german_upper_vowel(byte x) {
			return (_ascii[x] & LangType.GerUpper) > 0
				&& (_ascii[x] & LangType.LatinVowel) > 0;
		}
		internal static bool is_lower_vowel(byte x, InternalMorphLanguage langua) {
			switch (langua) {
				case InternalMorphLanguage.MorphRussian: return is_russian_lower_vowel(x);
				case InternalMorphLanguage.MorphEnglish: return is_english_lower_vowel(x);
				case InternalMorphLanguage.MorphGerman: return is_german_lower_vowel(x);
			}
			return false;
		}
		internal static bool is_russian_lower_vowel(byte x) {
			return ((_ascii[x] & LangType.RusLower) > 0)
				&& ((_ascii[x] & LangType.RussianVowel) > 0);

		}
		internal static bool is_english_lower_vowel(byte x) {
			return ((_ascii[x] & LangType.EngLower) > 0)
				&& ((_ascii[x] & LangType.LatinVowel) > 0);
		}
		internal static bool is_german_lower_vowel(byte x) {
			return (_ascii[x] & LangType.GerLower) > 0
				&& (_ascii[x] & LangType.LatinVowel) > 0;
		}
		internal static bool is_upper_consonant(byte x, InternalMorphLanguage langua) {
			if (!is_upper_alpha(x, langua)) return false;
			return !is_upper_vowel(x, langua);
		}
	}
}
