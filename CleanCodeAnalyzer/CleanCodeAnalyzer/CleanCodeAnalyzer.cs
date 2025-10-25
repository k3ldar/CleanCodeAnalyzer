using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using CleanCodeAnalyzer.Rules;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanCodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CleanCodeAnalyzer : DiagnosticAnalyzer
    {
        // Auto-discover all rule instances at initialization time
        private static readonly ImmutableArray<IStyleRule> StyleRules = DiscoverStyleRules();
        private static readonly ImmutableArray<ISyntaxTreeRule> SyntaxTreeRules = DiscoverSyntaxTreeRules();

        private static ImmutableArray<IStyleRule> DiscoverStyleRules()
        {
            return [.. typeof(CleanCodeAnalyzer).Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IStyleRule).IsAssignableFrom(t))
                .Select(t => (IStyleRule)System.Activator.CreateInstance(t))];
        }

        private static ImmutableArray<ISyntaxTreeRule> DiscoverSyntaxTreeRules()
        {
            return [.. typeof(CleanCodeAnalyzer).Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ISyntaxTreeRule).IsAssignableFrom(t))
                .Select(t => (ISyntaxTreeRule)System.Activator.CreateInstance(t))];
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                var descriptors = ImmutableArray.CreateBuilder<DiagnosticDescriptor>();

                // Use reflection to get the Rule property from each rule type
                foreach (var rule in StyleRules)
                {
                    var ruleProperty = rule.GetType().GetField("Rule", BindingFlags.Public | BindingFlags.Static);
                    if (ruleProperty?.GetValue(null) is DiagnosticDescriptor descriptor)
                    {
                        descriptors.Add(descriptor);
                    }
                }

                foreach (var rule in SyntaxTreeRules)
                {
                    var ruleProperty = rule.GetType().GetField("Rule", BindingFlags.Public | BindingFlags.Static);
                    if (ruleProperty?.GetValue(null) is DiagnosticDescriptor descriptor)
                    {
                        descriptors.Add(descriptor);
                    }
                }

                return descriptors.ToImmutable();
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            if (StyleRules.Length > 0)
            {
                // Collect all SyntaxKinds needed by all rules (you may want to make this configurable per rule)
                context.RegisterSyntaxNodeAction(AnalyzeNode,
                    SyntaxKind.Block,
                    SyntaxKind.SwitchSection,
                    SyntaxKind.CompilationUnit,
                    SyntaxKind.FieldDeclaration,
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxKind.IfStatement,
                    SyntaxKind.SwitchStatement);
            }

            if (SyntaxTreeRules.Length > 0)
            {
                context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
            }
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // Delegate to all discovered rules
            foreach (var rule in StyleRules)
            {
                rule.Analyze(context);
            }
        }

        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            // Delegate to all discovered rules
            foreach (var rule in SyntaxTreeRules)
            {
                rule.Analyze(context);
            }
        }
    }
}