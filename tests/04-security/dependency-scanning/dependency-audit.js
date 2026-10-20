#!/usr/bin/env node
// ═══════════════════════════════════════════════════════════════════════
// OWASP A06:2021 — Software Composition Analysis & Dependency Audit
// Checks:
// - NPM dependencies vulnerability audit
// - .NET NuGet packages vulnerability audit (dotnet list package --vulnerable)
// - Output report to tests/09-reports/security/dependency-audit-report.json
// ═══════════════════════════════════════════════════════════════════════

const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

const reportsDir = path.resolve(__dirname, '../../09-reports/security');
if (!fs.existsSync(reportsDir)) {
  fs.mkdirSync(reportsDir, { recursive: true });
}

console.log('═══════════════════════════════════════════════════════════════════');
console.log('   Smart Trade Logistics — Dependency Vulnerability Audit (SCA)');
console.log('═══════════════════════════════════════════════════════════════════');

const auditReport = {
  timestamp: new Date().toISOString(),
  npmScans: [],
  dotnetScans: [],
  summary: { critical: 0, high: 0, moderate: 0, low: 0 },
};

// 1. Audit Frontend NPM packages
const frontendPath = path.resolve(__dirname, '../../../frontend/web');
if (fs.existsSync(path.join(frontendPath, 'package.json'))) {
  console.log('\n[INFO] Auditing Frontend Web dependencies...');
  try {
    const rawNpm = execSync('npm audit --json', { cwd: frontendPath, stdio: 'pipe' }).toString();
    const parsed = JSON.parse(rawNpm);
    auditReport.npmScans.push({ target: 'frontend/web', vulnerabilities: parsed.vulnerabilities || {} });
  } catch (err) {
    if (err.stdout) {
      try {
        const parsed = JSON.parse(err.stdout.toString());
        auditReport.npmScans.push({ target: 'frontend/web', vulnerabilities: parsed.vulnerabilities || {} });
      } catch (_) {}
    }
  }
}

// 2. Audit .NET solution packages
const solutionRoot = path.resolve(__dirname, '../../../');
console.log('\n[INFO] Checking .NET NuGet vulnerable packages...');
try {
  const dotnetOut = execSync('dotnet list package --vulnerable', { cwd: solutionRoot, stdio: 'pipe' }).toString();
  auditReport.dotnetScans.push({ status: 'SUCCESS', details: dotnetOut });
  console.log('  [OK] .NET dependency scan executed successfully.');
} catch (err) {
  const output = (err.stdout ? err.stdout.toString() : '') + (err.stderr ? err.stderr.toString() : '');
  auditReport.dotnetScans.push({ status: 'AUDIT_OUTPUT', details: output });
}

// Save Report
const reportPath = path.join(reportsDir, 'dependency-audit-report.json');
fs.writeFileSync(reportPath, JSON.stringify(auditReport, null, 2));

console.log(`\n[SUCCESS] Dependency vulnerability report saved to: ${reportPath}`);
