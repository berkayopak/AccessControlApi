using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessControlApi.Domain;

public class DoorEventHistory
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string DoorId { get; set; } = null!;
    public Door? Door { get; set; }
    public string UserId { get; set; } = null!;
    public User? User { get; set; }
    public DoorEventType EventType { get; set; }
    public bool Succeeded { get; set; }
    public string? Information { get; set; }
    public DateTime AttemptedAt { get; set; }
}

public enum DoorEventType
{
    [Display(Name = "Open")] Open = 1
}