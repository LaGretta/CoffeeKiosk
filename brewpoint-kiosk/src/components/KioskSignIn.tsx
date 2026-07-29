import { useState, type FormEvent } from 'react';
import { login } from '../api/endpoints';
import { authStore } from '../auth/authStore';
import { ApiError, API_BASE_URL } from '../api/client';
import styles from './KioskSignIn.module.css';

interface KioskSignInProps {
  /** Optional notice, e.g. when the session expired and staff must re-auth. */
  notice?: string | null;
}

/**
 * One-time kiosk setup screen. A staff member signs the device in with the
 * Kiosk (or Staff) account; the JWT is then reused for the session. Shown
 * whenever there is no stored token, or after a 401 on order placement.
 */
export function KioskSignIn({ notice }: KioskSignInProps) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    if (busy) return;
    setError(null);
    setBusy(true);
    try {
      const auth = await login(username.trim(), password);
      if (auth.role !== 'Kiosk' && auth.role !== 'Staff') {
        setError(
          `This account is “${auth.role}”. Use a Kiosk or Staff account to place orders.`,
        );
        return;
      }
      authStore.signIn(auth);
    } catch (err) {
      if (err instanceof ApiError) {
        setError(
          err.isUnauthorized
            ? 'Incorrect username or password.'
            : err.message,
        );
      } else {
        setError('Sign-in failed. Please try again.');
      }
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className={styles.screen}>
      <div className={styles.card}>
        <div className={styles.brand}>
          <div className={styles.logo}>
            <span className={styles.logoRing} />
          </div>
          <span className={styles.wordmark}>Brewpoint</span>
        </div>

        <h1 className={styles.title}>Kiosk setup</h1>
        <p className={styles.subtitle}>
          Sign this device in once with the kiosk account to start taking orders.
        </p>

        {notice && <div className={styles.notice}>{notice}</div>}

        <form className={styles.form} onSubmit={submit}>
          <label className={styles.field}>
            <span className={styles.label}>Username</span>
            <input
              className={styles.input}
              type="text"
              autoComplete="username"
              autoCapitalize="none"
              autoCorrect="off"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              disabled={busy}
              required
            />
          </label>

          <label className={styles.field}>
            <span className={styles.label}>Password</span>
            <input
              className={styles.input}
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={busy}
              required
            />
          </label>

          {error && <div className={styles.error}>{error}</div>}

          <button
            type="submit"
            className={styles.submit}
            disabled={busy || !username.trim() || !password}
          >
            {busy ? (
              <span className={styles.spinner} aria-label="Signing in" />
            ) : (
              'Sign in kiosk'
            )}
          </button>
        </form>

        <div className={styles.apiHint}>Connected to {API_BASE_URL}</div>
      </div>
    </div>
  );
}
