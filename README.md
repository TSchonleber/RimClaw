# RimClaw

RimClaw is a full‑control integration between **RimWorld** and **OpenClaw**. It exposes the colony as a local API and lets OpenClaw issue validated commands (priorities, build orders, draft/undraft, zones, etc.).

## What this repo contains
- **RimWorld mod** (C#) with local HTTP server (`/state`, `/actions`, `/schema`)
- **OpenClaw bridge stub** (Python) for polling state + sending actions
- **Action schema + validator** (safe, bounded execution)
- **Terminal UI** (in‑game log of actions)

## Current status
- ✅ State snapshot: pawns + basic resources
- ✅ Action schema + validation scaffold
- ✅ Action execution: priorities, build orders, draft/undraft, zones
- ✅ Terminal UI panel (auto‑opens on terminal spawn)
- ⏳ Bills, schedules, combat behaviors (next)
- ⏳ Full action validator + safety caps (next)

## Roadmap (high‑level)
1. **Foundations** — API + state snapshot + terminal
2. **Controlled actions** — priority + build orders + draft/zone
3. **Full control** — bills, scheduling, combat
4. **Intelligence** — memory, delta prompts, local model default
5. **Public release** — installer + docs

## Installation (draft)
We’ll ship full setup instructions once the mod compiles into `Assemblies/OpenClaw.dll`.

## License
TBD
