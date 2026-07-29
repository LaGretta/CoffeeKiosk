import type { Product } from '../api/types';
import { money } from '../lib/money';
import { productVisual } from '../lib/productVisuals';
import styles from './ProductCard.module.css';

interface ProductCardProps {
  product: Product;
  onAdd: (product: Product) => void;
}

export function ProductCard({ product, onAdd }: ProductCardProps) {
  const soldOut = !product.isAvailable;
  const visual = productVisual(product.name, product.category);

  return (
    <div className={`${styles.card} ${soldOut ? styles.soldOut : ''}`}>
      <div className={styles.image} style={{ background: visual.bg }}>
        <div className={styles.disc} style={{ background: visual.disc }}>
          <div
            className={styles.discInner}
            style={{ background: visual.discInner }}
          />
        </div>
        {soldOut && <div className={styles.soldBadge}>Sold out</div>}
      </div>

      <div className={styles.body}>
        <div className={styles.name}>{product.name}</div>
        <div className={styles.desc}>{product.description}</div>
        <div className={styles.footer}>
          <span className={styles.price}>{money(product.basePrice)}</span>
          {soldOut ? (
            <button
              type="button"
              className={styles.addDisabled}
              disabled
              aria-disabled="true"
            >
              Sold out
            </button>
          ) : (
            <button
              type="button"
              className={styles.add}
              onClick={() => onAdd(product)}
            >
              + Add
            </button>
          )}
        </div>
      </div>
    </div>
  );
}
