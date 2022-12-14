using Domain;

namespace HumanResources.Domain
{
    internal class Employee : Entity, IEmployee
    {
        public EmployeeNumber Number { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        private Employee()
        {
            Number = new EmployeeNumber(DateTime.MinValue, 1);
            LastName = string.Empty;
            FirstName = string.Empty;
        }

        protected override IEnumerable<object> GetIdComponents()
        {
            yield return Number;
            //yield return Number.ToString(); beter?
        }

        public void Dismiss(bool withNotice = true)
        {
            if (withNotice)
            {
                Contracts.Require(EndDate == null, "Employee cannot have already received a notice of dismissal");
                if (StartDate > DateTime.Now.AddMonths(-3))
                {
                    EndDate = DateTime.Now.AddDays(7);
                }
                else if (StartDate > DateTime.Now.AddYears(-1))
                {
                    EndDate = DateTime.Now.AddDays(14);
                } else
                {
                    EndDate = DateTime.Now.AddDays(28);
                }
            } 
            else
            {
                EndDate = DateTime.Now;
            }
        }

        internal class Factory : IEmployeeFactory
        {
            public IEmployee CreateNew(string lastName, string firstName, DateTime startDate, int sequence)
            {
                Contracts.Require(startDate >= DateTime.Now.AddYears(-1), "The start date of an employee cannot be more than 1 year in the past");
                Contracts.Require(!string.IsNullOrEmpty(lastName), "The last name of an employee cannot be empty");
                Contracts.Require(lastName.Length >= 2, "The last name of an employee must at least have 2 characters");
                Contracts.Require(!string.IsNullOrEmpty(firstName), "The first name of an employee cannot be empty");
                Contracts.Require(firstName.Length >= 2, "The first name of an employee must at least have 2 characters");

                var employee = new Employee
                {
                    Number = new EmployeeNumber(startDate, sequence),
                    FirstName = firstName,
                    LastName = lastName,
                    StartDate = startDate,
                    EndDate = null
                };
                return employee;
            }
        }
    }
}
