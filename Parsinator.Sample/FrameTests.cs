using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Parsinator.Sample
{
    [TestFixture]
    public class FrameTests
    {
        [Test]
        public void Parse_Frame_ParsesFrame()
        {
            var frame = "242400676315802812345699553131333035392E30302C412C313032342E37363537392C4E2C30373532382E30313237392C572C302E3030302C2C3235303131377C31322E327C3139347C303030307C303030302C303030307C3032383033303433395BF80D0A";

            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new FromLineWithCountAfterPosition(key: "Header",  lineNumber: 1, startPosition: 0, charCount: 4),
                        new FromLineWithCountAfterPosition(key: "Length",  lineNumber: 1, startPosition: 4, charCount: 4),
                        new FromLineWithCountAfterPosition(key: "IMEI",    lineNumber: 1, startPosition: 8, charCount: 14),
                        new FromLineWithCountAfterPosition(key: "Cmd",     lineNumber: 1, startPosition: 22, charCount: 4),
                        new FromLineWithCountAfterPosition(key: "Body",    lineNumber: 1, startPosition: 26, charCount: -8),
                        new FromLineWithCountAfterPosition(key: "CheckSum",lineNumber: 1, startPosition: -8, charCount: 4),
                        new FromLineWithCountAfterPosition(key: "EOF",     lineNumber: 1, startPosition: -4, charCount: 4),
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
    }
}
