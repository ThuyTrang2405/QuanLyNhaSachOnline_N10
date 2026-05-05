import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Cart } from '../../services/cart';

@Component({
  standalone: true,
  selector: 'app-book-card',
  imports: [RouterLink, CommonModule],
  templateUrl: './book-card.html',
  styleUrl: './book-card.css'
})
export class BookCard {

  @Input() book: any;
  constructor(private cart: Cart) {}

  addToCart() {
    this.cart.addToCart(this.book);
    alert('Đã thêm vào giỏ');
  }

  getDiscount(): number {
    // Random discount từ 10-30% cho demo
    return Math.floor(Math.random() * 21) + 10;
  }

  getOriginalPrice(): number {
    const discount = this.getDiscount();
    return Math.floor(this.book.price / (1 - discount / 100));
  }

  getRating(): string {
    // Random rating từ 4.0-5.0
    const rating = (Math.random() * 1 + 4).toFixed(1);
    return rating;
  }

  getSoldCount(): string {
    // Random số lượng đã bán
    const sold = Math.floor(Math.random() * 1000) + 50;
    if (sold >= 1000) {
      return (sold / 1000).toFixed(1) + 'k';
    }
    return sold.toString();
  }

}