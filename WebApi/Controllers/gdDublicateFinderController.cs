using GDriveDuplicateFinder;
using Microsoft.AspNetCore.Mvc;
namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class gdDublicateFinderController : ControllerBase
    {
        
        [HttpGet("ReadAndFindDuplicates")]
        public async Task<IActionResult> ReadAndFindDuplicates()
        {
            DuplicatesFinder duplicatesFinder = new DuplicatesFinder();
            duplicatesFinder.ReadAndFindDuplicates();
            return Ok("kashif");
        }
    }
}
