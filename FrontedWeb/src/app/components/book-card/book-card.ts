import { Component, Input } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Cart } from '../../services/cart';
import { AuthService } from '../../services/auth';

@Component({
  standalone: true,
  selector: 'app-book-card',
  imports: [RouterLink, CommonModule],
  templateUrl: './book-card.html',
  styleUrl: './book-card.css'
})
export class BookCard {

  @Input() book: any;

  rating: string = '4.5';
  soldCount: string = '100';

  constructor(private cart: Cart, private auth: AuthService, private router: Router) {}

  addToCart(): void {
    if (!this.auth.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }
    this.cart.addToCart({
      maSach:  this.book.id,
      tenSach: this.book.title,
      hinhAnh: this.book.image,
      giaGoc:  this.book.price,
      slTon:   this.book.slTon ?? 99,
    }, 1);
  }
}
