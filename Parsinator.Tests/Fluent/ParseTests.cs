using NUnit.Framework;
using Parsinator.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator.Tests.Fluent
{
    [TestFixture]
    public class ParseTests
    {
        [Test]
        public void Parse_LineMatchesRegex_ParsesMatchedValue()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").Regex(new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var lines = FromText(@"
This line doesn't match the given regex
Value: 123456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_AnyInput_ReturnsGivenValue()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").Value("Any given value")
                    }
                }
            };
            var lines = FromText(@"
Value: 123456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any given value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_MultipleLineStringUntilRegex_ParsesStringFromLineNumberUntilRegex()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").FromLine(1).UntilRegex(new Regex(@"--End--")),
                    }
                }
            };
            var lines = FromText(@"
1 Any
2 value
--End--");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("1 Any 2 value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_PatternAndLineNumber_ParsesRegexInTheLine()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").FromLine(2).Regex(new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var lines = FromText(@"
1 This line will be ignored   -3
2 Value: 123456               -2
3 This line will be ignored   -1");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_MultipleLineStringBetweenTwoRegexes_ParseStringBetweenRegexes()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").FromRegex(new Regex(@"--Begin--")).ToRegex(new Regex(@"--End--")),
                    }
                }
            };
            var lines = FromText(@"
This line will be ignored
--Begin--
Any
value
--End--
This line will be ignored");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_MultipleLineStringBetweenTwoRegexes_ParseStringBetweenRegexesIncludingFirst()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").FromFirstRegex(new Regex(@"--Begin--")).ToRegex(new Regex(@"--End--")),
                    }
                }
            };
            var lines = FromText(@"
This line will be ignored
--Begin--
Any
value
--End--
This line will be ignored");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("--Begin-- Any value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_MultipleLineStringBetweenTwoRegexes_ParseStringBetweenRegexesIncludingSecond()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").FromRegex(new Regex(@"--Begin--")).ToLastRegex(new Regex(@"--End--")),
                    }
                }
            };
            var lines = FromText(@"
This line will be ignored
--Begin--
Any
value
--End--
This line will be ignored");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any value --End--", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_MultipleLineStringBetweenTwoRegexes_ParseStringBetweenRegexesIncludingFirstAndSecond()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").FromFirstRegex(new Regex(@"--Begin--")).ToLastRegex(new Regex(@"--End--")),
                    }
                }
            };
            var lines = FromText(@"
This line will be ignored
--Begin--
Any
value
--End--
This line will be ignored");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("--Begin-- Any value --End--", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_PositionAndCountInLine_ParsesStringInGivenPosition()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").FromLine(2).StartingAt(5).Chars(9)
                    }
                }
            };

            var lines = FromText(@"
12345123456789
12345Any value");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_LineMatchesGivenRegex_ParsesMultipleGroupsInMatchedValue()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.MultiGroupRegex(new Regex(@"Value:\s*(?<First>\d+)\s*(?<Second>\d+)"))
                    }
                }
            };
            var lines = FromText(@"
Value: 12345 67890");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("12345", ds["Key"]["First"]);
            Assert.AreEqual("67890", ds["Key"]["Second"]);
        }

        [Test]
        public void Parse_LineSeparatedByComma_ParsesValuesBetweenCommas()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").SplitBy(",")
                    }
                }
            };
            var lines = FromText(@"
Value: 123456, Result: Foo");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsTrue(ds["Key"].ContainsKey("Value[0]"));
            Assert.IsTrue(ds["Key"].ContainsKey("Value[1]"));

            Assert.AreEqual("Value: 123456", ds["Key"]["Value[0]"]);
            Assert.AreEqual("Result: Foo", ds["Key"]["Value[1]"]);
        }

        [Test]
        public void Parse_GivenPageAndMatchingPattern_ParsesPatternFromGivenPage()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").FromPage(2).Regex(new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };

            var lines = FromPagesText(
@"Page1 Value: 654321   Page-2",
@"Page2 Value: 123456   Page-1");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsTrue(ds["Key"].ContainsKey("Value"));
            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        private List<List<string>> FromText(string str)
        {
            return new List<List<string>> { str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToList() };
        }

        private List<List<string>> FromPagesText(params string[] str)
        {
            return str.Select(t => t.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList()).ToList();
        }
    }
}