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
        public bool paused;
        public string repeat_mode;
        public int target;
        public int min_skill;
        public int max_skill;
        public string schedule;
        public string[] ingredients;
        public int[] target_pos;
        public string target;
        public string[] pawns;
    }

    public class ActionResult
    {
        public string status;
        public List<ActionError> errors;
    }

    public class ActionError
    {
        public int index;
        public string code;
        public string message;
    }
}
