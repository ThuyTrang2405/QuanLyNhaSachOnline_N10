import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  // Toàn bộ app render phía client — tránh SSR hydration conflicts
  { path: '**', renderMode: RenderMode.Client },
];
