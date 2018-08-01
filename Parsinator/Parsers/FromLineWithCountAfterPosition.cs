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
                bool isInLine = false;
                if (StartPosition < 0)
                    isInLine = isInLine || line.Length + StartPosition >= 0;
                if (CharCount < 0)
                    isInLine = isInLine || StartPosition + (line.Length + CharCount) <= line.Length;
                if (StartPosition >= 0 && CharCount > 0)
                    isInLine = StartPosition + CharCount <= line.Length;

                if (isInLine)
                {
                    HasMatched = true;

                    var startIndex = (StartPosition < 0) ? line.Length + StartPosition : StartPosition;
                    var length = (CharCount < 0) ? line.Length + CharCount - startIndex : CharCount;

                    var value = line.Substring(startIndex, length);
                    return new KeyValuePair<string, string>(Key, value.Trim());
                }
            }
            return new KeyValuePair<string, string>();
        }
    }
}
