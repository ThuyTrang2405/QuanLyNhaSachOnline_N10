import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home';
import { BookDetail } from './pages/book-detail/book-detail';
import { Login } from './pages/login/login';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'book/:id', component: BookDetail },
    { path: 'login', component: Login }
];
