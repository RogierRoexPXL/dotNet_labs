using HumanResources.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.AppLogic
{
    internal class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeFactory _employeeFactory;

        public EmployeeService(IEmployeeRepository employeeRepository, IEmployeeFactory employeeFactory)
        {
            _employeeRepository = employeeRepository;
            _employeeFactory = employeeFactory;
        }

        public Task DismissAsync(EmployeeNumber employeeNumber, bool withNotice)
        {
            IEmployee employee = _employeeRepository.GetByNumberAsync(employeeNumber).Result;
            employee.Dismiss(withNotice);
            return _employeeRepository.CommitTrackedChangesAsync();
        }

        public Task<IEmployee> HireNewAsync(string lastName, string firstName, DateTime startDate)
        {
            var sequence = _employeeRepository.GetNumberOfStartersOnAsync(startDate).Result + 1;
            IEmployee hiredEmployee = _employeeFactory.CreateNew(lastName, firstName, startDate, sequence);
            _employeeRepository.AddAsync(hiredEmployee);

            return Task.FromResult(hiredEmployee);
        }
    }
}
