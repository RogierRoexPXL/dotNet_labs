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

        public async Task AddAsync(Employee newEmployee)
        {
            _context.Add(newEmployee);
            await _context.SaveChangesAsync();
        }

        public async Task<Employee> GetByNumberAsync(string number)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Number == number);
            return employee;
        }
    }
}
