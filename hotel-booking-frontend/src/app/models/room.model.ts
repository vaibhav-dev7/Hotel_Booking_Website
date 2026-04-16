export interface Room {
  roomId: number;
  hotelId: number;
  roomType: string;
  pricePerNight: number;
  capacity: number;
  isActive: boolean;
  amenities: string[];
}

export interface CreateRoom {
  hotelId: number;
  roomType: string;
  pricePerNight: number;
  capacity: number;
  amenityIds: number[];
}
