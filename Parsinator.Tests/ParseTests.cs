using NUnit.Framework;
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"), @default: ()=> "default")
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                        new ParseFromValue(key: "Value2", value: "Another value")
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromValue(key: "Value", value: "A value")
                    }
                },
                {
                    "Key2",
                    new List<IParse>
                    {
                        new ParseFromValue(key: "Value2", value: "Another value")
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Flatten(
                            parsed => string.Join(" ", parsed.Values),
                            new ParseFromLineNumberUntilFirstMatchOfRegex(key: "Value", lineNumber: 1, pattern: new Regex(@"--End--")))
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
        public void Parse_MultipleLineStringUntilRegex_ParseStringsAndEnumerateThem()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineNumberUntilFirstMatchOfRegex(key: "Value", lineNumber: 1, pattern: new Regex(@"--End--"))
                    }
                }
            };
            var lines = FromText(@"
Any
value
--End--");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any", ds["Key"]["Value[0]"]);
            Assert.AreEqual("value", ds["Key"]["Value[1]"]);
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
                        new ParseFromLineNumberWithRegex(key: "Value", lineNumber: 2, pattern: new Regex(@"Value:\s*(\d+)"))
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
        public void Parse_PatternInMultipleLinesAndLineNumber_ParsesRegexInTheGivenLine()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineNumberWithRegex(key: "Value", lineNumber: 2, pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var lines = FromText(@"
1 Value: 654321 This line will be ignored   -3
2 Value: 123456                             -2
3 This line will be ignored                 -1");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_PatternAndNegativeLineNumber_ParsesRegexInTheLineFromBottom()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineNumberWithRegex(key: "Value", lineNumber: -1, pattern: new Regex(@"Value:\s*(\d+)"))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Flatten(
                            parsed => string.Join(" ", parsed.Values),
                            new ParseFromRegexToRegex(key: "Value", first: new Regex(@"--Begin--"), second: new Regex(@"--End--"))),
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
        public void Parse_MultipleLineStringBetweenTwoRegexes_ParseStringBetweenRegexesAndEnumerateThem()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegexToRegex(key: "Value", first: new Regex(@"--Begin--"), second: new Regex(@"--End--")),
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

            Assert.AreEqual("Any", ds["Key"]["Value[0]"]);
            Assert.AreEqual("value", ds["Key"]["Value[1]"]);
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
                        new Flatten(
                            parsed => string.Join(" ", parsed.Values),
                            new ParseFromFirstRegexToRegex(key: "Value", first: new Regex(@"--Begin--"), second: new Regex(@"--End--")))
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
                        new Flatten(
                            parsed => string.Join(" ", parsed.Values),
                            new ParseFromRegexToLastRegex(key: "Value", first: new Regex(@"--Begin--"), second: new Regex(@"--End--")))
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
                        new Flatten(
                            allLines => string.Join(" ", allLines.Values),
                            new ParseFromFirstRegexToLastRegex(key: "Value", first: new Regex(@"--Begin--"), second: new Regex(@"--End--")))
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
        public void Parse_EmpytLineAndFixedValueParser_ParseFixedValue()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromValue(key: "Value", value: "Any value")
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromValue(key: "Value", value: "Any value")
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineWithCountAfterPosition(key: "Value", lineNumber: 2, startPosition: 5, charCount: 9)
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
        public void Parse_PositionAndNegativeCountInLine_ParsesStringWithCountFromEnd()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineWithCountAfterPosition(key: "Value", lineNumber: 2, startPosition: 5, charCount: -5)
                    }
                }
            };

            var lines = FromText(@"
1234512345678912345
12345Any value12345");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_PositionAfterHalfLengthAndNegativeCountInLine_ParsesStringWithCountFromEnd()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineWithCountAfterPosition(key: "Value", lineNumber: 2, startPosition: 9, charCount: -5)
                    }
                }
            };

            var lines = FromText(@"
1234512345678912345
12345Any value12345");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("value", ds["Key"]["Value"]);
        }


        [Test]
        public void Parse_NegativePositionAndCountInLine_ParsesStringInPositionFromEnd()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineWithCountAfterPosition(key: "Value", lineNumber: 2, startPosition: -9, charCount: 9)
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
        public void Parse_NegativePositionAndNegativeCountInLine_ParsesStringWithPositionAndCountFromEnd()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineWithCountAfterPosition(key: "Value", lineNumber: 2, startPosition: -14, charCount: -5)
                    }
                }
            };

            var lines = FromText(@"
1234512345678912345
12345Any value12345");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any value", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_PositionAndZeroCountInLine_DoesNotParse()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineWithCountAfterPosition(key: "Value", lineNumber: 2, startPosition: 5, charCount: 0)
                    }
                }
            };

            var lines = FromText(@"
1234512345678912345
12345Any value12345");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        [Test]
        public void Parse_PositionAndCountInLine_ParsesAndFlattensResult()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Flatten(
                            parsed => parsed["Value"].ToUpper(),
                            new ParseFromLineWithCountAfterPosition(key: "Value", lineNumber: 2, startPosition: 5, charCount: 9))
                    }
                }
            };

            var lines = FromText(@"
12345123456789
12345Any value");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("ANY VALUE", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_OrElseAndMatchInTheFirstParser_ParsesValueFromFirstParser()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new OrElse(
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Result: \s*(\d+)")))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new OrElse(
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Result: \s*(\d+)")))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Required(new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Required(new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"), @default: () => "default"))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Required(new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Required(new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Validate((parsed) => parsed.FirstOrDefault().Value?.Length == 6,
                                new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Validate((parsed) => parsed.FirstOrDefault().Value?.Length >= 10,
                                new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
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
        public void Parse_ExceptionInFlatten_ThrowsException()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Flatten(
                            parsed => throw new Exception("An unexpected exception"),
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Flatten(
                            (output) => $"{string.Join("", output.Values)}",
                            new AndThen(
                                new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                                new ParseFromRegex(key: "Result", pattern: new Regex(@"Result: \s*(\d+)"))))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Flatten(
                            (output) => $"{string.Join("", output.Values)}",
                            new AndThen(
                                new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                                new ParseFromRegex(key: "Result", pattern: new Regex(@"Result: \s*(\d+)"))))
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
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Flatten(
                            (output) => $"{string.Join("", output.Values)}",
                            new AndThen(
                                new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                                new ParseFromRegex(key: "Result", pattern: new Regex(@"Result: \s*(\d+)"))))
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
        public void Parse_AndThenAndMatchInBothParsersInTheSameLine_ParsersValueFromBothParsers()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new AndThen(
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new ParseFromRegex(key: "Result", pattern: new Regex(@"Result:\s*(\d+)")))
                    }
                }
            };

            var lines = FromText(@"
Value: 123 Result: 456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123", ds["Key"]["Value"]);
            Assert.AreEqual("456", ds["Key"]["Result"]);
        }

        [Test]
        public void Parse_MatchingParserToParseFrom_ParsesFromOutputOfParser()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromOutput(
                            new ParseFromLineWithCountAfterPosition(key: "Value", lineNumber: 2, startPosition: 5, charCount: 9),
                            new List<IParse>
                            {
                                new ParseFromRegex(key: "First", pattern: new Regex(@"(\w+)\s(\w+)"), factory: (groups) => groups["1"]),
                                new ParseFromRegex(key: "Second", pattern: new Regex(@"(\w+)\s(\w+)"), factory: (groups) => groups["2"]),
                            })
                    }
                }
            };
            var lines = FromText(@"
12345123456789
12345Any value");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Any", ds["Key"]["First"]);
            Assert.AreEqual("value", ds["Key"]["Second"]);
        }

        [Test]
        public void Parse_NonMatchingParserToParseFrom_DoesNotParseFromOutputOfParser()
        {
            var greatherThanLineLength = 100;
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromOutput(
                            new ParseFromLineWithCountAfterPosition(key: "Value", lineNumber: 2, startPosition: 5, charCount: greatherThanLineLength),
                            new List<IParse>
                            {
                                new ParseFromRegex(key: "First", pattern: new Regex(@"(\w+)\s(\w+)"), factory: (groups) => groups["1"]),
                                new ParseFromRegex(key: "Second", pattern: new Regex(@"(\w+)\s(\w+)"), factory: (groups) => groups["2"]),
                            })
                    }
                }
            };
            var lines = FromText(@"
12345123456789
12345Any value");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("First"));
            Assert.IsFalse(ds["Key"].ContainsKey("Second"));
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
                        new ParseFromMultiGroupRegex(pattern: new Regex(@"Value:\s*(?<First>\d+)\s*(?<Second>\d+)"))
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
        public void Parse_IfWithMatchingParserAndTruePredicate_ParsesThenParser()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new IfThen(
                            predicate: (str) => str.Length >= 6,
                            @if: new ParseFromRegex(key: "If", pattern: new Regex(@"Value:\s*(\d+)")),
                            then: new ParseFromRegex(key: "Then", pattern: new Regex(@"Result:\s*(\d+)")))
                    }
                }
            };
            var lines = FromText(@"
Value: 123456 This value has 6 chars, so it's valid
Result: 654321");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["If"]);
            Assert.AreEqual("654321", ds["Key"]["Then"]);
        }

        [Test]
        public void Parse_IfWithMatchingParserInTheSameLine_ParsesThenParser()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new IfThen(
                            predicate: (str) => str.Length >= 6,
                            @if: new ParseFromRegex(key: "If", pattern: new Regex(@"Value:\s*(\d+)")),
                            then: new ParseFromRegex(key: "Then", pattern: new Regex(@"Result:\s*(\d+)")))
                    }
                }
            };
            var lines = FromText(@"
Value: 123456 This value has 6 chars, so it's valid Result: 654321");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["If"]);
            Assert.AreEqual("654321", ds["Key"]["Then"]);
        }

        [Test]
        public void Parse_IfWithMatchingParserAndTruePredicate_DoesNotAddThenParserIfItDoesNotParse()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new IfThen(
                            predicate: (str) => str.Length >= 6,
                            @if: new ParseFromRegex(key: "If", pattern: new Regex(@"Value:\s*(\d+)")),
                            then: new ParseFromRegex(key: "Then", pattern: new Regex(@"Result:\s*(\d+)")))
                    }
                }
            };
            var lines = FromText(@"
Value: 123456 This value has 6 chars, so it's valid
Result: This line doesn't match");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsTrue(ds["Key"].ContainsKey("If"));
            Assert.AreEqual("123456", ds["Key"]["If"]);
            Assert.IsFalse(ds["Key"].ContainsKey("Then"));
        }

        [Test]
        public void Parse_IfWithMatchingParserAndFalsePredicate_ParsesElseParser()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new IfThen(
                            predicate: (str) => str.Length >= 6,
                            @if: new ParseFromRegex(key: "If", pattern: new Regex(@"Value:\s*(\d+)")),
                            then: new ParseFromRegex(key: "Then", pattern: new Regex(@"Result:\s*(\d+)")),
                            @else: new ParseFromRegex(key: "Else", pattern: new Regex(@"Foo:\s*(\w+)")))
                    }
                }
            };
            var lines = FromText(@"
Value: 123 This value has 3 chars, so it's invalid
Result: 654321
Foo: Bar");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123", ds["Key"]["If"]);
            Assert.IsFalse(ds["Key"].ContainsKey("Then"));
            Assert.AreEqual("Bar", ds["Key"]["Else"]);
        }

        [Test]
        public void Parse_IfWithNonMatchingParserAndTruePredicate_DoesNotParseSingleThenParser()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new IfThen(
                            predicate: (str) => str.Length >= 6,
                            @if: new ParseFromRegex(key: "If", pattern: new Regex(@"Value:\s*(\d+)")),
                            then: new ParseFromRegex(key: "Then", pattern: new Regex(@"Result:\s*(\d+)")))
                    }
                }
            };
            var lines = FromText(@"
This line doesn't match
Result: 654321");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("If"));
            Assert.IsFalse(ds["Key"].ContainsKey("Then"));
        }

        [Test]
        public void Parse_SingleMatchingLineAndMultipleNonMatchingLines_ParsesMatchedValueAndStopsParsing()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
                    }
                }
            };
            var lines = FromText(@"
This line doesn't match
Value: 123456
This line doesn't match
This line doesn't match
This line doesn't match
This line doesn't match");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_NotParseAndAMatchingParser_ThrowsException()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Not(new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
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
        public void Parse_NotParseAndANonMatchingParserWithDefaultValue_ThrowsException()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Not(new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"), @default: () => "Default"))
                    }
                }
            };
            var lines = FromText(@"
This line doesn't match the given regex");

            var parser = new Parser(p);

            var e = Assert.Throws<ArgumentException>(() => parser.Parse(lines));
            StringAssert.Contains("Value", e.Message);
            StringAssert.Contains("Default", e.Message);
        }

        [Test]
        public void Parse_NotParseAndANonMatchingParserWithoutDefaultValue_DoesNotThrowException()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Not(new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")))
                    }
                }
            };
            var lines = FromText(@"
This line doesn't match the given regex");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsFalse(ds["Key"].ContainsKey("Value"));
        }

        [Test]
        public void Parse_TwoMatchingParsers_ParsesValueAndConcatenatesThem()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Concatenate(key: "Value", separator: "|", parsers: new List<IParse>
                        {
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new ParseFromRegex(key: "Result", pattern: new Regex(@"Result:\s*(\w+)")),
                        })
                    }
                }
            };
            var lines = FromText(@"
Value: 123456
Result: Foo");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456|Foo", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_TwoNonMatchingParsersWithDefaultValues_ParsesValueAndConcatenatesDefaultValues()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Concatenate(key: "Value", separator: "|", parsers: new List<IParse>
                        {
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"), @default: () => "123456"),
                            new ParseFromRegex(key: "Result", pattern: new Regex(@"Result:\s*(\w+)"), @default: () => "Foo"),
                        })
                    }
                }
            };
            var lines = FromText(@"
This line doesn't match
This line doesn't match");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456|Foo", ds["Key"]["Value"]);
        }

        [Test]
        public void Parse_AMatchingAndNonMatchingParsersWithDefaultValue_ParsesValueAndConcatenatesParsedAndDefaultValues()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new Concatenate(key: "Value", separator: "|", parsers: new List<IParse>
                        {
                            new ParseFromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)")),
                            new ParseFromRegex(key: "Result", pattern: new Regex(@"Result:\s*(\w+)"), @default: () => "Foo"),
                        })
                    }
                }
            };
            var lines = FromText(@"
Value: 123456
This line doesn't match");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("123456|Foo", ds["Key"]["Value"]);
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
                        new ParseFromSplitting(key: "Value", separator: ",")
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
        public void Parse_LineSeparatedByCommaAndIgnoreTwoSegments_ParsesValuesAfterTheSecondComma()
        {
            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromSplitting(key: "Value", separator: ",", skipFirsts: 2)
                    }
                }
            };
            var lines = FromText(@"
This segment will be ignored, This segment will be ignored too, Value: 123456, Result: Foo");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.IsTrue(ds["Key"].ContainsKey("Value[0]"));
            Assert.IsTrue(ds["Key"].ContainsKey("Value[1]"));
            Assert.AreEqual("Value: 123456", ds["Key"]["Value[0]"]);
            Assert.AreEqual("Result: Foo", ds["Key"]["Value[1]"]);
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
