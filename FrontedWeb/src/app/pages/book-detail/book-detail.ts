import { Component, OnInit, signal, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Cart } from '../../services/cart';
import { AuthService } from '../../services/auth';
import { SachService, Sach } from '../../services/sach.service';

@Component({
  standalone: true,
  templateUrl: './book-detail.html',
  styleUrl: './book-detail.css',
  imports: [CommonModule, RouterModule],
})
export class BookDetail implements OnInit {

  book: Sach | null = null;
  quantity: number = 1;
  isLoading: boolean = true;
  addedMsg: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private cart: Cart,
    private auth: AuthService,
    private sachService: SachService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      this.isLoading = true;
      this.book = null;
      this.sachService.getChiTiet(id).subscribe({
        next: (data) => {
          this.book = data;
          this.isLoading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.isLoading = false;
          this.cdr.markForCheck();
          this.router.navigate(['/not-found']);
        }
      });
    });
  }

  addToCart(): void {
    if (!this.auth.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }
    if (!this.book) return;
    this.cart.addToCart({
      maSach:  this.book.id,
      tenSach: this.book.title,
      hinhAnh: this.book.image,
      giaGoc:  this.book.price,
      slTon:   this.book.slTon ?? 99,
    }, this.quantity);
    this.addedMsg = `Đã thêm "${this.book.title}" vào giỏ hàng`;
    setTimeout(() => this.addedMsg = '', 2500);
  }

  buyNow(): void {
    if (!this.auth.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }
    if (!this.book) return;
    this.addToCart();
    this.router.navigate(['/cart']);
  }

  increaseQty(): void {
    if (this.book && this.quantity < (this.book.slTon ?? 99)) {
      this.quantity++;
    }
  }

  decreaseQty(): void {
    if (this.quantity > 1) this.quantity--;
  }
}
