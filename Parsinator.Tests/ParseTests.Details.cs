using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator.Tests
{
	public partial class ParseTests
	{
		[Test]
		public void Parse_Details_ApplyDetailsInEveryLine()
		{
			var p = new Dictionary<String, IList<IParse>>
			{
				{
					"Header",
					new List<IParse>
					{
						new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
					}
				}
			};
			var s = new List<ISkip>();
			var t = new FromSkipTransform(
							new SkipBeforeRegexAndAfterRegex(
								before: new Regex(@"-- Details --"),
								after: new Regex(@"-- End of Details --")));

			var d = new Dictionary<String, IList<IParse>>
			{
				{
					"Details",
					new List<IParse>
					{
						new FromRegex(key: "Code", pattern: new Regex(@"^(\d)\s*(\w+)\s*(\d+)$"), factory: (group) => group["1"]),
						new FromRegex(key: "Name", pattern: new Regex(@"^(\d)\s*(\w+)\s*(\d+)$"), factory: (group) => group["2"]),
						new FromRegex(key: "Value", pattern: new Regex(@"^(\d)\s*(\w+)\s*(\d+)$"), factory: (group) => group["3"])
					}
				}
			};

			var lines = FromText(@"
Value: 20
-- Details --
1 Item1 10
2 Item2 10
-- End of Details --");

			var parser = new Parser(p, s, t, d);
			var ds = parser.Parse(lines);

			Assert.AreEqual("20", ds["Header"]["Value"]);

			Assert.IsTrue(ds.ContainsKey("Details[1]"));
			Assert.AreEqual("Item1", ds["Details[1]"]["Name"]);

			Assert.IsTrue(ds.ContainsKey("Details[2]"));
			Assert.AreEqual("Item2", ds["Details[2]"]["Name"]);
		}

		[Test]
		public void Parse_Details_ApplyDetailsInEveryLine2()
		{
			var p = new Dictionary<String, IList<IParse>>
			{
				{
					"Header",
					new List<IParse>
					{
						new FromRegex(key: "Value", pattern: new Regex(@"Value:\s*(\d+)"))
					}
				}
			};
			var s = new List<ISkip>();
			var t = new FromSkipTransform(
							new SkipBeforeRegexAndAfterRegex(
								before: new Regex(@"-- Details --"),
								after: new Regex(@"-- End of Details --")));

			var d = new Dictionary<String, IList<IParse>>
			{
				{
					"Details",
					new List<IParse>
					{
						new FromGenerator<int>(key: "Code", seed: 0, next: (current) => current + 1),
						new FromRegex(key: "Name", pattern: new Regex(@"^(\d)\s*(\w+)\s*(\d+)$"), factory: (group) => group["2"]),
						new FromRegex(key: "Value", pattern: new Regex(@"^(\d)\s*(\w+)\s*(\d+)$"), factory: (group) => group["3"])
					}
				}
			};

			var lines = FromText(@"
Value: 20
-- Details --
Item1 10
Item2 10
-- End of Details --");

			var parser = new Parser(p, s, t, d);
			var ds = parser.Parse(lines);

			Assert.AreEqual("20", ds["Header"]["Value"]);

			Assert.IsTrue(ds.ContainsKey("Details[1]"));
			Assert.AreEqual("1", ds["Details[1]"]["Code"]);

			Assert.IsTrue(ds.ContainsKey("Details[2]"));
			Assert.AreEqual("2", ds["Details[2]"]["Code"]);
		}
	}
}
