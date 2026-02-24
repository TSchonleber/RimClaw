using UnityEngine;
using Verse;

namespace OpenClaw
{
    public class OpenClawTerminalWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(600f, 420f);

        public override void DoWindowContents(Rect inRect)
        {
            Widgets.Label(new Rect(0, 0, inRect.width, 30f), "OpenClaw Terminal");
            Widgets.Label(new Rect(0, 40f, inRect.width, 30f), "Last actions:");
            Widgets.TextArea(new Rect(0, 70f, inRect.width, inRect.height - 90f), OpenClawTerminalState.LastLog ?? "No actions yet.");
        }
    }
}
