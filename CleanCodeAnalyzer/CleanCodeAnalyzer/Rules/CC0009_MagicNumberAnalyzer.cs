using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MagicNumberAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CC0009";

        private static readonly ImmutableHashSet<object> AllowedTrivialLiterals =
            ImmutableHashSet.Create<object>(0, 1, -1, true, false, "");

        private static readonly LocalizableString Title = "Avoid magic numbers (and un-named literals)";
        private static readonly LocalizableString MessageFormat = "Literal '{0}' should be replaced with a named constant";
        private static readonly LocalizableString Description =
            "Hardcoded numeric, string, or char literals decrease clarity and maintainability. " +
            "Replace with a well-named 'const' field or local constant that conveys semantic meaning.";
        private const string Category = "Style";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(
                AnalyzeLiteral,
                SyntaxKind.NumericLiteralExpression,
                SyntaxKind.StringLiteralExpression,
                SyntaxKind.CharacterLiteralExpression);
        }

        private static void AnalyzeLiteral(SyntaxNodeAnalysisContext context)
        {
            var literal = (LiteralExpressionSyntax)context.Node;

            // Enum member initializers are allowed.
            if (literal.Ancestors().OfType<EnumMemberDeclarationSyntax>().Any())
                return;

            // Attribute arguments are ignored (small declarative metadata).
            if (literal.Ancestors().OfType<AttributeArgumentSyntax>().Any())
                return;

            // Already part of a const or readonly declaration.
            if (IsWithinConstOrReadonlyDeclaration(literal))
                return;

            // Skip default literal forms.
            if (literal.IsKind(SyntaxKind.DefaultLiteralExpression))
                return;

            var valueOpt = context.SemanticModel.GetConstantValue(literal);
            if (valueOpt.HasValue && AllowedTrivialLiterals.Contains(valueOpt.Value))
                return;

            // Allow 0 in for-loop initializer.
            if (valueOpt.HasValue && Equals(valueOpt.Value, 0) && IsLoopInitializer(literal))
                return;

            // If the literal is part of a unary expression (e.g., -999), report the whole unary expression
            var nodeToReport = literal.Parent is PrefixUnaryExpressionSyntax unary
                ? unary
                : (SyntaxNode)literal;

            var diagnostic = Diagnostic.Create(Rule, nodeToReport.GetLocation(), literal.Token.Text);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsWithinConstOrReadonlyDeclaration(LiteralExpressionSyntax literal)
        {
            if (literal.Parent is EqualsValueClauseSyntax equals &&
                equals.Parent is VariableDeclaratorSyntax declarator)
            {
                var parent = declarator.Parent?.Parent;

                if (parent is FieldDeclarationSyntax fieldDecl &&
                    fieldDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword) || m.IsKind(SyntaxKind.ReadOnlyKeyword)))
                    return true;

                if (parent is LocalDeclarationStatementSyntax localDecl &&
                    localDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword)))
                    return true;
            }

            return false;
        }

        private static bool IsLoopInitializer(LiteralExpressionSyntax literal)
        {
            return literal.Parent is EqualsValueClauseSyntax evc &&
                evc.Parent is VariableDeclaratorSyntax vd &&
                vd.Parent?.Parent is ForStatementSyntax;
        }
    }
}