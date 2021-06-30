using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace ASPNETCoreLvl2Demo.Identity
{
    public class CustomUserStore : IUserPasswordStore<CustomUser>
    {
        private readonly IList<CustomUser> _users;
        private const string UsersFilePath = "users.json";


        public CustomUserStore()
        {
            _users = File.Exists(UsersFilePath) ? JsonConvert.DeserializeObject<List<CustomUser>>(File.ReadAllText(UsersFilePath)) : new List<CustomUser>();
        }


        public void Dispose()
        {
            File.WriteAllText(UsersFilePath, JsonConvert.SerializeObject(_users));
        }

        public Task<IdentityResult> CreateAsync(CustomUser user, CancellationToken cancellationToken)
        {
           _users.Add(user);

           return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(_users.Remove(_users.FirstOrDefault(u => u.Id == user.Id)) ? IdentityResult.Failed() : IdentityResult.Success);
        }

        public Task<CustomUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));
        }

        public Task<CustomUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName));
        }

        public Task<string> GetNormalizedUserNameAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(CustomUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(CustomUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;

            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(CustomUser user, CancellationToken cancellationToken)
        {
            _users.Remove(_users.FirstOrDefault(u => u.Id == user.Id));
            _users.Add(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetPasswordHashAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(CustomUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }
    }
}