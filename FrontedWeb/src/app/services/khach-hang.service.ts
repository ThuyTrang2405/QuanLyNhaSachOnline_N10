import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth';

export interface KhachHang {
  maNd: number;
  hoTen: string;
  email: string;
  sdt: string;
  diaChi: string;
  trangThaiTk: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class KhachHangService {

  private readonly API_URL = 'http://localhost:5206/api/KhachHang';

  constructor(
    private http: HttpClient,
    private auth: AuthService
  ) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Authorization': `Bearer ${this.auth.token}`,
      'Content-Type': 'application/json'
    });
  }

  getDanhSach(): Observable<KhachHang[]> {
    return this.http.get<KhachHang[]>(this.API_URL, {
      headers: this.getHeaders()
    });
  }

  capNhatTrangThai(maNd: number, trangThai: boolean): Observable<any> {
    return this.http.put(`${this.API_URL}/${maNd}/trang-thai`, { trangThai }, {
      headers: this.getHeaders()
    });
  }
}
