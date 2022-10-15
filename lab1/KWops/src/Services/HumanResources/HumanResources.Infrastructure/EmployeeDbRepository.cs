using HumanResources.AppLogic;
using HumanResources.Domain;
using Microsoft.EntityFrameworkCore;

namespace HumanResources.Infrastructure
{
    internal class EmployeeDbRepository : IEmployeeRepository
    {
        private readonly HumanResourcesContext _context;

        public EmployeeDbRepository(HumanResourcesContext context)
        {
            _context = context;
        }

        public async Task AddAsync(IEmployee newEmployee)
        {
            await _context.AddAsync(newEmployee);
            await _context.SaveChangesAsync();
        }

        public Task CommitTrackedChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<IEmployee> GetByNumberAsync(EmployeeNumber number)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Number == number);
            return employee;
        }

        public async Task<int> GetNumberOfStartersOnAsync(DateTime startDate)
        {
            var count = await _context.Employees.Where(e => e.StartDate.Date == startDate.Date).CountAsync();
            return count;
        }
    }
}
