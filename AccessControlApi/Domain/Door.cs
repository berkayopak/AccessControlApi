using System.ComponentModel.DataAnnotations.Schema;

namespace AccessControlApi.Domain;

public class Door
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<DoorRole>? DoorRoles { get; set; }
    public ICollection<DoorEventHistory>? DoorEventHistory { get; set; }
}