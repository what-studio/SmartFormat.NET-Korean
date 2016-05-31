using System;
using System.Linq;
using NUnit.Framework;
using SmartFormat;
using SmartFormat.Extensions;

namespace KoreanParticleFormatter.Tests.Extensions
{
	[TestFixture]
	class KoreanFormatterTests
	{
		[SetUp]
		public void Setup()
		{
			if (!Smart.Default.FormatterExtensions.Any(x => x is SmartFormat.Extensions.KoreanFormatter))
			{
				Smart.Default.FormatterExtensions.Insert(0, new SmartFormat.Extensions.KoreanFormatter(Smart.Default));
			}
		}

		[TestCase("{0:ko:아} 안녕", "나오", "나오야 안녕")]
		[TestCase("{0:ko:을} 칼로 깎는다", "사과", "사과를 칼로 깎는다")]
		[TestCase("{0:ko:으로}", "모리안", "모리안으로")]
		[TestCase("{0:ko:으로}", "퍼거스=대장장이", "퍼거스=대장장이로")]
		public void Test_simple(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:으로}", "퍼거스=대장장이", "퍼거스=대장장이로")]
		[TestCase("{0:으로}", "퍼거스(Ferghus)", "퍼거스(Ferghus)로")]
		public void Test_Implicit(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:를}", "", "을(를)")]
		[TestCase("{0:을}", "ㅋㅋㅋ", "ㅋㅋㅋ을(를)")]
		[TestCase("{0:은}", "", "은(는)")]
		[TestCase("{0:는}", "", "은(는)")]
		[TestCase("{0:이}", "", "이(가)")]
		[TestCase("{0:가}", "", "이(가)")]
		[TestCase("{0:과}", "", "과(와)")]
		[TestCase("{0:와}", "", "과(와)")]
		[TestCase("{0:으로}", "", "(으)로")]
		[TestCase("{0:로}", "", "(으)로")]
		public void Test_DoublePostposition(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:ko:으로}", "모리안(여신)", "모리안(여신)으로")]
		[TestCase("{0:ko:으로}", "퍼거스(대장장이(Ferghus))", "퍼거스(대장장이(Ferghus))로")]
		[TestCase("{0:가}?", "모리안,,,", "모리안,,,이?")]
		[TestCase("{0:을} 샀다.", "<듀랑고>", "<듀랑고>를 샀다.")]
		public void Test_Filter(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:ko:은} {1:ko:로} 불린다.", "나오", "검은사신", "나오는 검은사신으로 불린다.")]
		public void Test_Arg2(string format, object arg0, object arg1, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0, arg1));
		}

		[TestCase("<i>{0}</i>{0:ko:-으로}", "돌날", "<i>돌날</i>로")]
		[TestCase("{0:-으로}", "마법", "으로")]
		public void Test_DashPrefix(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:ko(아):{}} 안녕", "나오", "나오야 안녕")]
		[TestCase("{0:ko(아):아}", "나오", "아야")]
		public void Test_FormattingOption(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:ko(은):{}} {1:ko(으로):{}} 불린다.", "나오", "검은사신", "나오는 검은사신으로 불린다.")]
		[TestCase("{0:은} {1:으로} 변신 했다!", "밀레시안", "팔라딘", "밀레시안은 팔라딘으로 변신 했다!")]
		public void Test_FormattingOption_Arg2(string format, object arg0, object arg1, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0, arg1));
		}

		[TestCase("{0:야}", "친구", "친구야")]
		[TestCase("{0:야}", "사랑", "사랑아")]
		[TestCase("{0:아}", "사랑", "사랑아")]
		[TestCase("{0:여}", "친구", "친구여")]
		[TestCase("{0:여}", "사랑", "사랑이여")]
		[TestCase("{0:이시여}", "하늘", "하늘이시여")]
		[TestCase("{0:이시여}", "바다", "바다시여")]
		public void Test_Vocative_Particles(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:이다}", "나오", "나오다")]
		[TestCase("{0:이다}", "키홀", "키홀이다")]
		[TestCase("{0:이에요}", "나오", "나오예요")]
		[TestCase("{0:이에요}", "키홀", "키홀이에요")]
		[TestCase("{0:입니다}", "나오", "나오입니다")]
		[TestCase("{0:입니다}", "키홀", "키홀입니다")]
		[TestCase("{0:이다}", "Nao", "Nao(이)다")]
		[TestCase("{0:이에요}", "Nao", "Nao(이)에요")]
		[TestCase("{0:입니다}", "Nao", "Nao입니다")]
		[TestCase("{0:였습니다}", "Nao", "Nao(이)었습니다")]
		[TestCase("{0:였습니다}", "키홀", "키홀이었습니다")]
		[TestCase("{0:였습니다}", "나오", "나오였습니다")]
		[TestCase("{0:이었다}", "나오", "나오였다")]
		[TestCase("{0:이었지만}", "나오", "나오였지만")]
		[TestCase("{0:이지만}", "나오", "나오지만")]
		[TestCase("{0:이지만}", "키홀", "키홀이지만")]
		[TestCase("{0:지만}", "나오", "나오지만")]
		[TestCase("{0:지만}", "키홀", "키홀이지만")]
		[TestCase("{0:다}", "나오", "나오다")]
		[TestCase("{0:다}", "키홀", "키홀이다")]
		[TestCase("{0:이에요}", "나오", "나오예요")]
		[TestCase("{0:이에요}", "키홀", "키홀이에요")]
		[TestCase("{0:고}", "나오", "나오고")]
		[TestCase("{0:고}", "키홀", "키홀이고")]
		[TestCase("{0:고}", "모리안", "모리안이고")]
		[TestCase("{0:여서}", "나오", "나오여서")]
		[TestCase("{0:여서}", "키홀", "키홀이어서")]
		[TestCase("{0:이어서}", "나오", "나오여서")]
		[TestCase("{0:이어서}", "키홀", "키홀이어서")]
		[TestCase("{0:라고라}?", "키홀", "키홀이라고라?")]
		public void Test_Ida(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:ko(으로):으로}", "용사", "으로로")]
		[TestCase("{0:ko(으로):으로}", "마법", "으로으로")]
		[TestCase("{0:ko(으로):{}}", "마법", "마법으로")]
		[TestCase("{0:ko(으로):{}}", "나오(Lv.25)", "나오(Lv.25)로")]
		[TestCase("{0:ko(으로):{}}", "퍼거(?)스", "퍼거(?)스로")]
		[TestCase("{0:ko(으로):{}}", "헬로월드!", "헬로월드!로")]
		[TestCase("{0:ko(으로):{}}", "?_?", "?_?(으)로")]
		[TestCase("{0:ko(으로):<i>{}</i>}", "마법", "<i>마법</i>으로")]
		[TestCase("{0:ko(로서):{}}", "나오", "나오로서")]
		[TestCase("{0:ko(로서):{}}", "키홀", "키홀로서")]
		[TestCase("{0:ko(로서):{}}", "모리안", "모리안으로서")]
		[TestCase("{0:ko(로부터):{}}", "나오", "나오로부터")]
		[TestCase("{0:ko(로부터):{}}", "키홀", "키홀로부터")]
		[TestCase("{0:ko(로부터):{}}", "모리안", "모리안으로부터")]
		public void Test_Euro(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}


		[TestCase("{0:도}", "나오", "나오도")]
		[TestCase("{0:도}", "모리안", "모리안도")]
		[TestCase("{0:에서}", "판교", "판교에서")]
		[TestCase("{0:에서는}", "판교", "판교에서는")]
		[TestCase("{0:께서도}", "선생님", "선생님께서도")]
		[TestCase("{0:의}", "나오", "나오의")]
		[TestCase("{0:만}", "모리안", "모리안만")]
		[TestCase("{0:하고}", "키홀", "키홀하고")]
		public void Test_InvariantParticles(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}
	}
}
