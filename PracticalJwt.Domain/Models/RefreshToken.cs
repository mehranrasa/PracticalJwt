using System;

namespace PracticalJwt.Domain.Models
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        //1:1 fk
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
