import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { RatingService } from '../../services/rating.service';
import { CommentService } from '../../services/comment.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-games',
  templateUrl: './games.component.html',
  styleUrl: './games.component.css'
})
export class GamesComponent implements OnInit {
  games: any[] = [];
  filteredGames: any[] = [];
  genres: string[] = [];
  comments: any[] = [];

  searchTerm = '';
  selectedGenre = '';
  selectedRating = 0;

  isLoading = true;
  isLoggedIn = false;
  currentUser: any = null;
  errorMessage = '';

  newComment = '';
  isSubmittingComment = false;

  showAddGameModal = false;
  isSubmittingGame = false;
  newGame = {
    title: '',
    description: '',
    genre: '',
    thumbnailURL: '',
    downloadURL: ''
  };

  constructor(
    private gameService: GameService,
    private ratingService: RatingService,
    private commentService: CommentService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.checkAuthState();
    this.loadGames();
    this.loadGenres();
    this.loadComments();
  }

  checkAuthState(): void {
    this.authService.isLoggedIn$.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
      this.currentUser = this.authService.getCurrentUser();
    });
  }

  loadGames(): void {
    this.isLoading = true;
    this.gameService.GetAllGames(this.selectedGenre || undefined, this.selectedRating || undefined).subscribe({
      next: (response) => {
        if (response.status === 200 && response.data) {
          this.games = response.data;
          this.filteredGames = [...this.games];
          this.applySearch();
        } else {
          this.errorMessage = 'Failed to load games';
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading games:', error);
        this.errorMessage = 'Error loading games. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  loadGenres(): void {
    this.gameService.GetGenres().subscribe({
      next: (response) => {
        if (response.status === 200 && response.data) {
          this.genres = response.data;
        }
      },
      error: (error) => console.error('Error loading genres:', error)
    });
  }

  loadComments(): void {
    if (this.games.length > 0) {
      this.games.forEach(game => {
        this.commentService.GetGameComments(game.gameId).subscribe({
          next: (response) => {
            if (response.status === 200 && response.data) {
              this.comments = [...this.comments, ...response.data];
            }
          },
          error: (error) => console.error('Error loading comments:', error)
        });
      });
    }
  }

  onSearchInput(event: any): void {
    this.searchTerm = event.target.value;
    this.applySearch();
  }

  onGenreChange(): void {
    this.loadGames();
  }

  onRatingChange(): void {
    this.loadGames();
  }

  applySearch(): void {
    if (!this.searchTerm.trim()) {
      this.filteredGames = [...this.games];
      return;
    }

    this.filteredGames = this.games.filter(game =>
      game.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      game.description.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      game.genre.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  rateGame(game: any, rating: number): void {
    if (!this.isLoggedIn) {
      alert('Please sign in to rate games');
      return;
    }

    this.ratingService.AddRating(this.currentUser.accountId, game.gameId, rating).subscribe({
      next: (response) => {
        if (response.status === 200) {
          this.loadGames();
        }
      },
      error: (error) => {
        console.error('Error rating game:', error);
        alert('Failed to rate game. Please try again.');
      }
    });
  }

  getStarClass(gameRating: number, starNumber: number): string {
    if (gameRating >= starNumber) {
      return 'star filled';
    } else if (gameRating > starNumber - 1) {
      return 'star half-filled';
    }
    return 'star empty';
  }

  downloadGame(game: any): void {
    if (!this.isLoggedIn) {
      alert('Please sign in to download games');
      return;
    }

    this.gameService.DownloadGame(game.gameId).subscribe({
      next: (response) => {
        if (response.status === 200 && response.data?.downloadURL) {
          window.open(response.data.downloadURL, '_blank');
        }
      },
      error: (error) => {
        console.error('Download error:', error);
        alert('Please sign in to download games');
      }
    });
  }

  submitComment(): void {
    if (!this.isLoggedIn) {
      alert('Please sign in to comment');
      return;
    }

    if (!this.newComment.trim()) {
      return;
    }

    const gameToComment = this.games[0];
    if (!gameToComment) return;

    this.isSubmittingComment = true;
    this.commentService.AddComment(this.currentUser.accountId, gameToComment.gameId, this.newComment.trim()).subscribe({
      next: (response) => {
        if (response.status === 200) {
          this.newComment = '';
          this.loadComments();
        }
        this.isSubmittingComment = false;
      },
      error: (error) => {
        console.error('Error adding comment:', error);
        alert('Failed to add comment. Please try again.');
        this.isSubmittingComment = false;
      }
    });
  }

  deleteComment(commentId: number): void {
    if (!this.isLoggedIn) return;

    this.commentService.DeleteComment(commentId, this.currentUser.accountId).subscribe({
      next: (response) => {
        if (response.status === 200) {
          this.loadComments();
        }
      },
      error: (error) => {
        console.error('Error deleting comment:', error);
        alert('Failed to delete comment.');
      }
    });
  }

  canDeleteComment(comment: any): boolean {
    if (!this.isLoggedIn || !this.currentUser) return false;

    const userRole = this.currentUser.role;
    const commentRole = comment.role;

    if (comment.accountId === this.currentUser.accountId) return true;
    if (userRole === 'Creator') return true;
    if (userRole === 'Admin' && commentRole === 'User') return true;

    return false;
  }

  getGameComments(gameId: number): any[] {
    return this.comments.filter(comment => comment.gameId === gameId);
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedGenre = '';
    this.selectedRating = 0;
    this.loadGames();
  }

  onImageError(event: any): void {
    event.target.src = '/assets/images/default-game.png';
  }

  getGameTitle(gameId: number): string {
    const game = this.games.find(g => g.gameId === gameId);
    return game ? game.title : 'Unknown Game';
  }

  getUserRatingForGame(accountId: number, gameId: number): number {
    const userComment = this.comments.find(
      c => c.accountId === accountId && c.gameId === gameId
    );
    return userComment && userComment.rating ? userComment.rating : 0;
  }


  canAddGames(): boolean {
    if (!this.isLoggedIn || !this.currentUser) return false;
    return this.currentUser.role === 'Creator' || this.currentUser.role === 'Admin';
  }

  openAddGameModal(): void {
    this.showAddGameModal = true;
    this.newGame = {
      title: '',
      description: '',
      genre: '',
      thumbnailURL: '',
      downloadURL: ''
    };
  }

  closeAddGameModal(): void {
    this.showAddGameModal = false;
  }

  submitNewGame(): void {
    if (!this.isLoggedIn || !this.currentUser) {
      alert('Please sign in to add games');
      return;
    }

    if (!this.newGame.title.trim()) {
      alert('Please enter a game title');
      return;
    }
    if (!this.newGame.description.trim()) {
      alert('Please enter a game description');
      return;
    }
    if (!this.newGame.genre) {
      alert('Please select a genre');
      return;
    }
    if (!this.newGame.thumbnailURL.trim()) {
      alert('Please enter a thumbnail URL');
      return;
    }
    if (!this.newGame.downloadURL.trim()) {
      alert('Please enter a download URL');
      return;
    }

    this.isSubmittingGame = true;

    this.gameService.AddNewGame(this.newGame).subscribe({
      next: (response) => {
        if (response.status === 200) {
          alert('Game added successfully!');
          this.closeAddGameModal();
          this.loadGames();
        } else {
          alert('Failed to add game. Please try again.');
        }
        this.isSubmittingGame = false;
      },
      error: (error) => {
        console.error('Error adding game:', error);
        alert('Failed to add game. ' + (error.error?.message || 'Please try again.'));
        this.isSubmittingGame = false;
      }
    });
  }

  deleteGame(game: any): void {
    if (!this.canAddGames()) {
      alert('You do not have permission to delete games');
      return;
    }

    const confirmDelete = confirm(
      `Are you sure you want to delete "${game.title}"?\n\nThis action cannot be undone.`
    );

    if (!confirmDelete) {
      return;
    }

    this.gameService.DeleteGame(game.gameId).subscribe({
      next: (response) => {
        if (response.status === 200) {
          alert('Game deleted successfully!');
          this.loadGames();
        } else {
          alert('Failed to delete game. Please try again.');
        }
      },
      error: (error) => {
        console.error('Error deleting game:', error);
        alert('Failed to delete game. ' + (error.error?.message || 'Please try again.'));
      }
    });
  }
}
