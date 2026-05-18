import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject } from 'rxjs';

export interface CartItem {
  maSach: number;
  tenSach: string;
  hinhAnh: string;
  giaGoc: number;
  soLuong: number;
  slTon: number;
}

const CART_KEY = 'cart_items';

@Injectable({
  providedIn: 'root'
})
export class Cart {

  private _items$ = new BehaviorSubject<CartItem[]>([]);
  readonly items$ = this._items$.asObservable();

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    if (isPlatformBrowser(this.platformId)) {
      this._items$.next(this.loadFromStorage());
    }
  }

  private loadFromStorage(): CartItem[] {
    try {
      const raw = localStorage.getItem(CART_KEY);
      return raw ? JSON.parse(raw) : [];
    } catch {
      return [];
    }
  }

  private saveToStorage(items: CartItem[]): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(CART_KEY, JSON.stringify(items));
    }
  }

  private update(items: CartItem[]): void {
    this._items$.next(items);
    this.saveToStorage(items);
  }

  get items(): CartItem[] {
    return this._items$.value;
  }

  addToCart(item: Omit<CartItem, 'soLuong'>, soLuong: number = 1): void {
    const current = [...this._items$.value];
    const idx = current.findIndex(i => i.maSach === item.maSach);

    if (idx >= 0) {
      const newQty = current[idx].soLuong + soLuong;
      current[idx] = {
        ...current[idx],
        soLuong: Math.min(newQty, item.slTon)
      };
    } else {
      current.push({ ...item, soLuong: Math.min(soLuong, item.slTon) });
    }

    this.update(current);
  }

  updateQuantity(maSach: number, soLuong: number): void {
    const current = this._items$.value.map(i =>
      i.maSach === maSach ? { ...i, soLuong: Math.max(1, Math.min(soLuong, i.slTon)) } : i
    );
    this.update(current);
  }

  removeItem(maSach: number): void {
    this.update(this._items$.value.filter(i => i.maSach !== maSach));
  }

  clearCart(): void {
    this.update([]);
  }

  getTotalItems(): number {
    return this._items$.value.reduce((sum, i) => sum + i.soLuong, 0);
  }

  getTotalPrice(): number {
    return this._items$.value.reduce((sum, i) => sum + i.giaGoc * i.soLuong, 0);
  }
}
