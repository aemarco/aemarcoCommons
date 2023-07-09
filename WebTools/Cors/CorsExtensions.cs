using aemarcoCommons.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System;
using System.Linq;

namespace aemarcoCommons.WebTools.Cors;

public static class CorsExtensions
{
    public static CorsOptions AddCorsPolicies(this CorsOptions config, Type policyContainer)
    {
        foreach (var entry in policyContainer.GetFields())
        {
            var policyName = (string)entry.GetValue(null)!;
            var origins = policyContainer
                .GetAttributes<CorsOriginAttribute>(entry.Name)
                .Select(x => x.Origin)
                .ToArray();
            var headers = policyContainer
                .GetAttributes<CorsHeaderAttribute>(entry.Name)
                .Select(x => x.Header)
                .ToArray();
            var methods = policyContainer
                .GetAttributes<CorsMethodAttribute>(entry.Name)
                .Select(x => x.Method)
                .ToArray();
            var preflight = policyContainer
                .GetAttribute<CorsPreflightMaxAgeAttribute>(entry.Name)?
                .PreflightMaxAge;


            config.AddPolicy(policyName, builder =>
            {
                //origins
                if (origins.Length > 0)
                    builder.WithOrigins(origins);
                else
                    builder.AllowAnyOrigin();

                //header
                if (headers.Length > 0)
                    builder.WithHeaders(headers);
                else
                    builder.AllowAnyHeader();

                //methods
                if (methods.Length > 0)
                    builder.WithMethods(methods);
                else
                    builder.AllowAnyMethod();

                //preflight-cache
                if (preflight.HasValue)
                    builder.SetPreflightMaxAge(TimeSpan.FromSeconds(preflight.Value));

            });
        }
        return config;
    }
}