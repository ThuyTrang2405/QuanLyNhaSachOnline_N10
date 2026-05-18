import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth';

export interface GioHangItem {
  MaSach: number;
  SoLuongSP: number;
  DonGia: number;
}

export interface DonHangRequest {
  MaKH: number;
  DiaChiGiaoHang: string;
  GioHang: GioHangItem[];
}

@Injectable({
  providedIn: 'root'
})
export class DonHangService {

  private readonly API_URL = 'http://localhost:5206/api/DonHang';

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

  taoDonHang(request: DonHangRequest): Observable<any> {
    return this.http.post(`${this.API_URL}/tao-don`, request, {
      headers: this.getHeaders()
    });
  }

  getDonHangCuaToi(): Observable<any[]> {
    return this.http.get<any[]>(`${this.API_URL}/cua-toi`, {
      headers: this.getHeaders()
    });
  }

  // Lấy chi tiết 1 đơn hàng theo ID
  getChiTietDonHang(id: number): Observable<any> {
    return this.http.get<any>(`${this.API_URL}/${id}`, {
      headers: this.getHeaders()
    });
  }

  // Admin
  getAllDonHangAdmin(): Observable<any[]> {
    return this.http.get<any[]>(this.API_URL, {
      headers: this.getHeaders()
    });
  }

  // Admin
  updateTrangThaiDon(id: number, trangThaiMoi: number, maVanDon?: string): Observable<any> {
    return this.http.put(`${this.API_URL}/${id}/trang-thai`, { 
      trangThaiMoi, 
      maVanDon 
    }, {
      headers: this.getHeaders()
    });
  }
}


  