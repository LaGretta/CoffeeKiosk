import { useEffect, useState } from 'react';
import { getOrder } from '../api/endpoints';
import type { Order } from '../api/types';
import { isTerminal } from '../lib/status';

const POLL_MS = 4000;

export interface OrderStatusState {
  order: Order | null;
  /** A transient polling error (e.g. one dropped request); polling continues. */
  error: string | null;
}

/**
 * Polls GET /api/orders/{number} every few seconds so the customer sees the
 * order progress live. Seeds with the order returned at checkout, stops polling
 * once the order reaches a terminal status. Pass null to disable.
 */
export function useOrderStatus(
  orderNumber: string | null,
  initial: Order | null,
): OrderStatusState {
  const [order, setOrder] = useState<Order | null>(initial);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setOrder(initial);
    setError(null);
    if (!orderNumber) return;

    let cancelled = false;
    let timer: ReturnType<typeof setTimeout> | undefined;
    const controller = new AbortController();

    const tick = async () => {
      try {
        const latest = await getOrder(orderNumber, controller.signal);
        if (cancelled) return;
        setOrder(latest);
        setError(null);
        if (isTerminal(latest.status)) return; // stop scheduling
      } catch (err) {
        if (cancelled || (err instanceof DOMException && err.name === 'AbortError'))
          return;
        setError('Reconnecting…');
      }
      timer = setTimeout(tick, POLL_MS);
    };

    timer = setTimeout(tick, POLL_MS);

    return () => {
      cancelled = true;
      controller.abort();
      if (timer) clearTimeout(timer);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [orderNumber]);

  return { order, error };
}
