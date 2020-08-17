using NUnit.Framework;
using Parsinator.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator.Tests.Fluent
{
    [TestFixture]
    public class SkipTests
    {
        [Test]
        public void Parse_SkipBeforeRegexAndAfterRegex_DoNotParseTextBeforeOrAfterRegexes()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                Skip.BeforeRegex(new Regex(@"--BEGIN--")).AndAfterRegex(new Regex(@"--END--"))
            };

            var lines = FromText(@"
Value: 123456
--BEGIN--
This line won't be ignored
--END--
Value: 654321
");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        [Test]
        public void Parse_SkipLineCountFromLineNumber_DoNotParseValueInLinesInRange()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                Skip.Lines(3).StartingFromLine(2),
            };
            var lines = FromText(@"
1 This line won't be ignored                    -4
2 Value: 123456. This line will be ignored      -3
3 This line will be ignored too                 -2
4 This line will be ignored too                 -1");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        [Test]
        public void Parse_SkipFromFirstRegexToLastRegex_DoNotParseValueBetweenRegexes()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                Skip.FromFirstRegex(new Regex(@"--BEGIN--")).ToLastRegex(new Regex(@"--END--")),
            };
            var lines = FromText(@"
--BEGIN--
1 Value: 123456. This line will be ignored
2 This line will be ignored too
--END--");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        private List<List<String>> FromText(String str)
        {
            return new List<List<string>> { str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToList() };
        }
    }
}