export interface Product {
  id: number;
  name: string;
  price: number;
  stockQuantity: number;
  category: {
    id: number;
    name: string;
  };
}