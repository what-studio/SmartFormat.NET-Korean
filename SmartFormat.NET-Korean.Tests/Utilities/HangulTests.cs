using System;
using NUnit.Framework;
using SmartFormat.Utilities;

namespace KoreanParticleFormatter.Tests.Utilities
{
	[TestFixture]
	internal class HangulTests
	{
		private readonly Hangul _hangul = new Hangul();

		[TestCase('ㅇ', 'ㅏ', 'ㄴ', '안')]
		[TestCase('ㄴ', 'ㅕ', 'ㅇ', '녕')]
		[TestCase('ㅎ', 'ㅏ', '\0', '하')]
		[TestCase('ㅅ', 'ㅔ', '\0', '세')]
		[TestCase('ㅇ', 'ㅛ', '\0', '요')]
		public void Test_JoinPhonemes(char onset, char nuclues, char coda, char expectedResult)
		{
			Assert.AreEqual(expectedResult, _hangul.JoinPhonemes(onset, nuclues, coda));
		}

		[TestCase('안', new char[] { 'ㅇ', 'ㅏ', 'ㄴ' })]
		[TestCase('녕', new char[] { 'ㄴ', 'ㅕ', 'ㅇ' })]
		[TestCase('하', new char[] { 'ㅎ', 'ㅏ', '\0'})]
		[TestCase('세', new char[] { 'ㅅ', 'ㅔ', '\0'})]
		[TestCase('요', new char[] { 'ㅇ', 'ㅛ', '\0'})]
		public void Test_SplitPhonemes(char letter, char[] expectedResult)
		{
			Assert.AreEqual(expectedResult, _hangul.SplitPhonemes(letter));
		}

		[TestCase("10", '십')]
		[TestCase("200", '백')]
		[TestCase("3000", '천')]
		[TestCase("40000", '만')]
		[TestCase("500000", '만')]
		[TestCase("6000000", '만')]
		[TestCase("70000000", '만')]
		[TestCase("800000000", '억')]
		[TestCase("9000000000", '억')]
		[TestCase("1000001000", '천')]
		[TestCase("2000003020", '십')]
		[TestCase("0", '영')]
		[TestCase("1", '일')]
		[TestCase("2", '이')]
		[TestCase("3", '삼')]
		[TestCase("4", '사')]
		[TestCase("5", '오')]
		[TestCase("6", '육')]
		[TestCase("7", '칠')]
		[TestCase("8", '팔')]
		[TestCase("9", '구')]
		public void Test_LastHangulCharacterFromNumber(string numberString, char expectedResult)
		{
			Assert.AreEqual(expectedResult, _hangul.PickLastHangulCharacterFromNumber(numberString));
		}
	}
}
