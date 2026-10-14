import requests
import time

SERVICES = {
    "Identity": "http://api.smarttrade.com/health/identity",
    "Customs": "http://api.smarttrade.com/health/customs",
    "Shipping": "http://api.smarttrade.com/health/shipping",
    "Fleet": "http://api.smarttrade.com/health/fleet",
    "Payment": "http://api.smarttrade.com/health/payment",
    "ECTN": "http://api.smarttrade.com/health/ectn"
}

def check_infrastructure():
    print("--- Global Infrastructure Health Check ---")
    all_healthy = True
    for name, url in SERVICES.items():
        try:
            response = requests.get(url, timeout=5)
            if response.status_code == 200:
                print(f"[OK] {name} Service is UP")
            else:
                print(f"[ERROR] {name} Service returned {response.status_code}")
                all_healthy = False
        except Exception as e:
            print(f"[CRITICAL] {name} Service is DOWN: {str(e)}")
            all_healthy = False
    
    return all_healthy

if __name__ == "__main__":
    if check_infrastructure():
        print("\nInfrastructure is Healthy and Enterprise-Ready!")
    else:
        print("\nInfrastructure Failure Detected! Check logs immediately.")
