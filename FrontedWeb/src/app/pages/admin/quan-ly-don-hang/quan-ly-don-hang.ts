import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DonHangService } from '../../../services/don-hang.service';

@Component({
  standalone: true,
  selector: 'app-quan-ly-don-hang',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './quan-ly-don-hang.html',
  styleUrl: './quan-ly-don-hang.css'
})
export class QuanLyDonHang implements OnInit {
  danhSachGoc: any[] = [];
  danhSachHienThi: any[] = [];
  isLoading: boolean = true;
  errorMsg: string = '';
  
  tuKhoaTimKiem: string = '';
  loaiTimKiem: string = 'all';

  maVanDonInputs: { [key: number]: string } = {};

  constructor(
    private donHangService: DonHangService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.taiDanhSachDonHang();
  }

  taiDanhSachDonHang(): void {
    this.isLoading = true;
    this.donHangService.getAllDonHangAdmin().subscribe({
      next: (data) => {
        this.danhSachGoc = data.map((d: any) => ({
          maDh: d.MaDh,
          maNd:d.MaNd,
          tenKhachHang: d.TenKhachHang,
          ngayDat: d.NgayDat,
          tongTien: d.TongTien,
          trangThaiDon: d.TrangThaiDon,
          diaChiGh: d.DiaChiGh,
          maNguoiDuyet: d.MaNguoiDuyet,
          maVanDon: d.MaVanDon
        }));

        this.danhSachGoc.forEach(d => {
          this.maVanDonInputs[d.maDh] = d.maVanDon;
        });

        this.danhSachHienThi = [...this.danhSachGoc];
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.errorMsg = err?.error?.message || 'Không thể tải danh sách đơn hàng.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  // Hàm xử lý tìm kiếm đa năng có bộ lọc
  thucHienTimKiem(): void {
    const query = this.tuKhoaTimKiem.trim().toLowerCase();
    
    if (!query) {
      this.danhSachHienThi = [...this.danhSachGoc];
      this.cdr.markForCheck();
      return;
    } 
    
    this.danhSachHienThi = this.danhSachGoc.filter(d => {
      const maDonStr = '#' + d.maDh.toString().toLowerCase();
      const maDonGoc = d.maDh.toString().toLowerCase(); 
      const tenKHStr = d.tenKhachHang ? d.tenKhachHang.toString().toLowerCase() : '';
      const maVanDonStr = d.maVanDon ? d.maVanDon.toString().toLowerCase() : '';

      switch (this.loaiTimKiem) {
        case 'maDh':
          return maDonStr.includes(query) || maDonGoc.includes(query);
        case 'tenKh':
          return tenKHStr.includes(query);
        case 'maVd':
          return maVanDonStr.includes(query);
        default: 
          return maDonStr.includes(query) || maDonGoc.includes(query) || tenKHStr.includes(query) || maVanDonStr.includes(query);
      }
    });
    
    this.cdr.markForCheck();
  }

  thayDoiTrangThai(maDh: number, trangThaiMoi: number): void {
    const donHang = this.danhSachGoc.find(d => d.maDh === maDh);
    if (!donHang) return;

    const trangThaiCu = donHang.trangThaiDon;

    const trackingCode = this.maVanDonInputs[maDh] || donHang.maVanDon || '';
    
    if ((trangThaiMoi === 1 || trangThaiMoi === 2) && !trackingCode.trim()) {
      alert('Vui lòng nhập Mã vận đơn trước khi duyệt!');
      
      this.taiDanhSachDonHang(); 
      return;
    }

    if (trangThaiMoi === 3 && trangThaiCu !== 2) {
      alert('Cập nhật thất bại! Chỉ có thể chuyển sang "Hoàn thành" cho các đơn hàng đang ở trạng thái "Đang giao hàng".');
      this.taiDanhSachDonHang();
      return;
    }

    this.donHangService.updateTrangThaiDon(maDh, Number(trangThaiMoi), trackingCode.trim()).subscribe({
      next: () => {
        alert(`Cập nhật trạng thái đơn hàng #${maDh} thành công!`);
        this.taiDanhSachDonHang();
      },
      error: (err) => {
        alert(err?.error?.message || 'Cập nhật thất bại. Vui lòng thử lại!');
        this.taiDanhSachDonHang();
      }
    });
  }

  getTenTrangThai(status: number): string {
    switch (status) {
      case 0: return 'Chờ xác nhận';
      case 1: return 'Chờ lấy hàng';
      case 2: return 'Đang giao hàng';
      case 3: return 'Hoàn thành';
      case 4: return 'Đã hủy';
      default: return 'Không xác định';
    }
  }

  getClassTrangThai(status: number): string {
    switch (status) {
      case 0: return 'status-badge status-waiting';
      case 1: return 'status-badge status-preparing';
      case 2: return 'status-badge status-shipping';
      case 3: return 'status-badge status-success';
      case 4: return 'status-badge status-cancel';
      default: return 'status-badge';
    }
  }
}