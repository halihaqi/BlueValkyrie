using Game.Model;

namespace Game.Managers
{
    public class PlayerMgr : Singleton<PlayerMgr>
    {
        private PlayerInfo _nowPlayer;

        public const string PLAYER_DATA_KEY = "Player";

        public PlayerInfo NowPlayer
        {
            get => _nowPlayer;
            set => _nowPlayer = value;
        }
    }
}