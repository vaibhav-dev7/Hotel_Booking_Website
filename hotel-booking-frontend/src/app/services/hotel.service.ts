import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Hotel, CreateHotel } from '../models/hotel.model';

@Injectable({ providedIn: 'root' })
export class HotelService {
  // Auto-corrected port from 5000 to match your true ASP.NET backend
  private baseUrl = 'http://localhost:5297/api/hotels';

  constructor(private http: HttpClient) {}

  getAllHotels(): Observable<Hotel[]> {
    return this.http.get<Hotel[]>(this.baseUrl);
  }

  addHotel(data: CreateHotel): Observable<Hotel> {
    return this.http.post<Hotel>(this.baseUrl, data);
  }
}
