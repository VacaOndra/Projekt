using Microsoft.AspNetCore.Identity;

namespace Pojisteni.Models
{
    public class AccountIdentity : IdentityUser
    {
        public int? InsuredID { get; set; }

        public virtual Insured Insured { get; set; }

    }
}
