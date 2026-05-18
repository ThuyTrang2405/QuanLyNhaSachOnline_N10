import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { DonHangService } from '../../services/don-hang.service';

// 1. Thêm Interface để định nghĩa kiểu dữ liệu chuẩn (Giống cách Huệ dùng CartItem)
export interface LichSuDonHangItem {
  maDh: number;
  ngayDat: string;
  tongTien: number;
  trangThaiDon: number;
}

@Component({
  standalone: true,
  selector: 'app-order-history',
  imports: [CommonModule, RouterModule],
  templateUrl: './order-history.html',
  styleUrl: './order-history.css',
})
export class OrderHistory implements OnInit {
  
  // 2. Thay vì dùng any[], tụi mình dùng Interface vừa tạo cho code chặt chẽ
  danhSachDonHang: LichSuDonHangItem[] = [];
  isLoading: boolean = false;
  errorMsg: string = '';

  constructor(
    public auth: AuthService,
    private router: Router,
    private donHangService: DonHangService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    if (!this.auth.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }
    this.taiLichSuDonHang();
  }

  taiLichSuDonHang(): void {
    this.isLoading = true;
    this.errorMsg = '';

    this.donHangService.getDonHangCuaToi().subscribe({
      next: (data) => {
        // 3. DATA MAPPING: Biến đổi từ PascalCase (C#) sang camelCase (Angular)
        // Kỹ thuật này đối xứng 100% với cách Huệ làm ở file Checkout
        this.danhSachDonHang = data.map((don: any) => ({
          maDh: don.MaDh,
          ngayDat: don.NgayDat,
          tongTien: don.TongTien,
          trangThaiDon: don.TrangThaiDon
        }));

        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMsg = err?.error?.message || 'Không thể tải lịch sử đơn hàng. Vui lòng thử lại sau!';
        this.cdr.markForCheck();
      }
    });
  }

  // Các hàm tiện ích giữ nguyên
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