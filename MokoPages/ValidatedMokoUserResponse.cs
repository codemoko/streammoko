using Google.Cloud.BigQuery.V2;
using System.IdentityModel.Tokens.Jwt;

namespace MokoPages
{
    public class ValidatedMokoUserResponse
    {
        public JwtSecurityToken Token { get; set; }
        public BigQueryClient BigQueryClient { get; set; }

        public ValidatedMokoUserResponse(JwtSecurityToken jwtSecurityToken, BigQueryClient bigQueryClient)
        {
            Token = jwtSecurityToken;
            BigQueryClient = bigQueryClient;
        }
    }
}
