# Taco Mapper

A very lightweight C# object mapper with a fluent API that supports basic mapping, conditional mapping, and combine mapping. The library is designed to be readable, small, and efficient.

## Features

- 🌮 **Basic Property Mapping** - Auto-map properties with matching names and types
- 🌯 **Custom Transformations** - Transform values during mapping
- 🥙 **Conditional Mapping** - Map properties based on conditions
- 🌮 **Combine Mapping** - Combine multiple source properties into one destination property
- 🔥 **Fluent API** - Chain mapping configurations for readability
- 📦 **Collection Support** - Map collections of objects
- 🚫 **Ignore Properties** - Skip specific properties during mapping
- ⚡ **Lightweight** - Minimal dependencies and small footprint

## Quick Start

### Basic Auto-Mapping

```csharp
// Simple mapping for properties with matching names and types
var personDto = ObjectMapper.Map<Person, PersonDto>(person);
```

### Fluent API with Custom Mapping

```csharp
var mapper = ObjectMapper.Create<Person, PersonDto>()
    .Map(dest => dest.Id, src => src.Id)
    .Map(dest => dest.Email, src => src.Email)
    .Map(dest => dest.Age, src => src.DateOfBirth, dob => DateTime.Now.Year - dob.Year)
    .Map(dest => dest.Status, src => src.IsActive, active => active ? "Active" : "Inactive")
    .Combine(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");

var result = mapper.MapFrom(person);
```

### Conditional Mapping

```csharp
var mapper = ObjectMapper.Create<Person, PersonDto>()
    .Map(dest => dest.Id, src => src.Id)
    .MapIf(dest => dest.SalaryFormatted, 
           src => src.Salary, 
           salary => $"${salary:N2}", 
           src => src.IsActive) // Only map salary if person is active
    .Combine(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");
```

### Combine Multiple Properties

```csharp
var mapper = ObjectMapper.Create<Address, AddressDto>()
    .Map(dest => dest.Street, src => src.Street)
    .Map(dest => dest.City, src => src.City)
    .Map(dest => dest.ZipCode, src => src.ZipCode)
    .Combine(dest => dest.FormattedAddress, 
             src => $"{src.Street}, {src.City} {src.ZipCode}");
```

### Ignore Properties

```csharp
var mapper = ObjectMapper.Create<Person, PersonDto>()
    .Map(dest => dest.Id, src => src.Id)
    .Ignore(dest => dest.Email) // Skip email mapping
    .Combine(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");
```

### Collection Mapping

```csharp
var people = new List<Person> { /* ... */ };

// Using configured mapper
var mapper = ObjectMapper.Create<Person, PersonDto>()
    .Map(dest => dest.Id, src => src.Id)
    .Combine(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");

var results = mapper.MapFrom(people);

// Or simple auto-mapping
var simpleResults = ObjectMapper.Map<Person, PersonDto>(people);
```

## API Reference

### ObjectMapper Static Methods

- `Create<TSource, TDestination>()` - Creates a new mapper instance
- `Map<TSource, TDestination>(source)` - Simple auto-mapping for single object
- `Map<TSource, TDestination>(sources)` - Simple auto-mapping for collections

### IMapper Interface

- `Map(dest, src)` - Basic property mapping
- `Map(dest, src, transform)` - Property mapping with transformation
- `MapIf(dest, src, condition)` - Conditional property mapping
- `MapIf(dest, src, transform, condition)` - Conditional mapping with transformation
- `Combine(dest, combineFunction)` - Combine multiple source properties
- `Ignore(dest)` - Ignore a destination property
- `MapFrom(source)` - Execute mapping on single object
- `MapFrom(sources)` - Execute mapping on collection

## How It Works

The mapper uses:
- **Reflection** for auto-mapping properties with matching names and types
- **Expression trees** for fluent property selection
- **Compiled delegates** for efficient property access and transformations
- **Dictionary-based caching** for mapping configurations
- **Clean architecture** with organized core components

## Performance Considerations

- Mappers are reusable - create once, use many times
- Compiled expressions provide good performance after first use
- Auto-mapping uses reflection, so explicit mapping is faster for hot paths
- Small memory footprint with minimal allocations

## Requirements

- .NET 8.0 or later
- C# 12.0 language features

## License

MIT License
