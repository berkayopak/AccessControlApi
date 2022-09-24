using Microsoft.AspNetCore.Identity;

namespace AccessControlApi.Domain;

public class Role : IdentityRole
{
    public ICollection<DoorRole>? DoorRoles { get; set; }

    //Added for migration-tool, it needs empty constructor
    public Role()
    {
    }
    public Role(string roleName) : base(roleName)
    { 
    }
}