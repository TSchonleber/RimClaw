# Release Checklist (draft)

## Build
- [ ] Build `OpenClaw.dll` (Release)
- [ ] Copy to `Mod/Assemblies/`
- [ ] Verify mod loads + HTTP server starts

## Smoke Test
- [ ] `/state` returns JSON
- [ ] `/schema` returns actions
- [ ] `/actions` accepts a safe action (e.g., set_priority)
- [ ] `/delta` returns data when tick advances

## Safety
- [ ] Allowlist works (deny unlisted action)
- [ ] Destructive gate blocks combat by default
- [ ] Rate limiting triggers as expected

## Bridge
- [ ] `bridge/config.json` in place
- [ ] `use_delta` works with polling
- [ ] Model response parses to actions

## Docs
- [ ] Update README status
- [ ] Update install guide
- [ ] Tag release + version
