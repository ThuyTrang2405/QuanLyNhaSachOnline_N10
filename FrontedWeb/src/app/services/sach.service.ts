import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface Sach {
  id: number;
  title: string;
  author: string;
  price: number;
  image: string;
  description: string;
  category: string;
  slTon: number;
}

function mapSach(raw: any): Sach {
  return {
    id:
      raw?.id ??
      raw?.maSach ??
      0,

    title:
      raw?.title ??
      raw?.tenSach ??
      '',

    author:
      raw?.author ??
      raw?.tenTacGia ??
      '',

    price:
      raw?.price ??
      raw?.giaGoc ??
      0,

    image:
      raw?.image ??
      raw?.hinhAnh ??
      '',

    description:
      raw?.description ??
      raw?.moTa ??
      '',

    category:
      raw?.category ??
      raw?.tenTheLoai ??
      '',

    slTon:
      raw?.slTon ??
      0
  };
}

@Injectable({
  providedIn: 'root'
})
export class SachService {

  private readonly API_URL =
    'http://localhost:5206/api/Sach';

  constructor(
    private http: HttpClient
  ) {}

  getDanhSach(): Observable<Sach[]> {

    return this.http
      .get<any[]>(this.API_URL)
      .pipe(

        map((list) =>
          list.map((item) => mapSach(item))
        )

      );

  }

  getChiTiet(id: number): Observable<Sach> {

    return this.http
      .get<any>(`${this.API_URL}/${id}`)
      .pipe(

        map((item) => {

          console.log('RAW API:', item);

          return mapSach(item);

        })

      );

  }

}