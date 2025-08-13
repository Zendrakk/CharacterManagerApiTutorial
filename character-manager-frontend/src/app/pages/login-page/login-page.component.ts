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
  private authService = inject(AuthService);
  private router = inject(Router);

  userDto: UserDto = {
    username: '',
    password: ''
  };

  error: string | null = null;

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/landing']);
    }
  }

  onSubmit() {
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
