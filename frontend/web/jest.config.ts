export default {
  preset: 'ts-jest',
  testEnvironment: 'jest-environment-jsdom',
  setupFilesAfterEnv: ['<rootDir>/src/setupTests.ts'],
  moduleNameMapper: {
    '\\.(css|less|scss|sass)$': 'identity-obj-proxy',
    '^@/(.*)$': '<rootDir>/src/$1',
  },
  transform: {
    '^.+\\.tsx?$': [
      'ts-jest',
      {
        tsconfig: {
          jsx: 'react-jsx',
        },
        // Stub out import.meta so Vite-only code compiles under Jest
        diagnostics: { ignoreCodes: ['TS151001'] },
      },
    ],
  },
  globals: {
    'import.meta': {
      env: {
        VITE_API_GATEWAY_URL: '/api/v1',
      },
    },
  },

};

