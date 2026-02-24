using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace OpenClaw
{
    public static class OpenClawStateBuilder
    {
        private static int _version = 0;
        private static int _lastTick = 0;
        private static string _lastStateJson = "{}";

        public static string Build()
        {
            var sb = new StringBuilder();
            var map = Find.CurrentMap;
            var tick = Find.TickManager.TicksGame;
            _version += 1;
            sb.Append("{");
            sb.Append("\"tick\":").Append(tick).Append(',');
            sb.Append("\"version\":").Append(_version).Append(',');

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
                      .Append("\"work\":\"").Append(GetPrimaryWorkType(pawn)).Append("\",")
                      .Append("\"skills\":{")
                      .Append(BuildSkills(pawn)).Append("},")
                      .Append("\"traits\":[").Append(BuildTraits(pawn)).Append("],")
                      .Append("\"injuries\":[").Append(BuildInjuries(pawn)).Append("],")
                      .Append("\"needs\":{")
                      .Append("\"food\":").Append(GetNeed(pawn, "Food").ToString("0.00")).Append(',')
                      .Append("\"rest\":").Append(GetNeed(pawn, "Rest").ToString("0.00")).Append(',')
                      .Append("\"joy\":").Append(GetNeed(pawn, "Joy").ToString("0.00")).Append('}')
                      .Append(',')
                      .Append("\"inventory\":[");

                    var inventory = pawn.inventory?.innerContainer;
                    if (inventory != null)
                    {
                        var items = inventory.ToList();
                        for (int j = 0; j < items.Count; j++)
                        {
                            var item = items[j];
                            sb.Append("{")
                              .Append("\"name\":\"").Append(item.def?.defName ?? item.LabelShortCap).Append("\",")
                              .Append("\"count\":").Append(item.stackCount)
                              .Append("}");
                            if (j < items.Count - 1) sb.Append(',');
                        }
                    }
                    sb.Append("]}");
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
                int steel = CountThings(map, ThingDefOf.Steel);
                int wood = CountThings(map, ThingDefOf.WoodLog);
                int silver = CountThings(map, ThingDefOf.Silver);
                int components = CountThings(map, ThingDefOf.ComponentIndustrial);
                int meals = CountThings(map, ThingDefOf.MealSimple)
                            + CountThings(map, ThingDefOf.MealFine)
                            + CountThings(map, ThingDefOf.MealLavish)
                            + CountThings(map, ThingDefOf.MealSurvivalPack);
                sb.Append("\"food\":").Append(food).Append(',')
                  .Append("\"medicine\":").Append(medicine).Append(',')
                  .Append("\"steel\":").Append(steel).Append(',')
                  .Append("\"wood\":").Append(wood).Append(',')
                  .Append("\"silver\":").Append(silver).Append(',')
                  .Append("\"components\":").Append(components).Append(',')
                  .Append("\"meals\":").Append(meals);
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
            _lastTick = tick;
            _lastStateJson = sb.ToString();
            return _lastStateJson;
        }

        public static string BuildDelta(int sinceTick)
        {
            var current = Build();
            if (sinceTick >= _lastTick)
            {
                return "{\"tick\":" + _lastTick + ",\"delta\":{}}";
            }
            return "{\"tick\":" + _lastTick + ",\"delta\":" + current + "}";
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

        private static float GetNeed(Pawn pawn, string needDefName)
        {
            var need = pawn?.needs?.AllNeeds?.FirstOrDefault(n => n.def?.defName == needDefName);
            return need?.CurLevel ?? 0f;
        }

        private static string BuildSkills(Pawn pawn)
        {
            if (pawn?.skills == null) return "";
            var sb = new StringBuilder();
            var skills = pawn.skills.skills;
            for (int i = 0; i < skills.Count; i++)
            {
                var skill = skills[i];
                sb.Append("\"").Append(skill.def?.defName).Append("\":").Append(skill.Level);
                if (i < skills.Count - 1) sb.Append(',');
            }
            return sb.ToString();
        }

        private static string BuildTraits(Pawn pawn)
        {
            if (pawn?.story?.traits == null) return "";
            var traits = pawn.story.traits.allTraits;
            var sb = new StringBuilder();
            for (int i = 0; i < traits.Count; i++)
            {
                var trait = traits[i];
                sb.Append("\"").Append(trait.def?.defName).Append("\"");
                if (i < traits.Count - 1) sb.Append(',');
            }
            return sb.ToString();
        }

        private static string BuildInjuries(Pawn pawn)
        {
            if (pawn?.health?.hediffSet == null) return "";
            var injuries = pawn.health.hediffSet.hediffs.Where(h => h.Visible && h.Severity > 0f).ToList();
            var sb = new StringBuilder();
            for (int i = 0; i < injuries.Count; i++)
            {
                var injury = injuries[i];
                sb.Append("\"").Append(injury.def?.defName).Append(":").Append(injury.Severity.ToString("0.00")).Append("\"");
                if (i < injuries.Count - 1) sb.Append(',');
            }
            return sb.ToString();
        }
    }
}
