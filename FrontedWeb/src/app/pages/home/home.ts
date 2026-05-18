import { Component, OnInit, HostListener, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookCard } from '../../components/book-card/book-card';
import { SachService, Sach } from '../../services/sach.service';

// Cấu hình số lượng sách hiển thị mỗi lần tải
const PAGE_SIZE = 12;

@Component({
  standalone: true,
  selector: 'app-home',
  imports: [CommonModule, FormsModule, BookCard],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class HomeComponent implements OnInit {

  // Dữ liệu gốc từ API
  private allBooks: Sach[] = [];
  categories: string[] = [];
  isLoading: boolean = true;
  isLoadingMore: boolean = false;

  // Bộ lọc
  searchKeyword: string = '';
  selectedCategory: string = '';

  // Dữ liệu hiển thị sau khi lọc và phân trang
  visibleBooks: Sach[] = [];
  private currentPage: number = 0;
  hasMore: boolean = false;

  constructor(
    private sachService: SachService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  /**
   * Tải dữ liệu từ Service và khởi tạo bộ lọc thể loại
   */
  private loadData(): void {
    this.isLoading = true;
    this.sachService.getDanhSach().subscribe({
      next: (data: Sach[]) => {
        this.allBooks = data;
        // Lấy danh sách thể loại duy nhất từ dữ liệu sách
        this.categories = [...new Set(data.map(b => b.category).filter(c => !!c))].sort();
        this.isLoading = false;
        this.resetAndLoad();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('❌ Lỗi nạp danh sách sách:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  /**
   * Xử lý logic lọc dữ liệu dựa trên từ khóa và thể loại
   */
  get filtered(): Sach[] {
    const keyword = this.searchKeyword.toLowerCase().trim();
    return this.allBooks.filter(book => {
      // Tìm kiếm theo tên sách hoặc tác giả
      const matchKeyword = !keyword || 
        book.title.toLowerCase().includes(keyword) || 
        book.author.toLowerCase().includes(keyword);
      
      // Lọc theo thể loại
      const matchCategory = !this.selectedCategory || book.category === this.selectedCategory;
      
      return matchKeyword && matchCategory;
    });
  }

  /**
   * Trả về tổng số lượng sách sau khi lọc để hiển thị trên UI
   */
  get filteredTotal(): number {
    return this.filtered.length;
  }

  /**
   * Reset trạng thái phân trang và nạp lại danh sách hiển thị
   */
  private resetAndLoad(): void {
    this.currentPage = 0;
    this.visibleBooks = [];
    this.loadNextPage();
  }

  /**
   * Lấy trang dữ liệu tiếp theo từ danh sách đã lọc
   */
  private loadNextPage(): void {
    const source = this.filtered;
    const start = this.currentPage * PAGE_SIZE;
    const end = start + PAGE_SIZE;
    
    const batch = source.slice(start, end);
    this.visibleBooks = [...this.visibleBooks, ...batch];
    
    this.currentPage++;
    this.hasMore = end < source.length;
    
    // Yêu cầu Angular kiểm tra lại thay đổi giao diện
    this.cdr.detectChanges();
  }

  /**
   * Xử lý khi người dùng nhập liệu vào ô tìm kiếm
   */
  onSearchChange(): void {
    this.resetAndLoad();
  }

  /**
   * Xử lý khi chọn một thể loại
   */
  selectCategory(cat: string): void {
    this.selectedCategory = cat;
    this.resetAndLoad();
  }

  /**
   * Xóa toàn bộ bộ lọc
   */
  clearFilters(): void {
    this.searchKeyword = '';
    this.selectedCategory = '';
    this.resetAndLoad();
  }

  /**
   * Logic nạp thêm dữ liệu (Infinite Scroll / Load More)
   */
  loadMore(): void {
    if (!this.hasMore || this.isLoadingMore) return;
    
    this.isLoadingMore = true;
    // Giả lập độ trễ nạp dữ liệu để hiển thị spinner mượt hơn
    setTimeout(() => {
      this.loadNextPage();
      this.isLoadingMore = false;
    }, 400);
  }

  /**
   * Lắng nghe sự kiện cuộn chuột để kích hoạt Infinite Scroll
   */
  @HostListener('window:scroll')
  onScroll(): void {
    const scrolled = window.scrollY + window.innerHeight;
    const total = document.documentElement.scrollHeight;
    
    // Nếu cuộn gần tới đáy (còn 300px) thì tải thêm
    if (scrolled >= total - 300) {
      this.loadMore();
    }
  }
}