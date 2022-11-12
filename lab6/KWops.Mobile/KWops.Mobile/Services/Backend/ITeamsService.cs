using KWops.Mobile.Models;

namespace KWops.Mobile.Services.Backend;

public interface ITeamsService
{
    Task<IReadOnlyList<Team>> GetAllTeamsAsync();
}