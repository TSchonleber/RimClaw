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
                    ActionLimiter.RegisterError();
                    continue;
                }

                if (!ActionLimiter.AllowAction(out var limitError))
                {
                    result.status = "error";
                    result.errors.Add(limitError);
                    continue;
                }

                var key = $"{item.action}:{item.pawn ?? item.thing ?? item.target}";
                if (!ActionPolicy.AllowAction(item.action, key, out var policyError))
                {
                    result.status = "error";
                    result.errors.Add(policyError);
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
                    else if (item.action == "set_bill_ingredients")
                    {
                        SetBillIngredients(item);
                    }
                    else if (item.action == "move_to")
                    {
                        MoveTo(item);
                    }
                    else if (item.action == "attack")
                    {
                        AttackTarget(item);
                    }
                    else if (item.action == "attack_ranged")
                    {
                        AttackRanged(item);
                    }
                    else if (item.action == "flee")
                    {
                        Flee(item);
                    }
                    else if (item.action == "hold_position")
                    {
                        HoldPosition(item);
                    }
                    else if (item.action == "attack_pos")
                    {
                        AttackPos(item);
                    }
                    else if (item.action == "attack_thing")
                    {
                        AttackThing(item);
                    }
                    else if (item.action == "retreat")
                    {
                        Retreat(item);
                    }
                    else if (item.action == "group_draft")
                    {
                        GroupDraft(item, true);
                    }
                    else if (item.action == "group_undraft")
                    {
                        GroupDraft(item, false);
                    }
                    else if (item.action == "group_move")
                    {
                        GroupMove(item);
                    }
                    else if (item.action == "group_attack")
                    {
                        GroupAttack(item);
                    }
                }
                catch (System.Exception ex)
                {
                    result.status = "error";
                    result.errors.Add(ex.Message);
                    ActionLimiter.RegisterError();
                }
            }

            if (result.status == "ok") ActionLimiter.ClearErrors();

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

        private static void SetBillIngredients(ActionItem item)
        {
            var bill = FindBill(item);
            if (bill == null || item.ingredients == null) return;
            bill.ingredientFilter.SetDisallowAll();
            foreach (var name in item.ingredients)
            {
                var def = DefDatabase<ThingDef>.AllDefs.FirstOrDefault(t => t.defName == name || t.label == name);
                if (def != null)
                {
                    bill.ingredientFilter.SetAllow(def, true);
                }
            }
        }

        private static void MoveTo(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null || item.target_pos == null || item.target_pos.Length < 3) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn == null) return;
            var cell = new IntVec3(item.target_pos[0], item.target_pos[1], item.target_pos[2]);
            if (!cell.InBounds(map)) return;
            if (pawn.drafter != null) pawn.drafter.Drafted = true;
            pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Goto, cell));
        }

        private static void AttackTarget(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn == null) return;
            var targetPawn = map.mapPawns.AllPawnsSpawned.FirstOrDefault(p => p.LabelShortCap == item.target || p.Name?.ToStringShort == item.target);
            if (targetPawn == null) return;
            if (pawn.drafter != null) pawn.drafter.Drafted = true;
            pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.AttackMelee, targetPawn));
        }

        private static void AttackRanged(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn == null) return;
            var targetPawn = map.mapPawns.AllPawnsSpawned.FirstOrDefault(p => p.LabelShortCap == item.target || p.Name?.ToStringShort == item.target);
            if (targetPawn == null) return;
            if (pawn.drafter != null) pawn.drafter.Drafted = true;
            pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.AttackStatic, targetPawn));
        }

        private static void Flee(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn == null) return;
            var targetPawn = map.mapPawns.AllPawnsSpawned.FirstOrDefault(p => p.LabelShortCap == item.target || p.Name?.ToStringShort == item.target);
            if (targetPawn == null) return;
            if (pawn.drafter != null) pawn.drafter.Drafted = true;
            pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Flee, targetPawn));
        }

        private static void HoldPosition(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn == null) return;
            if (pawn.drafter != null) pawn.drafter.Drafted = true;
            pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Wait_Combat, pawn.Position));
        }

        private static void AttackPos(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null || item.target_pos == null || item.target_pos.Length < 3) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn == null) return;
            var cell = new IntVec3(item.target_pos[0], item.target_pos[1], item.target_pos[2]);
            if (!cell.InBounds(map)) return;
            if (pawn.drafter != null) pawn.drafter.Drafted = true;
            pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.AttackStatic, cell));
        }

        private static void AttackThing(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn == null) return;
            var thing = map.listerThings.AllThings.FirstOrDefault(t => t.LabelShortCap == item.target || t.def.defName == item.target || t.def.label == item.target);
            if (thing == null) return;
            if (pawn.drafter != null) pawn.drafter.Drafted = true;
            pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.AttackStatic, thing));
        }

        private static void Retreat(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null || item.target_pos == null || item.target_pos.Length < 3) return;
            var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == item.pawn || p.LabelShortCap == item.pawn);
            if (pawn == null) return;
            var cell = new IntVec3(item.target_pos[0], item.target_pos[1], item.target_pos[2]);
            if (!cell.InBounds(map)) return;
            if (pawn.drafter != null) pawn.drafter.Drafted = true;
            pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Goto, cell));
        }

        private static void GroupDraft(ActionItem item, bool drafted)
        {
            var map = Find.CurrentMap;
            if (map == null || item.pawns == null) return;
            foreach (var name in item.pawns)
            {
                var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == name || p.LabelShortCap == name);
                if (pawn?.drafter == null) continue;
                pawn.drafter.Drafted = drafted;
            }
        }

        private static void GroupMove(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null || item.pawns == null || item.target_pos == null || item.target_pos.Length < 3) return;
            var cell = new IntVec3(item.target_pos[0], item.target_pos[1], item.target_pos[2]);
            if (!cell.InBounds(map)) return;
            foreach (var name in item.pawns)
            {
                var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == name || p.LabelShortCap == name);
                if (pawn == null) continue;
                if (pawn.drafter != null) pawn.drafter.Drafted = true;
                pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Goto, cell));
            }
        }

        private static void GroupAttack(ActionItem item)
        {
            var map = Find.CurrentMap;
            if (map == null || item.pawns == null) return;
            var targetPawn = map.mapPawns.AllPawnsSpawned.FirstOrDefault(p => p.LabelShortCap == item.target || p.Name?.ToStringShort == item.target);
            if (targetPawn == null) return;
            foreach (var name in item.pawns)
            {
                var pawn = map.mapPawns.FreeColonists.FirstOrDefault(p => p.Name?.ToStringShort == name || p.LabelShortCap == name);
                if (pawn == null) continue;
                if (pawn.drafter != null) pawn.drafter.Drafted = true;
                pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.AttackMelee, targetPawn));
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
