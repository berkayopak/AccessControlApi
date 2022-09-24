using AccessControlApi.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8618

namespace AccessControlApi.Repository;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Door>()
        .HasKey(door => door.Id);

        builder.Entity<DoorRole>().HasKey(dr => new { dr.DoorId, dr.RoleId });

        builder.Entity<DoorRole>()
            .HasOne(dr => dr.Door)
            .WithMany(d => d.DoorRoles)
            .HasForeignKey(dr => dr.DoorId);

        builder.Entity<DoorRole>()
            .HasOne(dr => dr.Role)
            .WithMany(r => r.DoorRoles)
            .HasForeignKey(dr => dr.RoleId);

        builder.Entity<DoorEventHistory>()
            .HasKey(doorEventHistory => doorEventHistory.Id);

        builder.Entity<DoorEventHistory>()
            .HasOne(deh => deh.Door)
            .WithMany(d => d.DoorEventHistory)
            .HasForeignKey(deh => deh.DoorId);

        builder.Entity<DoorEventHistory>()
            .HasOne(deh => deh.User)
            .WithMany(u => u.DoorEventHistory)
            .HasForeignKey(deh => deh.UserId);

        base.OnModelCreating(builder);
    }

    public DbSet<Door> Doors { get; set; }
    public DbSet<DoorRole> DoorRoles { get; set; }
    public DbSet<DoorEventHistory> DoorEventHistory { get; set; }
}