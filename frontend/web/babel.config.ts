/**
 * Babel configuration used ONLY by Jest (via babel-jest).
 * Vite/esbuild does not read this file.
 *
 * - `babel-plugin-transform-vite-meta-env` rewrites `import.meta.env` to `process.env`
 *   so that Vite-specific syntax works under Jest (jsdom/node).
 * - Compiling to CommonJS also restores `jest.mock()` hoisting, which ts-jest's
 *   ESM mode does not support.
 *
 * Typed (`.ts`) rather than `.cjs` — supported natively by @babel/core >= 7.21.
 */
import type { TransformOptions } from '@babel/core';

const config: TransformOptions = {
  presets: [
    ['@babel/preset-env', { targets: { node: 'current' } }],
    ['@babel/preset-react', { runtime: 'automatic' }],
    '@babel/preset-typescript',
  ],
  plugins: ['babel-plugin-transform-vite-meta-env'],
};

export default config;