import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { Cart, CartItem } from '../../services/cart';
import { AuthService } from '../../services/auth';

@Component({
  standalone: true,
  selector: 'app-cart-page',
  imports: [CommonModule, RouterModule],
  templateUrl: './cart-page.html',
  styleUrl: './cart-page.css',
})
export class CartPage {

  constructor(public cart: Cart, private auth: AuthService, private router: Router) {}

  get items(): CartItem[] {
    return this.cart.items;
  }

  increaseQty(maSach: number, current: number, slTon: number): void {
    if (current < slTon) {
      this.cart.updateQuantity(maSach, current + 1);
    }
  }

  decreaseQty(maSach: number, current: number): void {
    if (current > 1) {
      this.cart.updateQuantity(maSach, current - 1);
    }
  }

  removeItem(maSach: number): void {
    this.cart.removeItem(maSach);
  }

  clearCart(): void {
    this.cart.clearCart();
  }

  checkout(): void {
    if (!this.auth.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }
    this.router.navigate(['/checkout']);
  }
}
