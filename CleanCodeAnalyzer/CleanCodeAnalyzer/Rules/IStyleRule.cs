using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    internal interface IStyleRule
    {
        void Analyze(SyntaxNodeAnalysisContext context);
    }
}