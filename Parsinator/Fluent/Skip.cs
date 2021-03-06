﻿using System.Text.RegularExpressions;

namespace Parsinator.Fluent
{
    public static class Skip
    {
        public static SkipBeforeRegexBuilder BeforeRegex(Regex before)
        {
            var builder = new SkipBeforeRegexBuilder
            {
                Before = before
            };
            return builder;
        }

        public static ISkip BlankLines
            => new SkipBlankLines();

        public static ISkip FromFirstMatchOfRegex(Regex regex)
            => new SkipFromFirstMatchOfRegex(regex);

        public static ISkip IfMatches(Regex regex)
            => new SkipIfMatches(regex);

        public static ISkip IfDoesNotMatch(Regex regex)
            => new SkipIfDoesNotMatch(regex);

        public static ISkip LinesFromStart(int lineCount)
            => new SkipLineCountFromStart(lineCount);

        public static ISkip OneLineFromStart
            => new SkipLineCountFromStart();

        public static ISkip LinesFromEnd(int lineCount)
            => new SkipLineCountFromEnd(lineCount);

        public static ISkip OneLineFromEnd
            => new SkipLineCountFromEnd();

        public static SkipLineCountBuilder Lines(int lineCount)
        {
            var builder = new SkipLineCountBuilder
            {
                LineCount = lineCount
            };
            return builder;
        }

        public static SkipFromFirstRegexBuilder FromFirstRegex(Regex firstRegex)
        {
            var builder = new SkipFromFirstRegexBuilder
            {
                FirstRegex = firstRegex
            };
            return builder;
        }
    }

    public class SkipBeforeRegexBuilder
    {
        public Regex Before { get; internal set; }

        public SkipBeforeRegexAndAfterRegex AndAfterRegex(Regex after)
        {
            return new SkipBeforeRegexAndAfterRegex(Before, after);
        }
    }

    public class SkipLineCountBuilder
    {
        public int LineCount { get; internal set; }

        public SkipLineCountFromLineNumber StartingFromLine(int lineNumber)
        {
            return new SkipLineCountFromLineNumber(lineNumber, LineCount);
        }
    }

    public class SkipFromFirstRegexBuilder
    {
        public Regex FirstRegex { get; internal set; }

        public ISkip ToLastRegex(Regex lastRegex)
        {
            return new SkipFromFirstRegexToLastRegex(FirstRegex, lastRegex);
        }
    }
}