using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace OpenClaw
{
    public class OpenClawMod : Mod
    {
        public static OpenClawConfig Config;

        [Serializable]
        private class ExternalConfig
        {
            public bool AllowDestructive = false;
            public int MaxActionsPerTick = 5;
            public int DefaultCooldownTicks = 60;
            public List<string> ActionAllowlist = new List<string>();
        }

        public OpenClawMod(ModContentPack content) : base(content)
        {
            Config = GetSettings<OpenClawConfig>();
            ApplyConfig();
        }

        public override string SettingsCategory() => "OpenClaw";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("Allow destructive actions", ref Config.AllowDestructive);
            listing.Label("Max actions per tick:");
            Config.MaxActionsPerTick = (int)listing.Slider(Config.MaxActionsPerTick, 1, 20);
            listing.Label("Cooldown ticks:");
            Config.DefaultCooldownTicks = (int)listing.Slider(Config.DefaultCooldownTicks, 0, 600);
            listing.GapLine();
            listing.Label("Action allowlist (comma or newline separated). Empty = allow all:");
            var rect = listing.GetRect(80f);
            Config.ActionAllowlistCsv = Widgets.TextArea(rect, Config.ActionAllowlistCsv ?? "");
            listing.End();
            ApplyConfig();
        }

        private static void ApplyConfig()
        {
            var fileConfig = LoadExternalConfig();
            if (fileConfig != null)
            {
                Config.AllowDestructive = fileConfig.AllowDestructive;
                if (fileConfig.MaxActionsPerTick > 0) Config.MaxActionsPerTick = fileConfig.MaxActionsPerTick;
                if (fileConfig.DefaultCooldownTicks >= 0) Config.DefaultCooldownTicks = fileConfig.DefaultCooldownTicks;
                if (fileConfig.ActionAllowlist != null)
                {
                    Config.ActionAllowlist = fileConfig.ActionAllowlist;
                    Config.ActionAllowlistCsv = string.Join(", ", fileConfig.ActionAllowlist);
                }
            }

            if (string.IsNullOrWhiteSpace(Config.ActionAllowlistCsv) && Config.ActionAllowlist != null && Config.ActionAllowlist.Count > 0)
            {
                Config.ActionAllowlistCsv = string.Join(", ", Config.ActionAllowlist);
            }

            var allowlist = ParseAllowlist(Config.ActionAllowlistCsv);
            Config.ActionAllowlist = allowlist;

            ActionPolicy.AllowDestructive = Config.AllowDestructive;
            ActionLimiter.MaxActionsPerTick = Config.MaxActionsPerTick;
            ActionPolicy.SetCooldown(Config.DefaultCooldownTicks);
            ActionPolicy.SetAllowlist(allowlist);
        }

        private static ExternalConfig LoadExternalConfig()
        {
            try
            {
                var path = Path.Combine(GenFilePaths.ConfigFolderPath, "OpenClaw.json");
                if (!File.Exists(path)) return null;
                var json = File.ReadAllText(path);
                return JsonUtility.FromJson<ExternalConfig>(json);
            }
            catch (Exception ex)
            {
                Log.Error($"[OpenClaw] Failed to load OpenClaw.json: {ex}");
                return null;
            }
        }

        private static List<string> ParseAllowlist(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return new List<string>();
            var parts = input.Split(new[] { ',', '\n', '\r', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
        }
    }
}
