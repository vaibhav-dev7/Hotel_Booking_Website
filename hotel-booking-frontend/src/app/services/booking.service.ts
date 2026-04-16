import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateBooking, BookingResponse, BookingHistory, AvailabilityResponse } from '../models/booking.model';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private baseUrl = 'http://localhost:5297/api/bookings';

  constructor(private http: HttpClient) {}

  createBooking(data: CreateBooking): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(this.baseUrl, data);
  }

  checkAvailability(roomId: number, checkIn: string, checkOut: string): Observable<AvailabilityResponse> {
    // Calls the GET endpoint /api/bookings/availability?roomId=X&checkIn=Y&checkOut=Z
    return this.http.get<AvailabilityResponse>(`${this.baseUrl}/availability?roomId=${roomId}&checkIn=${checkIn}&checkOut=${checkOut}`);
  }

  getMyBookings(): Observable<BookingHistory[]> {
    // The Backend API looks at the JWT token automatically to know WHO is calling this!
    return this.http.get<BookingHistory[]>(`${this.baseUrl}/my`);
  }

  // Example for rebooking quickly
  rebook(bookingId: number): Observable<BookingHistory> {
    return this.http.post<BookingHistory>(`${this.baseUrl}/rebook/${bookingId}`, {});
  }
}
