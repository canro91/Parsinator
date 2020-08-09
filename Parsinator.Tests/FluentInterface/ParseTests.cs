using NUnit.Framework;
using Parsinator.FluentInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator.Tests.FluentInterface
{
    [TestFixture]
    public class ParseTests
    {
        [Test]
        public void Parse_LineMatchesRegex_ParsesMatchedValue()
        {
            var p = new Dictionary<String, IList<IParse>>
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
        public void Parse_AnyInput_ReturnGivenValue()
        {
            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        Parse.Key("Value").Value("Another value")
                    }
                }
            };
            var lines = FromText(@"
Value: 123456");

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("Another value", ds["Key"]["Value"]);
        }

        private List<List<String>> FromText(String str)
        {
            return new List<List<string>> { str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToList() };
        }
    }
}