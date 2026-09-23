/**
 * Unit Tests: Payment Gateway & Currency Conversion Logic
 */

export interface PaymentRequest {
  declarationId: string;
  amountUsd: number;
  paymentMethod: 'CARD' | 'MOBILE_MONEY' | 'BANK_TRANSFER';
  currency: 'USD' | 'SOS' | 'EUR';
}

export interface PaymentResult {
  transactionId: string;
  amountUsd: number;
  convertedAmount: number;
  currency: string;
  processingFeeUsd: number;
  totalChargedUsd: number;
  status: 'SUCCESS' | 'PENDING' | 'FAILED';
}

export class PaymentService {
  private exchangeRates: Record<string, number> = {
    USD: 1.0,
    SOS: 26000.0, // 1 USD = 26,000 Somali Shilling
    EUR: 0.92,    // 1 USD = 0.92 EUR
  };

  private feePercentage: Record<string, number> = {
    CARD: 0.025,         // 2.5% fee
    MOBILE_MONEY: 0.01,  // 1.0% fee
    BANK_TRANSFER: 0.005 // 0.5% fee
  };

  public convertCurrency(amountUsd: number, targetCurrency: string): number {
    const rate = this.exchangeRates[targetCurrency];
    if (!rate) throw new Error(`Unsupported currency: ${targetCurrency}`);
    return Math.round(amountUsd * rate * 100) / 100;
  }

  public calculateProcessingFee(amountUsd: number, method: 'CARD' | 'MOBILE_MONEY' | 'BANK_TRANSFER'): number {
    const feeRate = this.feePercentage[method] ?? 0.02;
    return Math.round(amountUsd * feeRate * 100) / 100;
  }

  public processPayment(req: PaymentRequest): PaymentResult {
    if (req.amountUsd <= 0) {
      throw new Error('Payment amount must be greater than zero');
    }

    const fee = this.calculateProcessingFee(req.amountUsd, req.paymentMethod);
    const converted = this.convertCurrency(req.amountUsd, req.currency);
    const totalCharged = req.amountUsd + fee;

    return {
      transactionId: `TXN-${Math.floor(100000 + Math.random() * 900000)}`,
      amountUsd: req.amountUsd,
      convertedAmount: converted,
      currency: req.currency,
      processingFeeUsd: fee,
      totalChargedUsd: totalCharged,
      status: 'SUCCESS',
    };
  }
}

describe('PaymentService Unit Tests', () => {
  let paymentService: PaymentService;

  beforeEach(() => {
    paymentService = new PaymentService();
  });

  describe('Currency Conversion', () => {
    test('should convert USD to SOS accurately', () => {
      const converted = paymentService.convertCurrency(100, 'SOS');
      expect(converted).toBe(2600000);
    });

    test('should convert USD to EUR accurately', () => {
      const converted = paymentService.convertCurrency(100, 'EUR');
      expect(converted).toBe(92);
    });

    test('should throw error for unsupported currency', () => {
      expect(() => paymentService.convertCurrency(100, 'XYZ')).toThrow('Unsupported currency');
    });
  });

  describe('Processing Fee Calculation', () => {
    test('should calculate 2.5% fee for Credit Card payments', () => {
      const fee = paymentService.calculateProcessingFee(1000, 'CARD');
      expect(fee).toBe(25);
    });

    test('should calculate 1.0% fee for Mobile Money payments', () => {
      const fee = paymentService.calculateProcessingFee(1000, 'MOBILE_MONEY');
      expect(fee).toBe(10);
    });

    test('should calculate 0.5% fee for Bank Transfers', () => {
      const fee = paymentService.calculateProcessingFee(1000, 'BANK_TRANSFER');
      expect(fee).toBe(5);
    });
  });

  describe('Payment Processing Workflow', () => {
    test('should process payment and return successful transaction result', () => {
      const result = paymentService.processPayment({
        declarationId: 'DEC-2026-001',
        amountUsd: 500,
        paymentMethod: 'MOBILE_MONEY',
        currency: 'USD',
      });

      expect(result.status).toBe('SUCCESS');
      expect(result.transactionId).toMatch(/^TXN-\d{6}$/);
      expect(result.processingFeeUsd).toBe(5);
      expect(result.totalChargedUsd).toBe(505);
    });

    test('should reject payments with 0 or negative amount', () => {
      expect(() =>
        paymentService.processPayment({
          declarationId: 'DEC-2026-002',
          amountUsd: 0,
          paymentMethod: 'CARD',
          currency: 'USD',
        })
      ).toThrow('greater than zero');
    });
  });
});
