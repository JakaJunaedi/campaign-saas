import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';

export const routes: Routes = [
  // Guest Routes (Public Auth)
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },

  // Authenticated App Shell Routes
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'clients',
        loadComponent: () => import('./features/clients/client-list/client-list.component').then(m => m.ClientListComponent)
      },
      {
        path: 'campaigns',
        loadComponent: () => import('./features/campaigns/campaign-list/campaign-list.component').then(m => m.CampaignListComponent)
      },
      {
        path: 'campaigns/:id',
        loadComponent: () => import('./features/campaigns/campaign-detail/campaign-detail.component').then(m => m.CampaignDetailComponent)
      },
      {
        path: 'creators',
        loadComponent: () => import('./features/creators/creator-hub.component').then(m => m.CreatorHubComponent)
      },
      {
        path: 'deliverables',
        loadComponent: () => import('./features/deliverables/deliverable-list.component').then(m => m.DeliverableListComponent)
      },
      {
        path: 'reports',
        loadComponent: () => import('./features/reports/reports-dashboard.component').then(m => m.ReportsDashboardComponent)
      },
      {
        path: 'audit-logs',
        loadComponent: () => import('./features/audit/audit-log-list.component').then(m => m.AuditLogListComponent)
      },
      {
        path: 'admin',
        loadComponent: () => import('./features/admin/admin-overview.component').then(m => m.AdminOverviewComponent)
      },
      {
        path: 'portal',
        loadComponent: () => import('./features/portal/creator-portal.component').then(m => m.CreatorPortalComponent)
      }
    ]
  },


  // Fallback
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
