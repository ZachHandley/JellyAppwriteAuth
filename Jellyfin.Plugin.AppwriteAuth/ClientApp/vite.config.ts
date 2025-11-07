import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import cssInjectedByJsPlugin from 'vite-plugin-css-injected-by-js';
import tailwindcss from '@tailwindcss/vite';
import { resolve } from 'path';

export default defineConfig({
  plugins: [
    vue(),
    tailwindcss(),
    cssInjectedByJsPlugin(),
  ],

  resolve: {
    alias: {
      '@': resolve(__dirname, 'src'),
    },
  },

  build: {
    lib: {
      entry: resolve(__dirname, 'src/main.ts'),
      name: 'AppwriteAuthUI',
      fileName: () => 'appwrite-auth-bundle.js',
      formats: ['umd'],
    },
    outDir: '../wwwroot',
    emptyOutDir: true,
    cssCodeSplit: false,
    rollupOptions: {
      external: [], // Bundle everything (including Vue)
      output: {
        inlineDynamicImports: true,
        manualChunks: undefined,
      },
    },
    minify: 'esbuild',
    sourcemap: false,
  },

  server: {
    port: 5173,
    proxy: {
      '/Plugin/AppwriteAuth': {
        target: 'http://localhost:8096',
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
