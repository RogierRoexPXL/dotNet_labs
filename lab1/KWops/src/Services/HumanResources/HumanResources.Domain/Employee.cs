namespace HumanResources.Domain
{
    public class Employee
    {
        public string? Number { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate{ get; set; }

    }
}
