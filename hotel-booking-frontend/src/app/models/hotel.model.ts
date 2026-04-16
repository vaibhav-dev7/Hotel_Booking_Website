export interface Hotel {
  hotelId: number;
  name: string;
  location: string;
  description: string;
}

export interface CreateHotel {
  name: string;
  location: string;
  description: string;
}
