using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ReactiveShowcase.Examples;

/// <summary>
/// Reads the example files embedded into the assembly and turns them into highlighted rich text.
/// </summary>
internal static class ExampleSource {
    // Matches the LogicalName set for the embedded samples in the project file
    private const string ResourcePrefix = "Examples/";

    private static readonly Dictionary<string, string> cache = new();

    public static string Load(string file) {
        if (cache.TryGetValue(file, out var cached)) {
            return cached;
        }

        var assembly = typeof(ExampleSource).Assembly;
        using var stream = assembly.GetManifestResourceStream(ResourcePrefix + file);

        var text = stream == null ? $"// {file} is not embedded" : new StreamReader(stream).ReadToEnd();
        text = text.Replace("\r\n", "\n").Replace("\t", "    ");
        text = StripMetadata(text);
        text = StripPreamble(text);

        cache[file] = text;
        return text;
    }

    #region Preamble

    private static readonly Regex preambleLine = new(@"^(using [^\n]*;|namespace [\w.]+;|\s*)$");

    private static string StripPreamble(string code) {
        var lines = code.Split('\n');
        var first = 0;

        while (first < lines.Length && preambleLine.IsMatch(lines[first])) {
            first++;
        }

        return string.Join("\n", lines, first, lines.Length - first);
    }

    #endregion

    #region Metadata

    private static readonly Regex metadataClass = new(@"^[ \t]*(internal |public )?(sealed )?class \w+ : Example \{", RegexOptions.Multiline);

    private static string StripMetadata(string code) {
        var match = metadataClass.Match(code);
        if (!match.Success) {
            return code;
        }

        var end = FindClosingBrace(code, match.Index + match.Length - 1);
        if (end < 0) {
            return code;
        }

        var after = end + 1;
        while (after < code.Length && code[after] is '\n' or ' ') {
            after++;
        }

        return code.Substring(0, match.Index) + code.Substring(after);
    }

    /// <summary>
    /// Finds the brace closing the one at the given index, skipping strings, chars and comments.
    /// </summary>
    private static int FindClosingBrace(string code, int open) {
        var depth = 0;

        for (var i = open; i < code.Length; i++) {
            var c = code[i];

            if (c == '/' && i + 1 < code.Length && code[i + 1] == '/') {
                i = code.IndexOf('\n', i);
                if (i < 0) return -1;
                continue;
            }

            if (c is '"' or '\'') {
                for (i++; i < code.Length && code[i] != c; i++) {
                    if (code[i] == '\\') i++;
                }
                continue;
            }

            if (c == '{') {
                depth++;
            } else if (c == '}' && --depth == 0) {
                return i;
            }
        }

        return -1;
    }

    #endregion

    #region Highlighting

    private static readonly HashSet<string> keywords = [
        "using", "namespace", "class", "struct", "record", "enum", "interface", "internal", "public", "private",
        "protected", "static", "readonly", "override", "abstract", "virtual", "sealed", "partial", "new", "return",
        "var", "if", "else", "for", "foreach", "in", "while", "switch", "case", "default", "break", "continue",
        "true", "false", "null", "this", "base", "void", "bool", "int", "float", "string", "get", "set", "init",
        "with", "is", "not", "and", "or", "out", "ref", "const", "params", "typeof", "nameof", "yield", "lazy"
    ];

    // Order matters: comments and strings first, so keywords inside them aren't colored
    private static readonly Regex tokens = new(
        @"(?<comment>//[^\n]*)|(?<attr>\[[A-Z]\w*(\([^\)\n]*\))?\])|(?<string>\$?""(\\.|[^""\\\n])*"")|(?<char>'(\\.|[^'\\\n])')|(?<number>\b\d+(\.\d+)?f?\b)|(?<word>\b[A-Za-z_]\w*\b)",
        RegexOptions.Compiled
    );

    private const string CommentColor = "#6A9955";
    private const string StringColor = "#CE9178";
    private const string NumberColor = "#B5CEA8";
    private const string KeywordColor = "#569CD6";
    private const string TypeColor = "#4EC9B0";
    private const string AttributeColor = "#DCDCAA";
    private const string BindingColor = "#C586C0";
    
    public static string Highlight(string code) {
        var builder = new StringBuilder(code.Length * 2);
        var last = 0;

        foreach (Match match in tokens.Matches(code)) {
            AppendPlain(builder, code.Substring(last, match.Index - last));
            last = match.Index + match.Length;

            var color = Classify(match);
            if (color == null) {
                AppendPlain(builder, match.Value);
                continue;
            }

            builder.Append("<color=").Append(color).Append('>');
            AppendPlain(builder, match.Value);
            builder.Append("</color>");
        }

        AppendPlain(builder, code.Substring(last));
        return builder.ToString();
    }

    private static string? Classify(Match match) {
        if (match.Groups["comment"].Success) return CommentColor;
        if (match.Groups["attr"].Success) return AttributeColor;
        if (match.Groups["string"].Success || match.Groups["char"].Success) return StringColor;
        if (match.Groups["number"].Success) return NumberColor;

        var word = match.Value;
        if (keywords.Contains(word)) return KeywordColor;

        if (word.Length > 1 && word[0] == 's' && char.IsUpper(word[1])) return BindingColor;

        return char.IsUpper(word[0]) ? TypeColor : null;
    }

    private static void AppendPlain(StringBuilder builder, string text) {
        if (text.Length == 0) {
            return;
        }

        builder.Append("<noparse>").Append(text).Append("</noparse>");
    }

    #endregion
}
