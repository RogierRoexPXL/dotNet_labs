using AppLogic.Events;
using HumanResources.AppLogic.Events;
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
        private readonly IEventBus _eventBus;

        public EmployeeService(IEmployeeRepository employeeRepository, IEmployeeFactory employeeFactory, IEventBus eventBus)
        {
            _employeeRepository = employeeRepository;
            _employeeFactory = employeeFactory;
            _eventBus = eventBus;
        }

        public async Task DismissAsync(EmployeeNumber employeeNumber, bool withNotice)
        {
            IEmployee employee = await _employeeRepository.GetByNumberAsync(employeeNumber);
            employee.Dismiss(withNotice);
            await _employeeRepository.CommitTrackedChangesAsync();
        }

        public async Task<IEmployee> HireNewAsync(string lastName, string firstName, DateTime startDate)
        {
            int sequence = await _employeeRepository.GetNumberOfStartersOnAsync(startDate) + 1;
            IEmployee hiredEmployee = _employeeFactory.CreateNew(lastName, firstName, startDate, sequence);
            await _employeeRepository.AddAsync(hiredEmployee);

            var @event = new EmployeeHiredIntegrationEvent
            {
                Number = hiredEmployee.Number,
                LastName = hiredEmployee.LastName,
                FirstName = hiredEmployee.FirstName,
            };
            _eventBus.Publish(@event);

            return hiredEmployee;
        }
    }
}
