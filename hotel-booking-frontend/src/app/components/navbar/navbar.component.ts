import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  authService = inject(AuthService);
  router = inject(Router);

  searchQuery = '';

  onSearch() {
    if (this.searchQuery.trim()) {
      // Pushes the 'location' query param into the global route system dynamically
      this.router.navigate(['/hotels'], { queryParams: { location: this.searchQuery.trim() } });
      this.searchQuery = ''; // Reset aesthetically
    }
  }

  logout() {
    // Erase token and drop Signal back to null
    this.authService.logout();
    this.router.navigate(['/login']); // Redirect securely
  }
}
