// ═══════════════════════════════════════════════════════════════════════
// Backend E2E — Shipping, Customs, ECTN, Fleet, Payment APIs
// End-to-end API tests for every microservice
// ═══════════════════════════════════════════════════════════════════════

describe('Backend API — Microservices', () => {

  const API = Cypress.env('API_URL') || 'http://localhost:5000';
  let authToken;

  before(() => {
    // Acquire auth token for all tests
    cy.request({
      method: 'POST',
      url: `${API}/api/identity/login`,
      body: {
        email: Cypress.env('ADMIN_EMAIL'),
        password: Cypress.env('ADMIN_PASS'),
      },
    }).then((res) => {
      authToken = res.body.token;
    });
  });

  /** Helper – authenticated request */
  function apiReq(method, path, body = null) {
    return cy.request({
      method,
      url: `${API}${path}`,
      body,
      headers: {
        Authorization: `Bearer ${authToken}`,
        'Content-Type': 'application/json',
        'X-Tenant-Code': Cypress.env('TENANT_CODE'),
      },
      failOnStatusCode: false,
    });
  }

  // ─────────────── SHIPPING SERVICE ───────────────
  context('Shipping Service', () => {

    let orderId;

    it('POST /api/shipping/orders — should create a shipping order', () => {
      apiReq('POST', '/api/shipping/orders', {
        customer: 'E2E Corp',
        item: 'Machine Parts',
        weight: 3000,
        portOfLoading: 'Shanghai',
        portOfDischarge: 'Berbera',
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 201]);
        expect(res.body).to.have.property('id');
        orderId = res.body.id;
      });
    });

    it('GET /api/shipping/orders — should list orders', () => {
      apiReq('GET', '/api/shipping/orders').then((res) => {
        expect(res.status).to.eq(200);
        expect(res.body).to.be.an('array');
      });
    });

    it('GET /api/shipping/orders/:id — should return the created order', () => {
      apiReq('GET', `/api/shipping/orders/${orderId}`).then((res) => {
        expect(res.status).to.eq(200);
        expect(res.body.customer).to.eq('E2E Corp');
      });
    });

    it('PATCH /api/shipping/orders/:id/status — should update order status', () => {
      apiReq('PATCH', `/api/shipping/orders/${orderId}/status`, {
        status: 'In Transit',
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 204]);
      });
    });
  });

  // ─────────────── CUSTOMS SERVICE ───────────────
  context('Customs Service', () => {

    let declarationId;

    it('POST /api/customs/declarations — should create a declaration', () => {
      apiReq('POST', '/api/customs/declarations', {
        ectnNumber: 'ECTN-API-E2E',
        hsCode: '8471.30.00',
        value: 50000,
        currency: 'USD',
        countryOfOrigin: 'CN',
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 201]);
        expect(res.body).to.have.property('id');
        declarationId = res.body.id;
      });
    });

    it('GET /api/customs/declarations — should list declarations', () => {
      apiReq('GET', '/api/customs/declarations').then((res) => {
        expect(res.status).to.eq(200);
        expect(res.body).to.be.an('array');
      });
    });

    it('POST /api/customs/declarations/:id/approve — should approve declaration', () => {
      apiReq('POST', `/api/customs/declarations/${declarationId}/approve`, {
        inspectionResult: 'Passed',
        officerNote: 'E2E approval',
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 204]);
      });
    });
  });

  // ─────────────── ECTN SERVICE ───────────────
  context('ECTN Service', () => {

    let ectnId;

    it('POST /api/ectn/applications — should submit ECTN application', () => {
      apiReq('POST', '/api/ectn/applications', {
        vesselName: 'MV API Test',
        voyageNo: 'V-E2E-001',
        portOfLoading: 'Mombasa',
        portOfDischarge: 'Berbera',
        cargoDescription: 'Textiles',
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 201]);
        expect(res.body).to.have.property('id');
        ectnId = res.body.id;
      });
    });

    it('POST /api/ectn/applications/:id/validate — should validate ECTN', () => {
      apiReq('POST', `/api/ectn/applications/${ectnId}/validate`).then((res) => {
        expect(res.status).to.be.oneOf([200, 204]);
      });
    });

    it('POST /api/ectn/applications/:id/issue — should issue ECTN', () => {
      apiReq('POST', `/api/ectn/applications/${ectnId}/issue`).then((res) => {
        expect(res.status).to.be.oneOf([200, 204]);
      });
    });
  });

  // ─────────────── FLEET SERVICE ───────────────
  context('Fleet Service', () => {

    it('POST /api/fleet/assign — should assign a vehicle', () => {
      apiReq('POST', '/api/fleet/assign', {
        orderId: 'ORD-E2E-FLEET',
        truckId: 'TRK-A102',
        driverId: 'DRV-001',
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 201, 204]);
      });
    });

    it('POST /api/fleet/dispatch — should dispatch a shipment', () => {
      apiReq('POST', '/api/fleet/dispatch', {
        orderId: 'ORD-E2E-FLEET',
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 204]);
      });
    });

    it('GET /api/fleet/tracking/:id — should return tracking data', () => {
      apiReq('GET', '/api/fleet/tracking/ORD-E2E-FLEET').then((res) => {
        expect(res.status).to.be.oneOf([200, 404]); // 404 if no real GPS yet
      });
    });
  });

  // ─────────────── PAYMENT SERVICE ───────────────
  context('Payment Service', () => {

    let invoiceId;

    it('POST /api/payment/invoices — should create an invoice', () => {
      apiReq('POST', '/api/payment/invoices', {
        orderId: 'ORD-E2E-PAY',
        amount: 3750.0,
        currency: 'USD',
        dueDate: '2026-12-31',
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 201]);
        expect(res.body).to.have.property('id');
        invoiceId = res.body.id;
      });
    });

    it('GET /api/payment/invoices/:id — should return the invoice', () => {
      apiReq('GET', `/api/payment/invoices/${invoiceId}`).then((res) => {
        expect(res.status).to.eq(200);
        expect(res.body.amount).to.eq(3750.0);
      });
    });

    it('POST /api/payment/invoices/:id/pay — should process payment', () => {
      apiReq('POST', `/api/payment/invoices/${invoiceId}/pay`, {
        method: 'credit_card',
        cardNumber: '4242424242424242',
        expiry: '12/28',
        cvv: '123',
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 204]);
      });
    });
  });

  // ─────────────── NOTIFICATION SERVICE ───────────────
  context('Notification Service', () => {

    it('GET /api/notifications — should list notifications', () => {
      apiReq('GET', '/api/notifications').then((res) => {
        expect(res.status).to.eq(200);
        expect(res.body).to.be.an('array');
      });
    });
  });
});
