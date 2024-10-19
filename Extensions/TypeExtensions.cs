using System;
using System.Linq;

namespace aemarcoCommons.Extensions
{
    public static class TypeExtensions
    {
        public static string GetReadableTypeName(this Type type)
        {
            var result = type.IsGenericType
                ? $"{type.Name.Split('`')[0]}<{string.Join(",", type.GetGenericArguments().Select(x => x.Name))}>"
                : type.Name;
            return result;
        }
    }
}
