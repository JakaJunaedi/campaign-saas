import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { toast } from 'ngx-sonner';
import { ApiResponse } from '../models/api-response.model';
import { InAppNotificationDto } from '../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);

  readonly notifications = signal<InAppNotificationDto[]>([]);
  readonly unreadCount = signal<number>(0);
  readonly isLoading = signal<boolean>(false);

  loadNotifications(unreadOnly: boolean = false, take: number = 20): Observable<ApiResponse<InAppNotificationDto[]>> {
    this.isLoading.set(true);
    const params: Record<string, string> = { take: take.toString() };
    if (unreadOnly) params['unreadOnly'] = 'true';

    return this.http.get<ApiResponse<InAppNotificationDto[]>>('/api/v1/notifications', { params }).pipe(
      tap(res => {
        this.isLoading.set(false);
        if (res.success && res.data) {
          this.notifications.set(res.data);
        }
      })
    );
  }

  loadUnreadCount(): Observable<ApiResponse<number>> {
    return this.http.get<ApiResponse<number>>('/api/v1/notifications/unread-count').pipe(
      tap(res => {
        if (res.success && typeof res.data === 'number') {
          this.unreadCount.set(res.data);
        }
      })
    );
  }

  markAsRead(notificationId: string): Observable<void> {
    return this.http.patch<void>(`/api/v1/notifications/${notificationId}/read`, {}).pipe(
      tap(() => {
        this.notifications.update(list =>
          list.map(n => n.id === notificationId ? { ...n, isRead: true, readAt: new Date().toISOString() } : n)
        );
        this.unreadCount.update(c => Math.max(0, c - 1));
      })
    );
  }

  markAllAsRead(): Observable<void> {
    return this.http.post<void>('/api/v1/notifications/mark-all-read', {}).pipe(
      tap(() => {
        this.notifications.update(list =>
          list.map(n => ({ ...n, isRead: true, readAt: new Date().toISOString() }))
        );
        this.unreadCount.set(0);
        toast.success('All notifications marked as read');
      })
    );
  }

  showSuccess(message: string, description?: string) {
    toast.success(message, { description });
  }

  showError(message: string, description?: string) {
    toast.error(message, { description });
  }

  showInfo(message: string, description?: string) {
    toast.info(message, { description });
  }
}
