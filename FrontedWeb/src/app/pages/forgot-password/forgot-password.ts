import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-forgot-password',
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
})
export class ForgotPassword {
  email: string = '';
  submitted: boolean = false;
  errorMsg: string = '';

  onSubmit(): void {
    if (!this.email.trim()) {
      this.errorMsg = 'Vui lòng nhập email';
      return;
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email)) {
      this.errorMsg = 'Email không hợp lệ';
      return;
    }
    this.errorMsg = '';
    // Placeholder — sẽ gắn logic gọi API khi backend sẵn sàng
    this.submitted = true;
  }
}
