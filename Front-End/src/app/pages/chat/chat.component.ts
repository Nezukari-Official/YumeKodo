import { Component, OnInit, OnDestroy, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { ChatService } from '../../services/chat.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('chatWindow') private chatWindow!: ElementRef;
  @ViewChild('messageInput') private messageInput!: ElementRef;

  messages: any[] = [];
  currentMessage = '';
  isLoading = false;
  private subscription: Subscription = new Subscription();

  constructor(private ChatService: ChatService) {}

  ngOnInit(): void {
    this.subscription.add(
      this.ChatService.messages$.subscribe(messages => {
        this.messages = messages;
      })
    );
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  sendMessage(): void {
    if (!this.currentMessage.trim() || this.isLoading) {
      return;
    }

    const messageToSend = this.currentMessage.trim();
    this.currentMessage = '';
    this.isLoading = true;

    this.subscription.add(
      this.ChatService.Chat(messageToSend).subscribe({
        next: (response) => {
          this.isLoading = false;
          console.log('API Response:', response);
          
          if (response.status === 200 && response.data) {
            const reply = response.data.reply || response.data.Reply;
            const downloadLink = response.data.downloadLink;
            
            if (reply) {
              this.ChatService.addMessage(reply, false, downloadLink);
            }
          } else {
            this.ChatService.addMessage("Sorry, I couldn't process that. Please try again!", false);
          }
        },
        error: (error) => {
          this.isLoading = false;
          console.error('Chat error:', error);
          this.ChatService.addMessage("Oops! Something went wrong. Please check your connection and try again!", false);
        }
      })
    );
  }

  onKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  clearChat(): void {
    this.ChatService.clearChat();
  }

  private scrollToBottom(): void {
    try {
      if (this.chatWindow) {
        this.chatWindow.nativeElement.scrollTop = this.chatWindow.nativeElement.scrollHeight;
      }
    } catch (err) {
      console.error('Error scrolling to bottom:', err);
    }
  }
}
