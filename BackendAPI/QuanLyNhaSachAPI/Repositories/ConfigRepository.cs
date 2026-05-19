using System.Xml.Linq;
using QuanLyNhaSachAPI.DTOs;

namespace QuanLyNhaSachAPI.Repositories
{
    public interface IConfigRepository
    {
        Task<StoreConfigDTO?> GetStoreConfigAsync();
    }

    public class ConfigRepository : IConfigRepository
    {
        public async Task<StoreConfigDTO?> GetStoreConfigAsync()
        {
            try
            {
                string filePath = "StoreConfig.xml";

                if (!File.Exists(filePath))
                    return null;

                XDocument doc = await Task.Run(() => XDocument.Load(filePath));

                var config = doc.Descendants("StoreInfo").Select(x => new StoreConfigDTO
                {
                    TenNhaSach = x.Element("Name")?.Value,
                    DiaChi = x.Element("Address")?.Value,
                    LienHe = x.Element("Hotline")?.Value
                }).FirstOrDefault();

                return config;
            }
            catch
            {
                return null;
            }
        }
    }
}