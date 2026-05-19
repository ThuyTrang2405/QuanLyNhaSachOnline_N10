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


  private loadData(): void {
    this.isLoading = true;
    this.sachService.getDanhSach().subscribe({
      next: (data: Sach[]) => {
        this.allBooks = data;
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

 
  get filteredTotal(): number {
    return this.filtered.length;
  }

  
  private resetAndLoad(): void {
    this.currentPage = 0;
    this.visibleBooks = [];
    this.loadNextPage();
  }

  private loadNextPage(): void {
    const source = this.filtered;
    const start = this.currentPage * PAGE_SIZE;
    const end = start + PAGE_SIZE;
    
    const batch = source.slice(start, end);
    this.visibleBooks = [...this.visibleBooks, ...batch];
    
    this.currentPage++;
    this.hasMore = end < source.length;
    
    this.cdr.detectChanges();
  }

  onSearchChange(): void {
    this.resetAndLoad();
  }


  selectCategory(cat: string): void {
    this.selectedCategory = cat;
    this.resetAndLoad();
  }


  clearFilters(): void {
    this.searchKeyword = '';
    this.selectedCategory = '';
    this.resetAndLoad();
  }


  loadMore(): void {
    if (!this.hasMore || this.isLoadingMore) return;
    
    this.isLoadingMore = true;
    setTimeout(() => {
      this.loadNextPage();
      this.isLoadingMore = false;
    }, 400);
  }


  @HostListener('window:scroll')
  onScroll(): void {
    const scrolled = window.scrollY + window.innerHeight;
    const total = document.documentElement.scrollHeight;
    
    if (scrolled >= total - 300) {
      this.loadMore();
    }
  }
}