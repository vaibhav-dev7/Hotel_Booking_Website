import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormRecord, FormControl } from '@angular/forms';
import { HotelService } from '../../services/hotel.service';
import { RoomService } from '../../services/room.service';
import { CouponService } from '../../services/coupon.service';
import { Hotel, CreateHotel } from '../../models/hotel.model';
import { CreateRoom } from '../../models/room.model';
import { CreateCoupon } from '../../models/coupon.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  hotelService = inject(HotelService);
  roomService = inject(RoomService);
  couponService = inject(CouponService);
  fb = inject(FormBuilder);

  activeTab = 'hotels'; // 'hotels' | 'rooms' | 'coupons'

  // ---- HOTELS TAB ----
  hotels: Hotel[] = [];
  hotelSuccess = '';
  hotelError = '';

  hotelForm: FormGroup = this.fb.group({
    name:        ['', Validators.required],
    location:    ['', Validators.required],
    description: ['']
  });

  // ---- ROOMS TAB ----
  roomSuccess = '';
  roomError = '';

  roomForm: FormGroup = this.fb.group({
    hotelId:      [0, [Validators.required, Validators.min(1)]],
    roomType:     ['', Validators.required],
    pricePerNight:[0, [Validators.required, Validators.min(1)]],
    capacity:     [0, [Validators.required, Validators.min(1)]]
  });

  // Amenity name → ID mapping (fixed IDs from backend seed data)
  amenityMap: { [key: string]: number } = {
    'WiFi': 1, 'AC': 2, 'TV': 3, 'Pool': 4,
    'Gym': 5, 'Parking': 6, 'Restaurant': 7, 'Mini Bar': 8
  };

  // FormRecord for dynamic amenity checkboxes
  amenityForm: FormRecord<FormControl<boolean>> = this.fb.record(
    Object.keys(this.amenityMap).reduce((acc, key) => {
      acc[key] = this.fb.control<boolean>(false, { nonNullable: true });
      return acc;
    }, {} as { [key: string]: FormControl<boolean> })
  );

  // ---- COUPONS TAB ----
  couponSuccess = '';
  couponError = '';
  createdCoupons: CreateCoupon[] = [];
  todayDate = new Date().toISOString().split('T')[0];

  couponForm: FormGroup = this.fb.group({
    code:               ['', Validators.required],
    discountPercentage: [0, [Validators.required, Validators.min(1), Validators.max(100)]],
    expiryDate:         ['', Validators.required]
  });

  ngOnInit(): void {
    this.loadHotels();
  }

  loadHotels(): void {
    this.hotelService.getAllHotels().subscribe({
      next: (data) => this.hotels = data,
      error: (err) => console.error('Error loading hotels', err)
    });
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
    this.hotelSuccess = ''; this.hotelError = '';
    this.roomSuccess = ''; this.roomError = '';
    this.couponSuccess = ''; this.couponError = '';
  }

  getAmenityKeys(): string[] {
    return Object.keys(this.amenityMap);
  }

  // ========================
  //  HOTELS TAB ACTIONS
  // ========================
  addHotel(): void {
    if (this.hotelForm.invalid) {
      this.hotelForm.markAllAsTouched();
      this.hotelError = 'Name and Location are required.';
      return;
    }

    const payload: CreateHotel = this.hotelForm.value;

    this.hotelService.addHotel(payload).subscribe({
      next: () => {
        this.hotelSuccess = `Hotel "${payload.name}" added successfully!`;
        this.hotelForm.reset({ name: '', location: '', description: '' });
        this.loadHotels();
      },
      error: (err) => this.hotelError = err.error?.error || 'Failed to add hotel.'
    });
  }

  // ========================
  //  ROOMS TAB ACTIONS
  // ========================
  addRoom(): void {
    if (this.roomForm.invalid) {
      this.roomForm.markAllAsTouched();
      this.roomError = 'Hotel, Room Type, and Price are required.';
      return;
    }

    // Build amenityIds array from FormRecord
    const amenityIds: number[] = Object.keys(this.amenityForm.controls)
      .filter(key => this.amenityForm.controls[key].value)
      .map(key => this.amenityMap[key]);

    const payload: CreateRoom = { ...this.roomForm.value, amenityIds };

    this.roomService.addRoom(payload).subscribe({
      next: () => {
        this.roomSuccess = `Room "${payload.roomType}" added successfully!`;
        this.roomForm.reset({ hotelId: 0, roomType: '', pricePerNight: 0, capacity: 0 });
        // Reset amenity checkboxes
        Object.keys(this.amenityForm.controls).forEach(k =>
          this.amenityForm.controls[k].setValue(false)
        );
      },
      error: (err) => this.roomError = err.error?.error || 'Failed to add room.'
    });
  }

  // ========================
  //  COUPONS TAB ACTIONS
  // ========================
  createCoupon(): void {
    if (this.couponForm.invalid) {
      this.couponForm.markAllAsTouched();
      this.couponError = 'All coupon fields are required.';
      return;
    }

    const payload: CreateCoupon = this.couponForm.value;

    this.couponService.createCoupon(payload).subscribe({
      next: () => {
        this.couponSuccess = `Coupon "${payload.code}" created!`;
        this.createdCoupons.unshift({ ...payload });
        this.couponForm.reset({ code: '', discountPercentage: 0, expiryDate: '' });
      },
      error: (err) => this.couponError = err.error?.error || 'Failed to create coupon.'
    });
  }
}
