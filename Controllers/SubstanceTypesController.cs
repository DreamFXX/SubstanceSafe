using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubstanceSafe.Models;
using SubstanceSafe.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubstanceSafe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubstanceTypesController : ControllerBase
    {
        private readonly SubstancesDbContext _context;

        public SubstanceTypesController(SubstancesDbContext context)
        {
            _context = context;
        }

        // GET: api/SubstanceTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubstanceType>>> GetSubstanceTypes([FromQuery] int? categoryId)
        {
            var query = _context.SubstanceTypes.Include(t => t.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            return await query.OrderBy(t => t.Category!.Name).ThenBy(t => t.Name).ToListAsync();
        }

        // GET: api/SubstanceTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubstanceType>> GetSubstanceType(int id)
        {
            var substanceType = await _context.SubstanceTypes
                                            .Include(t => t.Category) // Include category details
                                            .FirstOrDefaultAsync(t => t.Id == id);

            if (substanceType == null)
            {
                return NotFound($"Substance type with ID {id} not found.");
            }

            return substanceType;
        }

        // PUT: api/SubstanceTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubstanceType(int id, SubstanceType substanceType)
        {
            if (id != substanceType.Id)
            {
                return BadRequest("ID in URL must match ID in body.");
            }

             if (string.IsNullOrWhiteSpace(substanceType.Name))
            {
                 return BadRequest("Substance type name cannot be empty.");
            }

            // Verify CategoryId exists
            bool categoryExists = await _context.SubstanceCategories.AnyAsync(c => c.Id == substanceType.CategoryId);
            if (!categoryExists)
            {
                return BadRequest($"Invalid CategoryId: {substanceType.CategoryId}. Category does not exist.");
            }

            // Check for duplicate name within the same category
            bool nameExists = await _context.SubstanceTypes
                                      .AnyAsync(t => t.Name == substanceType.Name && t.CategoryId == substanceType.CategoryId && t.Id != id);
            if (nameExists)
            {
                return Problem(
                    detail: $"A substance type with the name '{substanceType.Name}' already exists in this category.",
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Duplicate Substance Type Name");
            }


            _context.Entry(substanceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubstanceTypeExists(id))
                {
                    return NotFound($"Substance type with ID {id} not found.");
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

        // POST: api/SubstanceTypes
        [HttpPost]
        public async Task<ActionResult<SubstanceType>> PostSubstanceType(SubstanceType substanceType)
        {
             if (string.IsNullOrWhiteSpace(substanceType.Name))
            {
                 return BadRequest("Substance type name cannot be empty.");
            }

            // Verify CategoryId exists
            bool categoryExists = await _context.SubstanceCategories.AnyAsync(c => c.Id == substanceType.CategoryId);
            if (!categoryExists)
            {
                return BadRequest($"Invalid CategoryId: {substanceType.CategoryId}. Category does not exist.");
            }

            // Check for duplicate name within the same category
            bool nameExists = await _context.SubstanceTypes
                                      .AnyAsync(t => t.Name == substanceType.Name && t.CategoryId == substanceType.CategoryId);
             if (nameExists)
            {
                 return Problem(
                    detail: $"A substance type with the name '{substanceType.Name}' already exists in this category.",
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Duplicate Substance Type Name");
            }

            _context.SubstanceTypes.Add(substanceType);
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

            // Load the category details before returning
            await _context.Entry(substanceType).Reference(t => t.Category).LoadAsync();

            return CreatedAtAction(nameof(GetSubstanceType), new { id = substanceType.Id }, substanceType);
        }

        // DELETE: api/SubstanceTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubstanceType(int id)
        {
            var substanceType = await _context.SubstanceTypes.FindAsync(id);
            if (substanceType == null)
            {
                return NotFound($"Substance type with ID {id} not found.");
            }

            // Check if any SubstanceUsage uses this type before deleting
            bool isInUse = await _context.SubstanceUsages.AnyAsync(u => u.SubstanceTypeId == id);
            if (isInUse)
            {
                 return Problem(
                    detail: "Cannot delete substance type because it is currently associated with one or more usage records.",
                    statusCode: StatusCodes.Status400BadRequest, // Or 409 Conflict
                    title: "Substance Type In Use");
            }

            _context.SubstanceTypes.Remove(substanceType);
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

        private bool SubstanceTypeExists(int id)
        {
            return _context.SubstanceTypes.Any(e => e.Id == id);
        }
    }
}
