using System;
using System.Reflection;

public static class TypeExtensions
{
    public static FieldInfo GetPrivateField(this Type t, String name, BindingFlags bf = 
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
    {
        FieldInfo fi;
        while ((fi = t.GetField(name, bf)) == null && (t = t.BaseType) != null);
        return fi;
    }

    public static MethodInfo GetPrivateMethod(this Type t, String name, BindingFlags bf =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
    {
        MethodInfo fi;
        while ((fi = t.GetMethod(name, bf)) == null && (t = t.BaseType) != null) ;
        return fi;
    }

    public static bool IsAssignableTo(this Type type, Type assignableType)
    {
        return assignableType.IsAssignableFrom(type);
    }

    public static bool IsAssignableTo<TAssignable>(this Type type)
    {
        return IsAssignableTo(type, typeof(TAssignable));
    }
}
