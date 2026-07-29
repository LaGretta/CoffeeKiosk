/**
 * Kiosk session store.
 *
 * A real kiosk is signed in ONCE by staff and stays signed in for the session.
 * We keep the JWT in memory and mirror it to sessionStorage so a page refresh
 * during a shift doesn't force re-login. sessionStorage (not localStorage) is
 * deliberate: the token is a short-lived session secret and should not persist
 * as a long-term credential on the device. See README for the rationale.
 */

import type { AuthResponse } from '../api/types';

const STORAGE_KEY = 'brewpoint.kiosk.session';

export interface KioskSession {
  token: string;
  username: string;
  role: string;
}

type Listener = (session: KioskSession | null) => void;

function load(): KioskSession | null {
  try {
    const raw = sessionStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const parsed = JSON.parse(raw) as Partial<KioskSession>;
    if (parsed && typeof parsed.token === 'string' && parsed.token) {
      return {
        token: parsed.token,
        username: parsed.username ?? 'kiosk',
        role: parsed.role ?? 'Kiosk',
      };
    }
  } catch {
    /* ignore corrupt storage */
  }
  return null;
}

let current: KioskSession | null = load();
const listeners = new Set<Listener>();

function emit(): void {
  for (const listener of listeners) listener(current);
}

export const authStore = {
  get(): KioskSession | null {
    return current;
  },

  getToken(): string | null {
    return current?.token ?? null;
  },

  signIn(auth: AuthResponse): void {
    current = { token: auth.token, username: auth.username, role: auth.role };
    try {
      sessionStorage.setItem(STORAGE_KEY, JSON.stringify(current));
    } catch {
      /* storage may be unavailable; in-memory session still works */
    }
    emit();
  },

  signOut(): void {
    current = null;
    try {
      sessionStorage.removeItem(STORAGE_KEY);
    } catch {
      /* ignore */
    }
    emit();
  },

  subscribe(listener: Listener): () => void {
    listeners.add(listener);
    return () => listeners.delete(listener);
  },
};
