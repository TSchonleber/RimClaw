using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace OpenClaw
{
    public static class OpenClawStateBuilder
    {
        public static string Build()
        {
            var sb = new StringBuilder();
            var map = Find.CurrentMap;
            sb.Append("{");
            sb.Append("\"tick\":").Append(Find.TickManager.TicksGame).Append(',');

            // Summary placeholder (can be improved later)
            var colonists = map?.mapPawns?.FreeColonistsCount ?? 0;
            sb.Append("\"summary\":\"")
              .Append("colonists:")
              .Append(colonists)
              .Append("\"");

            // Alerts
            sb.Append(",\"alerts\":[");
            var alertList = new List<string>();
            if (map != null)
            {
                int food = CountThings(map, ThingCategoryDefOf.Foods);
                int medicine = CountThings(map, ThingDefOf.MedicineIndustrial);
                if (food < 50) alertList.Add("food_low");
                if (medicine < 3) alertList.Add("medicine_low");
                if (map.mapPawns?.AllPawnsSpawned?.Any(p => p.HostileTo(Faction.OfPlayer)) == true) alertList.Add("hostiles_present");

                // Mood alerts
                var lowMood = map.mapPawns.FreeColonists.Any(p => (p.needs?.mood?.CurLevel ?? 1f) < 0.25f);
                if (lowMood) alertList.Add("mood_critical");
            }
            for (int i = 0; i < alertList.Count; i++)
            {
                sb.Append("\"").Append(alertList[i]).Append("\"");
                if (i < alertList.Count - 1) sb.Append(',');
            }
            sb.Append("]");

            // Pawns
            sb.Append(",\"pawns\":[");
            if (map != null)
            {
                var pawns = map.mapPawns.FreeColonists.ToList();
                for (int i = 0; i < pawns.Count; i++)
                {
                    var pawn = pawns[i];
                    var mood = pawn.needs?.mood?.CurLevel ?? 0f;
                    var health = pawn.health?.summaryHealth?.SummaryHealthPercent ?? 0f;
                    sb.Append("{")
                      .Append("\"name\":\"").Append(pawn.Name?.ToStringShort ?? pawn.LabelShortCap).Append("\",")
                      .Append("\"mood\":").Append(mood.ToString("0.00")).Append(',')
                      .Append("\"health\":").Append(health.ToString("0.00")).Append(',')
                      .Append("\"work\":\"").Append(GetPrimaryWorkType(pawn)).Append("\"")
                      .Append("}");
                    if (i < pawns.Count - 1) sb.Append(',');
                }
            }
            sb.Append("]");

            // Resources (simple counts)
            sb.Append(",\"resources\":{");
            if (map != null)
            {
                int food = CountThings(map, ThingCategoryDefOf.Foods);
                int medicine = CountThings(map, ThingDefOf.MedicineIndustrial);
                sb.Append("\"food\":").Append(food).Append(',')
                  .Append("\"medicine\":").Append(medicine);
            }
            sb.Append("}");

            // Threats (hostile pawns)
            sb.Append(",\"threats\":[");
            var hostileCount = 0;
            if (map != null)
            {
                var hostiles = map.mapPawns.AllPawnsSpawned.Where(p => p.HostileTo(Faction.OfPlayer)).ToList();
                hostileCount = hostiles.Count;
                for (int i = 0; i < hostiles.Count; i++)
                {
                    sb.Append("\"").Append(hostiles[i].LabelShortCap).Append("\"");
                    if (i < hostiles.Count - 1) sb.Append(',');
                }
            }
            sb.Append("]");

            // Jobs (active)
            sb.Append(",\"jobs\":[");
            if (map != null)
            {
                var pawns = map.mapPawns.FreeColonists.ToList();
                for (int i = 0; i < pawns.Count; i++)
                {
                    var job = pawns[i].CurJob?.def?.defName ?? "idle";
                    sb.Append("\"").Append(pawns[i].LabelShortCap).Append(":").Append(job).Append("\"");
                    if (i < pawns.Count - 1) sb.Append(',');
                }
            }
            sb.Append("]");

            // Summary update if alerts
            if (alertList.Count > 0)
            {
                var alertSummary = string.Join(",", alertList.ToArray());
                sb.Append(",\"summary\":\"")
                  .Append("colonists:")
                  .Append(colonists)
                  .Append(";alerts:")
                  .Append(alertSummary)
                  .Append(";hostiles:")
                  .Append(hostileCount)
                  .Append("\"");
            }

            sb.Append("}");
            return sb.ToString();
        }

        private static string GetPrimaryWorkType(Pawn pawn)
        {
            if (pawn?.workSettings == null) return "unassigned";
            foreach (var def in DefDatabase<WorkTypeDef>.AllDefs)
            {
                if (pawn.workSettings.GetPriority(def) == 1)
                {
                    return def.defName;
                }
            }
            return "unassigned";
        }

        private static int CountThings(Map map, ThingCategoryDef category)
        {
            if (map == null || category == null) return 0;
            int count = 0;
            var things = map.listerThings.AllThings;
            foreach (var thing in things)
            {
                if (thing.def?.thingCategories != null && thing.def.thingCategories.Contains(category))
                {
                    count += thing.stackCount;
                }
            }
            return count;
        }

        private static int CountThings(Map map, ThingDef def)
        {
            if (map == null || def == null) return 0;
            int count = 0;
            var things = map.listerThings.AllThings;
            foreach (var thing in things)
            {
                if (thing.def == def)
                {
                    count += thing.stackCount;
                }
            }
            return count;
        }
    }
}
