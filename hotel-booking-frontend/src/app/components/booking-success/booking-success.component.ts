import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { BookingResponse } from '../../models/booking.model';

@Component({
  selector: 'app-booking-success',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './booking-success.component.html',
  styleUrls: ['./booking-success.component.css']
})
export class BookingSuccessComponent implements OnInit {
  router = inject(Router);

  booking: BookingResponse | null = null;

  ngOnInit(): void {
    // Angular Router state passes the booking object when navigating from BookingComponent
    const nav = this.router.getCurrentNavigation();
    const state = nav?.extras?.state as { booking: BookingResponse } | undefined;

    if (state?.booking) {
      this.booking = state.booking;
      // Also cache to localStorage as a fallback for page refresh
      localStorage.setItem('last_booking', JSON.stringify(state.booking));
    } else {
      // localStorage fallback if the user refreshes the page
      const cached = localStorage.getItem('last_booking');
      if (cached) {
        this.booking = JSON.parse(cached);
      }
    }
  }
}
