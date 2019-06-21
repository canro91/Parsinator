using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator.Tests
{
    public partial class ParseTests
    {
        [Test]
        public void Parse_GivenPageAndMatchingPattern_ParsesPatternFromGivenPage()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pageNumber: 2, pattern: new Regex(@"Value:\s*(\d+)")),
                    }
                }
            };

            var lines = FromPagesText(
@"Value: 654321",
@"Value: 123456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsTrue(ds["Key"].ContainsKey("Value"));
            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_GivenNegativePageAndMatchingPattern_ParsesPatternFromLastPage()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pageNumber: -1, pattern: new Regex(@"Value:\s*(\d+)")),
                    }
                }
            };

            var lines = FromPagesText(
@"1 Value: 654321  -2",
@"2 Value: 123456  -1");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_GivenNegativePageAndSinglePageText_ParsesPatternFromLastPage()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pageNumber: -1, pattern: new Regex(@"Value:\s*(\d+)")),
                    }
                }
            };

            var lines = FromText(@"
Value: 123456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_ParserWithoutPage_ParsesPatternInFirstPage()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                        new ParseFromRegex(key: "Result", pageNumber: -1, pattern: new Regex(@"Result: \s*(\d+)"))
                    }
                }
            };

            var lines = FromPagesText(
@"Value: 123456",
@"Result: 654321");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
            Assert.AreEqual("654321", ds["Key"]["Result"]);
        }

        [Test]
        public void Parse_SingleParserWithPageAndPatternInMultiplePages_ParsesPatternOnlyInGivenPage()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pageNumber: 1, pattern: new Regex(@"Value:\s*(\d+)")),
                    }
                }
            };

            var lines = FromPagesText(
@"Value: 123456",
@"Value: 654321");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_RequiredInnerParserWithPageAndValueParsed_DoesNotThrowException()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Required(new ParseFromRegex(key: "Value", pageNumber: -1, pattern: new Regex(@"Value:\s*(\d+)")))
                    }
                }
            };

            var lines = FromPagesText(
@"Value: 123456",
@"Value: 654321");

            var parser = new Parser(p);
            Dictionary<string, Dictionary<string, string>> ds = null;

            Assert.DoesNotThrow(() => { ds = parser.Parse(lines); });
            Assert.AreEqual("654321", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_ValidateInnerParserWithPageAndValueParsedThatSatisfyPredicate_ParsesValueAndDoesNotThrowException()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Validate((parsed) => parsed.FirstOrDefault().Value?.Length == 6,
                                new ParseFromRegex(key: "Value", pageNumber: -1, pattern: new Regex(@"Value:\s*(\d+)")))
                    }
                }
            };

            var lines = FromPagesText(
@"Value: 123456",
@"Value: 654321 This value has 6 chars, so it's valid");

            var parser = new Parser(p);
            Dictionary<string, Dictionary<string, string>> ds = null;

            Assert.DoesNotThrow(() => { ds = parser.Parse(lines); });
            Assert.AreEqual("654321", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_OrElseAndMatchInTheFirstParserWithPage_ParsesValueFromFirstParserInGivenPage()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new OrElse(
                            new ParseFromRegex(key: "Value", pageNumber: 2, pattern: new Regex(@"Value:\s*(\d+)")),
                            new ParseFromRegex(key: "Result", pageNumber: 3, pattern: new Regex(@"Result:\s*(\d+)")))
                    }
                }
            };

            var lines = FromPagesText(
@"Page1 This is a dummy page",
@"Page2 Value: 123456",
@"Page3 Result: 654321");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_OrElseAndMatchInTheSecondParserWithPage_ParsesValueFromFirstParserInGivenPage()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new OrElse(
                            new ParseFromRegex(key: "Value", pageNumber: 2, pattern: new Regex(@"Value:\s*(\d+)")),
                            new ParseFromRegex(key: "Result", pageNumber: 3, pattern: new Regex(@"Result:\s*(\d+)")))
                    }
                }
            };

            var lines = FromPagesText(
@"Page1 This is a dummy page",
@"Page2 Value: This line doesn't match",
@"Page3 Result: 654321");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("654321", ds["Key"]["Result"]);
        }

        [Test]
        public void Parse_AndThenAndMatchInBothParsersWithPages_ParsersValueFromBothParsersInGivenPages()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new AndThen(
                            (output) => $"{string.Join("", output.Values)}",
                            new ParseFromRegex(key: "Value", pageNumber: 2, pattern: new Regex(@"Value:\s*(\d+)")),
                            new ParseFromRegex(key: "Result", pageNumber: 3, pattern: new Regex(@"Result:\s*(\d+)")))
                    }
                }
            };

            var lines = FromPagesText(
@"Page1 This is a dummy page",
@"Page2 Value: 123",
@"Page3 Result: 456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value&Result"]);
        }

        [Test]
        public void Parse_ConcatenateTwoPagedParsers_ParsersValueFromBothParsersInGivenPages()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Concatenate(key: "Value", separator: "|", new List<IParse>
                        {
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new ParseFromRegex(key: "Result", pageNumber: 2, pattern: new Regex(@"Result:\s*(\w+)"))
                        })
                    }
                }
            };

            var lines = FromPagesText(
@"Page1 Value: 123456",
@"Page2 Result: Foo");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456|Foo", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_ConcatenateTwoPagedParsersAndNegativePageNumber_ParsersValueFromBothParsersInGivenPages()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Concatenate(key: "Value", separator: "|", new List<IParse>
                        {
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new ParseFromRegex(key: "Result", pageNumber: -1, pattern: new Regex(@"Result:\s*(\w+)"))
                        })
                    }
                }
            };

            var lines = FromPagesText(
@"Page1 Value: 123456   Page-2",
@"Page2 Result: Foo     Page-1");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456|Foo", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_ConcatenateTwoPagedParsersAnNegativePageNumberFirst_ParsersValueFromBothParsersInGivenPages()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Concatenate(key: "Value", separator: "|", new List<IParse>
                        {
                            new ParseFromRegex(key: "Result", pageNumber: -1, pattern: new Regex(@"Result:\s*(\w+)")),
                            new ParseFromRegex(key: "Value", pageNumber: 1, pattern: new Regex(@"Value:\s*(\d+)"))
                        })
                    }
                }
            };

            var lines = FromPagesText(
@"Page1 Value: 123456           Page-2",
@"Page2 Result: Foo             Page-1");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Foo|123456", ds["Key"]["Value"]);
        }
    }
}
