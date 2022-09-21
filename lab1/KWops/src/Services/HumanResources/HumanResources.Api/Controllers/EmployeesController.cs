using HumanResources.AppLogic;
using HumanResources.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResources.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet("{number}")]
        public async Task<IActionResult> Get(string number)
        {
            var employee = await _employeeRepository.GetByNumberAsync(number);

            if (employee == null) return NotFound();

            return Ok(employee);
        }

        [HttpPost]
        //public async Task<IActionResult> Post([FromBody] Employee newEmployee)
        public async Task<IActionResult> Post([FromBody] Employee newEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _employeeRepository.AddAsync(newEmployee);

            //TODO fullfill acceptance criteria
            string url =$"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}/{newEmployee.Number}"; ;
            return Created(url, newEmployee);
        }
    }
}
