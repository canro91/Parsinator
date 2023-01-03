using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator.Sample
{
    [TestFixture]
    public class FrameTests
    {
        [Test]
        public void Parse_PositionFrame_ParsesFrame()
        {
            var frame = "242400676315802812345699553131333035392E30302C412C313032342E37363537392C4E2C30373532382E30313237392C572C302E3030302C2C3235303131377C31322E327C3139347C303030307C303030302C303030307C3032383033303433395BF80D0A";

            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineWithCountAfterPosition(key: "Header",  lineNumber: 1, startPosition: 0, charCount: 4),
                        new ParseFromLineWithCountAfterPosition(key: "Length",  lineNumber: 1, startPosition: 4, charCount: 4),
                        new ParseFromLineWithCountAfterPosition(key: "IMEI",    lineNumber: 1, startPosition: 8, charCount: 14),
                        new ParseFromLineWithCountAfterPosition(key: "Cmd",     lineNumber: 1, startPosition: 22, charCount: 4),
                        new ParseFromLineWithCountAfterPosition(key: "Body",    lineNumber: 1, startPosition: 26, charCount: -8),
                        new ParseFromLineWithCountAfterPosition(key: "CheckSum",lineNumber: 1, startPosition: -8, charCount: 4),
                        new ParseFromLineWithCountAfterPosition(key: "EOF",     lineNumber: 1, startPosition: -4, charCount: 4),
                    }
                }
            };

            var lines = new List<List<string>> { new List<string> { frame } };

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("2424", ds["Key"]["Header"]);
            Assert.AreEqual("0067", ds["Key"]["Length"]);
            Assert.AreEqual("63158028123456", ds["Key"]["IMEI"]);
            Assert.AreEqual("9955", ds["Key"]["Cmd"]);
            Assert.AreEqual("3131333035392E30302C412C313032342E37363537392C4E2C30373532382E30313237392C572C302E3030302C2C3235303131377C31322E327C3139347C303030307C303030302C303030307C303238303330343339", ds["Key"]["Body"]);
            Assert.AreEqual("5BF8", ds["Key"]["CheckSum"]);
            Assert.AreEqual("0D0A", ds["Key"]["EOF"]);
        }

        [Test]
        public void Parse_PositionFrame_ParsesBody()
        {
            var frame = "242400676315802812345699553131333035392E30302C412C313032342E37363537392C4E2C30373532382E30313237392C572C302E3030302C2C3235303131377C31322E327C3139347C303030307C303030302C303030307C3032383033303433395BF80D0A";

            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineWithCountAfterPosition(key: "Header",  lineNumber: 1, startPosition: 0, charCount: 4),
                        new ParseFromLineWithCountAfterPosition(key: "Length",  lineNumber: 1, startPosition: 4, charCount: 4),
                        new ParseFromLineWithCountAfterPosition(key: "IMEI",    lineNumber: 1, startPosition: 8, charCount: 14),
                        new ParseFromLineWithCountAfterPosition(key: "Cmd",     lineNumber: 1, startPosition: 22, charCount: 4),
                        new ParseFromOutput(
                            parseFrom:
                                new Flatten(
                                    (parsed) => ConvertHexToAscii(parsed["Body"]),
                                    new ParseFromLineWithCountAfterPosition(key: "Body", lineNumber: 1, startPosition: 26, charCount: -8)),
                            parsers: new List<IParse>
                            {
                                new ParseFromMultiGroupRegex(pattern: new Regex(@"(?<TimeStamp>\d{6}\.\d{2}),(?<Validity>A|V),(?<Latitude>\d{4,5}\.\d{5}),(?<NorthSouth>N|S),(?<Longitude>\d{4,5}\.\d{5}),(?<EastWest>E|W),(?<SpeedInKnots>\d+\.\d+),(?<TrueCourse>\d+\.\d+)?,(?<DateStamp>\d{6})\|(?<Custom>\S*)"))
                            }),
                        new ParseFromLineWithCountAfterPosition(key: "CheckSum",lineNumber: 1, startPosition: -8, charCount: 4),
                        new ParseFromLineWithCountAfterPosition(key: "EOF",     lineNumber: 1, startPosition: -4, charCount: 4),
                    }
                }
            };

            var lines = new List<List<string>> { new List<string> { frame } };

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("2424", ds["Key"]["Header"]);
            Assert.AreEqual("0067", ds["Key"]["Length"]);
            Assert.AreEqual("63158028123456", ds["Key"]["IMEI"]);
            Assert.AreEqual("9955", ds["Key"]["Cmd"]);

            Assert.AreEqual("113059.00", ds["Key"]["TimeStamp"]);
            Assert.AreEqual("A", ds["Key"]["Validity"]);
            Assert.AreEqual("1024.76579", ds["Key"]["Latitude"]);
            Assert.AreEqual("N", ds["Key"]["NorthSouth"]);
            Assert.AreEqual("07528.01279", ds["Key"]["Longitude"]);
            Assert.AreEqual("W", ds["Key"]["EastWest"]);
            Assert.AreEqual("0.000", ds["Key"]["SpeedInKnots"]);
            Assert.AreEqual("", ds["Key"]["TrueCourse"]);
            Assert.AreEqual("250117", ds["Key"]["DateStamp"]);
            Assert.AreEqual("12.2|194|0000|0000,0000|028030439", ds["Key"]["Custom"]);

            Assert.AreEqual("5BF8", ds["Key"]["CheckSum"]);
            Assert.AreEqual("0D0A", ds["Key"]["EOF"]);
        }

        private string ConvertHexToAscii(String hexString)
        {
            string ascii = string.Empty;

            for (int i = 0; i < hexString.Length; i += 2)
            {
                String hs = hexString.Substring(i, 2);
                uint decval = System.Convert.ToUInt32(hs, 16);
                char character = System.Convert.ToChar(decval);
                ascii += character;
            }

            return ascii;
        }

        [Test]
        public void Parse_EventFrame_ParsesBody()
        {
            var frame = "24240064621700171234569999013135323331342C412C313032322E373936332C4E2C30373532392E303135312C572C3030302C3333322C3139303731377C31322E327C3139347C304230307C303030302C303439387C3030303030303030324BAA0D0A";

            var p = new Dictionary<string, IEnumerable<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromLineWithCountAfterPosition(key: "Header",  lineNumber: 1, startPosition: 0, charCount: 4),
                        new ParseFromLineWithCountAfterPosition(key: "Length",  lineNumber: 1, startPosition: 4, charCount: 4),
                        new ParseFromLineWithCountAfterPosition(key: "IMEI",    lineNumber: 1, startPosition: 8, charCount: 14),
                        new IfThen(
                            predicate: (str) => str == "9999",
                            @if: new ParseFromLineWithCountAfterPosition(key: "Cmd", lineNumber: 1, startPosition: 22, charCount: 4),
                            then: new AndThen(
                                first: new ParseFromLineWithCountAfterPosition(key: "Event", lineNumber: 1, startPosition: 26, charCount: 2),
                                second: new ParseFromOutput(
                                    parseFrom:
                                        new Flatten(
                                            parsed => ConvertHexToAscii(parsed["Body"]),
                                            new ParseFromLineWithCountAfterPosition(key: "Body", lineNumber: 1, startPosition: 28, charCount: -8)),
                                    parsers: new List<IParse>
                                    {
                                        new ParseFromMultiGroupRegex(pattern: new Regex(@"(?<TimeStamp>\d{6}(\.\d{2})?),(?<Validity>A|V),(?<Latitude>\d{4,5}\.\d{4,5}),(?<NorthSouth>N|S),(?<Longitude>\d{4,5}\.\d{4,5}),(?<EastWest>E|W),(?<SpeedInKnots>\d+(\.\d+)?),(?<TrueCourse>\d+(\.\d+)?)?,(?<DateStamp>\d{6})\|(?<Custom>\S*)"))
                                    })),
                            @else: new ParseFromOutput(
                                parseFrom:
                                    new Flatten(
                                        (parsed) => ConvertHexToAscii(parsed["Body"]),
                                        new ParseFromLineWithCountAfterPosition(key: "Body", lineNumber: 1, startPosition: 26, charCount: -8)),
                                parsers: new List<IParse>
                                {
                                    new ParseFromMultiGroupRegex(pattern: new Regex(@"(?<TimeStamp>\d{6}\.\d{2}),(?<Validity>A|V),(?<Latitude>\d{4,5}\.\d{5}),(?<NorthSouth>N|S),(?<Longitude>\d{4,5}\.\d{5}),(?<EastWest>E|W),(?<SpeedInKnots>\d+\.\d+),(?<TrueCourse>\d+\.\d+)?,(?<DateStamp>\d{6})\|(?<Custom>\S*)"))
                                })),
                        new ParseFromLineWithCountAfterPosition(key: "CheckSum",lineNumber: 1, startPosition: -8, charCount: 4),
                        new ParseFromLineWithCountAfterPosition(key: "EOF",     lineNumber: 1, startPosition: -4, charCount: 4),
                    }
                }
            };

            var lines = new List<List<string>> { new List<string> { frame } };

            var parser = new Parser(p);
            var ds = parser.Parse(lines);

            Assert.AreEqual("2424", ds["Key"]["Header"]);
            Assert.AreEqual("0064", ds["Key"]["Length"]);
            Assert.AreEqual("62170017123456", ds["Key"]["IMEI"]);
            Assert.AreEqual("9999", ds["Key"]["Cmd"]);
            Assert.AreEqual("01", ds["Key"]["Event"]);

            Assert.AreEqual("152314", ds["Key"]["TimeStamp"]);
            Assert.AreEqual("A", ds["Key"]["Validity"]);
            Assert.AreEqual("1022.7963", ds["Key"]["Latitude"]);
            Assert.AreEqual("N", ds["Key"]["NorthSouth"]);
            Assert.AreEqual("07529.0151", ds["Key"]["Longitude"]);
            Assert.AreEqual("W", ds["Key"]["EastWest"]);
            Assert.AreEqual("000", ds["Key"]["SpeedInKnots"]);
            Assert.AreEqual("332", ds["Key"]["TrueCourse"]);
            Assert.AreEqual("190717", ds["Key"]["DateStamp"]);
            Assert.AreEqual("12.2|194|0B00|0000,0498|000000002", ds["Key"]["Custom"]);

            Assert.AreEqual("4BAA", ds["Key"]["CheckSum"]);
            Assert.AreEqual("0D0A", ds["Key"]["EOF"]);
        }
    }
}
