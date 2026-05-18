import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  hoTen: string = '';
  email: string = '';
  tenDangNhap: string = '';
  sdt: string = '';
  matKhau: string = '';
  xacNhanMatKhau: string = '';
  gioiTinh: string = '';

  showPassword: boolean = false;
  showConfirmPassword: boolean = false;
  isLoading: boolean = false;

  // Lỗi validation client-side
  errors: { [key: string]: string } = {};
  // Lỗi từ server
  serverError: string = '';

  constructor(private auth: AuthService, private router: Router) {}

  toggleShowPassword(): void {
    this.showPassword = !this.showPassword;
  }

  toggleShowConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  validate(): boolean {
    this.errors = {};

    if (!this.hoTen.trim())
      this.errors['hoTen'] = 'Vui lòng nhập họ tên';

    if (!this.email.trim())
      this.errors['email'] = 'Vui lòng nhập email';
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email))
      this.errors['email'] = 'Email không hợp lệ';

    if (!this.tenDangNhap.trim())
      this.errors['tenDangNhap'] = 'Vui lòng nhập tên đăng nhập';
    else if (this.tenDangNhap.length < 4)
      this.errors['tenDangNhap'] = 'Tên đăng nhập phải có ít nhất 4 ký tự';

    if (!this.matKhau)
      this.errors['matKhau'] = 'Vui lòng nhập mật khẩu';
    else if (this.matKhau.length < 6)
      this.errors['matKhau'] = 'Mật khẩu phải có ít nhất 6 ký tự';

    if (!this.xacNhanMatKhau)
      this.errors['xacNhanMatKhau'] = 'Vui lòng xác nhận mật khẩu';
    else if (this.matKhau !== this.xacNhanMatKhau)
      this.errors['xacNhanMatKhau'] = 'Mật khẩu xác nhận không khớp';

    return Object.keys(this.errors).length === 0;
  }

  onSubmit(): void {
    this.serverError = '';
    if (!this.validate()) return;

    this.isLoading = true;

    this.auth.register({
      hoTen:       this.hoTen.trim(),
      email:       this.email.trim(),
      tenDangNhap: this.tenDangNhap.trim(),
      matKhau:     this.matKhau,
      sdt:         this.sdt || undefined,
      gioiTinh:    this.gioiTinh || undefined,
    }).subscribe({
      next: () => {
        this.isLoading = false;
        // Đăng ký thành công → chuyển sang trang đăng nhập
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isLoading = false;
        if (err.status === 409) {
          // Email hoặc tên đăng nhập đã tồn tại
          const msg: string = err.error?.message ?? '';
          if (msg.toLowerCase().includes('email'))
            this.errors['email'] = msg;
          else if (msg.toLowerCase().includes('tên đăng nhập'))
            this.errors['tenDangNhap'] = msg;
          else
            this.serverError = msg;
        } else if (err.status === 400) {
          this.serverError = 'Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.';
        } else if (err.status === 0) {
          this.serverError = 'Không thể kết nối đến server. Vui lòng thử lại sau.';
        } else {
          this.serverError = 'Đã có lỗi xảy ra. Vui lòng thử lại.';
        }
      }
    });
  }
}
