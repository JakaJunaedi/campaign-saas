import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgxSonnerToaster } from 'ngx-sonner';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgxSonnerToaster],
  template: `
    <router-outlet></router-outlet>
    <ngx-sonner-toaster richColors position="top-right" />
  `
})
export class AppComponent {}
