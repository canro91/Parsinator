using NUnit.Framework;
using Parsinator.Fluent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator.Sample
{
    [TestFixture]
    public class BookTests
    {
        // Excerpt from Alice in Wonderland
        // Source: https://www.gutenberg.org/files/11/11-0.txt
        private readonly string InputFile = @"
Title: Alice’s Adventures in Wonderland

Author: Lewis Carroll

Release Date: June 25, 2008 [EBook #11]
Last Updated: February 22, 2020

Language: English

Character set encoding: UTF-8

*** START OF THIS PROJECT GUTENBERG EBOOK ALICE’S ADVENTURES IN WONDERLAND ***



Produced by Arthur DiBianca and David Widger

[Illustration]




Alice’s Adventures in Wonderland

by Lewis Carroll

THE MILLENNIUM FULCRUM EDITION 3.0

Contents

 CHAPTER I.     Down the Rabbit-Hole
 CHAPTER II.    The Pool of Tears
 CHAPTER III.   A Caucus-Race and a Long Tale
 CHAPTER IV.    The Rabbit Sends in a Little Bill
 CHAPTER V.     Advice from a Caterpillar
 CHAPTER VI.    Pig and Pepper
 CHAPTER VII.   A Mad Tea-Party
 CHAPTER VIII.  The Queen’s Croquet-Ground
 CHAPTER IX.    The Mock Turtle’s Story
 CHAPTER X.     The Lobster Quadrille
 CHAPTER XI.    Who Stole the Tarts?
 CHAPTER XII.   Alice’s Evidence";

        private readonly string ExpectedXml = @"<Book>
  <GeneralInfo>
    <Title>Alice’s Adventures in Wonderland</Title>
    <Author>Lewis Carroll</Author>
    <Language>English</Language>
  </GeneralInfo>
  <Chapters>
    <Chapter Number=""I"" Title=""Down the Rabbit-Hole"" />
    <Chapter Number=""II"" Title=""The Pool of Tears"" />
    <Chapter Number=""III"" Title=""A Caucus-Race and a Long Tale"" />
    <Chapter Number=""IV"" Title=""The Rabbit Sends in a Little Bill"" />
    <Chapter Number=""V"" Title=""Advice from a Caterpillar"" />
    <Chapter Number=""VI"" Title=""Pig and Pepper"" />
    <Chapter Number=""VII"" Title=""A Mad Tea-Party"" />
    <Chapter Number=""VIII"" Title=""The Queen’s Croquet-Ground"" />
    <Chapter Number=""IX"" Title=""The Mock Turtle’s Story"" />
    <Chapter Number=""X"" Title=""The Lobster Quadrille"" />
    <Chapter Number=""XI"" Title=""Who Stole the Tarts?"" />
    <Chapter Number=""XII"" Title=""Alice’s Evidence"" />
  </Chapters>
</Book>";

        [Test]
        public void Parse_InputFile_ParsesFile()
        {
            var ds = new DataSet("Book")
            .WithTable(new DataTable("GeneralInfo")
                .WithColumn("Title", MappingType.Element)
                .WithColumn("Author", MappingType.Element)
                .WithColumn("Language", MappingType.Element))
            .WithTable(new DataTable("Chapters")
                .Empty())
            .WithTable(new DataTable("Chapter")
                .WithColumn("Number")
                .WithColumn("Title"))
            .WithRelation("Chapters", "Chapter");

            var s = new List<ISkip>
            {
                Skip.BlankLines
            };

            var p = new Dictionary<String, IList<IParse>>
            {
                {
                    "GeneralInfo",
                    new List<IParse>
                    {
                        Parse.Key("Title").Regex(new Regex(@"Title:\s*(.+)")),
                        Parse.Key("Author").FromLine(2).Regex(new Regex(@"Author:\s*(.+)")),
                        Parse.Key("Language").FromLine(5).Regex(new Regex(@"Language:\s*(.+)"))
                    }
                }
            };

            var transformation = new TransformFromSingleSkip(
                Skip.IfDoesNotMatch(new Regex(@"^(\s*)CHAPTER(.*)")));

            var d = new Dictionary<String, IList<IParse>>
            {
                {
                    "Chapter",
                    new List<IParse>
                    {
                        Parse.Key("Number").Regex(new Regex(@"CHAPTER\s*([A-Z]+)\.")),
                        Parse.Key("Title").Regex(new Regex(@"CHAPTER\s*.*\.\s*(.+)$")),
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