import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent {

  userData = {
    nickname: '',
    email: '',
    password: '',
  };

  repeatPassword: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  onSubmit() {
    if (this.userData.password !== this.repeatPassword) {
      alert('Passwords do not match!');
      return;
    }

    const payload = {
      Nickname: this.userData.nickname,
      Email: this.userData.email,
      PasswordHash: this.userData.password
    };

    this.authService.SignUp(payload).subscribe({
      next: (response) => {
        console.log('Sign up success:', response);
        if (response.status === 200) {
          this.router.navigate(['/verify-email'], { 
            queryParams: { email: this.userData.email } 
          });
        }
      },
      error: (error) => {
        console.error('Sign up failed:', error);
        alert('Sign up failed. Please try again.');
      }
    });
  }
}
