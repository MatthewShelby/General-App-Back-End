using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doctors;
using Doctors.Data;

namespace Doctors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SerductController(ApplicationDbContext context)
        {
            _context = context;
        }

         
        [HttpGet("get-my-serducts/{companyId}")]
        public async Task<ActionResult<IEnumerable<Serduct>>> GetMySerducts( string companyId)
        {
            List<Serduct> serducts = await _context.Serducts.Where(s => s.CompanyId == companyId).ToListAsync();
            return new  JsonResult(new {  data = serducts });
        }

        [HttpGet("get-serduct-by-id/{id}")]
        public async Task<ActionResult<Serduct>> GetSerductBtId(string id)
        {
            var serduct = await _context.Serducts.FindAsync(id);

            if (serduct == null)
            {
                return NotFound();
            }

            return serduct;
        }

        // PUT: api/Serduct/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSerduct(string id, Serduct serduct)
        {
            if (id != serduct.Id)
            {
                return BadRequest();
            }

            _context.Entry(serduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SerductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpPost("new-serduct")]
        public async Task<ActionResult<Serduct>> PostSerduct(Serduct serduct)
        {
            serduct.Id = Guid.NewGuid().ToString();
            serduct.Company = _context.Companies.Find(serduct.CompanyId);
            _context.Serducts.Add(serduct);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SerductExists(serduct.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSerduct", new { id = serduct.Id }, serduct);
        }

        // DELETE: api/Serduct/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Serduct>> DeleteSerduct(string id)
        {
            var serduct = await _context.Serducts.FindAsync(id);
            if (serduct == null)
            {
                return NotFound();
            }

            _context.Serducts.Remove(serduct);
            await _context.SaveChangesAsync();

            return serduct;
        }

        private bool SerductExists(string id)
        {
            return _context.Serducts.Any(e => e.Id == id);
        }
    }
}
