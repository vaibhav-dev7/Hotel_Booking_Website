import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { HotelService } from '../../services/hotel.service';
import { SearchService } from '../../services/search.service';
import { Hotel } from '../../models/hotel.model';
import { SearchFilters, SearchResult } from '../../models/search.model';

@Component({
  selector: 'app-hotel-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './hotel-list.component.html',
  styleUrls: ['./hotel-list.component.css']
})
export class HotelListComponent implements OnInit {
  hotelService = inject(HotelService);
  searchService = inject(SearchService);
  route = inject(ActivatedRoute);
  fb = inject(FormBuilder);

  // ==================== SIGNALS FOR STATE MANAGEMENT ====================

  // signal() creates a reactive primitive — when its value changes,
  // Angular automatically updates ONLY the parts of the template that read it.
  // No need for Zone.js change detection to scan the entire component tree.

  hotels = signal<Hotel[]>([]);                   // stores all hotels (unfiltered view)
  searchResults = signal<SearchResult[]>([]);      // stores filtered search results
  isFiltered = signal<boolean>(false);             // toggle: which grid to show
  isLoading = signal<boolean>(true);               // controls spinner visibility

  // computed() derives a value from other signals — recalculates ONLY when
  // the signals it reads change. Like a getter, but smarter (cached).
  // This replaces the old inline template expressions like "hotels.length"
  hotelCount = computed(() => this.hotels().length);
  resultCount = computed(() => this.searchResults().length);

  // ==================== NON-SIGNAL STATE (STATIC / FORM) ====================

  todayDate: string;                              // static — set once in constructor, never changes
  filterForm!: FormGroup;    // it? basically tell that this value is not initialized yet but it will be assigned later before using it in ngonit                       // reactive form — has its own change tracking system
  amenitiesList: string[] = ['WiFi', 'AC', 'TV', 'Pool', 'Gym', 'Parking', 'Restaurant', 'Mini Bar'];

  constructor() {
    // Generate YYYY-MM-DD native HTML format strictly for input[type="date"]
    const today = new Date();
    this.todayDate = today.toISOString().split('T')[0];
  }

  ngOnInit(): void {
    this.initFilterForm(); //initialize the form form must be intilized before used  if we use before intilization it may crash 

    // We subscribe to the URL Query! If a user typed a search on the Navbar, it catches here natively
    this.route.queryParams.subscribe(params => {
      if (params['location']) {
        this.filterForm.patchValue({ location: params['location'] }); //patch value is used to update the value of the form control
        this.applyFilters();
      } else {
        this.loadAllHotels();
      }
    });
  }

  // Builds the reactive form with all filter controls + nested amenities FormGroup
  initFilterForm() {
    // Build amenities group dynamically: { WiFi: false, AC: false, ... }
    const amenitiesGroup: { [key: string]: boolean } = {}; //empty object for amenites
    this.amenitiesList.forEach(a => amenitiesGroup[a] = false); // setting object in amenties from amenties list 

    this.filterForm = this.fb.group({
      location: [''],  //form builder automatically converts it to formcontrol
      minPrice: [null],
      maxPrice: [null],
      checkIn: [''],
      checkOut: [''],
      amenities: this.fb.group(amenitiesGroup) //nested form 
    });
  }

  loadAllHotels() {
    this.isFiltered.set(false);   // .set() updates a signal's value
    this.isLoading.set(true);
    this.hotelService.getAllHotels().subscribe({
      next: (data) => {
        this.hotels.set(data);    // .set() replaces the entire array
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error fetching hotels', err);
        this.isLoading.set(false);
      }
    });
  }

  // Ensures users cannot check out before they literally check in!
  get minCheckOutDate(): string {
    const checkIn = this.filterForm?.get('checkIn')?.value;
    if (!checkIn) return this.todayDate;
    const checkInDate = new Date(checkIn);
    checkInDate.setDate(checkInDate.getDate() + 1);
    return checkInDate.toISOString().split('T')[0];
  }

  applyFilters() {
    this.isLoading.set(true);
    this.isFiltered.set(true);

    const formValue = this.filterForm.value;

    // Filter amenity keys where value == true, join them via comma
    const selectedAmenities = Object.keys(formValue.amenities)
      .filter((key) => formValue.amenities[key])
      .join(','); // convert object into comma seprated string 

    const searchFilters: SearchFilters = {
      location: formValue.location || undefined, //if empty string it will send undefined  its like we left value is null use right value 
      minPrice: formValue.minPrice || undefined,
      maxPrice: formValue.maxPrice || undefined,
      checkIn: formValue.checkIn || undefined,
      checkOut: formValue.checkOut || undefined,
      amenities: selectedAmenities || undefined
    };

    this.searchService.searchRooms(searchFilters).subscribe({
      next: (results) => {
        this.searchResults.set(results);   // update signal with search results
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Search error', err);
        this.isLoading.set(false);
      }
    });
  }

  clearAll() {
    // Reset the entire form back to initial values
    this.filterForm.reset({
      location: '',
      minPrice: null, //String case we have empty string for empty
      maxPrice: null, //integr case we have null for empty
      checkIn: '',
      checkOut: '',
      amenities: this.amenitiesList.reduce((acc, a) => ({ ...acc, [a]: false }), {})
    }); //reduce converts array into obj

    // Reload core dataset safely without URL params
    this.loadAllHotels();
  }
}
