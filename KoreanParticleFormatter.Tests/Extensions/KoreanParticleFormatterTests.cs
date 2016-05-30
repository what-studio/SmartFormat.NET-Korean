using System;
using System.Linq;
using NUnit.Framework;
using SmartFormat;
using SmartFormat.Extensions;

namespace KoreanParticleFormatter.Tests.Extensions
{
	[TestFixture]
	class KoreanParticleFormatterTests
	{
		[SetUp]
		public void Setup()
		{
			if (!Smart.Default.FormatterExtensions.Any(x => x is SmartFormat.Extensions.KoreanParticleFormatter))
			{
				Smart.Default.FormatterExtensions.Insert(0, new SmartFormat.Extensions.KoreanParticleFormatter(Smart.Default));
			}
		}

		[TestCase("{0:ko:아} 안녕", "철수", "철수야 안녕")]
		[TestCase("{0:ko:을} 칼로 깎는다", "사과", "사과를 칼로 깎는다")]
		[TestCase("{0:ko:으로}", "김병장", "김병장으로")]
		[TestCase("{0:ko:으로}", "송하나=디바", "송하나=디바로")]
		public void Test_simple(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:으로}", "송하나=디바", "송하나=디바로")]
		[TestCase("{0:으로}", "송하나(디바(D.Va))", "송하나(디바(D.Va))로")]
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

		[TestCase("{0:ko:으로}", "포켓몬(피카츄)", "포켓몬(피카츄)으로")]
		[TestCase("{0:ko:으로}", "송하나(디바(D.Va))", "송하나(디바(D.Va))로")]
		[TestCase("{0:가}?", "임창정,,,", "임창정,,,이?")]
		[TestCase("{0:을} 샀다.", "<듀랑고>", "<듀랑고>를 샀다.")]
		public void Test_Filter(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:ko:은} {1:ko:을} 썼다", "피카츄", "전광석화", "피카츄는 전광석화를 썼다")]
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

		[TestCase("{0:ko(아):{}} 안녕", "철수", "철수야 안녕")]
		[TestCase("{0:ko(아):아}", "철수", "아야")]
		public void Test_FormattingOption(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:ko(은):{}} {1:ko(으로):{}} 진화 했다!", "피카츄", "라이츄", "피카츄는 라이츄로 진화 했다!")]
		[TestCase("{0:은} {1:으로} 진화 했다!", "피카츄", "라이츄", "피카츄는 라이츄로 진화 했다!")]
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

		[TestCase("{0:이다}", "피카츄", "피카츄다")]
		[TestCase("{0:이다}", "버터플", "버터플이다")]
		[TestCase("{0:이에요}", "피카츄", "피카츄예요")]
		[TestCase("{0:이에요}", "버터플", "버터플이에요")]
		[TestCase("{0:입니다}", "피카츄", "피카츄입니다")]
		[TestCase("{0:입니다}", "버터플", "버터플입니다")]
		[TestCase("{0:이다}", "God", "God(이)다")]
		[TestCase("{0:이에요}", "God", "God(이)에요")]
		[TestCase("{0:입니다}", "God", "God입니다")]
		[TestCase("{0:였습니다}", "God", "God(이)었습니다")]
		[TestCase("{0:였습니다}", "버터플", "버터플이었습니다")]
		[TestCase("{0:였습니다}", "피카츄", "피카츄였습니다")]
		[TestCase("{0:이었다}", "피카츄", "피카츄였다")]
		[TestCase("{0:이었지만}", "피카츄", "피카츄였지만")]
		[TestCase("{0:이지만}", "피카츄", "피카츄지만")]
		[TestCase("{0:이지만}", "버터플", "버터플이지만")]
		[TestCase("{0:지만}", "피카츄", "피카츄지만")]
		[TestCase("{0:지만}", "버터플", "버터플이지만")]
		[TestCase("{0:다}", "피카츄", "피카츄다")]
		[TestCase("{0:다}", "버터플", "버터플이다")]
		[TestCase("{0:이에요}", "피카츄", "피카츄예요")]
		[TestCase("{0:이에요}", "버터플", "버터플이에요")]
		[TestCase("{0:고}", "피카츄", "피카츄고")]
		[TestCase("{0:고}", "버터플", "버터플이고")]
		[TestCase("{0:고}", "리자몽", "리자몽이고")]
		[TestCase("{0:여서}", "피카츄", "피카츄여서")]
		[TestCase("{0:여서}", "버터플", "버터플이어서")]
		[TestCase("{0:이어서}", "피카츄", "피카츄여서")]
		[TestCase("{0:이어서}", "버터플", "버터플이어서")]
		[TestCase("{0:라고라}?", "버터플", "버터플이라고라?")]
		public void Test_Ida(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}

		[TestCase("{0:ko(으로):으로}", "용사", "으로로")]
		[TestCase("{0:ko(으로):으로}", "마법", "으로으로")]
		[TestCase("{0:ko(으로):{}}", "마법", "마법으로")]
		[TestCase("{0:ko(으로):{}}", "피카츄(Lv.25)", "피카츄(Lv.25)로")]
		[TestCase("{0:ko(으로):{}}", "피카(?)츄", "피카(?)츄로")]
		[TestCase("{0:ko(으로):{}}", "헬로월드!", "헬로월드!로")]
		[TestCase("{0:ko(으로):{}}", "?_?", "?_?(으)로")]
		[TestCase("{0:ko(으로):<i>{}</i>}", "마법", "<i>마법</i>으로")]
		[TestCase("{0:ko(로서):{}}", "피카츄", "피카츄로서")]
		[TestCase("{0:ko(로서):{}}", "버터플", "버터플로서")]
		[TestCase("{0:ko(로서):{}}", "고라파덕", "고라파덕으로서")]
		[TestCase("{0:ko(로부터):{}}", "피카츄", "피카츄로부터")]
		[TestCase("{0:ko(로부터):{}}", "버터플", "버터플로부터")]
		[TestCase("{0:ko(로부터):{}}", "고라파덕", "고라파덕으로부터")]
		public void Test_Euro(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}


		[TestCase("{0:도}", "피카츄", "피카츄도")]
		[TestCase("{0:도}", "고라파덕", "고라파덕도")]
		[TestCase("{0:에서}", "판교", "판교에서")]
		[TestCase("{0:에서는}", "판교", "판교에서는")]
		[TestCase("{0:께서도}", "각하", "각하께서도")]
		[TestCase("{0:의}", "이상해씨", "이상해씨의")]
		[TestCase("{0:만}", "리자몽", "리자몽만")]
		[TestCase("{0:하고}", "버터플", "버터플하고")]
		public void Test_InvariantParticles(string format, object arg0, string expectedResult)
		{
			Assert.AreEqual(expectedResult, Smart.Format(format, arg0));
		}
	}
}
