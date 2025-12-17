import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit, OnDestroy {
  isOpen = false;
  currentUser: any = null;
  isLoggedIn = false;
  private subscriptions: Subscription = new Subscription();

  showEditModal = false;
  isUpdating = false;
  editForm = {
    nickname: '',
    email: '',
    profileImageURL: ''
  };
  updateMessage = '';
  updateError = '';

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.subscriptions.add(
      this.authService.currentUser$.subscribe(user => {
        this.currentUser = user;
      })
    );

    this.subscriptions.add(
      this.authService.isLoggedIn$.subscribe(loggedIn => {
        this.isLoggedIn = loggedIn;
        if (!loggedIn) {
          this.isOpen = false;
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  toggleSidebar(): void {
    if (this.isLoggedIn) {
      this.isOpen = !this.isOpen;
    }
  }

  closeSidebar(): void {
    this.isOpen = false;
  }

  editAccount(): void {
    this.editForm = {
      nickname: this.currentUser?.nickname || '',
      email: this.currentUser?.email || '',
      profileImageURL: this.currentUser?.profileImageURL || ''
    };
    this.showEditModal = true;
    this.updateMessage = '';
    this.updateError = '';
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.updateMessage = '';
    this.updateError = '';
  }

  saveProfileChanges(): void {

    console.log('Current User:', this.currentUser);
    console.log('Is Logged In:', this.isLoggedIn);

    if (!this.editForm.nickname.trim()) {
      this.updateError = 'Nickname cannot be empty';
      return;
    }

    if (!this.editForm.email.trim()) {
      this.updateError = 'Email cannot be empty';
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.editForm.email)) {
      this.updateError = 'Please enter a valid email address';
      return;
    }

    this.isUpdating = true;
    this.updateError = '';
    this.updateMessage = '';

    this.authService.UpdateProfile(
      this.currentUser.email,
      this.editForm.nickname,
      this.editForm.email,
      this.editForm.profileImageURL
    ).subscribe({
      next: (response) => {
        if (response.status === 200) {
          this.updateMessage = 'Profile updated! Please sign in again to see all changes.';

          setTimeout(() => {
            this.closeEditModal();
            this.authService.SignOut();
          }, 2500);
        } else {
          this.updateError = 'Failed to update profile. Please try again.';
        }
        this.isUpdating = false;
      },
      error: (error) => {
        console.error('Update profile error:', error);
        this.updateError = error.error?.message || 'Failed to update profile. Please try again.';
        this.isUpdating = false;
      }
    });
  }


  logout(): void {
    this.authService.SignOut();
    this.closeSidebar();
  }

  getRoleDisplayName(role: string): string {
    switch (role) {
      case 'Creator': return 'Creator';
      case 'Admin': return 'Admin';
      case 'User': return 'User';
      default: return 'User';
    }
  }

  getRoleBadgeClass(role: string): string {
    switch (role) {
      case 'Creator': return 'role-creator';
      case 'Admin': return 'role-admin';
      case 'User': return 'role-user';
      default: return 'role-user';
    }
  }
}