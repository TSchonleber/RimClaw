using System;
using Verse;

namespace OpenClaw
{
    public static class ActionExecutor
    {
        public static string ApplyActions(string rawJson)
        {
            // TODO: parse JSON + validate + apply actions
            Log.Message($"[OpenClaw] Received actions: {rawJson}");
            return "{\"status\":\"ok\"}";
        }
    }
}
