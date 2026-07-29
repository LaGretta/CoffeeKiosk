/** Presentation metadata for the live order status shown after checkout. */

import type { OrderStatus } from '../api/types';

/** The happy-path progression the customer watches (Cancelled is off-path). */
export const STATUS_FLOW: OrderStatus[] = [
  'Placed',
  'Paid',
  'Preparing',
  'Ready',
  'Completed',
];

interface StatusMeta {
  /** Short line under the order number. */
  label: string;
  /** Longer, friendlier line. */
  message: string;
  /** Show the animated "working" dots for this status. */
  active: boolean;
}

const META: Record<OrderStatus, StatusMeta> = {
  Placed: {
    label: 'Order received',
    message: 'We’ve got your order',
    active: true,
  },
  Paid: {
    label: 'Payment confirmed',
    message: 'Sending it to the bar',
    active: true,
  },
  Preparing: {
    label: 'Being prepared',
    message: 'Preparing your order',
    active: true,
  },
  Ready: {
    label: 'Ready for pickup',
    message: 'Please collect at the counter',
    active: false,
  },
  Completed: {
    label: 'Completed',
    message: 'Enjoy — see you again soon',
    active: false,
  },
  Cancelled: {
    label: 'Order cancelled',
    message: 'Please speak to a barista',
    active: false,
  },
};

export function statusMeta(status: OrderStatus): StatusMeta {
  return META[status] ?? META.Placed;
}

/** Index within STATUS_FLOW, or -1 for off-path statuses (e.g. Cancelled). */
export function statusStep(status: OrderStatus): number {
  return STATUS_FLOW.indexOf(status);
}

export function isTerminal(status: OrderStatus): boolean {
  return status === 'Completed' || status === 'Cancelled';
}
