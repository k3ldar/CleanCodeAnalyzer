using CleanCodeAnalyzer.Test.Verifiers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace CleanCodeAnalyzer.Test
{
    public static partial class CSharpAnalyzerVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public class Test : CSharpAnalyzerTest<TAnalyzer, MSTestVerifier>
        {
            public Test()
            {
                SolutionTransforms.Add((solution, projectId) =>
                {
                    var project = solution.GetProject(projectId);
                    var compilationOptions = project.CompilationOptions;

                    compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                        compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));

                    solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                    // Enable XML documentation comment parsing
                    var parseOptions = (CSharpParseOptions)project.ParseOptions;
                    parseOptions = parseOptions.WithDocumentationMode(DocumentationMode.Parse);
                    solution = solution.WithProjectParseOptions(projectId, parseOptions);

                    return solution;
                });
            }
        }
    }
}
