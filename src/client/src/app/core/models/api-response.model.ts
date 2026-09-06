export interface ApiResponse<T> {
  success: boolean;
  data: T;
  error?: any;
  timestamp: string;
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}
