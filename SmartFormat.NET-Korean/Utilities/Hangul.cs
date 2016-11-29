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

		private readonly string _hangulDigits = "영일이삼사오육칠팔구";
		private readonly Dictionary<int, char> _hangul10Digits = new Dictionary<int, char>
		{
			{1, '십'}, {2, '백'}, {3, '천'}, {4, '만'},
			{8, '억'}, {12, '조'}, {16, '경'}, {20, '해'},
			{24, '자'}, {28, '양'}, {32, '구'}, {36, '간'},
			// 52: 항하사
			{40, '정'}, {44, '재'}, {48, '극'}, {52, '사'},
			// 56: 아승기, 60: 나유타, 64: 불가사의, 68: 무량대수
			{56, '기'}, {60, '타'}, {64, '의'}, {68, '수'},
			{72, '겁'}, {76, '업'}
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

		public static bool IsNumericChar(char c)
		{
			return (('0' <= c) && (c <= '9'));
		}

		public char PickLastHangulCharacterFromNumber(string value)
		{
			// finds the last non-zero digit.
			int startIndex = value.Length;
			for (int i = value.Length-1; i >= 0; i--)
			{
				if (!IsNumericChar(value[i]))
				{
					break;
				}
				startIndex = i;
				if (value[i] != '0')
				{
					break;
				}
			}

			int numberLength = value.Length - startIndex;
			if (numberLength == 1)
			{
				int toInt = (value[startIndex] - '0');
				return _hangulDigits[toInt];
			}

			int findKey = -1;
			foreach (var length in _hangul10Digits.Keys)
			{
				if (length == numberLength - 1)
				{
					return _hangul10Digits[length];
				}
				if (length > numberLength - 1)
				{
					break;
				}
				findKey = length;
			}

			// can't found key
			if (findKey == -1)
			{
				return _hangul10Digits.Last().Value;
			}

			return _hangul10Digits[findKey];
		}
	}
}
