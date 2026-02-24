using System.Collections.Generic;
using Verse;

namespace OpenClaw
{
    public static class ActionLimiter
    {
        public static int MaxActionsPerTick = 5;
        private static int _lastTick = -1;
        private static int _countThisTick = 0;
        private static int _errorStreak = 0;

        public static bool AllowAction(out string error)
        {
            error = null;
            var tick = Find.TickManager?.TicksGame ?? 0;
            if (tick != _lastTick)
            {
                _lastTick = tick;
                _countThisTick = 0;
            }

            if (_errorStreak >= 3)
            {
                error = "action lockout due to errors";
                return false;
            }

            if (_countThisTick >= MaxActionsPerTick)
            {
                error = "action limit reached";
                return false;
            }

            _countThisTick++;
            return true;
        }

        public static void RegisterError()
        {
            _errorStreak++;
        }

        public static void ClearErrors()
        {
            _errorStreak = 0;
        }
    }
}
