using AITextWriter.Model;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AITextWriter.Services.Tests;

public class AskPromptGeneratorTests
{
    [Theory]
    [InlineData(new string[] { "" }, new string[] { }, new string[]{""})]
    [InlineData(new[] { "user prompt 1" }, new string[] { }, new[]{"user prompt 1"})]
    [InlineData(new[] { "user prompt 1", "user prompt 2"  }, new string[] { }, new[]{"user prompt 1", "user prompt 2"})]
    [InlineData(new[] { ""  }, new string[] { "answer 1" }, new[]{"answer 1"})]
    [InlineData(new[] { "A", "C", "E" }, new string[] { "A", "B", "C", "D" }, new[]{"A", "B", "C", "D", "E"})]
    public async Task GenerateAskPromptAsync_ShouldReturnExpectedResults(string[] userInput,
        string[] assistantAndUserInput, string[] expectedOutput)
    {
        // Arrange
        var userPrompts = userInput.Select(s => new Prompt() { role = "user", content = s }).ToArray();

        var assistantPrompts = new List<Prompt>();
        for (var i = 0; i < assistantAndUserInput.Length; i++)
        {
            assistantPrompts.Add(i % 2 == 0
                ? new Prompt() { role = "assistant", content = assistantAndUserInput[i] }
                : new Prompt() { role = "user", content = assistantAndUserInput[i] });
        }

        // Act
        var sut = BuildServices().GetService<IAskPromptGenerator>();
        var result = await sut.GenerateAskPromptAsync(userPrompts, assistantPrompts.ToArray());

        // Assert
        for (int i = 0; i < result.Length; i++)
        {
            result[i].content.Should().Be(expectedOutput[i]);
        }
    }

    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IAskPromptGenerator, AskPromptGenerator>();
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }
}