using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    internal class MaximumLineLengthRule : ISyntaxTreeRule
    {
        private const int DefaultMaxLineLength = 120;
        private const string MaxLineLengthOptionName = "dotnet_diagnostic.CC0003.max_line_length";

        public static readonly DiagnosticDescriptor Rule = new(
            "CC0003",
            "Line exceeds maximum length",
            "Line length of {0} characters exceeds the maximum of {1} characters",
            "Style",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "Lines should not exceed a reasonable length to maintain readability and reduce horizontal scrolling.");

        public void Analyze(SyntaxTreeAnalysisContext context)
        {
            // Read the configured max line length from .editorconfig
            var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Tree);
            var maxLineLength = GetMaxLineLength(options);

            var text = context.Tree.GetText(context.CancellationToken);

            foreach (var line in text.Lines)
            {
                var lineText = line.ToString();
                var trimmedLine = lineText.TrimStart();

                // Skip blank lines and comment-only lines
                if (string.IsNullOrWhiteSpace(trimmedLine) ||
                    trimmedLine.StartsWith("//"))
                {
                    continue;
                }

                var lineLength = lineText.Length;

                if (lineLength > maxLineLength)
                {
                    var location = Location.Create(context.Tree, line.Span);
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        location,
                        lineLength,
                        maxLineLength);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static int GetMaxLineLength(AnalyzerConfigOptions options)
        {
            if (options.TryGetValue(MaxLineLengthOptionName, out var valueStr) &&
                int.TryParse(valueStr, out var value) &&
                value > 0)
            {
                return value;
            }

            return DefaultMaxLineLength;
        }
    }
}