using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace CleanCodeAnalyzer.Rules
{
    internal class MisusedDocumentCommentRule : ISyntaxTreeRule
    {
        public static readonly DiagnosticDescriptor Rule = new(
            "CC0006",
            "Document comment used as code comment",
            "Document comment syntax '///' should not be used for inline comments. Use '//' for regular comments instead.",
            "Documentation",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Document comments (///) are intended for XML documentation on type and member declarations. Use regular comments (//) for inline code comments.");

        public void Analyze(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            var sourceText = context.Tree.GetText(context.CancellationToken);

            // Get all trivia in the syntax tree
            var allTrivia = root.DescendantTrivia(descendIntoTrivia: false);

            foreach (var trivia in allTrivia)
            {
                // Check for single-line comments that start with ///
                if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    var triviaText = trivia.ToString();

                    // Check if it's a triple-slash comment
                    if (triviaText.StartsWith("///"))
                    {
                        // Get the correct location (excluding the newline at the end)
                        var location = GetCommentLocation(trivia, sourceText);

                        var diagnostic = Diagnostic.Create(
                            Rule,
                            location);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                // Check for actual documentation comments
                // Check if this comment is attached to a valid documentation target
                else if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) && 
                    !IsAttachedToValidDeclaration(trivia))
                {
                    var location = GetCommentLocation(trivia, sourceText);

                    var diagnostic = Diagnostic.Create(
                        Rule,
                        location);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static Location GetCommentLocation(SyntaxTrivia trivia, SourceText sourceText)
        {
            // Get the full span of the trivia
            var fullSpan = trivia.FullSpan;

            // Get the line containing this trivia  
            var startLine = sourceText.Lines.GetLineFromPosition(fullSpan.Start);
            var lineText = startLine.ToString();

            // Find where /// actually appears on this line
            var commentStartIndex = lineText.IndexOf("///");

            if (commentStartIndex >= 0)
            {
                // Calculate absolute position where /// starts
                var absoluteStart = startLine.Start + commentStartIndex;

                // Calculate the length: from /// to end of line (excluding newline)
                // The comment ends at the end of the line, not including \r\n
                var commentEndPosition = startLine.End; // This excludes the line ending
                var length = commentEndPosition - absoluteStart;

                return Location.Create(trivia.SyntaxTree, new TextSpan(absoluteStart, length));
            }

            // Fallback: use the trivia's own span
            return trivia.GetLocation();
        }

        private static bool IsAttachedToValidDeclaration(SyntaxTrivia trivia)
        {
            var token = trivia.Token;

            // Must be leading trivia
            if (!token.LeadingTrivia.Contains(trivia))
            {
                return false;
            }

            // Get the syntax node that owns this token
            var node = token.Parent;

            // Special case: For documentation trivia, check if we're directly before a declaration
            // by looking for a valid target in the token's parent hierarchy
            var current = node;
            while (current != null)
            {
                // If we hit a block, we're inside executable code - invalid
                if (current is BlockSyntax)
                {
                    return false;
                }

                // Check if this is a valid documentation target
                if (IsValidDocumentationTarget(current))
                {
                    // Found a valid target - now verify the documentation is attached correctly
                    // The documentation should be on a token that's part of the declaration itself,
                    // not on a token inside the declaration's body

                    // For members with bodies (methods, properties with bodies, etc.)
                    // If the token is inside the body, it's not valid
                    if (current is MethodDeclarationSyntax methodDecl && 
                        methodDecl.Body != null &&
                        methodDecl.Body.Span.Contains(token.Span))
                    {
                        return false;
                    }

                    return true;
                }

                current = current.Parent;
            }

            return false;
        }

        private static bool IsValidDocumentationTarget(SyntaxNode node)
        {
            return node is BaseTypeDeclarationSyntax ||     // class, struct, interface, enum, record
                   node is DelegateDeclarationSyntax ||
                   node is MethodDeclarationSyntax ||
                   node is PropertyDeclarationSyntax ||
                   node is FieldDeclarationSyntax ||
                   node is EventDeclarationSyntax ||
                   node is IndexerDeclarationSyntax ||
                   node is ConstructorDeclarationSyntax ||
                   node is OperatorDeclarationSyntax ||
                   node is ConversionOperatorDeclarationSyntax ||
                   node is EnumMemberDeclarationSyntax ||
                   node is NamespaceDeclarationSyntax ||
                   node is FileScopedNamespaceDeclarationSyntax;
        }
    }
}