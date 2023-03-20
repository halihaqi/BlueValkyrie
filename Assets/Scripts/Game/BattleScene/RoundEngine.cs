using System;
using Hali_Framework;

namespace Game.BattleScene
{
    public class RoundEngine
    {
        private bool _isEnemy;
        private int _maxRound;
        private int _curRound;
        private int _logicAp;
        private int _enemyLastAp = 0;
        private int _studentLastAp = 0;
        private int _curAp;
        private bool _isHalfOver;
        private bool _isOver;

        public bool IsEnemy => _isEnemy;

        public int CurRound => _curRound;

        public int CurAp => _curAp;

        public int MaxRound => _maxRound;

        public bool IsHalfOver => _isHalfOver;

        public bool IsOver => _isOver;

        public void Init(bool isEnemy, int maxRound, int logicAp)
        {
            _isEnemy = isEnemy;
            _curRound = _maxRound = maxRound;
            _curAp = _logicAp = logicAp;
            _isHalfOver = false;
        }

        public void Run()
        {
            --_curAp;
            if (_curAp <= 0)
            {
                _isEnemy = !_isEnemy;
                _curAp = Math.Min(_maxRound, _logicAp + (_isEnemy ? _enemyLastAp : _studentLastAp));
                if (_isHalfOver)
                {
                    _studentLastAp = 0;
                    _enemyLastAp = 0;
                    --_curRound;
                    if (_curRound <= 0)
                    {
                        _isOver = true;
                        return;
                    }
                }

                EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_HALF_ROUND_OVER);
                _isHalfOver = !_isHalfOver;
                if(!_isHalfOver)
                    EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROUND_OVER);
            }
            EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROUND_RUN);
        }

        public void HalfOver()
        {
            if (_isEnemy)
                _enemyLastAp = _curAp;
            else
                _studentLastAp = _curAp;
            _curAp = 0;
            Run();
        }

        public void ShutDown()
        {
            _curAp = 0;
            _curRound = 0;
            _isHalfOver = true;
            _isOver = true;
        }
    }
}