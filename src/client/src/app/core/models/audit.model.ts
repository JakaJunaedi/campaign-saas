export interface AuditLog {
  id: string;
  organizationId: string;
  userId?: string;
  actorEmail?: string;
  action: string;
  module: string;
  entityName: string;
  entityId: string;
  changesJson?: string;
  ipAddress?: string;
  timestamp: string;
}

export interface AuditPagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
