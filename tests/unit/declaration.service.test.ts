/**
 * Unit Tests: Customs Declaration & HS Code Logic
 */

export interface DeclarationItem {
  hsCode: string;
  description: string;
  quantity: number;
  unitPriceUsd: number;
  weightKg: number;
}

export interface TaxCalculationResult {
  totalFobUsd: number;
  importDutyUsd: number;
  vatUsd: number;
  totalTaxUsd: number;
}

export class DeclarationService {
  // Duty rates based on HS Code prefix
  private dutyRates: Record<string, number> = {
    '8703': 0.25, // Motor cars (25%)
    '8517': 0.05, // Smartphones / Telecoms (5%)
    '1006': 0.00, // Rice (Exempt 0%)
    '3004': 0.00, // Pharmaceuticals (Exempt 0%)
  };

  private standardVatRate = 0.15; // 15% VAT

  public validateHsCode(hsCode: string): boolean {
    const cleaned = hsCode.replace(/[\s.-]/g, '');
    return /^\d{6,10}$/.test(cleaned);
  }

  public calculateTaxes(items: DeclarationItem[]): TaxCalculationResult {
    let totalFobUsd = 0;
    let importDutyUsd = 0;

    for (const item of items) {
      const itemFob = item.quantity * item.unitPriceUsd;
      totalFobUsd += itemFob;

      const hsPrefix = item.hsCode.substring(0, 4);
      const dutyRate = this.dutyRates[hsPrefix] ?? 0.10; // Default 10% duty
      importDutyUsd += itemFob * dutyRate;
    }

    // VAT is calculated on (FOB + Duty)
    const vatUsd = (totalFobUsd + importDutyUsd) * this.standardVatRate;
    const totalTaxUsd = importDutyUsd + vatUsd;

    return {
      totalFobUsd: Math.round(totalFobUsd * 100) / 100,
      importDutyUsd: Math.round(importDutyUsd * 100) / 100,
      vatUsd: Math.round(vatUsd * 100) / 100,
      totalTaxUsd: Math.round(totalTaxUsd * 100) / 100,
    };
  }

  public getDeclarationStatus(
    isVerified: boolean,
    taxesPaid: boolean,
    hasInspectionFlags: boolean
  ): 'DRAFT' | 'PENDING_INSPECTION' | 'PENDING_PAYMENT' | 'CLEARED' | 'REJECTED' {
    if (hasInspectionFlags) return 'PENDING_INSPECTION';
    if (!isVerified) return 'DRAFT';
    if (!taxesPaid) return 'PENDING_PAYMENT';
    return 'CLEARED';
  }
}

describe('DeclarationService Unit Tests', () => {
  let declarationService: DeclarationService;

  beforeEach(() => {
    declarationService = new DeclarationService();
  });

  describe('HS Code Validation', () => {
    test('should validate valid HS codes', () => {
      expect(declarationService.validateHsCode('8703.23.00')).toBe(true);
      expect(declarationService.validateHsCode('8517620000')).toBe(true);
      expect(declarationService.validateHsCode('1006.30')).toBe(true);
    });

    test('should reject invalid HS codes', () => {
      expect(declarationService.validateHsCode('INVALID')).toBe(false);
      expect(declarationService.validateHsCode('123')).toBe(false);
    });
  });

  describe('Tax & Duty Calculations', () => {
    test('should calculate duty and VAT for motor vehicles (8703)', () => {
      const items: DeclarationItem[] = [
        {
          hsCode: '87032300',
          description: 'Toyota Land Cruiser',
          quantity: 2,
          unitPriceUsd: 30000,
          weightKg: 4500,
        },
      ];

      const result = declarationService.calculateTaxes(items);
      expect(result.totalFobUsd).toBe(60000);
      expect(result.importDutyUsd).toBe(15000); // 25% of 60,000
      expect(result.vatUsd).toBe(11250); // 15% of (60,000 + 15,000)
      expect(result.totalTaxUsd).toBe(26250);
    });

    test('should handle duty-exempt agricultural items (Rice - 1006)', () => {
      const items: DeclarationItem[] = [
        {
          hsCode: '10063000',
          description: 'Basmati Rice 50kg Bags',
          quantity: 100,
          unitPriceUsd: 50,
          weightKg: 5000,
        },
      ];

      const result = declarationService.calculateTaxes(items);
      expect(result.totalFobUsd).toBe(5000);
      expect(result.importDutyUsd).toBe(0); // 0% duty
      expect(result.vatUsd).toBe(750); // 15% of 5,000
      expect(result.totalTaxUsd).toBe(750);
    });
  });

  describe('Declaration Status Workflow', () => {
    test('should require inspection if flagged', () => {
      const status = declarationService.getDeclarationStatus(true, true, true);
      expect(status).toBe('PENDING_INSPECTION');
    });

    test('should require payment if verified but unpaid', () => {
      const status = declarationService.getDeclarationStatus(true, false, false);
      expect(status).toBe('PENDING_PAYMENT');
    });

    test('should clear declaration when verified and paid', () => {
      const status = declarationService.getDeclarationStatus(true, true, false);
      expect(status).toBe('CLEARED');
    });
  });
});
