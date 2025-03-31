using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubstanceSafe.Models;
using SubstanceSafe.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubstanceSafe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubstanceUsagesController : ControllerBase
    {
        private readonly SubstancesDbContext _context;

        public SubstanceUsagesController(SubstancesDbContext context)
        {
            _context = context;
        }

        // GET: api/SubstanceUsages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubstanceUsage>>> GetSubstanceUsages(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? substanceTypeId,
            [FromQuery] int? categoryId)
        {
            var query = _context.SubstanceUsages
                .Include(u => u.SubstanceType)
                    .ThenInclude(st => st!.Category) // Include Category via SubstanceType
                .AsQueryable();

            // Apply filters
            if (startDate.HasValue)
            {
                query = query.Where(u => u.UsageDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                // Add 1 day to endDate to include the whole day
                query = query.Where(u => u.UsageDate < endDate.Value.AddDays(1));
            }
            if (substanceTypeId.HasValue)
            {
                query = query.Where(u => u.SubstanceTypeId == substanceTypeId.Value);
            }
             if (categoryId.HasValue)
            {
                query = query.Where(u => u.SubstanceType != null && u.SubstanceType.CategoryId == categoryId.Value);
            }

            // Order by most recent first
            return await query.OrderByDescending(u => u.UsageDate).ToListAsync();
        }

        // GET: api/SubstanceUsages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubstanceUsage>> GetSubstanceUsage(int id)
        {
            var substanceUsage = await _context.SubstanceUsages
                .Include(u => u.SubstanceType)
                    .ThenInclude(st => st!.Category)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (substanceUsage == null)
            {
                return NotFound($"Substance usage record with ID {id} not found.");
            }

            return substanceUsage;
        }

        // PUT: api/SubstanceUsages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubstanceUsage(int id, SubstanceUsage substanceUsage)
        {
            if (id != substanceUsage.Id)
            {
                return BadRequest("ID in URL must match ID in body.");
            }

            // Validate SubstanceTypeId
            bool typeExists = await _context.SubstanceTypes.AnyAsync(t => t.Id == substanceUsage.SubstanceTypeId);
            if (!typeExists)
            {
                return BadRequest($"Invalid SubstanceTypeId: {substanceUsage.SubstanceTypeId}. Substance type does not exist.");
            }

             if (substanceUsage.Amount <= 0)
            {
                 return BadRequest("Amount must be positive.");
            }
             if (string.IsNullOrWhiteSpace(substanceUsage.Unit))
            {
                 return BadRequest("Unit cannot be empty.");
            }


            _context.Entry(substanceUsage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubstanceUsageExists(id))
                {
                    return NotFound($"Substance usage record with ID {id} not found.");
                }
                else
                {
                    throw;
                }
            }
             catch (DbUpdateException ex)
            {
                 return Problem(
                    detail: $"An error occurred while updating the database: {ex.InnerException?.Message ?? ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Database Update Error");
            }

            return NoContent();
        }

        // POST: api/SubstanceUsages
        [HttpPost]
        public async Task<ActionResult<SubstanceUsage>> PostSubstanceUsage(SubstanceUsage substanceUsage)
        {
            // Validate SubstanceTypeId
            bool typeExists = await _context.SubstanceTypes.AnyAsync(t => t.Id == substanceUsage.SubstanceTypeId);
            if (!typeExists)
            {
                return BadRequest($"Invalid SubstanceTypeId: {substanceUsage.SubstanceTypeId}. Substance type does not exist.");
            }

             if (substanceUsage.Amount <= 0)
            {
                 return BadRequest("Amount must be positive.");
            }
             if (string.IsNullOrWhiteSpace(substanceUsage.Unit))
            {
                 return BadRequest("Unit cannot be empty.");
            }

            // Ensure UsageDate is set (can default in model or here)
            if (substanceUsage.UsageDate == default)
            {
                substanceUsage.UsageDate = DateTime.UtcNow;
            }


            _context.SubstanceUsages.Add(substanceUsage);
            try
            {
                await _context.SaveChangesAsync();
            }
             catch (DbUpdateException ex)
            {
                 return Problem(
                    detail: $"An error occurred while saving to the database: {ex.InnerException?.Message ?? ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Database Save Error");
            }

            // Load related data before returning
             await _context.Entry(substanceUsage)
                .Reference(u => u.SubstanceType)
                .Query() // Necessary to use ThenInclude on a Reference
                .Include(st => st.Category)
                .LoadAsync();


            return CreatedAtAction(nameof(GetSubstanceUsage), new { id = substanceUsage.Id }, substanceUsage);
        }

        // DELETE: api/SubstanceUsages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubstanceUsage(int id)
        {
            var substanceUsage = await _context.SubstanceUsages.FindAsync(id);
            if (substanceUsage == null)
            {
                return NotFound($"Substance usage record with ID {id} not found.");
            }

            _context.SubstanceUsages.Remove(substanceUsage);
            try
            {
                await _context.SaveChangesAsync();
            }
             catch (DbUpdateException ex)
            {
                 return Problem(
                    detail: $"An error occurred while deleting from the database: {ex.InnerException?.Message ?? ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Database Delete Error");
            }

            return NoContent();
        }

        private bool SubstanceUsageExists(int id)
        {
            return _context.SubstanceUsages.Any(e => e.Id == id);
        }
    }
}
