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

        public async Task<Developer> GetByIdAsync(string number)
        {
            return await _context.Developers.FirstOrDefaultAsync(d => d.Id == number);
        }

        public async Task AddAsync(Developer developer)
        {
            await _context.Developers.AddAsync(developer);
            await CommitTrackedChangesAsync();
        }
    }
}
