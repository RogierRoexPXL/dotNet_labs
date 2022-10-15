using DevOps.Domain;
using Domain;

public class Team : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private readonly List<Developer> _developers;
    public IReadOnlyList<Developer> Developers => _developers;

    private Team(Guid id, string name)
    {
        Id = id;
        Name = name;
        _developers = new List<Developer>();
    }

    public static Team CreateNew(string name)
    {
        Contracts.Require(!String.IsNullOrEmpty(name), "The name of a new team can not be empty!");
        return new Team(Guid.NewGuid(), name);
    }

    public void Join(Developer developer)
    {
        Contracts.Require(!_developers.Contains(developer), $"Developer {developer.Id} is already part of this team!");
        _developers.Add(developer);
        developer.TeamId = Id;
    }

    protected override IEnumerable<object> GetIdComponents()
    {
        yield return Id;
    }
}