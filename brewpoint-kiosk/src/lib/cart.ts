/** Cart model + pure helpers. The server is the source of truth for the real
 *  total; these client-side sums are for live display only. */

import type { CreateOrder, Product, Size } from '../api/types';
import { productVisual, type ProductVisual } from './productVisuals';

export interface CartLine {
  /** `${productId}-${sizeId}` — merges identical product+size selections. */
  lineId: string;
  productId: number;
  sizeId: number;
  name: string;
  sizeName: string;
  /** Unit price = product.basePrice + size.priceModifier. */
  unit: number;
  qty: number;
  visual: ProductVisual;
}

export function lineIdFor(productId: number, sizeId: number): string {
  return `${productId}-${sizeId}`;
}

export function addLine(
  cart: CartLine[],
  product: Product,
  size: Size,
): CartLine[] {
  const lineId = lineIdFor(product.id, size.id);
  const existing = cart.find((l) => l.lineId === lineId);
  if (existing) {
    return cart.map((l) =>
      l.lineId === lineId ? { ...l, qty: l.qty + 1 } : l,
    );
  }
  const newLine: CartLine = {
    lineId,
    productId: product.id,
    sizeId: size.id,
    name: product.name,
    sizeName: size.name,
    unit: product.basePrice + size.priceModifier,
    qty: 1,
    visual: productVisual(product.name, product.category),
  };
  return [...cart, newLine];
}

export function changeQty(
  cart: CartLine[],
  lineId: string,
  delta: number,
): CartLine[] {
  return cart
    .map((l) => (l.lineId === lineId ? { ...l, qty: l.qty + delta } : l))
    .filter((l) => l.qty > 0);
}

export function cartCount(cart: CartLine[]): number {
  return cart.reduce((sum, l) => sum + l.qty, 0);
}

export function cartTotal(cart: CartLine[]): number {
  return cart.reduce((sum, l) => sum + l.unit * l.qty, 0);
}

export function toCreateOrder(cart: CartLine[]): CreateOrder {
  return {
    items: cart.map((l) => ({
      productId: l.productId,
      sizeId: l.sizeId,
      quantity: l.qty,
    })),
  };
}
