import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'
import { VitePWA } from 'vite-plugin-pwa'

// In development the API is reached through this proxy, so no CORS entry is needed for localhost.
const DEV_API = process.env.VITE_DEV_API ?? 'https://api.shifts.tohoop-festival.de'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    tailwindcss(),
    VitePWA({
      registerType: 'prompt',
      injectRegister: 'auto',
      includeAssets: ['icons/*.png', 'customization/*'],
      // The manifest is served from public/manifest.webmanifest so deployments can override it.
      manifest: false,
      workbox: {
        globPatterns: ['**/*.{js,mjs,css,html,svg,png,woff2}'],
        // Runtime configuration and customization are never served from the cache.
        globIgnores: ['config.json', 'customization/**', 'manifest.webmanifest'],
        navigateFallbackDenylist: [/^\/auth\/callback/],
        maximumFileSizeToCacheInBytes: 4 * 1024 * 1024,
        cleanupOutdatedCaches: true,
        runtimeCaching: [
          {
            urlPattern: /\/(config\.json|manifest\.webmanifest|customization\/.*)$/,
            handler: 'NetworkFirst',
            options: { cacheName: 'runtime-config', networkTimeoutSeconds: 4 },
          },
        ],
      },
      devOptions: { enabled: false },
    }),
  ],
  define: {
    __APP_VERSION__: JSON.stringify(process.env.VITE_APP_VERSION ?? 'dev'),
    __BUILD_TIME__: JSON.stringify(new Date().toISOString()),
  },
  resolve: {
    alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) },
  },
  server: {
    port: 5173,
    proxy: { '/api': { target: DEV_API, changeOrigin: true, rewrite: (p) => p.replace(/^\/api/, '') } },
  },
  preview: {
    port: 5173,
    proxy: { '/api': { target: DEV_API, changeOrigin: true, rewrite: (p) => p.replace(/^\/api/, '') } },
  },
})
