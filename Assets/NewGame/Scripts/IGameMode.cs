using UnityEngine;

namespace NewGame
{
    public interface IGameMode
    {
        // GameModeType ModeType { get; }

        void OnGameStart(LevelController level);
        void OnBottomBarFull();
        bool CanReturnItemToBoard();

        void OnTimeRunout();
    }


    public abstract class GameModeBase : IGameMode
    {
        public abstract eGameMode ModeType { get; }

        public abstract bool CanReturnItemToBoard();

        public virtual void OnGameStart(LevelController level)
        {
            Debug.Log("OnGameStart");
        }

        public virtual void OnBottomBarFull()
        {
            Debug.Log("OnBottomBarFull");
        }

        public virtual void OnTimeRunout() {}
    }

    public class GameMode_Normal : GameModeBase
    {
        public override eGameMode ModeType => eGameMode.Normal;

        public override bool CanReturnItemToBoard() => false;

        public override void OnBottomBarFull()
        {
            base.OnBottomBarFull();

            GameManager.Instance.EndGame(false);
        }
    }


    public class GameMode_Timer : GameModeBase
    {
        public override eGameMode ModeType => eGameMode.Timer;

        public override bool CanReturnItemToBoard() => true;

        public override void OnTimeRunout() => GameManager.Instance.EndGame(false);
    }
}
