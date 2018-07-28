﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator.Tests
{
    [TestFixture]
    public partial class ParseTests
    {
        [Test]
        public void Parse_LineMatchesGivenRegex_ParsesMatchedValue()
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
            var lines = FromText(@"
Value: 123456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_LineDoesNotMatchGivenRegex_ParsesDefaultValue()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"), @default: ()=> "default")
                    }
                }
            };
            var lines = FromText(@"
This line doesn't match the given regex");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("default", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_SingleCategoryAndMultipleParses_AppliesMutlipleParsers()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                        new FromValue(key: "Value2", value: "Another value")
                    }
                }
            };
            var lines = FromText(@"
Value: 123456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
            Assert.AreEqual("Another value", ds["Key"]["Value2"]);
        }

        [Test]
        public void Parse_MultipleCategories_AppliesParsesForEveryCategory()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromValue(key: "Value", value: "A value")
                    }
                },
                {
                    "Key2",
                    new List<IParse>
                    {
                        new FromValue(key: "Value2", value: "Another value")
                    }
                }
            };
            var lines = FromText(@"");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("A value", ds["Key"]["Value"]);
            Assert.AreEqual("Another value", ds["Key2"]["Value2"]);
        }

        [Test]
        public void Parse_MultipleLineStringUntilRegex_ParseStringFromLineNumberUntilRegex()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromLineNumberUntilFirstMatchOfRegex(key: "Value", lineNumber: 1, pattern: new Regex(@"--End--"), factory: (allLines) => string.Join(" ", allLines)),
                    }
                }
            };
            var lines = FromText(@"
Any
value
--End--");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_MultipleLineStringUntilRegex_ParseStringsAndConcatenateThemByDefault()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromLineNumberUntilFirstMatchOfRegex(key: "Value", lineNumber: 1, pattern: new Regex(@"--End--"))
                    }
                }
            };
            var lines = FromText(@"
Any
value
--End--");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_PatternAndLineNumber_ParsesRegexInTheLine()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromLineNumberWithRegex(key: "Value", lineNumber: 2, pattern: new Regex(@"Value:\s*(\d+)"))
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
        public void Parse_PatternAndNegativeLineNumber_ParsesRegexInTheLineFromBottom()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromLineNumberWithRegex(key: "Value", lineNumber: -1, pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var lines = FromText(@"
1 This line will be ignored   -3
2 This line will be ignored   -2
3 Value: 123456               -1");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_MultipleLineStringBetweenTwoRegexes_ParseStringBetweenRegexes()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegexToRegex(key: "Value", first: new Regex(@"--Begin--"), second: new Regex(@"--End--"), factory: (allLines) => string.Join(" ", allLines)),
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
        public void Parse_EmpytLineAndFixedValueParser_ParseFixedValue()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromValue(key: "Value", value: "Any value")
                    }
                }
            };
            var lines = FromText(@"");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_NotEmpytLineAndFixedValueParser_ParseFixedValue()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromValue(key: "Value", value: "Any value")
                    }
                }
            };
            var lines = FromText(@"
Anything");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_PositionAndCountInLine_ParsesStringInGivenPosition()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromLineWithCountAfterPosition(key: "Value", lineNumber: 2, startPosition: 5, charCount: 9)
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
        public void Parse_OrElseAndMatchInTheFirstParser_ParsesValueFromFirstParser()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new OrElse(
                            new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new FromRegex(key: "Value", pattern: new Regex(@"Result: \s*(\d+)")))
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
        public void Parse_OrElseAndMatchInTheSecondParser_ParsesValueFromSecondParser()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new OrElse(
                            new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new FromRegex(key: "Value", pattern: new Regex(@"Result: \s*(\d+)")))
                    }
                }
            };

            var lines = FromText(@"
Result: 123456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_RequiredAndPatternNotFound_ThrowsException()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Required(new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
                    }
                }
            };

            var lines = FromText(@"
This line doesn't match");

            var parser = new Parser(p);

            var e = Assert.Throws<ArgumentNullException>(() => parser.Parse(lines));
            StringAssert.Contains("Value", e.Message);
        }

        [Test]
        public void Parse_RequiredWithDefaultValueInInnerParser_DoesNotThrowException()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Required(new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"), @default: () => "default"))
                    }
                }
            };

            var lines = FromText(@"
This line doesn't match");

            var parser = new Parser(p);
            Dictionary<string, Dictionary<string, string>> ds = null;

            Assert.DoesNotThrow(() => { ds = parser.Parse(lines); });
            Assert.AreEqual("default", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_RequiredAndValueParsed_DoesNotThrowException()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Required(new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
                    }
                }
            };

            var lines = FromText(@"
Value: 123456");

            var parser = new Parser(p);
            Dictionary<string, Dictionary<string, string>> ds = null;

            Assert.DoesNotThrow(() => { ds = parser.Parse(lines); });
            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_RequiredAndValueToMatchIsNotInFirstLine_ParsesValueAndDoesNotThrowException()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Required(new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
                    }
                }
            };

            var lines = FromText(@"
This line doesn't match
This line doesn't match
This line doesn't match
Value: 123456");

            var parser = new Parser(p);
            Dictionary<string, Dictionary<string, string>> ds = null;

            Assert.DoesNotThrow(() => { ds = parser.Parse(lines); });
            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_ValidateAndValueParsedThatSatisfyPredicate_ParsesValueAndDoesNotThrowException()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Validate((parsed) => parsed.Length == 6,
                                new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
                    }
                }
            };

            var lines = FromText(@"
Value: 123456 This value has 6 chars, so it's valid");

            var parser = new Parser(p);
            Dictionary<string, Dictionary<string, string>> ds = null;

            Assert.DoesNotThrow(() => { ds = parser.Parse(lines); });
            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_ValidationAndValueParseThatDoesNotSatisfyPredicate_ParsesValueAndThrowsException()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Validate((parsed) => parsed.Length >= 10,
                                new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
                    }
                }
            };

            var lines = FromText(@"
Value: 123456 This value doesn't have more than 10 chars, so it's invalid");

            var parser = new Parser(p);

            var e = Assert.Throws<ArgumentException>(() => parser.Parse(lines));
            StringAssert.Contains("Value", e.Message);
            StringAssert.Contains("123456", e.Message);
        }

        [Test]
        public void Parse_ExceptionInCustomFactory_ThrowsException()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"), factory: (groups) => throw new Exception("An exception"))
                    }
                }
            };

            var lines = FromText(@"
Value: 123456");

            var parser = new Parser(p);

            var e = Assert.Throws<ArgumentException>(() => parser.Parse(lines));
            StringAssert.Contains("Value", e.Message);
            StringAssert.Contains("123456", e.Message);
        }

        [Test]
        public void Parse_AndThenAndMatchInBothParsers_ParsersValueFromBothParsers()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new AndThen(
                            (output) => $"{output.Item1}{output.Item2}",
                            new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new FromRegex(key: "Result", pattern: new Regex(@"Result: \s*(\d+)")))
                    }
                }
            };

            var lines = FromText(@"
Value: 123
Result: 456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value&Result"]);
        }

        [Test]
        public void Parse_AndThenAndFirstParserDoesNotMatch_DoesNotParse()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new AndThen(
                            (output) => $"{output.Item1}{output.Item2}",
                            new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new FromRegex(key: "Result", pattern: new Regex(@"Result: \s*(\d+)")))
                    }
                }
            };

            var lines = FromText(@"
Value: This line doesn't match
Result: 456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value&Result"));
        }

        [Test]
        public void Parse_AndThenAndSecondParserDoesNotMatch_DoesNotParse()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new AndThen(
                            (output) => $"{output.Item1}{output.Item2}",
                            new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new FromRegex(key: "Result", pattern: new Regex(@"Result: \s*(\d+)")))
                    }
                }
            };

            var lines = FromText(@"
Value: 123
Result: This line doesn't match");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value&Result"));
        }

        [Test]
        public void Parse_GivenPageAndMatchingPattern_ParsesPatternFromGivenPage()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromRegex(key: "Value", pageNumber: 2, pattern: new Regex(@"Value:\s*(\d+)")),
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
                        new FromRegex(key: "Value", pageNumber: -1, pattern: new Regex(@"Value:\s*(\d+)")),
                    }
                }
            };

            var lines = FromPagesText(
@"Value: 654321",
@"Value: 123456");

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
                        new FromRegex(key: "Value", pageNumber: -1, pattern: new Regex(@"Value:\s*(\d+)")),
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
                        new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                        new FromRegex(key: "Result", pageNumber: -1, pattern: new Regex(@"Result: \s*(\d+)"))
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
                        new FromRegex(key: "Value", pageNumber: 1, pattern: new Regex(@"Value:\s*(\d+)")),
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
                        new Required(new FromRegex(key: "Value", pageNumber: -1, pattern: new Regex(@"Value:\s*(\d+)")))
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
                        new Validate((parsed) => parsed.Length == 6,
                                new FromRegex(key: "Value", pageNumber: -1, pattern: new Regex(@"Value:\s*(\d+)")))
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

        private List<List<String>> FromPagesText(params String[] str)
        {
            return str.Select(t => t.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList()).ToList();
        }

        private List<List<String>> FromText(String str)
        {
            return new List<List<string>> { str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToList() };
        }
    }
}