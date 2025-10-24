import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../services/product.service';
import { Product } from '../models/product.model';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent {
  products: Product[] = [];
  pagedProducts: Product[] = [];
  errorMessage = '';

  currentPage = 1;
  pageSize = 5;
  totalPages = 1;

  constructor(private productService: ProductService) {
    this.productService.getProducts().subscribe({
      next: data => {
        this.products = data;
        this.totalPages = Math.ceil(this.products.length / this.pageSize);
        this.updatePagedProducts();
      },
      error: err => this.errorMessage = err.message
    });
  }

  updatePagedProducts(): void {
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.pagedProducts = this.products.slice(start, end);
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updatePagedProducts();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePagedProducts();
    }
  }
}