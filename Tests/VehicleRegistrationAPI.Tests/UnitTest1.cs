using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Threading.Tasks;
using Xunit;

public class VehicleApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public VehicleApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetVehicles_ShouldReturnOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/vehicles");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
