using Microsoft.AspNetCore.Identity;

namespace ASPNETCoreLvl2Demo.Identity
{
    public class CustomUser : IdentityUser
    {
        public string FavoriteMusician { get; set; }
    }
}