import { API_BASE_URL } from '../api/client';
import type { KioskSession } from '../auth/authStore';
import styles from './SettingsSheet.module.css';

interface SettingsSheetProps {
  session: KioskSession;
  onSignOut: () => void;
  onClose: () => void;
}

/** Small staff-facing panel: shows the signed-in kiosk account and lets staff
 *  sign the device out. Reachable by tapping the header helper text. */
export function SettingsSheet({ session, onSignOut, onClose }: SettingsSheetProps) {
  return (
    <>
      <div className={styles.scrim} onClick={onClose} />
      <div className={styles.panel} role="dialog" aria-modal="true" aria-label="Kiosk settings">
        <div className={styles.header}>
          <div className={styles.title}>Kiosk settings</div>
          <button type="button" className={styles.close} onClick={onClose} aria-label="Close">
            ×
          </button>
        </div>

        <dl className={styles.rows}>
          <div className={styles.row}>
            <dt className={styles.key}>Signed in as</dt>
            <dd className={styles.value}>{session.username}</dd>
          </div>
          <div className={styles.row}>
            <dt className={styles.key}>Role</dt>
            <dd className={styles.value}>{session.role}</dd>
          </div>
          <div className={styles.row}>
            <dt className={styles.key}>API</dt>
            <dd className={styles.valueSmall}>{API_BASE_URL}</dd>
          </div>
        </dl>

        <button type="button" className={styles.signOut} onClick={onSignOut}>
          Sign out kiosk
        </button>
      </div>
    </>
  );
}
