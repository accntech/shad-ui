using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShadUI.Demo;

public static class CodeSnippetHelper
{
    private static readonly ConcurrentDictionary<string, string[]> FileLines =
        new(StringComparer.Ordinal);

    private static string[] GetLines(string filePath)
    {
        var fullPath = Path.GetFullPath(filePath);
        return FileLines.GetOrAdd(fullPath, File.ReadAllLines);
    }

    internal static void ClearCache() => FileLines.Clear();

    /// <summary>
    ///     Extracts lines between two line numbers (inclusive, 1-based).
    /// </summary>
    public static string ExtractByLineRange(this string filePath, int startLine, int endLine)
    {
        try
        {
            var lines = GetLines(filePath);
            return string.Join(Environment.NewLine,
                lines.Skip(startLine - 1).Take(endLine - startLine + 1));
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    /// <summary>
    ///     Extracts lines from a file while skipping specified ranges of lines.
    /// </summary>
    /// <param name="filePath">Path to the file to extract lines from.</param>
    /// <param name="skipRanges">Ranges of lines to skip (inclusive, 1-based).</param>
    /// <returns>Extracted lines with specified ranges skipped.</returns>
    public static string ExtractWithSkipRanges(this string filePath, params Range[] skipRanges)
    {
        try
        {
            var lines = GetLines(filePath);
            var result = new List<string>();

            for (var i = 0; i < lines.Length; i++)
            {
                var lineNumber = i + 1;
                var shouldSkip = skipRanges.Any(range => 
                {
                    var start = range.Start.IsFromEnd ? lines.Length - range.Start.Value : range.Start.Value;
                    var end = range.End.IsFromEnd ? lines.Length - range.End.Value : range.End.Value;
                    
                    var rangeStart = start + 1;
                    return lineNumber >= rangeStart && lineNumber <= end;
                });
                
                if (!shouldSkip)
                {
                    result.Add(lines[i]);
                }
            }

            return string.Join(Environment.NewLine, result);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    /// <summary>
    ///     Extracts lines from a file while skipping specified ranges of lines.
    /// </summary>
    /// <param name="filePath">Path to the file to extract lines from.</param>
    /// <param name="skipRanges">Ranges of lines to skip as tuples (start, end) - inclusive, 1-based.</param>
    /// <returns>Extracted lines with specified ranges skipped.</returns>
    public static string ExtractWithSkipRanges(this string filePath, params (int start, int end)[] skipRanges)
    {
        var ranges = skipRanges.Select(tuple => new Range(tuple.start - 1, tuple.end)).ToArray();
        return ExtractWithSkipRanges(filePath, ranges);
    }

    public static string CleanIndentation(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var minIndent = int.MaxValue;
        var lines = input.Replace("\r\n", "\n").Split('\n');
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var indent = 0;
            while (indent < line.Length && char.IsWhiteSpace(line[indent]))
            {
                indent++;
            }

            if (indent < line.Length && indent < minIndent)
            {
                minIndent = indent;
            }
        }

        if (minIndent == int.MaxValue)
        {
            minIndent = 0;
        }

        var stringBuilder = new StringBuilder(input.Length);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (i > 0)
            {
                stringBuilder.AppendLine();
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                stringBuilder.Append(line);
            }
            else
            {
                stringBuilder.Append(line.Length >= minIndent ? line[minIndent..] : line);
            }
        }

        return stringBuilder.ToString();
    }
}
