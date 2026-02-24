using System;
using Verse;

namespace OpenClaw
{
    public class GameComponent_OpenClaw : GameComponent
    {
        public static GameComponent_OpenClaw Instance;

        public GameComponent_OpenClaw(Game game)
        {
            Instance = this;
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            OpenClawHttpServer.Start();
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
        }

        public override void GameComponentOnGUI()
        {
            base.GameComponentOnGUI();
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
