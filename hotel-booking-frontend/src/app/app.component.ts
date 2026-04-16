import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent],
  template: `
    <!-- The Navbar stays locked to the top of EVERY page -->
    <app-navbar></app-navbar>

    <!-- The Router renders the current page content here organically -->
    <router-outlet></router-outlet>
  `
})
export class AppComponent {
  title = 'hotel-booking-frontend';
}
