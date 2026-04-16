import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common'; //provide directivtes such as ngif,ngfor 
import { Router, RouterLink } from '@angular/router'; //routerlink allow to naviagte between pages without relodaing pages 
import { BookingService } from '../../services/booking.service';
import { BookingHistory } from '../../models/booking.model';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-bookings.component.html',
  styleUrls: ['./my-bookings.component.css']
})
export class MyBookingsComponent implements OnInit {
  bookingService = inject(BookingService);
  router = inject(Router);

  bookings: BookingHistory[] = [];
  isLoading = true;

  // Track which booking cards are expanded (for "Details" toggle)
  expandedBookings: Set<number> = new Set();
  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.isLoading = true;
    this.bookingService.getMyBookings().subscribe({
      next: (data) => {
        this.bookings = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching bookings', err);
        this.isLoading = false;
      }
    });
  }

  toggleDetails(bookingId: number): void {
    if (this.expandedBookings.has(bookingId)) {
      this.expandedBookings.delete(bookingId);
    } else {
      this.expandedBookings.add(bookingId);
    }
  }

  isExpanded(bookingId: number): boolean {
    return this.expandedBookings.has(bookingId);
  }

  // Quick Rebook: navigate to booking page with this room (let user pick new dates)
  quickRebook(booking: BookingHistory): void {
    this.router.navigate(['/booking', booking.roomId]);
  }
}
