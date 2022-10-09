using System;

namespace HumanResources.Domain
{
    public interface IEmployeeFactory
    {
        IEmployee CreateNew(string lastName, string firstName, DateTime startDate, int sequence);
    }
}
