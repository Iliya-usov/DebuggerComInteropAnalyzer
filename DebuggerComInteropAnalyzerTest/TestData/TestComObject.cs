using SharpGen.Runtime;
using System;
using System.Collections.Generic;

internal class TestComObject : ComObject
{
}

namespace SharpGen.Runtime
{
    public interface IUnknown
    {
    }

    public class ComObject : IUnknown
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ComSafeCastAttribute : Attribute
    {
    }
}

namespace JetBrains.Debugger.CorApi.SharpGenUtil
{
    using SharpGen.Runtime;

    public static class ComObjectEx
    {
        [ComSafeCast]
        public static T QI<T>(this IUnknown comObject) where T : class, IUnknown
        {
            throw new NotImplementedException();
        }

        [ComSafeCast]
        public static T QIStrong<T>(this IUnknown comObject) where T : class, IUnknown
        {
            throw new NotImplementedException();
        }

        [ComSafeCast]
        public static bool Is<T>(this IUnknown comObject) where T : class, IUnknown
        {
            throw new NotImplementedException();
        }

        [ComSafeCast]
        public static bool Is<T>(this IUnknown comObject, out T casted) where T : class, IUnknown
        {
            throw new NotImplementedException();
        }

        [ComSafeCast]
        public static IEnumerable<T> OfTypeQI<T>(this IEnumerable<IUnknown> enumerable) where T : class, IUnknown
        {
            throw new NotImplementedException();
        }
    }
}