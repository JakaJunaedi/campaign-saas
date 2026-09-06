export interface InAppNotificationDto {
  id: string;
  organizationId: string;
  userId: string;
  title: string;
  message: string;
  linkUrl?: string | null;
  isRead: boolean;
  createdAt: string;
  readAt?: string | null;
}
