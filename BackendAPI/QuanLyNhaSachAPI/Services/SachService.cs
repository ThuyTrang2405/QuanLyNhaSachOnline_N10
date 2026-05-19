using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Repositories;
using QuanLyNhaSachAPI.Models;

namespace QuanLyNhaSachAPI.Services
{
    public interface ISachService
    {
        Task<List<SachResponseDTO>> LayDanhSachSachAsync();
        Task<SachResponseDTO> LayChiTietSachAsync(int id);
        Task<Sach> ThemSachAsync(SachRequestDTO request);
        Task CapNhatSachAsync(int id, SachRequestDTO request);
        Task XoaSachAsync(int id);
    }

    public class SachService : ISachService
    {
        private readonly ISachRepository _sachRepo;

        public SachService(ISachRepository sachRepo)
        {
            _sachRepo = sachRepo;
        }

        public async Task<List<SachResponseDTO>> LayDanhSachSachAsync()
        {
            return await _sachRepo.GetAllActiveSachesAsync();
        }

        public async Task<SachResponseDTO> LayChiTietSachAsync(int id)
        {
            var sach = await _sachRepo.GetActiveSachByIdAsync(id);
            if (sach == null)
                throw new KeyNotFoundException("Không tìm thấy sách");
            return sach;
        }

        public async Task<Sach> ThemSachAsync(SachRequestDTO request)
        {
            var sachMoi = new Sach
            {
                TenSach = request.TenSach ?? "",
                GiaGoc = request.GiaGoc ?? 0m,
                MoTa = request.MoTa,
                Slton = request.Slton,
                HinhAnh = request.HinhAnh,
                MaTl = request.MaTl,
                MaTg = request.MaTg,
                TrangThaiS = true 
            };

            await _sachRepo.AddSachAsync(sachMoi);
            return sachMoi;
        }

        public async Task CapNhatSachAsync(int id, SachRequestDTO request)
        {
            var sach = await _sachRepo.GetSachByIdAsync(id);
            if (sach == null)
                throw new KeyNotFoundException("Không tìm thấy sách!");

            sach.TenSach = request.TenSach ?? "";
            sach.GiaGoc = request.GiaGoc ?? 0m;
            sach.MoTa = request.MoTa;
            sach.Slton = request.Slton;
            sach.HinhAnh = request.HinhAnh;
            sach.MaTl = request.MaTl;
            sach.MaTg = request.MaTg;

            await _sachRepo.UpdateSachAsync(sach);
        }

        public async Task XoaSachAsync(int id)
        {
            var sach = await _sachRepo.GetSachByIdAsync(id);
            if (sach == null)
                throw new KeyNotFoundException("Không tìm thấy sách!");

            sach.TrangThaiS = false; 
            await _sachRepo.UpdateSachAsync(sach);
        }
    }
}