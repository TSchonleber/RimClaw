using System.Collections.Generic;
using Verse;

namespace OpenClaw
{
    public class OpenClawConfig : ModSettings
    {
        public bool AllowDestructive = false;
        public int MaxActionsPerTick = 5;
        public int DefaultCooldownTicks = 60;
        public List<string> ActionAllowlist = new List<string>();
        public string ActionAllowlistCsv = "";

        public override void ExposeData()
        {
            Scribe_Values.Look(ref AllowDestructive, "AllowDestructive", false);
            Scribe_Values.Look(ref MaxActionsPerTick, "MaxActionsPerTick", 5);
            Scribe_Values.Look(ref DefaultCooldownTicks, "DefaultCooldownTicks", 60);
            Scribe_Collections.Look(ref ActionAllowlist, "ActionAllowlist", LookMode.Value);
            Scribe_Values.Look(ref ActionAllowlistCsv, "ActionAllowlistCsv", "");
        }
    }
}
