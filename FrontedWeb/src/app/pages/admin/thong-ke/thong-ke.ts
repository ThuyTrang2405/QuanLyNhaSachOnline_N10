import { Component, OnInit, ChangeDetectorRef } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThongKeService, DoanhThuItem } from '../../../services/thong-ke';

@Component({
  standalone: true,
  selector: 'app-thong-ke',
  imports: [CommonModule, RouterModule],
  templateUrl: './thong-ke.html',
  styleUrl: './thong-ke.css',
})
export class ThongKe implements OnInit {

  danhSach: DoanhThuItem[] = [];
  isLoading: boolean = true;
  errorMsg: string = '';

  constructor(
    private thongKeService: ThongKeService,
    private cdr: ChangeDetectorRef 
  ) {}

  ngOnInit(): void {
    this.thongKeService.getDoanhThu().subscribe({
      next: (data) => {
        this.danhSach = data;
        this.isLoading = false;
        
        this.cdr.detectChanges(); 
      },
      error: (err) => {
        this.isLoading = false;
        if (err.status === 403) {
          this.errorMsg = 'Bạn không có quyền xem trang này.';
        } else if (err.status === 0) {
          this.errorMsg = 'Không thể kết nối đến server.';
        } else {
          this.errorMsg = 'Đã có lỗi xảy ra khi tải dữ liệu.';
        }
        
        this.cdr.detectChanges(); 
      }
    });
  }

  get tongDoanhThu(): number {
    return this.danhSach.reduce((sum, item) => sum + item.doanhThu, 0);
  }

  get tongDonHang(): number {
    return this.danhSach.reduce((sum, item) => sum + item.soDonHang, 0);
  }

  getTenThang(thang: number): string {
    return `Tháng ${thang}`;
  }
}