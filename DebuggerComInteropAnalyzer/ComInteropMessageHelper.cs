using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using static DebuggerComInteropAnalyzer.ComInteropErrorKind;

namespace DebuggerComInteropAnalyzer
{
    public enum ComInteropErrorKind
    {
        IsExpression = 1,
        AsExpression = 2,
        CastExpression = 3,
        PatternDeclaration = 4,
        OfType = 5,
    }

    public static class ComInteropMessageHelper
    {
        public static string GetMessage(this ComInteropErrorKind kind) => ourKindToMessage[kind];

        public static DiagnosticDescriptor CreateDescriptor(this ComInteropErrorKind kind, string castTypeName = null)
        {
            castTypeName = string.IsNullOrEmpty(castTypeName) ? "IUnknown" : castTypeName;

            var message = string.Format(kind.GetMessage(), castTypeName);
            return new DiagnosticDescriptor(
                $"ComInterop{(int) kind:D2}",
                message,
                message,
                "ComInterop",
                DiagnosticSeverity.Error,
                true);
        }

        private static readonly IReadOnlyDictionary<ComInteropErrorKind, string> ourKindToMessage = new Dictionary<ComInteropErrorKind, string>
        {
            [IsExpression] = "Could not use 'is' expression with '{0}' type. Should use 'JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.Is<{0}>()' extension method",
            [AsExpression] = "Could not use 'as' expression with '{0}' type. Should use 'JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.QI<{0}>()' extension method",
            [CastExpression] = "Could not use 'cast' expression with '{0}' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.QIStrong<{0}>()' extension method",
            [PatternDeclaration] = "Could not use pattern matching expression with '{0}' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.QI<{0}>()' is {0} castedValue' expression",
            [OfType] = "Could not use 'System.Linq.Enumerable.OfType<{0}>' with '{0}' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.OfTypeQI<{0}>()' extension method"
        };
    }

}