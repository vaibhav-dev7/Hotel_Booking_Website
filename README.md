# 🏨 Hotel Booking System

> **Full-Stack Hotel Booking Website** — Enable customers to browse, select, and book hotel rooms seamlessly while ensuring secure, efficient operations.

---

## 📋 Project Overview

| Item | Detail |
|------|--------|
| **Project Type** | Full-Stack Web Application |
| **Backend** | ASP.NET 8 Web API |
| **Frontend** | Angular 21 |
| **Database** | MySQL |
| **Authentication** | JWT (JSON Web Tokens) |
| **API Documentation** | Swagger / Postman |

---

## 🎯 Problem Statement

Users currently have to manually search hotels across different platforms with no real-time availability, confusing booking processes, and no centralized place for offers, booking history, or quick rebooking.

**This system solves it by providing a single platform to:**
- Search and browse hotels with filters
- Check room availability in real-time
- Book rooms securely with coupon support
- View booking history and rebook with one click

---

## 👥 System Actors

| Actor | Role | Key Actions |
|-------|------|-------------|
| **Customer** | Primary user | Search hotels, filter, check availability, book rooms, apply coupons, view history, rebook |
| **Admin** | System manager | Add/manage hotels, rooms, amenities, coupons |
| **System** | Background logic | JWT auth, availability validation, coupon validation, price calculation, email notification |

---

## 🧩 Core Modules

| # | Module | Purpose |
|---|--------|---------|
| 1 | Authentication & Authorization | JWT login, registration, role-based access (User/Admin) |
| 2 | Hotel Management | CRUD operations for hotels |
| 3 | Room Management | CRUD for rooms + amenities mapping |
| 4 | Search & Filter | Dynamic multi-criteria filtering (location, dates, price, amenities) |
| 5 | Availability Management | Date-overlap logic to prevent double booking |
| 6 | Booking (Core) | Create booking, calculate price, generate reservation number |
| 7 | Coupon / Promotion | Validate & apply discount codes |
| 8 | Booking History | View past bookings + quick rebook |
| 9 | Email Notification | Send booking confirmation emails |
| 10 | Security & Rate Limiting | JWT middleware, API rate limiting |

---

## 🗄️ Database Schema (MySQL — 7 Tables)

### Users
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| UserId | INT | PK, AUTO_INCREMENT | Unique user ID |
| Name | VARCHAR(100) | NOT NULL | Full name |
| Email | VARCHAR(150) | UNIQUE, NOT NULL | Login email |
| PasswordHash | VARCHAR(255) | NOT NULL | Hashed password |
| Role | VARCHAR(20) | NOT NULL, DEFAULT 'User' | User / Admin |
| CreatedAt | DATETIME | DEFAULT CURRENT_TIMESTAMP | Registration time |

### Hotels
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| HotelId | INT | PK, AUTO_INCREMENT | Unique hotel ID |
| Name | VARCHAR(150) | NOT NULL | Hotel name |
| Location | VARCHAR(150) | NOT NULL | City / area |
| Description | TEXT | NULLABLE | Hotel details & facilities |

### Rooms
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| RoomId | INT | PK, AUTO_INCREMENT | Unique room ID |
| HotelId | INT | FK → Hotels(HotelId) | Parent hotel |
| RoomType | VARCHAR(100) | NOT NULL | Deluxe / Standard / Suite |
| PricePerNight | DECIMAL(10,2) | NOT NULL | Cost per night |
| Capacity | INT | NOT NULL | Max guests |
| IsActive | BOOLEAN | DEFAULT TRUE | Availability flag |

### Amenities
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| AmenityId | INT | PK, AUTO_INCREMENT | Unique amenity ID |
| Name | VARCHAR(100) | NOT NULL | WiFi, AC, Pool, etc. |

### RoomAmenities (Junction Table)
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| RoomId | INT | PK, FK → Rooms | Room reference |
| AmenityId | INT | PK, FK → Amenities | Amenity reference |

> Composite Primary Key: (RoomId, AmenityId) — Many-to-Many relationship

### Bookings
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| BookingId | INT | PK, AUTO_INCREMENT | Unique booking ID |
| UserId | INT | FK → Users(UserId) | Booker |
| RoomId | INT | FK → Rooms(RoomId) | Booked room |
| CheckInDate | DATE | NOT NULL | Check-in date |
| CheckOutDate | DATE | NOT NULL | Check-out date |
| TotalAmount | DECIMAL(10,2) | NOT NULL | Final price after discount |
| CouponId | INT | FK → Coupons, NULLABLE | Applied coupon |
| DiscountAmount | DECIMAL(10,2) | DEFAULT 0 | Discount value |
| ReservationNumber | VARCHAR(100) | UNIQUE, NOT NULL | Booking reference code |
| CreatedAt | DATETIME | DEFAULT CURRENT_TIMESTAMP | Booking timestamp |

### Coupons
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| CouponId | INT | PK, AUTO_INCREMENT | Unique coupon ID |
| Code | VARCHAR(50) | UNIQUE, NOT NULL | Coupon code |
| DiscountPercentage | DECIMAL(5,2) | NOT NULL | Discount % |
| ExpiryDate | DATE | NOT NULL | Expiration date |
| IsActive | BOOLEAN | DEFAULT TRUE | Active/inactive |

### Entity Relationships
```
Users ──(1:N)──> Bookings
Hotels ──(1:N)──> Rooms
Rooms ──(1:N)──> Bookings
Rooms ──(M:N)──> Amenities  (via RoomAmenities)
Coupons ──(1:N)──> Bookings  (optional)
```

---

## 🔌 REST API Design

### Auth APIs
| Endpoint | Method | Auth | Service | Purpose |
|----------|--------|------|---------|---------|
| `/api/auth/register` | POST | ❌ | AuthService → `RegisterUser()` | Create new user |
| `/api/auth/login` | POST | ❌ | AuthService → `LoginUser()` | Authenticate & return JWT |

### Hotel APIs
| Endpoint | Method | Auth | Service | Purpose |
|----------|--------|------|---------|---------|
| `/api/hotels` | GET | ❌ | HotelService → `GetAllHotels()` | Browse all hotels |
| `/api/hotels` | POST | ✅ Admin | HotelService → `AddHotel()` | Admin: add hotel |

### Room APIs
| Endpoint | Method | Auth | Service | Purpose |
|----------|--------|------|---------|---------|
| `/api/hotels/{hotelId}/rooms` | GET | ❌ | RoomService → `GetRoomsByHotel()` | Get rooms with amenities |
| `/api/rooms` | POST | ✅ Admin | RoomService → `AddRoom()` | Admin: add room |

### Search API
| Endpoint | Method | Auth | Service | Purpose |
|----------|--------|------|---------|---------|
| `/api/search` | GET | ❌ | SearchService → `SearchRooms()` | Dynamic multi-criteria filtering |

### Booking APIs
| Endpoint | Method | Auth | Service | Purpose |
|----------|--------|------|---------|---------|
| `/api/bookings` | POST | ✅ User | BookingService → `CreateBooking()` | Create booking |
| `/api/bookings/availability` | GET | ❌ | BookingService → `CheckAvailability()` | Check room availability |
| `/api/bookings/my` | GET | ✅ User | BookingService → `GetUserBookings()` | Booking history |
| `/api/bookings/rebook/{bookingId}` | POST | ✅ User | BookingService → `Rebook()` | Quick rebooking |

### Coupon APIs
| Endpoint | Method | Auth | Service | Purpose |
|----------|--------|------|---------|---------|
| `/api/coupons/validate` | POST | ✅ User | CouponService → `ValidateCoupon()` | Validate & return discount |
| `/api/coupons` | POST | ✅ Admin | CouponService → `CreateCoupon()` | Admin: create coupon |
| `/api/coupons/{id}/toggle` | PUT | ✅ Admin | CouponService → `ToggleCoupon()` | Admin: activate/deactivate |

---

## 🏗️ Backend Architecture (ASP.NET 8)

### Project Structure
```
HotelBookingAPI/
├── Controllers/
│   ├── AuthController.cs
│   ├── HotelsController.cs
│   ├── RoomsController.cs
│   ├── SearchController.cs
│   ├── BookingsController.cs
│   └── CouponsController.cs
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IHotelService.cs
│   │   ├── IRoomService.cs
│   │   ├── ISearchService.cs
│   │   ├── IBookingService.cs
│   │   ├── ICouponService.cs
│   │   └── IEmailService.cs
│   └── Implementations/
│       ├── AuthService.cs
│       ├── HotelService.cs
│       ├── RoomService.cs
│       ├── SearchService.cs
│       ├── BookingService.cs
│       ├── CouponService.cs
│       └── EmailService.cs
├── Models/
│   ├── User.cs
│   ├── Hotel.cs
│   ├── Room.cs
│   ├── Amenity.cs
│   ├── RoomAmenity.cs
│   ├── Booking.cs
│   └── Coupon.cs
├── DTOs/
│   ├── Auth/       (RegisterDto, LoginDto, AuthResponseDto)
│   ├── Hotel/      (HotelDto, CreateHotelDto)
│   ├── Room/       (RoomDto, CreateRoomDto)
│   ├── Booking/    (CreateBookingDto, BookingResponseDto, AvailabilityDto, BookingHistoryDto)
│   ├── Coupon/     (CreateCouponDto, ValidateCouponDto, CouponResponseDto)
│   └── Search/     (SearchFilterDto, SearchResultDto)
├── Data/
│   └── AppDbContext.cs
├── Middleware/
│   └── RateLimitingMiddleware.cs
├── Program.cs
└── appsettings.json
```

### Service Layer Functions

| Service | Functions |
|---------|-----------|
| **AuthService** | `RegisterUser()`, `LoginUser()` |
| **HotelService** | `GetAllHotels()`, `AddHotel()` |
| **RoomService** | `GetRoomsByHotel()`, `GetRoomById()`, `AddRoom()` |
| **SearchService** | `SearchRooms(filters)` |
| **BookingService** | `CreateBooking()`, `CheckAvailability()`, `GetUserBookings()`, `Rebook()` |
| **CouponService** | `ValidateCoupon()`, `CalculateDiscount()`, `CreateCoupon()`, `ToggleCoupon()` |
| **EmailService** | `SendBookingConfirmation()` |

---

## 🖥️ Frontend Architecture (Angular 21)

### Components (9 Total)

| # | Component | Purpose |
|---|-----------|---------|
| 1 | LoginComponent | User login form |
| 2 | RegisterComponent | User signup form |
| 3 | NavbarComponent | Project title, login/logout, search bar |
| 4 | HotelListComponent | Hotel cards + integrated filter UI |
| 5 | RoomListComponent | Room cards + availability check + "Book Now" |
| 6 | BookingComponent | Booking form + coupon + price calculation |
| 7 | BookingSuccessComponent | Reservation confirmation |
| 8 | MyBookingsComponent | Booking history + rebook |
| 9 | AdminDashboardComponent | Manage hotels, rooms, coupons |

### Angular Services & Security

| # | Name | Type | Functions |
|---|------|------|-----------|
| 1 | AuthService | Service | `login()`, `register()`, `logout()`, `getToken()`, `isLoggedIn()`, `getUserRole()` |
| 2 | HotelService | Service | `getAllHotels()`, `addHotel()` |
| 3 | RoomService | Service | `getRoomsByHotel()`, `addRoom()` |
| 4 | SearchService | Service | `searchRooms(filters)` |
| 5 | BookingService | Service | `createBooking()`, `checkAvailability()`, `getMyBookings()`, `rebook()` |
| 6 | CouponService | Service | `validateCoupon()`, `createCoupon()`, `toggleCoupon()` |
| 7 | AuthGuard | Guard | Protect routes — logged-in users only |
| 8 | AdminGuard | Guard | Protect routes — admin role only |
| 9 | AuthInterceptor | Interceptor | Auto-attach JWT token to HTTP requests |

### Component → Service Mapping

| Component | Service | Function | API |
|-----------|---------|----------|-----|
| LoginComponent | AuthService | `login()` | POST `/api/auth/login` |
| RegisterComponent | AuthService | `register()` | POST `/api/auth/register` |
| NavbarComponent | AuthService | `logout()`, `isLoggedIn()`, `getUserRole()` | — |
| HotelListComponent | HotelService | `getAllHotels()` | GET `/api/hotels` |
| HotelListComponent | SearchService | `searchRooms(filters)` | GET `/api/search` |
| RoomListComponent | RoomService | `getRoomsByHotel(id)` | GET `/api/hotels/{id}/rooms` |
| RoomListComponent | BookingService | `checkAvailability()` | GET `/api/bookings/availability` |
| BookingComponent | BookingService | `createBooking()` | POST `/api/bookings` |
| BookingComponent | CouponService | `validateCoupon()` | POST `/api/coupons/validate` |
| MyBookingsComponent | BookingService | `getMyBookings()` | GET `/api/bookings/my` |
| MyBookingsComponent | BookingService | `rebook()` | POST `/api/bookings/rebook/{id}` |
| AdminDashboardComponent | HotelService | `addHotel()` | POST `/api/hotels` |
| AdminDashboardComponent | RoomService | `addRoom()` | POST `/api/rooms` |
| AdminDashboardComponent | CouponService | `createCoupon()`, `toggleCoupon()` | POST/PUT `/api/coupons` |

### Frontend Routing

| Path | Component | Guard |
|------|-----------|-------|
| `/login` | LoginComponent | — |
| `/register` | RegisterComponent | — |
| `/hotels` | HotelListComponent | — |
| `/hotels/:hotelId/rooms` | RoomListComponent | — |
| `/booking/:roomId` | BookingComponent | AuthGuard |
| `/booking-success` | BookingSuccessComponent | AuthGuard |
| `/my-bookings` | MyBookingsComponent | AuthGuard |
| `/admin` | AdminDashboardComponent | AdminGuard |

### Frontend Project Structure
```
hotel-booking-frontend/
├── src/app/
│   ├── components/
│   │   ├── login/
│   │   ├── register/
│   │   ├── navbar/
│   │   ├── hotel-list/
│   │   ├── room-list/
│   │   ├── booking/
│   │   ├── booking-success/
│   │   ├── my-bookings/
│   │   └── admin-dashboard/
│   ├── services/
│   │   ├── auth.service.ts
│   │   ├── hotel.service.ts
│   │   ├── room.service.ts
│   │   ├── search.service.ts
│   │   ├── booking.service.ts
│   │   └── coupon.service.ts
│   ├── guards/
│   │   ├── auth.guard.ts
│   │   └── admin.guard.ts
│   ├── interceptors/
│   │   └── auth.interceptor.ts
│   ├── models/
│   ├── app.routes.ts
│   └── app.component.ts
```

---

## 🔐 Business Rules

| # | Rule | Validation | Layer |
|---|------|-----------|-------|
| 1 | Only authenticated users can book/view history | JWT token valid | Middleware |
| 2 | Hotel must exist before showing rooms | HotelId in DB | Service |
| 3 | Room must be active & belong to valid hotel | Room.IsActive + FK check | Service |
| 4 | Search dates: checkIn < checkOut | Date validation | Controller + Service |
| 5 | **No double booking** (date overlap check) | Query Bookings table | Service |
| 6 | Booking requires complete valid data | All fields present | Service |
| 7 | Coupon must be valid, active, and not expired | DB checks | Service |
| 8 | Email sent only after successful booking | Booking saved | Background Service |
| 9 | Users see only their own bookings | UserId match | Service |
| 10 | Rebook requires valid previous booking | Booking exists + belongs to user | Service |
| 11 | API rate limiting | Request count check | Middleware |
| 12 | Admin-only APIs protected | Role == Admin | Controller attribute |

---

## ✅ MVP Feature Checklist

| # | Use Case Requirement | Status |
|---|---------------------|--------|
| 1 | Browse hotels, rooms, and amenities | 🟢 Covered |
| 2 | Secure & efficient booking process | 🟢 Covered |
| 3 | Centralized portal: hotels, rooms, pricing, facilities | 🟢 Covered |
| 4 | Search and filter by location, dates, price, amenities | 🟢 Covered |
| 5 | Availability updates upon confirmed bookings | 🟢 Covered |
| 6 | Secure APIs (auth, authorization, rate limiting) | 🟢 Covered |
| 7 | REST endpoints validated via Postman/Swagger | 🟢 Covered |
| 8 | Code & design maintained in GitHub | 🟢 Covered |
| 9 | Email confirmation with booking details | 🟢 Covered |
| 10 | Booking history with quick rebooking | 🟢 Covered |
| 11 | Promotions: discount codes, loyalty, seasonal offers | 🟢 Covered |

---

## 🚀 Build Order

1. **Backend Foundation** → ASP.NET project + MySQL + EF Core + Models + Migrations
2. **Auth Module** → Register/Login + JWT
3. **Hotel & Room APIs** → CRUD + Amenities
4. **Search API** → Dynamic filtering
5. **Booking API** → Create + Availability + History + Rebook
6. **Coupon API** → Validate + Admin CRUD
7. **Email Service** → Booking confirmation
8. **Security** → Rate limiting
9. **Angular Setup** → Project + Routing + Services + Interceptor + Guards
10. **Frontend Components** → Login → Hotels → Rooms → Booking → History → Admin
11. **Polish & Test** → Swagger testing + E2E flows
12. **GitHub** → Initialize repo + push

---

## 📄 License

This project is built for educational / hackathon purposes.
