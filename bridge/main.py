import json
import time
import requests

CONFIG = {
    "base_url": "http://127.0.0.1:3456",
    "poll_interval": 60,
}


def poll_state():
    return requests.get(f"{CONFIG['base_url']}/state", timeout=10).json()


def send_actions(actions):
    return requests.post(f"{CONFIG['base_url']}/actions", json={"actions": actions}, timeout=10).json()


def main():
    while True:
        try:
            state = poll_state()
            # TODO: build prompt + call model
            print("state", json.dumps(state)[:200])
            # placeholder no-op
            send_actions([])
        except Exception as exc:
            print("bridge error", exc)
        time.sleep(CONFIG["poll_interval"])


if __name__ == "__main__":
    main()
