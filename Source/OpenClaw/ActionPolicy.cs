using System.Collections.Generic;
using Verse;

namespace OpenClaw
{
    public static class ActionPolicy
    {
        public static bool AllowDestructive = false;
        private static readonly Dictionary<string, int> _cooldowns = new Dictionary<string, int>();
        private static int DefaultCooldownTicks = 60;
        private static readonly HashSet<string> DestructiveActions = new HashSet<string>
        {
            "queue_deconstruct",
            "attack",
            "attack_ranged",
            "attack_pos",
            "attack_thing"
        };

        private static HashSet<string> _allowlist = new HashSet<string>();

        public static bool AllowAction(string action, string key, out string error)
        {
            error = null;
            if (_allowlist != null && _allowlist.Count > 0 && !_allowlist.Contains(action))
            {
                error = "action not in allowlist";
                return false;
            }

            if (!AllowDestructive && DestructiveActions.Contains(action))
            {
                error = "destructive actions disabled";
                return false;
            }

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

        public static void SetCooldown(int ticks)
        {
            DefaultCooldownTicks = ticks;
        }

        public static void SetAllowlist(List<string> allowlist)
        {
            _allowlist = allowlist == null ? new HashSet<string>() : new HashSet<string>(allowlist);
        }
    }
}
