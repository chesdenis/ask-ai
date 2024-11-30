using AITextWriter.Infrastructure.Abstractions;
using AITextWriter.Model;
using AITextWriter.Services.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AITextWriter.Services.Tests;

public class PromptEnricherTests
{
    [Fact]
    public async Task EnrichAsync_MustProduceCorrectResults()
    {
        // Arrange
        var textModelContextProvider = Substitute.For<ITextModelContextProvider>();
        textModelContextProvider.GetTagsAsync().Returns(Task.FromResult(new[]
            { "c#", "cloud", "story" }));

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<ITextModelContextProvider, ITextModelContextProvider>(
                        x => textModelContextProvider);
                    return x;
                }
            )
            .GetService<IPromptEnricher>();

        var result = await sut.EnrichAsync([
            new Prompt
            {
                role = "user",
                content = "write me hello world app"
            },
            new Prompt()
            {
                role = "assistant",
                content = "Some response"
            }
        ]);

        // Assert
        result.Should().HaveCount(2);
        result[0].content.Should().Be("Please use this context: c#, cloud, story.\nwrite me hello world app");
    }

    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IPromptEnricher, PromptEnricher>();
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }
}