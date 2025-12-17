import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent {

  signInData = {
    Email: '',
    PasswordHash: '',
  };

  constructor(private authService: AuthService) { }

  onSubmit() {
    this.authService.SignIn(this.signInData).subscribe({
      next: (response) => {
        console.log('Sign in success:', response);
        alert(response.response?.message || 'Sign In success!')
      },
      error: (error) => {
        console.error('Sign in failed:', error);
        alert(error.error?.message || 'Sign in failed!');
      }
    });

    const payload = {
      Email: this.signInData.Email,
      PasswordHash: this.signInData.PasswordHash
    };

  }
}
