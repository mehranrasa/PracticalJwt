using Microsoft.EntityFrameworkCore;
using PracticalJwt.Application.Data;
using PracticalJwt.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PracticalJwt.Application.Repositories
{
    public interface IUserRepository
    {
        Task<User> Create(User user);

        Task<bool> Delete(User user);
        Task<bool> Delete(int userID);

        Task<User> Get(int userID);
        Task<User> Get(string username, bool loadRefreshTokens = false);

        Task<int> GetUserAge(int userId);
        Task<int> GetUserAge(string username);

        Task<bool> UpdateUserRefreshToken(User user, string refreshToken, DateTime expiresAt);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task<User> Create(User user)
        {
            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
            return user;

        }

        public async Task<bool> Delete(User user)
        {
            _appDbContext.Users.Remove(user);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int userId)
        {
            _appDbContext.Users.Remove(
                await _appDbContext.Users.Where(u => u.ID == userId).FirstOrDefaultAsync()
            );
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<User> Get(int userId)
        {
            return await _appDbContext.Users.Where(u => u.ID == userId).FirstOrDefaultAsync();
        }

        public async Task<User> Get(string username, bool loadRefreshTokens = false)
        {
            var query = _appDbContext.Users.Where(u => u.Username.ToLower().Equals(username.ToLower()));
            if (loadRefreshTokens)
                query = query.Include(p => p.RefreshToken);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetUserAge(int userId)
        {
            var user = await _appDbContext.Users.Where(u => u.ID == userId).FirstOrDefaultAsync();
            return user.Age;
        }

        public async Task<int> GetUserAge(string username)
        {
            var user = await _appDbContext.Users.Where(u =>
                u.Username.ToLower().Equals(username.ToLower())).FirstOrDefaultAsync();
            return user.Age;
        }

        public async Task<bool> UpdateUserRefreshToken(User user, string newRefreshToken, DateTime expiresAt)
        {
            var lastRefreshToken = await _appDbContext.RefreshTokens.SingleOrDefaultAsync(r => r.UserId == user.ID);

            if (lastRefreshToken == null)
                lastRefreshToken = new RefreshToken()
                {
                    Token = newRefreshToken,
                    ExpiresAt = expiresAt,
                    UserId = user.ID
                };
            else
            {
                lastRefreshToken.Token = newRefreshToken;
                lastRefreshToken.ExpiresAt = expiresAt;
            }

            user.RefreshToken = lastRefreshToken;
            await _appDbContext.SaveChangesAsync();

            return true;
        }
    }
}
