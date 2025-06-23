using System.Linq.Expressions;

namespace TacoMapper.Core;

/// <summary>
/// Fluent mapper interface for configuring object mapping
/// </summary>
/// <typeparam name="TSource">Source object type</typeparam>
/// <typeparam name="TDestination">Destination object type</typeparam>
public interface IMapper<TSource, TDestination>
{
    /// <summary>
    /// Maps a source property to a destination property
    /// </summary>
    IMapper<TSource, TDestination> Map<TProp>(
        Expression<Func<TDestination, TProp>> destinationProperty,
        Expression<Func<TSource, TProp>> sourceProperty);

    /// <summary>
    /// Maps a source property to a destination property with a custom transformation
    /// </summary>
    IMapper<TSource, TDestination> Map<TSrcProp, TDestProp>(
        Expression<Func<TDestination, TDestProp>> destinationProperty,
        Expression<Func<TSource, TSrcProp>> sourceProperty,
        Func<TSrcProp, TDestProp> transform);

    /// <summary>
    /// Conditionally maps a property based on a predicate
    /// </summary>
    IMapper<TSource, TDestination> MapIf<TProp>(
        Expression<Func<TDestination, TProp>> destinationProperty,
        Expression<Func<TSource, TProp>> sourceProperty,
        Func<TSource, bool> condition);

    /// <summary>
    /// Conditionally maps a property with transformation based on a predicate
    /// </summary>
    IMapper<TSource, TDestination> MapIf<TSrcProp, TDestProp>(
        Expression<Func<TDestination, TDestProp>> destinationProperty,
        Expression<Func<TSource, TSrcProp>> sourceProperty,
        Func<TSrcProp, TDestProp> transform,
        Func<TSource, bool> condition);

    /// <summary>
    /// Combines multiple source properties into a single destination property
    /// </summary>
    IMapper<TSource, TDestination> Combine<TDestProp>(
        Expression<Func<TDestination, TDestProp>> destinationProperty,
        Func<TSource, TDestProp> combineFunction);

    /// <summary>
    /// Ignores a destination property (won't be mapped)
    /// </summary>
    IMapper<TSource, TDestination> Ignore<TProp>(
        Expression<Func<TDestination, TProp>> destinationProperty);

    /// <summary>
    /// Executes the mapping on a source object
    /// </summary>
    TDestination MapFrom(TSource source);

    /// <summary>
    /// Executes the mapping on a collection of source objects
    /// </summary>
    IEnumerable<TDestination> MapFrom(IEnumerable<TSource> sources);
}
