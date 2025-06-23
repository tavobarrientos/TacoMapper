using TacoMapper.Core;
using TacoMapper.Example.Models;


// Top-level statements - no Program class or Main method needed
Console.WriteLine("🌮 Taco Mapper Demo");
Console.WriteLine("===================");

// Sample data
List<Person> people =
        [
            new()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1990, 5, 15),
                Email = "john.doe@example.com",
                IsActive = true,
                Salary = 75000.50m
            },
            new()
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                DateOfBirth = new DateTime(1985, 8, 22),
                Email = "jane.smith@example.com",
                IsActive = false,
                Salary = 68000.75m
            }
        ];

// 1. Simple auto-mapping
Console.WriteLine("\n1. Simple Auto-Mapping:");
var simpleResult = ObjectMapper.Map<Person, PersonDto>(people.First());
Console.WriteLine($"   ID: {simpleResult.Id}, Email: {simpleResult.Email}");

// 2. Fluent mapping with all features
Console.WriteLine("\n2. Fluent Mapping with Custom Logic:");
var mapper = ObjectMapper.Create<Person, PersonDto>()
    .Map(dest => dest.Id, src => src.Id)
    .Map(dest => dest.Email, src => src.Email)
    .Map(dest => dest.Age, src => src.DateOfBirth, dob => DateTime.Now.Year - dob.Year)
    .Map(dest => dest.Status, src => src.IsActive, active => active ? "✅ Active" : "❌ Inactive")
    .MapIf(dest => dest.SalaryFormatted,
           src => src.Salary,
           salary => $"💰 ${salary:N2}",
           src => src.IsActive) // Only show salary if active
    .Combine(dest => dest.FullName, src => $"👤 {src.FirstName} {src.LastName}");

var results = mapper.MapFrom(people);

foreach (var result in results)
{
    Console.WriteLine($"   {result.FullName}");
    Console.WriteLine($"   Age: {result.Age}, Status: {result.Status}");
    Console.WriteLine($"   Email: {result.Email}");
    Console.WriteLine($"   Salary: {(string.IsNullOrEmpty(result.SalaryFormatted) ? "Hidden" : result.SalaryFormatted)}");
    Console.WriteLine();
}

// 3. Conditional mapping example
Console.WriteLine("3. Conditional Mapping Example:");
var conditionalMapper = ObjectMapper.Create<Person, PersonDto>()
    .Combine(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
    .MapIf(dest => dest.Status,
           src => src.IsActive,
           active => "VIP Member",
           src => src.Salary > 70000) // Only VIP if salary > 70k
    .Map(dest => dest.Email, src => src.Email);

Console.WriteLine("   VIP Status based on salary:");
foreach (var person in people)
{
    var result = conditionalMapper.MapFrom(person);
    Console.WriteLine($"   {result.FullName}: {(string.IsNullOrEmpty(result.Status) ? "Regular Member" : result.Status)}");
}

Console.WriteLine("\n🌮 Demo completed!");
