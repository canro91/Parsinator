using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator.Tests
{
    [TestFixture]
    public class SkipTests
    {
        [Test]
        public void Parse_SkipBeforeRegexAndAfterRegex_DoNotParseTextBeforeRegex()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipBeforeRegexAndAfterRegex(before: new Regex(@"--BEGIN--"), after: new Regex(@"--END--"))
            };

            var lines = FromText(@"
Value: 123456
--BEGIN--
This line won't be ignored
--END--
");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        [Test]
        public void Parse_SkipBeforeRegexAndAfterRegex_DoNotParseTextAfterRegex()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipBeforeRegexAndAfterRegex(before: new Regex(@"--BEGIN--"), after: new Regex(@"--END--"))
            };

            var lines = FromText(@"
--BEGIN--
This line won't be ignored
--END--
Value: 123456
");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        [Test]
        public void Parse_SkipBeforeRegexAndAfterRegex_ParseTextBetweenRegex()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipBeforeRegexAndAfterRegex(before: new Regex(@"--BEGIN--"), after: new Regex(@"--END--"))
            };

            var lines = FromText(@"
--BEGIN--
Value: 123456
--END--
");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_SkipBlankLines_ParsesTextInNonBlankLines()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipBlankLines()
            };

            var lines = FromText(@"




Value: 123456");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_SkipFromFirstMatchOfRegex_DoNotParseValueAfterMatch()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipFromFirstMatchOfRegex(pattern: new Regex(@"--Ignored all lines below this one--"))
            };
            var lines = FromText(@"
--Ignored all lines below this one--
This line will be ignored
This line will be ignored
Value: 123456 This line will be ignored too");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        [Test]
        public void Parse_SkipIfDoesNotMatch_DoNotParseValueInLinesWithMatch()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipIfDoesNotMatch(new Regex(@"Value:\s*(\d+)"))
            };
            var lines = FromText(@"
This line will be ignored
Value: 123456
This line will be ignored
This line will be ignored");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_SkipIfDoesNotMatch_ParsesTextInLinesThatDontMatch()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromLineWithCountAfterPosition(key: "Value", lineNumber: 1, startPosition: 5, charCount: 9)
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipIfDoesNotMatch(new Regex(@"^12345"))
            };

            var lines = FromText(@"
This line will be ignored
This line will be ignored
This line will be ignored
This line will be ignored
12345Any value");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_SkipIfMatches_DoNotParseValueInLinesWithMatch()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipIfMatches(new Regex(@"This line will be ignored"))
            };
            var lines = FromText(@"
This line will be ignored
Value: 123456
This line will be ignored
This line will be ignored");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_SkipLineCountFromStart_DoNotParseValueInLinesFromStart()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipLineCountFromStart(lineCount: 3),
            };
            var lines = FromText(@"
1 This line will be ignored
2 Value: 123456. This line will be ignored too
3 This line will be ignored
4 This line won't be ignored");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        [Test]
        public void Parse_SkipLineCountFromEnd_DoNotParseValueInLinesFromEnd()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipLineCountFromEnd(lineCount: 3),
            };
            var lines = FromText(@"
1 This line won't be ignored                    4
2 Value: 123456. This line will be ignored too  3
3 This line will be ignored                     2
4 This line will be ignored                     1");

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
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipLineCountFromLineNumber(lineNumber: 2, lineCount: 3),
            };
            var lines = FromText(@"
1 This line won't be ignored                    4
2 Value: 123456. This line will be ignored too  3
3 This line will be ignored                     2
4 This line will be ignored                     1");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        [Test]
        public void Parse_SkipFromFristRegexToLastRegex_DoNotParseValueBetweenRegexes()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipFromFirstRegexToLastRegex(first: new Regex(@"--BEGIN--"), last: new Regex(@"--END--")),
            };
            var lines = FromText(@"
--BEGIN--
1 Value: 123456. This line will be ignored too
2 This line will be ignored
--END--");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        [Test]
        public void Parse_SkipFromFristRegexToLastRegex_ParseValueBeforeFirstRegex()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipFromFirstRegexToLastRegex(first: new Regex(@"--BEGIN--"), last: new Regex(@"--END--")),
            };
            var lines = FromText(@"
Value: 654321
--BEGIN--
1 Value: 123456. This line will be ignored too
2 This line will be ignored
--END--");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsTrue(ds["Key"].ContainsKey("Value"));
            Assert.AreEqual("654321", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_SkipFromFristRegexToLastRegex_ParseValueAfterLastRegex()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipFromFirstRegexToLastRegex(first: new Regex(@"--BEGIN--"), last: new Regex(@"--END--")),
            };
            var lines = FromText(@"
--BEGIN--
1 Value: 123456. This line will be ignored too
2 This line will be ignored
--END--
Value: 654321");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsTrue(ds["Key"].ContainsKey("Value"));
            Assert.AreEqual("654321", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_SkipFromFirstRegexToLastRegexWithoutMatchingPattern_ParseValue()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var s = new List<ISkip>
            {
                new SkipFromFirstRegexToLastRegex(first: new Regex(@"--BEGIN--"), last: new Regex(@"--END--")),
            };
            var lines = FromText(@"
1 This line won't be ignored
2 Value: 123456
3 This line won't be ignored");

            var parser = new Parser(p, s);
            var ds = parser.Parse(lines);

            Assert.IsTrue(ds["Key"].ContainsKey("Value"));
            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }


        private List<List<String>> FromText(String str)
        {
            return new List<List<string>> { str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToList() };
        }
    }
}
