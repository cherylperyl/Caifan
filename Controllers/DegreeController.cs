using Caifan.Models;
using Korzh.EasyQuery.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Caifan.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class DegreeController : ControllerBase
    {
        private readonly DataContext _context;
        
        // constructor
        public DegreeController(DataContext context)
        {
            _context = context;
        }


        // Get all Degrees
        [HttpGet]
        public async Task<ActionResult<List<Degree>>> Get()
        {
            return Ok(await _context.Degrees
                .Include(d=>d.DegreeUniversities)
                .Include(d=>d.DegreeUsers)
                .ToListAsync());
        }
        
        // Get a Degree based on a given Degree ID (bid)
        [HttpGet("{degreeid}")]
        public async Task<ActionResult<Degree>> Get(string degreeid)
        {
            var degree = await _context.Degrees
                .Include(d=>d.DegreeUniversities)
                .Include(d=>d.DegreeUsers)
                .FirstOrDefaultAsync(d => d.DegreeId == degreeid);

            if (degree == null)
                return BadRequest("Major not found.");
            return Ok(degree);
        }
        
        // Add a new Degree
        [HttpPost]
        public async Task<ActionResult<List<Degree>>> AddMajor([FromBody] Degree degree)
        {
            _context.Degrees.Add(degree);
            await _context.SaveChangesAsync();
            
            return Ok(await _context.Degrees.ToListAsync());
        }
        
        // Update a Degree fields
        [HttpPut]
        public async Task<ActionResult<List<Degree>>> UpdateDegree(Degree request)
        {
            var dbDegree = await _context.Degrees.FindAsync(request.DegreeId);
            if (dbDegree == null)
                return BadRequest("Degree not found.");
            dbDegree.DegreeId = request.DegreeId;
            dbDegree.DegreeName = request.DegreeName;
            
            await _context.SaveChangesAsync();
            
            return Ok(await _context.Degrees.ToListAsync());
        }
        
        // Delete a Degree based on a given Degree ID (degreeid)
        [HttpDelete("{degreeid}")]
        public async Task<ActionResult<List<Degree>>> Delete(string degreeid)
        {
            var dbDegree = await _context.Degrees.FindAsync(degreeid);
            if (dbDegree == null)
                return BadRequest("Major not found.");

            _context.Degrees.Remove(dbDegree);
            await _context.SaveChangesAsync();
            
            return Ok(await _context.Degrees.ToListAsync());
        }
        
        [HttpGet("search/{text}")]
        public async Task<ActionResult<List<Country>>> TextSearch(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return Ok(await _context.Degrees.FullTextSearchQuery(text).ToListAsync());
            }
            else
            {
                return Ok(await _context.Degrees.ToListAsync());
            }
        }
    }
    
}