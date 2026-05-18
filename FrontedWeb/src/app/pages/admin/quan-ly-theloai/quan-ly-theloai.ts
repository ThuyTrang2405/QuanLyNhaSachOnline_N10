import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AdminTheLoaiService, TheLoai } from '../../../services/admin-theloai.service';

@Component({
  standalone: true,
  selector: 'app-quan-ly-theloai',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './quan-ly-theloai.html',
  styleUrl: './quan-ly-theloai.css',
})
export class QuanLyTheLoai implements OnInit {

  danhSach: TheLoai[] = [];
  isLoading = true;
  errorMsg = '';
  successMsg = '';

  // Thêm mới
  tenMoi = '';
  isAdding = false;

  // Sửa inline
  editingId: number | null = null;
  editingTen = '';
  isSaving = false;

  // Thêm ChangeDetectorRef vào constructor
  constructor(private service: AdminTheLoaiService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadDanhSach();
  }

  loadDanhSach(): void {
    this.isLoading = true;
    this.service.getDanhSach().subscribe({
      next: (data) => { 
        this.danhSach = data; 
        this.isLoading = false; 
        this.cdr.detectChanges(); // Ép cập nhật giao diện
      },
      error: () => { 
        this.isLoading = false; 
        this.errorMsg = 'Không thể tải danh sách thể loại.'; 
        this.cdr.detectChanges();
      }
    });
  }

  // Thêm mới
  them(): void {
    this.errorMsg = '';
    if (!this.tenMoi.trim()) { this.errorMsg = 'Vui lòng nhập tên thể loại'; return; }
    this.isAdding = true;
    this.service.them(this.tenMoi.trim()).subscribe({
      next: () => {
        this.isAdding = false;
        this.tenMoi = '';
        this.showSuccess('Thêm thể loại thành công!');
        this.loadDanhSach();
        this.cdr.detectChanges(); // Ép cập nhật giao diện
      },
      error: (err) => {
        this.isAdding = false;
        this.errorMsg = err.error?.message ?? 'Có lỗi xảy ra.';
        this.cdr.detectChanges();
      }
    });
  }

  // Bắt đầu sửa
  batDauSua(tl: TheLoai): void {
    this.editingId = tl.maTl;
    this.editingTen = tl.tenTl;
    this.errorMsg = '';
  }

  // Hủy sửa
  huyySua(): void {
    this.editingId = null;
    this.editingTen = '';
  }

  // Lưu sửa
  luuSua(): void {
    this.errorMsg = '';
    if (!this.editingTen.trim()) { this.errorMsg = 'Tên thể loại không được để trống'; return; }
    if (this.editingId === null) return;
    this.isSaving = true;
    this.service.sua(this.editingId, this.editingTen.trim()).subscribe({
      next: () => {
        this.isSaving = false;
        this.editingId = null;
        this.showSuccess('Cập nhật thể loại thành công!');
        this.loadDanhSach();
        this.cdr.detectChanges(); // Ép cập nhật giao diện
      },
      error: (err) => {
        this.isSaving = false;
        this.errorMsg = err.error?.message ?? 'Có lỗi xảy ra.';
        this.cdr.detectChanges();
      }
    });
  }

  // Xóa
  xoa(tl: TheLoai): void {
    if (!confirm(`Bạn có chắc muốn xóa thể loại "${tl.tenTl}"?`)) return;
    this.service.xoa(tl.maTl).subscribe({
      next: () => {
        this.showSuccess(`Đã xóa thể loại "${tl.tenTl}"`);
        this.loadDanhSach();
        this.cdr.detectChanges(); // Ép cập nhật giao diện
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Không thể xóa thể loại này.';
        this.cdr.detectChanges();
      }
    });
  }

  private showSuccess(msg: string): void {
    this.successMsg = msg;
    setTimeout(() => {
      this.successMsg = '';
      this.cdr.detectChanges(); // Cập nhật lại lúc ẩn thông báo
    }, 3000);
  }
}