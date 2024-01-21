using _Main.Scripts.Utilities.Singletons;

namespace _Main.Scripts.Managers
{
    public enum GameState
    {
        GamePlay,
        Win,
        Lose,
        Pause
    }

    public class GameManager : Singleton<GameManager>
    {
        private GameState _currentState;

        public GameState CurrentState => _currentState;

        public void SetNewState(GameState state)
        {
            _currentState = state;
        }
    }
}