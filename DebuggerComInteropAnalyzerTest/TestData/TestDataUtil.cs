using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using DebuggerComInteropAnalyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace DebuggerComInteropAnalyzerTest.TestData
{
    public static class TestDataUtil
    {
        public static ImmutableArray<Diagnostic> GetTestDiagnostics(this Type type)
        {
            using (var stream = typeof(Tests).Assembly.GetManifestResourceStream(type.FullName + ".cs"))
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                var syntaxTree = CSharpSyntaxTree.ParseText(text);
                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => !x.IsDynamic)
                    .Select(x => x.Location)
                    .Where(x => !string.IsNullOrEmpty(x) && File.Exists(x))
                    .Select(x => MetadataReference.CreateFromFile(x))
                    .ToArray();

                var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
                var compilation = CSharpCompilation.Create("AnalyzerTestCompilation", new []{syntaxTree}, assemblies, options);
                var compilationWithAnalyzers = compilation.WithAnalyzers(AnalyzersProvider.GetAllAnalyzers().ToImmutableArray());
                return compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;
            }
        }

        public static void DoTest(this Type type, string[] messages)
        {
            var diagnostics = type.GetTestDiagnostics();
            for (var i = 0; i < messages.Length && i < diagnostics.Length; i++)
            {
                Assert.AreEqual(diagnostics[i].ToString(), messages[i]);
            }

            if (diagnostics.Length > messages.Length)
            {
                var builder = new StringBuilder("Unexpected diagnostics").AppendLine();
                for (var i = messages.Length; i < diagnostics.Length; i++)
                {
                    builder.Append("\"");
                    builder.Append(diagnostics[i]);
                    builder.AppendLine("\",");
                }
                Assert.Fail(builder.ToString());
            }
            else if (diagnostics.Length < messages.Length)
            {
                var builder = new StringBuilder("Expected diagnostics").AppendLine();
                for (var i = diagnostics.Length; i < messages.Length; i++)
                {
                    builder.Append("\"");
                    builder.Append(messages[i]);
                    builder.AppendLine("\",");
                }
                Assert.Fail(builder.ToString());
            }
        }
    }
}