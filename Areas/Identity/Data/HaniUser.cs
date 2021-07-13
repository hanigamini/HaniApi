using Microsoft.AspNetCore.Identity;

namespace HaniApi.Areas.Identity.Data
{
	public class HaniUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
