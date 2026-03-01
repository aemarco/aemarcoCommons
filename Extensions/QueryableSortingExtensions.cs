using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable enable

namespace aemarcoCommons.Extensions;

public interface IOrdering
{
    bool Descending { get; }
}
public interface IPropertyOrdering : IOrdering
{
    string? OrderBy { get; }
}
public interface IFuncOrdering<T> : IOrdering
{
    Expression<Func<T, object>> Selector { get; }
}

public interface IMultiOrdering
{
    IEnumerable<IOrdering>? Orderings { get; }
}

public record PropertyOrder(string OrderBy, bool Descending) : IPropertyOrdering;
public record FuncOrder<T>(Expression<Func<T, object>> Selector, bool Descending) : IFuncOrdering<T>;




public static class QueryableSortingExtensions
{

    public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, IOrdering? ordering)
    {
        var rules = new List<IOrdering>();
        if (ordering is not null)
            rules.Add(ordering);
        return ApplyOrderingInternal(query, rules);
    }
    public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, IMultiOrdering? orderings)
    {
        List<IOrdering> rules = [.. orderings?.Orderings ?? []];
        return ApplyOrderingInternal(query, rules);
    }
    public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, IEnumerable<IOrdering>? orderings)
    {
        List<IOrdering> rules = [.. orderings ?? []];
        return ApplyOrderingInternal(query, rules);
    }

    //mixed
    private static IQueryable<T> ApplyOrderingInternal<T>(IQueryable<T> query, List<IOrdering> rules)
    {
        rules.RemoveAll(x => x is IPropertyOrdering { OrderBy: null });

        //allow for random ordering, which takes precedence
        if (rules.Any(x =>
                x is IPropertyOrdering propOrder &&
                string.Equals(propOrder.OrderBy, nameof(Random), StringComparison.OrdinalIgnoreCase)))
        {
            return query.OrderBy(_ => Guid.NewGuid());
        }

        //when no ordering, we inject default ordering on Id if supported
        if (rules.Count == 0 &&
            typeof(T).GetProperty(
                    "Id",
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                is { } idInfo)
        {
            rules.Add(new PropertyOrder(idInfo.Name, false));
        }

        IOrderedQueryable<T>? orderedQuery = null;
        foreach (var rule in rules)
        {
            var isFirst = orderedQuery is null;
            var usedQuery = orderedQuery ?? query;
            orderedQuery = rule switch
            {
                IPropertyOrdering p => (IOrderedQueryable<T>)ApplyPropertyOrder(usedQuery, p, isFirst),
                IFuncOrdering<T> f => ApplyFuncOrder(usedQuery, f, isFirst),
                _ => orderedQuery
            };
        }
        return orderedQuery ?? query;
    }

    //prop
    private static IQueryable<T> ApplyPropertyOrder<T>(IQueryable<T> query, IPropertyOrdering order, bool isFirst)
    {
        var entityType = typeof(T);

        //exception for unsupported properties
        if (entityType.GetProperty(
                order.OrderBy!,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
            is not { } propInfo)
            throw new NotSupportedException($"Ordering by '{order.OrderBy}' is not supported on type {typeof(T).Name}");

        // Build: x => x.PropName
        var arg = Expression.Parameter(entityType, "x");
        var property = Expression.Property(arg, propInfo);
        var selector = Expression.Lambda(property, arg);

        // Determine method: OrderBy/ThenBy etc.
        var methodName = isFirst
            ? (order.Descending ? "OrderByDescending" : "OrderBy")
            : (order.Descending ? "ThenByDescending" : "ThenBy");

        // Reflection to find the generic method on Queryable
        var method = GetCachedMethod(entityType, propInfo.PropertyType, methodName);

        return (IQueryable<T>)method.Invoke(null, [query, selector])!;
    }
    private static readonly ConcurrentDictionary<string, MethodInfo> MethodCache = new();
    private static MethodInfo GetCachedMethod(Type type, Type propType, string methodName)
    {
        var key = $"{methodName}:{type.AssemblyQualifiedName}:{propType.AssemblyQualifiedName}";
        return MethodCache.GetOrAdd(key, _ =>
            typeof(Queryable)
                .GetMethods()
                .Single(m => m.Name == methodName &&
                             m.IsGenericMethodDefinition &&
                             m.GetParameters().Length == 2)
                .MakeGenericMethod(type, propType)
        );
    }

    //func
    private static IOrderedQueryable<T> ApplyFuncOrder<T>(IQueryable<T> query, IFuncOrdering<T> order, bool isFirst)
    {
        var selector = order.Selector;

        return isFirst
            ? order.Descending
                ? query.OrderByDescending(selector)
                : query.OrderBy(selector)
            : order.Descending
                ? ((IOrderedQueryable<T>)query).ThenByDescending(selector)
                : ((IOrderedQueryable<T>)query).ThenBy(selector);
    }

}