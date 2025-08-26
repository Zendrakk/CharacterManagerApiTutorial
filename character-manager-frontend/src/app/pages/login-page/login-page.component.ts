import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserDto } from '../../models/user-dto'

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent implements OnInit {
  // Inject the authentication service to handle login requests
  private authService = inject(AuthService);

  // Inject Angular's Router to enable programmatic navigation after login
  private router = inject(Router);

  // Holds any error message to display in the UI when login fails
  error: string | null = null;

  // Represents the login form data (username and password)
  userDto: UserDto = {
    username: '', // Bound to the username input field
    password: '' // Bound to the password input field
  };

  /**
   * Angular lifecycle hook that runs once when the component is initialized.
   * Checks if the user is already logged in via the AuthService. If the user is logged in, automatically redirects them to the '/landing' page.
   */
  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/landing']);
    }
  }

  /**
   * Handles the login form submission.
   * 
   * Clears any previous error messages and calls AuthService.login() with the current user DTO.
   * Subscribes to the login Observable:
   *   - On success: navigates the user to the '/landing' page.
   *   - On error: sets a descriptive error message based on the type and structure of the server response.
   *     - Uses ModelState errors if available (e.g., Username field errors).
   *     - Falls back to a generic server error message or unknown error message if needed.
   */
  public onSubmit() {
    this.error = ""
    this.authService.login(this.userDto).subscribe({
      next: () => {
        this.router.navigate(['/landing']);
      },
      error: (er) => {
        if (er.error && er?.error?.errors?.Username[0]) {
          this.error = er?.error?.errors.Username[0];
        } else if (er.error && er?.error?.type == 'error') {
          this.error = 'A server error occurred.';
        } else if (er.error) {
          this.error = er.error
        } else {
          this.error = 'An unknown error occurred.';
        }
      }
    });
  }
}
