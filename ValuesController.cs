using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Doctors
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("test")]
        public string test()
        {
            return "Test is ok in values.";
        }
    }
}
