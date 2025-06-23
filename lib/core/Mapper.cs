using System.Linq.Expressions;
using System.Reflection;

namespace TacoMapper.Core;

/// <summary>
/// Lightweight object mapper with fluent API
/// </summary>
/// <typeparam name="TSource">Source object type</typeparam>
/// <typeparam name="TDestination">Destination object type</typeparam>
public class Mapper<TSource, TDestination> : IMapper<TSource, TDestination>
    where TDestination : new()
{
    private readonly Dictionary<string, PropertyMapping> _mappings = [];
    private readonly HashSet<string> _ignoredProperties = [];

    public static IMapper<TSource, TDestination> Create() => new Mapper<TSource, TDestination>();

    public IMapper<TSource, TDestination> Map<TProp>(
        Expression<Func<TDestination, TProp>> destinationProperty,
        Expression<Func<TSource, TProp>> sourceProperty)
    {
        var destPropName = GetPropertyName(destinationProperty);
        var srcFunc = sourceProperty.Compile();
        
        _mappings[destPropName] = new PropertyMapping
        {
            MapFunction = src => srcFunc((TSource)src),
            Condition = null
        };
        
        return this;
    }

    public IMapper<TSource, TDestination> Map<TSrcProp, TDestProp>(
        Expression<Func<TDestination, TDestProp>> destinationProperty,
        Expression<Func<TSource, TSrcProp>> sourceProperty,
        Func<TSrcProp, TDestProp> transform)
    {
        var destPropName = GetPropertyName(destinationProperty);
        var srcFunc = sourceProperty.Compile();
        
        _mappings[destPropName] = new PropertyMapping
        {
            MapFunction = src => transform(srcFunc((TSource)src)),
            Condition = null
        };
        
        return this;
    }

    public IMapper<TSource, TDestination> MapIf<TProp>(
        Expression<Func<TDestination, TProp>> destinationProperty,
        Expression<Func<TSource, TProp>> sourceProperty,
        Func<TSource, bool> condition)
    {
        var destPropName = GetPropertyName(destinationProperty);
        var srcFunc = sourceProperty.Compile();
        
        _mappings[destPropName] = new PropertyMapping
        {
            MapFunction = src => srcFunc((TSource)src),
            Condition = src => condition((TSource)src)
        };
        
        return this;
    }

    public IMapper<TSource, TDestination> MapIf<TSrcProp, TDestProp>(
        Expression<Func<TDestination, TDestProp>> destinationProperty,
        Expression<Func<TSource, TSrcProp>> sourceProperty,
        Func<TSrcProp, TDestProp> transform,
        Func<TSource, bool> condition)
    {
        var destPropName = GetPropertyName(destinationProperty);
        var srcFunc = sourceProperty.Compile();
        
        _mappings[destPropName] = new PropertyMapping
        {
            MapFunction = src => transform(srcFunc((TSource)src)),
            Condition = src => condition((TSource)src)
        };
        
        return this;
    }

    public IMapper<TSource, TDestination> Combine<TDestProp>(
        Expression<Func<TDestination, TDestProp>> destinationProperty,
        Func<TSource, TDestProp> combineFunction)
    {
        var destPropName = GetPropertyName(destinationProperty);
        
        _mappings[destPropName] = new PropertyMapping
        {
            MapFunction = src => combineFunction((TSource)src),
            Condition = null
        };
        
        return this;
    }

    public IMapper<TSource, TDestination> Ignore<TProp>(
        Expression<Func<TDestination, TProp>> destinationProperty)
    {
        var destPropName = GetPropertyName(destinationProperty);
        _ignoredProperties.Add(destPropName);
        return this;
    }

    public TDestination MapFrom(TSource source)
    {
        if (source == null) 
            throw new ArgumentNullException(nameof(source));

        var destination = new TDestination();
        var destType = typeof(TDestination);
        var srcType = typeof(TSource);

        // Apply explicit mappings
        foreach (var mapping in _mappings)
        {
            var destProperty = destType.GetProperty(mapping.Key);
            if (destProperty?.CanWrite == true)
            {
                if (mapping.Value.Condition == null || mapping.Value.Condition(source))
                {
                    var value = mapping.Value.MapFunction(source);
                    destProperty.SetValue(destination, value);
                }
            }
        }

        // Auto-map remaining properties with matching names and types
        var destProperties = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && !_mappings.ContainsKey(p.Name) && !_ignoredProperties.Contains(p.Name));

        foreach (var destProp in destProperties)
        {
            var srcProp = srcType.GetProperty(destProp.Name, BindingFlags.Public | BindingFlags.Instance);
            if (srcProp?.CanRead == true && AreTypesCompatible(srcProp.PropertyType, destProp.PropertyType))
            {
                var value = srcProp.GetValue(source);
                destProp.SetValue(destination, value);
            }
        }

        return destination;
    }

    public IEnumerable<TDestination> MapFrom(IEnumerable<TSource> sources)
    {
        return sources?.Select(MapFrom) ?? [];
    }

    private static string GetPropertyName<T>(Expression<T> expression)
    {
        return expression.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression member2 } => member2.Member.Name,
            _ => throw new ArgumentException("Expression must be a property accessor", nameof(expression))
        };
    }

    private static bool AreTypesCompatible(Type sourceType, Type destinationType)
    {
        if (sourceType == destinationType) return true;
        if (destinationType.IsAssignableFrom(sourceType)) return true;
        
        // Handle nullable types
        var srcUnderlyingType = Nullable.GetUnderlyingType(sourceType);
        var destUnderlyingType = Nullable.GetUnderlyingType(destinationType);
        
        if (srcUnderlyingType != null && destUnderlyingType != null)
            return srcUnderlyingType == destUnderlyingType;
        
        if (srcUnderlyingType != null)
            return srcUnderlyingType == destinationType;
            
        if (destUnderlyingType != null)
            return sourceType == destUnderlyingType;

        return false;
    }

    private class PropertyMapping
    {
        public required Func<object, object?> MapFunction { get; init; }
        public Func<object, bool>? Condition { get; init; }
    }
}
