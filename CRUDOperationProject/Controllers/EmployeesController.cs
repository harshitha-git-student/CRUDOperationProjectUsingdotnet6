using CRUDOperationProject.Data;
using CRUDOperationProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CRUDOperationProject.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            if (_dbContext.Employees == null)
            {
                return NotFound();
            }
            return await _dbContext.Employees.ToListAsync();
        }

        [HttpGet ("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            if(_dbContext.Employees == null) 
            {
                    return NotFound($"The Employee with id {id} not found");  
            }
            var employee = await _dbContext.Employees.FindAsync(id);
            if(employee == null)
            {
                return NotFound();
            }
            return employee;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
           
            _dbContext.Employees.Add(employee);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        [HttpPut]
        [Route("{id:int}", Name = "UpdateById")]
        public async Task<ActionResult> PutEmployee(int id, Employee employee)
        {
            if(id != employee.Id)
            {
                return BadRequest();
            }
            _dbContext.Entry(employee).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!EmployeeAvailable(id))
                {
                    return NotFound($"The Employee with id {id} not found in database. Update failed.");
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }
        private bool EmployeeAvailable(int id)
        {
            return (_dbContext.Employees?.Any(x => x.Id == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}", Name = "Delete")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            if(_dbContext.Employees == null)
            {
                return NotFound(id);
            }
            var employee = await _dbContext.Employees.FindAsync(id);
            if(employee == null)
            {
                return NotFound($"The Employee with id {id} not found in database. Delete failed");
            }
            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
