import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class Cart {

  items: any[] = [];

  addToCart(book: any) {
    this.items.push(book);
  }

  getItems() {
    return this.items;
  }
}