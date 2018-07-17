# Parsinator

Parsinator turns structured and unstructured text into a header-detail representation. You could use Parsinator to create an xml file from a pdf file, an object from a printer spool file or to parse relevant data from any text.

## Usage

Parsinator uses three type of entities:

* Skipper: It removes chunks of text from the text to parse
* Parser: It captures text based on a pattern
* Transformation: It reduces lines spawning multiples pages into a single stream of text.

Parsinator provides a set of basic skippers, parsers and transformation methods, but you can add your own entities.

### Single pattern

```csharp
var lines = new List<List<string>>
{
	new List<string>
	{
		"Any text",
		"Name: Alice"
	}
};

var parser = new Dictionary<string, IList<IParser>>
{
	{
		"PersonalData",
		new List<IParser>
		{
			new FromLineWithRegex(key: "FullName", lineNumber: 2, pattern: new Regex("^Name: (\w+)$"))
		}
	}
};
var parsinator = new Parser(parser);
Dictionary<string, Dictionary<string, string>> parsed = parsinator.Parse(lines);

Assert.IsTrue(parsed.ContainsKey("PersonalData"));
Assert.AreEqual("Alice", parsed["PersonalData"]["FullName"]);
```

### Multiple patterns

```csharp
var lines = new List<List<string>>
{
	new List<string>
	{
		"Any text",
		"Name: Alice",
		"Any text",
		"",
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
			new FromLineWithRegex(key: "FullName", lineNumber: 2, pattern: new Regex("^Name: (\w+)$")),
			new FromRegex(key: "Address", pattern: new Regex("Address: (\w+)$")
		}
	}
};

var parsinator = new Parser(parsers);
Dictionary<string, Dictionary<string, string>> parsed = parsinator.Parse(lines);

Assert.IsTrue(parsed.ContainsKey("PersonalData"));
var personalData = parsed["PersonalData"];
Assert.AreEqual("Alice", personalData["FullName"]);
Assert.AreEqual("Wonderland", personalData["Address"]);
```

### Xml

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

var parser = new FromLineWithRegex(key: "FullName", lineNumber: 2, pattern: new Regex("^Name: (\w+)$"));
var parsinator = new Parser(parser);
Dictionary<string, Dictionary<string, string>> parsed = parsinator.Parse(lines);

var xml = parsed.ToDataSet(dataSet).GetXml();

Assert.AreEqual("<Author><PersonalInfo Name="Alice" /></Author>", xml);
```
	
## License

MIT
