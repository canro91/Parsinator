# Parsinator

![](https://img.shields.io/badge/netstandard-2.0-brightgreen.svg) ![](https://github.com/canro91/Parsinator/workflows/Build/badge.svg) ![](https://img.shields.io/github/license/canro91/Parsinator)

Parsinator turns structured and unstructured text into a header-detail representation. You could use Parsinator to create an XML file from a pdf file or a C# object from a printer spool file. In general, you can use Parsinator to parse relevant data from any text.

## Why

Parsinator extracts relevant data from text files based on defined rules. It doesn't use any OCR technology or AI. Sorry!

You parse a text file by composing small functions to read or ignore text at the page or line level. Parsinator was heavily inspired by functional parsers combinators.

Read more about Parsinator motivation in [Parsinator, a tale of a pdf parser](https://canro91.github.io/2019/03/08/ATaleOfAPdfParser/).

## Usage

Parsinator uses three types of entities:

* Skipper: It removes chunks of text from the text to parse
* Parser: It captures text based on a pattern
* Transformation: It flattens lines spawning multiple pages

Parsinator provides a set of basic skippers, parsers, and transformation methods, but you can add your own entities. Find the list of the available entities in the [Wiki](https://github.com/canro91/Parsinator/wiki).

### Parse patterns

Parsinator finds text matching a regular expression in a given page or line.

```csharp
using Parsinator;

var lines = new List<List<string>>
{
    new List<string>
    {
        "Any text",
        "Name: Alice",
        "Any text",
        "Any text Address: Wonderland"
    }
};

var parsers = new Dictionary<string, IEnumerable<IParser>>
{
    {
        "PersonalData",
        new List<IParser>
        {
            new ParseFromLineWithRegex(key: "FullName", lineNumber: 2, pattern: new Regex("^Name: (\w+)$")),
            new ParseFromRegex(key: "Address", pattern: new Regex("Address: (\w+)$")
        }
    }
};
var parsinator = new Parser(parsers);
Dictionary<string, Dictionary<string, string>> parsed = parsinator.Parse(lines);

parsed.ContainsKey("PersonalData");
// true

parsed["PersonalData"]["FullName"];
// "Alice"

parsed["PersonalData"]["Address"];
// "Wonderland"
```

### Use a Fluent Interface

Alternatively, Parsinator has a fluent API to create skippers and parsers. Refer to the [Parsinator.Fluent](https://github.com/canro91/Parsinator/tree/master/Parsinator/Fluent) namespace for all supported skippers and parsers.

```csharp
using Parsinator.Fluent;

var parsers = new Dictionary<string, IEnumerable<IParser>>
{
    {
        "PersonalData",
        new List<IParser>
        {
            Parse.Key("FullName").FromLine(2).Regex(new Regex("^Name: (\w+)$")),
            Parse.Key("Address").Regex(new Regex("Address: (\w+)$")
        }
    }
};
```

### Create Xml

Parsinators relies on `DataSet` and `DataTable` to build XML files from parsed values. It provides extension methods to build tables and columns.

```csharp
var dataSet = new DataSet("Author")
    .WithTable(new DataTable("PersonalInfo")
        .WithColumn("Name"));

var lines = new List<List<string>>
{
    new List<string>
    {
        "Any text",
        "Name: Alice"
    }
};

var parser = Parse.Key("FullName").FromLine(2).Regex(new Regex("^Name: (\w+)$"));
var parsinator = new Parser(parser);
Dictionary<string, Dictionary<string, string>> parsed = parsinator.Parse(lines);

var xml = parsed.ToDataSet(dataSet).GetXml();
// "<Author><PersonalInfo Name="Alice" /></Author>"
```

Please, take a look at the [Sample project](https://github.com/canro91/Parsinator/tree/master/Parsinator.Sample) to see how to parse a plain-text invoice, a GPS frame, and an ebook table of content.

## Installation

Grab your own copy

## Contributing

Feel free to report any bug, ask for a new feature or just send a pull-request. All contributions are welcome.
	
## License

MIT
