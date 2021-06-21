using Newtonsoft.Json;

namespace PracticalJwt.Application.Dtos
{
    public class ResponseDto
    {
        [JsonProperty("succeed")]
        public bool Succeed { get; set; }

        [JsonProperty("result")]
        public object Result { get; set; }
    }
}
