import { Page } from '@playwright/test';

/**
 * Global setup for E2E tests.
 * This handles common configuration and provides utility functions
 * for authentication and state management.
 */
export async function globalSetup(page: Page) {
    // Example: Ensure the app is reachable before starting tests
    try {
        await page.goto(process.env.VITE_API_GATEWAY_URL || 'http://localhost:3000');
    } catch (e) {
        console.error('Critical Error: E2E Environment is not reachable');
        throw e;
    }
}

export async function loginAsUser(page: Page, email: string, password: string) {
    await page.goto('/auth/login');
    await page.fill('input[name="email"]', email);
    await page.fill('input[name="password"]', password);
    await page.click('button[type="submit"]');
    await page.waitForURL('**/dashboard');
}
