using DevOps.AppLogic;
using Microsoft.EntityFrameworkCore;

namespace DevOps.Infrastructure
{
    public class TeamDbRepository : ITeamRepository
    {
        DevOpsContext _context;

        public TeamDbRepository(DevOpsContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Team>> GetAllAsync()
        {
            return await _context.Teams.Include(t => t.Developers).ToListAsync();
        }

        public async Task<Team?> GetByIdAsync(Guid teamId)
        {
            return await _context.Teams.Include(t => t.Developers).FirstOrDefaultAsync(t => t.Id == teamId);
        }
    }
}
