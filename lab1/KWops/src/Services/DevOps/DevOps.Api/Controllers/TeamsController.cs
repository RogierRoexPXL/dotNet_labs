using AutoMapper;
using DevOps.Api.Models;
using DevOps.AppLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public TeamsController(ITeamService teamService, ITeamRepository teamRepository, IMapper mapper)
        {
            _teamService = teamService;
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IReadOnlyList<Team> allFreeDevelopers = await _teamRepository.GetAllAsync();
            List<TeamDetailModel> teamDetailModelList = new List<TeamDetailModel>();
            foreach (Team t in allFreeDevelopers)
            {
                teamDetailModelList.Add(_mapper.Map<TeamDetailModel>(t));
            }
            return Ok(teamDetailModelList);
        }

        [HttpPost("{id}/assemble")]
        [Authorize(policy:"write")]
        public async Task<IActionResult> AssembleTeam(Guid id, TeamAssembleInputModel model)
        {
            Team? team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            else
            {
                await _teamService.AssembleDevelopersAsyncFor(team, model.RequiredNumberOfDevelopers);
                var outputModel = _mapper.Map<TeamDetailModel>(team);
                
                return Ok();
            }
        }
    }
}
