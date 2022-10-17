using AutoMapper;
using HumanResources.Api.Models;
using HumanResources.AppLogic;
using HumanResources.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResources.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeesController(IEmployeeRepository employeeRepository, IEmployeeService employeeService, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _employeeService = employeeService;
            _mapper = mapper;
        }

        [HttpGet("{number}")]
        public async Task<IActionResult> GetByNumber(string number)
        {
            IEmployee? employee = await _employeeRepository.GetByNumberAsync(number);
            return employee == null ? NotFound() : Ok(_mapper.Map<EmployeeDetailModel>(employee));
        }

        [HttpPost]
        [Authorize(policy:"write")]
        public async Task<IActionResult> Add(EmployeeCreateModel model)
        {
            IEmployee hiredEmployee = await _employeeService.HireNewAsync(model.LastName, model.FirstName, model.StartDate);
            var outputModel = _mapper.Map<EmployeeDetailModel>(hiredEmployee);
            return CreatedAtAction(nameof(GetByNumber), new { number = outputModel.Number }, outputModel);
        }

        [HttpPost("{number}/dismiss")]
        public async Task<IActionResult> Dismiss(string number, [FromQuery] bool withNotice = true)
        {
            await _employeeService.DismissAsync(number, withNotice);
            return Ok();
        }
    }
}
