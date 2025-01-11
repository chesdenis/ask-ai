using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services;
using AskAI.Services.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AskAI.UnitTests;

public class PromptEnricherTests
{
    [Fact]
    public async Task EnrichAsync_MustProduceCorrectResults()
    {
        // Arrange
        var workSpaceContext = Substitute.For<IWorkSpaceContext>();
        workSpaceContext.GetTagsAsync("test-file.md").Returns(Task.FromResult(new[]
            { "c#", "cloud", "story" }));

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IWorkSpaceContext, IWorkSpaceContext>(
                        x => workSpaceContext);
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
                role = "user",
                content = "Some additional response"
            }
        ], "test-file.md");

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