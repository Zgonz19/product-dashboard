import { Routes } from '@angular/router';
import { ProductListComponent } from './product-list/product-list.component';

export const productsRoutes: Routes = [
  {
    path: '',
    component: ProductListComponent
  }
];