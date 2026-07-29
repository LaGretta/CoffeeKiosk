import { money } from '../lib/money';
import { CartIcon } from './icons';
import styles from './CartBar.module.css';

interface CartBarProps {
  count: number;
  total: number;
  onOpen: () => void;
}

export function CartBar({ count, total, onOpen }: CartBarProps) {
  return (
    <div className={styles.bar}>
      <div className={styles.left}>
        <div className={styles.iconCircle}>
          <CartIcon size={30} stroke="var(--bg-cream)" />
          {count > 0 && <span className={styles.badge}>{count}</span>}
        </div>
        <div className={styles.meta}>
          <span className={styles.countLabel}>
            {count === 1 ? '1 item' : `${count} items`}
          </span>
          <span className={styles.total}>{money(total)}</span>
        </div>
      </div>
      <button type="button" className={styles.viewButton} onClick={onOpen}>
        View order
      </button>
    </div>
  );
}
