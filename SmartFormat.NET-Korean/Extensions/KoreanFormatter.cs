using System;
using System.Linq;
using System.Text.RegularExpressions;
using SmartFormat.Core.Extensions;
using SmartFormat.Utilities;

namespace SmartFormat.Extensions
{
	public class KoreanFormatter : IFormatter
	{
		private string[] names = { "ko", "" };
		public string[] Names { get { return names; } set { names = value; } }
		private readonly SmartFormatter _formatter = null;
		private Hangul _hangul = new Hangul();

		private readonly Regex _filterPattern = new Regex(@"\(.*[^\(]?\)|[!@#$%^$*?,.:;'""\[\]{}<>]+");

		internal class SyllableInfo
		{
			// in Korean, `Coda` means final position of syllable
			public bool HasCoda;
			public bool HasRieulCoda;
		}

		// Particle phonology
		private readonly string[] _simpleParticles =
		{
			"을를", "아야", "이가", "은는", "과와"
		};

		private readonly string[] _idaExcepts =
		{
			"여", "시여"
		};

		private readonly Regex _invariantParticlePattern = new Regex(@"^((의|도|만|보다|부터|까지|마저|조차)$|에|께|하)");

		private readonly Regex _euroPattern = new Regex(@"^(으|\(으\))?로");
		private readonly Regex _idaPrefixPattern = new Regex(@"^이|\(이\)");

		public KoreanFormatter(SmartFormatter formatter)
		{
			_formatter = formatter;
		}

		public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
		{
			if (formattingInfo.Format == null || string.IsNullOrEmpty(formattingInfo.Format.RawText))
			{
				return false;
			}

			string currentValue = null;
			if (formattingInfo.CurrentValue is string)
			{
				currentValue = (string) formattingInfo.CurrentValue;
			}
			else
			{
				return false;
			}

			SyllableInfo syllableInfo = EvaluateSyllable(currentValue);

			var format = formattingInfo.FormatterOptions;
			bool implicitly = string.IsNullOrEmpty(format);
			bool onlyParticle = false;
			if (implicitly)
			{
				format = formattingInfo.Format.RawText;
				onlyParticle = format[0] == '-';
				if (onlyParticle)
				{
					format = format.Substring(1);
				}
			}

			string particle = ParticleConverter(format, syllableInfo);
			if (string.IsNullOrEmpty(particle))
			{
				return false;
			}

			if (onlyParticle)
			{
				formattingInfo.Write(particle);
			}
			else
			{
				if (implicitly)
				{
					formattingInfo.Write(currentValue);
					formattingInfo.Write(particle);
				}
				else
				{
					var newFormat = this._formatter.Parser.ParseFormat(formattingInfo.Format.RawText + particle);
					formattingInfo.Write(newFormat, formattingInfo.CurrentValue);
				}
			}
			return true;
		}

		private bool TryParseIda(string format, SyllableInfo syllableInfo, out string result)
		{
			// remove "이" or "(이)" prefix
			var suffix = _idaPrefixPattern.Replace(format, "");
			if (string.IsNullOrEmpty(suffix))
			{
				result = null;
				return false;
			}

			if (!_idaExcepts.Contains(suffix))
			{
				var phonemes = _hangul.SplitPhonemes(suffix[0]);
				if (phonemes == null)
				{
					result = null;
					return false;
				}
				var onset = phonemes[0];
				var nucleus = phonemes[1];
				var coda = phonemes[2];
				if (onset == 'ㅇ')
				{
					if (nucleus == 'ㅣ')
					{
						// No allomorphs when a form starts with "이" and has a coda.
						result = suffix;
						return true;
					}

					bool hasCoda = (syllableInfo == null || syllableInfo.HasCoda);
					char nextNucleus = '\0';
					if (!hasCoda && (nucleus == 'ㅓ' || nucleus == 'ㅔ'))
					{
						nextNucleus = (nucleus == 'ㅓ') ? 'ㅕ' : 'ㅖ';
					}
					else if (hasCoda && (nucleus == 'ㅕ' || nucleus == 'ㅖ'))
					{
						nextNucleus = (nucleus == 'ㅕ') ? 'ㅓ' : 'ㅔ';
					}

					if (nextNucleus != '\0')
					{
						var nextLetter = _hangul.JoinPhonemes('ㅇ', nextNucleus, coda);
						suffix = nextLetter + suffix.Substring(1);
					}
				}
			}

			if (syllableInfo == null)
			{
				result = "(이)" + suffix;
			}
			else
			{
				result = syllableInfo.HasCoda ? '이' + suffix : suffix;
			}
			return true;
		}

		private SyllableInfo EvaluateSyllable(string value)
		{
			var filteredValue = _filterPattern.Replace(value, "");

			if (string.IsNullOrEmpty(filteredValue))
			{
				return null;
			}

			var lastChar = filteredValue[filteredValue.Length - 1];

			// `Hangul` unicode range: 가(U+AC00) ~ 힣(U+D7A3)
			if (!(('가' <= lastChar) && (lastChar <= '힣')))
			{
				return null;
			}

			int jongsungExpr = (lastChar - '가') % 28;

			return new SyllableInfo()
			{
				HasCoda = jongsungExpr != 0,
				HasRieulCoda = jongsungExpr == 8
			};
		}

		private string ParticleConverter(string josaFormat, SyllableInfo syllableInfo)
		{
			if (josaFormat.Length == 1)
			{
				var josa = josaFormat[0];
				foreach (var j in _simpleParticles)
				{
					if (josa == j[0] || josa == j[1])
					{
						if (syllableInfo == null)
						{
							return string.Format("{0}({1})", j[0], j[1]);
						}

						int toIndex = syllableInfo.HasCoda ? 0 : 1;
						return j[toIndex].ToString();
					}
				}
			}

			var euroMatch = _euroPattern.Match(josaFormat);
			if (euroMatch.Success)
			{
				// remove '으로' prefix
				var suffixString = josaFormat.Substring(euroMatch.Value.Length);
				if (syllableInfo == null)
				{
					return string.Format("(으)로{0}", suffixString);
				}

				return (!syllableInfo.HasCoda || syllableInfo.HasRieulCoda) ? '로' + suffixString: "으로" + suffixString;
			}

			if (_invariantParticlePattern.IsMatch(josaFormat))
			{
				return josaFormat;
			}

			string idaResult = null;
			if (TryParseIda(josaFormat, syllableInfo, out idaResult))
			{
				return idaResult;
			}

			return null;
		}
	}
}
