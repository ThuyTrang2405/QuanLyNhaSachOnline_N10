import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth';

export interface TheLoai {
  maTl: number;
  tenTl: string;
}

@Injectable({ providedIn: 'root' })
export class AdminTheLoaiService {

  private readonly API_URL = 'http://localhost:5206/api/TheLoai';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({ 'Authorization': `Bearer ${this.auth.token}` });
  }

  getDanhSach(): Observable<TheLoai[]> {
    return this.http.get<TheLoai[]>(this.API_URL);
  }

  them(tenTl: string): Observable<any> {
    return this.http.post(this.API_URL, { tenTl }, { headers: this.getHeaders() });
  }

  sua(maTl: number, tenTl: string): Observable<any> {
    return this.http.put(`${this.API_URL}/${maTl}`, { tenTl }, { headers: this.getHeaders() });
  }

  xoa(maTl: number): Observable<any> {
    return this.http.delete(`${this.API_URL}/${maTl}`, { headers: this.getHeaders() });
  }
}
