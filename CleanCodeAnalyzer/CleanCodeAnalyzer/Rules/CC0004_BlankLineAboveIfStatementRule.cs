using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    internal class BlankLineAboveIfStatementRule : IStyleRule
    {
        public static readonly DiagnosticDescriptor Rule = new(
            "CC0004",
            "If statement should have exactly one blank line above it",
            "If statements should be preceded by exactly one blank line for better readability",
            "Style",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "If statements should be preceded by exactly one blank line to visually separate control flow logic from other statements, improving code readability.");

        public void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not IfStatementSyntax ifStatement)
            {
                return;
            }

            // Skip if this is an else-if statement
            if (ifStatement.Parent is ElseClauseSyntax)
            {
                return;
            }

            // Get the syntax tree text
            var syntaxTree = ifStatement.SyntaxTree;
            var text = syntaxTree.GetText();

            // Get the line position of the if statement
            var ifLocation = ifStatement.GetLocation();
            var ifLineSpan = ifLocation.GetLineSpan();
            var ifLineNumber = ifLineSpan.StartLinePosition.Line;

            // Skip if this is the first line in the file or in its containing block
            if (ifLineNumber == 0)
            {
                return;
            }

            // Check if this is the first statement in a block/scope
            if (IsFirstStatementInBlock(ifStatement))
            {
                return;
            }

            // Count blank lines above the if statement
            int blankLineCount = 0;
            int checkLine = ifLineNumber - 1;

            while (checkLine >= 0 && text.Lines[checkLine].ToString().Trim() == string.Empty)
            {
                blankLineCount++;
                checkLine--;
            }

            // If there's no previous statement (we've reached the start of a block), skip
            if (checkLine < 0)
            {
                return;
            }

            // Check if the previous non-blank line is a block opening brace
            var previousLineText = text.Lines[checkLine].ToString().Trim();
            if (previousLineText == "{" || previousLineText.EndsWith("{"))
            {
                return;
            }

            // Check if the previous statement is a closing brace of a control flow statement
            // This is handled by CC0005 (blank line below block), so we should skip it here
            if (IsPreviousStatementControlFlowBlock(ifStatement))
            {
                return;
            }

            // Report diagnostic if there isn't exactly one blank line
            if (blankLineCount != 1)
            {
                // Report on just the if keyword and condition, not the entire statement
                var diagnosticSpan = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(
                    ifStatement.IfKeyword.Span.Start,
                    ifStatement.CloseParenToken.Span.End);
                var diagnosticLocation = Location.Create(syntaxTree, diagnosticSpan);
                var diagnostic = Diagnostic.Create(Rule, diagnosticLocation);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private bool IsFirstStatementInBlock(IfStatementSyntax ifStatement)
        {
            var parent = ifStatement.Parent;

            // Check if parent is a block
            if (parent is BlockSyntax block)
            {
                return block.Statements.FirstOrDefault() == ifStatement;
            }

            // Check if parent is a switch section
            if (parent is SwitchSectionSyntax switchSection)
            {
                return switchSection.Statements.FirstOrDefault() == ifStatement;
            }

            // Check for compilation unit (global statements)
            if (parent is GlobalStatementSyntax globalStatement &&
                globalStatement.Parent is CompilationUnitSyntax compilationUnit)
            {
                return compilationUnit.Members.FirstOrDefault() == globalStatement;
            }

            return false;
        }

        private bool IsPreviousStatementControlFlowBlock(IfStatementSyntax ifStatement)
        {
            var parent = ifStatement.Parent;

            // Get the containing block
            if (parent is not BlockSyntax block)
            {
                return false;
            }

            // Find the position of the current if statement
            var statements = block.Statements;
            var currentIndex = statements.IndexOf(ifStatement);

            // If it's the first statement, there's no previous statement
            if (currentIndex <= 0)
            {
                return false;
            }

            // Get the previous statement
            var previousStatement = statements[currentIndex - 1];

            // Check if the previous statement is a control flow statement with a block
            return previousStatement switch
            {
                IfStatementSyntax ifStmt when ifStmt.Statement is BlockSyntax => true,
                SwitchStatementSyntax => true,
                ForStatementSyntax forStmt when forStmt.Statement is BlockSyntax => true,
                ForEachStatementSyntax foreachStmt when foreachStmt.Statement is BlockSyntax => true,
                WhileStatementSyntax whileStmt when whileStmt.Statement is BlockSyntax => true,
                DoStatementSyntax doStmt when doStmt.Statement is BlockSyntax => true,
                UsingStatementSyntax usingStmt when usingStmt.Statement is BlockSyntax => true,
                LockStatementSyntax lockStmt when lockStmt.Statement is BlockSyntax => true,
                TryStatementSyntax => true,
                _ => false
            };
        }
    }
}