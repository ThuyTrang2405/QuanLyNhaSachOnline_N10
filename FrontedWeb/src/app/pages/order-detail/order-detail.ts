import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DonHangService } from '../../services/don-hang.service';
import { AuthService } from '../../services/auth';
@Component({
  standalone: true,
  selector: 'app-order-detail',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './order-detail.html',
  styleUrl: './order-detail.css',
})
export class OrderDetail implements OnInit {
  donHang: any = null;
  isLoading: boolean = true;
  errorMsg: string = '';

  editTrangThai: number = 0;
  editMaVanDon: string = '';
  isUpdating: boolean = false;

  constructor(
    public auth:AuthService,
    private route: ActivatedRoute,
    private donHangService: DonHangService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.taiChiTietDonHang(Number(id));
    } else {
      this.errorMsg = 'Mã đơn hàng không hợp lệ.';
      this.isLoading = false;
    }
  }

  taiChiTietDonHang(id: number): void {
    this.donHangService.getChiTietDonHang(id).subscribe({
      next: (data) => {
        this.donHang = {
          maDh: data.maDh,
          maNd: data.maNd,
          maVanDon: data.maVanDon,
          ngayDat: data.ngayDat,
          trangThaiDon: data.trangThaiDon,
          diaChiGh: data.diaChiGh,
          tongTien: data.tongTien,
          chiTiet: data.chiTiet ? data.chiTiet.map((ct: any) => ({
            tenSach: ct.tenSach,
            slsach: ct.slsach,
            donGia: ct.donGia,
            thanhTien: ct.thanhTien
          })) : []
        };
        this.editTrangThai = this.donHang.trangThaiDon;
        this.editMaVanDon = this.donHang.maVanDon;

        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMsg = err?.error?.message || 'Không thể tải chi tiết đơn hàng.';
        this.cdr.markForCheck();
      }
    });
  }

  capNhatTrangThai(): void {
    const trangThaiMoi = Number(this.editTrangThai);
    const trangThaiCu = this.donHang.trangThaiDon;

    if ((trangThaiMoi === 1 || trangThaiMoi === 2) && !this.editMaVanDon.trim()) {
      alert('Vui lòng nhập Mã vận đơn trước khi duyệt giao hàng!');
      this.editTrangThai = trangThaiCu; 
      return;
    }

    if (trangThaiMoi === 3 && trangThaiCu !== 2) {
      alert('Cập nhật thất bại! Chỉ có thể chuyển sang "Hoàn thành" cho các đơn hàng đang ở trạng thái "Đang giao hàng".');
      this.editTrangThai = trangThaiCu; 
      return;
    }

    this.isUpdating = true;
    this.donHangService.updateTrangThaiDon(this.donHang.maDh, trangThaiMoi, this.editMaVanDon.trim()).subscribe({
      next: () => {
        alert('Cập nhật trạng thái đơn hàng thành công!');
        this.isUpdating = false;
        this.taiChiTietDonHang(this.donHang.maDh); // Load lại riêng 1 đơn này
      },
      error: (err) => {
        alert('Cập nhật thất bại. Vui lòng thử lại!');
        this.isUpdating = false;
        this.cdr.markForCheck();
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