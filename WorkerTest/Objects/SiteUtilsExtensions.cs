using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MokoPages;
using MudBlazor;
using Newtonsoft.Json;
using System.Text;

namespace WorkerTest.Objects
{
    public static class SiteUtilsExtensions
    {
        public static async Task<MokoRequest> GetMokoRequestAsync(this IAccessTokenProvider tokenProvider, MokoCommand command)
        {
            var tokenResult = await tokenProvider.RequestAccessToken(new AccessTokenRequestOptions { Scopes = new List<string> { "openid", "profile" } });
            tokenResult.TryGetToken(out var token);

            return new MokoRequest
            {
                Token = token?.Value,
                MokoCommand = command
            };
        }

        public static ByteArrayContent ToJsonByteArrayContent(this MokoRequest request)
        {
            var responseBody = JsonConvert.SerializeObject(request);
            return new ByteArrayContent(Encoding.UTF8.GetBytes(responseBody));
        }

        public static void PrintSnackbar(this ISnackbar snackbar, string message)
        {
            snackbar.Clear();
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            snackbar.Add(message, Severity.Normal, config => { config.HideIcon = true; });
        }
    }
}
