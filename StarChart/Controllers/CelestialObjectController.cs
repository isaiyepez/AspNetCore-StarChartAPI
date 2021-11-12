using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            try
            {
                var celestialObject = _context.CelestialObjects.Find(id);
                if (celestialObject == null)
                    return NotFound();
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
                return Ok(celestialObject);
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            try
            {
                var celestialObjects = _context.CelestialObjects.Where(e => e.Name == name);
                if (!celestialObjects.Any())
                    return NotFound();

                foreach (var celestialObject in celestialObjects)
                {
                    celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
                }
                return Ok(celestialObjects);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var celestialObjects = _context.CelestialObjects.ToList();
                if (celestialObjects == null)
                    return NotFound();

                foreach (var celestialObject in celestialObjects)
                {
                    celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
                }
                
                return Ok(celestialObjects);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var originalCelestialObject = _context.CelestialObjects.Find(id);
            if (originalCelestialObject == null)
                 return NotFound();
            originalCelestialObject.Name = celestialObject.Name;
            originalCelestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            originalCelestialObject.OrbitedObjectId = celestialObject.Id;
            _context.Update(originalCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var originalCelestialObject = _context.CelestialObjects.Find(id);
            if (originalCelestialObject == null)
                return NotFound();

            originalCelestialObject.Name = name;
            _context.Update(originalCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(e => e.Id == id || e.OrbitedObjectId == id);
            if (celestialObjects.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
