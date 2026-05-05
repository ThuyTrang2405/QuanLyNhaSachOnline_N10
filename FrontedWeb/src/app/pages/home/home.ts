import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookCard } from '../../components/book-card/book-card';
import { BOOKS } from '../../services/book';

@Component({
  standalone: true,
  selector: 'app-home',
  imports: [CommonModule, BookCard],
  templateUrl: './home.html'
})
export class HomeComponent {
  books = BOOKS;
}