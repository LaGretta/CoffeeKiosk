import type { Order } from '../api/types';
import { CheckIcon } from './icons';
import { STATUS_FLOW, statusMeta, statusStep } from '../lib/status';
import styles from './Confirmation.module.css';

interface ConfirmationProps {
  order: Order;
  waitMins: number;
  pollError: string | null;
  onReset: () => void;
}

function displayNumber(orderNumber: string): string {
  return /^\d+$/.test(orderNumber) ? `#${orderNumber}` : orderNumber;
}

export function Confirmation({
  order,
  waitMins,
  pollError,
  onReset,
}: ConfirmationProps) {
  const meta = statusMeta(order.status);
  const step = statusStep(order.status);
  const cancelled = order.status === 'Cancelled';
  const ready = order.status === 'Ready';
  const showWait = !cancelled && order.status !== 'Ready' && order.status !== 'Completed';

  return (
    <div className={styles.screen}>
      <div className={`${styles.checkCircle} ${cancelled ? styles.checkCancelled : ''}`}>
        {cancelled ? (
          <span className={styles.cross}>×</span>
        ) : (
          <CheckIcon size={88} stroke="var(--accent)" />
        )}
      </div>

      <div className={styles.eyebrow}>
        {cancelled ? 'Order cancelled' : 'Order confirmed'}
      </div>
      <div className={styles.numberLead}>Your order number is</div>
      <div className={styles.number}>{displayNumber(order.orderNumber)}</div>

      {/* Live status progression */}
      {!cancelled && (
        <div className={styles.steps} aria-hidden="true">
          {STATUS_FLOW.map((s, i) => (
            <span
              key={s}
              className={`${styles.pip} ${i <= step ? styles.pipOn : ''}`}
            />
          ))}
        </div>
      )}

      <div className={`${styles.statusLabel} ${ready ? styles.statusReady : ''}`}>
        {meta.label}
      </div>

      {showWait && (
        <div className={styles.wait}>Estimated wait · {waitMins} min</div>
      )}

      <div className={styles.message}>
        <span>{meta.message}</span>
        {meta.active && (
          <span className={styles.dots}>
            <span className={styles.dot} style={{ animationDelay: '0s' }} />
            <span className={styles.dot} style={{ animationDelay: '.2s' }} />
            <span className={styles.dot} style={{ animationDelay: '.4s' }} />
          </span>
        )}
      </div>

      {pollError && <div className={styles.pollError}>{pollError}</div>}

      <button type="button" className={styles.resetButton} onClick={onReset}>
        Start new order
      </button>
    </div>
  );
}
