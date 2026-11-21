import argparse
import json
import math
import random
import time
import uuid
from datetime import datetime, timezone
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path

REPORTS_DIR = Path(__file__).resolve().parent / "reports"
REQUIRED_REPORT_FIELDS = ("operatorId", "wizardId", "durationSec", "errors")

MACHINES = [
    {"id": "lathe", "name": "Lathe", "base_temp": 40.0, "base_rpm": 900, "phase": 0.0},
    {"id": "mill", "name": "Mill", "base_temp": 45.0, "base_rpm": 1400, "phase": 2.1},
    {"id": "press", "name": "Press", "base_temp": 50.0, "base_rpm": 60, "phase": 4.2},
]

WARNING_TEMP = 70.0
ALARM_TEMP = 80.0


def utc_now():
    return datetime.now(timezone.utc).isoformat(timespec="seconds")


def read_machine(machine):
    wave = math.sin(time.time() / 15.0 + machine["phase"])
    load = min(1.0, max(0.0, 0.6 + 0.35 * wave + random.uniform(-0.05, 0.05)))
    temperature = machine["base_temp"] + 35.0 * load + random.uniform(-1.0, 1.0)

    if temperature >= ALARM_TEMP:
        status = "alarm"
    elif temperature >= WARNING_TEMP:
        status = "warning"
    else:
        status = "ok"

    return {
        "id": machine["id"],
        "name": machine["name"],
        "temperature": round(temperature, 1),
        "rpm": int(machine["base_rpm"] * (0.5 + load)),
        "load": round(load, 2),
        "status": status,
    }


class MockHandler(BaseHTTPRequestHandler):
    delay = 0.0

    def do_GET(self):
        self.wait()
        if self.path == "/api/sensors":
            self.send_json(200, {"updatedAt": utc_now(), "machines": [read_machine(m) for m in MACHINES]})
        else:
            self.send_json(404, {"error": "not found"})

    def do_POST(self):
        self.wait()
        if self.path != "/api/reports":
            self.send_json(404, {"error": "not found"})
            return

        length = int(self.headers.get("Content-Length", 0))
        try:
            report = json.loads(self.rfile.read(length).decode("utf-8"))
        except (UnicodeDecodeError, json.JSONDecodeError):
            self.send_json(400, {"error": "body is not valid JSON"})
            return

        missing = [field for field in REQUIRED_REPORT_FIELDS if field not in report]
        if missing:
            self.send_json(400, {"error": "missing fields: " + ", ".join(missing)})
            return

        report_id = uuid.uuid4().hex[:8]
        received_at = utc_now()
        REPORTS_DIR.mkdir(exist_ok=True)
        file_name = f"{datetime.now():%Y%m%d_%H%M%S}_{report['operatorId']}_{report_id}.json"
        stored = {"id": report_id, "receivedAt": received_at, "report": report}
        (REPORTS_DIR / file_name).write_text(json.dumps(stored, ensure_ascii=False, indent=2), encoding="utf-8")

        print(f"Report {report_id}: operator {report['operatorId']}, "
              f"{report['durationSec']} s, {report['errors']} errors -> reports/{file_name}")
        self.send_json(201, {"id": report_id, "receivedAt": received_at})

    def wait(self):
        if self.delay > 0:
            time.sleep(self.delay)

    def send_json(self, code, payload):
        body = json.dumps(payload, ensure_ascii=False).encode("utf-8")
        self.send_response(code)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)


def main():
    parser = argparse.ArgumentParser(description="Mock workshop server: machine sensors and wizard reports")
    parser.add_argument("--port", type=int, default=8080)
    parser.add_argument("--delay", type=float, default=0.0, help="seconds to wait before each response")
    args = parser.parse_args()

    MockHandler.delay = args.delay
    server = ThreadingHTTPServer(("127.0.0.1", args.port), MockHandler)
    print(f"Mock server on http://localhost:{args.port} (GET /api/sensors, POST /api/reports), Ctrl+C to stop")
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        pass
    server.server_close()


if __name__ == "__main__":
    main()
