import type { Product, Size } from '../api/types';
import { deltaLabel, money } from '../lib/money';
import { productVisual } from '../lib/productVisuals';
import styles from './SizeSheet.module.css';

interface SizeSheetProps {
  product: Product;
  sizes: Size[];
  selectedSizeId: number | null;
  onSelectSize: (sizeId: number) => void;
  onAdd: () => void;
  onClose: () => void;
}

export function SizeSheet({
  product,
  sizes,
  selectedSizeId,
  onSelectSize,
  onAdd,
  onClose,
}: SizeSheetProps) {
  const visual = productVisual(product.name, product.category);
  const selected = sizes.find((s) => s.id === selectedSizeId) ?? sizes[0];
  const price = product.basePrice + (selected?.priceModifier ?? 0);

  return (
    <>
      <div className={styles.scrim} onClick={onClose} />
      <div
        className={styles.sheet}
        role="dialog"
        aria-modal="true"
        aria-label={`Choose size for ${product.name}`}
      >
        <div className={styles.handle} />

        <div className={styles.header}>
          <div className={styles.thumb} style={{ background: visual.bg }}>
            <div className={styles.thumbDisc} style={{ background: visual.disc }} />
          </div>
          <div>
            <div className={styles.name}>{product.name}</div>
            <div className={styles.desc}>{product.description}</div>
          </div>
        </div>

        <div className={styles.label}>Choose size</div>
        <div className={styles.sizes}>
          {sizes.map((size) => {
            const on = size.id === selected?.id;
            return (
              <button
                key={size.id}
                type="button"
                className={`${styles.pill} ${on ? styles.pillOn : styles.pillOff}`}
                onClick={() => onSelectSize(size.id)}
                aria-pressed={on}
              >
                <span className={styles.pillKey}>{size.name}</span>
                <span
                  className={styles.pillDelta}
                  style={{ color: on ? 'rgba(255,255,255,.85)' : 'var(--muted)' }}
                >
                  {deltaLabel(size.priceModifier)}
                </span>
              </button>
            );
          })}
        </div>

        <button type="button" className={styles.addButton} onClick={onAdd}>
          <span>Add to order</span>
          <span className={styles.addPrice}>· {money(price)}</span>
        </button>
      </div>
    </>
  );
}
