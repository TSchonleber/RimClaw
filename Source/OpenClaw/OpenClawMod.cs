using Verse;

namespace OpenClaw
{
    public class OpenClawMod : Mod
    {
        public static OpenClawConfig Config;

        public OpenClawMod(ModContentPack content) : base(content)
        {
            Config = GetSettings<OpenClawConfig>();
            ApplyConfig();
        }

        public override string SettingsCategory() => "OpenClaw";

        public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("Allow destructive actions", ref Config.AllowDestructive);
            listing.Label("Max actions per tick:");
            Config.MaxActionsPerTick = (int)listing.Slider(Config.MaxActionsPerTick, 1, 20);
            listing.Label("Cooldown ticks:");
            Config.DefaultCooldownTicks = (int)listing.Slider(Config.DefaultCooldownTicks, 0, 600);
            listing.End();
            ApplyConfig();
        }

        private static void ApplyConfig()
        {
            ActionPolicy.AllowDestructive = Config.AllowDestructive;
            ActionLimiter.MaxActionsPerTick = Config.MaxActionsPerTick;
            ActionPolicy.SetCooldown(Config.DefaultCooldownTicks);
            ActionPolicy.SetAllowlist(Config.ActionAllowlist);
        }
    }
}
