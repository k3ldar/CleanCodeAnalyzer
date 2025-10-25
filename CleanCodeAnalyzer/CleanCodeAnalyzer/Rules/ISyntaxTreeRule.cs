using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer.Rules
{
    internal interface ISyntaxTreeRule
    {
        void Analyze(SyntaxTreeAnalysisContext context);
    }
}