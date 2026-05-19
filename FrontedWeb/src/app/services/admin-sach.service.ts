import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth';

export interface SachAdmin {
  maSach: number;
  tenSach: string;
  giaGoc: number;
  hinhAnh: string;
  moTa: string;
  slTon: number; 
  tenTacGia?: string; 
  tenTheLoai?: string;
  trangThaiS?: boolean;
}

export interface SachForm {
  tenSach: string;
  giaGoc: number;
  hinhAnh: string;
  moTa: string;
  slton: number;
  maTl?: number;
  maTg?: number;
  maNxb?: number;
}

@Injectable({ providedIn: 'root' })
export class AdminSachService {

  private readonly API_URL = 'http://localhost:5206/api/Sach';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({ 'Authorization': `Bearer ${this.auth.token}` });
  }

  getDanhSach(): Observable<SachAdmin[]> {
    return this.http.get<SachAdmin[]>(this.API_URL, { headers: this.getHeaders() });
  }

  themSach(data: SachForm): Observable<any> {
    return this.http.post(this.API_URL, data, { headers: this.getHeaders() });
  }

  capNhatSach(id: number, data: SachForm): Observable<any> {
    return this.http.put(`${this.API_URL}/${id}`, data, { headers: this.getHeaders() });
  }

  anSach(id: number): Observable<any> {
    return this.http.delete(`${this.API_URL}/${id}`, { headers: this.getHeaders() });
  }
}
