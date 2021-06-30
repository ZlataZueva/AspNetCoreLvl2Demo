using Microsoft.AspNetCore.Authorization;

namespace ASPNETCoreLvl2Demo.Identity
{
    public class VaccinationRequirement : IAuthorizationRequirement
    {
        public string VaccineName { get; set; }


        public VaccinationRequirement(string vaccineName)
        {
            VaccineName = vaccineName;
        }
    }
}