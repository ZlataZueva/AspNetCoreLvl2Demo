using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ASPNETCoreLvl2Demo.Identity
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<CustomUser>
    {
        public CustomClaimsPrincipalFactory(UserManager<CustomUser> userManager, IOptions<IdentityOptions> optionsAccessor) 
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(CustomUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            if (user.FavoriteMusician != null)
            {
                identity.AddClaim(new Claim("FavoriteMusician", user.FavoriteMusician));
            }

            return identity;
        }
    }
}