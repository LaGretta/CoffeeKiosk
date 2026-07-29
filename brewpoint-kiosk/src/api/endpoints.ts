/** Typed calls for every endpoint the kiosk consumes. */

import { request } from './client';
import type {
  AuthResponse,
  CreateOrder,
  MenuResponse,
  Order,
} from './types';

/** POST /api/auth/login — staff signs the kiosk in once. */
export function login(
  username: string,
  password: string,
  signal?: AbortSignal,
): Promise<AuthResponse> {
  return request<AuthResponse>('/api/auth/login', {
    method: 'POST',
    body: { username, password },
    signal,
  });
}

/** GET /api/menu — public. Returns products + sizes. */
export function getMenu(signal?: AbortSignal): Promise<MenuResponse> {
  return request<MenuResponse>('/api/menu', { signal });
}

/** POST /api/orders — requires a Kiosk/Staff bearer token. */
export function createOrder(
  order: CreateOrder,
  token: string,
  signal?: AbortSignal,
): Promise<Order> {
  return request<Order>('/api/orders', {
    method: 'POST',
    body: order,
    token,
    signal,
  });
}

/** GET /api/orders/{number} — public status lookup by human order number. */
export function getOrder(
  orderNumber: string,
  signal?: AbortSignal,
): Promise<Order> {
  return request<Order>(`/api/orders/${encodeURIComponent(orderNumber)}`, {
    signal,
  });
}
