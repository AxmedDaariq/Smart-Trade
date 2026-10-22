module.exports = {
  testEnvironment: 'jest-playwright-electron',
  testMatch: ['**/e2e/**/*.test.ts'],
  setupFilesAfterEnv: ['<rootDir>/e2e/setup.ts'],
  verbose: true,
};
