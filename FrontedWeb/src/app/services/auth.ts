import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError } from 'rxjs';

export interface AuthResponse {
  token: string;
  maNd: number;
  hoTen: string;
  tenDangNhap: string;
  email: string;
  vaiTro: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly API_URL = 'http://localhost:5206/api/Auth';
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY  = 'auth_user';

  constructor(
    private http: HttpClient, 
    private router: Router, 
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  // =============================================
  // ĐĂNG NHẬP (Bắt buộc dùng POST)
  // =============================================
  login(taiKhoan: string, matKhau: string): Observable<any> {
    const body = {
      TaiKhoan: taiKhoan,
      MatKhau: matKhau
    };

    console.log('🚀 Gửi POST request đăng nhập:', body);

    // Phải là this.http.post chứ KHÔNG ĐƯỢC dùng this.http.get
    return this.http.post<any>(`${this.API_URL}/login`, body).pipe(
      tap((res: any) => {
        console.log('✅ Đăng nhập thành công, nhận phản hồi:', res);
        
        if (isPlatformBrowser(this.platformId)) {
          const mappedUser: AuthResponse = {
            token:       res.token,
            maNd:        res.maNd,
            hoTen:       res.hoTen,
            tenDangNhap: res.tenDangNhap,
            email:       res.email,
            vaiTro:      res.vaiTro
          };

          if (mappedUser.token) {
            localStorage.setItem(this.TOKEN_KEY, mappedUser.token);
            localStorage.setItem(this.USER_KEY, JSON.stringify(mappedUser));
          }
        }
      }),
      catchError(err => {
        console.error('❌ Lỗi phản hồi từ server:', err);
        return throwError(() => err);
      })
    );
  }

  // =============================================
  // ĐĂNG KÝ
  // =============================================
  register(data: any): Observable<any> {
    const body = {
      HoTen:       data.hoTen,
      Email:       data.email,
      TenDangNhap: data.tenDangNhap,
      MatKhau:     data.matKhau,
      Sdt:         data.sdt,
      GioiTinh:    data.gioiTinh
    };
    return this.http.post(`${this.API_URL}/register`, body);
  }

  // =============================================
  // ĐĂNG XUẤT
  // =============================================
  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.TOKEN_KEY);
      localStorage.removeItem(this.USER_KEY);
    }
    this.router.navigate(['/']);
  }

  get isLoggedIn(): boolean {
    if (!isPlatformBrowser(this.platformId)) return false;
    const token = localStorage.getItem(this.TOKEN_KEY);
    if (!token) return false;
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  get currentUser(): AuthResponse | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    const raw = localStorage.getItem(this.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }

  get token(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem(this.TOKEN_KEY);
  }

  get vaiTro(): string {
    return this.currentUser?.vaiTro || '';
  }
}