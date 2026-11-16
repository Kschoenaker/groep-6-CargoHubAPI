# Performance- en concurrencytest voor de CargoHub API (hoofdstuk 4 van het testrapport).
#
# Gebruik:
#   1. Start de API:  cd MyApi  en dan  dotnet run -c Release --no-launch-profile
#   2. Draai dit script in een tweede terminal:  python PerformanceTests/bench.py
#
# Het script gebruikt alleen standaard Python (geen extra packages nodig).

import json
import statistics
import time
import urllib.error
import urllib.request
from collections import Counter
from concurrent.futures import ThreadPoolExecutor

BASE_URL = "http://localhost:3000"


def get(path):
    """Stuurt een GET-request en geeft (tijd in ms, statuscode) terug."""
    start = time.perf_counter()
    try:
        with urllib.request.urlopen(BASE_URL + path, timeout=30) as response:
            response.read()
            code = response.status
    except urllib.error.HTTPError as error:
        code = error.code
    except Exception:
        code = 0  # geen verbinding of timeout
    return (time.perf_counter() - start) * 1000, code


def post(path, data):
    """Stuurt een POST-request met JSON en geeft (statuscode, antwoord) terug."""
    request = urllib.request.Request(
        BASE_URL + path,
        data=json.dumps(data).encode(),
        headers={"Content-Type": "application/json"},
        method="POST",
    )
    try:
        with urllib.request.urlopen(request, timeout=30) as response:
            return response.status, json.loads(response.read())
    except urllib.error.HTTPError as error:
        return error.code, None


def delete(path):
    request = urllib.request.Request(BASE_URL + path, method="DELETE")
    try:
        with urllib.request.urlopen(request, timeout=30) as response:
            return response.status
    except urllib.error.HTTPError as error:
        return error.code


def percentiel(waarden, p):
    waarden = sorted(waarden)
    return waarden[min(len(waarden) - 1, int(round(p * (len(waarden) - 1))))]


# ---------------------------------------------------------------------------
# 4.1 Endpoint-responsiviteit: elk endpoint 5 keer opwarmen, daarna 50 keer meten
# ---------------------------------------------------------------------------
ENDPOINTS = [
    "/api/v2/item",
    "/api/v2/item/1",
    "/api/v2/locations",
    "/api/v2/locations/1",
    "/api/v2/warehouses",
    "/api/v2/transfers",
    "/api/v2/shipments",
    "/api/v1/suppliers",
    "/api/v1/inventories",
    "/api/v1/clients",
    "/api/v1/orders",
    "/api/v2/itemtypes",
]

print("== 4.1 Responsiviteit (5 keer opwarmen, 50 metingen per endpoint)")
for path in ENDPOINTS:
    for _ in range(5):
        get(path)
    resultaten = [get(path) for _ in range(50)]
    tijden = [ms for ms, _ in resultaten]
    codes = sorted({code for _, code in resultaten})
    print(
        f"{path:<25} status={codes}  gemiddeld={statistics.mean(tijden):.1f} ms  "
        f"langzaamste={max(tijden):.1f} ms"
    )

# ---------------------------------------------------------------------------
# 4.2 Concurrency: 1000 requests met 1, 10, 50 en 100 gebruikers (threads) tegelijk
# ---------------------------------------------------------------------------
print("\n== 4.2 Concurrency: 1000 x GET /api/v2/locations")
for gebruikers in [1, 10, 50, 100]:
    start = time.perf_counter()
    with ThreadPoolExecutor(gebruikers) as pool:
        resultaten = list(pool.map(lambda _: get("/api/v2/locations"), range(1000)))
    totaal = time.perf_counter() - start
    tijden = [ms for ms, _ in resultaten]
    fouten = sum(1 for _, code in resultaten if code != 200)
    print(
        f"{gebruikers:>3} tegelijk  totaal={totaal:.2f} s  requests/s={1000 / totaal:.0f}  "
        f"gemiddeld={statistics.mean(tijden):.1f} ms  p95={percentiel(tijden, 0.95):.1f} ms  "
        f"langzaamste={max(tijden):.1f} ms  fouten={fouten}"
    )

# ---------------------------------------------------------------------------
# 4.2 Gelijktijdig schrijven: 100 gebruikers maken tegelijk dezelfde locatie aan.
# Verwacht: precies 1 keer 201 Created en 99 keer 409 Conflict.
# De aangemaakte testlocatie wordt daarna weer verwijderd.
# ---------------------------------------------------------------------------
print("\n== 4.2 Gelijktijdig schrijven: 100 x POST /api/v2/locations met dezelfde code")
locatie = {"warehouse_id": 1, "code": "BENCH.DUP", "name": "bench"}
with ThreadPoolExecutor(100) as pool:
    resultaten = list(pool.map(lambda _: post("/api/v2/locations", locatie), range(100)))
print("statuscodes:", dict(Counter(code for code, _ in resultaten)))

for code, body in resultaten:
    if code == 201 and body:
        print(f"testlocatie {body['id']} verwijderd:", delete(f"/api/v2/locations/{body['id']}"))
