import { Component } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { BOOKS } from '../../services/book';
import { CommonModule } from '@angular/common';
import { Cart } from '../../services/cart';

@Component({
  standalone: true,
  templateUrl: './book-detail.html',
  styleUrl: './book-detail.css',
  imports: [CommonModule, RouterModule],
})
export class BookDetail {

  book: any; // lưu sách
  quantity: number = 1;

  constructor(private route: ActivatedRoute, private cart: Cart) {
    const id = Number(this.route.snapshot.paramMap.get('id')); // lấy id từ URL
    this.book = BOOKS.find(b => b.id === id); // tìm sách
  }

  addToCart() {
    for(let i = 0; i < this.quantity; i++) {
      this.cart.addToCart(this.book);
    }
    alert(`Đã thêm ${this.quantity} cuốn "${this.book.title}" vào giỏ hàng`);
  }

  increaseQty() {
    this.quantity++;
  }

  decreaseQty() {
    if(this.quantity > 1) {
      this.quantity--;
    }
  }

}