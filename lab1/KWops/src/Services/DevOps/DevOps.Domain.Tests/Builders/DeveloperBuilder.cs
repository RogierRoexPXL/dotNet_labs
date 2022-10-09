using Test;

namespace DevOps.Domain.Tests.Builders
{
    public class DeveloperBuilder : BuilderBase<Developer>
    {
        public DeveloperBuilder()
        {
            Item = Developer.CreateNew(Random.NextString(), Random.NextString(), Random.NextString(),
                Random.NextDouble());
        }

        public DeveloperBuilder WithoutTeam()
        {
            SetProperty(e => e.TeamId, null);
            return this;
        }

        public DeveloperBuilder WithTeam(Team team)
        {
            SetProperty(e => e.TeamId, team.Id);
            return this;
        }
    }
}