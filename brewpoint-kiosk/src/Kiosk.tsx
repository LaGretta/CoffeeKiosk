import { useMemo, useState } from 'react';
import type { Category, Order, Product, Size } from './api/types';
import { createOrder } from './api/endpoints';
import { ApiError } from './api/client';
import { authStore, type KioskSession } from './auth/authStore';
import { useMenu } from './hooks/useMenu';
import { useOrderStatus } from './hooks/useOrderStatus';
import {
  addLine,
  cartCount,
  cartTotal,
  changeQty,
  toCreateOrder,
  type CartLine,
} from './lib/cart';
import { TopBar } from './components/TopBar';
import { CategoryTabs } from './components/CategoryTabs';
import { MenuGrid } from './components/MenuGrid';
import { CartBar } from './components/CartBar';
import { SizeSheet } from './components/SizeSheet';
import { CartSheet } from './components/CartSheet';
import { Confirmation } from './components/Confirmation';
import { SettingsSheet } from './components/SettingsSheet';
import { LoadingScreen, ErrorScreen } from './components/StatusScreens';

const CATEGORY_ORDER: Category[] = ['Coffee', 'Tea', 'Dessert'];

/** Pick the default size for the size sheet: prefer "M", else the middle one. */
function defaultSizeId(sizes: Size[]): number | null {
  if (sizes.length === 0) return null;
  const medium = sizes.find((s) => s.name.trim().toLowerCase() === 'm');
  if (medium) return medium.id;
  return sizes[Math.floor((sizes.length - 1) / 2)].id;
}

interface KioskProps {
  session: KioskSession;
  /** Called when the session is rejected mid-order (401/403). */
  onSessionExpired: (message: string) => void;
}

export function Kiosk({ session, onSessionExpired }: KioskProps) {
  const { state, reload } = useMenu();

  const [category, setCategory] = useState<Category | null>(null);
  const [cart, setCart] = useState<CartLine[]>([]);
  const [sheetProduct, setSheetProduct] = useState<Product | null>(null);
  const [sheetSizeId, setSheetSizeId] = useState<number | null>(null);
  const [cartOpen, setCartOpen] = useState(false);
  const [settingsOpen, setSettingsOpen] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [checkoutError, setCheckoutError] = useState<string | null>(null);
  const [placedOrder, setPlacedOrder] = useState<Order | null>(null);
  const [waitMins, setWaitMins] = useState(6);

  const menu = state.status === 'ready' ? state.data : null;

  // Categories that actually have products, in the canonical order.
  const categories = useMemo<Category[]>(() => {
    if (!menu) return [];
    return CATEGORY_ORDER.filter((c) =>
      menu.menu.some((p) => p.category === c),
    );
  }, [menu]);

  const counts = useMemo(() => {
    const result = { Coffee: 0, Tea: 0, Dessert: 0 } as Record<Category, number>;
    menu?.menu.forEach((p) => {
      result[p.category] = (result[p.category] ?? 0) + 1;
    });
    return result;
  }, [menu]);

  const activeCategory: Category | null =
    category && categories.includes(category) ? category : categories[0] ?? null;

  const visibleProducts = useMemo(
    () => menu?.menu.filter((p) => p.category === activeCategory) ?? [],
    [menu, activeCategory],
  );

  const count = cartCount(cart);
  const total = cartTotal(cart);

  // Live status polling once an order has been placed.
  const { order: liveOrder, error: pollError } = useOrderStatus(
    placedOrder?.orderNumber ?? null,
    placedOrder,
  );

  // ---- Handlers ----
  const openProduct = (product: Product) => {
    if (!menu) return;
    setSheetProduct(product);
    setSheetSizeId(defaultSizeId(menu.size));
  };

  const addToCart = () => {
    if (!sheetProduct || !menu) return;
    const size =
      menu.size.find((s) => s.id === sheetSizeId) ?? menu.size[0];
    if (!size) return;
    setCart((prev) => addLine(prev, sheetProduct, size));
    setSheetProduct(null);
  };

  const adjust = (lineId: string, delta: number) =>
    setCart((prev) => changeQty(prev, lineId, delta));

  const pay = async () => {
    if (submitting || cart.length === 0) return;
    const token = authStore.getToken();
    if (!token) {
      onSessionExpired('Please sign the kiosk in to place orders.');
      return;
    }
    setSubmitting(true);
    setCheckoutError(null);
    try {
      const order = await createOrder(toCreateOrder(cart), token);
      setWaitMins(Math.min(4 + count, 12));
      setPlacedOrder(order);
      setCart([]);
      setCartOpen(false);
    } catch (err) {
      if (err instanceof ApiError && err.isUnauthorized) {
        onSessionExpired('The kiosk session expired. Please sign in again.');
        return;
      }
      setCheckoutError(
        err instanceof ApiError
          ? err.message
          : 'Could not place the order. Please try again.',
      );
    } finally {
      setSubmitting(false);
    }
  };

  const startNewOrder = () => {
    setPlacedOrder(null);
    setCheckoutError(null);
  };

  const signOut = () => {
    setSettingsOpen(false);
    authStore.signOut();
  };

  // ---- Render ----
  if (state.status === 'loading') return <LoadingScreen />;

  if (state.status === 'error') {
    return (
      <ErrorScreen
        title={state.error.isNetwork ? 'Can’t reach the kitchen' : 'Menu unavailable'}
        detail={state.error.message}
        onRetry={reload}
      />
    );
  }

  if (categories.length === 0) {
    return (
      <ErrorScreen
        title="Menu is empty"
        detail="No products are available right now. Please check back soon."
        onRetry={reload}
      />
    );
  }

  // Confirmation takes over the whole canvas once an order is placed.
  if (placedOrder) {
    return (
      <Confirmation
        order={liveOrder ?? placedOrder}
        waitMins={waitMins}
        pollError={pollError}
        onReset={startNewOrder}
      />
    );
  }

  return (
    <>
      <TopBar onOpenSettings={() => setSettingsOpen(true)} />
      <CategoryTabs
        categories={categories}
        counts={counts}
        active={activeCategory!}
        onSelect={setCategory}
      />
      <MenuGrid products={visibleProducts} onAdd={openProduct} />
      <CartBar count={count} total={total} onOpen={() => setCartOpen(true)} />

      {sheetProduct && menu && (
        <SizeSheet
          product={sheetProduct}
          sizes={menu.size}
          selectedSizeId={sheetSizeId}
          onSelectSize={setSheetSizeId}
          onAdd={addToCart}
          onClose={() => setSheetProduct(null)}
        />
      )}

      {cartOpen && (
        <CartSheet
          cart={cart}
          total={total}
          submitting={submitting}
          error={checkoutError}
          onInc={(id) => adjust(id, 1)}
          onDec={(id) => adjust(id, -1)}
          onPay={pay}
          onClose={() => setCartOpen(false)}
        />
      )}

      {settingsOpen && (
        <SettingsSheet
          session={session}
          onSignOut={signOut}
          onClose={() => setSettingsOpen(false)}
        />
      )}
    </>
  );
}
