using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Repositories;
using QuanLyNhaSachAPI.Models;

namespace QuanLyNhaSachAPI.Services
{
    public interface ITheLoaiService
    {
        Task<List<TheLoaiResponseDTO>> LayDanhSachTheLoaiAsync();
        Task<TheLoaiResponseDTO> ThemTheLoaiAsync(TheLoaiRequestDTO request);
        Task CapNhatTheLoaiAsync(int id, TheLoaiRequestDTO request);
        Task XoaTheLoaiAsync(int id);
    }

    public class TheLoaiService : ITheLoaiService
    {
        private readonly ITheLoaiRepository _theLoaiRepo;

        public TheLoaiService(ITheLoaiRepository theLoaiRepo)
        {
            _theLoaiRepo = theLoaiRepo;
        }

        public async Task<List<TheLoaiResponseDTO>> LayDanhSachTheLoaiAsync()
        {
            return await _theLoaiRepo.GetAllTheLoaisAsync();
        }

        public async Task<TheLoaiResponseDTO> ThemTheLoaiAsync(TheLoaiRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.TenTl))
                throw new ArgumentException("Tên thể loại không được để trống");

            string tenMoi = request.TenTl.Trim();
            if (await _theLoaiRepo.CheckTenTheLoaiTonTaiAsync(tenMoi))
                throw new InvalidOperationException("Thể loại này đã tồn tại");

            var theLoai = new TheLoai { TenTl = tenMoi };
            await _theLoaiRepo.AddTheLoaiAsync(theLoai);

            return new TheLoaiResponseDTO { MaTl = theLoai.MaTl, TenTl = theLoai.TenTl };
        }

        public async Task CapNhatTheLoaiAsync(int id, TheLoaiRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.TenTl))
                throw new ArgumentException("Tên thể loại không được để trống");

            var theLoai = await _theLoaiRepo.GetTheLoaiByIdAsync(id);
            if (theLoai == null)
                throw new KeyNotFoundException("Không tìm thấy thể loại");

            string tenMoi = request.TenTl.Trim();
            if (await _theLoaiRepo.CheckTenTheLoaiTonTaiAsync(tenMoi, id))
                throw new InvalidOperationException("Tên thể loại này đã tồn tại");

            theLoai.TenTl = tenMoi;
            await _theLoaiRepo.UpdateTheLoaiAsync(theLoai);
        }

        public async Task XoaTheLoaiAsync(int id)
        {
            var theLoai = await _theLoaiRepo.GetTheLoaiByIdAsync(id);
            if (theLoai == null)
                throw new KeyNotFoundException("Không tìm thấy thể loại");

            if (await _theLoaiRepo.CheckTheLoaiDangDuocSuDungAsync(id))
                throw new InvalidOperationException("Không thể xóa vì đang có sách thuộc thể loại này");

            await _theLoaiRepo.DeleteTheLoaiAsync(theLoai);
        }
    }
}