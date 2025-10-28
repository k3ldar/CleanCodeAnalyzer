using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    internal class StringLiteralDuplicationRule : ISyntaxTreeRule
    {
        public const string DiagnosticId = "CC0010";

        private static readonly LocalizableString Title = "String literal duplicated";
        private static readonly LocalizableString MessageFormat =
            "String literal \"{0}\" is duplicated {1} times in this file. Extract to a named constant.";
        private static readonly LocalizableString Description =
            "Duplicated string literals increase maintenance burden and introduce inconsistency risk. " +
            "Extract repeated strings to a well-named 'const' or 'static readonly' field.";
        private const string Category = "Maintainability";

        public static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public void Analyze(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            var literals = new Dictionary<string, List<LiteralExpressionSyntax>>();

            // Collect all string literals
            CollectStringLiterals(root, literals, context.CancellationToken);

            // Report duplicates (threshold: 2+ occurrences, min length: 5 chars)
            const int minDuplicateCount = 2;
            const int minStringLength = 5;

            foreach (var kvp in literals)
            {
                var literalValue = kvp.Key;
                var occurrences = kvp.Value;

                if (occurrences.Count >= minDuplicateCount &&
                    literalValue.Length >= minStringLength &&
                    !IsTriviallySimilar(literalValue))
                {
                    // Report all but the first occurrence
                    for (int i = 1; i < occurrences.Count; i++)
                    {
                        var diagnostic = Diagnostic.Create(
                            Rule,
                            occurrences[i].GetLocation(),
                            literalValue,
                            occurrences.Count);

                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private static void CollectStringLiterals(
            SyntaxNode node,
            Dictionary<string, List<LiteralExpressionSyntax>> literals,
            System.Threading.CancellationToken cancellationToken)
        {
            if (node is LiteralExpressionSyntax literal &&
                literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                // Skip if part of const declaration (already intentional)
                if (IsWithinConstDeclaration(literal))
                    return;

                // Skip if inside an attribute (e.g. SuppressMessage) or attribute argument
                if (IsWithinAttribute(literal))
                    return;

                // Skip interpolated strings (handled separately)
                if (literal.Token.Text.StartsWith("$"))
                    return;

                // Skip empty strings
                var literalValue = (string)literal.Token.Value!;
                if (string.IsNullOrWhiteSpace(literalValue))
                    return;

                if (!literals.ContainsKey(literalValue))
                    literals[literalValue] = new List<LiteralExpressionSyntax>();

                literals[literalValue].Add(literal);
            }

            foreach (var child in node.ChildNodes())
            {
                CollectStringLiterals(child, literals, cancellationToken);
            }
        }

        private static bool IsWithinConstDeclaration(LiteralExpressionSyntax literal)
        {
            if (literal.Parent is EqualsValueClauseSyntax equals &&
                equals.Parent is VariableDeclaratorSyntax declarator)
            {
                var parent = declarator.Parent?.Parent;

                if (parent is FieldDeclarationSyntax fieldDecl &&
                    fieldDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword)))
                    return true;

                if (parent is LocalDeclarationStatementSyntax localDecl &&
                    localDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword)))
                    return true;
            }

            return false;
        }

        private static bool IsWithinAttribute(LiteralExpressionSyntax literal)
        {
            for (var node = literal.Parent; node != null; node = node.Parent)
            {
                if (node is AttributeArgumentSyntax || node is AttributeSyntax || node is AttributeListSyntax)
                    return true;
            }

            return false;
        }

        private static bool IsTriviallySimilar(string value)
        {
            // Skip single words, punctuation-only, or single characters
            return value.Length <= 1 ||
                !value.Any(char.IsLetter) ||
                value.All(c => char.IsPunctuation(c) || char.IsWhiteSpace(c));
        }
    }
}