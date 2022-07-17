using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MokoPages
{
    public enum MokoCommand
    {
        IMGS = 300,
        SYNCIMGS = 301, 
    }

    public class MokoRequest
    {
        public string? Token { get; set; }
        public MokoCommand? MokoCommand { get; set; }
        public Dictionary<string, string>? MokoCommandParameters { get; set; }

        public async Task<ValidatedMokoUserResponse> ValidateToken(string authUrl, string authApiUrl)
        {
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>
                    (authUrl, new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
            TokenValidationParameters validationParameters =
                new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = openIdConfig.Issuer,
                    ValidAudience = authApiUrl,
                    ValidateAudience = true,
                    IssuerSigningKeys = openIdConfig.SigningKeys, //should aim to provide a collection of keys here, keys is allowed
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = false
                };
            JwtSecurityTokenHandler handler = new();
            handler.ValidateToken(Token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var valMokoUserResp = new ValidatedMokoUserResponse(jwtToken);
            return valMokoUserResp;
        }
    }
}
