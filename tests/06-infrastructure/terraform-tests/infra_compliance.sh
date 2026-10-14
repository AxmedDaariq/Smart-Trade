#!/bin/bash
# Infrastructure Compliance Script
# Checks if the deployed infra meets enterprise standards

echo "Starting Infrastructure Compliance Check..."

# 1. Check if Load Balancer is using HTTPS
if curl -k -sI https://api.smarttrade.com | grep -q "HTTP/1.1 200 OK"; then
    echo "[PASS] SSL/TLS is active on the Load Balancer."
else
    echo "[FAIL] SSL/TLS is missing or misconfigured."
fi

# 2. Check for Open Ports (Basic Security Scan)
# We should NOT see port 5432 (Postgres) or 22 (SSH) open to the public
OPEN_PORTS=$(nmap -p 22,5432 api.smarttrade.com)
if echo "$OPEN_PORTS" | grep -q "open"; then
    echo "[FAIL] Security Breach: Database or SSH ports are exposed to the internet!"
else
    echo "[PASS] Infrastructure ports are properly firewalled."
fi

# 3. Check Cluster Health
K8S_NODES=$(kubectl get nodes | grep -c "Ready")
if [ "$K8S_NODES" -gt 2 ]; then
    echo "[PASS] High Availability (HA) is active. $K8S_NODES nodes ready."
else
    echo "[FAIL] Low Availability: Less than 3 nodes ready. Risk of downtime."
fi

echo "Compliance Check Finished."
