#!/bin/bash
# ═══════════════════════════════════════════════════════════════════════
# Smart Trade Logistics — OWASP ZAP Scanning Script (Bash / CI/CD)
# ═══════════════════════════════════════════════════════════════════════

TARGET_URL="${1:-http://localhost:5000}"
MODE="${2:-baseline}"

echo "================================================================"
echo " Starting OWASP ZAP Security Audit for Smart Trade Logistics"
echo " Target URL: $TARGET_URL"
echo " Mode:       $MODE"
echo "================================================================"

REPORTS_DIR="$(dirname "$0")/../../09-reports/security"
mkdir -p "$REPORTS_DIR"

node "$(dirname "$0")/zap-ci-runner.js" "--mode=$MODE" "--target=$TARGET_URL"
