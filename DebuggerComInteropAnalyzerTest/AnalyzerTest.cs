using DebuggerComInteropAnalyzerTest.TestData;
using NUnit.Framework;

namespace DebuggerComInteropAnalyzerTest
{
    public class Tests
    {
        [Test]
        public void SimpleErrorTest()
        {
            DoText<SimpleErrorTestData>(new[]
            {
                // is as 
                "(17,17): error ComInterop01: Could not use 'is' expression with 'TestComObject' type. Should use 'JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.Is<TestComObject>()' extension method",
                "(21,31): error ComInterop02: Could not use 'as' expression with 'TestComObject' type. Should use 'JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.QI<TestComObject>()' extension method",
                "(22,31): error ComInterop01: Could not use 'is' expression with 'TestComObject' type. Should use 'JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.Is<TestComObject>()' extension method",
                
                // is pattern matching
                "(27,17): error ComInterop04: Could not use pattern matching expression with 'TestComObject' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.QI<TestComObject>()' is TestComObject castedValue' expression",
                "(31,31): error ComInterop04: Could not use pattern matching expression with 'TestComObject' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.QI<TestComObject>()' is TestComObject castedValue' expression",
                
                // switch pattern matching
                "(38,17): error ComInterop04: Could not use pattern matching expression with 'TestComObject' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.QI<TestComObject>()' is TestComObject castedValue' expression",
                "(42,17): error ComInterop04: Could not use pattern matching expression with 'SharpGen.Runtime.IUnknown' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.QI<SharpGen.Runtime.IUnknown>()' is SharpGen.Runtime.IUnknown castedValue' expression",
                
                // OfType
                "(53,26): error ComInterop05: Could not use 'System.Linq.Enumerable.OfType<TestComObject>' with 'TestComObject' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.OfTypeQI<TestComObject>()' extension method",
                "(54,27): error ComInterop05: Could not use 'System.Linq.Enumerable.OfType<TestComObject>' with 'TestComObject' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.OfTypeQI<TestComObject>()' extension method",
                "(55,27): error ComInterop05: Could not use 'System.Linq.Enumerable.OfType<TestComObject>' with 'TestComObject' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.OfTypeQI<TestComObject>()' extension method",
                "(56,27): error ComInterop05: Could not use 'System.Linq.Enumerable.OfType<TestComObject>' with 'TestComObject' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.OfTypeQI<TestComObject>()' extension method",
                "(57,27): error ComInterop05: Could not use 'System.Linq.Enumerable.OfType<TestComObject>' with 'TestComObject' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.OfTypeQI<TestComObject>()' extension method",
                "(58,27): error ComInterop05: Could not use 'System.Linq.Enumerable.OfType<SharpGen.Runtime.IUnknown>' with 'SharpGen.Runtime.IUnknown' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.OfTypeQI<SharpGen.Runtime.IUnknown>()' extension method",
                "(58,27): error ComInterop05: Could not use 'System.Linq.Enumerable.OfType<TestComObject>' with 'TestComObject' type. Should use `JetBrains.Debugger.CorApi.SharpGenUtil.ComObjectEx.OfTypeQI<TestComObject>()' extension method",
            });
        }

        [Test]
        public void SimpleValidCastsTest()
        {
            DoText<SimpleValidCastsTestData>();
        }
        
        private static void DoText<T>(string[] messages = null) => typeof(T).DoTest(messages ?? new string[0]);
    }
}