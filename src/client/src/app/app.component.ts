import { Component, signal, inject, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  private http = inject(HttpClient);

  readonly title = signal('Campaign SaaS');
  readonly apiStatus = signal<'checking' | 'online' | 'offline'>('checking');
  readonly apiMessage = signal<string>('Connecting to API...');

  ngOnInit() {
    this.checkApiHealth();
  }

  checkApiHealth() {
    this.http.get<{ status: string, service: string }>('/api/').subscribe({
      next: (res) => {
        this.apiStatus.set('online');
        this.apiMessage.set(res.service || 'API Online');
      },
      error: () => {
        this.apiStatus.set('offline');
        this.apiMessage.set('Cannot connect to backend API (Port 5000)');
      }
    });
  }
}
