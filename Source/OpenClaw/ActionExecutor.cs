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
                    else if (item.action == "draft")
                    {
                        SetDraft(item, true);
                    }
                    else if (item.action == "undraft")
                    {
                        SetDraft(item, false);
                    }
                    else if (item.action == "set_zone")
                    {
                        SetZone(item);
                    }
                    else if (item.action == "create_bill")
                    {
                        CreateBill(item);
                    }
                    else if (item.action == "set_bill_count")
                    {
                        SetBillCount(item);
                    }
                    else if (item.action == "set_bill_pause")
                    {
                        SetBillPause(item);
                    }
                    else if (item.action == "set_bill_repeat_mode")
                    {
                        SetBillRepeatMode(item);
                    }
                    else if (item.action == "set_bill_target")
                    {
                        SetBillTarget(item);
                    }
                    else if (item.action == "set_bill_skill_range")
                    {
                        SetBillSkillRange(item);
                    }
                    else if (item.action == "set_schedule")
                    {
                        SetSchedule(item);
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

        private static void SetDraft(ActionItem item, bool drafted)
        {
            var map = Find.CurrentMap;
            if (map == null) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn?.drafter == null) return;
            pawn.drafter.Drafted = drafted;
        }

        private static void SetZone(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn == null || string.IsNullOrWhiteSpace(item.zone)) return;
            var zone = map.zoneManager.AllZones.FirstOrDefault(z => z.label == item.zone);
            if (zone == null) return;
            pawn.playerSettings.AreaRestriction = zone as Area;
        }

        private static void CreateBill(ActionItem item)
        {
            var table = FindTable(item.table);
            if (table == null) return;
            var recipe = DefDatabase<RecipeDef>.AllDefs.FirstOrDefault(r => r.defName == item.recipe || r.label == item.recipe);
            if (recipe == null) return;
            var bill = recipe.MakeNewBill();
            table.BillStack.AddBill(bill);
        }

        private static void SetBillCount(ActionItem item)
        {
            var bill = FindBill(item);
            if (bill == null) return;
            bill.repeatMode = BillRepeatModeDefOf.RepeatCount;
            bill.repeatCount = item.count;
        }

        private static void SetBillPause(ActionItem item)
        {
            var bill = FindBill(item);
            if (bill == null) return;
            bill.suspended = item.paused;
        }

        private static void SetBillRepeatMode(ActionItem item)
        {
            var bill = FindBill(item);
            if (bill == null) return;
            if (item.repeat_mode == "forever")
            {
                bill.repeatMode = BillRepeatModeDefOf.RepeatForever;
            }
            else if (item.repeat_mode == "count")
            {
                bill.repeatMode = BillRepeatModeDefOf.RepeatCount;
            }
            else if (item.repeat_mode == "target")
            {
                bill.repeatMode = BillRepeatModeDefOf.TargetCount;
            }
        }

        private static void SetBillTarget(ActionItem item)
        {
            var bill = FindBill(item);
            if (bill == null) return;
            bill.repeatMode = BillRepeatModeDefOf.TargetCount;
            bill.targetCount = item.target;
        }

        private static void SetBillSkillRange(ActionItem item)
        {
            var bill = FindBill(item);
            if (bill == null) return;
            bill.allowedSkillRange = new FloatRange(item.min_skill, item.max_skill);
        }

        private static void SetSchedule(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn?.timetable == null) return;
            var parts = item.schedule.Split(',');
            if (parts.Length != 24) return;
            for (int hour = 0; hour < 24; hour++)
            {
                var label = parts[hour].Trim().ToLower();
                var assignment = TimeAssignmentDefOf.Anything;
                if (label == "work") assignment = TimeAssignmentDefOf.Work;
                else if (label == "sleep") assignment = TimeAssignmentDefOf.Sleep;
                else if (label == "joy") assignment = TimeAssignmentDefOf.Joy;
                else if (label == "meditate") assignment = TimeAssignmentDefOf.Meditate;
                pawn.timetable.SetAssignment(hour, assignment);
            }
        }

        private static Bill FindBill(ActionItem item)
        {
            var table = FindTable(item.table);
            if (table == null) return null;
            var recipe = DefDatabase<RecipeDef>.AllDefs.FirstOrDefault(r => r.defName == item.recipe || r.label == item.recipe);
            if (recipe == null) return null;
            foreach (var bill in table.BillStack)
            {
                if (bill.recipe == recipe)
                {
                    return bill;
                }
            }
            return null;
        }

        private static Building_WorkTable FindTable(string table)
        {
            var map = Find.CurrentMap;
            if (map == null || string.IsNullOrWhiteSpace(table)) return null;
            return map.listerBuildings.AllBuildingsColonistOfClass<Building_WorkTable>()
                .FirstOrDefault(b => b.def.defName == table || b.LabelShortCap == table || b.def.label == table);
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
