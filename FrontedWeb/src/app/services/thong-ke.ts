import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth';

export interface DoanhThuItem {
  nam: number;
  thang: number;
  soDonHang: number;
  doanhThu: number;
}

@Injectable({
  providedIn: 'root'
})
export class ThongKeService {

  private readonly API_URL = 'http://localhost:5206/api/ThongKe';

  constructor(private http: HttpClient, private auth: AuthService) {}

  getDoanhThu(): Observable<DoanhThuItem[]> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.auth.token}`
    });
    return this.http.get<DoanhThuItem[]>(`${this.API_URL}/doanh-thu`, { headers });
  }
}
