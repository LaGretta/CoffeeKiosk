import { useState } from 'react';
import { useAuth } from './auth/useAuth';
import { useStageScale } from './hooks/useStageScale';
import { Kiosk } from './Kiosk';
import { KioskSignIn } from './components/KioskSignIn';

export default function App() {
  useStageScale();
  const session = useAuth();
  const [notice, setNotice] = useState<string | null>(null);

  return (
    <div className="stage-viewport">
      <div className="stage">
        {session ? (
          <Kiosk session={session} onSessionExpired={setNotice} />
        ) : (
          <KioskSignIn notice={notice} />
        )}
      </div>
    </div>
  );
}
