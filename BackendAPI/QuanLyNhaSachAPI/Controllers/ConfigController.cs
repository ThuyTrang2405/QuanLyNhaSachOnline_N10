using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System.Linq;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStoreConfig()
        {
            XDocument doc = XDocument.Load("StoreConfig.xml");
            var config = doc.Descendants("StoreInfo").Select(x => new
            {
                TenNhaSach = x.Element("Name")?.Value,
                DiaChi = x.Element("Address")?.Value,
                LienHe = x.Element("Hotline")?.Value
            }).FirstOrDefault();

            return Ok(config);
        }
    }
}