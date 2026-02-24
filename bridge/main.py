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
    "max_actions_per_cycle": 5,
    "trigger_alerts": ["food_low", "medicine_low", "hostiles_present", "mood_critical"],
    "trigger_delta_keys": ["alerts", "threats", "jobs"],
    "prompt_system_path": "prompts/system.txt",
    "prompt_user_path": "prompts/user.txt",
    "use_delta": true,
    "safe_mode": true,
    "action_cooldowns": {},
}

SCHEMA_ACTIONS = set()
PROMPT_SYSTEM = ""
PROMPT_USER = ""
ACTION_LAST_RUN = {}
DESTRUCTIVE_ACTIONS = {
    "attack",
    "attack_ranged",
    "attack_pos",
    "attack_thing",
    "group_attack",
}


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


def load_prompts():
    global PROMPT_SYSTEM, PROMPT_USER
    base_dir = os.path.dirname(__file__)
    system_path = os.path.join(base_dir, CONFIG.get("prompt_system_path", "prompts/system.txt"))
    user_path = os.path.join(base_dir, CONFIG.get("prompt_user_path", "prompts/user.txt"))
    if os.path.exists(system_path):
        with open(system_path, "r") as handle:
            PROMPT_SYSTEM = handle.read().strip()
    if os.path.exists(user_path):
        with open(user_path, "r") as handle:
            PROMPT_USER = handle.read().strip()


def load_schema():
    global SCHEMA_ACTIONS
    try:
        schema = requests.get(f"{CONFIG['base_url']}/schema", timeout=10).json()
        actions = schema.get("actions") or []
        SCHEMA_ACTIONS = {a.get("action") for a in actions if a.get("action")}
    except Exception as exc:
        print("schema load error", exc)
        SCHEMA_ACTIONS = set()


def poll_state(last_tick):
    if CONFIG.get("use_delta"):
        resp = requests.get(f"{CONFIG['base_url']}/delta", params={"since": last_tick}, timeout=10).json()
        tick = resp.get("tick", last_tick)
        delta = resp.get("delta") or {}
        if delta == {}:
            return None, tick
        return delta, tick
    return requests.get(f"{CONFIG['base_url']}/state", timeout=10).json(), last_tick


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


def should_trigger(state, delta):
    alerts = state.get("alerts") or []
    trigger_alerts = set(CONFIG.get("trigger_alerts") or [])
    if any(a in trigger_alerts for a in alerts):
        return True
    trigger_delta_keys = set(CONFIG.get("trigger_delta_keys") or [])
    return any(k in delta for k in trigger_delta_keys)


def pack_prompt(state, delta, last_response):
    return {
        "system": PROMPT_SYSTEM,
        "summary": state.get("summary"),
        "alerts": state.get("alerts"),
        "threats": state.get("threats"),
        "jobs": state.get("jobs"),
        "delta": delta,
        "last_response": last_response,
        "instructions": PROMPT_USER,
        "schema_actions": sorted(SCHEMA_ACTIONS),
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


def apply_policy(actions):
    max_actions = CONFIG.get("max_actions_per_cycle", 5)
    if not actions:
        return []

    filtered = []
    safe_mode = CONFIG.get("safe_mode", True)
    cooldowns = CONFIG.get("action_cooldowns") or {}
    now = time.time()

    for action in actions:
        action_type = action.get("action") or action.get("type")
        if action_type is None:
            continue
        if safe_mode and action_type in DESTRUCTIVE_ACTIONS:
            continue
        cooldown = cooldowns.get(action_type)
        if cooldown:
            last = ACTION_LAST_RUN.get(action_type, 0)
            if now - last < cooldown:
                continue
        filtered.append(action)
        ACTION_LAST_RUN[action_type] = now
        if len(filtered) >= max_actions:
            break

    return filtered


def main():
    load_config()
    load_prompts()
    load_schema()
    last_state = None
    last_tick = 0
    last_action_response = None
    while True:
        try:
            state, last_tick = poll_state(last_tick)
            if state is None:
                time.sleep(CONFIG["poll_interval"])
                continue
            delta = diff_state(last_state, state)
            print("delta", json.dumps(delta)[:400])
            last_state = state

            actions = []
            if should_trigger(state, delta):
                payload = pack_prompt(state, delta, last_action_response)
                print("trigger", json.dumps(payload)[:400])
                model_text = call_model(payload)
                actions = apply_policy(parse_actions(model_text))

            last_action_response = send_actions(actions)
        except Exception as exc:
            print("bridge error", exc)
        time.sleep(CONFIG["poll_interval"])


if __name__ == "__main__":
    main()
