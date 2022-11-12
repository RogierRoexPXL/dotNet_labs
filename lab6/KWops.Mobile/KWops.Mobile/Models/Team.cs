namespace KWops.Mobile.Models;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IList<Developer> Developers { get; set; }
}