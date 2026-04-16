import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BookingService } from '../../services/booking.service';
import { CouponService } from '../../services/coupon.service';
import { RoomService } from '../../services/room.service';
import { Room } from '../../models/room.model';
import { CreateBooking, BookingResponse } from '../../models/booking.model';
import { CouponResponse } from '../../models/coupon.model';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css']
})
export class BookingComponent implements OnInit {
  route = inject(ActivatedRoute);
  router = inject(Router);
  bookingService = inject(BookingService);
  couponService = inject(CouponService);
  roomService = inject(RoomService);
  fb = inject(FormBuilder);

  // Room data
  room: Room | null = null; //hold room detail from local strorgae 
  roomId!: number;
  todayDate: string;
  tomorrowDate: string;

  // Coupon state
  couponResponse: CouponResponse | null = null;
  couponError = '';
  isValidatingCoupon = false;

  // Availability state
  isAvailable = true;
  availabilityError = '';

  // Submission state
  isSubmitting = false;
  bookingError = '';

  // Calculated price values
  nights = 0;
  subtotal = 0;
  discount = 0;
  total = 0;

  // Reactive Forms
  bookingForm: FormGroup;
  couponForm: FormGroup;

  constructor() {
    const today = new Date();
    this.todayDate = this.formatDate(today);

    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.tomorrowDate = this.formatDate(tomorrow);

    // Default to today → tomorrow so users don't have to type
    this.bookingForm = this.fb.group({
      checkIn: [this.todayDate, Validators.required],
      checkOut: [this.tomorrowDate, Validators.required]
    });

    this.couponForm = this.fb.group({
      couponCode: ['']
    });
  }

  // Helper to format a Date as YYYY-MM-DD
  private formatDate(d: Date): string {
    return d.toISOString().split('T')[0];
  }

  ngOnInit(): void {
    this.roomId = Number(this.route.snapshot.paramMap.get('roomId'));

    const checkIn = this.route.snapshot.queryParamMap.get('checkIn') || '';
    const checkOut = this.route.snapshot.queryParamMap.get('checkOut') || '';

    this.bookingForm.patchValue({ checkIn, checkOut });

    this.loadRoomDetails();

    if (checkIn && checkOut) {
      this.recalculate();
    }
  }

  loadRoomDetails(): void {
    const cached = localStorage.getItem('selected_room');
    if (cached) {
      this.room = JSON.parse(cached);
    }
  }

  get checkIn(): string { return this.bookingForm.get('checkIn')?.value || ''; }
  get checkOut(): string { return this.bookingForm.get('checkOut')?.value || ''; }
  get couponCode(): string { return this.couponForm.get('couponCode')?.value || ''; }

  // Recalculate price breakdown every time dates or coupon changes
  recalculate(): void {
    const { checkIn, checkOut } = this.bookingForm.value;
    if (!checkIn || !checkOut || !this.room) return;

    const start = new Date(checkIn);
    const end = new Date(checkOut);

    this.nights = Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
    if (this.nights < 1) this.nights = 0;

    this.subtotal = this.nights * (this.room?.pricePerNight || 0);

    if (this.couponResponse?.isValid) {
      this.discount = (this.subtotal * this.couponResponse.discountPercentage) / 100;
    } else {
      this.discount = 0;
    }

    this.total = this.subtotal - this.discount;
  }

  // Called whenever the user changes dates
  onDateChange(): void {
    this.availabilityError = '';
    this.isAvailable = true;

    const { checkIn, checkOut } = this.bookingForm.value;
    if (!checkIn || !checkOut) return;

    // Validate: check-in must be today or future
    if (checkIn < this.todayDate) {
      this.bookingForm.patchValue({ checkIn: this.todayDate });
      this.availabilityError = 'Check-in date cannot be in the past. Reset to today.';
      this.recalculate();
      return;
    }

    // Validate: check-out must be after check-in
    if (checkOut <= checkIn) {
      const nextDay = new Date(checkIn);
      nextDay.setDate(nextDay.getDate() + 1);
      this.bookingForm.patchValue({ checkOut: this.formatDate(nextDay) });
      this.availabilityError = 'Check-out must be after check-in. Auto-corrected.';
      this.recalculate();
      return;
    }

    this.recalculate();

    // Check availability with the backend
    this.bookingService.checkAvailability(this.roomId, checkIn, checkOut).subscribe({
      next: (res) => {
        this.isAvailable = res.isAvailable;
        if (!res.isAvailable) {
          this.availabilityError = 'Room is not available for these dates. Please choose different dates.';
        }
      },
      error: () => {
        this.isAvailable = false;
        this.availabilityError = 'Could not verify availability. Please try again.';
      }
    });
  }

  applyCoupon(): void {
    const code = this.couponCode.trim();
    if (!code) return;

    this.isValidatingCoupon = true;
    this.couponError = '';
    this.couponResponse = null;

    this.couponService.validateCoupon(code).subscribe({
      next: (res) => {
        this.isValidatingCoupon = false;
        if (res.isValid) {
          this.couponResponse = res;
          this.recalculate();
        } else {
          this.couponError = 'Invalid or expired coupon.';
        }
      },
      error: () => {
        this.isValidatingCoupon = false;
        this.couponError = 'Could not validate coupon. Please try again.';
      }
    });
  }

  confirmBooking(): void {
    if (!this.isAvailable || this.nights < 1) return;

    this.isSubmitting = true;
    this.bookingError = '';

    const { checkIn, checkOut } = this.bookingForm.value;

    const bookingData: CreateBooking = {
      roomId: this.roomId,
      checkInDate: checkIn,
      checkOutDate: checkOut,
      couponCode: this.couponResponse?.isValid ? this.couponResponse.code : undefined
    };

    this.bookingService.createBooking(bookingData).subscribe({
      next: (response: BookingResponse) => {
        this.isSubmitting = false;
        this.router.navigate(['/booking-success'], { state: { booking: response } });
      },
      error: (err) => {
        this.isSubmitting = false;
        this.bookingError = err.error?.error || 'Booking failed. Please try again.';
      }
    });
  }

  get canConfirm(): boolean {
    return this.isAvailable && this.nights > 0 && !this.isSubmitting && this.bookingForm.valid;
  }
}
