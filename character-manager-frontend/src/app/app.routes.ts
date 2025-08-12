import { Routes } from '@angular/router';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
    { path: '', component: LoginPageComponent },
    { path: 'landing', component: LandingPageComponent, canActivate: [authGuard] },
    { path: '**', redirectTo: '' } // catch-all redirect
];
