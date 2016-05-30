using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace SmartFormat.Utilities
{
	/// <summary>
	/// Manipulates Hangul letters.
	/// </summary>
	public class Hangul
	{
		private readonly char[] _onsets =
		{
			'ㄱ','ㄲ','ㄴ','ㄷ','ㄸ','ㄹ','ㅁ','ㅂ','ㅃ','ㅅ','ㅆ','ㅇ','ㅈ',
			'ㅉ','ㅊ','ㅋ','ㅌ','ㅍ','ㅎ'
		};

		private readonly char[] _nucleuses =
		{
			'ㅏ','ㅐ','ㅑ','ㅒ','ㅓ','ㅔ','ㅕ','ㅖ','ㅗ','ㅘ','ㅙ','ㅚ','ㅛ',
			'ㅜ','ㅝ','ㅞ','ㅟ','ㅠ','ㅡ','ㅢ','ㅣ'
		};

		private readonly char[] _codas =
		{
			'\0', 'ㄱ','ㄲ','ㄳ','ㄴ','ㄵ','ㄶ','ㄷ','ㄹ','ㄺ','ㄻ','ㄼ','ㄽ','ㄾ',
			'ㄿ','ㅀ','ㅁ','ㅂ','ㅄ','ㅅ','ㅆ','ㅇ','ㅈ','ㅊ','ㅋ','ㅌ','ㅍ','ㅎ'
		};

		public char JoinPhonemes(char onset, char nucleus, char coda='\0')
		{
			return (char)((Array.IndexOf(_onsets, onset) * _nucleuses.Length + Array.IndexOf(_nucleuses, nucleus)) * _codas.Length + Array.IndexOf(_codas, coda) + '가');
		}

		public char[] SplitPhonemes(char letter, bool onset = true, bool nucleus = true, bool coda = true)
		{
			if (!(('가' <= letter) && (letter <= '힣')))
			{
				return null;
			}

			var phonemes = new char[3];

			var offset = letter - '가';
			if (onset)
			{
				phonemes[0] = _onsets[offset / (_nucleuses.Length * _codas.Length)];
			}
			if (nucleus)
			{
				phonemes[1] = _nucleuses[(offset / _codas.Length) % _nucleuses.Length];
			}
			if (coda)
			{
				phonemes[2] = _codas[offset % _codas.Length];
			}
			return phonemes;
		}
	}
}
