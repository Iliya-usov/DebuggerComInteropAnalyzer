using System.Collections.Generic;
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
    public class RiderDebuggerComCastAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableArray<DiagnosticDescriptor> ourDescriptors = new List<ComInteropErrorKind>
        {
            ComInteropErrorKind.AsExpression,
            ComInteropErrorKind.IsExpression,
            ComInteropErrorKind.CastExpression,
            ComInteropErrorKind.PatternDeclaration
        }.Select(x => x.CreateDescriptor()).ToImmutableArray(); 

        public override void Initialize(AnalysisContext context)
        {
            if (!Debugger.IsAttached)
            {
                context.EnableConcurrentExecution();
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterCompilationStartAction(x =>
             {
                 x.RegisterSyntaxNodeAction(analysisContext => AnalyzeContext(analysisContext),
                     SyntaxKind.IsExpression,
                     SyntaxKind.AsExpression,
                     SyntaxKind.CastExpression,
                     SyntaxKind.DeclarationPattern);
             });
        }

        private static void AnalyzeContext(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node.Kind())
            {
                case SyntaxKind.IsExpression:
                    AnalyzeIsExpression(context);
                    return;
                case SyntaxKind.AsExpression:
                    AnalyzerAsExpression(context);
                    return;
                case SyntaxKind.DeclarationPattern:
                    AnalyzeDeclarationPatternExpression(context);
                    return;
                case SyntaxKind.CastExpression:
                    AnalyzeCastExpression(context);
                    return;
            }
        }

        private static void AnalyzerAsExpression(SyntaxNodeAnalysisContext context)
        {
            AnalyzeAsIsBinaryExpression(context, ComInteropErrorKind.AsExpression);
        }

        private static void AnalyzeIsExpression(SyntaxNodeAnalysisContext context)
        {
            AnalyzeAsIsBinaryExpression(context, ComInteropErrorKind.IsExpression);
        }

        private static void AnalyzeCastExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is CastExpressionSyntax castExpressionSyntax)
            {
                AnalyzeNode(context, castExpressionSyntax.Type, castExpressionSyntax, ComInteropErrorKind.CastExpression);
            }
        }

        private static void AnalyzeDeclarationPatternExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DeclarationPatternSyntax declarationPatternSyntax)
                AnalyzeNode(context, declarationPatternSyntax.Type, declarationPatternSyntax.Parent, ComInteropErrorKind.PatternDeclaration);
        }

        private static void AnalyzeAsIsBinaryExpression(SyntaxNodeAnalysisContext context, ComInteropErrorKind kind)
        {
            var node = context.Node;
            AnalyzeNode(context, node.ChildNodes().Last(), node, kind);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context, SyntaxNode comTypeNode, SyntaxNode parentNode, ComInteropErrorKind kind)
        {
            if (!comTypeNode.IsComObject(context)) return;

            var node = parentNode.ChildNodes().First();
            if (node is InvocationExpressionSyntax invocationExpressionSyntax)
            {
                if (context.IsComSafeCast(comTypeNode, invocationExpressionSyntax)) return;
            }
            else if (node is ConditionalAccessExpressionSyntax)
            {
                var conditionalAccess = node.DescendantNodesAndSelf().OfType<ConditionalAccessExpressionSyntax>().Last();
                if (conditionalAccess.ChildNodes().LastOrDefault() is InvocationExpressionSyntax invocationExpressionSyntax2)
                {
                    if (context.IsComSafeCast(comTypeNode, invocationExpressionSyntax2)) return;
                }
            }
            
            context.ReportDiagnostic(Diagnostic.Create(kind.CreateDescriptor(comTypeNode.GetTypeInfo(context).Type?.ToString()), parentNode.GetLocation()));
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ourDescriptors;
    }
}