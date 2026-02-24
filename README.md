# RimClaw

RimClaw is a full‑control integration between **RimWorld** and **OpenClaw**. It exposes the colony as a local API and lets OpenClaw issue validated commands (priorities, build orders, draft/undraft, zones, etc.).

## What this repo contains
- **RimWorld mod** (C#) with local HTTP server (`/state`, `/delta`, `/actions`, `/schema`)
- **OpenClaw bridge stub** (Python) for polling state + sending actions
- **Action schema + validator** (safe, bounded execution)
- **Terminal UI** (in‑game log of actions)

## Current status
- ✅ State snapshot: pawns + basic resources
- ✅ Action schema + validation scaffold
- ✅ Actions: priorities, build orders, draft/undraft, zones
- ✅ Bills: create, repeat/pause/target, skill range, ingredient filters
- ✅ Schedules: set timetable
- ✅ Combat: move, attack (melee/ranged/pos/thing), flee/retreat, group orders
- ✅ Terminal UI panel (auto‑opens on terminal spawn)
- ✅ Action rate limiting + cooldowns
- ✅ Destructive action gate (off by default)
- ✅ Allowlists + config settings (UI + external JSON)

## Roadmap (full)
- [x] **Phase 0 — Decisions & Architecture**
  - Full‑control RimWorld mod + OpenClaw bridge
  - Local model default Llama‑3.1 8B (14B optional)
  - Event‑driven + 60s polling
- [x] **Phase 1 — Foundations**
  - Local HTTP server (`/state`, `/actions`, `/schema`)
  - State snapshot + in‑game terminal UI
- [x] **Phase 2 — Controlled Actions**
  - Priorities, build queue, draft/undraft, zones
- [x] **Phase 3 — Full Control**
  - Bills, schedules, combat, group orders
- [x] **Phase 4 — Safety Layer**
  - Rate limiting + cooldowns
  - Destructive action gate
  - Allowlist (UI + external config)
- [ ] **Phase 5 — Intelligence**
  - Delta prompts + policy scaffolding
  - Model defaults + prompt templates
- [ ] **Phase 6 — Public Release**
  - Installer, docs, versioning

## Installation (draft)
We’ll ship full setup instructions once the mod compiles into `Assemblies/OpenClaw.dll`.

## Bridge config (draft)
Copy `bridge/config.sample.json` to `bridge/config.json` (or set `RIMCLAW_CONFIG`) to override defaults, including action allowlist and delta polling (`use_delta`).

## License
TBD
