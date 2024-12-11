using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VNUK.VNUKDbContext;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectorController : ControllerBase
    {
        private readonly VNUK_DbContext _context;

        public InspectorController(VNUK_DbContext context) 
        {
            _context = context;
        }

        
    }
}
