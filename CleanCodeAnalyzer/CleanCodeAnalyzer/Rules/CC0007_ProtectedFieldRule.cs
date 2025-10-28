using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    internal class ProtectedFieldRule : IStyleRule
    {
        public static readonly DiagnosticDescriptor Rule = new(
            "CC0007",
            "Protected field detected",
            "Protected field '{0}' should not be used in classes",
            "Design",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Protected fields are inherently dangerous as they can lead to unknown states and break encapsulation. Consider using protected properties or private fields instead.");

        public void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is FieldDeclarationSyntax fieldDeclaration)
            {
                // Check if the field has protected modifier but NOT protected internal
                var hasProtectedModifier = fieldDeclaration.Modifiers
                    .Any(m => m.IsKind(SyntaxKind.ProtectedKeyword));

                var hasInternalModifier = fieldDeclaration.Modifiers
                    .Any(m => m.IsKind(SyntaxKind.InternalKeyword));

                // Check if the field is const or readonly (these are safe)
                var isConstOrReadonly = fieldDeclaration.Modifiers
                    .Any(m => m.IsKind(SyntaxKind.ConstKeyword) || m.IsKind(SyntaxKind.ReadOnlyKeyword));

                // Only report if it's protected but not protected internal, and not const or readonly
                if (hasProtectedModifier && !hasInternalModifier && !isConstOrReadonly)
                {
                    // Report diagnostic for each variable declared in this field declaration
                    foreach (var variable in fieldDeclaration.Declaration.Variables)
                    {
                        var diagnostic = Diagnostic.Create(
                            Rule,
                            variable.Identifier.GetLocation(),
                            variable.Identifier.Text);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}