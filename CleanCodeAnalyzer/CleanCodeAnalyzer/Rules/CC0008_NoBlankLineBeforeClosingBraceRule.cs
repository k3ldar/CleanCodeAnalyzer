using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    internal class NoBlankLineBeforeClosingBraceRule : ISyntaxTreeRule
    {
        public static readonly DiagnosticDescriptor Rule = new(
            "CC0008",
            "Unnecessary blank line before closing brace",
            "Closing braces should not be preceded by blank lines",
            "Style",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Blank lines immediately before closing braces reduce code density without adding readability. Remove unnecessary blank lines before closing braces.");

        public void Analyze(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            var text = context.Tree.GetText();

            // Find all closing brace tokens
            var closingBraces = root.DescendantTokens()
                .Where(token => token.IsKind(SyntaxKind.CloseBraceToken));

            foreach (var closingBrace in closingBraces)
            {
                var location = closingBrace.GetLocation();
                var lineSpan = location.GetLineSpan();
                var closingBraceLine = lineSpan.StartLinePosition.Line;

                // Can't have a blank line before if this is the first line
                if (closingBraceLine == 0)
                {
                    continue;
                }

                // If the closing brace is not the first non-whitespace character on its line,
                // it's part of a single-line construct (e.g., `public int X { get; set; }`) and
                // we should not report a blank-line-before-closing-brace diagnostic for that case.
                var lineStart = text.Lines[closingBraceLine].Start;
                var bracePosition = closingBrace.SpanStart;

                if (bracePosition > lineStart)
                {
                    var leadingTextSpan = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(lineStart, bracePosition);
                    var leadingText = text.ToString(leadingTextSpan);
                    if (!string.IsNullOrWhiteSpace(leadingText))
                    {
                        // There is non-whitespace text before the closing brace on the same line -> skip
                        continue;
                    }
                }

                // Get the line immediately above the closing brace
                var lineAbove = closingBraceLine - 1;
                var lineAboveText = text.Lines[lineAbove].ToString().Trim();

                // Check if the line above is blank
                if (string.IsNullOrWhiteSpace(lineAboveText))
                {
                    // Count consecutive blank lines above
                    int blankLineCount = 0;
                    int checkLine = lineAbove;

                    while (checkLine >= 0 && string.IsNullOrWhiteSpace(text.Lines[checkLine].ToString()))
                    {
                        blankLineCount++;
                        checkLine--;
                    }

                    // Report diagnostic on the blank line(s) before the closing brace
                    // We'll report on the first blank line found
                    var blankLineStart = text.Lines[lineAbove].Start;
                    var blankLineEnd = text.Lines[lineAbove].End;
                    var diagnosticSpan = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(
                        blankLineStart,
                        blankLineEnd);
                    var diagnosticLocation = Location.Create(context.Tree, diagnosticSpan);

                    var diagnostic = Diagnostic.Create(Rule, diagnosticLocation);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
