import { Page } from '@playwright/test';
import { loginAsUser } from './setup';

describe('User Onboarding Journey', () => {
    let page: Page;

    beforeAll(async () => {
        // Playwright handles browser context in jest-playwright
    });

    test('should allow a new user to register and login successfully', async () => {
        // 1. Registration
        await page.goto('/auth/register');
        await page.fill('input[name="email"]', `testuser_${Date.now()}@example.com`);
        await page.fill('input[name="username"]', 'testuser');
        await page.fill('input[name="password"]', 'SecurePassword123!');
        await page.fill('input[name="confirmPassword"]', 'SecurePassword123!');
        await page.click('button[type="submit"]');

        // Verify registration success (e.g., redirect to login or success message)
        await page.waitForURL('**/auth/login');

        // 2. Login
        await loginAsUser(page, 'testuser_already_exists@example.com', 'SecurePassword123!');

        // 3. Dashboard Verification
        const dashboardHeader = await page.locator('h1').textContent();
        expect(dashboardHeader).toContain('Dashboard');

        // Verify authentication state
        const cookies = await page.context().cookies();
        const hasRefreshCookie = cookies.some(c => c.name === 'smarttrade_refresh');
        expect(hasRefreshCookie).toBe(true);
    });
});
