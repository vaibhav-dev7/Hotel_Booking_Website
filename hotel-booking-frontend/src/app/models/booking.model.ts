export interface CreateBooking {
  roomId: number;
  checkInDate: string;
  checkOutDate: string;
  couponCode?: string;
}

export interface BookingResponse {
  bookingId: number;
  reservationNumber: string;
  hotelName: string;
  roomType: string;
  checkInDate: string;
  checkOutDate: string;
  totalAmount: number;
  discountAmount: number;
  couponCode: string;
}

export interface BookingHistory {
  bookingId: number;
  reservationNumber: string;
  hotelName: string;
  hotelLocation: string;
  roomType: string;
  roomId: number;
  checkInDate: string;
  checkOutDate: string;
  totalAmount: number;
  discountAmount: number;
  couponCode: string;
  createdAt: string;
}

export interface AvailabilityResponse {
  isAvailable: boolean;
}
