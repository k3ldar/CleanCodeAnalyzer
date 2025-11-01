using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    internal class MultipleStatementsPerLineRule : IStyleRule
    {
        public static readonly DiagnosticDescriptor Rule = new(
            "CC0001",
            "Multiple statements on one line",
            "Multiple statements should not be placed on the same line",
            "Style",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Each statement should be on its own line for better readability.");

        public void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is CompilationUnitSyntax compilationUnit)
            {
                AnalyzeCompilationUnit(context, compilationUnit);
            }
            else
            {
                var statements = GetStatements(context.Node);
                if (statements.HasValue && statements.Value.Count >= 2)
                {
                    CheckMultipleStatementsOnLine(context, statements.Value);
                }
            }
        }

        private void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context, CompilationUnitSyntax compilationUnit)
        {
            var statements = compilationUnit.Members
                .OfType<GlobalStatementSyntax>()
                .Select(gs => gs.Statement)
                .ToList();

            if (statements.Count >= 2)
            {
                CheckMultipleStatementsOnLine(context, statements);
            }
        }

        private void CheckMultipleStatementsOnLine(
            SyntaxNodeAnalysisContext context,
            IEnumerable<StatementSyntax> statements)
        {
            var statementsByLine = statements
                .GroupBy(s => s.GetLocation().GetLineSpan().StartLinePosition.Line)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var group in statementsByLine)
            {
                var statementsOnLine = group.ToList();
                for (int i = 1; i < statementsOnLine.Count; i++)
                {
                    var diagnostic = Diagnostic.Create(Rule, statementsOnLine[i].GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private SyntaxList<StatementSyntax>? GetStatements(SyntaxNode node)
        {
            return node switch
            {
                BlockSyntax block => (SyntaxList<StatementSyntax>?)block.Statements,
                SwitchSectionSyntax switchSection => (SyntaxList<StatementSyntax>?)switchSection.Statements,
                _ => null,
            };
        }
    }
}