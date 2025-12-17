import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private ApiUrl = 'https://localhost:7242/api/Yume';
  private messagesSubject = new BehaviorSubject<any[]>([]);
  public messages$ = this.messagesSubject.asObservable();

  constructor(private http: HttpClient) {
    this.addMessage("Hi! I'm Yume, your gaming assistant! Ask me about games or just chat!", false);
  }

  Chat(message: string): Observable<any> {
    const payload = { Message: message };
    this.addMessage(message, true);

    return this.http.post<any>(`${this.ApiUrl}/Chat`, payload);
  }

  addMessage(content: string, isUser: boolean, downloadLink?: string): void {
    const currentMessages = this.messagesSubject.value;
    const newMessage = {
      content: content,
      isUser: isUser,
      timestamp: new Date(),
      downloadLink: downloadLink
    };
    this.messagesSubject.next([...currentMessages, newMessage]);
  }

  getMessages(): any[] {
    return this.messagesSubject.value;
  }

  clearChat(): void {
    this.messagesSubject.next([]);
    this.addMessage("Hi! I'm Yume, your gaming assistant! Ask me about games or just chat!", false);
  }
}
