using System.Linq;
using JetBrains.Debugger.CorApi.SharpGenUtil;
using SharpGen.Runtime;
// for new namespaces







namespace DebuggerComInteropAnalyzerTest.TestData
{
    internal class SimpleValidCastsTestData
    {
        public static void IsAsValidTest(IUnknown unknown)
        {
            var myComObjcet = unknown.QI<TestComObject>();
            var myComObjcet2 = unknown.QIStrong<TestComObject>();
            var isComObject = unknown.Is<TestComObject>();
        }
        
        public static void IsPatternErrorTest(IUnknown unknown)
        {
            if (unknown.QI<TestComObject>() is TestComObject testComObject)
            {
            }

            var isComObject = unknown.QI<TestComObject>() is TestComObject testComObject2 && testComObject2.ToString() == "blablabla";
        }
        
        public static void OfTypeErrorTest(IUnknown[] comArrayOfType)
        {
            var ofType = comArrayOfType.OfTypeQI<TestComObject>();
            var ofType2 = comArrayOfType.Select(x => x).OfTypeQI<TestComObject>();
            var ofType3 = ComObjectEx.OfTypeQI<TestComObject>(comArrayOfType);
            var ofType4 = ComObjectEx.OfTypeQI<TestComObject>(comArrayOfType).Select(x => x);
            var ofType5 = ComObjectEx.OfTypeQI<TestComObject>(comArrayOfType.Where(x => x != null)).Select(x => x);
        }
    }
}