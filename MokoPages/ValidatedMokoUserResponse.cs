using System.IdentityModel.Tokens.Jwt;

namespace MokoPages
{
    public class ValidatedMokoUserResponse
    {
        public JwtSecurityToken Token { get; set; }

        public ValidatedMokoUserResponse(JwtSecurityToken jwtSecurityToken)
        {
            Token = jwtSecurityToken;
        }
    }
}
