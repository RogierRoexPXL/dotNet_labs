using DevOps.Domain;

namespace DevOps.AppLogic
{
    internal class TeamService : ITeamService
    {
        IDeveloperRepository _developerRepository;

        public TeamService(IDeveloperRepository developerRepository)
        {
            _developerRepository = developerRepository;
        }

        public Task AssembleDevelopersAsyncFor(Team team, int requiredNumberOfDevelopers)
        {
            var rnd = new Random();
            var freeDevelopers = _developerRepository.FindDevelopersWithoutATeamAsync().Result
                .OrderBy(_ => rnd.Next())
                .ToList();

            int counter = requiredNumberOfDevelopers > freeDevelopers.Count
                ? freeDevelopers.Count
                : requiredNumberOfDevelopers;

            for (var i = 0; i < counter; i++)
            {
                team.Join(freeDevelopers.ElementAt(i));
            }

            return _developerRepository.CommitTrackedChangesAsync();
        }
    }
}
