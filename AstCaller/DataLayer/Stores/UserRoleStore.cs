using System;
using System.Threading;
using System.Threading.Tasks;
using AstCaller.Models;
using Microsoft.AspNetCore.Identity;

namespace AstCaller.DataLayer.Stores
{
    public class UserRoleStore : IRoleStore<UserRoleModel>
    {
        private bool _disposed;

        public Task<IdentityResult> CreateAsync(UserRoleModel role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(UserRoleModel role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserRoleModel> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserRoleModel> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(UserRoleModel role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(UserRoleModel role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(UserRoleModel role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(UserRoleModel role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(UserRoleModel role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(UserRoleModel role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // There's nothing to dispose here
            }

            _disposed = true;
        }

        ~UserRoleStore()
        {
            Dispose(false);
        }
    }
}
