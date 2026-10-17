import { Page } from '@playwright/test';
import { loginAsUser } from './setup';

describe('Shipping Lifecycle Journey', () => {
    let page: Page;

    test('should allow admin to create shipment and assign driver', async () => {
        // 1. Admin Login
        await loginAsUser(page, 'admin@smarttrade.com', 'AdminPassword123!');

        // 2. Navigate to Shipments
        await page.click('nav >> text=Shipments');
        await page.waitForURL('**/shipments');

        // 3. Create New Shipment
        await page.click('button:has-text("Create Shipment")');
        await page.fill('input[name="customerName"]', 'Global Logistics Ltd');
        await page.fill('input[name="origin"]', 'Djibouti Port');
        await page.fill('input[name="destination"]', 'Addis Ababa');
        await page.selectOption('select[name="shippingMethod"]', 'Road');
        await page.click('button[type="submit"]');

        // Verify creation
        await page.waitForSelector('.alert-success');
        const shipmentId = await page.locator('.shipment-id').first().textContent();
        expect(shipmentId).not.toBeNull();

        // 4. Assign Driver
        await page.click(`text=${shipmentId}`);
        await page.click('button:has-text("Assign Driver")');
        await page.selectOption('select[name="driverId"]', 'driver_01');
        await page.click('button:has-text("Confirm Assignment")');

        // Verify status change
        const status = await page.locator('.status-badge').textContent();
        expect(status).toContain('Assigned');
    });

    test('should allow customs to clear shipment', async () => {
        // Login as Customs Officer
        await loginAsUser(page, 'customs_officer@smarttrade.com', 'CustomsPassword123!');

        await page.click('nav >> text=Customs');
        await page.click('tr:first-child >> td:nth-child(2) >> a'); // Open first pending shipment

        await page.click('button:has-text("Upload Clearance")');
        await page.setInputFiles('input[type="file"]', 'tests/fixtures/clearance_doc.pdf');
        await page.click('button:has-text("Mark as Cleared")');

        const status = await page.locator('.status-badge').textContent();
        expect(status).toContain('Cleared');
    });
});
