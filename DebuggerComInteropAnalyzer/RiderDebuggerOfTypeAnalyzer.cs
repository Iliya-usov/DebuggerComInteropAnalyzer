using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DebuggerComInteropAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RiderDebuggerOfTypeAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor ourDiagnosticDescriptor = ComInteropErrorKind.OfType.CreateDescriptor();

        public override void Initialize(AnalysisContext context)
        {          
            if (!Debugger.IsAttached)
            {
                context.EnableConcurrentExecution();
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterCompilationStartAction(x =>
            {
                x.RegisterSyntaxNodeAction(analysisContext => AnalyzeContext(analysisContext),SyntaxKind.InvocationExpression);
            });
        }

        private static void AnalyzeContext(SyntaxNodeAnalysisContext analysisContext)
        {
            if (analysisContext.Node is InvocationExpressionSyntax)
            {
                AnalyzeInvocationExpression(analysisContext);
            }
        }

        private static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var linqEnumerableTypeName = ComInteropAnalyzerUtil.GetLinqEnumerableTypeName();
            if (linqEnumerableTypeName == null) return;

            var candidates = context.Node.ChildNodes()
                .Select(x => (SymbolInfo: x.GetSymbolInfo(context).Symbol as IMethodSymbol, Node: x))
                .Where(x => x.SymbolInfo != null);

            foreach (var (symbolInfo, syntaxNode) in candidates)
            {
                if (!symbolInfo.IsGenericMethod || symbolInfo.Name != "OfType" || !symbolInfo.ContainingType.ToString().Equals(linqEnumerableTypeName)) continue;
                var typeArgument = symbolInfo.TypeArguments.SingleOrDefault();
                if (typeArgument == null) continue;

                foreach (var candidate in typeArgument.GetTypeSymbolOrCandidates())
                {
                    if (candidate.IsComObject(context))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(ComInteropErrorKind.OfType.CreateDescriptor(candidate.ToString()), syntaxNode.GetLocation()));
                        return;
                    }
                }
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ourDiagnosticDescriptor);
    }
}