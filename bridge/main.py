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


def diff_state(prev, curr):
    if not prev:
        return curr
    delta = {}
    for key in curr:
        if prev.get(key) != curr.get(key):
            delta[key] = curr.get(key)
    return delta


def main():
    last_state = None
    while True:
        try:
            state = poll_state()
            delta = diff_state(last_state, state)
            print("delta", json.dumps(delta)[:400])
            last_state = state
            # TODO: build prompt + call model
            send_actions([])
        except Exception as exc:
            print("bridge error", exc)
        time.sleep(CONFIG["poll_interval"])


if __name__ == "__main__":
    main()
