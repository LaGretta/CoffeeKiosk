import { useSyncExternalStore } from 'react';
import { authStore, type KioskSession } from './authStore';

/** React binding for the kiosk session store. */
export function useAuth(): KioskSession | null {
  return useSyncExternalStore(authStore.subscribe, authStore.get);
}
