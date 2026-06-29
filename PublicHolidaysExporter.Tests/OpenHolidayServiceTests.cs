using System.Net;
using PublicHolidaysExporter.Services;

namespace PublicHolidaysExporter.Tests;

public class OpenHolidaysServiceTests
{
    [Fact]
    public async Task GetPublicHolidaysAsync_ShouldUseRequestedLanguageHolidayName_WhenAvailable()
    {
        var jsonResponse = """
        [
            {
                "startDate": "2026-01-01",
                "name": [
                    {
                        "language": "EN",
                        "text": "New Year's Day"
                    },
                    {
                        "language": "MT",
                        "text": "L-Ewwel tas-Sena"
                    }
                ]
            }
        ]
        """;

        var service = CreateService(jsonResponse);

        var result = await service.GetPublicHolidaysAsync(
            "MT",
            "EN",
            new DateTime(2026, 1, 1),
            new DateTime(2026, 12, 31)
        );

        Assert.Single(result);
        Assert.Equal("New Year's Day", result[0].Name);
        Assert.Equal("MT", result[0].CountryCode);
        Assert.Equal("EN", result[0].Language);
        Assert.Equal(new DateTime(2026, 1, 1), result[0].Date);
    }

    [Fact]
    public async Task GetPublicHolidaysAsync_ShouldReturnUnknownHoliday_WhenNoHolidayNameExists()
    {
        var jsonResponse = """
        [
            {
                "startDate": "2026-01-01",
                "name": []
            }
        ]
        """;

        var service = CreateService(jsonResponse);

        var result = await service.GetPublicHolidaysAsync(
            "MT",
            "EN",
            new DateTime(2026, 1, 1),
            new DateTime(2026, 12, 31)
        );

        Assert.Single(result);
        Assert.Equal("Unknown Holiday", result[0].Name);
    }

    private static OpenHolidaysService CreateService(string jsonResponse)
    {
        var handler = new FakeHttpMessageHandler(jsonResponse);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://openholidaysapi.org/")
        };

        return new OpenHolidaysService(httpClient);
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _jsonResponse;

        public FakeHttpMessageHandler(string jsonResponse)
        {
            _jsonResponse = jsonResponse;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_jsonResponse)
            };

            response.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return Task.FromResult(response);
        }
    }
}