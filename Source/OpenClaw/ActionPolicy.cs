using System.Collections.Generic;
using Verse;

namespace OpenClaw
{
    public static class ActionPolicy
    {
        public static bool AllowDestructive = false;
        private static readonly Dictionary<string, int> _cooldowns = new Dictionary<string, int>();
        private static readonly int DefaultCooldownTicks = 60;

        public static bool AllowAction(string key, out string error)
        {
            error = null;
            var tick = Find.TickManager?.TicksGame ?? 0;
            if (_cooldowns.TryGetValue(key, out var lastTick))
            {
                if (tick - lastTick < DefaultCooldownTicks)
                {
                    error = "cooldown active";
                    return false;
                }
            }
            _cooldowns[key] = tick;
            return true;
        }
    }
}
