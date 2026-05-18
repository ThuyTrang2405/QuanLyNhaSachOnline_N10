import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  taiKhoan: string = '';   // email hoặc tên đăng nhập
  matKhau: string = '';

  isLoading: boolean = false;
  errorMsg: string = '';

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit(): void {
    this.errorMsg = '';

    // Validate cơ bản phía client
    if (!this.taiKhoan.trim()) {
      this.errorMsg = 'Vui lòng nhập email hoặc tên đăng nhập';
      return;
    }
    if (!this.matKhau) {
      this.errorMsg = 'Vui lòng nhập mật khẩu';
      return;
    }

    this.isLoading = true;

    this.auth.login(this.taiKhoan.trim(), this.matKhau).subscribe({
      next: () => {
        this.isLoading = false;
        // Đăng nhập thành công → về trang chủ
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isLoading = false;
        // Lấy thông báo lỗi từ backend nếu có
        if (err.status === 401) {
          this.errorMsg = err.error?.message ?? 'Tài khoản hoặc mật khẩu không đúng';
        } else if (err.status === 0) {
          this.errorMsg = 'Không thể kết nối đến server. Vui lòng thử lại sau.';
        } else {
          this.errorMsg = 'Đã có lỗi xảy ra. Vui lòng thử lại.';
        }
      }
    });
  }
}
