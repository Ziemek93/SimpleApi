using Microsoft.AspNetCore.Identity;

namespace AuthApi.Models.Entities;

public class ApplicationUser : IdentityUser
{ 
    public int ClientId { get; set; }
}