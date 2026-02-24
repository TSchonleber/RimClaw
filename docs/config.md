# RimClaw Config (draft)

## Safety toggles

- `AllowDestructive` (default: false)
  - Gates combat/attack actions until explicitly enabled.

- `MaxActionsPerTick` (default: 5)
  - Action limiter.

- `DefaultCooldownTicks` (default: 60)
  - Per-action cooldown window.

- `ActionAllowlist` (default: empty = allow all)
  - If set, only listed actions are permitted.
- Settings UI: "Action allowlist" supports comma or newline separated actions.

## External config file (optional)

Create `OpenClaw.json` in the RimWorld config folder (`%APPDATA%/RimWorld/Config` on Windows, `~/Library/Application Support/RimWorld/Config` on macOS). If present, it overrides UI settings.

Example:
```json
{
  "AllowDestructive": false,
  "MaxActionsPerTick": 5,
  "DefaultCooldownTicks": 60,
  "ActionAllowlist": ["set_priority", "queue_build"]
}
```
