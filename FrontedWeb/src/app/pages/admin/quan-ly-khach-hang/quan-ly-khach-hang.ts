import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { KhachHangService, KhachHang } from '../../../services/khach-hang.service';

@Component({
  standalone: true,
  selector: 'app-quan-ly-khach-hang',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './quan-ly-khach-hang.html',
  styleUrl: './quan-ly-khach-hang.css',
})
export class QuanLyKhachHang implements OnInit {

  danhSach: KhachHang[] = [];
  danhSachLoc: KhachHang[] = [];
  isLoading = true;
  errorMsg = '';
  successMsg = '';
  tuKhoa = '';

  constructor(
    private service: KhachHangService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadDanhSach();
  }

  loadDanhSach(): void {
    this.isLoading = true;
    this.errorMsg = '';
    this.service.getDanhSach().subscribe({
      next: (data) => {
        this.danhSach = data;
        this.loc();
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMsg = err.status === 403
          ? 'Bạn không có quyền truy cập trang này.'
          : 'Không thể tải danh sách khách hàng.';
        this.cdr.markForCheck();
      }
    });
  }

  loc(): void {
    const kw = this.tuKhoa.toLowerCase().trim();
    this.danhSachLoc = !kw
      ? [...this.danhSach]
      : this.danhSach.filter(kh =>
          kh.hoTen.toLowerCase().includes(kw) ||
          kh.email.toLowerCase().includes(kw) ||
          kh.sdt.includes(kw)
        );
  }

  toggleTrangThai(kh: KhachHang): void {
    const moi = !kh.trangThaiTk;
    const xacNhan = moi
      ? `Mở khóa tài khoản "${kh.hoTen}"?`
      : `Khóa tài khoản "${kh.hoTen}"? Khách hàng sẽ không thể đăng nhập.`;

    if (!confirm(xacNhan)) return;

    this.service.capNhatTrangThai(kh.maNd, moi).subscribe({
      next: () => {
        kh.trangThaiTk = moi;
        this.loc();
        this.showSuccess(moi ? `Đã mở khóa tài khoản "${kh.hoTen}"` : `Đã khóa tài khoản "${kh.hoTen}"`);
        this.cdr.markForCheck();
      },
      error: () => {
        this.errorMsg = 'Cập nhật trạng thái thất bại. Vui lòng thử lại.';
        this.cdr.markForCheck();
      }
    });
  }

  private showSuccess(msg: string): void {
    this.successMsg = msg;
    setTimeout(() => { this.successMsg = ''; this.cdr.markForCheck(); }, 3000);
  }
}
