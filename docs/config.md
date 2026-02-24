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
