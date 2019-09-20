using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DebuggerComInteropAnalyzer
{
    internal static class ComInteropAnalyzerUtil
    {
        public static bool IsComObject(this SyntaxNode node, SyntaxNodeAnalysisContext context, bool ignoreComObjectClass = true)
        {
            return node.GetTypeInfo(context).Type.IsComObject(context, ignoreComObjectClass);
        }
    
        public static bool IsComObject(this ITypeSymbol typeSymbol, SyntaxNodeAnalysisContext context, bool ignoreComObjectClass = true)
        {
            var iUnknownTypeName = GetIUnknownTypeName();
            if (iUnknownTypeName == null) return false;

            if (ignoreComObjectClass)
            {
                var comObjectClassTypeName = GetComObjectClassTypeName();
                if (comObjectClassTypeName != null && typeSymbol.ToString().Equals(comObjectClassTypeName))
                    return false;
            }
      
            return typeSymbol.GetSelfAndAllInterfaces().Any(@interface => @interface.ToString().Equals(iUnknownTypeName));
        }

        public static TypeInfo GetTypeInfo(this SyntaxNode node, SyntaxNodeAnalysisContext context) => context.SemanticModel.GetTypeInfo(node);
        public static SymbolInfo GetSymbolInfo(this SyntaxNode node, SyntaxNodeAnalysisContext context) => context.SemanticModel.GetSymbolInfo(node);

        public static string GetIUnknownTypeName() => "SharpGen.Runtime.IUnknown";
        public static string GetComObjectClassTypeName() => "SharpGen.Runtime.ComObject";
        public static string GetLinqEnumerableTypeName() => "System.Linq.Enumerable";
        public static string GetComSafeCastAttributeTypeName() => "SharpGen.Runtime.ComSafeCastAttribute";

        public static IEnumerable<ITypeSymbol> GetSelfAndAllInterfaces(this ITypeSymbol type)
        {
            if (type == null) yield break;
      
            yield return type;
            foreach (var @interface in type.AllInterfaces)
                yield return @interface;
        }

        public static IEnumerable<ITypeSymbol> GetSelfAndAllBaseClassesAndAllInterfaces(this ITypeSymbol type)
        {
            if (type == null)  yield break;

            yield return type;

            var baseType = type.BaseType;
            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
            
            foreach (var @interface in type.AllInterfaces)
                yield return @interface;
        }

        public static IEnumerable<ITypeSymbol> GetTypeSymbolOrCandidates(this ITypeSymbol symbol)
        {
            yield return symbol;
            if (symbol is IErrorTypeSymbol errorTypeSymbol)
            {
                foreach (var candidate in errorTypeSymbol.CandidateSymbols.OfType<ITypeSymbol>())
                {
                    yield return candidate;
                }
            }
        }
        
        public static bool IsComSafeCast(this SyntaxNodeAnalysisContext context, SyntaxNode comTypeNode, InvocationExpressionSyntax currentNode)
        {
            var symbolInfo = currentNode.GetSymbolInfo(context);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
            if (methodSymbol == null) return false;
            
            var typeInfo = comTypeNode.GetTypeInfo(context).Type;
            if (typeInfo == null) return false;
            
            var comCastAttribute = GetComSafeCastAttributeTypeName();
            if (comCastAttribute == null) return false;
            
            foreach (var attribute in methodSymbol.GetAttributes())
            {
                if (!attribute.AttributeClass.ToString().Equals(comCastAttribute)) continue;

                foreach (var returnType in methodSymbol.ReturnType.GetTypeSymbolOrCandidates())
                {
                    foreach (var item in returnType.GetSelfAndAllBaseClassesAndAllInterfaces())
                    {
                        if (item.Equals(typeInfo))
                            return true;
                    }
                }

                break;
            }

            return false;
        }
    }
}