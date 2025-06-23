using TacoMapper.Core;
using Xunit;

namespace TacoMapper.Tests;

public class MapperTests
{
    [Fact]
    public void BasicMapping_ShouldMapMatchingProperties()
    {
        // Arrange
        var person = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act - Simple auto-mapping
        var result = ObjectMapper.Map<Person, PersonDto>(person);

        // Assert
        Assert.Equal(person.Id, result.Id);
        Assert.Equal(person.Email, result.Email);
    }

    [Fact]
    public void FluentMapping_ShouldMapWithCustomTransformations()
    {
        // Arrange
        var person = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 5, 15),
            Email = "john.doe@example.com",
            IsActive = true,
            Salary = 75000.50m
        };

        // Act - Fluent mapping with transformations
        var mapper = ObjectMapper.Create<Person, PersonDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Age, src => src.DateOfBirth, dob => DateTime.Now.Year - dob.Year)
            .Map(dest => dest.Status, src => src.IsActive, active => active ? "Active" : "Inactive")
            .Map(dest => dest.SalaryFormatted, src => src.Salary, salary => $"${salary:N2}")
            .Combine(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");

        var result = mapper.MapFrom(person);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("john.doe@example.com", result.Email);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal(DateTime.Now.Year - 1990, result.Age);
        Assert.Equal("Active", result.Status);
        Assert.Equal("$75,000.50", result.SalaryFormatted);
    }

    [Fact]
    public void ConditionalMapping_ShouldMapBasedOnCondition()
    {
        // Arrange
        var activePerson = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            IsActive = true,
            Salary = 75000m
        };

        var inactivePerson = new Person
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
            IsActive = false,
            Salary = 60000m
        };

        // Act
        var mapper = ObjectMapper.Create<Person, PersonDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Combine(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
            .MapIf(dest => dest.SalaryFormatted, src => src.Salary, 
                   salary => $"${salary:N2}", 
                   src => src.IsActive)
            .Map(dest => dest.Status, src => src.IsActive, active => active ? "Active" : "Inactive");

        var activeResult = mapper.MapFrom(activePerson);
        var inactiveResult = mapper.MapFrom(inactivePerson);

        // Assert
        Assert.Equal("$75,000.00", activeResult.SalaryFormatted);
        Assert.Equal("Active", activeResult.Status);
        
        Assert.Empty(inactiveResult.SalaryFormatted); // Should not be mapped due to condition
        Assert.Equal("Inactive", inactiveResult.Status);
    }

    [Fact]
    public void CombineMapping_ShouldCombineMultipleSourceProperties()
    {
        // Arrange
        var address = new Address
        {
            Street = "123 Main St",
            City = "Anytown",
            ZipCode = "12345"
        };

        // Act
        var mapper = ObjectMapper.Create<Address, AddressDto>()
            .Map(dest => dest.Street, src => src.Street)
            .Map(dest => dest.City, src => src.City)
            .Map(dest => dest.ZipCode, src => src.ZipCode)
            .Combine(dest => dest.FormattedAddress, 
                    src => $"{src.Street}, {src.City} {src.ZipCode}");

        var result = mapper.MapFrom(address);

        // Assert
        Assert.Equal("123 Main St", result.Street);
        Assert.Equal("Anytown", result.City);
        Assert.Equal("12345", result.ZipCode);
        Assert.Equal("123 Main St, Anytown 12345", result.FormattedAddress);
    }

    [Fact]
    public void IgnoreMapping_ShouldSkipIgnoredProperties()
    {
        // Arrange
        var person = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var mapper = ObjectMapper.Create<Person, PersonDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Ignore(dest => dest.Email) // Ignore email mapping
            .Combine(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");

        var result = mapper.MapFrom(person);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.FullName);
        Assert.Empty(result.Email); // Should be empty since it's ignored
    }

    [Fact]
    public void CollectionMapping_ShouldMapCollections()
    {
        // Arrange
        var people = new List<Person>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", IsActive = true },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", IsActive = false }
        };

        // Act
        var mapper = ObjectMapper.Create<Person, PersonDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Combine(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
            .Map(dest => dest.Status, src => src.IsActive, active => active ? "Active" : "Inactive");

        var results = mapper.MapFrom(people).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal("John Doe", results[0].FullName);
        Assert.Equal("Active", results[0].Status);
        Assert.Equal("Jane Smith", results[1].FullName);
        Assert.Equal("Inactive", results[1].Status);
    }

    [Fact]
    public void SimpleStaticMapping_ShouldWorkWithoutConfiguration()
    {
        // Arrange
        var address = new Address
        {
            Street = "123 Main St",
            City = "Anytown",
            ZipCode = "12345"
        };

        // Act - Using static method for simple mapping
        var result = ObjectMapper.Map<Address, AddressDto>(address);

        // Assert - Only matching property names should be mapped
        Assert.Equal("123 Main St", result.Street);
        Assert.Equal("Anytown", result.City);
        Assert.Equal("12345", result.ZipCode);
        Assert.Empty(result.FormattedAddress); // Not mapped automatically
    }
}
