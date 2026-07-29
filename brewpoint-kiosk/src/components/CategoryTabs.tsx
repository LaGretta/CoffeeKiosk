import type { Category } from '../api/types';
import styles from './CategoryTabs.module.css';

interface CategoryTabsProps {
  categories: Category[];
  counts: Record<Category, number>;
  active: Category;
  onSelect: (category: Category) => void;
}

export function CategoryTabs({
  categories,
  counts,
  active,
  onSelect,
}: CategoryTabsProps) {
  return (
    <nav className={styles.tabs}>
      {categories.map((category) => {
        const on = category === active;
        return (
          <button
            key={category}
            type="button"
            className={`${styles.tab} ${on ? styles.tabOn : styles.tabOff}`}
            onClick={() => onSelect(category)}
            aria-pressed={on}
          >
            <span className={styles.name}>{category}</span>
            <span className={`${styles.count} ${on ? styles.countOn : styles.countOff}`}>
              {counts[category] ?? 0}
            </span>
          </button>
        );
      })}
    </nav>
  );
}
