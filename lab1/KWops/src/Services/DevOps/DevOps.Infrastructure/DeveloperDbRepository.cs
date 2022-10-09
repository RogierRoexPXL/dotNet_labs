using DevOps.AppLogic;
using DevOps.Domain;
using Microsoft.EntityFrameworkCore;

namespace DevOps.Infrastructure
{
    public class DeveloperDbRepository : IDeveloperRepository
    {
        private DevOpsContext _context;

        public DeveloperDbRepository(DevOpsContext context)
        {
            _context = context;
        }

        public Task CommitTrackedChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Developer>> FindDevelopersWithoutATeamAsync()
        {
            return await _context.Developers.Where(d => d.TeamId == null).ToListAsync();
        }
    }
}
