import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home';
import { BookDetail } from './pages/book-detail/book-detail';
import { Login } from './pages/login/login';
import { CartPage } from './pages/cart-page/cart-page';
import { Register } from './pages/register/register';
import { ForgotPassword } from './pages/forgot-password/forgot-password';
import { NotFound } from './pages/not-found/not-found';
import { Checkout } from './pages/checkout/checkout';
import { OrderSuccess } from './pages/order-success/order-success';
import { OrderDetail } from './pages/order-detail/order-detail';
import { OrderHistory } from './pages/order-history/order-history';
import { ThongKe } from './pages/admin/thong-ke/thong-ke';
import { QuanLySach } from './pages/admin/quan-ly-sach/quan-ly-sach';
import { QuanLyTheLoai } from './pages/admin/quan-ly-theloai/quan-ly-theloai';
import { QuanLyKhachHang } from './pages/admin/quan-ly-khach-hang/quan-ly-khach-hang';
import { QuanLyDonHang } from './pages/admin/quan-ly-don-hang/quan-ly-don-hang';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'book/:id', component: BookDetail },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'cart', component: CartPage },
  { path: 'checkout', component: Checkout },
  { path: 'order-success', component: OrderSuccess },
  { path: 'forgot-password', component: ForgotPassword },
  { path: 'don-hang/:id', component: OrderDetail },
  { path: 'my-orders', component: OrderHistory },
  { path: 'admin/thong-ke',           component: ThongKe,          canActivate: [adminGuard] },
  { path: 'admin/quan-ly-sach',       component: QuanLySach,       canActivate: [adminGuard] },
  { path: 'admin/quan-ly-theloai',    component: QuanLyTheLoai,    canActivate: [adminGuard] },
  { path: 'admin/quan-ly-khach-hang', component: QuanLyKhachHang,  canActivate: [adminGuard] },
  { path: 'admin/quan-ly-don-hang', component: QuanLyDonHang, canActivate: [adminGuard] },
  { path: '**', component: NotFound },
];
