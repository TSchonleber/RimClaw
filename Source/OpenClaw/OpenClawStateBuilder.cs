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
            sb.Append("\"summary\":\"")
              .Append("colonists:")
              .Append(map?.mapPawns?.FreeColonistsCount ?? 0)
              .Append("\"");

            // Alerts
            sb.Append(",\"alerts\":[");
            if (map != null)
            {
                var alerts = new List<string>();
                int food = CountThings(map, ThingCategoryDefOf.Foods);
                int medicine = CountThings(map, ThingDefOf.MedicineIndustrial);
                if (food < 50) alerts.Add("food_low");
                if (medicine < 3) alerts.Add("medicine_low");
                if (map.mapPawns?.AllPawnsSpawned?.Any(p => p.HostileTo(Faction.OfPlayer)) == true) alerts.Add("hostiles_present");
                for (int i = 0; i < alerts.Count; i++)
                {
                    sb.Append("\"").Append(alerts[i]).Append("\"");
                    if (i < alerts.Count - 1) sb.Append(',');
                }
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
            if (map != null)
            {
                var hostiles = map.mapPawns.AllPawnsSpawned.Where(p => p.HostileTo(Faction.OfPlayer)).ToList();
                for (int i = 0; i < hostiles.Count; i++)
                {
                    sb.Append("\"").Append(hostiles[i].LabelShortCap).Append("\"");
                    if (i < hostiles.Count - 1) sb.Append(',');
                }
            }
            sb.Append("]");

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
