using System.Collections.Generic;
using RimWorld;
using Verse;

namespace OpenClaw
{
    public static class ActionValidator
    {
        public static List<string> Validate(ActionItem item)
        {
            var errors = new List<string>();
            if (item == null)
            {
                errors.Add("action missing");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(item.action))
            {
                errors.Add("action type missing");
                return errors;
            }

            if (item.action == "set_priority")
            {
                if (string.IsNullOrWhiteSpace(item.pawn)) errors.Add("pawn missing");
                if (string.IsNullOrWhiteSpace(item.work)) errors.Add("work missing");
                if (item.level < 1 || item.level > 4) errors.Add("level out of range");
            }

            if (item.action == "queue_build")
            {
                if (string.IsNullOrWhiteSpace(item.thing)) errors.Add("thing missing");
                if (item.pos == null || item.pos.Length < 3) errors.Add("pos missing");
            }

            if (item.action == "draft" || item.action == "undraft")
            {
                if (string.IsNullOrWhiteSpace(item.pawn)) errors.Add("pawn missing");
            }

            if (item.action == "set_zone")
            {
                if (string.IsNullOrWhiteSpace(item.pawn)) errors.Add("pawn missing");
                if (string.IsNullOrWhiteSpace(item.zone)) errors.Add("zone missing");
            }

            if (item.action == "create_bill")
            {
                if (string.IsNullOrWhiteSpace(item.table)) errors.Add("table missing");
                if (string.IsNullOrWhiteSpace(item.recipe)) errors.Add("recipe missing");
            }

            if (item.action == "set_bill_count")
            {
                if (string.IsNullOrWhiteSpace(item.table)) errors.Add("table missing");
                if (string.IsNullOrWhiteSpace(item.recipe)) errors.Add("recipe missing");
                if (item.count < 0) errors.Add("count invalid");
            }

            return errors;
        }
    }
}
