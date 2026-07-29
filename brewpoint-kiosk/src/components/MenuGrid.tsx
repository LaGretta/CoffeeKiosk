import type { Product } from '../api/types';
import { ProductCard } from './ProductCard';
import styles from './MenuGrid.module.css';

interface MenuGridProps {
  products: Product[];
  onAdd: (product: Product) => void;
}

export function MenuGrid({ products, onAdd }: MenuGridProps) {
  return (
    <div className={`${styles.scroll} bp-scroll`}>
      {products.length === 0 ? (
        <div className={styles.empty}>Nothing in this category yet.</div>
      ) : (
        <div className={styles.grid}>
          {products.map((product) => (
            <ProductCard key={product.id} product={product} onAdd={onAdd} />
          ))}
        </div>
      )}
    </div>
  );
}
