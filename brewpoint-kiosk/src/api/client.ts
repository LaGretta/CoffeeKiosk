/**
 * Thin typed fetch wrapper around the CoffeeKiosk API.
 * Base URL comes from VITE_API_URL (see .env / README).
 */

const RAW_BASE = import.meta.env.VITE_API_URL ?? 'http://localhost:5011';
export const API_BASE_URL = RAW_BASE.replace(/\/+$/, '');

/** Error thrown for any non-2xx response or network failure. */
export class ApiError extends Error {
  readonly status: number;
  /** True when the failure was a transport/network error (API unreachable). */
  readonly isNetwork: boolean;

  constructor(message: string, status: number, isNetwork = false) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.isNetwork = isNetwork;
  }

  get isUnauthorized(): boolean {
    return this.status === 401 || this.status === 403;
  }
}

export interface RequestOptions {
  method?: string;
  body?: unknown;
  /** Bearer token for authenticated endpoints. */
  token?: string | null;
  signal?: AbortSignal;
}

/** Try to pull a human-readable message out of an error/problem-details body. */
function extractMessage(payload: unknown, fallback: string): string {
  if (payload && typeof payload === 'object') {
    const obj = payload as Record<string, unknown>;
    for (const key of ['detail', 'title', 'message', 'error']) {
      const value = obj[key];
      if (typeof value === 'string' && value.trim()) return value;
    }
  }
  if (typeof payload === 'string' && payload.trim()) return payload;
  return fallback;
}

export async function request<T>(
  path: string,
  { method = 'GET', body, token, signal }: RequestOptions = {},
): Promise<T> {
  const headers: Record<string, string> = { Accept: 'application/json' };
  if (body !== undefined) headers['Content-Type'] = 'application/json';
  if (token) headers.Authorization = `Bearer ${token}`;

  let response: Response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
      signal,
    });
  } catch (err) {
    if (err instanceof DOMException && err.name === 'AbortError') throw err;
    throw new ApiError(
      'Cannot reach the server. Is the CoffeeKiosk API running?',
      0,
      true,
    );
  }

  // Parse the body once; tolerate empty and non-JSON responses.
  const text = await response.text();
  let payload: unknown = null;
  if (text) {
    try {
      payload = JSON.parse(text);
    } catch {
      payload = text;
    }
  }

  if (!response.ok) {
    throw new ApiError(
      extractMessage(payload, `Request failed (${response.status})`),
      response.status,
    );
  }

  return payload as T;
}
