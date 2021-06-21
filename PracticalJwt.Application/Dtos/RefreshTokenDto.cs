using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PracticalJwt.Application.Dtos
{
    public class RefreshTokenDto
    {
        [JsonProperty("accessToken")]
        [Required]
        public string AccessToken { get; set; }

        [JsonProperty("refreshToken")]
        [Required]
        public string RefreshToken { get; set; }
    }
}
