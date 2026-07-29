# Brewpoint Kiosk — frontend

A customer-facing **self-order kiosk** for the Brewpoint coffee shop. It is a
standalone **React + TypeScript + Vite** app that consumes the existing
**CoffeeKiosk .NET API** over HTTP. Portrait-first (designed for a 1080 × 1920
in-store tablet), touch-friendly, warm neutrals + one terracotta accent.

The customer browses the menu by category, taps an item to pick a size, builds
an order in a cart, checks out, and then watches the **live order status**
(Placed → Paid → Preparing → Ready → Completed) on the confirmation screen.

> This folder is **frontend only**. The backend was left untouched except for a
> single minimal dev-CORS addition (see [CORS](#the-one-backend-change-cors)).
> **Nothing was committed** — everything is left in the working tree for review.

---

## Requirements

- Node.js 18+ (developed on Node 24)
- The CoffeeKiosk API running locally (see below)

## Run it

```bash
npm install
npm run dev
```

The dev server starts on **http://localhost:5173**.

Other scripts:

```bash
npm run build      # type-check (tsc -b) + production build to dist/
npm run preview    # preview the production build
npm run typecheck  # type-check only
```

### API base URL

The app reads the API base URL from **`VITE_API_URL`**. A `.env` with a sensible
dev default is included; copy `.env.example` if you need to change it:

```
VITE_API_URL=http://localhost:5011
```

### Running the API for local dev

Start the API with the **`http`** launch profile so it listens on plain
`http://localhost:5011` (no HTTPS redirect to fight in the browser):

```bash
dotnet run --project CoffeeKiosk.API --launch-profile http
```

> If you run the `https` profile instead, `UseHttpsRedirection` will 307-redirect
> the kiosk's `http://localhost:5011` calls to `https://localhost:7179`, whose dev
> certificate the browser will reject. Use the `http` profile for the kiosk, or
> point `VITE_API_URL` at the HTTPS origin and trust the dev cert.

---

## How the kiosk signs in

A real kiosk is signed in **once** by a staff member and then stays signed in.

- On first launch (no stored token) the app shows a **Kiosk setup** screen.
- Staff enter the **Kiosk** (or **Staff**) account's username + password. The app
  calls `POST /api/auth/login`, and attaches the returned JWT as
  `Authorization: Bearer <token>` on `POST /api/orders`.
- Menu browsing (`GET /api/menu`) and status checks (`GET /api/orders/{number}`)
  are public and need no token.
- If the token is missing or rejected (401/403) during checkout, the app returns
  to the Kiosk setup screen with a notice.
- Tap the header ("Tap to order" / "Table service") to open **Kiosk settings**
  and sign the device out.

Credentials are **never hardcoded**.

### Token storage choice

The JWT is kept **in memory and mirrored to `sessionStorage`** (key
`brewpoint.kiosk.session`) — not `localStorage`. Rationale: the token is a
short-lived session secret, so it should not persist as a long-term credential on
the device. `sessionStorage` survives a page refresh during a shift but is
cleared when the tab/session ends. For a production kiosk you would typically use
a short-lived token refreshed from a device-bound secret; that's out of scope for
this dev build.

---

## Architecture

```
src/
  api/
    types.ts        TS interfaces mirroring the real API DTOs
    client.ts       fetch wrapper: base URL, bearer, ApiError, network handling
    endpoints.ts    login / getMenu / createOrder / getOrder
  auth/
    authStore.ts    sessionStorage-backed kiosk session + subscribe()
    useAuth.ts       React binding (useSyncExternalStore)
  hooks/
    useMenu.ts        GET /api/menu with loading/error/ready + retry
    useOrderStatus.ts polls GET /api/orders/{number} until a terminal status
    useStageScale.ts  scales the 1080×1920 canvas to fit any viewport
  lib/
    money.ts          "<amount> ₴" formatting
    cart.ts           cart model + pure add/qty/total/toCreateOrder helpers
    productVisuals.ts warm two-tone placeholder art per product
    status.ts         status → label / step / terminal
  components/         TopBar, CategoryTabs, ProductCard, MenuGrid, CartBar,
                     SizeSheet, CartSheet, Confirmation, KioskSignIn,
                     SettingsSheet, StatusScreens, icons
  Kiosk.tsx           the ordering state machine
  App.tsx             session gate (sign-in vs kiosk) + scaled canvas
```

Loading, empty, and error states (API down, empty menu, 401, failed checkout)
are all handled — no blank screens.

---

## API contract — important notes (matches the REAL API)

The API differs from the original kiosk brief in two places; the frontend follows
the **real** API, verified against the .NET DTOs and `Program.cs`:

- **`GET /api/menu` returns `{ menu, size }`**, not `{ products, sizes }`.
- **Enums serialize as strings** (`JsonStringEnumConverter` is registered): e.g.
  category `"Coffee" | "Tea" | "Dessert"`, status `"Placed" | "Paid" | …`.

Endpoints consumed:

| Endpoint | Auth | Used for |
| --- | --- | --- |
| `GET /api/menu` | public | products + sizes |
| `POST /api/auth/login` | public | kiosk sign-in |
| `POST /api/orders` | Kiosk/Staff | checkout |
| `GET /api/orders/{number}` | public | live status polling |

### Product imagery

The API has no product images. The design uses warm two-tone "disc" placeholders,
which are reproduced here (`lib/productVisuals.ts`) — exact palette for the known
menu items, deterministic on-brand fallback for anything else. Swap for real
photography in production.

---

## The one backend change: CORS

The API had **no CORS** configured, so a browser on `localhost:5173` could not
call it. The **only** backend edit made was a minimal **development** CORS policy
in `CoffeeKiosk.API/Program.cs`:

```csharp
// service registration
const string KioskCorsPolicy = "BrewpointKioskDev";
builder.Services.AddCors(options =>
    options.AddPolicy(KioskCorsPolicy, policy => policy
        .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()));

// pipeline (after UseHttpsRedirection, before UseAuthentication)
app.UseCors(KioskCorsPolicy);
```

No other backend code was touched.

---

## Seeding data for a local run

The API's `DbSeeder` only creates an **admin** account (`admin` / `admin123`,
role Admin). It does **not** seed a Kiosk/Staff user, products, or sizes — so a
fresh database has an empty menu and no account the kiosk can sign in with
(`POST /api/orders` requires role **Kiosk or Staff**; Admin is rejected).

You can seed everything through the existing **admin** endpoints (data only, no
code changes). With the API running:

```bash
# 1) get an admin token
TOKEN=$(curl -s -X POST http://localhost:5011/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}' \
  | sed -E 's/.*"token":"([^"]+)".*/\1/')

# 2) a Kiosk account for the kiosk to sign in with
#    (note: this create-user endpoint is defined as GET with a JSON body)
curl -s -X GET http://localhost:5011/api/auth/users \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"username":"kiosk","email":"kiosk@coffeekiosk.local","password":"kiosk123","role":"Kiosk"}'

# 3) sizes
for s in '{"name":"S","priceModifier":0}' '{"name":"M","priceModifier":10}' '{"name":"L","priceModifier":20}'; do
  curl -s -X POST http://localhost:5011/api/sizes \
    -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d "$s"; done

# 4) a couple of products
curl -s -X POST http://localhost:5011/api/products \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"name":"Latte","description":"Espresso with silky steamed milk","basePrice":55,"category":"Coffee","isAvailable":true}'
```

Then sign the kiosk in with `kiosk` / `kiosk123`.

---

## ⚠️ Known backend blocker (checkout) — NOT fixed here

Placing an order currently **fails** with `404 "Order not found"`. This is a bug
in the API, not the frontend. In
`CoffeeKiosk.Application/Service/OrderService.cs`, `CreateAsync` treats the
authenticated user's id as an existing order id and requires that "order" to
already be **Paid** before it will create anything:

```csharp
public async Task<OrderResponseDto> CreateAsync(int kioskUserId, CreateOrderDto dto, CancellationToken ct)
{
    var findorder = await _iorderrepo.GetByIdAsync(kioskUserId, ct); // looks up an order by USER id
    if (findorder == null)
        throw new KeyNotFoundException("Order not found");            // → always 404 for a new order
    if (findorder.Status != OrderStatus.Paid)
        throw new UnauthorizedAccessException("Invalid order status");
    var order = _mapper.Map<Order>(dto);                              // items/price/number logic missing
    ...
}
```

As written, checkout can never succeed, and the `items` payload is never turned
into order lines. Per the project rules the backend was **left untouched** —
this needs a fix on the API side (rebuild `CreateAsync` to actually create an
order from the cart items, compute the total, generate an order number, and
return it). Once that's fixed, the frontend checkout + live status flow will work
end-to-end with no frontend changes. The frontend already handles the current
404 gracefully (it shows the API's message in the cart rather than crashing).

---

## What was verified

- `npm run build` — clean type-check + production build.
- Against the running API: menu loads over CORS, prices/format correct, category
  tabs, size sheet, cart, quantity steppers, and the checkout request (correct
  `Authorization: Bearer` + `{ items:[{ productId, sizeId, quantity }] }` body).
- Checkout is blocked only by the backend `CreateAsync` bug documented above.
