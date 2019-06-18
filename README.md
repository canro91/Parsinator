# Parsinator

Parsinator turns structured and unstructured text into a header-detail representation. You could use Parsinator to create an xml file from a pdf file, an object from a printer spool file or to parse relevant data from any text.

## Why

Parsinator allows you to extract relevant information from any text-based file. You can parse a file by composing small functions to read or ignore text at the page ot line level. Parsinator was heavily inspired by functional parsers combinators. You can read more about Parsinator motivation [here](https://canro91.github.io/2019/03/08/ATaleOfAPdfParser/).

## Usage

Parsinator uses three type of entities:

* Skipper: It removes chunks of text from the text to parse
* Parser: It captures text based on a pattern
* Transformation: It reduces lines spawning multiples pages into a single stream of text.

Parsinator provides a set of basic skippers, parsers and transformation methods, but you can add your own entities.

### Parse patterns

```csharp
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

var parsers = new Dictionary<string, IList<IParser>>
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

Assert.IsTrue(parsed.ContainsKey("PersonalData"));
Assert.AreEqual("Alice", parsed["PersonalData"]["FullName"]);
Assert.AreEqual("Wonderland", parsed["PersonalData"]["Address"]);
```

### Create Xml

Parsinators relies on `DataSet` and `DataTable` to build xml files from parsed values. It provides extension methods to build tables and columns.

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

var parser = new ParseFromLineWithRegex(key: "FullName", lineNumber: 2, pattern: new Regex("^Name: (\w+)$"));
var parsinator = new Parser(parser);
Dictionary<string, Dictionary<string, string>> parsed = parsinator.Parse(lines);

var xml = parsed.ToDataSet(dataSet).GetXml();

Assert.AreEqual("<Author><PersonalInfo Name="Alice" /></Author>", xml);
```

## Installation

Grab your own copy

## Contributing

Feel free to report any bug, ask for a new feature or just send a pull-request. All contributions are welcome.
	
## License

MIT
