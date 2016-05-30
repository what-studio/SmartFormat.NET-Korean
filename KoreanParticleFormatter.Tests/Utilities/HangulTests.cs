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
	}
}
