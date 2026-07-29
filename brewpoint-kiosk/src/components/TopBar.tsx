import styles from './TopBar.module.css';

interface TopBarProps {
  onOpenSettings: () => void;
}

/** Header: logo mark + wordmark, tagline, and a discreet settings affordance. */
export function TopBar({ onOpenSettings }: TopBarProps) {
  return (
    <header className={styles.bar}>
      <div className={styles.brand}>
        <div className={styles.logo}>
          <span className={styles.logoRing} />
        </div>
        <div className={styles.wordmarkWrap}>
          <span className={styles.wordmark}>Brewpoint</span>
          <span className={styles.tagline}>Order &amp; Relax</span>
        </div>
      </div>

      <button
        type="button"
        className={styles.helper}
        onClick={onOpenSettings}
        aria-label="Kiosk settings"
        title="Kiosk settings"
      >
        <span className={styles.helperStrong}>Tap to order</span>
        <span className={styles.helperMuted}>Table service · Dine in</span>
      </button>
    </header>
  );
}
