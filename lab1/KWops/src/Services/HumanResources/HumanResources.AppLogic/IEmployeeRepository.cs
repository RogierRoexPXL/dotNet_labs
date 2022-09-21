using HumanResources.Domain;

namespace HumanResources.AppLogic
{
    public interface IEmployeeRepository
    {
        Task AddAsync(Employee newEmployee);
        Task<Employee> GetByNumberAsync(string number);
    }
}
