import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AdminSachService, SachAdmin, SachForm } from '../../../services/admin-sach.service';

@Component({
  standalone: true,
  selector: 'app-quan-ly-sach',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './quan-ly-sach.html',
  styleUrl: './quan-ly-sach.css',
})
export class QuanLySach implements OnInit {

  danhSach: SachAdmin[] = [];
  isLoading = true;
  errorMsg = '';
  successMsg = '';

  // Trạng thái form
  showForm = false;
  isEditing = false;
  editingId: number | null = null;
  isSaving = false;

  form: SachForm = this.emptyForm();

  // Thêm ChangeDetectorRef vào constructor
  constructor(private sachService: AdminSachService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadDanhSach();
  }

  private emptyForm(): SachForm {
    return { tenSach: '', giaGoc: 0, hinhAnh: '', moTa: '', slton: 0 };
  }

  loadDanhSach(): void {
    this.isLoading = true;
    this.sachService.getDanhSach().subscribe({
      next: (data) => {
        this.danhSach = data;
        this.isLoading = false;
        this.cdr.detectChanges(); // Ép cập nhật giao diện
      },
      error: () => {
        this.isLoading = false;
        this.errorMsg = 'Không thể tải danh sách sách. Vui lòng thử lại.';
        this.cdr.detectChanges();
      }
    });
  }

  // Mở form thêm mới
  openThemMoi(): void {
    this.isEditing = false;
    this.editingId = null;
    this.form = this.emptyForm();
    this.showForm = true;
    this.errorMsg = '';
    this.successMsg = '';
  }

// Mở form sửa
 openSua(sach: any): void { 
    this.isEditing = true;
    this.editingId = sach.maSach; 
    this.form = {
      tenSach: sach.tenSach, 
      giaGoc:  sach.giaGoc,  
      hinhAnh: sach.hinhAnh, 
      moTa:    sach.moTa,    
      slton:   sach.slTon,
    };
    this.showForm = true;
    this.errorMsg = '';
    this.successMsg = '';
  }

  // Đóng form
  closeForm(): void {
    this.showForm = false;
    this.form = this.emptyForm();
    this.editingId = null;
  }

  // Lưu (thêm hoặc sửa)
  luuSach(): void {
    if (!this.form.tenSach.trim()) {
      this.errorMsg = 'Vui lòng nhập tên sách';
      return;
    }
    if (this.form.giaGoc <= 0) {
      this.errorMsg = 'Giá phải lớn hơn 0';
      return;
    }

    this.isSaving = true;
    this.errorMsg = '';

    const request$ = this.isEditing && this.editingId !== null
      ? this.sachService.capNhatSach(this.editingId, this.form)
      : this.sachService.themSach(this.form);

    request$.subscribe({
      next: () => {
        this.isSaving = false;
        this.successMsg = this.isEditing ? 'Cập nhật sách thành công!' : 'Thêm sách thành công!';
        this.closeForm();
        this.loadDanhSach();
        this.cdr.detectChanges(); // Ép cập nhật giao diện
        setTimeout(() => {
          this.successMsg = '';
          this.cdr.detectChanges();
        }, 3000);
      },
      error: (err) => {
        this.isSaving = false;
        this.errorMsg = err.error?.message ?? 'Có lỗi xảy ra. Vui lòng thử lại.';
        this.cdr.detectChanges();
      }
    });
  }

anSach(sach: any): void {
    if (!confirm(`Bạn có chắc muốn ẩn sách "${sach.tenSach}"?`)) return;

    this.sachService.anSach(sach.maSach).subscribe({
      next: () => {
        this.successMsg = `Đã ẩn sách "${sach.tenSach}"`;
        this.loadDanhSach();
        this.cdr.detectChanges(); // Ép cập nhật giao diện
        setTimeout(() => {
          this.successMsg = '';
          this.cdr.detectChanges();
        }, 3000);
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Không thể ẩn sách. Vui lòng thử lại.';
        this.cdr.detectChanges();
      }
    });
  }
}