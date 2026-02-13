namespace Insurance.UserService.Endpoints.Api.Data;

using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    public string MobileNumber { get; set; } = default!;
}