using System.Net.Http.Json;
using System.Text.Json;
using GameStore.Application.DTOs.UserDtos;
using GameStore.Domain.Settings;
using Microsoft.Extensions.Options;

namespace GameStore.Application.Services.UserServices;
public class ExternalAuthService(
    IHttpClientFactory httpClientFactory,
    IOptions<AuthServiceSettings> options) : IExternalAuthService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<AuthResult> AuthenticateAsync(LoginDto login)
    {
        var client = httpClientFactory.CreateClient();

        var authRequest = new AuthDto
        {
            Email = login.Login,
            Password = login.Password,
        };

        var response = await client.PostAsJsonAsync(options.Value.AuthenticationUri, authRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var successResult = JsonSerializer.Deserialize<AuthSuccessResponse>(
                responseContent, JsonOptions);
            return new AuthResult { Success = true, User = successResult };
        }

        var errorResult = JsonSerializer.Deserialize<AuthErrorResponse>(
            responseContent, JsonOptions);
        return new AuthResult { Success = false, Error = errorResult };
    }
}
