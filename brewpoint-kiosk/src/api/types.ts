/**
 * TypeScript mirrors of the CoffeeKiosk API DTOs.
 *
 * IMPORTANT — these match the REAL API shapes (verified against the .NET DTOs
 * and Program.cs), which differ from the original kiosk brief in two places:
 *   - GET /api/menu returns `{ menu, size }`, NOT `{ products, sizes }`.
 *   - Enums serialize as STRINGS (JsonStringEnumConverter is registered).
 */

export type Category = 'Coffee' | 'Tea' | 'Dessert';

export type OrderStatus =
  | 'Placed'
  | 'Paid'
  | 'Preparing'
  | 'Ready'
  | 'Completed'
  | 'Cancelled';

export type Role = 'Kiosk' | 'Staff' | 'Admin';

/** Item in GET /api/menu -> menu[] (ProductResponseDto). */
export interface Product {
  id: number;
  name: string;
  description: string;
  basePrice: number;
  category: Category;
  isAvailable: boolean;
}

/** Item in GET /api/menu -> size[] (SizeResponseDto). */
export interface Size {
  id: number;
  name: string;
  priceModifier: number;
}

/** GET /api/menu response. */
export interface MenuResponse {
  menu: Product[];
  size: Size[];
}

/** POST /api/auth/login response (AuthResponseDto). */
export interface AuthResponse {
  id: number;
  username: string;
  email: string;
  role: Role;
  token: string;
}

/** One line in a POST /api/orders body (CreateOrderItemDto). */
export interface CreateOrderItem {
  productId: number;
  sizeId: number;
  quantity: number;
}

/** POST /api/orders body (CreateOrderDto). */
export interface CreateOrder {
  items: CreateOrderItem[];
}

/** Line in an order response (OrderItemResponseDto). */
export interface OrderItem {
  productName: string;
  sizeName: string;
  quantity: number;
  unitPrice: number;
}

/** POST /api/orders + GET /api/orders/{number} response (OrderResponseDto). */
export interface Order {
  id: number;
  orderNumber: string;
  totalPrice: number;
  status: OrderStatus;
  createdAt: string;
  items: OrderItem[];
}
