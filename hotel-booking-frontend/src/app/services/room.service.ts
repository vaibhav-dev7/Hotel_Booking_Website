import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Room, CreateRoom } from '../models/room.model';

@Injectable({ providedIn: 'root' })
export class RoomService {
  private baseUrl = 'http://localhost:5297/api';

  constructor(private http: HttpClient) {}

  getRoomsByHotel(hotelId: number): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.baseUrl}/hotels/${hotelId}/rooms`);
  }

  addRoom(data: CreateRoom): Observable<Room> {
    return this.http.post<Room>(`${this.baseUrl}/rooms`, data);
  }
}
