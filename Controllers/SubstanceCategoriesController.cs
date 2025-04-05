using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubstanceSafe.Models;
using SubstanceSafe.Services;

namespace SubstanceSafe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubstanceCategoriesController : ControllerBase
    {
        private readonly SubstancesDbContext _context;

        public SubstanceCategoriesController(SubstancesDbContext context)
        {
            _context = context;
        }

        // GET: api/SubstanceCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubstanceCategory>>> GetSubstanceCategories()
        {
            // Include related SubstanceTypes if needed in the future: .Include(c => c.SubstanceTypes)
            return await _context.SubstanceCategories.OrderBy(c => c.Name).ToListAsync();
        }

        // GET: api/SubstanceCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubstanceCategory>> GetSubstanceCategory(int id)
        {
            // Include related SubstanceTypes if needed: .Include(c => c.SubstanceTypes)
            var substanceCategory = await _context.SubstanceCategories
                                                .FirstOrDefaultAsync(c => c.Id == id);

            if (substanceCategory == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            return substanceCategory;
        }

        // PUT: api/SubstanceCategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubstanceCategory(int id, SubstanceCategory substanceCategory)
        {
            if (id != substanceCategory.Id)
            {
                return BadRequest("ID in URL must match ID in body.");
            }

            if (string.IsNullOrWhiteSpace(substanceCategory.Name))
            {
                 return BadRequest("Category name cannot be empty.");
            }

            // Check for duplicate name before saving changes
            bool nameExists = await _context.SubstanceCategories
                                      .AnyAsync(c => c.Name == substanceCategory.Name && c.Id != id);
            if (nameExists)
            {
                // Using ProblemDetails for better error reporting
                return Problem(
                    detail: $"A category with the name '{substanceCategory.Name}' already exists.",
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Duplicate Category Name");
            }

            _context.Entry(substanceCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubstanceCategoryExists(id))
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                else
                {
                    throw; // Re-throw unexpected exceptions
                }
            }
            catch (DbUpdateException ex)
            {
                 // Handle potential database errors (like unique constraint violation if index wasn't checked)
                 return Problem(
                    detail: $"An error occurred while updating the database: {ex.InnerException?.Message ?? ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Database Update Error");
            }

            return NoContent(); // Standard response for successful PUT
        }

        // POST: api/SubstanceCategories
        [HttpPost]
        public async Task<ActionResult<SubstanceCategory>> PostSubstanceCategory(SubstanceCategory substanceCategory)
        {
            if (string.IsNullOrWhiteSpace(substanceCategory.Name))
            {
                 return BadRequest("Category name cannot be empty.");
            }

             // Check for duplicate name before adding
            bool nameExists = await _context.SubstanceCategories
                                      .AnyAsync(c => c.Name == substanceCategory.Name);
            if (nameExists)
            {
                 return Problem(
                    detail: $"A category with the name '{substanceCategory.Name}' already exists.",
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Duplicate Category Name");
            }

            _context.SubstanceCategories.Add(substanceCategory);

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


            // Return 201 Created with location header and the created object
            return CreatedAtAction(nameof(GetSubstanceCategory), new { id = substanceCategory.Id }, substanceCategory);
        }

        // DELETE: api/SubstanceCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubstanceCategory(int id)
        {
            var substanceCategory = await _context.SubstanceCategories.FindAsync(id);
            if (substanceCategory == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            // Check if any SubstanceType uses this category before deleting
            bool isInUse = await _context.SubstanceTypes.AnyAsync(t => t.CategoryId == id);
            if (isInUse)
            {
                 return Problem(
                    detail: "Cannot delete category because it is currently associated with one or more substance types.",
                    statusCode: StatusCodes.Status400BadRequest, // Or 409 Conflict
                    title: "Category In Use");
            }

            _context.SubstanceCategories.Remove(substanceCategory);
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

            return NoContent(); // Standard response for successful DELETE
        }

        private bool SubstanceCategoryExists(int id)
        {
            return _context.SubstanceCategories.Any(e => e.Id == id);
        }
    }
}
