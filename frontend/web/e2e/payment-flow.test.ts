import { Page } from '@playwright/test';
import { loginAsUser } from './setup';

describe('Payment and Billing Journey', () => {
    let page: Page;

    test('should generate invoice and process payment successfully', async () => {
        // 1. Admin Login to trigger billing
        await loginAsUser(page, 'admin@smarttrade.com', 'AdminPassword123!');

        // 2. Navigate to Billing/Payments
        await page.click('nav >> text=Payments');
        await page.waitForURL('**/payments');

        // 3. Generate Invoice for a delivered shipment
        // We assume there's a shipment already marked as 'Delivered'
        await page.click('tr:first-child >> button:has-text("Generate Invoice")');

        // Verify invoice creation
        await page.waitForSelector('.invoice-detail');
        const invoiceNumber = await page.locator('.invoice-number').textContent();
        expect(invoiceNumber).not.toBeNull();

        // 4. Simulate Customer Payment
        // In a real E2E, we might switch users or use a customer portal
        // For this test, we use the customer's perspective
        await page.click('button:has-text("Pay Now")');

        // Mocking a payment gateway interaction (e.g., entering card details)
        await page.fill('input[name="cardNumber"]', '4242424242424242');
        await page.fill('input[name="expiry"]', '12/26');
        await page.fill('input[name="cvv"]', '123');
        await page.click('button:has-text("Confirm Payment")');

        // 5. Verification
        await page.waitForSelector('.payment-success-message');
        const paymentStatus = await page.locator('.status-badge').textContent();
        expect(paymentStatus).toContain('Paid');

        // Verify that the invoice status updated to 'Paid'
        const invoiceStatus = await page.locator('.invoice-status').textContent();
        expect(invoiceStatus).toBe('Paid');
    });
});
