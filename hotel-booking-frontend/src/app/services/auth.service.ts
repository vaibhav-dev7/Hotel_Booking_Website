import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginRequest, RegisterRequest, AuthResponse } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // NOTE: I auto-corrected the port from 5000 to 5297 because that is your actual .NET running port!
  private baseUrl = 'http://localhost:5297/api/auth';

  // --- ANGULAR SIGNALS ---
  // signal() holds our state reactively. When it changes, the UI updates instantly everywhere.
  currentUser = signal<AuthResponse | null>(null);

  // computed() variables rely on the signal. If currentUser changes, these automatically recalculate!
  isLoggedIn = computed(() => this.currentUser() !== null);
  isAdmin = computed(() => this.currentUser()?.role === 'Admin');
  userName = computed(() => this.currentUser()?.name || '');

  constructor(private http: HttpClient) {
    this.restoreState();
  }

  // Initialize service by checking localStorage for existing credentials
  private restoreState() {
    const token = localStorage.getItem('jwt_token');
    const name = localStorage.getItem('user_name');
    const role = localStorage.getItem('user_role');

    if (token && name && role) {
      // Rehydrate the signal state when the user refreshes the browser
      this.currentUser.set({ token, name, role });
    }
  }

  register(data: RegisterRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, data);
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    // tap() allows us to run side-effects (like storing local data) without altering the observable stream
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, data).pipe(
      tap((response) => {
        // Save to browser memory
        localStorage.setItem('jwt_token', response.token);
        localStorage.setItem('user_name', response.name);
        localStorage.setItem('user_role', response.role);

        // Update the reactive signal globally
        this.currentUser.set(response);
      })
    );
  }

  logout(): void {
    // Wipe browser memory
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user_name');
    localStorage.removeItem('user_role');

    // Set signal strictly to null
    this.currentUser.set(null);
  }

  getToken(): string | null {
    return localStorage.getItem('jwt_token');
  }

  // Helper method to decode JWT tokens manually if we ever need the hidden internal claims (like raw UserId)
  private decodeToken(token: string): any {
    try {
      const payload = token.split('.')[1];
      const decodedPayload = atob(payload); // Decodes base64 string
      return JSON.parse(decodedPayload);
    } catch (e) {
      return null;
    }
  }
}
