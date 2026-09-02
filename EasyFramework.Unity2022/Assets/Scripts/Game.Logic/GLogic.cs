using EasyFramework;

namespace Game.Logic
{
    public static class GLogic
    {
        public static FSM FSM = new FSM(typeof(GameStateAttribute));

        public static void Enter()
        {
            FDebug.Log($"GLogic - Enter");
            
            FSM.Enter<GameInitState>();
        }
    }
    
    static class HotUpdate
    {
        public static void Enter()
        {
            GLogic.Enter();
        }
    }
}