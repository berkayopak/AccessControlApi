using AccessControlApi.Domain;
using AccessControlApi.Dto.Source;
using Microsoft.EntityFrameworkCore;

namespace AccessControlApi.Repository;

public class DoorRepository : IDoorRepository
{
    private readonly ApplicationDbContext _context;

    public DoorRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Door> Add(Door door)
    {
        var result = (await _context.Doors.AddAsync(door)).Entity;
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<DoorRole> AssignRole(DoorRole doorRole)
    {
        var res = (await _context.DoorRoles.AddAsync(doorRole)).Entity;
        await _context.SaveChangesAsync();

        return res;
    }

    public async Task<bool> IsDoorRoleExists(DoorRole doorRole) =>
        await _context.DoorRoles.AnyAsync(dr => dr.RoleId == doorRole.RoleId && dr.DoorId == doorRole.DoorId);

    public async Task<bool> HasDoorAnyMatchingRole(string doorId, IReadOnlyCollection<string> roleList) =>
        await _context.DoorRoles.AnyAsync(dr => dr.DoorId == doorId && roleList.Contains(dr.Role!.Name));

    public async Task<Door?> Find(string doorId) =>
        await _context.Doors.FindAsync(doorId);

    //P.S: Actually we can move DoorEventHistory related functions to its own repository.
    //But I thought it would be simpler to keep it on the same repository,
    //since we don't have too many operations about DoorEventHistory in this project.
    public async Task<DoorEventHistory> AddEventToHistory(DoorEventHistory doorEventHistory)
    {
        var result = (await _context.DoorEventHistory.AddAsync(doorEventHistory)).Entity;
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<int> GetDoorEventHistoryCount(GetDoorEventHistoryFilter filter) =>
        await GetDoorEventHistoryBaseQuery(filter)
            .CountAsync();

    public async Task<IReadOnlyCollection<DoorEventHistory>> GetDoorEventHistory(GetDoorEventHistoryFilter filter) =>
        await GetDoorEventHistoryBaseQuery(filter)
            .Skip(filter.Skip)
            .Take(filter.Take)
            .ToListAsync();

    private IQueryable<DoorEventHistory> GetDoorEventHistoryBaseQuery(GetDoorEventHistoryFilter filter) =>
        _context.DoorEventHistory
            .Where(deh => filter.DoorIds == null || filter.DoorIds.Contains(deh.DoorId))
            .Where(deh => filter.UserIds == null || filter.UserIds.Contains(deh.UserId))
            .Where(deh => filter.DoorEventType == null || deh.EventType == filter.DoorEventType)
            .Where(deh => filter.Succeeded == null || deh.Succeeded == filter.Succeeded)
            .Where(deh => filter.StartDate == null || deh.AttemptedAt >= filter.StartDate)
            .Where(deh => filter.EndDate == null || deh.AttemptedAt <= filter.EndDate)
            .Include(deh => deh.User)
            .Include(deh => deh.Door)
            .OrderByDescending(deh => deh.AttemptedAt);
}