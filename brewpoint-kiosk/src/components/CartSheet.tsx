import type { CartLine } from '../lib/cart';
import { money } from '../lib/money';
import { CartIcon } from './icons';
import styles from './CartSheet.module.css';

interface CartSheetProps {
  cart: CartLine[];
  total: number;
  submitting: boolean;
  error: string | null;
  onInc: (lineId: string) => void;
  onDec: (lineId: string) => void;
  onPay: () => void;
  onClose: () => void;
}

export function CartSheet({
  cart,
  total,
  submitting,
  error,
  onInc,
  onDec,
  onPay,
  onClose,
}: CartSheetProps) {
  const hasItems = cart.length > 0;

  return (
    <>
      <div className={styles.scrim} onClick={submitting ? undefined : onClose} />
      <div
        className={styles.sheet}
        role="dialog"
        aria-modal="true"
        aria-label="Your order"
      >
        <div className={styles.header}>
          <div className={styles.title}>Your order</div>
          <button
            type="button"
            className={styles.close}
            onClick={onClose}
            disabled={submitting}
            aria-label="Close"
          >
            ×
          </button>
        </div>

        <div className={`${styles.list} bp-scroll`}>
          {!hasItems ? (
            <div className={styles.empty}>
              <div className={styles.emptyCircle}>
                <CartIcon size={52} stroke="var(--muted-3)" />
              </div>
              <div className={styles.emptyTitle}>Your cart is empty</div>
              <div className={styles.emptySub}>
                Add a drink or treat to get started.
              </div>
              <button
                type="button"
                className={styles.browseButton}
                onClick={onClose}
              >
                Browse menu
              </button>
            </div>
          ) : (
            cart.map((line) => (
              <div key={line.lineId} className={styles.line}>
                <div
                  className={styles.thumb}
                  style={{ background: line.visual.bg }}
                >
                  <div
                    className={styles.thumbDisc}
                    style={{ background: line.visual.disc }}
                  />
                </div>
                <div className={styles.lineMeta}>
                  <div className={styles.lineName}>{line.name}</div>
                  <div className={styles.lineSub}>
                    Size {line.sizeName} · {money(line.unit)}
                  </div>
                </div>
                <div className={styles.stepper}>
                  <button
                    type="button"
                    className={styles.stepBtn}
                    onClick={() => onDec(line.lineId)}
                    disabled={submitting}
                    aria-label={`Decrease ${line.name}`}
                  >
                    −
                  </button>
                  <span className={styles.qty}>{line.qty}</span>
                  <button
                    type="button"
                    className={styles.stepBtn}
                    onClick={() => onInc(line.lineId)}
                    disabled={submitting}
                    aria-label={`Increase ${line.name}`}
                  >
                    +
                  </button>
                </div>
                <div className={styles.lineTotal}>
                  {money(line.unit * line.qty)}
                </div>
              </div>
            ))
          )}
        </div>

        {hasItems && (
          <div className={styles.footer}>
            {error && <div className={styles.error}>{error}</div>}
            <div className={styles.totalRow}>
              <span className={styles.totalLabel}>Total</span>
              <span className={styles.totalValue}>{money(total)}</span>
            </div>
            <button
              type="button"
              className={styles.payButton}
              onClick={onPay}
              disabled={submitting}
            >
              {submitting ? (
                <span className={styles.spinner} aria-label="Placing order" />
              ) : (
                `Pay ${money(total)}`
              )}
            </button>
          </div>
        )}
      </div>
    </>
  );
}
