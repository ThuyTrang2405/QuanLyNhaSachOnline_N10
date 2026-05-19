using Microsoft.AspNetCore.Mvc;
using QuanLyNhaSachAPI.Services;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _configService;

        public ConfigController(IConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreConfig()
        {
            try
            {
                var config = await _configService.LayCauHinhNhaSachAsync();
                return Ok(config);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}