
namespace EasyFramework.Samples
{
    public static class FTest
    {
        public static readonly FSM<GameStateAttribute> FSM = new ();
        
        public static void Enter()
        {
            FDebug.Log("FTest Enter");
            
            FSM.Enter<GameInitState>();
        }
    }

    public static class HotUpdate
    {
        private static void Enter() => FTest.Enter();
    }
}