using System;
using System.Collections.Generic;

namespace ShadUI.Demo.Controls;

internal enum BrowserSyntaxKind
{
    Plain,
    Keyword,
    Type,
    Attribute,
    String,
    Comment,
    Number,
    Punctuation
}

internal readonly record struct BrowserSyntaxToken(string Text, BrowserSyntaxKind Kind);

/// <summary>
/// Small, allocation-conscious syntax tokenizer for browser demo snippets.
/// It deliberately supports only the XAML/XML and C# constructs used by the catalog.
/// </summary>
internal static class BrowserSyntaxHighlighter
{
    public static IReadOnlyList<BrowserSyntaxToken> Tokenize(string? text, string? language)
    {
        if (string.IsNullOrEmpty(text)) return Array.Empty<BrowserSyntaxToken>();

        return language?.ToLowerInvariant() switch
        {
            "cs" or "csharp" => TokenizeCSharp(text),
            "xaml" or "xml" => TokenizeXaml(text),
            _ => [new BrowserSyntaxToken(text, BrowserSyntaxKind.Plain)]
        };
    }

    private static IReadOnlyList<BrowserSyntaxToken> TokenizeXaml(string text)
    {
        var tokens = new List<BrowserSyntaxToken>();
        var index = 0;
        var inTag = false;
        var expectTagName = false;

        while (index < text.Length)
        {
            if (!inTag)
            {
                if (StartsWith(text, index, "<!--"))
                {
                    var end = text.IndexOf("-->", index + 4, StringComparison.Ordinal);
                    end = end < 0 ? text.Length : Math.Min(text.Length, end + 3);
                    AddToken(tokens, text, index, end - index, BrowserSyntaxKind.Comment);
                    index = end;
                    continue;
                }

                if (text[index] == '<')
                {
                    var length = index + 1 < text.Length && text[index + 1] is '/' or '?' or '!' ? 2 : 1;
                    AddToken(tokens, text, index, length, BrowserSyntaxKind.Punctuation);
                    index += length;
                    inTag = true;
                    expectTagName = true;
                    continue;
                }

                var start = index;
                while (index < text.Length && text[index] != '<') index++;
                AddToken(tokens, text, start, index - start, BrowserSyntaxKind.Plain);
                continue;
            }

            if (char.IsWhiteSpace(text[index]))
            {
                var start = index;
                while (index < text.Length && char.IsWhiteSpace(text[index])) index++;
                AddToken(tokens, text, start, index - start, BrowserSyntaxKind.Plain);
                continue;
            }

            if (StartsWith(text, index, "/>" ) || StartsWith(text, index, "?>"))
            {
                AddToken(tokens, text, index, 2, BrowserSyntaxKind.Punctuation);
                index += 2;
                inTag = false;
                expectTagName = false;
                continue;
            }

            if (text[index] == '>')
            {
                AddToken(tokens, text, index, 1, BrowserSyntaxKind.Punctuation);
                index++;
                inTag = false;
                expectTagName = false;
                continue;
            }

            if (text[index] is '\'' or '"')
            {
                var quote = text[index];
                var start = index++;
                while (index < text.Length && text[index] != quote) index++;
                if (index < text.Length) index++;
                AddToken(tokens, text, start, index - start, BrowserSyntaxKind.String);
                continue;
            }

            if (text[index] == '=')
            {
                AddToken(tokens, text, index++, 1, BrowserSyntaxKind.Punctuation);
                continue;
            }

            if (IsXamlNameCharacter(text[index]))
            {
                var start = index;
                while (index < text.Length && IsXamlNameCharacter(text[index])) index++;
                AddToken(tokens, text, start, index - start,
                    expectTagName ? BrowserSyntaxKind.Type : BrowserSyntaxKind.Attribute);
                expectTagName = false;
                continue;
            }

            AddToken(tokens, text, index++, 1, BrowserSyntaxKind.Punctuation);
        }

        return tokens;
    }

    private static IReadOnlyList<BrowserSyntaxToken> TokenizeCSharp(string text)
    {
        var tokens = new List<BrowserSyntaxToken>();
        var index = 0;

        while (index < text.Length)
        {
            if (char.IsWhiteSpace(text[index]))
            {
                var start = index;
                while (index < text.Length && char.IsWhiteSpace(text[index])) index++;
                AddToken(tokens, text, start, index - start, BrowserSyntaxKind.Plain);
                continue;
            }

            if (StartsWith(text, index, "//"))
            {
                var start = index;
                index += 2;
                while (index < text.Length && text[index] != '\n') index++;
                AddToken(tokens, text, start, index - start, BrowserSyntaxKind.Comment);
                continue;
            }

            if (StartsWith(text, index, "/*"))
            {
                var start = index;
                var end = text.IndexOf("*/", index + 2, StringComparison.Ordinal);
                index = end < 0 ? text.Length : Math.Min(text.Length, end + 2);
                AddToken(tokens, text, start, index - start, BrowserSyntaxKind.Comment);
                continue;
            }

            if (TryReadCSharpString(text, index, out var stringEnd))
            {
                AddToken(tokens, text, index, stringEnd - index, BrowserSyntaxKind.String);
                index = stringEnd;
                continue;
            }

            if (text[index] == '#')
            {
                var start = index;
                while (index < text.Length && text[index] != '\n') index++;
                AddToken(tokens, text, start, index - start, BrowserSyntaxKind.Keyword);
                continue;
            }

            if (char.IsDigit(text[index]))
            {
                var start = index;
                while (index < text.Length &&
                       (char.IsLetterOrDigit(text[index]) || text[index] is '.' or '_' or '+' or '-')) index++;
                AddToken(tokens, text, start, index - start, BrowserSyntaxKind.Number);
                continue;
            }

            if (IsIdentifierStart(text[index]) || text[index] == '@' && index + 1 < text.Length && IsIdentifierStart(text[index + 1]))
            {
                var start = index;
                if (text[index] == '@') index++;
                while (index < text.Length && IsIdentifierPart(text[index])) index++;

                var identifierStart = text[start] == '@' ? start + 1 : start;
                var identifier = text.AsSpan(identifierStart, index - identifierStart);
                var kind = IsCSharpKeyword(identifier)
                    ? BrowserSyntaxKind.Keyword
                    : char.IsUpper(identifier[0])
                        ? BrowserSyntaxKind.Type
                        : BrowserSyntaxKind.Plain;
                AddToken(tokens, text, start, index - start, kind);
                continue;
            }

            AddToken(tokens, text, index++, 1, BrowserSyntaxKind.Punctuation);
        }

        return tokens;
    }

    private static bool TryReadCSharpString(string text, int start, out int end)
    {
        end = start;
        var index = start;
        var verbatim = false;

        if (StartsWith(text, index, "$@\"") || StartsWith(text, index, "@$\""))
        {
            verbatim = true;
            index += 2;
        }
        else if (StartsWith(text, index, "@\""))
        {
            verbatim = true;
            index++;
        }
        else if (StartsWith(text, index, "$\""))
        {
            index++;
        }

        if (index >= text.Length || text[index] is not ('"' or '\'')) return false;

        var quote = text[index++];
        while (index < text.Length)
        {
            if (verbatim && text[index] == quote && index + 1 < text.Length && text[index + 1] == quote)
            {
                index += 2;
                continue;
            }

            if (!verbatim && text[index] == '\\')
            {
                index = Math.Min(text.Length, index + 2);
                continue;
            }

            if (text[index++] == quote) break;
        }

        end = index;
        return true;
    }

    private static void AddToken(
        List<BrowserSyntaxToken> tokens,
        string source,
        int start,
        int length,
        BrowserSyntaxKind kind)
    {
        if (length <= 0) return;

        var value = source.Substring(start, length);
        if (tokens.Count > 0 && tokens[^1].Kind == kind)
        {
            var previous = tokens[^1];
            tokens[^1] = new BrowserSyntaxToken(previous.Text + value, kind);
            return;
        }

        tokens.Add(new BrowserSyntaxToken(value, kind));
    }

    private static bool StartsWith(string text, int index, string value)
    {
        return index + value.Length <= text.Length &&
               text.AsSpan(index, value.Length).SequenceEqual(value.AsSpan());
    }

    private static bool IsXamlNameCharacter(char value)
    {
        return char.IsLetterOrDigit(value) || value is '_' or ':' or '.' or '-';
    }

    private static bool IsIdentifierStart(char value)
    {
        return char.IsLetter(value) || value == '_';
    }

    private static bool IsIdentifierPart(char value)
    {
        return char.IsLetterOrDigit(value) || value == '_';
    }

    private static bool IsCSharpKeyword(ReadOnlySpan<char> value)
    {
        return value is
            "abstract" or "as" or "base" or "bool" or "break" or "byte" or "case" or "catch" or
            "char" or "checked" or "class" or "const" or "continue" or "decimal" or "default" or
            "delegate" or "do" or "double" or "else" or "enum" or "event" or "explicit" or
            "extern" or "false" or "finally" or "fixed" or "float" or "for" or "foreach" or
            "goto" or "if" or "implicit" or "in" or "int" or "interface" or "internal" or "is" or
            "lock" or "long" or "namespace" or "new" or "null" or "object" or "operator" or "out" or
            "override" or "params" or "private" or "protected" or "public" or "readonly" or "ref" or
            "return" or "sbyte" or "sealed" or "short" or "sizeof" or "stackalloc" or "static" or
            "string" or "struct" or "switch" or "this" or "throw" or "true" or "try" or "typeof" or
            "uint" or "ulong" or "unchecked" or "unsafe" or "ushort" or "using" or "virtual" or
            "void" or "volatile" or "while" or "async" or "await" or "get" or "set" or "init" or
            "record" or "required" or "var" or "when" or "where" or "yield";
    }
}
