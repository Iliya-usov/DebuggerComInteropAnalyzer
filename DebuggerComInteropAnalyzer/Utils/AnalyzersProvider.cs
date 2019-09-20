using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DebuggerComInteropAnalyzer.Utils
{
    public static class AnalyzersProvider
    {
        public static IEnumerable<DiagnosticAnalyzer> GetAllAnalyzers()
        {
            return typeof(AnalyzersProvider).Assembly.GetTypes()
                .Where(x => x.IsSubclassOf(typeof(DiagnosticAnalyzer)))
                .Select(x => Activator.CreateInstance(x)).OfType<DiagnosticAnalyzer>()
                .ToArray();
        }
    }
}