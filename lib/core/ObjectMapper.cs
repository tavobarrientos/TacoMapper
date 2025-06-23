namespace TacoMapper.Core;

/// <summary>
/// Static entry point for creating mappers
/// </summary>
public static class ObjectMapper
{
    /// <summary>
    /// Creates a new mapper from TSource to TDestination
    /// </summary>
    /// <typeparam name="TSource">Source object type</typeparam>
    /// <typeparam name="TDestination">Destination object type</typeparam>
    /// <returns>A new mapper instance</returns>
    public static IMapper<TSource, TDestination> Create<TSource, TDestination>()
        where TDestination : new()
    {
        return new Mapper<TSource, TDestination>();
    }

    /// <summary>
    /// Creates and executes a simple mapping without configuration
    /// </summary>
    /// <typeparam name="TSource">Source object type</typeparam>
    /// <typeparam name="TDestination">Destination object type</typeparam>
    /// <param name="source">Source object to map from</param>
    /// <returns>Mapped destination object</returns>
    public static TDestination Map<TSource, TDestination>(TSource source)
        where TDestination : new()
    {
        return Create<TSource, TDestination>().MapFrom(source);
    }

    /// <summary>
    /// Creates and executes a simple mapping on a collection without configuration
    /// </summary>
    /// <typeparam name="TSource">Source object type</typeparam>
    /// <typeparam name="TDestination">Destination object type</typeparam>
    /// <param name="sources">Source objects to map from</param>
    /// <returns>Mapped destination objects</returns>
    public static IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> sources)
        where TDestination : new()
    {
        return Create<TSource, TDestination>().MapFrom(sources);
    }
}
