import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-verify-email',
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.css']
})
export class VerifyEmailComponent implements OnInit {
  email = '';
  verificationCode = '';
  isVerifying = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['email']) {
        this.email = params['email'];
      }
    });
  }

  verifyEmail(): void {
    if (!this.email.trim()) {
      this.errorMessage = 'Please enter your email address';
      return;
    }

    if (!this.verificationCode.trim()) {
      this.errorMessage = 'Please enter the verification code';
      return;
    }

    if (this.verificationCode.length !== 4) {
      this.errorMessage = 'Verification code must be 4 digits';
      return;
    }

    this.isVerifying = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.VerifyEmail(this.email, this.verificationCode).subscribe({
      next: (response) => {
        if (response.status === 200) {
          this.successMessage = 'Email verified successfully! Redirecting to sign in...';
          setTimeout(() => {
            this.router.navigate(['/sign-in']);
          }, 2000);
        } else {
          this.errorMessage = response.message || 'Verification failed. Please try again.';
        }
        this.isVerifying = false;
      },
      error: (error) => {
        console.error('Verification error:', error);
        this.errorMessage = error.error?.message || 'Wrong verification code or email. Please try again.';
        this.isVerifying = false;
      }
    });
  }
  resendCode(): void {
    if (!this.email.trim()) {
      this.errorMessage = 'Please enter your email address first';
      return;
    }
    alert('Code resent! Please check your email.');
  }
  onCodeInput(event: any): void {
    const input = event.target.value;
    this.verificationCode = input.replace(/[^0-9]/g, '').slice(0, 4);
  }
}
