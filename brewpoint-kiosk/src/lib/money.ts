/** Currency formatting — Ukrainian hryvnia, shown as "<amount> ₴". */

const HRYVNIA = '₴'; // ₴

/**
 * Format a price the way the design does: whole numbers show with no decimals
 * ("45 ₴"), fractional amounts keep two places ("45.50 ₴").
 */
export function money(amount: number): string {
  const rounded = Math.round(amount * 100) / 100;
  const text = Number.isInteger(rounded)
    ? String(rounded)
    : rounded.toFixed(2);
  return `${text} ${HRYVNIA}`;
}

/** Size delta label for the size pills: "incl." for 0, otherwise "+<n> ₴". */
export function deltaLabel(priceModifier: number): string {
  return priceModifier === 0 ? 'incl.' : `+${money(priceModifier)}`;
}
