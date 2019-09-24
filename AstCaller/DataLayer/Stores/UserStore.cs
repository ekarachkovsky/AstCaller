using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AstCaller.Models;
using AstCaller.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AstCaller.DataLayer.Stores
{
    public class UserStore : IUserStore<UserModel>, IUserPasswordStore<UserModel>
    {
        private readonly MainContext _context;

        public UserStore(MainContext context)
        {
            _context = context;
        }

        public Task<IdentityResult> CreateAsync(UserModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(UserModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public async Task<UserModel> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var id))
            {
                throw new ArgumentException(nameof(userId));
            }

            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Id==id);

            return new UserModel
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Login = user.Login,
                Name = user.Fullname,
                Password = user.Password
            };
        }

        public async Task<UserModel> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedUserName))
            {
                throw new ArgumentNullException(nameof(normalizedUserName));
            }

            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Login == normalizedUserName);

            return new UserModel
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Login = user.Login,
                Name = user.Fullname,
                Password = user.Password
            };
        }

        public Task<string> GetNormalizedUserNameAsync(UserModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(UserModel user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Password);
        }

        public Task<string> GetUserIdAsync(UserModel user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(UserModel user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Fullname);
        }

        public Task<bool> HasPasswordAsync(UserModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(UserModel user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(UserModel user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(UserModel user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(UserModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
