/** @type {import('jest').Config} */
const config = {
  testEnvironment: 'jest-environment-jsdom',
  setupFilesAfterEnv: ['<rootDir>/src/setupTests.ts'],

  moduleNameMapper: {
    // CSS modules
    '\\.(css|less|scss|sass)$': 'identity-obj-proxy',
    // Path alias
    '^@/(.*)$': '<rootDir>/src/$1',
  },

  // babel-jest (CommonJS) + babel-plugin-transform-vite-meta-env handles both
  // JSX/TS and Vite's `import.meta.env`, and restores jest.mock() hoisting.
  transform: {
    '^.+\\.(t|j)sx?$': 'babel-jest',
  },
};

export default config;
