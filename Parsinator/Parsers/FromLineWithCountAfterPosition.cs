using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class FromLineWithCountAfterPosition : IParse
    {
        private readonly int LineNumber;
        private readonly int StartPosition;
        private readonly int CharCount;

        public FromLineWithCountAfterPosition(string key, int lineNumber, int startPosition, int charCount)
        {
            this.Key = key;
            this.LineNumber = lineNumber;
            this.StartPosition = startPosition;
            this.CharCount = charCount;
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public KeyValuePair<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            if (lineNumber == this.LineNumber || (this.LineNumber < 0 && lineNumberFromBottom == this.LineNumber))
            {
                var isInLine = StartPosition + CharCount <= line.Length;
                if (isInLine)
                {
                    HasMatched = true;
                    var value = line.Substring(StartPosition, CharCount);
                    return new KeyValuePair<string, string>(Key, value.Trim());
                }
            }
            return new KeyValuePair<string, string>();
        }
    }
}
