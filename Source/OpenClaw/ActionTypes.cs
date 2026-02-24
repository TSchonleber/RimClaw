using System.Collections.Generic;

namespace OpenClaw
{
    public class ActionRequest
    {
        public List<ActionItem> actions;
    }

    public class ActionItem
    {
        public string action;
        public string pawn;
        public string work;
        public int level;
        public int[] pos;
        public string thing;
        public string zone;
        public string table;
        public string recipe;
        public int count;
    }

    public class ActionResult
    {
        public string status;
        public List<string> errors;
    }
}
