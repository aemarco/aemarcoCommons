using System;
// ReSharper disable ClassNeverInstantiated.Global

namespace aemarcoCommons.WebTools.Cors;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class CorsOriginAttribute : Attribute
{
    public string Origin { get; }

    public CorsOriginAttribute(string origin)
    {
        Origin = origin;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class CorsHeaderAttribute : Attribute
{
    public string Header { get; }

    public CorsHeaderAttribute(string header)
    {
        Header = header;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class CorsMethodAttribute : Attribute
{
    public string Method { get; }

    public CorsMethodAttribute(string method)
    {
        Method = method;

    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class CorsPreflightMaxAgeAttribute : Attribute
{
    public int PreflightMaxAge { get; }

    public CorsPreflightMaxAgeAttribute(int preflightMaxAge)
    {
        PreflightMaxAge = preflightMaxAge;
    }
}