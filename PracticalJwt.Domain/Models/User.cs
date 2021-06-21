namespace PracticalJwt.Domain.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public int Age { get; set; }

        public RefreshToken RefreshToken { get; set; }
    }
}
