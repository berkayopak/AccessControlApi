namespace AccessControlApi.Domain;

public class DoorRole
{
    public string RoleId { get; set; } = null!;
    public Role? Role { get; set; }
    public string DoorId { get; set; } = null!;
    public Door? Door { get; set; }
}