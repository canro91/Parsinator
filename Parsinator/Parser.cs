using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Parsinator
{
    public class Parser
    {
        private readonly Dictionary<String, Dictionary<String, String>> _output;
        private readonly IDictionary<String, IList<IParse>> _headerParsers;
        private readonly IList<ISkip> _headerSkipers;
        private readonly ITransform _transform;
        private readonly IDictionary<String, IList<IParse>> _detailParsers;

        public Parser(IDictionary<String, IList<IParse>> headerParsers, IList<ISkip> headerSkipers, ITransform transform, IDictionary<String, IList<IParse>> detailParse)
        {
            _headerParsers = headerParsers;
            _headerSkipers = headerSkipers;
            _transform = transform;
            _detailParsers = detailParse;
            _output = new Dictionary<string, Dictionary<string, string>>();
        }

        public Parser(IDictionary<string, IList<IParse>> headerParsers, IList<ISkip> headerSkipers)
            : this(headerParsers, headerSkipers, null, null)
        {
        }

        public Parser(IDictionary<string, IList<IParse>> headerParsers)
            : this(headerParsers, new List<ISkip>(), null, null)
        {
        }

        public Dictionary<string, Dictionary<string, string>> Parse(List<List<String>> lines)
        {
            List<List<String>> pages = _headerSkipers.Chain(lines);

            // WARNING: To find values from header, the parsers are only applied
            // on the first page, if a page number isn't specified
            foreach (var page in pages.Select((Content, Number) => new { Number, Content }))
            {
                var parsers = FindPasersForPage(_headerParsers, page.Number, lines.Count);
                if (parsers.Any())
                    ParseOnceInPage(parsers, page.Content);
            }

            if (_detailParsers != null && _detailParsers.Any())
            {
                // WARNING: Remove from the equation wrapping lines from page to the next
                List<String> details = (_transform != null)
                        ? _transform.Transform(pages)
                        : pages.SelectMany(t => t).ToList();

                ParseInEveryLine(_detailParsers, details);
            }

            return _output;
        }

        private IDictionary<string, IEnumerable<IParse>> FindPasersForPage(IDictionary<string, IList<IParse>> parsers, int pageIndex, int totalPages)
        {
            bool isInPage(IParse t)
                => (pageIndex == 0)
                    ? !t.PageNumber.HasValue || t.PageNumber == 1 || (t.PageNumber == -1 && totalPages == 1)
                    : t.PageNumber == pageIndex + 1 || t.PageNumber == pageIndex - totalPages;

            var hasParsers = parsers.Any(t => t.Value.Any(isInPage));
            return hasParsers
                    ? parsers.ToDictionary(k => k.Key, v => v.Value.Where(isInPage))
                    : new Dictionary<string, IEnumerable<IParse>>();
        }

        private void ParseOnceInPage(IDictionary<string, IEnumerable<IParse>> toParse, List<string> page)
        {
            foreach (var item in toParse)
            {
                var sectionName = item.Key;
                var parsers = item.Value;

                var row = new Dictionary<string, string>();
                foreach (var line in page.Select((Content, Number) => new { Number, Content }))
                {
                    if (!parsers.Any(t => !t.HasMatched))
                        break;

                    // TODO Call only once a parser if it has line number
                    foreach (var parser in parsers.Where(t => !t.HasMatched))
                    {
                        var result = parser.Parse(line.Content, line.Number + 1, line.Number - page.Count);
                        if (parser.HasMatched)
                        {
                            row.Merge(result);
                        }
                    }
                }

                foreach (var parser in parsers.Where(t => !t.HasMatched).Where(t => t.Default != null))
                {
                    row[parser.Key] = parser.Default();
                }
                _output.AddOrMerge(sectionName, row);
            }
        }

        private void ParseInEveryLine(IDictionary<string, IList<IParse>> toParse, List<string> page)
        {
            foreach (var item in toParse)
            {
                var sectionName = item.Key;
                var parsers = item.Value;

                foreach (var line in page.Select((Content, Number) => new { Number, Content }))
                {
                    var row = new Dictionary<string, string>();

                    foreach (var parser in parsers)
                    {
                        var result = parser.Parse(line.Content, line.Number + 1, line.Number - page.Count);
                        if (parser.HasMatched)
                        {
                            row.Merge(result);
                        }
                        else if (parser.Default != null)
                        {
                            row[parser.Key] = parser.Default();
                        }
                    }
                    _output[$"{sectionName}[{line.Number + 1}]"] = row;
                }
            }
        }
    }
}
