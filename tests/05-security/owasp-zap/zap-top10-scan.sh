#!/bin/bash
# Professional OWASP ZAP Top 10 Scanning Script
# This script automates the detection of the top 10 most critical web vulnerabilities

TARGET_URL="http://localhost:5000"
REPORT_DIR="tests/05-security/owasp-zap/reports"
mkdir -p $REPORT_DIR

echo "----------------------------------------------------------------"
echo "Starting OWASP Top 10 Enterprise Security Scan for Smart Trade"
echo "Target: $TARGET_URL"
echo "----------------------------------------------------------------"

# 1. Spidering (Mapping the Attack Surface)
echo "[1/3] Spidering application to find all endpoints..."
docker run --rm -v $(pwd):/zap/wrk/:rw owasp/zap2docker-stable zap-baseline.py \
    -t $TARGET_URL -g gen.conf -r spider_report.html -x options.conf

# 2. Active Scanning (Attacking the endpoints)
echo "[2/3] Launching Active Scan (Testing Top 10 Vulnerabilities)..."
# This will test for:
# - A01: Broken Access Control
# - A02: Cryptographic Failures
# - A03: Injection (SQLi, XSS)
# - A04: Insecure Design
# - A05: Security Misconfiguration
# - A06: Vulnerable and Outdated Components
# - A07: Identification and Authentication Failures
# - A08: Software and Data Integrity Failures
# - A09: Security Logging and Monitoring Failures
# - A10: Server-Side Request Forgery (SSRF)

docker run --rm -v $(pwd):/zap/wrk/:rw owasp/zap2docker-stable zap-full-scan.py \
    -t $TARGET_URL -r full_security_report.html

# 3. Report Generation
echo "[3/3] Consolidating results..."
mv full_security_report.html $REPORT_DIR/full_security_report.html
mv spider_report.html $REPORT_DIR/spider_report.html

echo "----------------------------------------------------------------"
echo "Scan Complete! Please review the reports in: $REPORT_DIR"
echo "----------------------------------------------------------------"
