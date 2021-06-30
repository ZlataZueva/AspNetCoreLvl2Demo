using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ASPNETCoreLvl2Demo.Identity
{
    public class VaccinationRequirementHandler : AuthorizationHandler<VaccinationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, VaccinationRequirement requirement)
        {
            if (context.User.HasClaim(
                c => c.Type == "VaccinationCertificateId" && c.Issuer == "http://ministryofhealth"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}