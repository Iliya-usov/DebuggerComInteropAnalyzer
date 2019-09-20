using System.Linq;
using SharpGen.Runtime;
// for new namespaces







namespace DebuggerComInteropAnalyzerTest.TestData
{
    internal class SimpleErrorTestData
    {
        public static void IsAsErrorTest(IUnknown unknown)
        {
            if (unknown is TestComObject)
            {
            }

            var myComObjcet = unknown as TestComObject;
            var isComObject = unknown is TestComObject;
        }
        
        public static void IsPatternErrorTest(IUnknown unknown)
        {
            if (unknown is TestComObject testComObject)
            {
            }

            var isComObject = unknown is TestComObject testComObject2 && testComObject2.ToString() == "blablabla";
        }
        
        public static void SwitchErrorTest(object unknown)
        {
            switch (new object())
            {
                case TestComObject testComObject:
                {
                    break;
                }
                case IUnknown com3 when (com3.Equals(new object())):
                {
                    break;
                }
                case string s:
                    break;
            }
        }
        
        public static void OfTypeErrorTest(IUnknown[] comArrayOfType)
        {
            var ofType = comArrayOfType.OfType<TestComObject>();
            var ofType2 = comArrayOfType.Select(x => x).OfType<TestComObject>();
            var ofType3 = Enumerable.OfType<TestComObject>(comArrayOfType);
            var ofType4 = Enumerable.OfType<TestComObject>(comArrayOfType).Select(x => x);
            var ofType5 = Enumerable.OfType<TestComObject>(comArrayOfType.Where(x => x != null)).Select(x => x);
            var ofType6 = Enumerable.OfType<TestComObject>(comArrayOfType.Where(x => x != null)).Select(x => x).OfType<IUnknown>();
        }
    }
}