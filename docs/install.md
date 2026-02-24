# Install (draft)

## Build the mod
1. Open `Source/OpenClaw/OpenClaw.csproj` in Visual Studio or Rider.
2. Add references to RimWorld `Managed` DLLs (Assembly-CSharp.dll, UnityEngine.dll).
3. Build Release.
4. Copy `OpenClaw.dll` into `Mod/Assemblies/`.

## Enable in RimWorld
1. Place the `Mod/` folder into your RimWorld mods directory.
2. Enable **OpenClaw RimWorld Integration** in the Mod list.
3. Launch a colony and confirm the terminal UI appears when placed/spawned.

## Configure (optional)
- Mod settings UI: set allowlist, cooldowns, and destructive gate.
- External config: create `OpenClaw.json` in the RimWorld Config folder.

## Bridge
- Copy `bridge/config.sample.json` to `bridge/config.json`.
- Run `python bridge/main.py`.
