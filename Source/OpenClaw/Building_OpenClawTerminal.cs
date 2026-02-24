using Verse;

namespace OpenClaw
{
    public class Building_OpenClawTerminal : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override string GetInspectString()
        {
            return "OpenClaw terminal online.";
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!Find.WindowStack.IsOpen<OpenClawTerminalWindow>())
            {
                Find.WindowStack.Add(new OpenClawTerminalWindow());
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
