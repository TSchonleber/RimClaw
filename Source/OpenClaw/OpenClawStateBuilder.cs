using System.Text;
using Verse;

namespace OpenClaw
{
    public static class OpenClawStateBuilder
    {
        public static string Build()
        {
            // TODO: Replace with real colony state snapshot
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"tick\":").Append(Find.TickManager.TicksGame).Append(',');
            sb.Append("\"summary\":\"placeholder state\"");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
