namespace HumanResources.Domain
{
    public interface IEmployee
    {
        EmployeeNumber Number { get; }
        string FirstName { get; }
        string LastName { get; }
        DateTime StartDate { get; }
        DateTime? EndDate { get; }

        void Dismiss(bool withNotice = true);
    }
}