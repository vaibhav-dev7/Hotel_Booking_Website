export interface SearchResult {
  hotelId: number;
  hotelName: string;
  location: string;
  roomId: number;
  roomType: string;
  pricePerNight: number;
  capacity: number;
  amenities: string[];
}

export interface SearchFilters {
  location?: string;
  minPrice?: number;
  maxPrice?: number;
  amenities?: string;
  checkIn?: string;
  checkOut?: string;
}
