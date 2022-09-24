using Microsoft.AspNetCore.Identity;

namespace AccessControlApi.Domain;

public class User : IdentityUser
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<DoorEventHistory>? DoorEventHistory { get; set; }
}