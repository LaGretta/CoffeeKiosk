import { useCallback, useEffect, useState } from 'react';
import { getMenu } from '../api/endpoints';
import type { MenuResponse } from '../api/types';
import { ApiError } from '../api/client';

type State =
  | { status: 'loading' }
  | { status: 'error'; error: ApiError }
  | { status: 'ready'; data: MenuResponse };

export interface UseMenu {
  state: State;
  reload: () => void;
}

/** Loads GET /api/menu with loading/error/ready states and a manual retry. */
export function useMenu(): UseMenu {
  const [state, setState] = useState<State>({ status: 'loading' });
  const [nonce, setNonce] = useState(0);

  const reload = useCallback(() => {
    setState({ status: 'loading' });
    setNonce((n) => n + 1);
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    getMenu(controller.signal)
      .then((data) => setState({ status: 'ready', data }))
      .catch((err) => {
        if (err instanceof DOMException && err.name === 'AbortError') return;
        const error =
          err instanceof ApiError
            ? err
            : new ApiError('Failed to load the menu.', 0, true);
        setState({ status: 'error', error });
      });
    return () => controller.abort();
  }, [nonce]);

  return { state, reload };
}
