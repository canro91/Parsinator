﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator.Sample
{
    [TestFixture]
    public class ParseTest
    {
        private readonly String InputFile = @"
+-------------------------------------------------------------------------------------------------------------------------------------+
|                                                             STARK INDUSTRIES                                                        |
+-------------------------------------------------------------------------------------------------------------------------------------+
| Customer  : James Rhodey Rhodes                                   Number    : 1000                                                  |
| Contact   : Pepper Potts                                          Date      : 2015-04-15                                            |
| Address   : 1-23 Main St.                                         Seller    : Tony Starks                                           |
| City      : Los Angeles                                                                                                             |
+-------------------------------------------------------------------------------------------------------------------------------------+
| Code                D e s c r i p t i o n                             Quantity         Unit Price                      Total        |
+-------------------------------------------------------------------------------------------------------------------------------------+
| WAR-MACHINE-2       IRON PATRIOT                                   	1                1,000,000.00                    1,000,000.00 |
+-----------------------------------------------------------------------------------------------------------+-------------------------+
|                                                                                                           |             T O T A L   |
|                                                                                                           |          1,000,000.00   |
+-----------------------------------------------------------------------------------------------------------+-------------------------+";

        private readonly String ExpectedXml = @"<Invoice>
  <Billing Number=""1000"" Date=""2015-04-15"" Total=""1,000,000.00"" />
  <Customer Name=""James Rhodey Rhodes"">
    <Address AddressLine=""1-23 Main St."" City=""Los Angeles"" />
  </Customer>
  <Supplier Contact=""Pepper Potts"" Seller=""Tony Starks"" />
  <Products>
    <Product Code=""WAR-MACHINE-2"" Description=""IRON PATRIOT"" Quantity=""1"" UnitPrice=""1,000,000.00"" Total=""1,000,000.00"" />
  </Products>
</Invoice>";

        [Test]
        public void Parse_InputFile_ParsesFile()
        {
            var ds = new DataSet("Invoice")
                        .WithTable(new DataTable("Billing")
                            .WithColumn("Number")
                            .WithColumn("Date")
                            .WithColumn("Total"))
                        .WithTable(new DataTable("Customer")
                            .WithColumn("Name"))
                        .WithTable(new DataTable("Address")
                            .WithColumn("AddressLine")
                            .WithColumn("City"))
                        .WithRelation("Customer", "Address")
                        .WithTable(new DataTable("Supplier")
                            .WithColumn("Contact")
                            .WithColumn("Seller"))
                        .WithTable(new DataTable("Products")
                            .Empty())
                        .WithTable(new DataTable("Product")
                            .WithColumn("Code")
                            .WithColumn("Description")
                            .WithColumn("Quantity")
                            .WithColumn("UnitPrice")
                            .WithColumn("Total"))
                        .WithRelation("Products", "Product");

            var s = new List<ISkip>
            {
                new SkipIfMatches(new Regex(@"(\+([\-\+])+\+)"))
            };

            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "Billing",
                    new List<IParse>
                    {
                        new FromLineNumberWithRegex(key: "Number", lineNumber: 2, pattern: new Regex(@"Number\s+:\s+(\d+)")),
                        new FromLineNumberWithRegex(key: "Date", lineNumber: 3, pattern: new Regex(@"Date\s+:\s+(\d{1,4}\-\d{1,2}\-\d{1,2})")),
                        new FromLineNumberWithRegex(key: "Total", lineNumber: -1, pattern: new Regex(@"\|\s+([\d\,\.]+)\s+\|")),
                    }
                },
                {
                    "Customer",
                    new List<IParse>
                    {
                        new FromLineNumberWithRegex(key: "Name", lineNumber: 2, pattern: new Regex(@"\|\s+Customer\s+:\s+([\w\s]+)Number"))
                    }
                },
                {
                    "Address",
                    new List<IParse>
                    {
                        new FromLineNumberWithRegex(key: "AddressLine", lineNumber: 4, pattern: new Regex(@"\|\s+Address\s+:([\w\s\-_\.]+)Seller")),
                        new FromLineNumberWithRegex(key: "City", lineNumber: 5, pattern: new Regex(@"\|\s+City\s+:([\w\s\-_\.]+)\|")),
                    }
                },
                {
                    "Supplier",
                    new List<IParse>
                    {
                        new FromLineNumberWithRegex(key: "Contact", lineNumber: 3, pattern: new Regex(@"\|\s+Contact\s+:([\w\s\-_\.]+)Date")),
                        new FromLineNumberWithRegex(key: "Seller", lineNumber: 4, pattern: new Regex(@"Seller\s+:([\w\s\-_\.]+)\|")),
                    }
                }
            };

            var transformation = new FromSkipTransform(
                new SkipBeforeRegexAndAfterRegex(
                    before: new Regex(@"\|\s+Code\s+.+Total\s+\|"),
                    after: new Regex(@"\|\s+\|\s+T O T A L\s+\|")));

            var detailRegex = new Regex(@"\|\s+(?<Code>[\w\-]+)\s+(?<Description>[\w\s]{1,48})\s+(?<Quantity>\d+)\s+(?<UnitPrice>[\d\,\.]+)\s+(?<Total>[\d\,\.]+)\s+\|");
            var d = new Dictionary<String, IList<IParse>>
            {
                {
                    "Product",
                    new List<IParse>
                    {
                        new FromRegex(key: "Code", pattern: detailRegex, factory: (g) => g["Code"].Value),
                        new FromRegex(key: "Description", pattern: detailRegex, factory: (g) => g["Description"].Value),
                        new FromRegex(key: "Quantity", pattern: detailRegex, factory: (g) => g["Quantity"].Value),
                        new FromRegex(key: "UnitPrice", pattern: detailRegex, factory: (g) => g["UnitPrice"].Value),
                        new FromRegex(key: "Total", pattern: detailRegex, factory: (g) => g["Total"].Value),
                    }
                }
            };

            var lines = FromText(InputFile);

            var parser = new Parser(p, s, transformation, d);
            var parsed = parser.Parse(lines);

            var xml = parsed.ToDataSet(ds).GetXml();

            Assert.AreEqual(ExpectedXml, xml);
        }

        private List<List<String>> FromText(String str)
        {
            return new List<List<string>> { str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToList() };
        }
    }
}
