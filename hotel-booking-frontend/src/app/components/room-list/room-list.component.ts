import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { RoomService } from '../../services/room.service';
import { BookingService } from '../../services/booking.service';
import { AuthService } from '../../services/auth.service';
import { Room } from '../../models/room.model';

// Each room has its own isolated state object tracked in the map below
interface RoomState {
  status: 'idle' | 'checking' | 'available' | 'unavailable';
  isChecked: boolean;
}

@Component({
  selector: 'app-room-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.css']
})
export class RoomListComponent implements OnInit {
  roomService = inject(RoomService);
  bookingService = inject(BookingService);
  authService = inject(AuthService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  fb = inject(FormBuilder);

  hotelId!: number;
  rooms: Room[] = [];
  isLoading = true;
  todayDate: string;

  // Each room gets its own reactive FormGroup for date inputs (keyed by roomId)
  roomForms: { [roomId: number]: FormGroup } = {};

  // Each room gets its own UI state (availability status + isChecked flag)
  roomStates: { [roomId: number]: RoomState } = {};

  constructor() {
    const today = new Date();
    this.todayDate = today.toISOString().split('T')[0];
  }

  ngOnInit(): void {
    // Extract the :hotelId from the URL route
    this.hotelId = Number(this.route.snapshot.paramMap.get('hotelId'));
    this.loadRooms();
  }

  loadRooms(): void {
    this.isLoading = true;
    this.roomService.getRoomsByHotel(this.hotelId).subscribe({
      next: (data) => {
        this.rooms = data;
        // Initialize each room with its own FormGroup + state object
        data.forEach(room => {
          // Reactive form for the two date inputs per room
          this.roomForms[room.roomId] = this.fb.group({
            checkIn: [''],
            checkOut: ['']
          });

          // UI state (status + isChecked) remains a plain object
          this.roomStates[room.roomId] = {
            status: 'idle',
            isChecked: false
          };

          // Listen for any date changes → reset availability if already checked
          this.roomForms[room.roomId].valueChanges.subscribe(() => {
            this.onDateChange(room.roomId);
          });
        });
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching rooms', err);
        this.isLoading = false;
      }
    });
  }

  // Called automatically when any date input changes via valueChanges subscription
  onDateChange(roomId: number): void {
    const state = this.roomStates[roomId];
    // If user changes a date AFTER checking, we RESET: hide availability + BOOK NOW
    if (state.isChecked) {
      state.status = 'idle';
      state.isChecked = false;
    }
  }

  // Can the CHECK AVAILABILITY button be clicked? Both dates must be filled
  canCheck(roomId: number): boolean {
    const form = this.roomForms[roomId];
    const state = this.roomStates[roomId];
    const checkIn = form.get('checkIn')?.value;
    const checkOut = form.get('checkOut')?.value;
    return !!checkIn && !!checkOut && !state.isChecked && state.status !== 'checking';
  }

  // Get min date for check-out (must be at least day after check-in)
  getMinCheckOut(roomId: number): string {
    const checkIn = this.roomForms[roomId]?.get('checkIn')?.value;
    if (!checkIn) return this.todayDate;
    const d = new Date(checkIn);
    d.setDate(d.getDate() + 1);
    return d.toISOString().split('T')[0];
  }

  checkAvailability(roomId: number): void {
    const state = this.roomStates[roomId];
    const form = this.roomForms[roomId];
    state.status = 'checking'; // Show spinner state

    const checkIn = form.get('checkIn')?.value;
    const checkOut = form.get('checkOut')?.value;

    this.bookingService.checkAvailability(roomId, checkIn, checkOut).subscribe({
      next: (response) => {
        // Mark as checked — prevents spamming the button
        state.isChecked = true;
        state.status = response.isAvailable ? 'available' : 'unavailable';
      },
      error: (err) => {
        console.error('Availability check error', err);
        state.status = 'unavailable';
        state.isChecked = true;
      }
    });
  }

  bookRoom(roomId: number): void {
    const form = this.roomForms[roomId];

    if (!this.authService.isLoggedIn()) {
      // Guest → redirect to login so they can authenticate first
      this.router.navigate(['/login']);
      return;
    }

    // Cache selected room details so BookingComponent can read them without an extra API call
    const selectedRoom = this.rooms.find(r => r.roomId === roomId);
    if (selectedRoom) {
      localStorage.setItem('selected_room', JSON.stringify(selectedRoom));
    }

    // Pass check-in/check-out as query params to the booking page
    this.router.navigate(['/booking', roomId], {
      queryParams: {
        checkIn: form.get('checkIn')?.value,
        checkOut: form.get('checkOut')?.value
      }
    });
  }
}
