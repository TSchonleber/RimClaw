using System.Collections.Generic;
using System.Text;
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
                // TODO: apply actions
            }

            return Serialize(result);
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
