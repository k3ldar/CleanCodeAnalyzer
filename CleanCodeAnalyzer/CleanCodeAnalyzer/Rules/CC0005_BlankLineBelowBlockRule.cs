using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    internal class BlankLineBelowBlockRule : IStyleRule
    {
        public static readonly DiagnosticDescriptor Rule = new(
            "CC0005",
            "Block statement should have exactly one blank line below it",
            "Control flow blocks should be followed by exactly one blank line for better readability",
            "Style",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "Control flow blocks (if, for, while, switch, using, try-catch, etc.) should be followed by exactly one blank line to visually separate logical sections and improve code readability.");

        public void Analyze(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode nodeToCheck = null;
            SyntaxToken? closingBraceToken = null;

            // Check for if statements (not else-if)
            if (context.Node is IfStatementSyntax ifStatement)
            {
                // Skip if this is part of an if-else chain (followed by else clause)
                if (ifStatement.Else != null)
                {
                    return;
                }

                // Skip if this is an else-if statement
                if (ifStatement.Parent is ElseClauseSyntax)
                {
                    return;
                }

                nodeToCheck = ifStatement;

                // Get the closing brace if the if statement has a block
                if (ifStatement.Statement is BlockSyntax ifBlock)
                {
                    closingBraceToken = ifBlock.CloseBraceToken;
                }
            }
            // Check for switch statements
            else if (context.Node is SwitchStatementSyntax switchStatement)
            {
                nodeToCheck = switchStatement;
                closingBraceToken = switchStatement.CloseBraceToken;
            }
            // Check for any block syntax (to catch closing braces)
            else if (context.Node is BlockSyntax block)
            {
                // Only check blocks that are direct children of statements we care about
                // Skip method blocks, class blocks, etc.
                if (block.Parent is IfStatementSyntax ||
                    block.Parent is ElseClauseSyntax ||
                    block.Parent is ForStatementSyntax ||
                    block.Parent is ForEachStatementSyntax ||
                    block.Parent is WhileStatementSyntax ||
                    block.Parent is DoStatementSyntax ||
                    block.Parent is UsingStatementSyntax ||
                    block.Parent is LockStatementSyntax ||
                    block.Parent is TryStatementSyntax ||
                    block.Parent is CatchClauseSyntax ||
                    block.Parent is FinallyClauseSyntax)
                {
                    // These are handled by their parent statement checks
                    return;
                }

                nodeToCheck = block;
                closingBraceToken = block.CloseBraceToken;
            }

            if (nodeToCheck == null)
            {
                return;
            }

            // Get the syntax tree text
            var syntaxTree = nodeToCheck.SyntaxTree;
            var text = syntaxTree.GetText();

            // Determine which token to check after
            var tokenToCheckAfter = closingBraceToken ?? nodeToCheck.GetLastToken();

            // Get the line position of the closing token
            var tokenLocation = tokenToCheckAfter.GetLocation();
            var tokenLineSpan = tokenLocation.GetLineSpan();
            var tokenLineNumber = tokenLineSpan.EndLinePosition.Line;

            // Check if this is the last line in the file
            if (tokenLineNumber >= text.Lines.Count - 1)
            {
                return;
            }

            // Check if this is the last statement in a block/scope
            if (IsLastStatementInBlock(nodeToCheck))
            {
                return;
            }

            // Count blank lines below
            int blankLineCount = 0;
            int checkLine = tokenLineNumber + 1;

            while (checkLine < text.Lines.Count && text.Lines[checkLine].ToString().Trim() == string.Empty)
            {
                blankLineCount++;
                checkLine++;
            }

            // If there's no next statement (we've reached the end of a block), skip
            if (checkLine >= text.Lines.Count)
            {
                return;
            }

            // Check if the next non-blank line is a closing brace
            var nextLineText = text.Lines[checkLine].ToString().Trim();
            if (nextLineText == "}" || nextLineText.StartsWith("}"))
            {
                return;
            }

            // Report diagnostic if there isn't exactly one blank line
            if (blankLineCount != 1)
            {
                var diagnostic = Diagnostic.Create(Rule, tokenLocation);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private bool IsLastStatementInBlock(SyntaxNode node)
        {
            // Navigate up to find the containing statement
            var statement = node as StatementSyntax ?? node.FirstAncestorOrSelf<StatementSyntax>();
            if (statement == null)
            {
                return false;
            }

            var parent = statement.Parent;

            // Check if parent is a block
            if (parent is BlockSyntax block)
            {
                return block.Statements.LastOrDefault() == statement;
            }

            // Check if parent is a switch section
            if (parent is SwitchSectionSyntax switchSection)
            {
                return switchSection.Statements.LastOrDefault() == statement;
            }

            // Check for compilation unit (global statements)
            if (parent is GlobalStatementSyntax globalStatement &&
                globalStatement.Parent is CompilationUnitSyntax compilationUnit)
            {
                return compilationUnit.Members.LastOrDefault() == globalStatement;
            }

            return false;
        }
    }
}