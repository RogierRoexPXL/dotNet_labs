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

        public async Task AssembleDevelopersAsyncFor(Team team, int requiredNumberOfDevelopers)
        {
            var rnd = new Random();
            IReadOnlyList<Developer> developers = await _developerRepository.FindDevelopersWithoutATeamAsync();
            var freeDevelopers = developers;
            freeDevelopers = freeDevelopers
                .OrderBy(_ => rnd.Next())
                .ToList();

            int counter = requiredNumberOfDevelopers > freeDevelopers.Count
                ? freeDevelopers.Count
                : requiredNumberOfDevelopers;

            for (var i = 0; i < counter; i++)
            {
                team.Join(freeDevelopers.ElementAt(i));
            }

            await _developerRepository.CommitTrackedChangesAsync();
        }
    }
}
