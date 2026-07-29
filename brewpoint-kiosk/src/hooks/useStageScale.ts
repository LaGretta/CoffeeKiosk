import { useLayoutEffect } from 'react';

const STAGE_W = 1080;
const STAGE_H = 1920;

/**
 * Scales the fixed 1080×1920 kiosk canvas to fit the current viewport while
 * preserving aspect ratio (letterboxed). On a real portrait kiosk the scale is
 * ~1; in a desktop browser it shrinks to fit. Sets the --stage-scale CSS var.
 */
export function useStageScale(): void {
  useLayoutEffect(() => {
    const apply = () => {
      const scale = Math.min(
        window.innerWidth / STAGE_W,
        window.innerHeight / STAGE_H,
      );
      document.documentElement.style.setProperty(
        '--stage-scale',
        String(scale),
      );
    };
    apply();
    window.addEventListener('resize', apply);
    return () => window.removeEventListener('resize', apply);
  }, []);
}
