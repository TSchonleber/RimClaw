using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace OpenClaw
{
    public static class ActionExecutor
    {
        public static string ApplyActions(string rawJson)
        {
            var result = new ActionResult { status = "ok", errors = new List<string>() };
            if (string.IsNullOrWhiteSpace(rawJson))
            {
                result.status = "error";
                result.errors.Add("empty payload");
                return Serialize(result);
            }

            ActionRequest request = null;
            try
            {
                request = JsonUtility.FromJson<ActionRequest>(rawJson);
            }
            catch
            {
                result.status = "error";
                result.errors.Add("invalid json");
                return Serialize(result);
            }

            if (request?.actions == null || request.actions.Count == 0)
            {
                return Serialize(result);
            }

            foreach (var item in request.actions)
            {
                var errors = ActionValidator.Validate(item);
                if (errors.Count > 0)
                {
                    result.status = "error";
                    result.errors.AddRange(errors);
                    continue;
                }

                Log.Message($"[OpenClaw] Action received: {item.action}");
                OpenClawTerminalState.LastLog += $"\n{item.action} -> {item.pawn ?? item.thing}";
                try
                {
                    if (item.action == "set_priority")
                    {
                        ApplyPriority(item);
                    }
                    else if (item.action == "queue_build")
                    {
                        QueueBuild(item);
                    }
                }
                catch (System.Exception ex)
                {
                    result.status = "error";
                    result.errors.Add(ex.Message);
                }
            }

            return Serialize(result);
        }

        private static void ApplyPriority(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn?.workSettings == null) return;
            var workType = DefDatabase<WorkTypeDef>.AllDefs.FirstOrDefault(w => w.defName == item.work || w.label == item.work);
            if (workType == null) return;
            pawn.workSettings.SetPriority(workType, item.level);
        }

        private static void QueueBuild(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null || item.pos == null || item.pos.Length < 3) return;
            var thingDef = DefDatabase<ThingDef>.AllDefs.FirstOrDefault(t => t.defName == item.thing || t.label == item.thing);
            if (thingDef == null) return;
            var cell = new IntVec3(item.pos[0], item.pos[1], item.pos[2]);
            if (!cell.InBounds(map)) return;
            GenConstruct.PlaceBlueprintForBuild(thingDef, cell, map, Rot4.North, Faction.OfPlayer, null);
        }

        private static string Serialize(ActionResult result)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"status\":\"").Append(result.status).Append("\"");
            if (result.errors != null && result.errors.Count > 0)
            {
                sb.Append(",\"errors\":[");
                for (int i = 0; i < result.errors.Count; i++)
                {
                    sb.Append("\"").Append(result.errors[i]).Append("\"");
                    if (i < result.errors.Count - 1) sb.Append(",");
                }
                sb.Append("]");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
