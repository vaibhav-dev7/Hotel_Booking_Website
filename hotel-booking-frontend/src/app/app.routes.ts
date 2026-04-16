import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/hotels', pathMatch: 'full' },

  // Public Routes
  {
    path: 'login',
    loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./components/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'hotels',
    loadComponent: () => import('./components/hotel-list/hotel-list.component').then(m => m.HotelListComponent)
  },
  {
    path: 'hotels/:hotelId/rooms',
    loadComponent: () => import('./components/room-list/room-list.component').then(m => m.RoomListComponent)
  },

  // Auth-protected Routes
  {
    path: 'booking/:roomId',
    canActivate: [authGuard],
    loadComponent: () => import('./components/booking/booking.component').then(m => m.BookingComponent)
  },
  {
    path: 'booking-success',
    canActivate: [authGuard],
    loadComponent: () => import('./components/booking-success/booking-success.component').then(m => m.BookingSuccessComponent)
  },
  {
    path: 'my-bookings',
    canActivate: [authGuard],
    loadComponent: () => import('./components/my-bookings/my-bookings.component').then(m => m.MyBookingsComponent)
  },

  // Admin Routes
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./components/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent)
  },

  // Fallback
  { path: '**', redirectTo: '/hotels' }
];
