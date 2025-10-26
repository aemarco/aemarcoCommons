using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aemarcoCommons.Extensions;

public static class AttributeExtensions
{

    public static IEnumerable<T> GetAttributes<T>(this Type type, string memberName = null)
        where T : Attribute
    {
        //get attributes on type
        if (memberName == null)
        {
            return type
                .GetCustomAttributes(typeof(T))
                .OfType<T>();
        }
        //get attributes on member
        return type
            .GetMember(memberName)
            .FirstOrDefault()?
            .GetCustomAttributes(typeof(T))
            .OfType<T>() ?? new List<T>(); // ?? because we want a collection also if there is no member found

    }
    public static T GetAttribute<T>(this Type type, string memberName = null)
        where T : Attribute
    {
        //get attribute on object, type or member
        return type.GetAttributes<T>(memberName).FirstOrDefault();
    }
    public static bool HasAttribute<T>(this Type type, string memberName = null)
        where T : Attribute
    {
        return type.GetAttribute<T>(memberName) != null;
    }

    public static T GetAttribute<T>(this Enum value)
        where T : Attribute
    {
        var enumMember = value.GetType().GetMember(value.ToString()).First();
        var attributes = enumMember.GetCustomAttributes(typeof(T), false).Cast<T>();
        return attributes.FirstOrDefault();
    }

    public static IEnumerable<T> GetAttributes<T>(this object obj, string memberName = null)
        where T : Attribute
    {
        //get attributes on object, type or member
        return obj.GetType().GetAttributes<T>(memberName);
    }
    public static T GetAttribute<T>(this object obj, string memberName = null)
        where T : Attribute
    {
        return obj.GetType().GetAttribute<T>(memberName);
    }
    public static bool HasAttribute<T>(this object obj, string memberName = null)
        where T : Attribute
    {
        return obj.GetType().HasAttribute<T>(memberName);
    }

}