using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace Parsinator.Tests
{
    public partial class ParseTests
    {
        [Test]
        public void CreateDataset_MissingTableInRelation_ThrowException()
        {
            var dataSetName = "DataSet";
            var missingTableName = "MissingTableName";

            var e = Assert.Throws<ArgumentNullException>(() =>
            {
                new DataSet(dataSetName)
                    .WithTable(new DataTable("Table"))
                    // Missing table creation
                    .WithRelation("Table", missingTableName);
            });
            StringAssert.Contains(dataSetName, e.Message);
            StringAssert.Contains(missingTableName, e.Message);
        }

        [Test]
        public void Parse_MultipleParsers_BuildsXmlWithParsedValues()
        {
            var ds = new DataSet("Person")
                        .WithTable(new DataTable("FullName")
                            .WithColumn("Name")
                            .WithColumn("LastName"))
                        .WithTable(new DataTable("Address")
                            .WithColumn("StreetName")
                            .WithColumn("City"));

            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "FullName",
                    new List<IParse>
                    {
                        new ParseFromLineNumberWithRegex(key: "Name", lineNumber: 1, pattern: new Regex(@"Name:\s*(\w+);")),
                        new ParseFromLineNumberWithRegex(key: "LastName", lineNumber: 1, pattern: new Regex(@"Last name:\s*(\w+)")),
                    }
                },
                {
                    "Address",
                    new List<IParse>
                    {
                        new ParseFromLineNumberWithRegex(key: "StreetName", lineNumber: 2, pattern: new Regex(@"Street name:\s*(\w+);")),
                        new ParseFromLineNumberWithRegex(key: "City", lineNumber: 2, pattern: new Regex(@"City:\s*(\w+)")),
                    }
                }
            };
            var lines = FromText(@"
Name: John; Last name: Doe
Street name: Main; City: Wonderland");

            var parser = new Parser(p);
            var parsed = parser.Parse(lines);

            var xml = parsed.ToDataSet(ds).GetXml();

            Assert.AreEqual(@"<Person>
  <FullName Name=""John"" LastName=""Doe"" />
  <Address StreetName=""Main"" City=""Wonderland"" />
</Person>", xml);
        }

        [Test]
        public void Parse_NotExistingTable_BuildsXml()
        {
            var ds = new DataSet("DataSetName")
                        .WithTable(new DataTable("NotExistingTableName")
                            .WithColumn("ColumnName"));

            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Key",
                    new List<IParse>
                    {
                        new ParseFromValue(key: "Name", value: "Any value")
                    }
                }
            };
            var lines = FromText(@"
Anything");

            var parser = new Parser(p);
            var parsed = parser.Parse(lines);

            var e = Assert.Throws<ArgumentNullException>(() => parsed.ToDataSet(ds).GetXml());
            StringAssert.Contains("DataSetName", e.Message);
            StringAssert.Contains("Key", e.Message);
        }
    }
}
