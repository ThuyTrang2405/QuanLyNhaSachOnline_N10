using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Repositories;

namespace QuanLyNhaSachAPI.Services
{
    public interface IConfigService
    {
        Task<StoreConfigDTO> LayCauHinhNhaSachAsync();
    }

    public class ConfigService : IConfigService
    {
        private readonly IConfigRepository _configRepo;

        public ConfigService(IConfigRepository configRepo)
        {
            _configRepo = configRepo;
        }

        public async Task<StoreConfigDTO> LayCauHinhNhaSachAsync()
        {
            var config = await _configRepo.GetStoreConfigAsync();

            if (config == null)
                throw new FileNotFoundException("Không tìm thấy file cấu hình hoặc file bị lỗi định dạng.");

            return config;
        }
    }
}