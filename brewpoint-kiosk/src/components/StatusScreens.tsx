import styles from './StatusScreens.module.css';

/** Full-canvas loading state (menu fetch). */
export function LoadingScreen({ label = 'Loading menu…' }: { label?: string }) {
  return (
    <div className={styles.screen}>
      <span className={styles.spinner} />
      <div className={styles.text}>{label}</div>
    </div>
  );
}

interface ErrorScreenProps {
  title: string;
  detail?: string;
  onRetry?: () => void;
}

/** Full-canvas error state with an optional retry (menu down, etc.). */
export function ErrorScreen({ title, detail, onRetry }: ErrorScreenProps) {
  return (
    <div className={styles.screen}>
      <div className={styles.errorMark}>!</div>
      <div className={styles.errorTitle}>{title}</div>
      {detail && <div className={styles.text}>{detail}</div>}
      {onRetry && (
        <button type="button" className={styles.retry} onClick={onRetry}>
          Try again
        </button>
      )}
    </div>
  );
}
