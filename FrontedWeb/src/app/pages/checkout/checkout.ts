import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { Cart, CartItem } from '../../services/cart';
import { AuthService } from '../../services/auth';
import { DonHangService } from '../../services/don-hang.service';

@Component({
  standalone: true,
  selector: 'app-checkout',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css',
})
export class Checkout implements OnInit {

  diaChiGiaoHang: string = '';
  errorMsg: string = '';
  isLoading: boolean = false;

  constructor(
    public cart: Cart,
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
    if (this.cart.items.length === 0) {
      this.router.navigate(['/']);
    }
  }

  get items(): CartItem[] {
    return this.cart.items;
  }

  xacNhanDatHang(): void {
    this.errorMsg = '';

    if (!this.diaChiGiaoHang.trim()) {
      this.errorMsg = 'Vui lòng nhập địa chỉ giao hàng';
      return;
    }

    const maKH = this.auth.currentUser?.maNd;
    if (!maKH) {
      this.errorMsg = 'Không xác định được tài khoản. Vui lòng đăng nhập lại.';
      return;
    }

    this.isLoading = true;

    const request = {
      MaKH: maKH,
      DiaChiGiaoHang: this.diaChiGiaoHang.trim(),
      GioHang: this.cart.items.map(item => ({
        MaSach: item.maSach,
        SoLuongSP: item.soLuong,
        DonGia: item.giaGoc
      }))
    };

    this.donHangService.taoDonHang(request).subscribe({
      next: () => {
        this.isLoading = false;
        this.cart.clearCart();
        this.cdr.markForCheck();
        this.router.navigate(['/order-success']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMsg = err?.error?.message || 'Đặt hàng thất bại. Vui lòng thử lại.';
        this.cdr.markForCheck();
      }
    });
  }
}
