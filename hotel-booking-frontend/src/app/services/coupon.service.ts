import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CouponResponse, CreateCoupon } from '../models/coupon.model';

@Injectable({ providedIn: 'root' })
export class CouponService {
  private baseUrl = 'http://localhost:5297/api/coupons';

  constructor(private http: HttpClient) {}

  // Validates a coupon code dynamically (e.g. SAVE20)
  validateCoupon(code: string): Observable<CouponResponse> {
    return this.http.post<CouponResponse>(`${this.baseUrl}/validate`, { code });
  }

  createCoupon(data: CreateCoupon): Observable<any> {
    return this.http.post(`${this.baseUrl}`, data);
  }

  toggleCoupon(couponId: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/${couponId}/toggle`, {});
  }
}
