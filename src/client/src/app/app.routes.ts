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
        loadComponent: () => import('./features/placeholders/feature-placeholder.component').then(m => m.FeaturePlaceholderComponent),
        data: { title: 'Campaign Reporting & PDF Engine', description: 'Manual metrics aggregation and asynchronous PDF export generation.' }
      },
      {
        path: 'audit-logs',
        loadComponent: () => import('./features/placeholders/feature-placeholder.component').then(m => m.FeaturePlaceholderComponent),
        data: { title: 'Security & Audit Trail', description: 'Cross-module tenant activity logging and audit trail.' }
      }
    ]
  },

  // Fallback
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
