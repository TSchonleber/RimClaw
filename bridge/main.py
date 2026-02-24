import json
import os
import time
import requests

CONFIG = {
    "base_url": "http://127.0.0.1:3456",
    "poll_interval": 60,
    "model_provider": "ollama",
    "model_name": "llama3.1:8b",
    "ollama_url": "http://127.0.0.1:11434",
    "action_allowlist": [],
}

SCHEMA_ACTIONS = set()


def load_config():
    config_path = os.environ.get(
        "RIMCLAW_CONFIG",
        os.path.join(os.path.dirname(__file__), "config.json"),
    )
    if not os.path.exists(config_path):
        return
    with open(config_path, "r") as handle:
        data = json.load(handle)
    CONFIG.update(data)


def load_schema():
    global SCHEMA_ACTIONS
    try:
        schema = requests.get(f"{CONFIG['base_url']}/schema", timeout=10).json()
        actions = schema.get("actions") or []
        SCHEMA_ACTIONS = {a.get("action") for a in actions if a.get("action")}
    except Exception as exc:
        print("schema load error", exc)
        SCHEMA_ACTIONS = set()


def poll_state():
    return requests.get(f"{CONFIG['base_url']}/state", timeout=10).json()


def validate_actions(actions):
    allowed = set(CONFIG.get("action_allowlist") or [])
    validated = []
    for action in actions:
        action_type = action.get("action") or action.get("type")
        if action_type is None:
            continue
        if SCHEMA_ACTIONS and action_type not in SCHEMA_ACTIONS:
            continue
        if allowed and action_type not in allowed:
            continue
        validated.append(action)
    return validated


def send_actions(actions):
    actions = validate_actions(actions)
    return requests.post(f"{CONFIG['base_url']}/actions", json={"actions": actions}, timeout=10).json()


def diff_state(prev, curr):
    if not prev:
        return curr
    delta = {}
    for key in curr:
        if prev.get(key) != curr.get(key):
            delta[key] = curr.get(key)
    return delta


def should_trigger(state):
    alerts = state.get("alerts") or []
    return any(a in ["food_low", "medicine_low", "hostiles_present", "mood_critical"] for a in alerts)


def pack_prompt(state, delta, last_response):
    return {
        "summary": state.get("summary"),
        "alerts": state.get("alerts"),
        "threats": state.get("threats"),
        "jobs": state.get("jobs"),
        "delta": delta,
        "last_response": last_response,
    }


def call_model(payload):
    if CONFIG.get("model_provider") == "ollama":
        prompt = json.dumps(payload)
        resp = requests.post(
            f"{CONFIG['ollama_url']}/api/generate",
            json={"model": CONFIG["model_name"], "prompt": prompt, "stream": False},
            timeout=60,
        )
        data = resp.json()
        return data.get("response", "{}").strip()
    return "{}"


def parse_actions(text):
    try:
        obj = json.loads(text)
        return obj.get("actions", [])
    except Exception:
        return []


def main():
    load_config()
    load_schema()
    last_state = None
    last_action_response = None
    while True:
        try:
            state = poll_state()
            delta = diff_state(last_state, state)
            print("delta", json.dumps(delta)[:400])
            last_state = state

            actions = []
            if should_trigger(state):
                payload = pack_prompt(state, delta, last_action_response)
                print("trigger", json.dumps(payload)[:400])
                model_text = call_model(payload)
                actions = parse_actions(model_text)

            last_action_response = send_actions(actions)
        except Exception as exc:
            print("bridge error", exc)
        time.sleep(CONFIG["poll_interval"])


if __name__ == "__main__":
    main()
