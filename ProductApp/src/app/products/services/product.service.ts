import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Product } from '../models/product.model';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = `${environment.apiBaseUrl}api/products`;
  
  constructor(private http: HttpClient) {}

  getProducts(): Observable<Product[]> {
    const headers = new HttpHeaders({
      'x-api-key': environment.apiKey
    });

    return this.http.get<Product[]>(this.apiUrl, { headers }).pipe(
      catchError(error => {
        console.error('API error:', error);
        return throwError(() => new Error('Failed to load products'));
      })
    );
  }
}
