namespace DevOps.AppLogic
{
    public interface ITeamRepository
    {
        Task<IReadOnlyList<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(Guid teamId);
    }
}