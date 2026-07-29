/**
 * The API has no product imagery, so we reproduce the design's warm two-tone
 * "disc" placeholder per product. Known products from the design get their exact
 * palette; anything else is assigned a deterministic on-brand palette by
 * category so new menu items still look intentional. Swap for real photography
 * in production (see design handoff).
 */

import type { Category } from '../api/types';

export interface ProductVisual {
  bg: string;
  disc: string;
  discInner: string;
}

/** Exact palettes from the design, keyed by normalized product name. */
const BY_NAME: Record<string, ProductVisual> = {
  espresso: { bg: '#EBDCC7', disc: '#3A2318', discInner: '#5A3A26' },
  americano: { bg: '#EBDCC7', disc: '#4A2E1E', discInner: '#6B452E' },
  latte: { bg: '#EFE3D2', disc: '#8A5A38', discInner: '#C9A97E' },
  cappuccino: { bg: '#EFE3D2', disc: '#7A4E30', discInner: '#D8C0A0' },
  'flat white': { bg: '#EFE3D2', disc: '#9A6A44', discInner: '#D6BE9C' },
  'green tea': { bg: '#E4EBDB', disc: '#5E7A46', discInner: '#94B06E' },
  'masala chai': { bg: '#EDE0CE', disc: '#8A5A34', discInner: '#C79A64' },
  'matcha latte': { bg: '#E4EBD6', disc: '#6E8C3E', discInner: '#AEC77A' },
  croissant: { bg: '#F0E4CE', disc: '#C79A5A', discInner: '#E6C88E' },
  cheesecake: { bg: '#F1E7D6', disc: '#E0C99A', discInner: '#F4E6C6' },
  'blueberry muffin': { bg: '#E7E0EC', disc: '#6B5A86', discInner: '#A794C2' },
};

/** Fallback palettes so unknown products still fit the category's mood. */
const BY_CATEGORY: Record<Category, ProductVisual[]> = {
  Coffee: [
    { bg: '#EBDCC7', disc: '#3A2318', discInner: '#5A3A26' },
    { bg: '#EFE3D2', disc: '#8A5A38', discInner: '#C9A97E' },
    { bg: '#EFE3D2', disc: '#7A4E30', discInner: '#D8C0A0' },
    { bg: '#EBDCC7', disc: '#4A2E1E', discInner: '#6B452E' },
  ],
  Tea: [
    { bg: '#E4EBDB', disc: '#5E7A46', discInner: '#94B06E' },
    { bg: '#EDE0CE', disc: '#8A5A34', discInner: '#C79A64' },
    { bg: '#E4EBD6', disc: '#6E8C3E', discInner: '#AEC77A' },
  ],
  Dessert: [
    { bg: '#F0E4CE', disc: '#C79A5A', discInner: '#E6C88E' },
    { bg: '#F1E7D6', disc: '#E0C99A', discInner: '#F4E6C6' },
    { bg: '#E7E0EC', disc: '#6B5A86', discInner: '#A794C2' },
  ],
};

const NEUTRAL: ProductVisual = {
  bg: '#EFE3D2',
  disc: '#8A5A38',
  discInner: '#C9A97E',
};

function hash(text: string): number {
  let h = 0;
  for (let i = 0; i < text.length; i++) {
    h = (h * 31 + text.charCodeAt(i)) | 0;
  }
  return Math.abs(h);
}

export function productVisual(name: string, category: Category): ProductVisual {
  const known = BY_NAME[name.trim().toLowerCase()];
  if (known) return known;

  const palette = BY_CATEGORY[category] ?? [];
  if (palette.length === 0) return NEUTRAL;
  return palette[hash(name) % palette.length];
}
