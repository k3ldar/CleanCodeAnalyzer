using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    internal class ChainedAssignmentRule : IStyleRule
    {
        public static readonly DiagnosticDescriptor Rule = new(
            "CC0002",
            "Chained assignment detected",
            "Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity",
            "Style",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Chained assignments can reduce code readability and make debugging more difficult. " +
                        "They can obscure the flow of data and make it harder to understand which variables are being assigned. " +
                        "Additionally, they can cause confusion with operator precedence and may lead to unintended side effects. " +
                        "Use separate assignment statements for better maintainability.");

        public void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not AssignmentExpressionSyntax assignment)
            {
                return;
            }

            // Check if this is a simple assignment (=)
            if (assignment.Kind() != SyntaxKind.SimpleAssignmentExpression)
            {
                return;
            }

            // Check if the right side is another assignment expression
            if (assignment.Right is AssignmentExpressionSyntax)
            {
                // Only report if this assignment is NOT itself on the right side of another assignment
                // This ensures we only report once per chained assignment at the outermost level
                if (assignment.Parent is not AssignmentExpressionSyntax parentAssignment ||
                    parentAssignment.Right != assignment)
                {
                    // Find the parent statement to include the semicolon
                    var statement = assignment.FirstAncestorOrSelf<StatementSyntax>();
                    var diagnosticLocation = statement != null ? statement.GetLocation() : assignment.GetLocation();
                    var diagnostic = Diagnostic.Create(Rule, diagnosticLocation);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}