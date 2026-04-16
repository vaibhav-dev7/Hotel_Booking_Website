import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SearchFilters, SearchResult } from '../models/search.model';

@Injectable({ providedIn: 'root' })
export class SearchService {
  private baseUrl = 'http://localhost:5297/api/search';

  constructor(private http: HttpClient) { }

  searchRooms(filters: SearchFilters): Observable<SearchResult[]> {
    // Build query parameters dynamically. Only attach parameters that aren't null or empty!
    let params = new HttpParams(); // create an empty obkject 


    //add value in object based on contion 
    // if the value is present then add it in object else ignore it 
    if (filters.location) {
      params = params.set('location', filters.location);
    }
    if (filters.minPrice) {
      params = params.set('minPrice', filters.minPrice.toString());
    }
    if (filters.maxPrice) {
      params = params.set('maxPrice', filters.maxPrice.toString());
    }
    if (filters.amenities) {
      params = params.set('amenities', filters.amenities);
    }
    if (filters.checkIn) {
      params = params.set('checkIn', filters.checkIn);
    }
    if (filters.checkOut) {
      params = params.set('checkOut', filters.checkOut);
    }

    // Fire the GET request with the nicely formatted query string
    return this.http.get<SearchResult[]>(this.baseUrl, { params });
  }
}
