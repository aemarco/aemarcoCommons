using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extensions.AttributeExtensions
{
    public static class ReadingExtensions
    {

        public static T GetAttribute<T>(this Type type, string memberName = null) 
            where T : Attribute
        {
            //get attribute on type
            if (memberName == null)
            {
                return (T)type.GetCustomAttribute(typeof(T));
            }
            //get attribute on member
            else
            {
                return (T) type.GetMember(memberName).FirstOrDefault()?.GetCustomAttribute(typeof(T));
            }
        }

        public static T GetAttribute<T>(this object obj, string memberName = null) 
            where T : Attribute
        {
            return obj.GetType().GetAttribute<T>(memberName);
        }


        public static bool HasAttribute<T>(this object obj, string memberName = null)
            where T : Attribute
        {
            return obj.GetAttribute<T>(memberName) != null;
        }




        public static IEnumerable<T> GetAttributes<T>(this Type type, string memberName = null) 
            where T : Attribute
        {
            //get attribute on type
            if (memberName == null)
            {
                return (IEnumerable<T>)type.GetCustomAttributes(typeof(T));
            }
            //get attribute on member
            else
            {
                return (IEnumerable<T>) type.GetMember(memberName).FirstOrDefault()?.GetCustomAttributes(typeof(T));
            }
        }

        public static IEnumerable<T> GetAttributes<T>(this object obj, string memberName = null) 
            where T : Attribute
        {
            return obj.GetType().GetAttributes<T>(memberName);
        }



    }
}
